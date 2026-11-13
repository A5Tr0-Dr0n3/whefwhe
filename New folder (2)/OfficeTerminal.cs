using UnityEngine;

public class OfficeTerminal : MonoBehaviour
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

	[SerializeField]
	private Renderer powerLight;

	[SerializeField]
	private ParticleSystem powerSparks;

	[SerializeField]
	private ParticleSystem deadSparks;

	[SerializeField]
	private MeshFilter lightOn;

	[SerializeField]
	private MeshFilter lightOff;

	[SerializeField]
	private MeshFilter lightIdle;

	[SerializeField]
	private MeshFilter lightDead;

	[SerializeField]
	private MeshFilter realLight;

	[SerializeField]
	private VVRConsumable[] consumableComponents;

	private ObjectPowerSwitch mSwitch;

	private bool firstTimeFull;

	[SerializeField]
	private GvrAudioSource fillSound;

	private void Awake()
	{
		consumableComponents = GetComponentsInChildren<VVRConsumable>();
		mSwitch = GetComponentInChildren<ObjectPowerSwitch>();
	}

	private void Update()
	{
		if (IsFull() && VVR_Offices.instance.GetAllTerminalsFull())
		{
			if (!firstTimeFull)
			{
				firstTimeFull = true;
				fillSound.Play();
				mSwitch.ResetHandle();
				deadSparks.Stop();
				changePowerState(ObjectPowerSwitch.leverState.idle);
			}
			if (mSwitch != null && mSwitch.currentLeverState != lastLeverState)
			{
				changePowerState(mSwitch.currentLeverState);
			}
		}
		else
		{
			if (realLight.sharedMesh != lightDead.sharedMesh)
			{
				realLight.sharedMesh = lightDead.sharedMesh;
			}
			if (!deadSparks.isPlaying)
			{
				deadSparks.Play();
			}
			firstTimeFull = false;
		}
	}

	private void changePowerState(ObjectPowerSwitch.leverState handleState)
	{
		lastLeverState = handleState;
		powerSparks.Play();
		switch (handleState)
		{
		case ObjectPowerSwitch.leverState.on:
			realLight.sharedMesh = lightOn.sharedMesh;
			powerState = PowerState.On;
			break;
		case ObjectPowerSwitch.leverState.idle:
			realLight.sharedMesh = lightIdle.sharedMesh;
			powerState = PowerState.Idle;
			break;
		case ObjectPowerSwitch.leverState.off:
			realLight.sharedMesh = lightOff.sharedMesh;
			powerState = PowerState.Off;
			break;
		}
	}

	public bool IsFull()
	{
		if (consumableComponents == null)
		{
			return true;
		}
		bool result = true;
		for (int i = 0; i < consumableComponents.Length; i++)
		{
			if (consumableComponents[i].currentObjectState != VVRConsumable.objectState.wholeStatic && consumableComponents[i].currentObjectState != VVRConsumable.objectState.wholeShaking)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public PowerState GetPowerState()
	{
		if (IsFull())
		{
			return powerState;
		}
		return PowerState.Idle;
	}
}
