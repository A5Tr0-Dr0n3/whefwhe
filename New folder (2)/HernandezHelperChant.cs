using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using UnityEngine;

public class HernandezHelperChant : MonoBehaviour
{
	[SerializeField]
	private HernandezTweakablesScriptableObject tweakables;

	private VVRDialogueSource mDialogueSource;

	private GvrAudioSource mAudioSource;

	private List<NPCDialogueClip> chantingList;

	private int chantIndex;

	private SafeCoroutine chantThread;

	private const float chantInterval = 2f;

	private const float minPitch = 0.9f;

	private const float maxPitch = 1.1f;

	private void Start()
	{
		mDialogueSource = GetComponent<VVRDialogueSource>();
		mAudioSource = GetComponent<GvrAudioSource>();
	}

	private void lerpPitch(float t)
	{
		mAudioSource.pitch = Mathf.Lerp(0.9f, 1.1f, t);
	}

	private void retrieveAndRandomizeChantList()
	{
		chantingList = new List<NPCDialogueClip>();
		chantingList.AddRange(tweakables.helperDialogue.pluckHimChantClips);
		List<NPCDialogueClip> list = new List<NPCDialogueClip>();
		while (chantingList.Count > 0)
		{
			int index = Random.Range(0, chantingList.Count);
			list.Add(chantingList[index]);
			chantingList.RemoveAt(index);
		}
		chantingList = list;
	}

	private IEnumerator chantSequence()
	{
		yield return new WaitForSeconds(Random.Range(0f, 2f));
		while (true)
		{
			mDialogueSource.PlayClip(chantingList[chantIndex]);
			chantIndex = (chantIndex + 1) % chantingList.Count;
			while (mDialogueSource.isPlaying)
			{
				yield return null;
			}
			yield return new WaitForSeconds(2f);
		}
	}

	public void StartChanting(float indexT)
	{
		StopChanting();
		lerpPitch(indexT);
		retrieveAndRandomizeChantList();
		chantThread = GlobalCoroutineContainer.AddThread(chantSequence(), this);
	}

	public void StopChanting()
	{
		if (chantThread != null && !chantThread.IsStopped)
		{
			chantThread.Stop();
		}
	}

	public void Celebrate()
	{
		StopChanting();
		mDialogueSource.ForcePlayClipOverwrite(tweakables.helperDialogue.hernandezDeathCelebration);
	}
}
