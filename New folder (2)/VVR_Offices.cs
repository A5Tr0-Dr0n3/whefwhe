using UnityEngine;

public class VVR_Offices : SceneBehaviorManager
{
	public UnionStapler stapler;

	public AudioClip message;

	[SerializeField]
	private OfficeTerminal[] allOfficeTerminals;

	public static VVR_Offices instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		loadAllOfficeHeadsets();
		PersistentDataManager.MarkHasBeenToOffice();
		if (!PersistentDataManager.GetStamperMessageHeard())
		{
			stapler.RingStapler(message, UnionStapler.StaplerLocation.Stamper, 5f);
		}
	}

	private void loadAllOfficeHeadsets()
	{
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_STAMPER_01_FLAMINGOS);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_STAMPER_02_FISH);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_STAMPER_04_MINECRAFT);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_STAMPER_05_BEACH);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_STAMPER_06_TURBINE);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_STAMPER_07_PRINTING);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_STAMPER_08_LASERS);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_STAMPER_09_PHONEBOOTH);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_STAMPER_10_DEADCHAZ);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_STAMPER_13_SOCCER);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_STAMPER_14_CTHULHU);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_STAMPER_15_SPOTLIGHTS);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_STAMPER_16_BOIDSWITCH);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_STAMPER_19_BAIT);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_STAMPER_20_SKULL);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_STAMPER_21_FAUST);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_STAMPER_23_PLUGS);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_STAMPER_25_FIREWATCH);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_ARCHIVE);
	}

	public bool GetAllTerminalsFull()
	{
		bool result = true;
		for (int i = 0; i < allOfficeTerminals.Length; i++)
		{
			if (!allOfficeTerminals[i].IsFull())
			{
				result = false;
			}
		}
		return result;
	}
}
