using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class MetaSaveLoadUtility
{
	public const string metaSaveFileName = "metasave.metasave";

	public static VVR_METASAVEDATA Load(bool writeBlankOnFailure = true)
	{
		VVR_METASAVEDATA vVR_METASAVEDATA = null;
		string text = Application.persistentDataPath + "/metasave.metasave";
		if (File.Exists(text))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = File.Open(text, FileMode.Open);
			try
			{
				vVR_METASAVEDATA = binaryFormatter.Deserialize(fileStream) as VVR_METASAVEDATA;
				fileStream.Close();
			}
			catch (Exception ex)
			{
				fileStream.Close();
				SafeDebug.Log(ex.ToString());
				string s = File.ReadAllText(text);
				try
				{
					MemoryStream memoryStream = new MemoryStream();
					byte[] array = Convert.FromBase64String(s);
					memoryStream.Write(array, 0, array.Length);
					memoryStream.Seek(0L, SeekOrigin.Begin);
					vVR_METASAVEDATA = binaryFormatter.Deserialize(memoryStream) as VVR_METASAVEDATA;
				}
				catch (Exception ex2)
				{
					SafeDebug.Log(ex2.ToString());
				}
			}
			if (vVR_METASAVEDATA != null)
			{
				SafeDebug.Log("<color=green>Successfully loaded meta save data at " + text + "!</color>");
			}
			else
			{
				SafeDebug.LogError("<color=#fa8072>Tried to load meta save data at " + text + ", but our file failed the VVR_MetaSaveData cast!</color>");
				if (writeBlankOnFailure)
				{
					VVR_METASAVEDATA vVR_METASAVEDATA2 = new VVR_METASAVEDATA();
					Save(vVR_METASAVEDATA2);
					return vVR_METASAVEDATA2;
				}
			}
		}
		else
		{
			SafeDebug.Log("<color=green>Tried to load meta save at " + text + ", but no file on disk!</color>");
			if (writeBlankOnFailure)
			{
				VVR_METASAVEDATA vVR_METASAVEDATA3 = new VVR_METASAVEDATA();
				Save(vVR_METASAVEDATA3);
				return vVR_METASAVEDATA3;
			}
		}
		return vVR_METASAVEDATA;
	}

	public static VVR_METASAVEDATA.SubtitlesState SysLangToSubState()
	{
		SystemLanguage systemLanguage = Application.systemLanguage;
		SafeDebug.Log("New save; setting lanuage to " + systemLanguage);
		return systemLanguage switch
		{
			SystemLanguage.ChineseSimplified => VVR_METASAVEDATA.SubtitlesState.CHINESE_S, 
			SystemLanguage.ChineseTraditional => VVR_METASAVEDATA.SubtitlesState.CHINESE_T, 
			SystemLanguage.English => VVR_METASAVEDATA.SubtitlesState.NONE, 
			SystemLanguage.French => VVR_METASAVEDATA.SubtitlesState.FRENCH, 
			SystemLanguage.German => VVR_METASAVEDATA.SubtitlesState.GERMAN, 
			SystemLanguage.Italian => VVR_METASAVEDATA.SubtitlesState.ITALIAN, 
			SystemLanguage.Japanese => VVR_METASAVEDATA.SubtitlesState.JAPANESE, 
			SystemLanguage.Korean => VVR_METASAVEDATA.SubtitlesState.KOREAN, 
			SystemLanguage.Spanish => VVR_METASAVEDATA.SubtitlesState.SPANISH, 
			_ => VVR_METASAVEDATA.SubtitlesState.NONE, 
		};
	}

	public static void Save(VVR_METASAVEDATA data)
	{
		string text = Application.persistentDataPath + "/metasave.metasave";
		if (data != null)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = File.Open(text, FileMode.OpenOrCreate);
			binaryFormatter.Serialize(fileStream, data);
			SafeDebug.Log("<color=magenta>Saved metaSaveData to " + text + "</color>");
			fileStream.Close();
		}
	}

	public static void DeleteMetaSave()
	{
		string text = Application.persistentDataPath + '/' + "metasave.metasave";
		if (Load(writeBlankOnFailure: false) != null)
		{
			File.Delete(text);
		}
		else
		{
			SafeDebug.LogError("Failed to delete nonexistent meta save at " + text);
		}
	}
}
