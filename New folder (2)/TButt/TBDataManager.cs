using System;
using System.IO;
using UnityEngine;

namespace TButt;

public static class TBDataManager
{
	public enum PathType
	{
		PersistentDataPath,
		ProjectFolder,
		ResourcesFolder
	}

	[Serializable]
	public class Wrapper<T>
	{
		public T[] Items;
	}

	public class Async
	{
		public static void Save(object obj, string fileName, string containerDisplayName = "", string containerDisplayDescription = "", bool useEncryption = false)
		{
			if (!Application.isPlaying)
			{
				SerializeObjectToFile(obj, fileName, PathType.PersistentDataPath, useEncryption);
			}
			else if (TBCore.GetActiveService() == VRService.None)
			{
				SerializeObjectToFile(obj, fileName, PathType.PersistentDataPath, useEncryption);
				Internal.FireDataSavedEvent(new DataResult
				{
					dataStatus = DataStatus.Success,
					fileName = fileName
				});
			}
			else
			{
				TBServiceManager.DataManagement.Save(SerializeString(obj, useEncryption), fileName, string.Empty, string.Empty);
			}
		}

		public static void Load(string fileName)
		{
			if (!Application.isPlaying)
			{
				Debug.LogError("Async load can only be used in Play mode.");
			}
			else if (TBCore.GetActiveService() == VRService.None)
			{
				Internal.FireDataLoadedEvent(new DataResult
				{
					fileName = fileName,
					dataStatus = DataStatus.Success,
					serializedData = LoadStringFromStorageDefault(persistentDataPath + fileName)
				});
			}
			else
			{
				TBServiceManager.DataManagement.Load(fileName);
			}
		}

		public static void Delete(string fileName)
		{
			if (!Application.isPlaying)
			{
				Debug.LogError("Async delete can only be used in Play mode.");
			}
			else if (TBCore.GetActiveService() == VRService.None)
			{
				Internal.FireDataDeletedEvent(new DataResult
				{
					fileName = fileName,
					dataStatus = DataStatus.Success,
					serializedData = null
				});
			}
			else
			{
				TBServiceManager.DataManagement.Delete(fileName);
			}
		}
	}

	public struct DataResult
	{
		public string fileName;

		public string serializedData;

		public DataStatus dataStatus;
	}

	public enum DataStatus
	{
		Unknown,
		Error,
		Success
	}

	public class Events
	{
		public delegate void TBDataEvent(DataResult result);

		public static TBDataEvent OnDataSaved;

		public static TBDataEvent OnDataLoaded;

		public static TBDataEvent OnDataDeleted;
	}

	public class Internal
	{
		public static void FireDataSavedEvent(DataResult result)
		{
			if (Events.OnDataSaved != null)
			{
				Events.OnDataSaved(result);
			}
		}

		public static void FireDataLoadedEvent(DataResult result)
		{
			Debug.Log("Fired OnDataLoaded event");
			if (Events.OnDataLoaded != null)
			{
				Events.OnDataLoaded(result);
			}
		}

		public static void FireDataDeletedEvent(DataResult result)
		{
			Debug.Log("Fired OnDataDeleted event");
			if (Events.OnDataDeleted != null)
			{
				Events.OnDataDeleted(result);
			}
		}
	}

	public static string persistentDataPath = Application.persistentDataPath + "/";

	private static string _encryptionPassword = "O!^KNin8!mbnbxkN3*8!0!Kd%0912C1u";

	public static void SerializeObjectToFile(object obj, string fileName, PathType pathType = PathType.PersistentDataPath, bool useEncryption = false)
	{
		if (obj == null)
		{
			Debug.LogError("Attempted to serialize an object, but the object was null.");
			return;
		}
		string serializedString = SerializeString(obj, useEncryption);
		string pathForType = GetPathForType(pathType);
		if (pathType != PathType.PersistentDataPath || Application.isPlaying)
		{
		}
		SaveStringToStorageDefault(serializedString, pathForType + fileName);
	}

	public static T DeserializeFromFile<T>(string fileName, PathType pathType = PathType.PersistentDataPath, bool useEncryption = false)
	{
		if (pathType == PathType.ResourcesFolder)
		{
			TextAsset textAsset = Resources.Load(fileName) as TextAsset;
			string text = null;
			if (textAsset != null)
			{
				text = textAsset.ToString();
			}
			if (string.IsNullOrEmpty(text))
			{
				return GetNullOrEmptyOfType<T>();
			}
			return DeserializeString<T>(text, fileName, useEncryption);
		}
		string pathForType = GetPathForType(pathType);
		string empty = string.Empty;
		if (pathType != PathType.PersistentDataPath || Application.isPlaying)
		{
		}
		empty = LoadStringFromStorageDefault(pathForType + fileName);
		return DeserializeString<T>(empty, fileName, useEncryption);
	}

	public static void SaveStringToStorageDefault(string serializedString, string pathAndFileName)
	{
		using FileStream fileStream = File.Open(pathAndFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
		using StreamWriter streamWriter = new StreamWriter(fileStream);
		fileStream.SetLength(0L);
		streamWriter.Write(serializedString);
		streamWriter.Close();
		fileStream.Close();
	}

	public static string LoadStringFromStorageDefault(string pathAndFileName)
	{
		using FileStream fileStream = new FileStream(pathAndFileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);
		using StreamReader streamReader = new StreamReader(fileStream);
		string result = streamReader.ReadToEnd();
		fileStream.Close();
		streamReader.Close();
		return result;
	}

	public static T DeserializeString<T>(string serializedString, string fileName, bool useEncryption = false)
	{
		if (!string.IsNullOrEmpty(serializedString))
		{
			if (useEncryption)
			{
				try
				{
					string json = TBDataEncryptor.Decrypt(serializedString, _encryptionPassword);
					return (T)JsonUtility.FromJson(json, typeof(T));
				}
				catch
				{
					Debug.LogError("Attempted to deserialize an encrypted string, but was not able to cast to the requested type T. Will now attempt to deserialize without decrypting.");
				}
			}
			try
			{
				return (T)JsonUtility.FromJson(serializedString, typeof(T));
			}
			catch
			{
				Debug.LogError("The serialized string did not match the expected format, and will be replaced.");
				return GetNullOrEmptyOfType<T>();
			}
		}
		Debug.LogWarning("TBData attempted to deserialize file " + fileName + " but the deserialized version of the data was null. This expected if the file didn't exist.");
		return GetNullOrEmptyOfType<T>();
	}

	public static string SerializeString(object obj, bool useEncryption = false)
	{
		string text = ((obj.GetType() != typeof(string)) ? JsonUtility.ToJson(obj) : ((string)obj));
		if (useEncryption)
		{
			text = TBDataEncryptor.Encrypt(text, _encryptionPassword);
		}
		return text;
	}

	public static T GetNullOrEmptyOfType<T>()
	{
		if (Nullable.GetUnderlyingType(typeof(T)) == null)
		{
			return (T)Activator.CreateInstance(typeof(T));
		}
		return (T)JsonUtility.FromJson(string.Empty, typeof(T));
	}

	private static string GetPathForType(PathType pathtype)
	{
		return pathtype switch
		{
			PathType.PersistentDataPath => persistentDataPath, 
			PathType.ProjectFolder => string.Empty, 
			PathType.ResourcesFolder => string.Empty, 
			_ => string.Empty, 
		};
	}

	public static bool HasDataFile(string fileName, PathType pathType)
	{
		if (Application.isPlaying)
		{
		}
		return File.Exists(GetPathForType(pathType) + fileName);
	}

	public static void DeleteDataFile(string fileName, PathType pathType)
	{
		if (File.Exists(GetPathForType(pathType) + fileName))
		{
			File.Delete(GetPathForType(pathType) + fileName);
		}
	}

	public static string ReadStringFromFile(string fileName, PathType pathType)
	{
		string pathForType = GetPathForType(pathType);
		using FileStream fileStream = new FileStream(pathForType + fileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read);
		using StreamReader streamReader = new StreamReader(fileStream);
		string text = streamReader.ReadToEnd();
		fileStream.Close();
		streamReader.Close();
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		Debug.LogWarning("TBData attempted to deserialize file " + fileName + " but the deserialized version of the data was null. This expected if the file didn't exist.");
		return string.Empty;
	}

	public static void WriteStringToFile(string text, string fileName, PathType pathType = PathType.PersistentDataPath)
	{
		if (text == null)
		{
			Debug.LogError("Attempted to save a string, but the string was null.");
			return;
		}
		string pathForType = GetPathForType(pathType);
		using FileStream fileStream = File.Open(pathForType + fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
		using StreamWriter streamWriter = new StreamWriter(fileStream);
		fileStream.SetLength(0L);
		streamWriter.Write(text);
		streamWriter.Close();
		fileStream.Close();
	}

	public static T[] FromJsonWrapper<T>(Wrapper<T> wrapper)
	{
		return wrapper.Items;
	}

	public static Wrapper<T> ToJsonWrapper<T>(T[] array)
	{
		Wrapper<T> wrapper = new Wrapper<T>();
		wrapper.Items = array;
		return wrapper;
	}
}
