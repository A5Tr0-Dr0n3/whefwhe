using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class SaveLoadRuntimeWatcher : MonoBehaviour
{
	private static SaveLoadRuntimeWatcher _instanceBacking;

	private static float lastSaveTimestamp;

	private const float minSaveDelta = 1f;

	private SafeCoroutine delaySaveThread;

	private static SaveLoadRuntimeWatcher instance
	{
		get
		{
			if (_instanceBacking == null)
			{
				_instanceBacking = Object.FindObjectOfType<SaveLoadRuntimeWatcher>();
			}
			return _instanceBacking;
		}
		set
		{
			_instanceBacking = value;
		}
	}

	private void Awake()
	{
		instance = this;
	}

	public static void DelaySave(float delay)
	{
		if (instance.delaySaveThread != null && instance.delaySaveThread.IsRunning)
		{
			instance.delaySaveThread.Stop();
			SafeDebug.Log("<color=#8b1fba>Cancelling Existing Save Sequence Thread</color>");
		}
		instance.delaySaveThread = GlobalCoroutineContainer.AddThread(instance.delaySaveSequence(delay), instance);
	}

	private IEnumerator delaySaveSequence(float delay)
	{
		SafeDebug.Log("<color=#8b1fba>Beginning Delayed Save Sequence!</color>");
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(delay, this);
		while (EagerScenes.instance.GetIsLoading())
		{
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(delay, this);
		}
		PersistentDataManager.WriteActiveSaveToDisk();
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			SafeDebug.Log("<color=green>Saving from OnApplicationPause() at Time.time " + Time.time + " !</color>");
			tryWriteSave();
		}
	}

	private void OnApplicationQuit()
	{
		SafeDebug.Log("<color=green>Saving from OnApplicationQuit() " + Time.time + " !</color>");
		tryWriteSave();
	}

	private void tryWriteSave()
	{
		if (Time.time - lastSaveTimestamp > 1f)
		{
			if (EagerScenes.instance != null && EagerScenes.instance.ActiveScene != null)
			{
				PersistentDataManager.TidySaveValues(EagerScenes.instance.ActiveScene);
			}
			PersistentDataManager.WriteActiveSaveToDisk();
		}
		else
		{
			SafeDebug.Log("<color=green>Attempted Save canceled; too recent in timestamp to the last successful save</color>");
		}
		lastSaveTimestamp = Time.time;
	}
}
