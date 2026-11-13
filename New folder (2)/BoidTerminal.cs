using System.Collections.Generic;
using UnityEngine;

public class BoidTerminal : MonoBehaviour
{
	public enum PowerState
	{
		Off,
		Idle,
		On,
		Dead
	}

	[SerializeField]
	private PowerState powerState;

	private PowerState previousPowerState;

	private ObjectPowerSwitch.leverState lastLeverState;

	[Header("FX")]
	[SerializeField]
	private Renderer powerLight;

	[SerializeField]
	private ParticleSystem powerSparks;

	[SerializeField]
	private Material lightOn;

	[SerializeField]
	private Material lightOff;

	[SerializeField]
	private Material lightIdle;

	private ObjectPowerSwitch mSwitch;

	private bool firstTimeFull;

	[Header("SFX")]
	public AudioEvent switchSound;

	public GvrAudioSource music;

	private float currPitch = 1f;

	[Header("MOVEMENT")]
	public List<BoidController> boidz;

	public RotateObject spinner;

	[Header("ENIRONMENTS")]
	public ElevatorEnvironments environmentController;

	public ElizaEnvironmentProperties onEnvironment;

	public ElizaEnvironmentProperties idleEnvironment;

	public ElizaEnvironmentProperties offEnvironment;

	private void Awake()
	{
		mSwitch = GetComponentInChildren<ObjectPowerSwitch>();
	}

	private void Update()
	{
		if (!firstTimeFull)
		{
			firstTimeFull = true;
			changePowerState(ObjectPowerSwitch.leverState.idle);
			mSwitch.ResetHandle();
		}
		if (mSwitch != null && mSwitch.currentLeverState != lastLeverState)
		{
			changePowerState(mSwitch.currentLeverState);
			switchSound.Play();
		}
		if (Mathf.Abs(music.pitch - currPitch) > 0.01f)
		{
			music.pitch = Mathf.Lerp(music.pitch, currPitch, 1f * Time.deltaTime);
		}
	}

	private void changePowerState(ObjectPowerSwitch.leverState handleState)
	{
		lastLeverState = handleState;
		powerSparks.Play();
		switch (handleState)
		{
		case ObjectPowerSwitch.leverState.on:
		{
			environmentController.MatchEnvironment(1f, onEnvironment);
			spinner.rotation.y = 50f;
			currPitch = 2f;
			powerState = PowerState.On;
			powerLight.sharedMaterial = lightOn;
			for (int j = 0; j < boidz.Count; j++)
			{
				boidz[j].velocity = 8f;
			}
			break;
		}
		case ObjectPowerSwitch.leverState.idle:
			environmentController.MatchEnvironment(1f, idleEnvironment);
			spinner.rotation.y = 20f;
			currPitch = 1f;
			if (powerState == PowerState.Off)
			{
				for (int k = 0; k < boidz.Count; k++)
				{
					boidz[k].PowerUpAll();
				}
			}
			else
			{
				for (int l = 0; l < boidz.Count; l++)
				{
					boidz[l].velocity = 4f;
				}
			}
			powerState = PowerState.Idle;
			powerLight.sharedMaterial = lightIdle;
			break;
		case ObjectPowerSwitch.leverState.off:
		{
			environmentController.MatchEnvironment(1f, offEnvironment);
			spinner.rotation.y = 0f;
			currPitch = 0.5f;
			powerLight.sharedMaterial = lightOff;
			powerState = PowerState.Off;
			for (int i = 0; i < boidz.Count; i++)
			{
				boidz[i].PowerDownAll();
			}
			break;
		}
		}
	}
}
