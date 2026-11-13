using System.Collections;
using System.Collections.Generic;
using GVR.Gfx;
using TButt;
using TMPro;
using UnityEngine;

public class HRMerge : SceneBehaviorManager
{
	public bool beeCaughtMerge = true;

	[Header("References:")]
	[SerializeField]
	private MergeScriptableObject mergeContent;

	[SerializeField]
	private GameObject hubParent;

	[SerializeField]
	private GameObject mergeParent;

	[SerializeField]
	private EnvironmentProperties envDark;

	[SerializeField]
	private EnvironmentProperties envHub;

	[SerializeField]
	private GvrAudioSource soundtrack;

	[SerializeField]
	private GameObject newUserSubtitles;

	[Header("Merge:")]
	[SerializeField]
	private VVRDialogueSource chazCoreDialogue;

	[SerializeField]
	private GvrAudioSource chazCoreDialogueSource;

	[SerializeField]
	private GameObject chazCore;

	[SerializeField]
	private ChazController chazLeft;

	[SerializeField]
	private ChazController chazRight;

	[SerializeField]
	private GameObject loadingBar;

	[SerializeField]
	private MergingObject[] loadObjects;

	[SerializeField]
	private Transform floatingObjects;

	[SerializeField]
	private Transform rotatingLights;

	[SerializeField]
	private TextMeshPro fileText;

	[SerializeField]
	private TextSwapper fileSwapper;

	[SerializeField]
	private ChazController brainChaz;

	[SerializeField]
	private GameObject brainLegRight;

	[SerializeField]
	private GameObject brainLegLeft;

	[SerializeField]
	private GameObject brainLegLeftLower;

	[SerializeField]
	private GameObject brainLegRightLower;

	[SerializeField]
	private GameObject brainBrain;

	[SerializeField]
	private RootPositioner brainRootPositioner;

	[SerializeField]
	private GvrAudioSource mergeSound;

	[SerializeField]
	private GvrAudioSource corePlugInSound;

	[SerializeField]
	private GvrAudioSource unplugSound;

	[SerializeField]
	private GvrAudioSource dialupSound;

	[SerializeField]
	private Rigidbody plugRB;

	[SerializeField]
	private Transform beeCore;

	[SerializeField]
	private Transform coreSwivelPos;

	[SerializeField]
	private Transform coreRestPos;

	[SerializeField]
	private GameObject beeLaserDummy;

	[SerializeField]
	private GameObject beeLaserDummy2;

	[SerializeField]
	private Transform mainCore;

	private float loweredVolumeAmount = 0.3f;

	private float debugSpeedUp;

	private float nextFileTime;

	private float nextFileCount;

	private float nextFileMin = 0.01f;

	private float nextFileMax = 0.025f;

	private float nextLongFileMin = 0.5f;

	private float nextLongFileMax = 2f;

	private float nextLongFileCount;

	private float longFileLoadMin = 0.15f;

	private float longFileLoadMax = 0.5f;

	private float loadingTime;

	private float loadingTimeMax = 15f;

	private float objectMoveSpeed = 10f;

	private int longFilePauseIndex;

	private string[] uploadedFiles = new string[18]
	{
		"TEXT_181", "TEXT_182", "TEXT_183", "TEXT_184", "TEXT_185", "TEXT_186", "TEXT_187", "TEXT_188", "TEXT_189", "TEXT_190",
		"TEXT_191", "TEXT_192", "TEXT_193", "TEXT_194", "TEXT_195", "TEXT_196", "TEXT_197", "TEXT_198"
	};

	private string[] uploadedFilesPause = new string[18]
	{
		"TEXT_181", "TEXT_182", "TEXT_183", "TEXT_184", "TEXT_185", "TEXT_186", "TEXT_187", "TEXT_188", "TEXT_189", "TEXT_190",
		"TEXT_191", "TEXT_192", "TEXT_193", "TEXT_194", "TEXT_195", "TEXT_196", "TEXT_197", "TEXT_198"
	};

	[Header("Transition:")]
	[SerializeField]
	private GameObject pupilMask;

	[Header("Login:")]
	[SerializeField]
	private VVRDialogueSource chazDialogue;

	[SerializeField]
	private GameObject eyeVignette;

	[SerializeField]
	private AudioSource servoSounds;

	[SerializeField]
	private GameObject archiveHeadsets;

	[SerializeField]
	private GameObject oldAestheticObjects;

	[SerializeField]
	private GameObject newAestheticObjects;

	[SerializeField]
	private VVR_Point hubLight;

	[SerializeField]
	private GvrAudioSource lightsOnSound;

	[SerializeField]
	private GvrAudioSource objectSpawnSound;

	[SerializeField]
	private GvrAudioSource objectSwitchSound;

	[SerializeField]
	private GvrAudioSource loginSoundtrack;

	[SerializeField]
	private GvrAudioSource newUserSound;

	[SerializeField]
	private GvrAudioSource loginSound;

	[SerializeField]
	private GvrAudioSource flapSound;

	[SerializeField]
	private VVRConsumable activitudeV;

	[SerializeField]
	private VVRConsumable[] activitudeLetters;

	[SerializeField]
	private VVRConsumer fakeConsumer;

	[SerializeField]
	private SkipMergeButton skipButton;

	private Vector3 eyePos = new Vector3(0f, -0.25f, 0.25f);

	private Quaternion lastHeadRot;

	private Quaternion headRot;

	private Vector3 headPosition;

	private Vector3 lastHeadPosition;

	private float headVel;

	private float pitchRandomAmount = 0.7f;

	private float pitchRandomSpeed = 3f;

	private float pitchShiftSpeed = 10f;

	private float targetPitch;

	private float currentVolume;

	private float targetVolume;

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		hubParent.SetActive(value: false);
		mergeParent.SetActive(value: false);
		GetComponent<EagerScene>().mEnvironmentProperties = envDark;
		envDark.pushAllProperties();
		loadingBar.transform.localScale = Vector3.up + Vector3.forward;
		loadingBar.transform.parent.gameObject.SetActive(value: false);
		fileText.text = string.Empty;
		GlobalCoroutineContainer.AddThread(HRMergeCoroutine(), this);
	}

	private IEnumerator HRMergeCoroutine()
	{
		yield return null;
		if ((bool)VVR_HR.instance && VVR_HR.instance.battleHasBegun)
		{
			beeCaughtMerge = true;
		}
		else
		{
			beeCaughtMerge = false;
		}
		ToolManager.AllTeleporters.TeleportEnabled = false;
		if (!PersistentDataManager.GetCanSkipMerge())
		{
			ToolManager.ToggleManipulator(toggle: false);
			skipButton.gameObject.SetActive(value: false);
		}
		else
		{
			skipButton.gameObject.SetActive(value: true);
		}
		EagerHeadsetManager.SetLockPlayerInScene(lockPlayer: true);
		EagerScenes.instance.DestroyAllScenesExcept(TCScenes.S.VVR_ENDING_MERGE);
		yield return GlobalCoroutineContainer.AddThread(HRMergeSequenceCoroutine(), this);
		mergeParent.SetActive(value: false);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(4f, this);
		pupilMask.transform.parent = EagerMaster.instance.mainCamera.transform;
		pupilMask.transform.localPosition = Vector3.forward * 0.2f;
		pupilMask.transform.localRotation = Quaternion.identity;
		pupilMask.SetActive(value: true);
		PupilTransitionController.instance.AnimateEyes(openEyes: false);
		hubParent.SetActive(value: true);
		envHub.pointLights = new List<VVR_Point> { hubLight };
		GetComponent<EagerScene>().mEnvironmentProperties = envHub;
		envHub.pushAllProperties();
		EagerMaster.instance.player.transform.position = Vector3.up;
		EagerMaster.instance.headsetHeightManager.NormalizePlayerHeight();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(3f, this);
		yield return GlobalCoroutineContainer.AddThread(HRHubSequenceCoroutine(), this);
	}

	private IEnumerator HRMergeSequenceCoroutine()
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
		CharacterFlip.SetCanFlip(newValue: false);
		loadingTime = 0f;
		mergeParent.SetActive(value: true);
		Vector3 startPos = chazCore.transform.position;
		float coreMoveCount = 0f;
		float coreMoveTime = 0.5f;
		while (coreMoveCount < coreMoveTime)
		{
			coreMoveCount += Time.deltaTime;
			float lerpVal = Mathf.Clamp01(coreMoveCount / coreMoveTime);
			lerpVal = lerpVal * lerpVal * (3f - 2f * lerpVal);
			chazCore.transform.position = Vector3.Lerp(startPos, startPos - Vector3.up * 0.75f, lerpVal);
			yield return null;
		}
		if (beeCaughtMerge)
		{
			chazCoreDialogue.PlayClip(mergeContent.dialogue.mCaughtWelcome);
		}
		else
		{
			chazCoreDialogue.PlayClip(mergeContent.dialogue.mPeacefulWelcome);
		}
		while (chazCoreDialogue.isPlaying)
		{
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		soundtrack.Play();
		loadingBar.transform.parent.gameObject.SetActive(value: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
		chazCoreDialogue.PlayClip(mergeContent.dialogue.mMergeSpeech);
		chazCoreDialogue.PlayClip(mergeContent.dialogue.mPostMergeChatter);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		while (loadingTime < loadingTimeMax)
		{
			brainChaz.ReStraighten();
			if (nextFileCount > 0f)
			{
				nextFileCount -= Time.deltaTime * debugSpeedUp;
				nextLongFileCount -= Time.deltaTime * debugSpeedUp;
			}
			else
			{
				rotatingLights.Rotate(Vector3.up * 25f);
				floatingObjects.transform.position -= Vector3.forward * objectMoveSpeed * Time.deltaTime;
				loadingTime += nextFileTime;
				loadingBar.transform.localScale = new Vector3(loadingTime / loadingTimeMax, 1f, 1f);
				for (int i = 0; i < loadObjects.Length; i++)
				{
					loadObjects[i].NextGameObject();
				}
				if (nextLongFileCount < 0f && longFilePauseIndex < uploadedFilesPause.Length)
				{
					fileSwapper.SetNewKey(uploadedFilesPause[longFilePauseIndex]);
					longFilePauseIndex = Mathf.Clamp(longFilePauseIndex + 1, 0, uploadedFilesPause.Length);
					nextFileTime = Random.Range(longFileLoadMin, longFileLoadMax);
					nextFileCount = nextFileTime;
					nextLongFileCount = Random.Range(nextLongFileMin, nextLongFileMax) + nextFileTime;
				}
				else
				{
					fileSwapper.SetNewKey(uploadedFiles[Random.Range(0, uploadedFiles.Length)]);
					nextFileTime = Random.Range(nextFileMin, nextFileMax);
					nextFileCount = nextFileTime;
				}
			}
			yield return null;
		}
		beeCore.parent = mergeParent.transform;
		fileText.text = string.Empty;
		fileText.transform.parent.gameObject.SetActive(value: false);
		unplugSound.Play();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
		mergeSound.Play();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
		coreMoveCount = 0f;
		coreMoveTime = 2f;
		startPos = beeCore.transform.position;
		while (coreMoveCount < coreMoveTime)
		{
			coreMoveCount += Time.deltaTime;
			float lerpVal = Mathf.Clamp01(coreMoveCount / coreMoveTime);
			lerpVal = lerpVal * lerpVal * (3f - 2f * lerpVal);
			beeCore.transform.position = Vector3.Lerp(startPos, coreSwivelPos.position, lerpVal);
			yield return null;
		}
		Object.Destroy(plugRB.GetComponent<FixedJoint>());
		plugRB.AddForce(Vector3.up * 250f);
		coreMoveCount = 0f;
		coreMoveTime = 1f;
		while (coreMoveCount < coreMoveTime)
		{
			coreMoveCount += Time.deltaTime;
			float lerpVal = Mathf.Clamp01(coreMoveCount / coreMoveTime);
			lerpVal = lerpVal * lerpVal * (3f - 2f * lerpVal);
			beeCore.transform.rotation = Quaternion.Euler(180f * lerpVal, 180f * lerpVal, 0f);
			yield return null;
		}
		coreMoveCount = 0f;
		coreMoveTime = 4f;
		while (coreMoveCount < coreMoveTime)
		{
			coreMoveCount += Time.deltaTime;
			float lerpVal = Mathf.Clamp01(coreMoveCount / coreMoveTime);
			lerpVal = lerpVal * lerpVal * (3f - 2f * lerpVal);
			beeCore.transform.position = Vector3.Lerp(coreSwivelPos.position, coreRestPos.position, lerpVal);
			soundtrack.volume = Mathf.Clamp(1f - lerpVal, loweredVolumeAmount, 1f);
			chazCoreDialogueSource.volume = Mathf.Clamp(1f - lerpVal, loweredVolumeAmount, 1f);
			yield return null;
		}
		coreMoveCount = 0f;
		coreMoveTime = 1f;
		startPos = chazCore.transform.position;
		while (coreMoveCount < coreMoveTime)
		{
			coreMoveCount += Time.deltaTime;
			float lerpVal = Mathf.Clamp01(coreMoveCount / coreMoveTime);
			lerpVal = lerpVal * lerpVal * (3f - 2f * lerpVal);
			beeCore.transform.position = Vector3.Lerp(coreRestPos.position, coreRestPos.position + Vector3.up * 2.15f, lerpVal);
			chazCore.transform.position = Vector3.Lerp(startPos, startPos + Vector3.up * 1.5f, lerpVal);
			yield return null;
		}
		beeCore.parent = mainCore;
		coreMoveCount = 0f;
		coreMoveTime = 6f;
		startPos = mainCore.position;
		while (coreMoveCount < coreMoveTime)
		{
			coreMoveCount += Time.deltaTime;
			float lerpVal = Mathf.Clamp01(coreMoveCount / coreMoveTime);
			lerpVal *= lerpVal;
			mainCore.transform.position = Vector3.Lerp(startPos, startPos + Vector3.up * 7.5f, lerpVal);
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
		chazLeft.transform.parent.gameObject.SetActive(value: true);
		chazLeft.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.2f, this);
		chazRight.transform.parent.gameObject.SetActive(value: true);
		chazRight.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Angry, immediately: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
		chazLeft.WalkTo(chazLeft.transform.position + chazLeft.transform.parent.forward * 7.5f);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		chazRight.WalkTo(chazRight.transform.position + chazRight.transform.parent.forward * 7.5f);
		coreMoveCount = 0f;
		coreMoveTime = 5.5f;
		while (coreMoveCount < coreMoveTime)
		{
			coreMoveCount += Time.deltaTime;
			float lerpVal = Mathf.Clamp01(coreMoveCount / coreMoveTime);
			lerpVal *= lerpVal;
			soundtrack.volume = Mathf.Clamp((1f - lerpVal) * loweredVolumeAmount, 0f, loweredVolumeAmount);
			chazCoreDialogueSource.volume = Mathf.Clamp((1f - lerpVal) * loweredVolumeAmount, 0f, loweredVolumeAmount);
			yield return null;
		}
		Object.Destroy(brainRootPositioner);
		Object.Destroy(brainChaz);
		Vector3 brainPos = brainBrain.transform.position;
		chazLeft.LazerObject(brainLegLeftLower, destroy: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.25f, this);
		chazRight.LazerObject(brainBrain, destroy: true);
		while (brainBrain.activeInHierarchy)
		{
			yield return null;
		}
		Object.Destroy(brainLegRight.GetComponent<HingeJoint>());
		Object.Destroy(brainLegLeft.GetComponent<HingeJoint>());
		brainLegLeft.GetComponent<Rigidbody>().AddExplosionForce(200f, brainPos, 25f);
		brainLegRight.GetComponent<Rigidbody>().AddExplosionForce(300f, brainPos, 25f);
		brainLegRight.GetComponent<Rigidbody>().AddTorque(Vector3.up * 300f);
		brainLegLeft.GetComponent<Rigidbody>().AddTorque(Vector3.up * -500f);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.15f, this);
		chazLeft.LazerObject(brainLegRightLower, destroy: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.35f, this);
		chazRight.LazerObject(brainLegLeft, destroy: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.25f, this);
		chazLeft.LazerObject(brainLegRight, destroy: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
		beeLaserDummy.transform.position = new Vector3(EagerMaster.instance.mainCamera.transform.position.x, EagerMaster.instance.mainCamera.transform.position.y, EagerMaster.instance.mainCamera.transform.position.z + 0.25f);
		chazLeft.LazerObject(beeLaserDummy);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.1f, this);
		beeLaserDummy.transform.position = new Vector3(EagerMaster.instance.mainCamera.transform.position.x, EagerMaster.instance.mainCamera.transform.position.y, EagerMaster.instance.mainCamera.transform.position.z + 0.25f);
		chazRight.LazerObject(beeLaserDummy2);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		PersistentDataManager.OverwriteCanSkipMerge(s: true);
		skipButton.gameObject.SetActive(value: false);
		ToolManager.ToggleManipulator(toggle: false);
	}

	private IEnumerator HRHubSequenceCoroutine()
	{
		servoSounds.Play();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.5f, this);
		chazDialogue.ForcePlayClipOverwrite(mergeContent.dialogue.lPositionsEveryone);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
		pupilMask.SetActive(value: false);
		PupilTransitionController.instance.AnimateEyes(openEyes: true);
		while (chazDialogue.isPlaying)
		{
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
		chazDialogue.PlayClip(mergeContent.dialogue.lElon);
		while (chazDialogue.isPlaying)
		{
			yield return null;
		}
		flapSound.Play();
		chazDialogue.PlayClip(mergeContent.dialogue.lHanPo);
		while (chazDialogue.isPlaying)
		{
			yield return null;
		}
		archiveHeadsets.SetActive(value: true);
		objectSpawnSound.Play();
		chazDialogue.PlayClip(mergeContent.dialogue.lLogo);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2.25f, this);
		fakeConsumer.gameObject.SetActive(value: true);
		fakeConsumer.isSucking = false;
		for (int i = 0; i < activitudeLetters.Length; i++)
		{
			activitudeLetters[i].OneShotTransfer(fakeConsumer, Random.Range(0.1f, 0.5f));
		}
		fakeConsumer.gameObject.SetActive(value: false);
		while (chazDialogue.isPlaying)
		{
			yield return null;
		}
		fakeConsumer.gameObject.SetActive(value: true);
		fakeConsumer.isSucking = false;
		activitudeV.OneShotTransfer(fakeConsumer, 1f);
		fakeConsumer.gameObject.SetActive(value: false);
		chazDialogue.PlayClip(mergeContent.dialogue.lCarl);
		while (chazDialogue.isPlaying)
		{
			yield return null;
		}
		float lightLerp = 0f;
		lightsOnSound.Play();
		while (lightLerp < 1f)
		{
			lightLerp += Time.deltaTime;
			hubLight.intensity = lightLerp;
			envHub.pushPointLights();
			yield return null;
		}
		chazDialogue.PlayClip(mergeContent.dialogue.lBeeHelp);
		while (chazDialogue.isPlaying)
		{
			yield return null;
		}
		oldAestheticObjects.SetActive(value: false);
		newAestheticObjects.SetActive(value: true);
		objectSwitchSound.Play();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		chazDialogue.PlayClip(mergeContent.dialogue.lItsBeautiful);
		while (chazDialogue.isPlaying)
		{
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.25f, this);
		chazDialogue.PlayClip(mergeContent.dialogue.lGladToHaveYou);
		while (chazDialogue.isPlaying)
		{
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.2f, this);
		newUserSound.Play();
		newUserSubtitles.GetComponent<SubtitleRenderer>().SetIsPlaying(s: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(3f, this);
		newUserSubtitles.SetActive(value: false);
		chazDialogue.PlayClip(mergeContent.dialogue.lGameFaces);
		float soundtrackLerp = 0f;
		float soundtrackMaxVolume = 0.2f;
		loginSoundtrack.Play();
		while (soundtrackLerp < 1f)
		{
			soundtrackLerp += Time.deltaTime;
			loginSoundtrack.volume = Mathf.Lerp(0f, soundtrackMaxVolume, soundtrackLerp);
			yield return null;
		}
		while (newUserSound.isPlaying)
		{
			yield return null;
		}
		loginSound.Play();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		chazDialogue.PlayClip(mergeContent.dialogue.lWelcome);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.75f, this);
		CharacterFlip.SetCanFlip(newValue: true);
		EagerHeadsetManager.instance.RemoveHeadset();
	}

	private void Update()
	{
		if (hubParent.activeInHierarchy)
		{
			eyePos = EagerMaster.instance.mainCamera.transform.position;
			eyeVignette.transform.rotation = EagerMaster.instance.mainCamera.transform.rotation;
			eyeVignette.transform.position = eyePos;
			headRot = EagerMaster.instance.mainCamera.transform.rotation;
			headVel = Quaternion.Angle(headRot, lastHeadRot);
			headVel *= Time.deltaTime;
			headVel = ((!(headVel < 0.0075f)) ? headVel : 0f);
			headVel *= 100000f;
			lastHeadRot = headRot;
			headVel = Mathf.Clamp(headVel, 0.3f, 1.25f);
			targetVolume = ((!(headVel > 0.4f)) ? 0f : 1f);
			if (TBCore.GetActivePlatform() != VRPlatform.OculusMobile && targetVolume == 0f)
			{
				headPosition = EagerMaster.instance.mainCamera.transform.position;
				headVel = Vector3.Distance(headPosition, lastHeadPosition) / Time.deltaTime;
				headVel = ((!(headVel < 0.09f)) ? headVel : 0f);
				headVel *= 100000f;
				lastHeadRot = headRot;
				headVel = Mathf.Clamp(headVel, 0.3f, 1.25f);
				targetVolume = ((!(headVel > 0.4f)) ? 0f : 1f);
			}
			lastHeadPosition = EagerMaster.instance.mainCamera.transform.position;
			targetPitch = Mathf.Lerp(headVel, headVel * Mathf.PerlinNoise(Time.time * pitchRandomSpeed, 0f), pitchRandomAmount);
			currentVolume = Utility.TCVRLerp(currentVolume, targetVolume, pitchShiftSpeed);
			servoSounds.volume = currentVolume;
			servoSounds.pitch = Utility.TCVRLerp(servoSounds.pitch, targetPitch, pitchShiftSpeed);
			targetVolume = 1f;
		}
		if (mergeParent.activeInHierarchy)
		{
			if (Input.GetKey(KeyCode.Space))
			{
				debugSpeedUp = 50f;
			}
			else
			{
				debugSpeedUp = 1f;
			}
		}
	}
}
