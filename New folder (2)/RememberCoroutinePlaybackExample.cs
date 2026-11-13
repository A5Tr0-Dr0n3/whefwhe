using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RememberCoroutinePlaybackExample : MonoBehaviour
{
	private List<IEnumerator> coroutinePool;

	private void Start()
	{
		coroutinePool = new List<IEnumerator>
		{
			coroutine1(),
			coroutine2(),
			coroutine3(),
			coroutine4(),
			coroutine5()
		};
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			PlayRandomCoroutine();
		}
	}

	public void PlayRandomCoroutine()
	{
		if (coroutinePool.Count > 0)
		{
			IEnumerator enumerator = coroutinePool[Random.Range(0, coroutinePool.Count)];
			if (PersistentDataManager.HasPlayedCoroutine(enumerator))
			{
				SafeDebug.Log("<color=green>Skipped " + PersistentDataManager.IEnumeratorToString(enumerator) + "!</color>");
				coroutinePool.Remove(enumerator);
				PlayRandomCoroutine();
			}
			else
			{
				GlobalCoroutineContainer.AddThread(enumerator, this);
				PersistentDataManager.MarkCoroutinePlayed(enumerator);
			}
		}
	}

	private IEnumerator coroutine1()
	{
		SafeDebug.Log("Coroutine 1");
		yield return null;
	}

	private IEnumerator coroutine2()
	{
		SafeDebug.Log("Coroutine 2");
		yield return null;
	}

	private IEnumerator coroutine3()
	{
		SafeDebug.Log("Coroutine 3");
		yield return null;
	}

	private IEnumerator coroutine4()
	{
		SafeDebug.Log("Coroutine 4");
		yield return null;
	}

	private IEnumerator coroutine5()
	{
		SafeDebug.Log("Coroutine 5");
		yield return null;
	}
}
