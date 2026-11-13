using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class DollhouseObject : ObjectBase
{
	[Header("Tweakables")]
	[SerializeField]
	protected float minLeapvalue = 260f;

	[SerializeField]
	protected float maxLeapValue = 390f;

	public float launchPower = 0.75f;

	[SerializeField]
	private FurnitureType mFurnitureType;

	[SerializeField]
	private AnimationCurve bobCurve;

	[Header("Scene References")]
	[SerializeField]
	protected TransformTargetRocket mRocket;

	[SerializeField]
	protected Transform target;

	[SerializeField]
	protected Transform homeTransform;

	[SerializeField]
	protected JobTina sceneRef;

	public DollhouseObjectState currentState;

	private float bounceDistance;

	protected Vector3 dirToTarget = Vector3.zero;

	protected float distanceMod = 1f;

	protected float hopTimer;

	protected float transitionT = 1f;

	private float lateralSpeed = 45f;

	private float verticalSpeedMin = 65f;

	private float verticalSpeedMax = 95f;

	internal Vector3 holdPos = default;

	protected Quaternion holdRot = default;

	protected bool firstTableJump = true;

	protected Quaternion startRotation;

	protected float jumpHeightCutoff = 0.5f;

	private float timeStuck;

	private Vector3 posLastFrame;

	private bool hasPlayedInterrupt;

	internal SafeCoroutine delayedSwitchThread;

	protected float getUpTimer;

	protected float startRocketForce;

	protected Transform startParent;

	private bool grabbedByTina;

	public float bobRate;

	protected bool settingStateOnDelay;

	protected float bobT;

	protected int bobDir = 1;

	protected float talkT;

	protected int talkDir = 1;

	protected override void Awake()
	{
		base.Awake();
		init();
	}

	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		if (currentState == DollhouseObjectState.jumpToTable || currentState == DollhouseObjectState.onGround)
		{
			rb.velocity = Vector3.zero;
		}
		if (mFurnitureType != FurnitureType.notFurniture && currentState != DollhouseObjectState.onGround && currentState != DollhouseObjectState.jumpToTable)
		{
			playGrabInterrupt();
		}
		transform.SetParent(sceneRef.holdRoot, worldPositionStays: true);
		if (currentState == DollhouseObjectState.scriptedSequence)
		{
			SetState(DollhouseObjectState.inactive);
		}
		mRocket.SetRocketDisabled(s: true);
		rb.isKinematic = false;
	}

	public override void OnDrop()
	{
		base.OnDrop();
		transform.SetParent(startParent, worldPositionStays: true);
		if (!sceneRef.IsOnTable(transform.position, gameObject))
		{
			SetState(DollhouseObjectState.onGround);
		}
	}

	protected virtual void FixedUpdate()
	{
		tickInactiveTimer();
		processState();
	}

	public void SetState(DollhouseObjectState s)
	{
		if (currentState != DollhouseObjectState.jumpToTable && rb != null)
		{
			rb.velocity = Vector3.zero;
		}
		holdPos = transform.position;
		holdRot = transform.rotation;
		transitionT = 0f;
		if (mRocket != null)
		{
			mRocket.SetRocketDisabled(s: true);
			mRocket.SetMaxForce(startRocketForce);
		}
		if (s == DollhouseObjectState.gettingUp)
		{
			target.position = transform.position;
			target.rotation = startRotation;
			mRocket.SetMaxForce(0f);
			getUpTimer = 0f;
			rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
		}
		if (s == DollhouseObjectState.home)
		{
			holdPos = transform.position;
			holdRot = transform.rotation;
		}
		if (delayedSwitchThread != null)
		{
			delayedSwitchThread.Stop();
			delayedSwitchThread = null;
		}
		currentState = s;
		firstTableJump = true;
	}

	public void SetStateOnDelay(DollhouseObjectState s, float t)
	{
		if (delayedSwitchThread == null)
		{
			delayedSwitchThread = GlobalCoroutineContainer.AddThread(SetStateOnDelayCoroutine(s, t), this);
		}
	}

	private IEnumerator SetStateOnDelayCoroutine(DollhouseObjectState s, float t)
	{
		settingStateOnDelay = true;
		SetState(DollhouseObjectState.inactive);
		while (isGrabbed)
		{
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(t, this);
		SetState(s);
		delayedSwitchThread = null;
		settingStateOnDelay = false;
	}

	protected virtual void processState()
	{
		if (sceneRef != null && sceneRef.ended)
		{
			enabled = false;
		}
		else
		{
			if (isGrabbed || grabbedByTina)
			{
				return;
			}
			if (currentState == DollhouseObjectState.onGround || currentState == DollhouseObjectState.jumpToTable || isGrabbed)
			{
				if (transform.parent != sceneRef.holdRoot)
				{
					transform.SetParent(sceneRef.holdRoot, worldPositionStays: true);
				}
			}
			else if (!isGrabbed && transform.parent != startParent)
			{
				transform.SetParent(startParent, worldPositionStays: true);
			}
			switch (currentState)
			{
			case DollhouseObjectState.home:
				if (transitionT <= 1f)
				{
					transform.position = Vector3.Lerp(holdPos, homeTransform.position, transitionT);
					transform.rotation = Quaternion.Lerp(holdRot, homeTransform.rotation, transitionT);
					transitionT += Time.deltaTime * 3f;
				}
				if (!sceneRef.IsOnTable(transform.position, gameObject))
				{
					SetState(DollhouseObjectState.onGround);
				}
				else if (Vector3.Distance(transform.position, homeTransform.position) > 0.15f && !isGrabbed)
				{
					SetState(DollhouseObjectState.gettingUp);
				}
				break;
			case DollhouseObjectState.onGround:
				if (sceneRef.IsOnTable(transform.position, gameObject))
				{
					SetStateOnDelay(DollhouseObjectState.gettingUp, 2f);
					break;
				}
				mRocket.SetRocketDisabled(s: true);
				if (sceneRef != null && sceneRef.table != null && !isGrabbed && rb.velocity.sqrMagnitude < 0.05f)
				{
					target.position = sceneRef.table.position;
					processHop();
				}
				break;
			case DollhouseObjectState.jumpToTable:
				if (sceneRef.IsOnTable(transform.position, gameObject))
				{
					SetStateOnDelay(DollhouseObjectState.gettingUp, 2f);
					break;
				}
				mRocket.SetRocketDisabled(s: true);
				if (rb.velocity.sqrMagnitude < 0.005f)
				{
					hopTimer += Time.deltaTime;
				}
				if (hopTimer > 1f)
				{
					if (firstTableJump)
					{
						hopTimer = 0.8f;
					}
					else
					{
						hopTimer = 0f;
					}
					tryLeap();
				}
				break;
			case DollhouseObjectState.gettingUp:
				if (!sceneRef.IsOnTable(transform.position, gameObject))
				{
					SetState(DollhouseObjectState.onGround);
					break;
				}
				mRocket.SetRocketDisabled(s: false);
				mRocket.SetTarget(target);
				getUpTimer += Time.deltaTime;
				if ((double)rb.velocity.sqrMagnitude < 0.005 && getUpTimer > 1.2f)
				{
					mRocket.SetMaxForce(startRocketForce);
					SetState(DollhouseObjectState.onTable);
				}
				break;
			case DollhouseObjectState.onTable:
				if (!sceneRef.IsOnTable(transform.position, gameObject))
				{
					SetState(DollhouseObjectState.onGround);
				}
				else if ((double)Vector3.Distance(transform.position, homeTransform.position) < 0.1)
				{
					SetState(DollhouseObjectState.home);
					rb.isKinematic = false;
				}
				break;
			case DollhouseObjectState.inDollhouse:
			case DollhouseObjectState.toHome:
			case DollhouseObjectState.inactive:
				break;
			}
		}
	}

	private void playGrabInterrupt()
	{
		if (!hasPlayedInterrupt || mFurnitureType == FurnitureType.generic)
		{
			sceneRef.PlayFurnitureInterruption(mFurnitureType);
			if (sceneRef.currentState > TinaState.SelectCharacter && sceneRef.reactThread == null)
			{
				hasPlayedInterrupt = true;
			}
		}
	}

	public void SetParentToHoldRoot()
	{
		transform.SetParent(sceneRef.holdRoot, worldPositionStays: true);
	}

	public void ResetParent()
	{
		transform.SetParent(startParent, worldPositionStays: true);
	}

	protected virtual void init()
	{
		if (mRocket != null)
		{
			mRocket.SetRocketDisabled(s: true);
			startRocketForce = mRocket.GetMaxForce();
		}
		if (homeTransform == null)
		{
			SafeDebug.LogWarning("Youre missing a home transform on: " + name);
		}
		startRotation = transform.rotation;
		startParent = transform.parent;
	}

	protected virtual void processHop()
	{
		hopTimer += Time.deltaTime;
		if (hopTimer >= 0.25f)
		{
			bounceDistance = DistanceSquared(target.position, transform.position);
			if ((double)bounceDistance <= 0.1225)
			{
				GetComponent<Rigidbody>().velocity = Vector3.zero;
				transitionT = 0f;
				holdPos = transform.position;
				target.position = transform.position;
				if (delayedSwitchThread == null)
				{
					SetState(DollhouseObjectState.jumpToTable);
				}
			}
			else
			{
				if (bounceDistance > 30f)
				{
					distanceMod = bounceDistance / 10f;
				}
				else
				{
					distanceMod = 1f;
				}
				dirToTarget = (target.position - transform.position).normalized;
				rb.AddForce(Vector3.up * Random.Range(verticalSpeedMin, verticalSpeedMax) * launchPower + dirToTarget * lateralSpeed * launchPower * distanceMod);
				hopTimer = 0f;
			}
		}
		else if ((double)DistanceSquared(target.position, transform.position) <= 1.59)
		{
			mRocket.SetRocketDisabled(s: true);
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			transitionT = 0f;
			hopTimer = 0f;
			holdPos = transform.position;
			target.position = transform.position;
			if (delayedSwitchThread == null)
			{
				SetState(DollhouseObjectState.jumpToTable);
			}
		}
	}

	protected virtual void tryLeap()
	{
		rb.velocity = Vector3.zero;
		bounceDistance = DistanceSquared(target.position, transform.position);
		if (firstTableJump)
		{
			if (sceneRef != null && target != null && sceneRef.table != null)
			{
				target.position = sceneRef.table.position;
			}
			distanceMod = 1f;
			dirToTarget = Vector3.up;
			rb.AddForce(Vector3.up * Random.Range(verticalSpeedMin, verticalSpeedMax) * launchPower / 100f + dirToTarget * 1f * launchPower * distanceMod, ForceMode.Impulse);
			firstTableJump = false;
		}
		else if ((double)bounceDistance <= 0.9)
		{
			if (sceneRef != null && sceneRef.table != null)
			{
				target.position = sceneRef.table.position;
			}
			dirToTarget = (transform.position - target.position).normalized;
			distanceMod = 2.5f;
			rb.AddForce(Vector3.up * (Random.Range(minLeapvalue, maxLeapValue) * Mathf.Clamp(rb.mass, 0.5f, 1f)) * launchPower / 55f + dirToTarget * 0.35f * launchPower * distanceMod, ForceMode.Impulse);
		}
		else
		{
			if (sceneRef != null && sceneRef.table != null)
			{
				target.position = sceneRef.table.position;
			}
			distanceMod = 3.4f;
			dirToTarget = (target.position - transform.position).normalized;
			rb.AddForce(Vector3.up * (Random.Range(minLeapvalue, maxLeapValue) * Mathf.Clamp(rb.mass, 0.5f, 1f)) * launchPower / 55f + dirToTarget * 0.35f * launchPower * distanceMod, ForceMode.Impulse);
		}
	}

	protected void tickInactiveTimer()
	{
		if (currentState != DollhouseObjectState.onTable)
		{
			return;
		}
		if (DistanceSquared(posLastFrame, transform.position) <= 0.000625f)
		{
			timeStuck += Time.deltaTime;
			if (timeStuck > 3.5f)
			{
				mRocket.SetRocketDisabled(s: true);
				rb.velocity = Vector3.zero;
				target.position = sceneRef.table.position;
				distanceMod = 30f;
				dirToTarget = transform.up;
				timeStuck = 0f;
			}
		}
		else
		{
			timeStuck = 0f;
		}
		posLastFrame = transform.position;
	}

	protected float DistanceSquared(Vector3 a, Vector3 b)
	{
		return (a - b).sqrMagnitude;
	}

	public void SetPosFromNavAgent(Vector3 pos, Quaternion rot, bool bob = true)
	{
		if (bob)
		{
			stepBobT();
			Vector3 vector = Vector3.up * bobCurve.Evaluate(bobT);
			transform.localPosition = pos + vector;
			transform.localRotation = rot;
		}
		else
		{
			transform.localPosition = pos;
			transform.localRotation = rot;
		}
	}

	protected void stepBobT()
	{
		if (bobDir == 1)
		{
			bobT += Time.deltaTime / bobRate;
		}
		if (bobDir == -1)
		{
			bobT -= Time.deltaTime / bobRate;
		}
		if (bobT >= 1f)
		{
			bobDir = -1;
		}
		if (bobT <= 0f)
		{
			bobDir = 1;
		}
	}

	protected void stepTalkT()
	{
		if (talkDir == 1)
		{
			talkT += Time.deltaTime / 0.1f;
		}
		if (talkDir == -1)
		{
			talkT -= Time.deltaTime / 0.1f;
		}
		if (talkT >= 1f)
		{
			talkDir = -1;
		}
		if (talkT <= 0f)
		{
			talkDir = 1;
		}
	}

	public void AttachToTina(Transform mouth)
	{
		grabbedByTina = true;
		transform.SetParent(mouth);
		rb.isKinematic = true;
		transform.localPosition = Vector3.zero;
	}

	public void DetatchFromTina()
	{
		grabbedByTina = false;
		transform.SetParent(startParent);
		rb.isKinematic = false;
		SetState(DollhouseObjectState.inactive);
		SetStateOnDelay(DollhouseObjectState.gettingUp, 1.5f);
	}
}
