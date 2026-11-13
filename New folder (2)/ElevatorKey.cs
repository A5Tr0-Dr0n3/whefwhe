using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class ElevatorKey : MonoBehaviour
{
	public Transform elevatorDock;

	public Transform keyHole;

	public Transform keyWaitPos;

	private Animator keyAnimator;

	public string animatorTrigger = "Unlock";

	public AnimationClip unlockClip;

	public GvrAudioSource angelicMusic;

	public GameObject dockedGlowCard;

	private SafeCoroutine animationThread;

	private void Start()
	{
		keyAnimator = GetComponent<Animator>();
		keyAnimator.enabled = false;
	}

	public void animateToElevator()
	{
		if (animationThread == null || !animationThread.IsRunning)
		{
			animationThread = GlobalCoroutineContainer.AddThread(animateToElevatorThread(), this);
		}
	}

	private IEnumerator animateToElevatorThread()
	{
		transform.SetParent(elevatorDock, worldPositionStays: true);
		while (Vector3.SqrMagnitude(transform.position - elevatorDock.position) > 0.01f || Quaternion.Angle(transform.rotation, elevatorDock.rotation) > 0.01f)
		{
			transform.position = Vector3.Lerp(transform.position, elevatorDock.position, 1f * Time.deltaTime);
			transform.rotation = Quaternion.Slerp(transform.rotation, elevatorDock.rotation, 1f * Time.deltaTime);
			yield return null;
		}
		keyAnimator.enabled = true;
		yield return null;
	}

	public void animateUnlock(GameObject lockedSafe)
	{
		if (animationThread == null || !animationThread.IsRunning)
		{
			animationThread = GlobalCoroutineContainer.AddThread(animateUnlockThread(lockedSafe), this);
		}
	}

	private IEnumerator animateUnlockThread(GameObject lockedSafe)
	{
		if (!keyAnimator.enabled)
		{
			keyAnimator.enabled = true;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		keyAnimator.SetTrigger(animatorTrigger);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		while (keyAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "ElevatorKeyOpen")
		{
			angelicMusic.volume = Mathf.Clamp(Mathf.Lerp(angelicMusic.volume, 0f, 1.2f * Time.deltaTime), 0f, 0.99f);
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		keyAnimator.enabled = false;
		float lerpCount = 0f;
		float lerpTime = 1f;
		Quaternion startRot = lockedSafe.transform.localRotation;
		transform.parent = lockedSafe.transform;
		while (lerpCount < lerpTime)
		{
			lerpCount += Time.deltaTime;
			float lerp = lerpCount / lerpTime;
			lerp = Mathf.Clamp01(lerp * lerp * (3f - 2f * lerp));
			lockedSafe.transform.localRotation = Quaternion.Euler(0f, lerp * -180f, 0f);
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
	}
}
