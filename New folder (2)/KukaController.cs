using System;
using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class KukaController : MonoBehaviour
{
	[Header("My stuff")]
	[SerializeField]
	internal Animator mAnimator;

	[SerializeField]
	private AnimationClip introClip;

	[SerializeField]
	private AnimationClip chooseDollClip;

	[SerializeField]
	public Transform mMouth;

	[Header("Furniture references")]
	[SerializeField]
	private DollhouseObject crib;

	[SerializeField]
	private DollhouseObject bed;

	[SerializeField]
	private DollhouseObject nightTable;

	[SerializeField]
	private Transform roof;

	[SerializeField]
	private Transform finalRoofPosition;

	[SerializeField]
	private Transform remote;

	[Header("Scene references")]
	[SerializeField]
	private JobTina sceneRef;

	[Header("Talker refs")]
	[SerializeField]
	private SkinnedMeshRenderer talker;

	[SerializeField]
	private AudioSourceLoudness dialogueLoudness;

	[SerializeField]
	private float talkThreshold = 35f;

	[SerializeField]
	private float talkScale = 2500f;

	[SerializeField]
	private float currentTalkValue;

	[Header("Controller Settings")]
	public float minAngle = -10f;

	public float maxAngle = 10f;

	private float jiggleT;

	private Quaternion startRemoteRot = Quaternion.identity;

	private bool hasRemote;

	private Vector3 rotationAxis;

	internal TinaAnimationState mAnimationState;

	internal float normalizedTime;

	internal float heldTime;

	internal float headsetResumeTime;

	internal float introResumeTime;

	private bool playingChooseDollLoop;

	private void Awake()
	{
		jiggleT = 0.5f;
	}

	private void Update()
	{
		talk();
		if ((double)normalizedTime <= 0.99)
		{
			normalizedTime = mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
		}
	}

	private void talk()
	{
		currentTalkValue = Mathf.Lerp(currentTalkValue, dialogueLoudness.lerpLoudness * talkScale, Time.deltaTime);
		Quaternion a = startRemoteRot * Quaternion.AngleAxis(minAngle, rotationAxis);
		Quaternion b = startRemoteRot * Quaternion.AngleAxis(maxAngle, rotationAxis);
		if (sceneRef.tinaDoll == null)
		{
			if (!(dialogueLoudness.lerpLoudness * talkScale > talkThreshold))
			{
				return;
			}
			talker.SetBlendShapeWeight(0, dialogueLoudness.lerpLoudness * talkScale);
			if (hasRemote)
			{
				jiggleT += Time.deltaTime * 3f;
				if (jiggleT >= 1f)
				{
					jiggleT = 0f;
				}
				remote.transform.localRotation = Quaternion.Slerp(a, b, Mathf.Sin(jiggleT * (float)Math.PI * 2f));
			}
		}
		else
		{
			if (!(dialogueLoudness.lerpLoudness * talkScale > talkThreshold) || sceneRef.tinaDoll.IsTalking())
			{
				return;
			}
			talker.SetBlendShapeWeight(0, dialogueLoudness.lerpLoudness * talkScale);
			if (hasRemote)
			{
				jiggleT += Time.deltaTime * 3f;
				if (jiggleT >= 1f)
				{
					jiggleT = 0f;
				}
				remote.transform.localRotation = Quaternion.Slerp(a, b, Mathf.Sin(jiggleT * (float)Math.PI * 2f));
			}
		}
	}

	public SafeCoroutine PlayChooseDollLoop()
	{
		return GlobalCoroutineContainer.AddThread(PlayChooseDollLoopCoroutine(), this);
	}

	public void StopChooseDollLoop()
	{
		playingChooseDollLoop = false;
		normalizedTime = 0f;
	}

	private IEnumerator PlayChooseDollLoopCoroutine()
	{
		yield return null;
	}

	public SafeCoroutine PlayIntroClip()
	{
		return GlobalCoroutineContainer.AddThread(PlayIntroClipCoroutine(), this);
	}

	private IEnumerator PlayIntroClipCoroutine()
	{
		mAnimator.SetBool("DoIntro", value: true);
		mAnimationState = TinaAnimationState.Intro;
		normalizedTime = 0f;
		while (mAnimator.GetBool("DoIntro"))
		{
			yield return null;
		}
		normalizedTime = 0f;
		mAnimationState = TinaAnimationState.PickCharacterLoop;
		yield return null;
	}

	public void StopIntroClip()
	{
		mAnimator.SetBool("DoIntro", value: false);
		normalizedTime = 0f;
	}

	public void PickUpCrib()
	{
		crib.ForceDropByPlayer();
		crib.AttachToTina(mMouth);
	}

	public void DropCrib()
	{
		crib.DetatchFromTina();
	}

	public void PickUpBed()
	{
		bed.ForceDropByPlayer();
		bed.AttachToTina(mMouth);
	}

	public void DropBed()
	{
		bed.DetatchFromTina();
	}

	public void PickUpRoof()
	{
		roof.SetParent(mMouth);
		roof.transform.localPosition = Vector3.zero;
		GlobalCoroutineContainer.AddThread(RoofRotationCoroutine(), this);
	}

	public void DropRoof()
	{
		roof.SetParent(sceneRef.dollhouseRoot);
		GlobalCoroutineContainer.AddThread(SetRoof(0.15f), this);
	}

	private IEnumerator RoofRotationCoroutine()
	{
		while (roof.transform.parent == mMouth)
		{
			roof.transform.rotation = finalRoofPosition.rotation;
			yield return null;
		}
	}

	public void PickUpNightTable()
	{
		nightTable.ForceDropByPlayer();
		nightTable.AttachToTina(mMouth);
	}

	public void DropNightTable()
	{
		nightTable.DetatchFromTina();
	}

	private IEnumerator SetRoof(float duration)
	{
		float timer = 0f;
		Vector3 startPos = roof.position;
		Quaternion startRot = roof.rotation;
		while (timer <= 1f)
		{
			timer += Time.deltaTime / duration;
			roof.transform.position = Vector3.Lerp(startPos, finalRoofPosition.position, timer);
			roof.transform.rotation = Quaternion.Lerp(startRot, finalRoofPosition.rotation, timer);
			yield return null;
		}
	}

	public void StartGrabRemote()
	{
		mAnimator.SetBool("Remote", value: true);
		normalizedTime = 0f;
		mAnimationState = TinaAnimationState.PickUpRemote;
	}

	public void PickUpRemote()
	{
		remote.SetParent(mMouth);
		remote.transform.localRotation = Quaternion.Euler(new Vector3(-90f, 0f, -90f));
		remote.transform.localPosition = Vector3.zero;
		hasRemote = true;
		startRemoteRot = remote.transform.localRotation;
		rotationAxis = remote.right;
	}

	public void EndGrabRemote()
	{
		mAnimator.SetBool("EndRemote", value: true);
		mAnimationState = TinaAnimationState.MainLoop;
		normalizedTime = 0f;
	}

	public void StartPickADoll()
	{
		mAnimator.SetBool("PickADoll", value: true);
		mAnimationState = TinaAnimationState.PickADoll;
		normalizedTime = 0f;
	}

	public void StopPickADoll()
	{
		mAnimator.SetBool("PickADoll", value: false);
		mAnimationState = TinaAnimationState.PickCharacterLoop;
		normalizedTime = 0f;
	}

	public void StartInterruptionAnimation()
	{
		introResumeTime = mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
		if (headsetResumeTime != 0f)
		{
			introResumeTime = headsetResumeTime;
		}
		headsetResumeTime = 0f;
		normalizedTime = 0f;
		mAnimator.SetBool("Interrupt", value: true);
		mAnimator.SetFloat("IntroReturn", introResumeTime);
		mAnimator.SetFloat("SpeedMult", 0f);
		if (sceneRef.currentState == TinaState.SelectCharacter)
		{
			mAnimationState = TinaAnimationState.PickCharacterInterr;
		}
		else if (sceneRef.currentState == TinaState.PlayTime)
		{
			mAnimationState = TinaAnimationState.Interruption;
		}
	}

	public void StopInterruptionAnimation()
	{
		mAnimator.SetBool("Interrupt", value: false);
		normalizedTime = 0f;
		if (sceneRef.currentState == TinaState.SelectCharacter)
		{
			mAnimationState = TinaAnimationState.PickCharacterLoop;
		}
		else if (sceneRef.currentState == TinaState.PlayTime)
		{
			mAnimationState = TinaAnimationState.MainLoop;
		}
		GlobalCoroutineContainer.AddThread(ResumeAfterInterrupt(), this);
	}

	public void StartVacuumAnimation()
	{
		normalizedTime = 0f;
		introResumeTime = mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
		if (headsetResumeTime != 0f)
		{
			introResumeTime = headsetResumeTime;
		}
		headsetResumeTime = 0f;
		mAnimator.SetBool("Vacuum", value: true);
		mAnimator.SetFloat("IntroReturn", introResumeTime);
		mAnimator.SetFloat("SpeedMult", 0f);
		switch (sceneRef.currentState)
		{
		case TinaState.Intro:
			mAnimationState = TinaAnimationState.VacuumInterruptionIntro;
			break;
		case TinaState.SelectCharacter:
			if (mAnimator.GetBool("Remote"))
			{
				mAnimationState = TinaAnimationState.VacuumInterruptionRemote;
			}
			else
			{
				mAnimationState = TinaAnimationState.VacuumInterruptionPickCharacter;
			}
			break;
		case TinaState.PlayTime:
			mAnimationState = TinaAnimationState.VacuumInterruption;
			break;
		}
	}

	public void StopVacuumAnimation()
	{
		normalizedTime = 0f;
		mAnimator.SetFloat("IntroReturn", introResumeTime);
		mAnimator.SetBool("Vacuum", value: false);
		GlobalCoroutineContainer.AddThread(ResumeAfterInterrupt(), this);
		switch (sceneRef.currentState)
		{
		case TinaState.Intro:
			mAnimationState = TinaAnimationState.Intro;
			break;
		case TinaState.SelectCharacter:
			if (mAnimator.GetBool("Remote"))
			{
				mAnimationState = TinaAnimationState.PickUpRemote;
			}
			else
			{
				mAnimationState = TinaAnimationState.PickCharacterLoop;
			}
			break;
		case TinaState.PlayTime:
			mAnimationState = TinaAnimationState.MainLoop;
			break;
		}
	}

	private IEnumerator ResumeAfterInterrupt()
	{
		yield return null;
		while (mAnimator.IsInTransition(1))
		{
			yield return null;
		}
		if (mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime != introResumeTime)
		{
		}
		mAnimator.SetFloat("SpeedMult", 1f);
		normalizedTime = 0f;
	}

	public void StartEndSequence()
	{
		mAnimator.SetBool("Ending", value: true);
		mAnimationState = TinaAnimationState.EndSequence;
		normalizedTime = 0f;
	}

	public void StopAllAnimations()
	{
		mAnimator.speed = 0f;
	}

	public void ResumeAllAnimations()
	{
		mAnimator.speed = 1f;
	}
}
