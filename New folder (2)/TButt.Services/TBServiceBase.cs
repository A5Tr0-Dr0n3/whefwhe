using System.Collections.Generic;
using UnityEngine;

namespace TButt.Services;

public class TBServiceBase : MonoBehaviour
{
	public virtual void Initialize()
	{
		CheckEntitlement();
		GetUserID();
	}

	protected virtual void CheckEntitlement()
	{
	}

	protected virtual void EvalEntitlement(bool passed)
	{
		if (passed)
		{
			Debug.Log("Verified entitlement check for " + TBCore.GetActiveService());
			TBServiceManager.OnEntitlementCheckComplete();
		}
		else
		{
			Debug.LogError("Failed entitlement check for " + TBCore.GetActiveService());
			Application.Quit();
		}
	}

	protected virtual void GetUserID()
	{
	}

	public virtual void UnlockAchievement(string token)
	{
	}

	public virtual Dictionary<string, bool> GetAchievementDictionary()
	{
		return new Dictionary<string, bool>();
	}

	public virtual void SetRichPresence(string token)
	{
	}

	public virtual void SignInPlayer()
	{
	}

	public virtual bool IsPlayerSignedIn()
	{
		return false;
	}

	public virtual void SaveDataToFile(string serializedString, string fileName, string containerDisplayName, string containerDisplayDescription)
	{
		TBDataManager.SaveStringToStorageDefault(serializedString, TBDataManager.persistentDataPath + fileName);
		TBDataManager.DataResult result = new TBDataManager.DataResult
		{
			fileName = fileName,
			dataStatus = TBDataManager.DataStatus.Success
		};
		ConfirmSaveData(result);
	}

	public virtual void LoadDataFromFile(string fileName)
	{
		TBDataManager.DataResult result = new TBDataManager.DataResult
		{
			fileName = fileName,
			dataStatus = TBDataManager.DataStatus.Success,
			serializedData = TBDataManager.LoadStringFromStorageDefault(TBDataManager.persistentDataPath + fileName)
		};
		ConfirmLoadData(result);
	}

	public virtual void DeleteDataFile(string fileName)
	{
		TBDataManager.DeleteDataFile(fileName, TBDataManager.PathType.PersistentDataPath);
		TBDataManager.DataResult result = new TBDataManager.DataResult
		{
			fileName = fileName,
			dataStatus = TBDataManager.DataStatus.Success,
			serializedData = null
		};
		ConfirmDeleteData(result);
	}

	public void ConfirmSaveData(TBDataManager.DataResult result)
	{
		TBDataManager.Internal.FireDataSavedEvent(result);
	}

	public void ConfirmLoadData(TBDataManager.DataResult result)
	{
		TBDataManager.Internal.FireDataLoadedEvent(result);
	}

	public void ConfirmDeleteData(TBDataManager.DataResult result)
	{
		TBDataManager.Internal.FireDataDeletedEvent(result);
	}
}
