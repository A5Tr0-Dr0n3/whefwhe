using System;
using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using UnityEngine;

public class GlobalCoroutineContainer : MonoBehaviour
{
	[Serializable]
	private class GlobalCoroutine
	{
		public SafeCoroutine thread;

		public MonoBehaviour owner;

		public bool pausedByGCC;

		public string threadName;

		public GlobalCoroutine(SafeCoroutine threadIn, MonoBehaviour ownerIn, string nameIn)
		{
			thread = threadIn;
			owner = ownerIn;
			pausedByGCC = false;
			threadName = nameIn;
		}
	}

	public static GlobalCoroutineContainer instance;

	private bool allThreadsPaused;

	private List<GlobalCoroutine> threads = new List<GlobalCoroutine>();

	private List<GlobalCoroutine> pausedThreads = new List<GlobalCoroutine>();

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			return;
		}
		SafeDebug.LogWarning("Destroying duplicate GlobalCoroutineContainer!");
		UnityEngine.Object.Destroy(this);
	}

	private void Start()
	{
		EagerScenes.instance.OnSceneActivated += reactToSceneActivated;
	}

	public static SafeCoroutine AddGlobalThread(IEnumerator newThread)
	{
		if (instance == null)
		{
			SafeDebug.LogError("Trying to add global coroutine thread before GlobalCoroutineContainer is instantiated!");
		}
		return AddThread(newThread, instance);
	}

	public static SafeCoroutine AddThread(IEnumerator newThread, MonoBehaviour owner)
	{
		if (instance != null)
		{
			SafeCoroutine safeCoroutine = instance.StartSafeCoroutine(newThread);
			instance.threads.Add(new GlobalCoroutine(safeCoroutine, owner, newThread.ToString()));
			return safeCoroutine;
		}
		SafeDebug.LogError("Trying to add coroutine thread before GlobalCoroutineContainer is instantiated!");
		return null;
	}

	public static SafeCoroutine SafeWaitForSeconds(float time, MonoBehaviour owner)
	{
		return instance.StartSafeCoroutine(safeWaitForSecondsSequence(time, owner));
	}

	private static IEnumerator safeWaitForSecondsSequence(float time, MonoBehaviour owner)
	{
		float t = 0f;
		while (t < time && !(owner == null))
		{
			if (owner.enabled && owner.gameObject.activeInHierarchy)
			{
				t += Time.deltaTime;
			}
			yield return null;
		}
	}

	private void Update()
	{
		if (!allThreadsPaused)
		{
			ManageThreads();
		}
	}

	private void reactToSceneActivated(EagerScene oldScene, EagerScene newScene)
	{
		ManageThreads();
	}

	public void PauseAllThreads()
	{
		allThreadsPaused = true;
		for (int i = 0; i < threads.Count; i++)
		{
			if (!threads[i].pausedByGCC && threads[i].thread.State == SafeCoroutineState.Running)
			{
				threads[i].pausedByGCC = true;
				threads[i].thread.Pause();
				pausedThreads.Add(threads[i]);
			}
		}
	}

	public void ResumeAllThreads()
	{
		allThreadsPaused = false;
		for (int i = 0; i < pausedThreads.Count; i++)
		{
			pausedThreads[i].pausedByGCC = false;
			pausedThreads[i].thread.Resume();
			pausedThreads.RemoveAt(i);
		}
	}

	public void ManageThreads()
	{
		for (int i = 0; i < threads.Count; i++)
		{
			if (threads[i].owner != null && !threads[i].thread.IsStopped && !threads[i].thread.HasFinished)
			{
				if ((!threads[i].owner.enabled || !threads[i].owner.gameObject.activeInHierarchy) && !threads[i].pausedByGCC && threads[i].thread.IsRunning)
				{
					threads[i].pausedByGCC = true;
					threads[i].thread.Pause();
				}
				if (threads[i].owner.enabled && threads[i].owner.gameObject.activeInHierarchy && threads[i].pausedByGCC && threads[i].thread.IsPaused)
				{
					threads[i].pausedByGCC = false;
					threads[i].thread.Resume();
				}
			}
			else
			{
				if (threads[i].thread.IsRunning)
				{
					threads[i].thread.Stop();
				}
				threads.RemoveAt(i);
				i--;
			}
		}
	}
}
