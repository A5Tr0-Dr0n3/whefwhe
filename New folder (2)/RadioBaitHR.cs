using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class RadioBaitHR : SceneBehaviorManager
{
	public ObjectBase HRHeadset;

	public GameObject neonBee;

	public GameObject neonArrow;

	public GvrAudioSource headsetGlitch;

	public GvrAudioSource headsetMessage;

	public Transform neonVisible;

	public SubtitleRenderer subtitles;

	private SafeCoroutine endThread;

	private WaitForSeconds flicker = new WaitForSeconds(0.02f);

	private void Start()
	{
		neonBee.SetActive(value: false);
		neonArrow.SetActive(value: false);
	}

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		GlobalCoroutineContainer.AddThread(radioBaitSequence(), this);
	}

	private void Update()
	{
		if (ToolManager.PlayerGrabbingObjectBase(HRHeadset) && endThread == null)
		{
			endThread = GlobalCoroutineContainer.AddThread(endRadioBaitSequence(), this);
		}
	}

	private IEnumerator endRadioBaitSequence()
	{
		HRHeadset.undroppable = true;
		headsetGlitch.Play();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.25f, this);
		headsetMessage.Play();
		subtitles.SetIsPlaying(s: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.5f, this);
		subtitles.SetIsPlaying(s: false);
		EagerHeadsetManager.SetCanRemoveHeadset(canRemove: true);
		EagerScenes.instance.TakeOffAllHeadsets();
	}

	private IEnumerator radioBaitSequence()
	{
		yield return new WaitForSeconds(2.5f);
		bool onScreen = false;
		int halfSecondCounter = 0;
		while (!onScreen && halfSecondCounter < 40)
		{
			yield return new WaitForSeconds(0.5f);
			halfSecondCounter++;
			Vector3 screenPoint = EagerMaster.instance.mainCamera.WorldToViewportPoint(neonVisible.position);
			onScreen = screenPoint.z > 0f && screenPoint.x > 0f && screenPoint.x < 1f && screenPoint.y > 0f && screenPoint.y < 1f;
		}
		yield return flicker;
		neonBee.SetActive(value: true);
		yield return flicker;
		neonBee.SetActive(value: false);
		yield return flicker;
		neonBee.SetActive(value: true);
		yield return flicker;
		neonBee.SetActive(value: false);
		yield return flicker;
		neonBee.SetActive(value: true);
		yield return flicker;
		neonBee.SetActive(value: false);
		yield return flicker;
		neonBee.SetActive(value: true);
		yield return flicker;
		neonBee.SetActive(value: false);
		yield return flicker;
		neonBee.SetActive(value: true);
		yield return new WaitForSeconds(1.5f);
		neonArrow.SetActive(value: true);
		yield return flicker;
		neonArrow.SetActive(value: false);
		yield return flicker;
		neonArrow.SetActive(value: true);
		yield return flicker;
		neonArrow.SetActive(value: false);
		yield return flicker;
		neonArrow.SetActive(value: true);
		yield return null;
	}
}
