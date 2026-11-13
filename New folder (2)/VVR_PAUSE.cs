using System;
using System.Collections;
using Edelweiss.Coroutine;
using TButt;
using UnityEngine;

[RequireComponent(typeof(GvrAudioSource))]
public class VVR_PAUSE : SceneBehaviorManager
{
	[SerializeField]
	private EagerScene mScene;

	[SerializeField]
	private Transform pauseBubble;

	[SerializeField]
	private Animator pauseAnimator;

	[SerializeField]
	private GameObject pauseText;

	[SerializeField]
	private PauseController mPauseController;

	[SerializeField]
	private float scaleAnimationDelay = 0.8f;

	[SerializeField]
	private float controllerRotateSpeed = 180f;

	[SerializeField]
	private float fadeOutTime = 1f;

	[SerializeField]
	private AudioClip pauseLoadSound;

	private GvrAudioSource mSource;

	private Vector3 cachedPlayerPosition;

	private bool cachedCanTeleport;

	private bool cachedCanGrab;

	private bool cachedCanDrop;

	private bool cachedCanScroll;

	private SafeCoroutine fadeOutThread;

	protected override void OnSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		mSource = GetComponent<GvrAudioSource>();
		cachedPlayerPosition = EagerMaster.instance.player.transform.position;
		EagerMaster.instance.player.transform.position = mScene.playerPos;
		pauseBubble.rotation = Quaternion.LookRotation(EagerMaster.instance.mainCamera.transform.forward.Flatten());
		cachedCanTeleport = Teleporter.GetTeleportEnabled();
		cachedCanGrab = ToolManager.AllManipulators.UserCanGrab;
		cachedCanDrop = ToolManager.AllManipulators.UserCanDrop;
		cachedCanScroll = ToolManager.AllManipulators.UserCanScroll;
		Teleporter.SetTeleportEnabled(enabledIn: false);
		ToolManager.AllManipulators.UserCanDrop = false;
		ToolManager.AllManipulators.UserCanGrab = false;
		ToolManager.AllManipulators.UserCanScroll = false;
		if (VVR_HUB.instance != null)
		{
			VVR_HUB.instance.ignoreNextReentry = true;
		}
		StartCoroutine(popInTextOnDelay());
	}

	private IEnumerator popInTextOnDelay()
	{
		pauseText.SetActive(value: false);
		yield return new WaitForSeconds(scaleAnimationDelay);
		pauseText.SetActive(value: true);
		pauseAnimator.SetTrigger("Enter");
		yield return new WaitForSeconds(0.2f);
		mSource.PlayOneShot(pauseLoadSound);
	}

	public override void OnSceneDeactivated()
	{
		base.OnSceneDeactivated();
		EagerMaster.instance.player.transform.position = cachedPlayerPosition;
		Teleporter.SetTeleportEnabled(cachedCanTeleport);
		ToolManager.AllManipulators.UserCanDrop = cachedCanDrop;
		ToolManager.AllManipulators.UserCanGrab = cachedCanGrab;
		ToolManager.AllManipulators.UserCanScroll = cachedCanScroll;
	}

	private void Update()
	{
		mPauseController.transform.rotation = Quaternion.AngleAxis(controllerRotateSpeed * Time.deltaTime, Vector3.up) * mPauseController.transform.rotation;
		if (ToolManager.AnyControllerGetButtonDown(TBInput.Button.Action1) || ToolManager.AnyControllerGetButtonDown(TBInput.Button.Action2))
		{
			if (fadeOutThread == null || !fadeOutThread.IsRunning)
			{
				fadeOutThread = GlobalCoroutineContainer.AddThread(fadeOutSequence(), this);
			}
		}
		else if (ToolManager.AnyControllerGetButtonDown(TBInput.Button.Any) && (fadeOutThread == null || !fadeOutThread.IsRunning))
		{
			fadeOutThread = GlobalCoroutineContainer.AddThread(fadeOutSequence(), this);
		}
	}

	private void Start()
	{
		TBCore.Events.OnTrackingReset = (TBCore.TBCoreEvent)Delegate.Combine(TBCore.Events.OnTrackingReset, new TBCore.TBCoreEvent(DoRecenter));
	}

	private void OnDestroy()
	{
		TBCore.Events.OnTrackingReset = (TBCore.TBCoreEvent)Delegate.Remove(TBCore.Events.OnTrackingReset, new TBCore.TBCoreEvent(DoRecenter));
	}

	private void DoRecenter()
	{
		pauseBubble.rotation = Quaternion.identity;
	}

	private IEnumerator fadeOutSequence()
	{
		FadeBubbleTransitionController.SetFadeT(0f);
		float timer = 0f;
		while (true)
		{
			float num;
			timer = (num = timer + Time.deltaTime);
			if (!(num <= fadeOutTime))
			{
				break;
			}
			FadeBubbleTransitionController.SetFadeT(timer / fadeOutTime);
			yield return null;
		}
		FadeBubbleTransitionController.SetFadeT(1f);
		EagerScenes.instance.LeavePauseScene();
	}
}
