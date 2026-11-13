using System.Collections;
using Edelweiss.Coroutine;
using GVR.Gfx;
using UnityEngine;

public class VVR_JobEndlessDesire : SceneBehaviorJob
{
	public EndlessDesireSequenceScriptableObject tweakables;

	[SerializeField]
	private GameObject butter;

	[SerializeField]
	private ObjectToaster[] kitchenToasters;

	[SerializeField]
	private VVRDialogueSource butterDialogueSource;

	[SerializeField]
	private EnvironmentProperties kitchenEnviroProperties;

	[SerializeField]
	private MaterialUpdateController kitchenMaterialProperties;

	[SerializeField]
	private EnvironmentProperties[] otherEnviroProperties;

	[SerializeField]
	private MaterialUpdateController[] otherMaterialProperties;

	[SerializeField]
	private VVRConsumable butterEatTrigger;

	public VVRConsumable[] invincibles;

	public GameObject butterToast;

	public GameObject butterCarrot;

	public GameObject[] randObj;

	public Transform[] randTrans;

	[SerializeField]
	private VVRConsumable kitchenTable;

	private SafeCoroutine endlessThread;

	private int introIndex;

	public static VVR_JobEndlessDesire instance;

	protected override void OnFirstTimeSceneActivated()
	{
		if (instance != null)
		{
			throw new UnityException("Only one instance of Endless Desire in a scene is allowed.");
		}
		instance = this;
		introIndex = PersistentDataManager.GetKitchenLoopCounter();
		switch (introIndex)
		{
		case 0:
			butterToast.SetActive(value: true);
			break;
		case 1:
			butterCarrot.SetActive(value: true);
			break;
		default:
		{
			for (int i = 0; i < 5; i++)
			{
				randObj[Random.Range(0, randObj.Length)].transform.position = randTrans[Random.Range(0, randTrans.Length)].position;
			}
			int num = Random.Range(0, otherEnviroProperties.Length);
			kitchenEnviroProperties.gameObject.SetActive(value: false);
			otherEnviroProperties[num].gameObject.SetActive(value: true);
			GetComponent<EagerScene>().mEnvironmentProperties = otherEnviroProperties[num];
			GetComponent<EagerScene>().defaultMaterialProperties = otherMaterialProperties[num];
			otherEnviroProperties[num].pushAllProperties();
			break;
		}
		}
		for (int j = 0; j < kitchenToasters.Length; j++)
		{
			kitchenToasters[j].enabled = false;
		}
		for (int k = 0; k < invincibles.Length; k++)
		{
			if (invincibles[k] != null)
			{
				invincibles[k].unconsumable = true;
			}
		}
		base.OnFirstTimeSceneActivated();
		endlessThread = GlobalCoroutineContainer.AddThread(endlessCoroutine(), this);
		butterEatTrigger.unconsumable = true;
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_BACKSTAGE_ELEVATOR);
	}

	private void Update()
	{
		if (endlessThread != null && endlessThread.IsRunning && butter.GetComponent<VVRConsumable>().isBeingAffectedByVacuum)
		{
			endlessThread.Stop();
			GlobalCoroutineContainer.AddThread(VacuumCoroutine(), this);
		}
	}

	private IEnumerator endlessCoroutine()
	{
		float timer = 0f;
		while (!EagerScenes.instance.ActiveScene || !EagerScenes.SceneIsLoaded(EagerScenes.BACKSTAGE_LEVEL))
		{
			timer += Time.deltaTime;
			yield return null;
		}
		for (; timer < tweakables.waitBeforeFirstLine; timer += Time.deltaTime)
		{
		}
		if (introIndex < tweakables.dialogue.FirstIntros.Length)
		{
			SafeDebug.Log(butterDialogueSource.ToString());
			SafeDebug.Log(tweakables.ToString());
			butterDialogueSource.PlayClip(tweakables.dialogue.FirstIntros[introIndex]);
			while (butterDialogueSource.isPlaying)
			{
				yield return null;
			}
		}
		else if (introIndex - tweakables.dialogue.FirstIntros.Length < tweakables.dialogue.StaminaIntros.Length)
		{
			butterDialogueSource.PlayClip(tweakables.dialogue.StaminaIntros[introIndex - tweakables.dialogue.FirstIntros.Length]);
			while (butterDialogueSource.isPlaying)
			{
				yield return null;
			}
		}
		else
		{
			butterDialogueSource.PlayClip(tweakables.dialogue.StaminaUpdate[Random.Range(0, tweakables.dialogue.StaminaIntros.Length)]);
			while (butterDialogueSource.isPlaying)
			{
				yield return null;
			}
		}
		while (butter != null)
		{
			yield return null;
		}
		GoToBackstage();
	}

	private IEnumerator VacuumCoroutine()
	{
		butterDialogueSource.PlayClip(tweakables.dialogue.VacuumReaction[Random.Range(0, tweakables.dialogue.VacuumReaction.Length)]);
		butterEatTrigger.unconsumable = false;
		while (butterEatTrigger != null)
		{
			yield return null;
		}
		butterDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.Yells[Random.Range(0, tweakables.dialogue.Yells.Length)]);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.4f, this);
		butter.GetComponent<VVRConsumable>().unconsumable = false;
		butter.GetComponent<ObjectButter>().mouth.unconsumable = false;
		butter.GetComponent<VVRConsumable>().OneShotTransfer(ToolManager.BestBlackHoleConsumer(butter.transform.position), -5f);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.4f, this);
		kitchenTable.unconsumable = false;
		for (int i = 0; i < invincibles.Length; i++)
		{
			if (invincibles[i] != null)
			{
				invincibles[i].unconsumable = false;
			}
		}
		PersistentDataManager.IncrementKitchenLoopCounter();
		GoToBackstage();
	}
}
