using UnityEngine;

public class LoginBgmRadio : MonoBehaviour
{
	private enum RadioMode
	{
		adminMusic,
		activitudeMusic,
		silent
	}

	[SerializeField]
	private ObjectButton mButton;

	[SerializeField]
	private GvrAudioSource defaultBgmSource;

	[SerializeField]
	private GvrAudioSource adminBgmSource;

	private RadioMode radioMode = RadioMode.activitudeMusic;

	private float defaultBgmMaxVolume = 1f;

	private float adminBgmMaxVolume = 1f;

	private bool adminMode;

	public void ActivateAdminMode(bool adminModeIn)
	{
		setRadioMode((!adminModeIn) ? RadioMode.activitudeMusic : RadioMode.adminMusic);
		adminMode = adminModeIn;
	}

	private void Awake()
	{
		mButton.onClickEvent.AddListener(cycleRadioMode);
		defaultBgmMaxVolume = defaultBgmSource.volume;
		adminBgmMaxVolume = adminBgmSource.volume;
	}

	private void cycleRadioMode()
	{
		switch (radioMode)
		{
		case RadioMode.adminMusic:
			setRadioMode(RadioMode.activitudeMusic);
			break;
		case RadioMode.activitudeMusic:
			setRadioMode(RadioMode.silent);
			break;
		case RadioMode.silent:
			setRadioMode((!adminMode) ? RadioMode.activitudeMusic : RadioMode.adminMusic);
			break;
		}
	}

	private void setRadioMode(RadioMode newMode)
	{
		radioMode = newMode;
		switch (newMode)
		{
		case RadioMode.adminMusic:
			defaultBgmSource.Pause();
			adminBgmSource.Play();
			break;
		case RadioMode.activitudeMusic:
			defaultBgmSource.Play();
			adminBgmSource.Pause();
			break;
		case RadioMode.silent:
			defaultBgmSource.Pause();
			adminBgmSource.Pause();
			break;
		}
	}

	public void SetVolumeT(float t)
	{
		defaultBgmSource.volume = defaultBgmMaxVolume * t;
		adminBgmSource.volume = adminBgmMaxVolume * t;
	}
}
