using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConsumableSaveSyncer : MonoBehaviour
{
	[SerializeField]
	private EagerScene mEagerScene;

	[SerializeField]
	private List<VVRConsumable> mConsumables;

	public void GenerateConsumableList()
	{
		mConsumables = Object.FindObjectsOfType<VVRConsumable>().ToList();
		for (int i = 0; i < mConsumables.Count; i++)
		{
			if (mConsumables[i].destroyOnConsume)
			{
				mConsumables.Remove(mConsumables[i]);
				i--;
			}
		}
	}

	public void SaveConsumables()
	{
		ConsumableSaveData[] array = new ConsumableSaveData[mConsumables.Count];
		for (int i = 0; i < mConsumables.Count; i++)
		{
			if (mConsumables[i] != null && !mConsumables[i].destroyOnConsume)
			{
				array[i] = new ConsumableSaveData(mConsumables[i]);
			}
		}
		PersistentDataManager.PushConsumableData(mEagerScene.Scene.Name, array);
		SafeDebug.Log("<color=green>Consumables Saved!</color>");
	}

	private void OnApplicationQuit()
	{
		SaveConsumables();
	}

	public void LoadConsumables()
	{
		ConsumableSaveData[] levelConsumableData = PersistentDataManager.GetLevelConsumableData(mEagerScene.Scene.Name);
		if (levelConsumableData == null)
		{
			SafeDebug.LogWarning("No saved data consumableData for level " + mEagerScene.Scene.Name);
			return;
		}
		if (levelConsumableData.Length != mConsumables.Count)
		{
			SafeDebug.LogWarning("Saved Consumable List has a different count than the current scene's consumable list!");
		}
		for (int i = 0; i < mConsumables.Count && i < levelConsumableData.Length; i++)
		{
			if (mConsumables[i] != null && levelConsumableData[i] != null)
			{
				mConsumables[i].LoadFromSaveData(levelConsumableData[i]);
			}
			else
			{
				SafeDebug.LogWarning("<color=red>One or more consumables in save syncing list is null!</color>");
			}
		}
		SafeDebug.Log("<color=green>Consumables Loaded!</color>");
	}
}
