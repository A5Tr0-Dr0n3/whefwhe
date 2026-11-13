using System;
using UnityEngine;

public class ElizaRotationFollow : MonoBehaviour
{
	public VVR_Eliza sceneReference;

	private ObjectBase elizaObject;

	private Vector3 lookPosition;

	private Vector3 lookRotation;

	private Vector3 extraTurn = new Vector3(0f, 180f, 0f);

	private float lookStartY;

	public float sideEyeAngleMax = 45f;

	public float sideEyeXRange = 0.2f;

	public float lookEasing = 0.1f;

	public float slowLookEasing = 0.04f;

	public float eyeEasing = 0.5f;

	public GameObject eyeTarget;

	private Vector3 eyeStartPosition;

	private Vector3 eyeLookPosition;

	private float eyeXOffset;

	private float lookTime;

	private float lookDuration;

	private float t;

	private Quaternion startLookRotation;

	private Quaternion targetRotation;

	private bool doingLook;

	private bool turnOnLookAtEnd;

	private bool useSlowLook;

	private bool doEyeLookOnly;

	private bool beingGrabbed;

	private ElizaTeleportEffect grabEffect;

	private float grabEffectTime = 0.4f;

	private Transform _lookTarget;

	private bool lookingAway;

	public float lookAwayAngle = -90f;

	private Vector3 lookAwayVector;

	[HideInInspector]
	public bool lookEnabled;

	public ObjectBase elizaObjReference => elizaObject;

	public bool doingRise => doingLook;

	public Transform lookTarget => _lookTarget;

	public void SetElizaObject(ObjectBase s)
	{
		elizaObject = s;
	}

	private void Start()
	{
		eyeStartPosition = eyeTarget.transform.localPosition;
		elizaObject = GetComponentInParent<ObjectBase>();
		grabEffect = GetComponent<ElizaTeleportEffect>();
		lookAwayVector = new Vector3(0f, lookAwayAngle, 0f);
	}

	private void OnEnable()
	{
		Teleporter.OnFinishTeleportEvent += playerTeleported;
	}

	private void OnDisable()
	{
		Teleporter.OnFinishTeleportEvent -= playerTeleported;
	}

	private void Update()
	{
		if (elizaObject == null)
		{
			elizaObject = GetComponentInParent<ObjectBase>();
		}
		if (elizaObject.isGrabbed && !beingGrabbed)
		{
			beingGrabbed = true;
			grabEffect.StartAmbientBlocky(grabEffectTime);
			sceneReference.PlayElizaGrabFeedback();
			sceneReference.StartGrabWorldGlitch();
		}
		if (beingGrabbed && !elizaObject.isGrabbed)
		{
			beingGrabbed = false;
			grabEffect.CompleteAmbientBlocky(grabEffectTime);
			sceneReference.EndElizaGrabFeedback();
			sceneReference.EndGrabWorldGlitch();
		}
		if (lookEnabled && !elizaObject.isGrabbed)
		{
			lookPosition = _lookTarget.position;
			lookPosition.y = transform.position.y;
			lookStartY = transform.rotation.eulerAngles.y;
			transform.LookAt(lookPosition);
			if (lookingAway)
			{
				transform.Rotate(extraTurn + lookAwayVector);
			}
			else
			{
				transform.Rotate(extraTurn);
			}
			lookRotation = transform.rotation.eulerAngles;
			eyeLookPosition = eyeStartPosition;
			if (doEyeLookOnly)
			{
				if (lookStartY - lookRotation.y > 180f)
				{
					lookStartY -= 360f;
				}
				else if (lookRotation.y - lookStartY > 180f)
				{
					lookStartY += 360f;
				}
				eyeXOffset = (1f - eyeEasing) * eyeXOffset + eyeEasing * (Mathf.Abs(lookRotation.y - lookStartY) / sideEyeAngleMax * sideEyeXRange);
				if (eyeXOffset > sideEyeXRange)
				{
					eyeXOffset = sideEyeXRange;
				}
				if (eyeXOffset < 0f - sideEyeXRange)
				{
					eyeXOffset = 0f - sideEyeXRange;
				}
			}
			else
			{
				eyeXOffset = 0f;
			}
			if (lookRotation.y > lookStartY)
			{
				eyeLookPosition.x -= eyeXOffset;
			}
			else
			{
				eyeLookPosition.x += eyeXOffset;
			}
			eyeTarget.transform.localPosition = eyeLookPosition;
			if (doEyeLookOnly)
			{
				lookRotation.y = lookStartY;
			}
			else
			{
				if (lookStartY - lookRotation.y > 180f)
				{
					lookStartY -= 360f;
				}
				else if (lookRotation.y - lookStartY > 180f)
				{
					lookStartY += 360f;
				}
				if (!useSlowLook)
				{
					lookRotation.y = (1f - lookEasing) * lookStartY + lookEasing * lookRotation.y;
				}
				else
				{
					lookRotation.y = (1f - slowLookEasing) * lookStartY + slowLookEasing * lookRotation.y;
				}
			}
			transform.rotation = Quaternion.Euler(lookRotation);
		}
		else if (doingLook)
		{
			handleTurn();
		}
	}

	private void handleTurn()
	{
		if (lookTime > lookDuration)
		{
			doingLook = false;
			if (turnOnLookAtEnd)
			{
				turnOnLookAtEnd = false;
				lookEnabled = true;
				ReevaluateTarget(doAnyway: true);
			}
		}
		else
		{
			lookTime += Time.deltaTime;
			t = lookTime / lookDuration;
			t = Mathf.Sin(t * (float)Math.PI * 0.5f);
			transform.rotation = Quaternion.Lerp(startLookRotation, targetRotation, t);
		}
	}

	private void SetGrab()
	{
		beingGrabbed = true;
	}

	private void playerTeleported(Vector3 startpos, Vector3 endpos)
	{
		ReevaluateTarget();
	}

	public void ReevaluateTarget(bool doAnyway = false)
	{
		if (!sceneReference.elizaIsOn && !doAnyway)
		{
			return;
		}
		if (_lookTarget == null)
		{
			_lookTarget = EagerMaster.instance.mainCamera.transform;
		}
		lookPosition = _lookTarget.position;
		lookPosition.y = transform.position.y;
		lookStartY = transform.rotation.eulerAngles.y;
		transform.LookAt(lookPosition);
		if (lookingAway)
		{
			transform.Rotate(extraTurn + lookAwayVector);
		}
		else
		{
			transform.Rotate(extraTurn);
		}
		lookRotation = transform.rotation.eulerAngles;
		if (Mathf.Abs(lookRotation.y - lookStartY) < sideEyeAngleMax)
		{
			doEyeLookOnly = true;
		}
		else
		{
			doEyeLookOnly = false;
			if (lookStartY - lookRotation.y > 180f)
			{
				lookStartY -= 360f;
			}
			else if (lookRotation.y - lookStartY > 180f)
			{
				lookStartY += 360f;
			}
		}
		lookRotation.y = lookStartY;
		transform.rotation = Quaternion.Euler(lookRotation);
	}

	public void DoTurn(Quaternion tRotation, float lDuration)
	{
		doingLook = true;
		targetRotation = tRotation;
		startLookRotation = transform.rotation;
		lookDuration = lDuration;
		lookTime = 0f;
	}

	public void RiseTurn(float lDuration)
	{
		if (_lookTarget == null)
		{
			_lookTarget = EagerMaster.instance.mainCamera.transform;
		}
		turnOnLookAtEnd = true;
		Vector3 eulerAngles = transform.rotation.eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		DoTurn(Quaternion.Euler(eulerAngles), lDuration);
	}

	public void SetLookTarget(Transform nTarget)
	{
		_lookTarget = nTarget;
	}

	public void SetLookAway(bool newLookAway)
	{
		lookingAway = newLookAway;
		ReevaluateTarget();
	}

	public void SlowLook(bool newLook)
	{
		useSlowLook = newLook;
	}
}
