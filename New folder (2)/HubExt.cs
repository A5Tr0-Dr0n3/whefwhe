using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class HubExt : VVR_HUB
{
	[Header("Extended Game References:")]
	[SerializeField]
	private Transform[] chazResetPositions;

	[SerializeField]
	private PlayerCollisionTrigger relaxationZoneBounds;

	[SerializeField]
	private GameObject flipperRoot;

	[SerializeField]
	private GameObject startDome;

	[SerializeField]
	private GameObject flipperDome;

	[SerializeField]
	private FlipperLever flipperLever;

	private bool hasDoneFridgeComment;

	private bool hasDoneSnackComment;

	private bool hasDoneVrComment;

	private bool hasDoneRelaxationZoneComment;

	private bool hasDoneToastComment;

	private bool hasDoneSodaComment;

	private bool hasDoneRelaxedComment;

	private bool hasDoneAppreciationComment;

	private bool hasDoneStereoscopeComment;

	private bool hasDoneSensoramaComment;

	private bool hasDoneLinkTrainerComment;

	private int relaxationZoneMonologueIndex;

	private bool playerInRelaxationZone;

	internal bool settingUpFlipper;

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		hasDoneSnackComment = PersistentDataManager.GetSnackZoneIntro();
		hasDoneRelaxationZoneComment = PersistentDataManager.GetRelaxationZoneIntro();
		hasDoneVrComment = PersistentDataManager.GetVrZoneIntro();
		hasDoneFlipTutorial = PersistentDataManager.GetHasDoneFlipTutorial();
		relaxationZoneMonologueIndex = PersistentDataManager.GetRelaxationZoneMonologueIndex();
		if (jobState <= JobStates.Macy)
		{
			flipperRoot.SetActive(value: false);
			startDome.SetActive(value: true);
			flipperDome.SetActive(value: false);
		}
		else
		{
			flipperRoot.SetActive(value: true);
			startDome.SetActive(value: false);
			flipperDome.SetActive(value: true);
		}
	}

	protected override void OnSceneActivated()
	{
		base.OnSceneActivated();
		if (!ignoreNextReentry)
		{
			if (!hasStartedJob)
			{
				return;
			}
			if (jobState == JobStates.Macy)
			{
				if (JobMacy.instance.currentState == MacyState.KickedOut)
				{
					InterruptMacy();
				}
				else
				{
					InterruptJob();
				}
				jobHeadset.GetComponent<Headset>().ResetScale();
			}
			if (!hasCompletedJob && jobState != JobStates.Macy)
			{
				InterruptJob();
			}
			else if (hasCompletedJob && (!PersistentDataManager.GetVacuumState() || wasInJail || wasInUnion || jobState == JobStates.Tina))
			{
				FinishJob();
			}
			else if (jobState != JobStates.Macy)
			{
				InterruptJob();
			}
		}
		else
		{
			ignoreNextReentry = false;
		}
	}

	public void CommentSnackZone()
	{
		if (chazComments && chazControl.isActiveAndEnabled && Time.time - lastComment > 5f && (commentThread == null || !commentThread.IsRunning) && !hasDoneSnackComment)
		{
			commentThread = GlobalCoroutineContainer.AddThread(CommentSnackZoneCoroutine(), this);
			hasDoneSnackComment = true;
			PersistentDataManager.OverwriteSnackZoneIntro(1);
		}
	}

	private IEnumerator CommentSnackZoneCoroutine()
	{
		chazDialogueSource.PlayClip(hubContent.dialogue.snackZone);
		yield return null;
	}

	public void CommentRelaxationZone()
	{
		if (chazComments && chazControl.isActiveAndEnabled && Time.time - lastComment > 5f && (commentThread == null || !commentThread.IsRunning) && !hasDoneRelaxationZoneComment)
		{
			commentThread = GlobalCoroutineContainer.AddThread(CommentRelaxationZoneCoroutine(), this);
			hasDoneRelaxationZoneComment = true;
			PersistentDataManager.OverwriteRelaxationZoneIntro(1);
		}
	}

	private IEnumerator CommentRelaxationZoneCoroutine()
	{
		chazDialogueSource.PlayClip(hubContent.dialogue.relaxationZone);
		yield return null;
	}

	public void CommentVrZone()
	{
		if (chazComments && chazControl.isActiveAndEnabled && Time.time - lastComment > 5f && (commentThread == null || !commentThread.IsRunning) && !hasDoneVrComment)
		{
			commentThread = GlobalCoroutineContainer.AddThread(CommentVrZoneCoroutine(), this);
			hasDoneVrComment = true;
			PersistentDataManager.OverwriteVrZoneIntro(1);
		}
	}

	private IEnumerator CommentVrZoneCoroutine()
	{
		chazDialogueSource.PlayClip(hubContent.dialogue.vrZone);
		yield return null;
	}

	public void CommentVrZoneStereoscope()
	{
		if (chazComments && chazControl.isActiveAndEnabled && Time.time - lastComment > 5f && (commentThread == null || !commentThread.IsRunning) && !hasDoneStereoscopeComment)
		{
			commentThread = GlobalCoroutineContainer.AddThread(CommentVrZoneSteroscopeCoroutine(), this);
			hasDoneStereoscopeComment = true;
		}
	}

	private IEnumerator CommentVrZoneSteroscopeCoroutine()
	{
		chazDialogueSource.PlayClip(hubContent.dialogue.vrZoneStereoscope);
		yield return null;
	}

	public void CommentVrZoneLinkTrainer()
	{
		if (chazComments && chazControl.isActiveAndEnabled && Time.time - lastComment > 5f && (commentThread == null || !commentThread.IsRunning) && !hasDoneLinkTrainerComment)
		{
			commentThread = GlobalCoroutineContainer.AddThread(CommentVrZoneLinkTrainerCoroutine(), this);
			hasDoneLinkTrainerComment = true;
		}
	}

	private IEnumerator CommentVrZoneLinkTrainerCoroutine()
	{
		chazDialogueSource.PlayClip(hubContent.dialogue.vrZoneLinkTrainer);
		yield return null;
	}

	public void CommentVrZoneSensorama()
	{
		if (chazComments && chazControl.isActiveAndEnabled && Time.time - lastComment > 5f && (commentThread == null || !commentThread.IsRunning) && !hasDoneSensoramaComment)
		{
			commentThread = GlobalCoroutineContainer.AddThread(CommentVrZoneSensoramrCoroutine(), this);
			hasDoneSensoramaComment = true;
		}
	}

	private IEnumerator CommentVrZoneSensoramrCoroutine()
	{
		chazDialogueSource.PlayClip(hubContent.dialogue.vrZoneSensorama);
		yield return null;
	}

	public void CommentSnackSoda()
	{
		if (chazComments && chazControl.isActiveAndEnabled && Time.time - lastComment > 5f && (commentThread == null || !commentThread.IsRunning) && !hasDoneSodaComment)
		{
			hasDoneSodaComment = true;
			commentThread = GlobalCoroutineContainer.AddThread(CommentSnackSodaCoroutine(), this);
		}
	}

	private IEnumerator CommentSnackSodaCoroutine()
	{
		chazDialogueSource.PlayClip(hubContent.dialogue.snackZoneSoda);
		yield return null;
	}

	public void CommentSnackFridge()
	{
		if (chazComments && chazControl.isActiveAndEnabled && Time.time - lastComment > 5f && (commentThread == null || !commentThread.IsRunning) && !hasDoneFridgeComment)
		{
			hasDoneFridgeComment = true;
			commentThread = GlobalCoroutineContainer.AddThread(CommentSnackFridgeCoroutine(), this);
		}
	}

	private IEnumerator CommentSnackFridgeCoroutine()
	{
		chazDialogueSource.PlayClip(hubContent.dialogue.snackZoneFridge);
		yield return null;
	}

	public void CommentSnackToast()
	{
		if (chazComments && chazControl.isActiveAndEnabled && Time.time - lastComment > 5f && (commentThread == null || !commentThread.IsRunning) && !hasDoneToastComment)
		{
			commentThread = GlobalCoroutineContainer.AddThread(CommentSnackToastCoroutine(), this);
		}
	}

	private IEnumerator CommentSnackToastCoroutine()
	{
		chazDialogueSource.PlayClip(hubContent.dialogue.snackZoneToast);
		yield return null;
	}

	public void CommentRelaxationZoneMonologue()
	{
		playerInRelaxationZone = true;
		if (chazComments && chazControl.isActiveAndEnabled && Time.time - lastComment > 5f && (commentThread == null || !commentThread.IsRunning))
		{
			commentThread = GlobalCoroutineContainer.AddThread(CommentRelaxationZoneMonologueCoroutine(), this);
		}
	}

	public void ExitRelaxationZone()
	{
		playerInRelaxationZone = false;
	}

	private IEnumerator CommentRelaxationZoneMonologueCoroutine()
	{
		while (relaxationZoneMonologueIndex < hubContent.dialogue.relaxationMonologues.Length && playerInRelaxationZone)
		{
			DialoguePlaybackInstance hold = chazDialogueSource.PlayClip(hubContent.dialogue.relaxationMonologues[relaxationZoneMonologueIndex]);
			while (chazDialogueSource.isPlaying && playerInRelaxationZone)
			{
				yield return null;
			}
			if (!playerInRelaxationZone)
			{
				chazDialogueSource.PurgePlaybackClipFromQueue(hold);
			}
			relaxationZoneMonologueIndex++;
			PersistentDataManager.OverwriteRelaxationZoneMonologueIndex(relaxationZoneMonologueIndex);
		}
		yield return null;
	}

	public void EnableFlipper()
	{
		flipperRoot.SetActive(value: true);
		startDome.SetActive(value: false);
		flipperDome.SetActive(value: true);
	}

	protected override SafeCoroutine SetUpFlipper()
	{
		return GlobalCoroutineContainer.AddThread(SetUpFlipperCoroutine(), this);
	}

	private IEnumerator SetUpFlipperCoroutine()
	{
		chazComments = false;
		settingUpFlipper = true;
		chazControl.WalkTo(chazResetPos);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		chazDialogueSource.PlayClip(hubContent.dialogue.flipperSetup);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(5.5f, this);
		startDome.SetActive(value: false);
		flipperRoot.SetActive(value: true);
		flipperDome.SetActive(value: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2.5f, this);
		flipperLever.SetIntroSequence(s: true);
		flipperLever.FlipToSnackZone(right: false);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(flipperZone.GetRotationTime() + 3.5f, this);
		flipperLever.FlipToRelaxationZone(right: false);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(flipperZone.GetRotationTime() + 3.5f, this);
		flipperLever.FlipToVrZone(right: false);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(flipperZone.GetRotationTime() + 3.5f, this);
		flipperLever.FlipToNothing(right: true);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		settingUpFlipper = false;
		flipperLever.SetIntroSequence(s: false);
		chazComments = true;
	}

	private void InterruptMacy()
	{
		if (macyThread == null || Time.time - lastTime > 15f)
		{
			KillAllMainThreads();
			macyThread = GlobalCoroutineContainer.AddThread(InterruptMacyCoroutine(), this);
		}
	}

	private IEnumerator InterruptMacyCoroutine()
	{
		chazComments = false;
		jobHeadset.GetComponent<Headset>().isActivated = false;
		ChazRetrieveHeadsets();
		chazDialogueSource.ForcePlayClipOverwrite(hubContent.dialogue.timeToGetBigger);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(16f, this);
		EagerHeadsetManager.instance.SetForceFatalError(s: true);
		EagerScenes.instance.headsets[EagerScenes.instance.headsets.Count - 1].spawnScale = 5f;
		yield return PlayerScaler.instance.ScaleHead(Vector3.one * 5f, 3f);
		while (chazDialogueSource.isPlaying)
		{
			yield return null;
		}
		jobHeadset.GetComponent<Headset>().isActivated = true;
		if (ToolManager.PlayerGrabbingGameObject(jobHeadset))
		{
			jobHeadset.GetComponent<ObjectBase>().ForceDropByPlayer();
		}
		else if (chazControl.GetComponentInChildren<FixedJoint>() != null)
		{
			chazControl.GetComponentInChildren<FixedJoint>().breakForce = 10f;
			chazControl.GetComponentInChildren<FixedJoint>().breakTorque = 10f;
		}
		JobMacy.instance.currentState = MacyState.ReEntry;
		jobHeadset.GetComponent<Headset>().SendToEagerHeadset();
		chazComments = true;
		yield return null;
	}
}
