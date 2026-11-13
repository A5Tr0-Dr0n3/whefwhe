using System.Collections;
using UnityEngine;

public class VVR_DeadChaz : SceneBehaviorManager
{
	[SerializeField]
	private GvrAudioSource[] chazLines;

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		GlobalCoroutineContainer.AddThread(DeadChazCoroutine(), this);
	}

	private IEnumerator DeadChazCoroutine()
	{
		yield return null;
		int i = 0;
		while (true)
		{
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(Random.Range(5f, 10f), this);
			chazLines[i].Play();
			while (chazLines[i].isPlaying)
			{
				yield return null;
			}
			i++;
			if (i >= chazLines.Length)
			{
				i = 0;
			}
		}
	}
}
