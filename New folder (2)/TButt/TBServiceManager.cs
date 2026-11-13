using System.Collections.Generic;
using TButt.Services;
using UnityEngine;

namespace TButt;

public class TBServiceManager : MonoBehaviour
{
	public class DataManagement
	{
		public static void Save(string serializedString, string fileName, string containerDisplayName = "", string containerDisplayDescription = "")
		{
			if (_activeService == null)
			{
				Debug.LogError("Attempted to use service to save data capsules before service was initialized.");
			}
			else
			{
				_activeService.SaveDataToFile(serializedString, fileName, containerDisplayName, containerDisplayDescription);
			}
		}

		public static void Load(string fileName)
		{
			if (_activeService == null)
			{
				Debug.LogError("Attempted to use service to load data capsules before service was initialized.");
			}
			else
			{
				_activeService.LoadDataFromFile(fileName);
			}
		}

		public static void Delete(string fileName)
		{
			if (_activeService == null)
			{
				Debug.LogError("Attempted to use service to delete data capsules before service was initialized.");
			}
			else
			{
				_activeService.DeleteDataFile(fileName);
			}
		}
	}

	public class Events
	{
		public delegate void TBServiceEvent();

		public static TBServiceEvent OnUserLoggedIn;

		public static TBServiceEvent OnUserLoggedOut;
	}

	public class Internal
	{
		public static void FireOnUserLoggedInEvent()
		{
			if (Events.OnUserLoggedIn != null)
			{
				Events.OnUserLoggedIn();
			}
		}

		public static void FireOnUserLoggedOutEvent()
		{
			if (Events.OnUserLoggedOut != null)
			{
				Events.OnUserLoggedOut();
			}
		}
	}

	private static Dictionary<string, bool> _achievementList;

	private static TBServiceBase _activeService;

	public static void Initialize()
	{
		switch (TBCore.GetActiveService())
		{
		case VRService.Oculus:
			_activeService = TBCore.instance.gameObject.AddComponent<TBOculusService>();
			break;
		case VRService.Steam:
			_activeService = TBCore.instance.gameObject.AddComponent<TBSteamService>();
			break;
		case VRService.XboxLive:
			_activeService = TBCore.instance.gameObject.AddComponent<TBXBoxService>();
			break;
		}
		_activeService.Initialize();
	}

	public static void OnEntitlementCheckComplete()
	{
		_achievementList = _activeService.GetAchievementDictionary();
	}

	public static bool IsAchievementUnlocked(string token)
	{
		if (_achievementList == null)
		{
			return true;
		}
		if (_achievementList.TryGetValue(token, out var value))
		{
			return value;
		}
		return true;
	}

	public static void UnlockAchievement(string token)
	{
		if (!(_activeService == null) && !IsAchievementUnlocked(token))
		{
			_activeService.UnlockAchievement(token);
		}
	}

	public static void UpdateAchievementList(string token, bool value)
	{
		if (_achievementList.TryGetValue(token, out var _))
		{
			_achievementList[token] = value;
		}
		else
		{
			Debug.LogError("Attempted to update unlock status for the token " + token + " but it doesn't exist in the achievement dictionary!");
		}
	}

	public static void SetAchievementList(Dictionary<string, bool> list)
	{
		_achievementList = list;
	}

	public static void SignInPlayer()
	{
		_activeService.SignInPlayer();
	}

	public static bool IsPlayerSignedIn()
	{
		return _activeService.IsPlayerSignedIn();
	}

	public static void SetRichPresence(string token)
	{
		if (!(_activeService == null))
		{
			_activeService.SetRichPresence(token);
		}
	}
}
