using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubLogOutController : MonoBehaviour
{
	[SerializeField]
	private ObjectButton logOutButton;

	[SerializeField]
	private VVR_HUB hubController;

	[SerializeField]
	private ChazController chaz;

	[SerializeField]
	private List<ObjectButton> buttonsToDisable = new List<ObjectButton>();

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
		hubController.PauseAllMainThreads();
		chaz.PowerDown(pitch: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.75f, this);
		hubController.LightsOff();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.75f, this);
		EagerScenes.LogOut();
	}
}
