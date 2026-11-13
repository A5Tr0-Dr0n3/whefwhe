using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VVR_SAVEDATA
{
	public DateTime lastDayPlayed = DateTime.Today;

	public StartingScene reloadScene;

	public TutorialLevelSequence.TutorialStates tutorialCompletion;

	public TutorialLevelSequence.AestheticObjects aestheticObject;

	public VVR_HUB.HubStates hubState;

	public VVR_HUB.JobStates jobState;

	public Backstage.BackstageStates backstageState;

	public bool hasBeenToOffice;

	public bool hasSetupStamper;

	public bool hasBeenToArchives;

	public VVR_Archive.ArchiveStates archiveState;

	public ElevatorSaveData elevatorData;

	public VVR_Hernandez.HernandezLevelState hernandezState;

	public int hernandezRestorationMonologueIndex;

	public VVR_Eliza.ElizaStates elizaState;

	public int lastHubInterruptIndex;

	public float savedVacuumValue;

	public bool hasVacuum;

	public bool hasInfoDesk;

	public int kitchenLoopCounter;

	public int backstageCaughtCounter;

	public SerializableDictionary<string, ConsumableSaveData[]> levelConsumableData;

	public SerializableDictionary<int, JobReview> jobReviews;

	public SerializableDictionary<string, int> arbitraryIntDictionary;

	public SerializableDictionary<string, string> arbitraryStringDictionary;

	public const string versionNumberStrKey = "VERSION NUMBER";

	public const string usernameStrKey = "USERNAME";

	public const string lastPlayedStrKey = "LASTPLAYED";

	public const string infoDeskLocKey = "INFO DESK LOCATION";

	public const string beeSlotIntKey = "BEE SLOT";

	public const string elizaCanyonKey = "ELIZA BALOON";

	public const string elizaSpaceKey = "ELIZA PLANET";

	public const string elizaCaveKey = "ELIZA CAVE";

	public const string elizaVolcanoKey = "ELIZA VOLCANO";

	public const string elizaSunsetKey = "ELIZA BEACH";

	public const string elizaEnvironmentsKey = "ELIZA ENVIRONMENTS FILLED";

	public const string suspiciousIndexKey = "SUSPICIOUS INTERRUPT";

	public const string genericIndexKey = "GENERIC INTERRUPT";

	public const string snackZoneIntroKey = "SNACK ZONE INTRO";

	public const string relaxationZoneIntroKey = "RELAXATION ZONE INTRO";

	public const string vrZoneIntroKey = "VR ZONE INTRO";

	public const string flipTutorialKey = "TRIPPLE TAP";

	public const string relaxationMonologueIndexKey = "RELAX BEE";

	public const string skipMergeCutsceneKey = "IVE BEEN HERE BEFORE";

	public List<string> playedCoroutines;

	[Header("Hub Prejob Interactions")]
	public int HRCage;

	public int chazAbuse;

	public int saveButton;

	[Header("Union Interactions")]
	public bool union3Completed;

	public bool stamperMessageHeard;

	public bool infoMessageHeard;

	public bool vacuumMessageHeard;

	public bool archiveMessageHeard;

	[HideInInspector]
	public bool canActivateDebug;

	[HideInInspector]
	public DebugPanelType debugPanelType;

	[Header("ADMIN")]
	public bool hasAdminAccess;

	[Header("Endgame Save Data")]
	public DateTime endGameTimerStart;

	public VVR_SAVEDATA(int beeSlotIn, bool fromScriptableObjectConstructor = false)
	{
		reloadScene = StartingScene.VVR_LVL_LOGIN;
		tutorialCompletion = TutorialLevelSequence.TutorialStates.Intro;
		canActivateDebug = false;
		elevatorData = new ElevatorSaveData();
		savedVacuumValue = 0f;
		jobReviews = new SerializableDictionary<int, JobReview>();
		levelConsumableData = new SerializableDictionary<string, ConsumableSaveData[]>();
		playedCoroutines = new List<string>();
		arbitraryIntDictionary = new SerializableDictionary<string, int>();
		arbitraryStringDictionary = new SerializableDictionary<string, string>();
		if (!fromScriptableObjectConstructor)
		{
			arbitraryStringDictionary["USERNAME"] = UsernameGenerator.GenerateUsername();
		}
		WriteMissingDictionaryValues(beeSlotIn, fromScriptableObjectConstructor);
	}

	public void WriteMissingDictionaryValues(int beeSlotIn = -1, bool fromScriptableObjectConstructor = false)
	{
		if (arbitraryIntDictionary == null)
		{
			arbitraryIntDictionary = new SerializableDictionary<string, int>();
		}
		if (arbitraryStringDictionary == null)
		{
			arbitraryStringDictionary = new SerializableDictionary<string, string>();
		}
		if (!fromScriptableObjectConstructor)
		{
			arbitraryStringDictionary["VERSION NUMBER"] = Application.version;
		}
		else
		{
			arbitraryStringDictionary["VERSION NUMBER"] = "Constructed from ScriptableObject; Version number not available";
		}
		if (!arbitraryStringDictionary.ContainsKey("USERNAME"))
		{
			arbitraryStringDictionary.Add("USERNAME", "727108-R1");
		}
		if (!arbitraryStringDictionary.ContainsKey("LASTPLAYED"))
		{
			arbitraryStringDictionary.Add("LASTPLAYED", DateTime.Now.ToString());
		}
		if (!arbitraryStringDictionary.ContainsKey("INFO DESK LOCATION"))
		{
			arbitraryStringDictionary.Add("INFO DESK LOCATION", "0f-0.35f7.5f");
		}
		if (beeSlotIn != -1 && !arbitraryIntDictionary.ContainsKey("BEE SLOT"))
		{
			arbitraryIntDictionary["BEE SLOT"] = beeSlotIn;
		}
		if (!arbitraryIntDictionary.ContainsKey("ELIZA BALOON"))
		{
			arbitraryIntDictionary.Add("ELIZA BALOON", 0);
		}
		if (!arbitraryIntDictionary.ContainsKey("ELIZA BEACH"))
		{
			arbitraryIntDictionary.Add("ELIZA BEACH", 0);
		}
		if (!arbitraryIntDictionary.ContainsKey("ELIZA CAVE"))
		{
			arbitraryIntDictionary.Add("ELIZA CAVE", 0);
		}
		if (!arbitraryIntDictionary.ContainsKey("ELIZA PLANET"))
		{
			arbitraryIntDictionary.Add("ELIZA PLANET", 0);
		}
		if (!arbitraryIntDictionary.ContainsKey("ELIZA VOLCANO"))
		{
			arbitraryIntDictionary.Add("ELIZA VOLCANO", 0);
		}
		if (!arbitraryIntDictionary.ContainsKey("ELIZA ENVIRONMENTS FILLED"))
		{
			arbitraryIntDictionary.Add("ELIZA ENVIRONMENTS FILLED", 0);
		}
		if (!arbitraryIntDictionary.ContainsKey("SUSPICIOUS INTERRUPT"))
		{
			arbitraryIntDictionary.Add("SUSPICIOUS INTERRUPT", 0);
		}
		if (!arbitraryIntDictionary.ContainsKey("GENERIC INTERRUPT"))
		{
			arbitraryIntDictionary.Add("GENERIC INTERRUPT", 0);
		}
	}

	public void WipeConsumables()
	{
		levelConsumableData = new SerializableDictionary<string, ConsumableSaveData[]>();
	}

	public string GetBeeSlotKey()
	{
		return "BEE SLOT";
	}
}
