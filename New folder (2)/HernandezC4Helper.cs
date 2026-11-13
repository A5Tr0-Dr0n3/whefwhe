using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HernandezC4Helper : MonoBehaviour
{
	[Serializable]
	public class C4Trigger
	{
		public Transform transform;

		public bool armed = true;

		public float radius = 0.5f;

		public bool fired { get; private set; }

		public bool CheckTrigger(Vector3 testPos)
		{
			if (armed && !fired && Vector3.Distance(testPos, transform.position) < radius)
			{
				fired = true;
				return true;
			}
			return false;
		}
	}

	[SerializeField]
	private HernandezTweakablesScriptableObject tweakables;

	[SerializeField]
	private OfficeCurtainController officeCurtains;

	[SerializeField]
	private C4Trigger addressPlayerTrigger;

	[SerializeField]
	private C4Trigger grabSupplementTrigger;

	[SerializeField]
	private C4Trigger openDoorEntranceTrigger;

	[SerializeField]
	private C4Trigger reachedHernandezTrigger;

	[SerializeField]
	private C4Trigger openDoorExitTrigger;

	[SerializeField]
	private C4Trigger closeDoorExitTrigger;

	[SerializeField]
	private C4Trigger postHernandezLoopingTrigger;

	[SerializeField]
	public VVRDialogueSource mDialogueSource;

	[SerializeField]
	private Transform[] walkToPlayerPath;

	[SerializeField]
	private Transform[] walkToHernandezPath;

	[SerializeField]
	private Transform[] postPlayerWarningPath;

	[SerializeField]
	private Transform[] postHernandezLoopingPath;

	[SerializeField]
	private ObjectBase supplementObject;

	[SerializeField]
	private FixedJoint grabHand;

	[SerializeField]
	private HernandezHelperEyes mEyes;

	[SerializeField]
	private SubtitleRenderer hernandezSubtitles;

	[SerializeField]
	private SubtitleRenderer c4Subtitles;

	[SerializeField]
	private float curtainOpenness = 0.35f;

	private HernandezHelperBody mBody;

	private bool addressingPlayer;

	private bool dismissedFromHernandez;

	private bool hernandezIsDead;

	private VVR_Hernandez.C4ArrivesCallback hernandezArrivalCallback;

	private void OnDrawGizmos()
	{
		DrawTrigger(grabSupplementTrigger);
		DrawTrigger(openDoorEntranceTrigger);
		DrawTrigger(reachedHernandezTrigger);
		DrawTrigger(openDoorExitTrigger);
		DrawTrigger(closeDoorExitTrigger);
		DrawTrigger(addressPlayerTrigger);
		DrawTrigger(postHernandezLoopingTrigger);
		if (mBody == null)
		{
			mBody = GetComponent<HernandezHelperBody>();
		}
		if (walkToPlayerPath != null)
		{
			HernandezHelperBody.DrawPath(walkToPlayerPath, loopIn: false, mBody);
		}
		if (postPlayerWarningPath != null)
		{
			HernandezHelperBody.DrawPath(postPlayerWarningPath, loopIn: true, mBody);
		}
		if (walkToHernandezPath != null)
		{
			HernandezHelperBody.DrawPath(walkToHernandezPath, loopIn: false, mBody);
		}
		if (postHernandezLoopingPath != null)
		{
			HernandezHelperBody.DrawPath(postHernandezLoopingPath, mBody);
		}
	}

	public void DrawTrigger(C4Trigger trigger)
	{
		if (trigger.transform != null)
		{
			Gizmos.color = ((!trigger.fired) ? new Color(0.4f, 0.8f, 0.2f) : new Color(0.8f, 0.4f, 0.2f));
			Gizmos.DrawWireSphere(trigger.transform.position, trigger.radius);
		}
	}

	private void Awake()
	{
		mBody = GetComponent<HernandezHelperBody>();
	}

	private void Start()
	{
		addressPlayerTrigger.transform = EagerMaster.instance.mainCamera.transform;
		List<Transform> list = walkToPlayerPath.ToList();
		list.Add(EagerMaster.instance.mainCamera.transform);
		walkToPlayerPath = list.ToArray();
		if (addressPlayerTrigger.armed)
		{
			mBody.OverridePath(walkToPlayerPath, newLooping: false);
		}
	}

	public void HeadToHernandez(VVR_Hernandez.C4ArrivesCallback callback)
	{
		hernandezArrivalCallback = callback;
		addressPlayerTrigger.armed = false;
		mBody.OverridePath(walkToHernandezPath, newLooping: false);
		openDoorEntranceTrigger.armed = true;
		reachedHernandezTrigger.armed = true;
		openDoorExitTrigger.armed = true;
		closeDoorExitTrigger.armed = true;
		postHernandezLoopingTrigger.armed = true;
	}

	public void DismissFromHernandez()
	{
		dismissedFromHernandez = true;
	}

	public void ForcePostHernandezPath()
	{
		mBody.SetDefaultPath(postHernandezLoopingPath);
		mBody.OverridePath(postHernandezLoopingPath, newLooping: true);
		addressPlayerTrigger.armed = false;
		openDoorEntranceTrigger.armed = false;
		reachedHernandezTrigger.armed = false;
		openDoorExitTrigger.armed = false;
		closeDoorExitTrigger.armed = false;
		postHernandezLoopingTrigger.armed = false;
	}

	public void SetHernandezDeadState()
	{
		hernandezIsDead = true;
	}

	private void Update()
	{
		mBody.shouldMove = !addressingPlayer && (!reachedHernandezTrigger.fired || dismissedFromHernandez);
		manageStateLogic();
	}

	private void manageStateLogic()
	{
		if (addressPlayerTrigger.CheckTrigger(transform.position))
		{
			if (!hernandezIsDead)
			{
				GlobalCoroutineContainer.AddThread(addressPlayerWarning(), this);
			}
			else
			{
				GlobalCoroutineContainer.AddThread(addressPlayerHernandezDead(), this);
			}
		}
		if (grabSupplementTrigger.CheckTrigger(transform.position))
		{
			GlobalCoroutineContainer.AddThread(grabSequence(), this);
		}
		if (openDoorEntranceTrigger.CheckTrigger(transform.position))
		{
			officeCurtains.SetTargetCurtainOpenness(curtainOpenness);
		}
		if (openDoorExitTrigger.CheckTrigger(transform.position))
		{
			officeCurtains.SetTargetCurtainOpenness(curtainOpenness);
		}
		if (closeDoorExitTrigger.CheckTrigger(transform.position))
		{
			officeCurtains.SetTargetCurtainOpenness(0f);
		}
		if (reachedHernandezTrigger.CheckTrigger(transform.position))
		{
			officeCurtains.SetTargetCurtainOpenness(0f);
			if (hernandezArrivalCallback != null)
			{
				hernandezArrivalCallback();
			}
		}
		if (postHernandezLoopingTrigger.CheckTrigger(transform.position))
		{
			mBody.SetDefaultPath(postHernandezLoopingPath);
			mBody.OverridePath(postHernandezLoopingPath, newLooping: true);
			UnityEngine.Object.Destroy(grabHand);
		}
	}

	private IEnumerator grabSequence()
	{
		Vector3 posVel = Vector3.zero;
		Transform originalParent = supplementObject.transform.parent;
		supplementObject.transform.parent = grabHand.transform;
		supplementObject.rb.isKinematic = true;
		while (supplementObject.transform.localPosition.sqrMagnitude > 0.0025000002f)
		{
			supplementObject.transform.localPosition = Vector3.SmoothDamp(supplementObject.transform.localPosition, Vector3.zero, ref posVel, 0.2f);
			supplementObject.transform.localRotation = Quaternion.Slerp(supplementObject.transform.localRotation, Quaternion.identity, Time.deltaTime * 4f);
			yield return null;
		}
		supplementObject.transform.parent = originalParent;
		supplementObject.rb.isKinematic = false;
		grabHand.autoConfigureConnectedAnchor = false;
		grabHand.connectedBody = supplementObject.rb;
		grabHand.connectedAnchor = Vector3.zero;
		grabHand.anchor = Vector3.zero;
	}

	private IEnumerator addressPlayerWarning()
	{
		addressingPlayer = true;
		mBody.SetLookAtTarget(EagerMaster.instance.mainCamera.transform);
		mDialogueSource.PlayClip(tweakables.helperDialogue.mobileHelperWarning);
		c4Subtitles.gameObject.SetActive(value: true);
		hernandezSubtitles.gameObject.SetActive(value: false);
		mEyes.EnableBlink(enable: false);
		mEyes.SetOverrideEye("!", "!");
		mBody.AddBobForce(Vector3.up * 75f);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.1f, this);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.7f, this);
		mEyes.ReleaseOverrideEye();
		mBody.AddBobForce(Vector3.down * 25f);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.1f, this);
		mEyes.SetOverrideEye("O", "O");
		mBody.AddBobForce(Vector3.up * 25f);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.3f, this);
		mEyes.SetOverrideEye("X", "X");
		mBody.AddBobForce(Vector3.down * 25f);
		while (mDialogueSource.isPlaying)
		{
			yield return null;
		}
		c4Subtitles.gameObject.SetActive(value: false);
		hernandezSubtitles.gameObject.SetActive(value: true);
		mEyes.ReleaseOverrideEye();
		mEyes.EnableBlink(enable: true);
		mEyes.ForceBlink();
		mBody.SetLookAtTarget(null);
		addressingPlayer = false;
		mBody.OverridePath(postPlayerWarningPath, newLooping: true);
	}

	private IEnumerator addressPlayerHernandezDead()
	{
		addressingPlayer = true;
		mBody.SetLookAtTarget(EagerMaster.instance.mainCamera.transform);
		mDialogueSource.PlayClip(tweakables.helperDialogue.hernandezIsDeadGreeting);
		c4Subtitles.gameObject.SetActive(value: true);
		if (hernandezSubtitles != null)
		{
			hernandezSubtitles.gameObject.SetActive(value: false);
		}
		mEyes.ReleaseOverrideEye();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.7f, this);
		mEyes.SetOverrideEye(".", ".");
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.2f, this);
		mEyes.ReleaseOverrideEye();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.78f, this);
		mEyes.SetOverrideEye("^", "^");
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.72f, this);
		mEyes.ReleaseOverrideEye();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2.5f, this);
		mEyes.SetOverrideEye("<=", "<=");
		while (mDialogueSource.isPlaying)
		{
			yield return null;
		}
		c4Subtitles.gameObject.SetActive(value: false);
		if (hernandezSubtitles != null)
		{
			hernandezSubtitles.gameObject.SetActive(value: true);
		}
		mEyes.ReleaseOverrideEye();
		mBody.SetLookAtTarget(null);
		addressingPlayer = false;
		mBody.OverridePath(postHernandezLoopingPath, newLooping: true);
		mBody.SetDefaultPath(postHernandezLoopingPath);
	}
}
