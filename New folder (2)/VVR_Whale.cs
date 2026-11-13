using System;
using System.Collections;
using UnityEngine;

public class VVR_Whale : SceneBehaviorManager
{
	public enum WhaleStates
	{
		CleaningBernice,
		CompleteBarnacles,
		PostHeadset
	}

	[SerializeField]
	private WhaleStates whaleState;

	public WhaleSequenceScriptableObject tweakables;

	public GameObject whale;

	private Material whaleMaterial;

	private Material barnacleMaterial;

	public ObjectPlug[] plugs;

	public ObjectPlug[] plugsSecondVisit;

	public Transform[] outlets;

	public VVRConsumable[] barnacles;

	public Renderer[] partiallyConsumedBarnacles;

	public ObjectBase headset;

	public GameObject headsetEffect;

	[SerializeField]
	private VVRDialogueSource whaleDialogueSource;

	public AudioLoop cleanMusic;

	public AudioLoop glitchMusic;

	private bool whaleCompleted;

	private bool cableInterrupt;

	private bool gaveCableInterrupt;

	private bool cableTaskCompleted;

	private bool barnaclePendingFeedback;

	private float allowBarnacleFeedbackTime = 10f;

	private float allowBarnacleFeedbackCount;

	private bool giveHint;

	private int currentPlugCount;

	private int currentBarnacleCount;

	private float delayTransitionTime = 0.8f;

	private float delayTransitionCountdown;

	private float currentTransitionTime;

	private float paddingTimer;

	private bool secondVisit;

	private Vector3 headsetStartPosition;

	private Quaternion headsetStartRotation;

	public static VVR_Whale instance;

	private void Awake()
	{
		if (instance != null)
		{
			throw new UnityException("Only one instance of VVR_Whale in a scene is allowed.");
		}
		instance = this;
	}

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_BOID_DREAM);
		SetUpSceneObjects();
		GlobalCoroutineContainer.AddThread(WhaleCoroutine(), this);
	}

	protected override void OnSceneActivated()
	{
		if (PersistentDataManager.GetArchiveState() > VVR_Archive.ArchiveStates.Bernice)
		{
			secondVisit = true;
		}
	}

	private void SetUpSceneObjects()
	{
		cleanMusic.Play();
		glitchMusic.Play();
		headsetStartPosition = headset.transform.position;
		headsetStartRotation = headset.transform.rotation;
		whaleMaterial = whale.GetComponent<Renderer>().sharedMaterial;
		barnacleMaterial = barnacles[0].GetComponent<Renderer>().sharedMaterial;
		if (PersistentDataManager.GetArchiveState() > VVR_Archive.ArchiveStates.Bernice)
		{
			secondVisit = true;
		}
		if (!secondVisit)
		{
			for (int i = 0; i < plugsSecondVisit.Length; i++)
			{
				plugsSecondVisit[i].transform.parent.gameObject.SetActive(value: false);
			}
			plugs[0].PlugInImmediate(outlets[0]);
			for (int j = 0; j < barnacles.Length; j++)
			{
				barnacles[j].gameObject.GetComponent<Collider>().enabled = false;
			}
			currentPlugCount = getPlugCount();
			currentBarnacleCount = getBarnacleVacuumCount();
			whaleState = WhaleStates.CleaningBernice;
		}
		else
		{
			for (int k = 0; k < plugs.Length; k++)
			{
				plugs[k].transform.parent.gameObject.SetActive(value: false);
			}
			plugsSecondVisit[0].PlugInImmediate(outlets[1]);
			plugsSecondVisit[1].PlugInImmediate(outlets[2]);
			plugsSecondVisit[2].PlugInImmediate(outlets[0]);
			currentPlugCount = outlets.Length;
			currentBarnacleCount = barnacles.Length;
			clearAllBarnacles();
			headset.gameObject.SetActive(value: true);
			headsetEffect.gameObject.SetActive(value: true);
			whaleState = WhaleStates.PostHeadset;
			secondVisit = true;
			paddingTimer = tweakables.berniceMonologuePrePadding;
		}
		DistortWhale();
	}

	private void clearAllBarnacles()
	{
		for (int i = 0; i < barnacles.Length; i++)
		{
			barnacles[i].gameObject.GetComponent<Collider>().enabled = false;
			barnacles[i].gameObject.GetComponent<Renderer>().enabled = false;
		}
	}

	private void Update()
	{
		switch (currentPlugCount)
		{
		case 0:
			glitchMusic.SetSourceVolume(1f);
			cleanMusic.SetSourceVolume(0f);
			break;
		case 1:
			glitchMusic.SetSourceVolume(0.9f);
			cleanMusic.SetSourceVolume(0.1f);
			break;
		case 2:
			glitchMusic.SetSourceVolume(0.6f);
			cleanMusic.SetSourceVolume(0.3f);
			break;
		case 3:
			glitchMusic.SetSourceVolume(0f);
			cleanMusic.SetSourceVolume(1f);
			break;
		}
	}

	private IEnumerator WhaleCoroutine()
	{
		if (whaleState == WhaleStates.CleaningBernice)
		{
			yield return new WaitForSeconds(tweakables.berniceMonologuePrePadding);
			barnaclePendingFeedback = false;
			for (int i = 0; i < tweakables.dialogue.aBerniceMonologue.Length; i++)
			{
				whaleDialogueSource.PlayClip(tweakables.dialogue.aBerniceMonologue[i]);
				while (whaleDialogueSource.isPlaying && whaleState == WhaleStates.CleaningBernice)
				{
					if (currentPlugCount != getPlugCount() && !whaleCompleted)
					{
						if (currentPlugCount < getPlugCount())
						{
							delayTransitionCountdown = delayTransitionTime;
						}
						currentPlugCount = getPlugCount();
						if (getPlugCount() < plugs.Length)
						{
							currentTransitionTime = tweakables.transitionTime;
						}
						else
						{
							currentTransitionTime = tweakables.completeTransitionTime;
						}
					}
					UpdateWhaleDistortion();
					if (getPlugCount() > 1 && !gaveCableInterrupt)
					{
						cableInterrupt = true;
					}
					if (!cableTaskCompleted && getPlugCount() >= outlets.Length)
					{
						cableTaskCompleted = true;
						cableInterrupt = true;
					}
					if (getBarnacleVacuumCount() > currentBarnacleCount && allowBarnacleFeedbackCount <= 0f)
					{
						barnaclePendingFeedback = true;
						allowBarnacleFeedbackCount = allowBarnacleFeedbackTime;
						currentBarnacleCount = getBarnacleVacuumCount();
					}
					if (cableInterrupt && tweakables.dialogue.aBerniceMonologue[i].mInterruptability != Interruptability.uninterruptable && delayTransitionCountdown <= 0f)
					{
						if (!gaveCableInterrupt)
						{
							switch (i)
							{
							case 4:
								whaleDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.aFinishPluggingCables);
								whaleDialogueSource.InterruptWithClip(tweakables.dialogue.aBerniceMonologueInterruptions[Mathf.FloorToInt(UnityEngine.Random.Range(0, tweakables.dialogue.aBerniceMonologueInterruptions.Length))], enqueueIfCantInterrupt: true, stackInterruptions: true);
								whaleDialogueSource.PlayClip(tweakables.dialogue.aBerniceMonologue[i]);
								break;
							case 2:
								whaleDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.aFinishPluggingCables);
								break;
							default:
								whaleDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.aFinishPluggingCables);
								if (i < tweakables.dialogue.aBerniceMonologue.Length - 1)
								{
									whaleDialogueSource.InterruptWithClip(tweakables.dialogue.aBerniceMonologueInterruptions[Mathf.FloorToInt(UnityEngine.Random.Range(0, tweakables.dialogue.aBerniceMonologueInterruptions.Length))], enqueueIfCantInterrupt: true, stackInterruptions: true);
								}
								break;
							}
							gaveCableInterrupt = true;
						}
						else
						{
							whaleDialogueSource.InterruptWithClip(tweakables.dialogue.aFinalPlugLine);
						}
						cableInterrupt = false;
					}
					if (barnaclePendingFeedback && tweakables.dialogue.aBerniceMonologue[i].mInterruptability != Interruptability.uninterruptable)
					{
						if (!whaleDialogueSource.isPlayingInterruption())
						{
							whaleDialogueSource.InterruptWithClip(tweakables.dialogue.bBarnacleInterruptions[Mathf.FloorToInt(UnityEngine.Random.Range(0, tweakables.dialogue.bBarnacleInterruptions.Length))]);
							whaleDialogueSource.InterruptWithClip(tweakables.dialogue.aBerniceMonologueInterruptions[Mathf.FloorToInt(UnityEngine.Random.Range(0, tweakables.dialogue.aBerniceMonologueInterruptions.Length))], enqueueIfCantInterrupt: true, stackInterruptions: true);
						}
						barnaclePendingFeedback = false;
					}
					if ((float)getBarnacleVacuumCount() >= Mathf.Floor((float)barnacles.Length * tweakables.barnacleCompletionPercentageDuringMonologue))
					{
						whaleState = WhaleStates.CompleteBarnacles;
					}
					allowBarnacleFeedbackCount -= Time.deltaTime;
					yield return null;
				}
				if (i != 2 || cableTaskCompleted)
				{
					continue;
				}
				int currentCableLine = 0;
				float delayTimer = tweakables.cablePleaPadding;
				while (!cableTaskCompleted)
				{
					if (currentPlugCount != getPlugCount() && !whaleCompleted)
					{
						if (currentPlugCount < getPlugCount())
						{
							delayTransitionCountdown = delayTransitionTime;
						}
						currentPlugCount = getPlugCount();
						if (getPlugCount() < plugs.Length)
						{
							currentTransitionTime = tweakables.transitionTime;
						}
						else
						{
							currentTransitionTime = tweakables.completeTransitionTime;
						}
					}
					UpdateWhaleDistortion();
					if (getPlugCount() > 1 && !gaveCableInterrupt && delayTransitionCountdown <= 0f)
					{
						whaleDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.aFinishPluggingCables);
						gaveCableInterrupt = true;
					}
					if (getPlugCount() >= outlets.Length && delayTransitionCountdown <= 0f)
					{
						cableTaskCompleted = true;
						whaleDialogueSource.InterruptWithClip(tweakables.dialogue.aFinalPlugLine);
					}
					if (!whaleDialogueSource.isPlaying)
					{
						delayTimer -= Time.deltaTime;
						if (delayTimer <= 0f)
						{
							if (currentCableLine == 2 && gaveCableInterrupt)
							{
								currentCableLine++;
							}
							whaleDialogueSource.PlayClip(tweakables.dialogue.aWaitForCables[currentCableLine]);
							if (currentCableLine < tweakables.dialogue.aWaitForCables.Length - 1)
							{
								currentCableLine++;
								delayTimer = tweakables.cablePleaPadding;
							}
							else
							{
								delayTimer = tweakables.cablePleaRepeatTime;
							}
						}
					}
					yield return null;
				}
			}
			paddingTimer = tweakables.barnaclePleaPadding;
			while ((float)getBarnacleVacuumCount() < Mathf.Floor((float)barnacles.Length * tweakables.barnacleCompletionPercentage))
			{
				if (currentPlugCount != getPlugCount() && !whaleCompleted)
				{
					if (currentPlugCount < getPlugCount())
					{
						delayTransitionCountdown = delayTransitionTime;
					}
					currentPlugCount = getPlugCount();
					if (getPlugCount() < plugs.Length)
					{
						currentTransitionTime = tweakables.transitionTime;
					}
					else
					{
						currentTransitionTime = tweakables.completeTransitionTime;
					}
				}
				UpdateWhaleDistortion();
				if (!cableTaskCompleted && getPlugCount() >= outlets.Length)
				{
					cableInterrupt = true;
					cableTaskCompleted = true;
				}
				if (cableInterrupt && delayTransitionCountdown <= 0f)
				{
					whaleDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.aFinishPluggingCables);
					cableInterrupt = false;
					paddingTimer = tweakables.barnaclePleaPadding;
				}
				if (getBarnacleVacuumCount() > currentBarnacleCount && allowBarnacleFeedbackCount <= 0f)
				{
					barnaclePendingFeedback = true;
					allowBarnacleFeedbackCount = allowBarnacleFeedbackTime;
					currentBarnacleCount = getBarnacleVacuumCount();
				}
				if (barnaclePendingFeedback)
				{
					whaleDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.bBarnacleInterruptions[Mathf.FloorToInt(UnityEngine.Random.Range(0, tweakables.dialogue.bBarnacleInterruptions.Length))]);
					barnaclePendingFeedback = false;
					paddingTimer = tweakables.barnacleEncouragementPadding;
				}
				if (paddingTimer > 0f)
				{
					if (!whaleDialogueSource.isPlaying)
					{
						paddingTimer -= Time.deltaTime;
					}
				}
				else if (getBarnacleVacuumCount() <= 0)
				{
					whaleDialogueSource.PlayClip(tweakables.dialogue.bBarnaclePleas[Mathf.FloorToInt(UnityEngine.Random.Range(0, tweakables.dialogue.bBarnaclePleas.Length))]);
					paddingTimer = tweakables.barnaclePleaPadding;
				}
				else
				{
					whaleDialogueSource.PlayClip(tweakables.dialogue.bBarnacleEncouragement[Mathf.FloorToInt(UnityEngine.Random.Range(0, tweakables.dialogue.bBarnacleEncouragement.Length))]);
					paddingTimer = tweakables.barnacleEncouragementPadding;
				}
				allowBarnacleFeedbackCount -= Time.deltaTime;
				yield return null;
			}
			whaleState = WhaleStates.CompleteBarnacles;
		}
		if (whaleState == WhaleStates.CompleteBarnacles)
		{
			paddingTimer = 0f;
			for (int j = 0; j < tweakables.dialogue.cBerniceThanks.Length; j++)
			{
				while (paddingTimer > 0f)
				{
					paddingTimer -= Time.deltaTime;
					yield return null;
				}
				whaleDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.cBerniceThanks[j]);
				paddingTimer = tweakables.dialogue.cBerniceThanks[j].postPadding;
				if (j == tweakables.dialogue.cBerniceThanks.Length - 1)
				{
					headset.gameObject.SetActive(value: true);
					headsetEffect.gameObject.SetActive(value: true);
					PersistentDataManager.OverwriteArchiveState(VVR_Archive.ArchiveStates.Hernandez);
				}
				while (whaleDialogueSource.isPlaying)
				{
					yield return null;
				}
			}
			paddingTimer = tweakables.postHeadSetDialoguePadding;
			giveHint = true;
			whaleState = WhaleStates.PostHeadset;
		}
		while (whaleState == WhaleStates.PostHeadset)
		{
			if (paddingTimer > 0f)
			{
				if (!whaleDialogueSource.isPlaying)
				{
					paddingTimer -= Time.deltaTime;
				}
				if (headsetEffect.activeSelf && ToolManager.PlayerGrabbingObjectBase(headset))
				{
					headsetEffect.SetActive(value: false);
				}
				yield return null;
				continue;
			}
			if (secondVisit)
			{
				if (giveHint)
				{
					whaleDialogueSource.PlayClip(tweakables.dialogue.dBerniceHints[Mathf.FloorToInt(UnityEngine.Random.Range(0, tweakables.dialogue.dBerniceHints.Length))]);
				}
				else
				{
					whaleDialogueSource.PlayClip(tweakables.dialogue.dBernicePassDialogue[Mathf.FloorToInt(UnityEngine.Random.Range(0, tweakables.dialogue.dBernicePassDialogue.Length))]);
				}
				giveHint = !giveHint;
			}
			else
			{
				whaleDialogueSource.PlayClip(tweakables.dialogue.dBerniceHints[Mathf.FloorToInt(UnityEngine.Random.Range(0, tweakables.dialogue.dBerniceHints.Length))]);
			}
			paddingTimer = tweakables.postHeadSetDialoguePadding;
		}
	}

	private void DistortWhale()
	{
		float num = 0f;
		int num2 = 0;
		num = ((whaleState != WhaleStates.PostHeadset) ? getPlugCount() : 3) switch
		{
			1 => tweakables.blockyOneCableDistortion, 
			2 => tweakables.blockyTwoCabeDistortion, 
			3 => tweakables.blockyComplete, 
			_ => tweakables.blockyMinDistortion, 
		};
		whaleMaterial.SetFloat("_BlockScale", num);
		barnacleMaterial.SetFloat("_BlockScale", num);
	}

	private void UpdateWhaleDistortion()
	{
		if (whaleCompleted)
		{
			return;
		}
		if (currentTransitionTime > 0f)
		{
			if (delayTransitionCountdown > 0f)
			{
				delayTransitionCountdown -= Time.deltaTime;
				return;
			}
			currentTransitionTime -= Time.deltaTime;
			float num = 0f;
			num = ((getPlugCount() >= plugs.Length) ? ((tweakables.completeTransitionTime - currentTransitionTime) / tweakables.completeTransitionTime) : ((tweakables.transitionTime - currentTransitionTime) / tweakables.transitionTime));
			num = Mathf.Sin(num * (float)Math.PI * 0.5f);
			float a = whaleMaterial.GetFloat("_BlockScale");
			a = getPlugCount() switch
			{
				1 => Mathf.Lerp(a, tweakables.blockyOneCableDistortion, num), 
				2 => Mathf.Lerp(a, tweakables.blockyTwoCabeDistortion, num), 
				3 => Mathf.Lerp(a, tweakables.blockyComplete, num), 
				_ => Mathf.Lerp(a, tweakables.blockyMinDistortion, num), 
			};
			whaleMaterial.SetFloat("_BlockScale", a);
			barnacleMaterial.SetFloat("_BlockScale", a);
		}
		else if (getPlugCount() >= plugs.Length)
		{
			whaleCompleted = true;
			whaleMaterial.SetFloat("_BlockScale", tweakables.blockyMassive);
			barnacleMaterial.SetFloat("_BlockScale", tweakables.blockyMassive);
			for (int i = 0; i < barnacles.Length; i++)
			{
				barnacles[i].gameObject.GetComponent<Collider>().enabled = true;
			}
		}
	}

	private int getPlugCount()
	{
		int num = 0;
		bool flag = true;
		for (int i = 0; i < plugs.Length; i++)
		{
			flag = true;
			if (!plugs[i].pluggedIn)
			{
				continue;
			}
			for (int j = 0; j < plugs.Length; j++)
			{
				if (plugs[j].pluggedIn && j > i && plugs[j].targetTransformReference == plugs[i].targetTransformReference)
				{
					flag = false;
				}
			}
			if (flag)
			{
				num++;
			}
		}
		return num;
	}

	private float getPlugCompletion()
	{
		float num = 0f;
		float num2 = 0f;
		bool flag = true;
		for (int i = 0; i < plugs.Length; i++)
		{
			flag = true;
			if (!plugs[i].pluggedIn)
			{
				continue;
			}
			for (int j = 0; j < plugs.Length; j++)
			{
				if (plugs[j].pluggedIn && j > i && plugs[j].targetTransformReference == plugs[i].targetTransformReference)
				{
					flag = false;
				}
			}
			if (flag)
			{
				num2++;
			}
		}
		return num2 / ((float)plugs.Length * 1f - 1f);
	}

	private int getBarnacleVacuumCount()
	{
		int num = 0;
		for (int i = 0; i < barnacles.Length; i++)
		{
			if (barnacles[i].currentObjectState != VVRConsumable.objectState.wholeStatic && barnacles[i].currentObjectState != VVRConsumable.objectState.wholeShaking)
			{
				num++;
			}
		}
		return num;
	}

	private void distortPartialBarnacles()
	{
		for (int i = 0; i < partiallyConsumedBarnacles.Length; i++)
		{
			partiallyConsumedBarnacles[i].material.SetFloat("_SuckUp", 0.66f);
		}
	}

	public void ResetHeadset()
	{
		headset.transform.position = headsetStartPosition;
		headset.transform.rotation = headsetStartRotation;
		headset.rb.useGravity = false;
		headset.rb.velocity = Vector3.zero;
		headset.rb.angularVelocity = Vector3.zero;
		headsetEffect.gameObject.SetActive(value: true);
	}
}
