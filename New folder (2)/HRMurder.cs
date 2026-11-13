using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class HRMurder : SceneBehaviorManager
{
	public static HRMurder instance;

	[Header("Tweakables")]
	[SerializeField]
	private float holeOpenTime = 4f;

	[SerializeField]
	private AnimationCurve holeOpen;

	[SerializeField]
	private float holeCloseTime = 1f;

	[SerializeField]
	private AnimationCurve holeClose;

	[SerializeField]
	private float forceQuitDelay = 13f;

	[Header("Chaz References")]
	[SerializeField]
	private VVRDialogueSource chazDialogue;

	[SerializeField]
	private ChazController chazController;

	[Header("Dark Room Refs")]
	[SerializeField]
	private GameObject darkRoom;

	[SerializeField]
	private GameObject pupil;

	[SerializeField]
	private GameObject frontPanel;

	[SerializeField]
	private GameObject fakeDarkRoom;

	[SerializeField]
	private GameObject fakeHole;

	[SerializeField]
	private GvrAudioSource startupSound;

	[SerializeField]
	private GvrAudioSource pupilOpenSound;

	[SerializeField]
	private GameObject textPanel;

	[Header("Startup Sequence")]
	[SerializeField]
	private GvrAudioSource rebootSound;

	[SerializeField]
	private TextMeshPro textSequence1;

	[SerializeField]
	private TextMeshPro textSequence2;

	[SerializeField]
	private ObjectSwapper textSequence2Swapper;

	[SerializeField]
	private string key1;

	[SerializeField]
	private string key2;

	private string sequence1String;

	private string sequence2String;

	[Header("Countdown")]
	[SerializeField]
	private GameObject countdownText;

	[Header("Tea Party")]
	[SerializeField]
	private ObjectTrigger teaTrigger;

	[SerializeField]
	private VVRDialogueSource chazSource;

	[SerializeField]
	private ObjectBase sugar;

	[SerializeField]
	private GameObject scone;

	[SerializeField]
	private ObjectBase buddy;

	[Header("Misc")]
	[SerializeField]
	private ElevatorEnvironments envSwitcher;

	[SerializeField]
	private ElizaEnvironmentProperties toSwitch;

	[SerializeField]
	private ObjectTrigger outsideTrigger;

	[SerializeField]
	private GvrAudioSource outsideMusic;

	[Header("Dialogue")]
	[SerializeField]
	private NPCDialogueClip[] endDialogue;

	[Header("Tea Curves")]
	private DateTime timeAtStart;

	public DateTime timeToEnd;

	private HRMurderState currentState;

	private bool portholeOpened;

	private bool firstTimeInScene;

	[HideInInspector]
	public bool skipTimer;

	[HideInInspector]
	public string timeString;

	[HideInInspector]
	public bool murderSceneComplete;

	private bool holdingScones;

	private bool pickedUpScone;

	private bool pickingUpBuddy;

	private void Awake()
	{
		if (instance != null)
		{
			throw new UnityException("Only one instance of HRMurder in a scene is allowed.");
		}
		instance = this;
		timeAtStart = DateTime.Now;
		timeToEnd = timeAtStart;
		timeToEnd = timeToEnd.AddMinutes(52.0);
		timeToEnd = timeToEnd.AddSeconds(47.0);
	}

	private void Update()
	{
		if (buddy.isGrabbed)
		{
			if (!pickingUpBuddy)
			{
				pickingUpBuddy = true;
				chazController.EyeLookAt(buddy.transform, 0f, ChazController.eyeColor.Default, immediately: true);
			}
		}
		else if (pickingUpBuddy)
		{
			pickingUpBuddy = false;
			chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		}
	}

	public void SkipToOneMoreMinute()
	{
		timeToEnd = DateTime.Now.AddMinutes(1.0);
	}

	public override void OnSceneDeactivated()
	{
		PersistentDataManager.OverwriteEndGameTimerStart(timeToEnd);
	}

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		EagerScenes.instance.DestroyAllScenesExcept(TCScenes.S.VVR_ENDING_MURDER);
		currentState = HRMurderState.intro;
		timeAtStart = DateTime.Now;
		timeToEnd = timeAtStart;
		timeToEnd = timeToEnd.AddMinutes(52.0);
		timeToEnd = timeToEnd.AddSeconds(47.0);
		if (PersistentDataManager.GetGetEndGameTimerStart() == default(DateTime))
		{
			PersistentDataManager.OverwriteEndGameTimerStart(timeToEnd);
			firstTimeInScene = true;
		}
		textSequence1.gameObject.SetActive(value: false);
		textSequence2.gameObject.SetActive(value: false);
		sequence1String = SubtitleLoader.instance.GetDictionary()[key1].dialogueLines[0].text;
		sequence2String = SubtitleLoader.instance.GetDictionary()[key2].dialogueLines[0].text;
		textSequence1.text = string.Empty;
		textSequence2.text = string.Empty;
		GlobalCoroutineContainer.AddThread(HRMurderCoroutine(), this);
	}

	protected override void OnSceneActivated()
	{
		base.OnSceneActivated();
		timeToEnd = PersistentDataManager.GetGetEndGameTimerStart();
	}

	private IEnumerator raiseScone(float wait)
	{
		holdingScones = true;
		chazController.EyeLookAt(scone.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		ObjectBase[] scones = scone.GetComponentsInChildren<ObjectBase>();
		for (int i = 0; i < scones.Length; i++)
		{
			scones[i].ungrabbable = false;
		}
		Vector3 targetSconePos = scone.transform.position;
		targetSconePos.y += 0.3f;
		float sequenceT = 0f;
		while (sequenceT < 1f)
		{
			sequenceT += Time.deltaTime / 1f;
			scone.transform.position = Vector3.Slerp(scone.transform.position, targetSconePos, sequenceT);
			yield return null;
		}
		float timer = 0f;
		while (!pickedUpScone && timer < wait)
		{
			timer += Time.deltaTime;
			for (int j = 0; j < scones.Length; j++)
			{
				if (scones[j].isGrabbed)
				{
					chazController.EyeLookAt(scones[j].transform, 0f, ChazController.eyeColor.Default, immediately: true);
					pickedUpScone = true;
				}
			}
			yield return null;
		}
		holdingScones = false;
		sequenceT = 0f;
		targetSconePos.y -= 0.3f;
		while (sequenceT < 1f)
		{
			sequenceT += Time.deltaTime / 1f;
			scone.transform.position = Vector3.Slerp(scone.transform.position, targetSconePos, sequenceT);
			yield return null;
		}
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
	}

	private IEnumerator raiseSugar()
	{
		sugar.ungrabbable = false;
		chazController.EyeLookAt(sugar.transform, 0f, ChazController.eyeColor.Default, immediately: true);
		Rigidbody sugarRb = sugar.GetComponent<Rigidbody>();
		sugarRb.isKinematic = true;
		Vector3 targetSugarPos = sugar.transform.position;
		targetSugarPos.y += 0.3f;
		float sequenceT = 0f;
		while (sequenceT < 1f)
		{
			sequenceT += Time.deltaTime / 1f;
			sugar.transform.position = Vector3.Slerp(sugar.transform.position, targetSugarPos, sequenceT);
			yield return null;
		}
		sugarRb.useGravity = false;
		chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
	}

	private IEnumerator HRMurderCoroutine()
	{
		if (firstTimeInScene)
		{
			countdownText.gameObject.SetActive(value: false);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(3f, this);
			textSequence1.gameObject.SetActive(value: true);
			textSequence2.gameObject.SetActive(value: true);
			rebootSound.Play();
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(3f, this);
			int sequenceIndex = 0;
			float sequenceTime = 1.5f;
			float sequenceCount = 0f;
			while (sequenceCount < sequenceTime)
			{
				sequenceCount += Time.deltaTime;
				sequenceIndex = Mathf.FloorToInt(((float)sequence1String.Length - 1f) * Mathf.Clamp01(sequenceCount / sequenceTime));
				textSequence1.text = sequence1String.Substring(0, sequenceIndex);
				yield return null;
			}
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
			textSequence1.gameObject.SetActive(value: false);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
			do
			{
				textSequence2 = textSequence2Swapper.activeObject.GetComponent<TextMeshPro>();
			}
			while (textSequence2 == null);
			sequenceIndex = 0;
			sequenceTime = 6f;
			sequenceCount = 0f;
			while (sequenceCount < sequenceTime)
			{
				sequenceCount += Time.deltaTime;
				sequenceIndex = Mathf.FloorToInt(((float)sequence2String.Length - 1f) * Mathf.Clamp01(sequenceCount / sequenceTime));
				textSequence2.text = sequence2String.Substring(0, sequenceIndex);
				yield return null;
			}
			timeAtStart = DateTime.Now;
			timeToEnd = timeAtStart;
			timeToEnd = timeToEnd.AddMinutes(52.0);
			timeToEnd = timeToEnd.AddSeconds(47.0);
			PersistentDataManager.OverwriteEndGameTimerStart(timeToEnd);
		}
		do
		{
			textSequence2 = textSequence2Swapper.activeObject.GetComponent<TextMeshPro>();
		}
		while (textSequence2 == null);
		if (currentState == HRMurderState.intro)
		{
			textSequence2.GetComponent<VVRConsumable>().unconsumable = false;
			countdownText.gameObject.SetActive(value: true);
			textSequence2.text = sequence2String;
			if (textSequence2 != null)
			{
				textSequence2.gameObject.SetActive(value: true);
			}
		}
		while (currentState == HRMurderState.intro)
		{
			if (skipTimer)
			{
				timeToEnd = DateTime.Now.AddMinutes(1.0);
			}
			if (checkTime())
			{
				timeString = "0:00:00";
				startupSound.Play();
				countdownText.gameObject.SetActive(value: false);
				if (textSequence2 != null)
				{
					textSequence2.gameObject.SetActive(value: false);
				}
				textSequence1.gameObject.SetActive(value: false);
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
				pupilOpenSound.Play();
				frontPanel.SetActive(value: false);
				textPanel.SetActive(value: false);
				float sequenceT = 0f;
				Vector3 origScale = pupil.transform.localScale;
				Vector3 targetScale = new Vector3(18f, 18f, 1f);
				outsideMusic.Play();
				while (!portholeOpened)
				{
					sequenceT += Time.deltaTime / holeOpenTime;
					pupil.transform.localScale = Vector3.Lerp(origScale, targetScale, holeOpen.Evaluate(sequenceT));
					if (sequenceT >= 1f)
					{
						portholeOpened = true;
					}
					yield return null;
				}
				currentState = HRMurderState.holeOpen;
			}
			else
			{
				tickClock();
			}
			yield return null;
		}
		bool hasSpoken = false;
		while (currentState == HRMurderState.holeOpen)
		{
			bool portholeClosed = false;
			Vector3 targetScale2 = new Vector3(0.1f, 0.1f, fakeHole.transform.localScale.z);
			Vector3 screenPoint = EagerMaster.instance.mainCamera.WorldToViewportPoint(fakeHole.transform.position);
			bool holeOnScreen = screenPoint.z > 0f && screenPoint.x > 0f && screenPoint.x < 1f && screenPoint.y > 0f && screenPoint.y < 1f;
			float sequenceT2 = 0f;
			if (!hasSpoken)
			{
				chazController.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0f, ChazController.eyeColor.Default, immediately: true);
				chazDialogue.PlayClip(endDialogue[0]);
				hasSpoken = true;
			}
			while (chazDialogue.isPlaying)
			{
				if (outsideTrigger.playerInside)
				{
					currentState = HRMurderState.outside;
					darkRoom.SetActive(value: false);
					fakeDarkRoom.SetActive(value: true);
					Vector3 origScale2 = fakeHole.transform.localScale;
					targetScale2 = new Vector3(0.1f, 0.1f, origScale2.z);
					sequenceT2 = 0f;
					screenPoint = EagerMaster.instance.mainCamera.WorldToViewportPoint(fakeHole.transform.position);
					holeOnScreen = screenPoint.z > 0f && screenPoint.x > 0f && screenPoint.x < 1f && screenPoint.y > 0f && screenPoint.y < 1f;
					while (!portholeClosed && holeOnScreen)
					{
						sequenceT2 += Time.deltaTime / holeCloseTime;
						fakeHole.transform.localScale = Vector3.Lerp(origScale2, targetScale2, holeClose.Evaluate(sequenceT2));
						if (sequenceT2 >= 1f)
						{
							fakeHole.SetActive(value: false);
							portholeClosed = true;
						}
						yield return null;
					}
				}
				yield return null;
			}
			if (outsideTrigger.playerInside)
			{
				currentState = HRMurderState.outside;
				darkRoom.SetActive(value: false);
				fakeDarkRoom.SetActive(value: true);
			}
			yield return null;
		}
		while (currentState == HRMurderState.outside)
		{
			bool portholeClosed2 = false;
			Vector3 targetScale3 = new Vector3(0.1f, 0.1f, fakeHole.transform.localScale.z);
			Vector3 screenPoint2 = EagerMaster.instance.mainCamera.WorldToViewportPoint(fakeHole.transform.position);
			bool holeOnScreen2 = screenPoint2.z > 0f && screenPoint2.x > 0f && screenPoint2.x < 1f && screenPoint2.y > 0f && screenPoint2.y < 1f;
			float sequenceT3 = 0f;
			Vector3 origScale3;
			if (teaTrigger.playerInside)
			{
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
				chazDialogue.PlayClip(endDialogue[1]);
				while (chazDialogue.isPlaying)
				{
					origScale3 = fakeHole.transform.localScale;
					targetScale3 = new Vector3(0.1f, 0.1f, origScale3.z);
					sequenceT3 = 0f;
					screenPoint2 = EagerMaster.instance.mainCamera.WorldToViewportPoint(fakeHole.transform.position);
					holeOnScreen2 = screenPoint2.z > 0f && screenPoint2.x > 0f && screenPoint2.x < 1f && screenPoint2.y > 0f && screenPoint2.y < 1f;
					while (!portholeClosed2 && holeOnScreen2)
					{
						sequenceT3 += Time.deltaTime / holeCloseTime;
						fakeHole.transform.localScale = Vector3.Lerp(origScale3, targetScale3, holeClose.Evaluate(sequenceT3));
						if (sequenceT3 >= 1f)
						{
							fakeHole.SetActive(value: false);
							portholeClosed2 = true;
						}
						yield return null;
					}
					yield return null;
				}
				GlobalCoroutineContainer.AddThread(raiseScone(2f), this);
				while (holdingScones)
				{
					yield return null;
				}
				if (pickedUpScone)
				{
					chazDialogue.PlayClip(endDialogue[2]);
				}
				while (chazDialogue.isPlaying)
				{
					origScale3 = fakeHole.transform.localScale;
					targetScale3 = new Vector3(0.1f, 0.1f, origScale3.z);
					sequenceT3 = 0f;
					screenPoint2 = EagerMaster.instance.mainCamera.WorldToViewportPoint(fakeHole.transform.position);
					holeOnScreen2 = screenPoint2.z > 0f && screenPoint2.x > 0f && screenPoint2.x < 1f && screenPoint2.y > 0f && screenPoint2.y < 1f;
					while (!portholeClosed2 && holeOnScreen2)
					{
						sequenceT3 += Time.deltaTime / holeCloseTime;
						fakeHole.transform.localScale = Vector3.Lerp(origScale3, targetScale3, holeClose.Evaluate(sequenceT3));
						if (sequenceT3 >= 1f)
						{
							fakeHole.SetActive(value: false);
							portholeClosed2 = true;
						}
						yield return null;
					}
					yield return null;
				}
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
				chazDialogue.PlayClip(endDialogue[3]);
				while (chazDialogue.isPlaying)
				{
					origScale3 = fakeHole.transform.localScale;
					targetScale3 = new Vector3(0.1f, 0.1f, origScale3.z);
					sequenceT3 = 0f;
					screenPoint2 = EagerMaster.instance.mainCamera.WorldToViewportPoint(fakeHole.transform.position);
					holeOnScreen2 = screenPoint2.z > 0f && screenPoint2.x > 0f && screenPoint2.x < 1f && screenPoint2.y > 0f && screenPoint2.y < 1f;
					while (!portholeClosed2 && holeOnScreen2)
					{
						sequenceT3 += Time.deltaTime / holeCloseTime;
						fakeHole.transform.localScale = Vector3.Lerp(origScale3, targetScale3, holeClose.Evaluate(sequenceT3));
						if (sequenceT3 >= 1f)
						{
							fakeHole.SetActive(value: false);
							portholeClosed2 = true;
						}
						yield return null;
					}
					yield return null;
				}
				yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
				chazDialogue.PlayClip(endDialogue[4]);
				while (chazDialogue.isPlaying)
				{
					origScale3 = fakeHole.transform.localScale;
					targetScale3 = new Vector3(0.1f, 0.1f, origScale3.z);
					sequenceT3 = 0f;
					screenPoint2 = EagerMaster.instance.mainCamera.WorldToViewportPoint(fakeHole.transform.position);
					holeOnScreen2 = screenPoint2.z > 0f && screenPoint2.x > 0f && screenPoint2.x < 1f && screenPoint2.y > 0f && screenPoint2.y < 1f;
					while (!portholeClosed2 && holeOnScreen2)
					{
						sequenceT3 += Time.deltaTime / holeCloseTime;
						fakeHole.transform.localScale = Vector3.Lerp(origScale3, targetScale3, holeClose.Evaluate(sequenceT3));
						if (sequenceT3 >= 1f)
						{
							fakeHole.SetActive(value: false);
							portholeClosed2 = true;
						}
						yield return null;
					}
					yield return null;
				}
				float timer = 0f;
				GlobalCoroutineContainer.AddThread(raiseSugar(), this);
				while (timer < forceQuitDelay)
				{
					timer += Time.deltaTime;
					if (sugar.isGrabbed)
					{
						Rigidbody component = sugar.GetComponent<Rigidbody>();
						component.isKinematic = false;
						timer = forceQuitDelay + 1f;
					}
					yield return null;
				}
				currentState = HRMurderState.chazDoneTalking;
				murderSceneComplete = true;
				EagerHeadsetManager.instance.RemoveHeadset();
			}
			origScale3 = fakeHole.transform.localScale;
			targetScale3 = new Vector3(0.1f, 0.1f, origScale3.z);
			sequenceT3 = 0f;
			screenPoint2 = EagerMaster.instance.mainCamera.WorldToViewportPoint(fakeHole.transform.position);
			holeOnScreen2 = screenPoint2.z > 0f && screenPoint2.x > 0f && screenPoint2.x < 1f && screenPoint2.y > 0f && screenPoint2.y < 1f;
			while (!portholeClosed2 && holeOnScreen2)
			{
				sequenceT3 += Time.deltaTime / holeCloseTime;
				fakeHole.transform.localScale = Vector3.Lerp(origScale3, targetScale3, holeClose.Evaluate(sequenceT3));
				if (sequenceT3 >= 1f)
				{
					fakeHole.SetActive(value: false);
					portholeClosed2 = true;
				}
				yield return null;
			}
			yield return null;
		}
		while (currentState == HRMurderState.chazDoneTalking)
		{
			yield return null;
		}
		yield return null;
	}

	private void tickClock()
	{
		TimeSpan timeSpan = timeToEnd - DateTime.Now;
		if (timeSpan.Minutes >= 10 && timeSpan.Seconds >= 10)
		{
			timeString = "0:" + timeSpan.Minutes + ":" + timeSpan.Seconds;
		}
		else if (timeSpan.Minutes < 10 && timeSpan.Seconds >= 10)
		{
			timeString = "0:0" + timeSpan.Minutes + ":" + timeSpan.Seconds;
		}
		else if (timeSpan.Minutes >= 10 && timeSpan.Seconds < 10)
		{
			timeString = "0:" + timeSpan.Minutes + ":0" + timeSpan.Seconds;
		}
		else if (timeSpan.Minutes < 10 && timeSpan.Seconds < 10)
		{
			timeString = "0:0" + timeSpan.Minutes + ":0" + timeSpan.Seconds;
		}
	}

	private bool checkTime()
	{
		if (DateTime.Compare(DateTime.Now, timeToEnd) > 0)
		{
			return true;
		}
		return false;
	}

	public void SubtractMinutesByTen()
	{
		timeToEnd = timeToEnd.AddMinutes(-10.0);
	}

	public void SubtractMinutesByOne()
	{
		timeToEnd = timeToEnd.AddMinutes(-1.0);
	}

	public void SubtractSecondsByTen()
	{
		timeToEnd = timeToEnd.AddSeconds(-10.0);
	}

	public void SubtractSecondsByOne()
	{
		timeToEnd = timeToEnd.AddSeconds(-1.0);
	}
}
