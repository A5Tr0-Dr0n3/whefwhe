using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using UnityEngine;

public class JobGardenNight : SceneBehaviorJob
{
	[Header("References:")]
	[SerializeField]
	private GardenNightScriptableObject gardenScriptable;

	[SerializeField]
	private VVRDialogueSource sprinklesDialogue;

	[SerializeField]
	private GvrAudioSource sprinklesAudioSource;

	[SerializeField]
	private ObjectPinwheel sprinkles;

	[SerializeField]
	private ObjectTrigger playerTrigger;

	[Header("Sweeping Sequence:")]
	[SerializeField]
	private ObjectBroom broom;

	[SerializeField]
	private List<GameObject> dirtPiles;

	[SerializeField]
	private Transform turnAroundTarget;

	private Vector3 dirtPileSize;

	private SafeCoroutine mainCoroutine;

	private float vacuumFill;

	private bool vacuumingLevel;

	private int dirtRemaining;

	private int targetDirtForReset = 3;

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		vacuumFill = EagerMaster.instance.vacuumFill;
		mainCoroutine = GlobalCoroutineContainer.AddThread(GardenNightCoroutine(), this);
		GlobalCoroutineContainer.AddThread(SprinklesAliveCheck(), this);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_BACKSTAGE_ELEVATOR);
	}

	private void OnEnable()
	{
	}

	private void Start()
	{
		dirtPileSize = dirtPiles[0].transform.localScale;
	}

	private IEnumerator GardenNightCoroutine()
	{
		yield return null;
		sprinklesDialogue.PlayClip(gardenScriptable.dialogue.wCryLoop);
		sprinklesAudioSource.loop = true;
		while (!playerTrigger.playerInside)
		{
			yield return null;
		}
		sprinklesAudioSource.loop = false;
		sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.wWhatAreYouDoingHere);
		sprinklesDialogue.PlayClip(gardenScriptable.dialogue.wItDoesntEvenMatter);
		sprinklesDialogue.PlayClip(gardenScriptable.dialogue.wTakeTheBroom);
		while (sprinklesDialogue.isPlaying)
		{
			yield return null;
		}
		sprinkles.lookTarget = turnAroundTarget;
		sprinklesDialogue.PlayClip(gardenScriptable.dialogue.wDeadToMe);
		while (!ToolManager.PlayerGrabbingGameObject(broom.gameObject))
		{
			yield return null;
		}
		sprinklesDialogue.PlayClip(gardenScriptable.dialogue.sStartSweeping);
		sprinklesDialogue.PlayClip(gardenScriptable.dialogue.sSideToSide);
		sprinklesDialogue.PlayClip(gardenScriptable.dialogue.sInsideAndOut);
		sprinklesDialogue.PlayClip(gardenScriptable.dialogue.sSweepFaster);
		sprinklesDialogue.PlayClip(gardenScriptable.dialogue.sSweepUpYourShame);
		bool dirtSweptUp = false;
		while (!dirtSweptUp)
		{
			dirtSweptUp = true;
			dirtRemaining = 0;
			for (int i = 0; i < dirtPiles.Count; i++)
			{
				if (dirtPiles[i] != null && dirtPiles[i].activeInHierarchy && dirtPiles[i].GetComponent<Renderer>().bounds.size.magnitude > 0.5f)
				{
					dirtRemaining++;
					if (dirtRemaining > targetDirtForReset)
					{
						dirtSweptUp = false;
						break;
					}
				}
			}
			yield return null;
		}
		sprinklesAudioSource.loop = false;
		broom.broomCanSweep = false;
		sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.dYouThinkYoureDone);
		while (sprinklesDialogue.isPlaying)
		{
			yield return null;
		}
		float lerpTime = 0f;
		while (lerpTime < 1f)
		{
			lerpTime += Time.deltaTime;
			for (int j = 0; j < dirtPiles.Count; j++)
			{
				if (dirtPiles[j] != null)
				{
					dirtPiles[j].gameObject.SetActive(value: true);
					dirtPiles[j].transform.localScale = Vector3.Lerp(Vector3.zero, dirtPileSize, lerpTime);
				}
			}
			yield return null;
		}
		sprinklesAudioSource.loop = true;
		sprinklesDialogue.PlayClip(gardenScriptable.dialogue.dDirtDirtDirt);
		yield return null;
	}

	private IEnumerator SprinklesAliveCheck()
	{
		yield return null;
		while (sprinkles != null)
		{
			if (!vacuumingLevel && EagerMaster.instance.vacuumFill != vacuumFill)
			{
				if (mainCoroutine != null && mainCoroutine.IsRunning)
				{
					mainCoroutine.Stop();
					mainCoroutine = null;
				}
				vacuumingLevel = true;
				sprinklesAudioSource.loop = false;
				sprinklesDialogue.ForcePlayClipOverwrite(gardenScriptable.dialogue.vNoooo);
			}
			else if (vacuumingLevel && !sprinklesDialogue.isPlaying)
			{
				sprinklesDialogue.PlayClip(gardenScriptable.dialogue.vWhy);
			}
			yield return null;
		}
		if (mainCoroutine != null && mainCoroutine.IsRunning)
		{
			mainCoroutine.Stop();
			mainCoroutine = null;
		}
		GoToBackstage();
	}
}
