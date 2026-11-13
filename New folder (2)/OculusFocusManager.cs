using TButt;
using UnityEngine;

public class OculusFocusManager : MonoBehaviour
{
	private bool m_wasUserPresent = true;

	private bool manipulatorWasOn = true;

	private void OnEnable()
	{
		OVRManager.InputFocusAcquired += OnInputFocusAcquired;
		OVRManager.InputFocusLost += OnInputFocusLost;
		if (TBCore.GetActivePlatform() == VRPlatform.OculusMobile)
		{
			OVRManager.HMDMounted += OnInputFocusAcquired;
			OVRManager.HMDUnmounted += OnInputFocusLost;
		}
	}

	private void OnInputFocusLost()
	{
		Time.timeScale = 0f;
		GlobalCoroutineContainer.instance.PauseAllThreads();
		manipulatorWasOn = ToolManager.ActiveToolGroups[0].Manipulator.gameObject.activeSelf;
		ToolManager.ToggleManipulator(toggle: false);
		GetComponent<GvrAudioListener>().globalGainDb = -99f;
		AudioListener.volume = 0f;
	}

	private void OnInputFocusAcquired()
	{
		Time.timeScale = 1f;
		GlobalCoroutineContainer.instance.ResumeAllThreads();
		if (manipulatorWasOn)
		{
			ToolManager.ToggleManipulator(toggle: true);
		}
		GetComponent<GvrAudioListener>().globalGainDb = 0f;
		AudioListener.volume = 1f;
		AudioListener.pause = false;
	}

	private void Update()
	{
		if (TBCore.GetActivePlatform() == VRPlatform.OculusMobile)
		{
			if (!OVRPlugin.userPresent && m_wasUserPresent)
			{
				OnInputFocusLost();
			}
			else if (OVRPlugin.userPresent && !m_wasUserPresent)
			{
				OnInputFocusAcquired();
			}
			m_wasUserPresent = OVRPlugin.userPresent;
		}
	}

	private void OnDisable()
	{
		OVRManager.InputFocusAcquired -= OnInputFocusAcquired;
		OVRManager.InputFocusLost -= OnInputFocusLost;
		if (TBCore.GetActivePlatform() == VRPlatform.OculusMobile)
		{
			OVRManager.HMDMounted -= OnInputFocusAcquired;
			OVRManager.HMDUnmounted -= OnInputFocusLost;
		}
	}
}
