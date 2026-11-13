using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HurryUpDialogueUnit
{
	[Serializable]
	public class HurryUpDialogue
	{
		public float time;

		[SerializeField]
		private NPCDialogueClip clip;

		public bool fired;

		public NPCDialogueClip Fire()
		{
			fired = true;
			return clip;
		}
	}

	[SerializeField]
	private List<HurryUpDialogue> hurryUps;

	private float timer;

	private DialoguePlaybackInstance dialogueInstance;

	public bool hasFiredOnce { get; private set; }

	public void Reset()
	{
		hasFiredOnce = false;
		timer = 0f;
		dialogueInstance = null;
		for (int i = 0; i < hurryUps.Count; i++)
		{
			hurryUps[i].fired = false;
		}
	}

	public void Tick(float deltaTime, VVRDialogueSource dSource)
	{
		if (hasFiredOnce && dialogueInstance != null && dialogueInstance.playbackProgress != DialoguePlaybackProgress.finished)
		{
			return;
		}
		timer += deltaTime;
		for (int i = 0; i < hurryUps.Count; i++)
		{
			if (!hurryUps[i].fired && timer >= hurryUps[i].time)
			{
				dialogueInstance = dSource.InterruptWithClip(hurryUps[i].Fire());
				hasFiredOnce = true;
				break;
			}
		}
	}
}
