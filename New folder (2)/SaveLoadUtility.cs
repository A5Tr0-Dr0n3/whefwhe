using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using UnityEngine;

public class SaveLoadUtility
{
	public const string savePrefix = "playerInfo";

	public const string saveExtension = ".dat";

	public const string backupExtension = ".saveBackup";

	public const string saveScriptableObjectDirectory = "Assets/Saved Save States";

	public const string legacySaveDirectory = "Saved Save States/Legacy Saves";

	public const int maxSaveCount = 3;

	public static void SaveToSlot(VVR_SAVEDATA saveData, int targetSlot)
	{
		if (IsValidSaveSlot(targetSlot))
		{
			SafeDebug.Log("<color=green>Saving to slot: " + targetSlot + "</color>");
			Save(saveData, getSavePath(targetSlot));
		}
		else
		{
			SafeDebug.Log("<color=#fa8072>Skipping save with invalid save slot " + targetSlot + "</color>");
		}
	}

	private static void Save(VVR_SAVEDATA saveData, string path)
	{
		if (saveData != null)
		{
			if (saveData.arbitraryStringDictionary == null)
			{
				saveData.arbitraryStringDictionary = new SerializableDictionary<string, string>();
			}
			saveData.arbitraryStringDictionary["LASTPLAYED"] = DateTime.Now.ToString();
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = File.Open(path, FileMode.OpenOrCreate);
			binaryFormatter.Serialize(fileStream, saveData);
			SafeDebug.Log("<color=green>Saved data to " + path + "</color>");
			fileStream.Close();
		}
	}

	public static VVR_SAVEDATA LoadFromSlot(int targetSlot, bool writeBlankOnFailure)
	{
		if (IsValidSaveSlot(targetSlot))
		{
			return Load(getSavePath(targetSlot), writeBlankOnFailure);
		}
		SafeDebug.Log("<color=#fa8072>Skipping load with invalid save slot " + targetSlot + "</color>");
		return null;
	}

	private static VVR_SAVEDATA Load(string path, bool writeBlankOnFailure, bool dontCheckIfExists = false, bool quietly = false)
	{
		SafeDebug.Log("Path: " + path + " writeBlankOnFailure: " + writeBlankOnFailure + " dontCheckIfExists: " + dontCheckIfExists + " Quietly: " + quietly);
		VVR_SAVEDATA vVR_SAVEDATA = null;
		if (dontCheckIfExists || saveExists(path))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = File.Open(path, FileMode.Open);
			try
			{
				vVR_SAVEDATA = binaryFormatter.Deserialize(fileStream) as VVR_SAVEDATA;
				fileStream.Close();
			}
			catch (Exception ex)
			{
				fileStream.Close();
				SafeDebug.Log(ex.ToString());
				string s = File.ReadAllText(path);
				try
				{
					MemoryStream memoryStream = new MemoryStream();
					byte[] array = Convert.FromBase64String(s);
					memoryStream.Write(array, 0, array.Length);
					memoryStream.Seek(0L, SeekOrigin.Begin);
					vVR_SAVEDATA = binaryFormatter.Deserialize(memoryStream) as VVR_SAVEDATA;
				}
				catch (Exception ex2)
				{
					SafeDebug.Log(ex2.ToString());
				}
			}
			if (vVR_SAVEDATA != null)
			{
				if (!quietly)
				{
					SafeDebug.Log("<color=green>Successfully loaded data at " + path + "!</color>");
				}
			}
			else
			{
				SafeDebug.LogError("<color=#fa8072>Tried to Load at " + path + ", but our file failed the VVR_SAVEDATA cast!</color>");
				if (writeBlankOnFailure)
				{
					writeBlankSaveToPath(path);
				}
			}
		}
		else
		{
			SafeDebug.Log("<color=green>Tried to Load at " + path + ", but no file on disk!</color>");
			if (writeBlankOnFailure)
			{
				writeBlankSaveToPath(path);
			}
		}
		return vVR_SAVEDATA;
	}

	public static void DeleteAtSlot(int targetSlot)
	{
		if (IsValidSaveSlot(targetSlot))
		{
			SafeDebug.Log("<color=green>Deleting save at slot: " + targetSlot + "</color>");
			DeleteSave(getSavePath(targetSlot));
		}
		else
		{
			SafeDebug.Log("<color=#fa8072>Failed to delete save at invalid slot: " + targetSlot + "</color>");
		}
	}

	private static void DeleteSave(string path)
	{
		if (saveExists(path))
		{
			File.Delete(path);
		}
		else
		{
			SafeDebug.LogError("Failed to delete nonexistent save file at " + path);
		}
	}

	private static VVR_SAVEDATA writeBlankSaveToPath(string targetPath)
	{
		SafeDebug.Log("<color=green>Writing new empty save at  " + targetPath + "</color>");
		VVR_SAVEDATA vVR_SAVEDATA = new VVR_SAVEDATA(GetNextAvailableBeeSlot());
		Save(vVR_SAVEDATA, targetPath);
		return vVR_SAVEDATA;
	}

	private static void moveSaveFile(string oldPath, string newPath, VVR_SAVEDATA saveData = null)
	{
		if (comparePaths(oldPath, newPath))
		{
			return;
		}
		if (saveExists(oldPath))
		{
			if (saveData == null)
			{
				saveData = Load(oldPath, writeBlankOnFailure: false);
			}
			Save(saveData, newPath);
			DeleteSave(oldPath);
			SafeDebug.Log("<color=green>Moved file " + oldPath + " to " + newPath + "</color>");
		}
		else
		{
			SafeDebug.LogError("Error! Trying to move a nonexistent save file at invalid path " + oldPath + ". No can do");
		}
	}

	private static bool comparePaths(string a, string b)
	{
		a = Path.GetFullPath(a);
		b = Path.GetFullPath(b);
		return a == b;
	}

	public static bool IsValidSaveSlot(int testSlotIndex)
	{
		return testSlotIndex >= 0 && testSlotIndex < 3;
	}

	private static string getSavePath(int slotIndex)
	{
		return Application.persistentDataPath + "/playerInfo_" + slotIndex + ".dat";
	}

	public static string GetLegacyUnslottedSavePath()
	{
		return "playerInfo.dat";
	}

	public static int CreateNewAccount(bool writeImmediatelyToDisk)
	{
		VVR_SAVEDATA newAccountData;
		return CreateNewAccount(writeImmediatelyToDisk, out newAccountData);
	}

	public static int CreateNewAccount(bool immediatelyWriteToDisk, out VVR_SAVEDATA newAccountData)
	{
		List<VVR_SAVEDATA> andCleanAllAccountSaves = GetAndCleanAllAccountSaves(clean: true, out var _);
		if (andCleanAllAccountSaves.Count >= 3)
		{
			SafeDebug.LogError("We're trying to make a new account, but there's already " + andCleanAllAccountSaves.Count + " saves!");
			newAccountData = null;
			return -1;
		}
		newAccountData = new VVR_SAVEDATA(getNextAvailableBeeSlot(andCleanAllAccountSaves));
		int count = andCleanAllAccountSaves.Count;
		if (immediatelyWriteToDisk)
		{
			Save(newAccountData, getSavePath(count));
		}
		return count;
	}

	public static VVR_SAVEDATA[] GetAllAccountSaves()
	{
		List<string> pathsOut = new List<string>();
		return GetAndCleanAllAccountSaves(clean: false, out pathsOut).ToArray();
	}

	public static VVR_SAVEDATA[] GetAndCleanAllAccountSaves(bool clean)
	{
		List<string> pathsOut = new List<string>();
		return GetAndCleanAllAccountSaves(clean, out pathsOut).ToArray();
	}

	public static List<VVR_SAVEDATA> GetAndCleanAllAccountSaves(bool clean, out List<string> pathsOut)
	{
		List<string> allSavePaths = getAllSavePaths();
		List<string> list = allSavePaths.Where((string file) => Regex.IsMatch(file, ".*_[0-9]*\\.dat")).ToList();
		List<string> second = allSavePaths.Where((string file) => !Regex.IsMatch(file, ".*_[0-9]*\\.dat")).ToList();
		list.Sort((string i1, string i2) => i1.CompareTo(i2));
		List<string> list2 = list.Concat(second).ToList();
		if (PersistentDataManager.NeedBackup())
		{
			backupSavePaths(list2, Application.version);
			PersistentDataManager.MarkBackedUp();
		}
		List<VVR_SAVEDATA> list3 = new List<VVR_SAVEDATA>();
		pathsOut = new List<string>();
		for (int num = 0; num < list2.Count; num++)
		{
			string savePath = getSavePath(num);
			if (num < 3)
			{
				VVR_SAVEDATA vVR_SAVEDATA = Load(list2[num], writeBlankOnFailure: false, dontCheckIfExists: false, quietly: true);
				string text = vVR_SAVEDATA.arbitraryStringDictionary["VERSION NUMBER"];
				switch (text)
				{
				case "1.5.02":
				case "1.3.4":
				case "1.3.3":
				case "1.2.2":
				case "1.2.1":
				case "1.1":
				case "1.0":
				case "1.6.06":
					SafeDebug.Log("Updating save: " + num + " to extended save");
					SafeDebug.Log("Job state was: " + vVR_SAVEDATA.jobState);
					if (vVR_SAVEDATA.jobState > VVR_HUB.JobStates.Macy)
					{
						vVR_SAVEDATA.jobState++;
						SafeDebug.Log("Incrementing job state for macy");
					}
					if (vVR_SAVEDATA.jobState >= VVR_HUB.JobStates.Tina)
					{
						vVR_SAVEDATA.jobState++;
						SafeDebug.Log("Incrementing job state for tina");
					}
					SafeDebug.Log("New job state is: " + vVR_SAVEDATA.jobState);
					SafeDebug.Log("Setting save version number to: " + Application.version);
					vVR_SAVEDATA.arbitraryStringDictionary["VERSION NUMBER"] = Application.version;
					SafeDebug.Log("Saving save to: " + savePath);
					Save(vVR_SAVEDATA, savePath);
					break;
				default:
					SafeDebug.Log("Not fixing your pre extended build because yoru version number is: " + text);
					break;
				}
				moveSaveFile(list2[num], savePath, vVR_SAVEDATA);
				pathsOut.Add(savePath);
				list3.Add(vVR_SAVEDATA);
			}
			else
			{
				DeleteSave(savePath);
			}
		}
		if (clean)
		{
			list3 = fixBrokenSaves(pathsOut, list3);
		}
		return list3;
	}

	private static bool saveExists(string pathIn)
	{
		return (from path in getAllSavePaths()
			where comparePaths(path, pathIn)
			select path).ToList().Count > 0;
	}

	private static List<string> getAllSavePaths()
	{
		return (from file in Directory.GetFiles(Application.persistentDataPath).ToList()
			where Regex.IsMatch(file, ".*\\.dat")
			select file).ToList();
	}

	public static int GetNextAvailableBeeSlot()
	{
		return getNextAvailableBeeSlot(GetAndCleanAllAccountSaves(clean: false).ToList());
	}

	private static int getNextAvailableBeeSlot(List<VVR_SAVEDATA> data)
	{
		List<int> list = Enumerable.Range(0, 3).ToList();
		for (int i = 0; i < data.Count; i++)
		{
			if (data[i].arbitraryIntDictionary.ContainsKey("BEE SLOT") && list.Contains(data[i].arbitraryIntDictionary["BEE SLOT"]))
			{
				list.Remove(data[i].arbitraryIntDictionary["BEE SLOT"]);
			}
		}
		if (list.Count == 0)
		{
			SafeDebug.LogError("All of the bee slots are apparently taken? This is kind of fucked up, but I guess we'll default to zero. Let's make sure that this doesn't happen in a shipped build though, yeah?");
			list.Add(0);
		}
		return list[0];
	}

	private static List<VVR_SAVEDATA> fixBrokenSaves(List<string> paths, List<VVR_SAVEDATA> data)
	{
		for (int i = 0; i < data.Count; i++)
		{
			if (!data[i].arbitraryIntDictionary.ContainsKey("BEE SLOT") || !IsValidSaveSlot(data[i].arbitraryIntDictionary["BEE SLOT"]))
			{
				data[i].WriteMissingDictionaryValues(getNextAvailableBeeSlot(data));
				if (!data[i].arbitraryStringDictionary.ContainsKey("LASTPLAYED"))
				{
					data[i].arbitraryStringDictionary.Add("LASTPLAYED", DateTime.Now.Date.ToString());
				}
			}
			else
			{
				data[i].WriteMissingDictionaryValues();
				if (!data[i].arbitraryStringDictionary.ContainsKey("LASTPLAYED"))
				{
					data[i].arbitraryStringDictionary.Add("LASTPLAYED", DateTime.Now.Date.ToString());
				}
			}
		}
		return data;
	}

	public static void RunLegacySaveUnitTests()
	{
		List<string> list = (from file in Directory.GetFiles(Application.dataPath + '/' + "Saved Save States/Legacy Saves").ToList()
			where Regex.IsMatch(file, ".*\\.saveBackup((?!meta).)*$")
			select file).ToList();
		string text = ((list.Count != 0) ? ("found files " + string.Join(", ", list.ToArray())) : "[none]");
		SafeDebug.Log("<color=magenta>Running legacy save unit tests on: " + text + "</color>");
		foreach (string item in list)
		{
			VVR_SAVEDATA vVR_SAVEDATA = Load(item, writeBlankOnFailure: false, dontCheckIfExists: true);
			if (vVR_SAVEDATA == null)
			{
				SafeDebug.LogError("<color=magenta>Failed to convert file " + item + " into VVR_SAVEDATA; big oops</color>");
			}
		}
	}

	private static void backupSavePaths(List<string> originalPaths, string versionNumberSuffix)
	{
		for (int i = 0; i < originalPaths.Count; i++)
		{
			string text = originalPaths[i].Replace(".dat", ".saveBackup_" + versionNumberSuffix);
			if (!File.Exists(text))
			{
				File.Copy(originalPaths[i], text);
			}
		}
	}
}
