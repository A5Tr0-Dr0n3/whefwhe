using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class AndroidSaveUtility
{
	private const int savePushChunkSize = 2047;

	private const string deleteCommandPath = "/VVR_Scripts/Systems/Save-Load/AndroidBatchScripts/DeleteDataFromAndroid.bat";

	private const string writeCommandPath = "/VVR_Scripts/Systems/Save-Load/AndroidBatchScripts/WriteDataToAndroid.bat";

	public static void PurgeAndroidSaves()
	{
		string[] array = new string[4] { "playerInfo.dat", null, null, null };
		for (int i = 0; i < 3; i++)
		{
			array[i + 1] = "playerInfo" + '_' + i + ".dat";
		}
		string[] array2 = array;
		foreach (string targetFilename in array2)
		{
			deleteDataFromAndroid(targetFilename);
		}
	}

	public static void PurgeAndroidMetasave()
	{
		deleteDataFromAndroid("metasave.metasave");
	}

	private static void deleteDataFromAndroid(string targetFilename)
	{
		string text = Application.dataPath + "/VVR_Scripts/Systems/Save-Load/AndroidBatchScripts/DeleteDataFromAndroid.bat";
		SafeDebug.Log("<color=green>Scrubbing Android Save using command " + text + " and parameters " + targetFilename + "</color>");
		ProcessStartInfo processStartInfo = new ProcessStartInfo(text, targetFilename);
		processStartInfo.CreateNoWindow = true;
		processStartInfo.UseShellExecute = false;
		processStartInfo.RedirectStandardError = true;
		processStartInfo.RedirectStandardOutput = true;
		Process process = Process.Start(processStartInfo);
		process.OutputDataReceived += printProcessOutput;
		process.BeginOutputReadLine();
		process.ErrorDataReceived += printErrorOutput;
		process.BeginErrorReadLine();
		process.WaitForExit();
		process.Close();
	}

	public static void OverwriteAndroidSaves()
	{
		PurgeAndroidSaves();
		List<string> pathsOut = new List<string>();
		List<VVR_SAVEDATA> andCleanAllAccountSaves = SaveLoadUtility.GetAndCleanAllAccountSaves(clean: true, out pathsOut);
		for (int i = 0; i < pathsOut.Count; i++)
		{
			pushDataToAndroid(Path.GetFileName(pathsOut[i]), andCleanAllAccountSaves[i]);
		}
	}

	public static void OverwriteAndroidSaveLegacyUnslotted()
	{
		PurgeAndroidSaves();
		if (PersistentDataManager.GetActiveSaveSlot() != -1)
		{
			VVR_SAVEDATA vVR_SAVEDATA = SaveLoadUtility.LoadFromSlot(PersistentDataManager.GetActiveSaveSlot(), writeBlankOnFailure: false);
			if (vVR_SAVEDATA != null)
			{
				pushDataToAndroid(SaveLoadUtility.GetLegacyUnslottedSavePath(), vVR_SAVEDATA);
			}
		}
	}

	public static void OverwriteAndroidMetasave()
	{
		VVR_METASAVEDATA targetData = MetaSaveLoadUtility.Load();
		pushDataToAndroid("metasave.metasave", targetData);
	}

	private static void pushDataToAndroid(string targetFilename, object targetData)
	{
		string text = Application.dataPath + "/VVR_Scripts/Systems/Save-Load/AndroidBatchScripts/WriteDataToAndroid.bat";
		SafeDebug.Log("<color=green>Overwriting Android Save at " + targetFilename + " using command " + text + "</color>");
		string text2 = string.Empty;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, targetData);
			string text3 = Convert.ToBase64String(memoryStream.ToArray());
			text2 = text3;
		}
		text2 = text2.Replace("=", "\"=\"");
		SafeDebug.Log("<color=green>Pushing save data " + text2 + "</color>");
		int num = Mathf.CeilToInt((float)text2.Length / 2047f);
		string[] array = new string[num];
		int num2 = 2047;
		for (int i = 0; i < num; i++)
		{
			if (text2.Length < num2)
			{
				num2 = text2.Length;
			}
			array[i] = text2.Substring(0, num2);
			SafeDebug.Log("<color=green>Chunk " + (i + 1) + ": " + array[i] + "</color>");
			text2 = text2.Substring(num2, text2.Length - num2);
		}
		for (int j = 0; j < array.Length; j++)
		{
			string arguments = array[j] + " " + targetFilename + " " + ((j != 0) ? "APPEND" : "OVERWRITE");
			ProcessStartInfo processStartInfo = new ProcessStartInfo(text, arguments);
			processStartInfo.CreateNoWindow = true;
			processStartInfo.UseShellExecute = false;
			processStartInfo.RedirectStandardError = true;
			processStartInfo.RedirectStandardOutput = true;
			Process process = Process.Start(processStartInfo);
			process.OutputDataReceived += printProcessOutput;
			process.BeginOutputReadLine();
			process.ErrorDataReceived += printErrorOutput;
			process.BeginErrorReadLine();
			process.WaitForExit();
			process.Close();
		}
	}

	private static void printProcessOutput(object sender, DataReceivedEventArgs e)
	{
		if (!string.IsNullOrEmpty(e.Data) && e.Data.Trim().Length > 0)
		{
			SafeDebug.Log("D>" + '`' + e.Data + '`');
		}
	}

	private static void printErrorOutput(object sender, DataReceivedEventArgs e)
	{
		if (!string.IsNullOrEmpty(e.Data) && e.Data.Trim().Length > 0)
		{
			SafeDebug.LogError("E>" + '`' + e.Data + '`');
		}
	}
}
