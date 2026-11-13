using System.Collections;
using System.Collections.Generic;
using GVR.Gfx;
using UnityEngine;

public class LoginLogOutController : MonoBehaviour
{
	[SerializeField]
	private ObjectButton logOutButton;

	[SerializeField]
	private ChazController chaz;

	[SerializeField]
	private List<ObjectButton> buttonsToDisable = new List<ObjectButton>();

	[SerializeField]
	private GvrAudioSource lightSwitch;

	[SerializeField]
	private EnvironmentProperties darkEnvironment;

	[SerializeField]
	private EnvironmentProperties lightEnvironment;

	[SerializeField]
	private MaterialUpdateController darkMaterialProperties;

	private void Awake()
	{
		logOutButton.onClickEvent.AddListener(logOutButtonPress);
	}

	private void logOutButtonPress()
	{
		EagerHeadsetManager.SetLockPlayerInScene(lockPlayer: true);
		buttonsToDisable.ForEach((ObjectButton b) =>
		{
			b.ungrabbable = true;
		});
		GlobalCoroutineContainer.AddThread(logOutSequence(), this);
	}

	private IEnumerator logOutSequence()
	{
		chaz.PowerDown(pitch: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.75f, this);
		lightSwitch.Play();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.4f, this);
		darkEnvironment.gameObject.SetActive(value: true);
		lightEnvironment.gameObject.SetActive(value: false);
		darkEnvironment.Activate();
		darkMaterialProperties.PushMaterials();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.75f, this);
		EagerScenes.LogOut();
	}
}
