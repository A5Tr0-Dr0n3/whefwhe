using System.Collections;
using GVR.Gfx;
using TButt;
using UnityEngine;

public class VVR_Archive : SceneBehaviorManager
{
	public enum ArchiveStates
	{
		Bernice,
		Hernandez,
		Eliza,
		ArchivesFinished
	}

	public static VVR_Archive instance;

	public UnionStapler stapler;

	public AudioClip message;

	private ArchiveStates archiveState;

	private bool firstActivation = true;

	[SerializeField]
	public EnvironmentProperties environment;

	[SerializeField]
	private GameObject elizaHeadset;

	[SerializeField]
	private Headset hernandezHeadset;

	[SerializeField]
	private VVR_Point elizaLight;

	[SerializeField]
	private VVR_Point hernandezLight;

	[SerializeField]
	private DesktopLight elizaDesktopLight;

	[SerializeField]
	private DesktopLight hernandezDesktopLight;

	[CurveRange(0f, 1f, -1f, 1f)]
	public AnimationCurve lightSpawnCurve;

	[SerializeField]
	private PostHernandezChazCaptureSequence captureSequence;

	[SerializeField]
	private float lightRangeSmall = 1.9f;

	[SerializeField]
	private float lightRangeLarge = 5f;

	private float hernandezLightIntensity;

	private float elizaLightIntensity;

	private bool captureSequenceArmed;

	private void Awake()
	{
		if (instance != null)
		{
			throw new UnityException("Only one instance of VVR_Archive in a scene is allowed.");
		}
		instance = this;
	}

	protected override void OnFirstTimeSceneActivated()
	{
		if (!PersistentDataManager.GetHasBeenToArchives())
		{
			PersistentDataManager.MarkHasBeenToArchives();
			PersistentDataManager.OverwriteInfoDeskState(gotInfoDesk: true);
			PersistentDataManager.WriteSaveDelay(1f);
		}
		archiveState = PersistentDataManager.GetArchiveState();
		elizaLightIntensity = elizaLight.intensity;
		hernandezLightIntensity = hernandezLight.intensity;
		StartArchive();
		firstActivation = false;
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_LVL_JAIL);
		if (!PersistentDataManager.GetArchiveMessageHeard())
		{
			stapler.RingStapler(message, UnionStapler.StaplerLocation.Archive, 5f);
		}
	}

	protected override void OnSceneActivated()
	{
		if (captureSequenceArmed)
		{
			captureSequence.BeginCaptureSequence();
			return;
		}
		captureSequence.Deactivate();
		if (!firstActivation && archiveState != PersistentDataManager.GetArchiveState() && !EagerHeadsetManager.GetPlayerLockedInScene())
		{
			StartArchive();
		}
	}

	public override void OnSceneDeactivated()
	{
		base.OnSceneDeactivated();
		Teleporter.SetTeleportEnabled(enabledIn: true);
	}

	private void StartArchive()
	{
		archiveState = PersistentDataManager.GetArchiveState();
		SafeDebug.Log(archiveState.ToString());
		elizaHeadset.SetActive(value: false);
		hernandezHeadset.gameObject.SetActive(value: false);
		hernandezHeadset.gameObject.SetActive(value: false);
		elizaLight.intensity = 0f;
		hernandezLight.intensity = 0f;
		if (hernandezDesktopLight.mLight == null || elizaDesktopLight == null)
		{
			GlobalCoroutineContainer.AddThread(setupDesktopLightsOnDelay(), this);
		}
		else
		{
			hernandezDesktopLight.mLight.intensity = 0f;
			elizaDesktopLight.mLight.intensity = 0f;
		}
		switch (archiveState)
		{
		case ArchiveStates.Bernice:
			ArchiveBernice();
			break;
		case ArchiveStates.Eliza:
			ArchiveEliza();
			break;
		case ArchiveStates.Hernandez:
			ArchiveHernandez();
			break;
		case ArchiveStates.ArchivesFinished:
			ArchiveHR();
			break;
		}
	}

	private IEnumerator setupDesktopLightsOnDelay()
	{
		while (elizaDesktopLight.mLight == null)
		{
			yield return null;
		}
		while (hernandezDesktopLight.mLight == null)
		{
			yield return null;
		}
		elizaDesktopLight.mLight.intensity = 0f;
		hernandezDesktopLight.mLight.intensity = 0f;
	}

	private void ArchiveBernice()
	{
		GlobalCoroutineContainer.AddThread(ArchiveIntroCoroutine(), this);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_ARCHIVE_WHALE);
	}

	private IEnumerator ArchiveIntroCoroutine()
	{
		yield return null;
	}

	private void ArchiveHernandez()
	{
		EnableHernandez();
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_ARCHIVE_WHALE);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_ARCHIVE_HERNANDEZ);
	}

	private void ArchiveEliza()
	{
		EnableEliza();
		EnableHernandez();
		FinishHernandez();
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_ARCHIVE_WHALE);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_ARCHIVE_HERNANDEZ_POSTDEATH);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_ARCHIVE_ELIZA);
	}

	private void ArchiveHR()
	{
		FinishEliza();
		FinishHernandez();
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_ARCHIVE_WHALE);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_ARCHIVE_HERNANDEZ_POSTDEATH);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_ARCHIVE_ELIZA);
	}

	private void EnableEliza()
	{
		if (TBCore.GetActivePlatform() != VRPlatform.OculusMobile)
		{
			spawnDesktopLight(elizaDesktopLight, 2f);
		}
		elizaLight.intensity = elizaLightIntensity;
		elizaDesktopLight.mLight.intensity = elizaLightIntensity;
		environment.pushPointLights();
		elizaHeadset.SetActive(value: true);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_ARCHIVE_ELIZA);
	}

	private void EnableHernandez()
	{
		if (TBCore.GetActivePlatform() != VRPlatform.OculusMobile)
		{
			spawnDesktopLight(hernandezDesktopLight, 2f);
		}
		hernandezLight.intensity = hernandezLightIntensity;
		hernandezDesktopLight.mLight.intensity = hernandezLightIntensity;
		environment.pushPointLights();
		hernandezHeadset.gameObject.SetActive(value: true);
	}

	private void FinishEliza()
	{
		elizaLight.intensity = elizaLightIntensity;
		elizaLight.range = lightRangeLarge;
		elizaDesktopLight.mLight.intensity = elizaLightIntensity;
		elizaDesktopLight.mLight.range = lightRangeLarge;
		environment.pushPointLights();
	}

	private void FinishHernandez()
	{
		hernandezHeadset.Scene = TCScenes.S.VVR_ARCHIVE_HERNANDEZ_POSTDEATH;
		hernandezLight.range = lightRangeLarge;
		hernandezLight.intensity = hernandezLightIntensity;
		hernandezDesktopLight.mLight.range = lightRangeLarge;
		hernandezDesktopLight.mLight.intensity = hernandezLightIntensity;
		environment.pushPointLights();
	}

	private void spawnLight(VVR_Point light, float duration)
	{
		GlobalCoroutineContainer.AddThread(spawnLightCoroutine(light, duration), this);
	}

	private void spawnDesktopLight(DesktopLight light, float duration)
	{
		if (light == null)
		{
			Debug.LogError("Light passed in is null");
		}
		else
		{
			GlobalCoroutineContainer.AddThread(spawnDesktopLightCoroutine(light, duration), this);
		}
	}

	public static void ArmChazCaptureSequence()
	{
		instance.captureSequenceArmed = true;
	}

	public static void DisarmChazCaptureSequence()
	{
		instance.captureSequenceArmed = false;
	}

	private IEnumerator spawnLightCoroutine(VVR_Point light, float duration)
	{
		light.gameObject.SetActive(value: true);
		duration = Mathf.Clamp(duration, 0f, 10f);
		float orgRange = lightRangeSmall;
		float time = 0f;
		yield return new WaitForSeconds(2f);
		while (time < duration)
		{
			light.range = orgRange * (lightSpawnCurve.Evaluate(time / duration) + 1f);
			environment.pushPointLights();
			time += Time.deltaTime;
			yield return null;
		}
		yield return null;
	}

	private IEnumerator spawnDesktopLightCoroutine(DesktopLight light, float duration)
	{
		light.gameObject.SetActive(value: true);
		duration = Mathf.Clamp(duration, 0f, 10f);
		float endRange = light._range;
		float endIntensity = light._intensity;
		float time = 0f;
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
		while (time < duration)
		{
			light.mLight.range = endRange * (lightSpawnCurve.Evaluate(time / duration) + 1f);
			light.mLight.intensity = endIntensity * (lightSpawnCurve.Evaluate(time / duration) + 1f);
			time += Time.deltaTime;
			yield return null;
		}
	}
}
