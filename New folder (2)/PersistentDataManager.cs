using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentDataManager
{
	public delegate void OnSaveSlotChangeHandler(bool saveDataAvailable);

	public delegate void OnLanguageChangeHandler();

	private static int activeSaveSlot = -1;

	private static VVR_SAVEDATA _savedDataInternal;

	private static VVR_METASAVEDATA _savedMetadataInternal;

	private static VVR_SAVEDATA cachedSavedData
	{
		get
		{
			if (_savedDataInternal == null)
			{
				_savedDataInternal = SaveLoadUtility.LoadFromSlot(activeSaveSlot, writeBlankOnFailure: false);
			}
			return _savedDataInternal;
		}
		set
		{
			_savedDataInternal = value;
		}
	}

	private static VVR_METASAVEDATA cachedMetaSaveData
	{
		get
		{
			if (_savedMetadataInternal == null)
			{
				_savedMetadataInternal = MetaSaveLoadUtility.Load();
				if (_savedMetadataInternal.subtitlesState == VVR_METASAVEDATA.SubtitlesState.NUMELEMENTS)
				{
					OverwriteSubtitlesState(MetaSaveLoadUtility.SysLangToSubState());
				}
			}
			return _savedMetadataInternal;
		}
		set
		{
			_savedMetadataInternal = value;
		}
	}

	public static event OnSaveSlotChangeHandler onSaveSlotChange;

	public static event OnLanguageChangeHandler onLanguageChange;

	public static int GetActiveSaveSlot()
	{
		return activeSaveSlot;
	}

	public static void CreateNewAccount()
	{
		int num = SaveLoadUtility.CreateNewAccount(immediatelyWriteToDisk: false, out var newAccountData);
		SetActiveSaveSlot(num);
		cachedSavedData = newAccountData;
	}

	public static void CreateNewSceneAccount(bool pastSaveThreshold)
	{
		CreateNewAccount();
		if (pastSaveThreshold)
		{
			cachedSavedData.tutorialCompletion = TutorialLevelSequence.TutorialStates.Move;
		}
		WriteActiveSaveToDisk();
	}

	public static void SetActiveSaveSlot(int slotIndex)
	{
		if (!SaveLoadUtility.IsValidSaveSlot(slotIndex) && slotIndex != -1)
		{
			SafeDebug.LogError("Trying to choose invalid save slot " + slotIndex + ", which is outside the valid range of 0 and " + 2 + "(-1, \"undetermined slot,\" is also tolerated)");
			return;
		}
		int num = activeSaveSlot;
		activeSaveSlot = slotIndex;
		SafeDebug.Log("<color=magenta>Swapped save slot from " + num + " to " + slotIndex + "</color>");
		if (num == slotIndex)
		{
			return;
		}
		cachedSavedData = SaveLoadUtility.LoadFromSlot(activeSaveSlot, writeBlankOnFailure: false);
		if (onSaveSlotChange != null)
		{
			onSaveSlotChange(HasAccountLoaded() && IsAccountPastSaveThreshold());
			if (cachedSavedData != null)
			{
				cachedSavedData.WriteMissingDictionaryValues();
			}
		}
	}

	public static bool HasAccountLoaded()
	{
		return SaveLoadUtility.IsValidSaveSlot(GetActiveSaveSlot());
	}

	public static bool IsAccountPastSaveThreshold()
	{
		if (activeSaveSlot == -1 || cachedSavedData == null)
		{
			return false;
		}
		return GetSavedTutorialState() >= TutorialLevelSequence.TutorialStates.Move;
	}

	public static void ReplaceActiveSaveData(VVR_SAVEDATA newData)
	{
		cachedSavedData = newData;
		SaveLoadUtility.SaveToSlot(newData, activeSaveSlot);
	}

	public static void WriteActiveSaveToDisk()
	{
		if (IsAccountPastSaveThreshold())
		{
			UpdateLastDatePlayed();
			SaveLoadUtility.SaveToSlot(cachedSavedData, activeSaveSlot);
		}
		else
		{
			SafeDebug.Log("<color=#fa8072>Skipping save write on save file which is not past the save threshold</color>");
		}
	}

	public static void DeleteActiveSaveData()
	{
		SaveLoadUtility.DeleteAtSlot(activeSaveSlot);
		SetActiveSaveSlot(-1);
	}

	public static void WriteSaveDelay(float delay)
	{
		SaveLoadRuntimeWatcher.DelaySave(delay);
	}

	public static void TidySaveValues(EagerScene activeScene)
	{
		if (activeScene != null && activeScene.mConsumableSyncer != null)
		{
			activeScene.mConsumableSyncer.SaveConsumables();
		}
		if (EagerMaster.instance != null)
		{
			OverwriteVacuumValue(EagerMaster.instance.vacuumFill);
		}
	}

	public static void PreWarmMetaSaveFile()
	{
		VVR_METASAVEDATA.SubtitlesState subtitlesState = cachedMetaSaveData.subtitlesState;
		cachedMetaSaveData.subtitlesState = subtitlesState;
		MetaSaveLoadUtility.Save(cachedMetaSaveData);
	}

	public static bool GetCanActivateDebug()
	{
		return cachedMetaSaveData.canActivateDebug;
	}

	public static DebugPanelType GetDebugPanelType()
	{
		return cachedMetaSaveData.debugPanelType;
	}

	public static VVR_METASAVEDATA.SubtitlesState GetSubtitleLanguage()
	{
		return cachedMetaSaveData.subtitlesState;
	}

	public static void OverwriteSubtitlesState(VVR_METASAVEDATA.SubtitlesState s)
	{
		cachedMetaSaveData.subtitlesState = s;
		if (onLanguageChange != null)
		{
			onLanguageChange();
		}
		SafeDebug.Log("New subtitle language is: " + cachedMetaSaveData.subtitlesState);
	}

	public static void ToggleSubtitles()
	{
		if (cachedMetaSaveData.subtitlesState != VVR_METASAVEDATA.SubtitlesState.ENGLISH)
		{
			OverwriteSubtitlesState(VVR_METASAVEDATA.SubtitlesState.ENGLISH);
		}
		else
		{
			OverwriteSubtitlesState(VVR_METASAVEDATA.SubtitlesState.NONE);
		}
		MetaSaveLoadUtility.Save(cachedMetaSaveData);
	}

	public static void SetUseHacktivitudeMenu(bool useHacktivitude)
	{
		cachedMetaSaveData.useHacktivitudeMenuFlag = useHacktivitude;
		MetaSaveLoadUtility.Save(cachedMetaSaveData);
	}

	public static bool GetUseHacktivitudeMenu()
	{
		return cachedMetaSaveData.useHacktivitudeMenuFlag;
	}

	public static bool NeedBackup()
	{
		return Application.isPlaying && cachedMetaSaveData.lastBackedUpVersionNumber != Application.version;
	}

	public static string GetLastBackedUpVersionNumber()
	{
		return cachedMetaSaveData.lastBackedUpVersionNumber;
	}

	public static void MarkBackedUp()
	{
		cachedMetaSaveData.lastBackedUpVersionNumber = Application.version;
		MetaSaveLoadUtility.Save(cachedMetaSaveData);
	}

	public static StandaloneGraphicsMode GetGraphicsMode()
	{
		return cachedMetaSaveData.graphicsMode;
	}

	public static void OverwriteGraphicsMode(StandaloneGraphicsMode s)
	{
		cachedMetaSaveData.graphicsMode = s;
		MetaSaveLoadUtility.Save(cachedMetaSaveData);
		SafeDebug.Log("New graphics setting is: " + cachedMetaSaveData.graphicsMode);
	}

	public static void OverwriteLastLoadedScene(TCPlatformScene lastLoadedScene)
	{
		StartingScene reloadScene = (StartingScene)Enum.Parse(typeof(StartingScene), lastLoadedScene.Identifier);
		cachedSavedData.reloadScene = reloadScene;
	}

	[Obsolete("Use GetLastLoadedScene instead!")]
	public static string GetLastLoadedSceneName()
	{
		if (HasAccountLoaded())
		{
			return cachedSavedData.reloadScene.ToString();
		}
		return EagerScenes.LOGIN_LEVEL.Name;
	}

	public static TCPlatformScene GetLastLoadedScene()
	{
		if (HasAccountLoaded())
		{
			return TCScenes.SceneFromIdentifier(cachedSavedData.reloadScene.ToString());
		}
		return EagerScenes.LOGIN_LEVEL;
	}

	public static DateTime GetLastDayPlayed()
	{
		return cachedSavedData.lastDayPlayed;
	}

	public static string GetVersionNumber()
	{
		return cachedSavedData.arbitraryStringDictionary["VERSION NUMBER"];
	}

	public static string GetUsername()
	{
		return cachedSavedData.arbitraryStringDictionary["USERNAME"];
	}

	public static Vector3 GetInfoDeskPosition()
	{
		float[] array = new float[3];
		int num = 0;
		string text = cachedSavedData.arbitraryStringDictionary["INFO DESK LOCATION"];
		for (int i = 0; i < array.Length; i++)
		{
			num = text.IndexOf("f");
			array[i] = float.Parse(text.Substring(0, num));
			if (num + 1 < text.Length)
			{
				text = text.Substring(num + 1);
			}
		}
		return new Vector3(array[0], array[1], array[2]);
	}

	public static void SetInfoDeskPosition(Vector3 pos)
	{
		cachedSavedData.arbitraryStringDictionary["INFO DESK LOCATION"] = string.Concat(pos.x + "f" + pos.y + "f" + pos.z + "f");
	}

	public static string GetLastTimePlayedString()
	{
		return cachedSavedData.arbitraryStringDictionary["LASTPLAYED"];
	}

	public static void UpdateLastDatePlayed()
	{
		cachedSavedData.lastDayPlayed = DateTime.Now.Date;
		if (cachedSavedData.arbitraryStringDictionary.ContainsKey("LASTPLAYED"))
		{
			cachedSavedData.arbitraryStringDictionary["LASTPLAYED"] = DateTime.Now.Date.ToString();
		}
		else
		{
			cachedSavedData.arbitraryStringDictionary.Add("LASTPLAYED", DateTime.Now.Date.ToString());
		}
	}

	public static int GetBeeslot()
	{
		return cachedSavedData.arbitraryIntDictionary[cachedSavedData.GetBeeSlotKey()];
	}

	public static int GetElizaCanyonState()
	{
		if (cachedSavedData.arbitraryIntDictionary.ContainsKey("ELIZA BALOON"))
		{
			return cachedSavedData.arbitraryIntDictionary["ELIZA BALOON"];
		}
		cachedSavedData.arbitraryIntDictionary.Add("ELIZA BALOON", 0);
		return 0;
	}

	public static int GetElizaSunsetState()
	{
		if (cachedSavedData.arbitraryIntDictionary.ContainsKey("ELIZA BEACH"))
		{
			return cachedSavedData.arbitraryIntDictionary["ELIZA BEACH"];
		}
		cachedSavedData.arbitraryIntDictionary.Add("ELIZA BEACH", 0);
		return 0;
	}

	public static int GetElizaCaveState()
	{
		if (cachedSavedData.arbitraryIntDictionary.ContainsKey("ELIZA CAVE"))
		{
			return cachedSavedData.arbitraryIntDictionary["ELIZA CAVE"];
		}
		cachedSavedData.arbitraryIntDictionary.Add("ELIZA CAVE", 0);
		return 0;
	}

	public static int GetElizaSpaceState()
	{
		if (cachedSavedData.arbitraryIntDictionary.ContainsKey("ELIZA PLANET"))
		{
			return cachedSavedData.arbitraryIntDictionary["ELIZA PLANET"];
		}
		cachedSavedData.arbitraryIntDictionary.Add("ELIZA PLANET", 0);
		return 0;
	}

	public static int GetElizaVolcanoState()
	{
		if (cachedSavedData.arbitraryIntDictionary.ContainsKey("ELIZA VOLCANO"))
		{
			return cachedSavedData.arbitraryIntDictionary["ELIZA VOLCANO"];
		}
		cachedSavedData.arbitraryIntDictionary.Add("ELIZA VOLCANO", 0);
		return 0;
	}

	public static int GetElizaEnvironmentsFilled()
	{
		if (cachedSavedData.arbitraryIntDictionary.ContainsKey("ELIZA ENVIRONMENTS FILLED"))
		{
			return cachedSavedData.arbitraryIntDictionary["ELIZA ENVIRONMENTS FILLED"];
		}
		cachedSavedData.arbitraryIntDictionary.Add("ELIZA ENVIRONMENTS FILLED", 0);
		return 0;
	}

	public static void SetElizaCanyonState(int s)
	{
		if (cachedSavedData.arbitraryIntDictionary.ContainsKey("ELIZA BALOON"))
		{
			cachedSavedData.arbitraryIntDictionary["ELIZA BALOON"] = s;
		}
		else
		{
			cachedSavedData.arbitraryIntDictionary.Add("ELIZA BALOON", s);
		}
	}

	public static void SetElizaSunsetState(int s)
	{
		if (cachedSavedData.arbitraryIntDictionary.ContainsKey("ELIZA BEACH"))
		{
			cachedSavedData.arbitraryIntDictionary["ELIZA BEACH"] = s;
		}
		else
		{
			cachedSavedData.arbitraryIntDictionary.Add("ELIZA BEACH", s);
		}
	}

	public static void SetElizaCaveState(int s)
	{
		if (cachedSavedData.arbitraryIntDictionary.ContainsKey("ELIZA CAVE"))
		{
			cachedSavedData.arbitraryIntDictionary["ELIZA CAVE"] = s;
		}
		else
		{
			cachedSavedData.arbitraryIntDictionary.Add("ELIZA CAVE", s);
		}
	}

	public static void SetElizaSpaceState(int s)
	{
		if (cachedSavedData.arbitraryIntDictionary.ContainsKey("ELIZA PLANET"))
		{
			cachedSavedData.arbitraryIntDictionary["ELIZA PLANET"] = s;
		}
		else
		{
			cachedSavedData.arbitraryIntDictionary.Add("ELIZA PLANET", s);
		}
	}

	public static void SetElizaVolcanoState(int s)
	{
		if (cachedSavedData.arbitraryIntDictionary.ContainsKey("ELIZA VOLCANO"))
		{
			cachedSavedData.arbitraryIntDictionary["ELIZA VOLCANO"] = s;
		}
		else
		{
			cachedSavedData.arbitraryIntDictionary.Add("ELIZA VOLCANO", s);
		}
	}

	public static void SetElizaEnvironmentsFilled(int s)
	{
		if (cachedSavedData.arbitraryIntDictionary.ContainsKey("ELIZA ENVIRONMENTS FILLED"))
		{
			cachedSavedData.arbitraryIntDictionary["ELIZA ENVIRONMENTS FILLED"] = s;
		}
		else
		{
			cachedSavedData.arbitraryIntDictionary.Add("ELIZA ENVIRONMENTS FILLED", s);
		}
	}

	public static int GetSuspiciousIndex()
	{
		if (cachedSavedData.arbitraryIntDictionary.ContainsKey("SUSPICIOUS INTERRUPT"))
		{
			return cachedSavedData.arbitraryIntDictionary["SUSPICIOUS INTERRUPT"];
		}
		return 0;
	}

	public static int GetGenericIndex()
	{
		if (cachedSavedData.arbitraryIntDictionary.ContainsKey("GENERIC INTERRUPT"))
		{
			return cachedSavedData.arbitraryIntDictionary["GENERIC INTERRUPT"];
		}
		return 0;
	}

	public static void OverwriteSuspiciousIndex(int s)
	{
		cachedSavedData.arbitraryIntDictionary["SUSPICIOUS INTERRUPT"] = s;
	}

	public static void OverwriteGenericIndex(int s)
	{
		cachedSavedData.arbitraryIntDictionary["GENERIC INTERRUPT"] = s;
	}

	public static void OverwriteSnackZoneIntro(int s)
	{
		if (cachedSavedData.arbitraryIntDictionary.ContainsKey("SNACK ZONE INTRO"))
		{
			cachedSavedData.arbitraryIntDictionary["SNACK ZONE INTRO"] = s;
		}
		else
		{
			cachedSavedData.arbitraryIntDictionary.Add("SNACK ZONE INTRO", s);
		}
	}

	public static bool GetSnackZoneIntro()
	{
		if (cachedSavedData != null && cachedSavedData.arbitraryIntDictionary.ContainsKey("SNACK ZONE INTRO"))
		{
			return cachedSavedData.arbitraryIntDictionary["SNACK ZONE INTRO"] == 1;
		}
		return false;
	}

	public static void OverwriteRelaxationZoneIntro(int s)
	{
		if (cachedSavedData.arbitraryIntDictionary.ContainsKey("RELAXATION ZONE INTRO"))
		{
			cachedSavedData.arbitraryIntDictionary["RELAXATION ZONE INTRO"] = s;
		}
		else
		{
			cachedSavedData.arbitraryIntDictionary.Add("RELAXATION ZONE INTRO", s);
		}
	}

	public static bool GetRelaxationZoneIntro()
	{
		if (cachedSavedData != null && cachedSavedData.arbitraryIntDictionary.ContainsKey("RELAXATION ZONE INTRO"))
		{
			return cachedSavedData.arbitraryIntDictionary["RELAXATION ZONE INTRO"] == 1;
		}
		return false;
	}

	public static void OverwriteVrZoneIntro(int s)
	{
		if (cachedSavedData.arbitraryIntDictionary.ContainsKey("VR ZONE INTRO"))
		{
			cachedSavedData.arbitraryIntDictionary["VR ZONE INTRO"] = s;
		}
		else
		{
			cachedSavedData.arbitraryIntDictionary.Add("VR ZONE INTRO", s);
		}
	}

	public static bool GetVrZoneIntro()
	{
		if (cachedSavedData != null && cachedSavedData.arbitraryIntDictionary.ContainsKey("VR ZONE INTRO"))
		{
			return cachedSavedData.arbitraryIntDictionary["VR ZONE INTRO"] == 1;
		}
		return false;
	}

	public static void OverwriteRelaxationZoneMonologueIndex(int s)
	{
		if (cachedSavedData.arbitraryIntDictionary.ContainsKey("RELAX BEE"))
		{
			cachedSavedData.arbitraryIntDictionary["RELAX BEE"] = s;
		}
		else
		{
			cachedSavedData.arbitraryIntDictionary.Add("RELAX BEE", s);
		}
	}

	public static int GetRelaxationZoneMonologueIndex()
	{
		if (cachedSavedData.arbitraryIntDictionary.ContainsKey("RELAX BEE"))
		{
			return cachedSavedData.arbitraryIntDictionary["RELAX BEE"];
		}
		return 0;
	}

	public static void OverwriteHasDoneFlipTutorial(int s)
	{
		if (!cachedSavedData.arbitraryIntDictionary.ContainsKey("TRIPPLE TAP"))
		{
			cachedSavedData.arbitraryIntDictionary.Add("TRIPPLE TAP", s);
		}
		else
		{
			cachedSavedData.arbitraryIntDictionary["TRIPPLE TAP"] = s;
		}
	}

	public static bool GetHasDoneFlipTutorial()
	{
		if (!cachedSavedData.arbitraryIntDictionary.ContainsKey("TRIPPLE TAP"))
		{
			return false;
		}
		return cachedSavedData.arbitraryIntDictionary["TRIPPLE TAP"] == 1;
	}

	public static void OverwriteCanSkipMerge(bool s)
	{
		if (cachedSavedData.arbitraryIntDictionary.ContainsKey("IVE BEEN HERE BEFORE"))
		{
			if (s)
			{
				cachedSavedData.arbitraryIntDictionary["IVE BEEN HERE BEFORE"] = 1;
			}
			else
			{
				cachedSavedData.arbitraryIntDictionary["IVE BEEN HERE BEFORE"] = 0;
			}
		}
		else if (s)
		{
			cachedSavedData.arbitraryIntDictionary.Add("IVE BEEN HERE BEFORE", 1);
		}
		else
		{
			cachedSavedData.arbitraryIntDictionary.Add("IVE BEEN HERE BEFORE", 0);
		}
	}

	public static bool GetCanSkipMerge()
	{
		if (cachedSavedData.arbitraryIntDictionary.ContainsKey("IVE BEEN HERE BEFORE"))
		{
			if (cachedSavedData.arbitraryIntDictionary["IVE BEEN HERE BEFORE"] == 1)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public static TutorialLevelSequence.TutorialStates GetSavedTutorialState()
	{
		return cachedSavedData.tutorialCompletion;
	}

	public static void OverwriteTutorialState(TutorialLevelSequence.TutorialStates stateIn)
	{
		cachedSavedData.tutorialCompletion = stateIn;
	}

	public static VVR_HUB.HubStates GetSavedHubState()
	{
		return cachedSavedData.hubState;
	}

	public static void OverwriteHubState(VVR_HUB.HubStates stateIn)
	{
		cachedSavedData.hubState = stateIn;
	}

	public static TutorialLevelSequence.AestheticObjects GetAestheticObject()
	{
		return cachedSavedData.aestheticObject;
	}

	public static void OverwriteAestheticObject(TutorialLevelSequence.AestheticObjects stateIn)
	{
		cachedSavedData.aestheticObject = stateIn;
	}

	public static Backstage.BackstageStates GetBackstageState()
	{
		return cachedSavedData.backstageState;
	}

	public static void OverwriteBackstageState(Backstage.BackstageStates stateIn)
	{
		cachedSavedData.backstageState = stateIn;
	}

	public static VVR_HUB.JobStates GetJobState()
	{
		return cachedSavedData.jobState;
	}

	public static void OverwriteJobState(VVR_HUB.JobStates stateIn)
	{
		cachedSavedData.jobState = stateIn;
	}

	public static bool GetHasBeenToArchives()
	{
		return cachedSavedData.hasBeenToArchives;
	}

	public static void MarkHasBeenToArchives()
	{
		cachedSavedData.hasBeenToArchives = true;
	}

	public static VVR_Archive.ArchiveStates GetArchiveState()
	{
		return cachedSavedData.archiveState;
	}

	public static void OverwriteArchiveState(VVR_Archive.ArchiveStates stateIn)
	{
		cachedSavedData.archiveState = stateIn;
	}

	public static VVR_Hernandez.HernandezLevelState GetHernandezState()
	{
		return cachedSavedData.hernandezState;
	}

	public static void OverwriteHernandezState(VVR_Hernandez.HernandezLevelState stateIn)
	{
		cachedSavedData.hernandezState = stateIn;
	}

	public static int GetHernandezRestorationMonologueIndex()
	{
		return cachedSavedData.hernandezRestorationMonologueIndex;
	}

	public static void SetHernandezRestorationMonologueIndex(int newIndex)
	{
		cachedSavedData.hernandezRestorationMonologueIndex = newIndex;
	}

	public static bool GetVacuumState()
	{
		return cachedSavedData.hasVacuum;
	}

	public static void OverwriteInfoDeskState(bool gotInfoDesk)
	{
		cachedSavedData.hasInfoDesk = gotInfoDesk;
	}

	public static bool GetInfoDeskState()
	{
		return cachedSavedData.hasInfoDesk;
	}

	public static void OverwriteElevatorData(ElevatorSaveData dataIn)
	{
		cachedSavedData.elevatorData = dataIn;
	}

	public static ElevatorSaveData GetElevatorData()
	{
		return cachedSavedData.elevatorData;
	}

	public static void SetElevatorKey(ElevatorSaveData.KeyStates state)
	{
		cachedSavedData.elevatorData.keyState = state;
	}

	public static void OverwriteVacuumValue(float newValue)
	{
		cachedSavedData.savedVacuumValue = newValue;
	}

	public static float GetSavedVacuumValue()
	{
		return cachedSavedData.savedVacuumValue;
	}

	public static void OverwriteKitchenLoopCounter(int counter)
	{
		cachedSavedData.kitchenLoopCounter = counter;
	}

	public static void IncrementKitchenLoopCounter()
	{
		cachedSavedData.kitchenLoopCounter++;
	}

	public static int GetKitchenLoopCounter()
	{
		return cachedSavedData.kitchenLoopCounter;
	}

	public static void IncrementBackstageCaughtCounter()
	{
		cachedSavedData.backstageCaughtCounter++;
	}

	public static int GetBackstageCaughtCounter()
	{
		return cachedSavedData.backstageCaughtCounter;
	}

	public static void OverwriteElizaData(VVR_Eliza.ElizaStates newState)
	{
		cachedSavedData.elizaState = newState;
	}

	public static VVR_Eliza.ElizaStates GetElizaData()
	{
		return cachedSavedData.elizaState;
	}

	public static SerializableDictionary<int, JobReview> GetJobReviews()
	{
		return cachedSavedData.jobReviews;
	}

	public static void OverwriteJobReviews(SerializableDictionary<int, JobReview> reviewsIn)
	{
		cachedSavedData.jobReviews = reviewsIn;
	}

	public static ConsumableSaveData[] GetLevelConsumableData(string levelName)
	{
		if (cachedSavedData.levelConsumableData == null)
		{
			cachedSavedData.levelConsumableData = new SerializableDictionary<string, ConsumableSaveData[]>();
		}
		if (cachedSavedData.levelConsumableData.ContainsKey(levelName))
		{
			return cachedSavedData.levelConsumableData[levelName];
		}
		return null;
	}

	public static void PushConsumableData(string levelName, ConsumableSaveData[] consumableData)
	{
		if (cachedSavedData.levelConsumableData == null)
		{
			cachedSavedData.levelConsumableData = new SerializableDictionary<string, ConsumableSaveData[]>();
		}
		if (cachedSavedData.levelConsumableData.ContainsKey(levelName))
		{
			cachedSavedData.levelConsumableData[levelName] = consumableData;
		}
		else
		{
			cachedSavedData.levelConsumableData.Add(levelName, consumableData);
		}
	}

	public static void OverwriteHasVaccum(bool s)
	{
		cachedSavedData.hasVacuum = s;
	}

	public static void OverwriteHasAdmin(bool hasAdminIn)
	{
		cachedSavedData.hasAdminAccess = hasAdminIn;
		if (hasAdminIn)
		{
			EagerScenes.CheckLoadUnderhubScenes();
			HeadsetRemovalUIManager.instance.canRemove = true;
		}
	}

	public static bool GetHasAdmin()
	{
		return cachedSavedData.hasAdminAccess;
	}

	public static void MarkHasBeenToOffice()
	{
		cachedSavedData.hasBeenToOffice = true;
	}

	public static bool GetHasBeenToOffice()
	{
		return cachedSavedData.hasBeenToOffice;
	}

	public static void MarkUnion3Completed()
	{
		cachedSavedData.union3Completed = true;
	}

	public static bool GetUnion3Completed()
	{
		return cachedSavedData.union3Completed;
	}

	public static void MarkStamperMessageHeard()
	{
		cachedSavedData.stamperMessageHeard = true;
	}

	public static bool GetStamperMessageHeard()
	{
		return cachedSavedData.stamperMessageHeard;
	}

	public static void MarkArchiveMessageHeard()
	{
		cachedSavedData.archiveMessageHeard = true;
	}

	public static bool GetArchiveMessageHeard()
	{
		return cachedSavedData.archiveMessageHeard;
	}

	public static void MarkInfoMessageHeard()
	{
		cachedSavedData.infoMessageHeard = true;
	}

	public static bool GetInfoMessageHeard()
	{
		return cachedSavedData.infoMessageHeard;
	}

	public static void MarkVacuumMessageHeard()
	{
		cachedSavedData.vacuumMessageHeard = true;
	}

	public static bool GetVacuumMessageHeard()
	{
		return cachedSavedData.vacuumMessageHeard;
	}

	public static void OverwriteLastHubInterruptIndex(int s)
	{
		cachedSavedData.lastHubInterruptIndex = s;
	}

	public static int GetLastHubInterruptIndex()
	{
		return cachedSavedData.lastHubInterruptIndex;
	}

	public static void OverwriteEndGameTimerStart(DateTime s)
	{
		cachedSavedData.endGameTimerStart = s;
	}

	public static DateTime GetGetEndGameTimerStart()
	{
		return cachedSavedData.endGameTimerStart;
	}

	public static void OverwriteHasSetupStamper(bool s)
	{
		cachedSavedData.hasSetupStamper = s;
	}

	public static bool GetHasSetupStamper()
	{
		return cachedSavedData.hasSetupStamper;
	}

	public static void MarkCoroutinePlayed(IEnumerator routine)
	{
		if (!cachedSavedData.playedCoroutines.Contains(IEnumeratorToString(routine)))
		{
			cachedSavedData.playedCoroutines.Add(IEnumeratorToString(routine));
		}
	}

	public static void MarkCoroutinePlayed(string routine)
	{
		if (!cachedSavedData.playedCoroutines.Contains(routine))
		{
			cachedSavedData.playedCoroutines.Add(routine);
		}
	}

	public static void ClearCoroutines()
	{
		cachedSavedData.playedCoroutines.Clear();
	}

	public static bool HasPlayedCoroutine(IEnumerator routine)
	{
		return cachedSavedData.playedCoroutines.Contains(IEnumeratorToString(routine));
	}

	public static bool HasPlayedCoroutine(string routine)
	{
		return cachedSavedData.playedCoroutines.Contains(routine);
	}

	public static List<string> GetPlayedCoroutines()
	{
		return cachedSavedData.playedCoroutines;
	}

	public static void PrejobSetSaveButton()
	{
		cachedSavedData.saveButton++;
	}

	public static int PrejobGetSaveButton()
	{
		return cachedSavedData.saveButton;
	}

	public static void PrejobSetHRCage()
	{
		cachedSavedData.HRCage++;
	}

	public static int PrejobGetHRCage()
	{
		return cachedSavedData.HRCage;
	}

	public static void PrejobSetChazAbuse()
	{
		cachedSavedData.chazAbuse++;
	}

	public static int PrejobGetChazAbuse()
	{
		return cachedSavedData.chazAbuse;
	}

	private void reactToSceneLoaded(string newSceneName)
	{
		if (newSceneName == TCScenes.S.VVR_LVL__HUB.Identifier || newSceneName == TCScenes.S.VVR_LVL__BACKSTAGE.Identifier)
		{
			cachedSavedData.reloadScene = (StartingScene)Enum.Parse(typeof(StartingScene), newSceneName);
		}
	}

	public static string IEnumeratorToString(IEnumerator routine)
	{
		string text = string.Empty + routine.GetType();
		return text.Split('<', '>')[1];
	}
}
