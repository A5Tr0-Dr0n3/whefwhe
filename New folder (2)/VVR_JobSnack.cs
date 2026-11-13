using System.Collections;
using Edelweiss.Coroutine;
using GVR.Gfx;
using UnityEngine;

public class VVR_JobSnack : SceneBehaviorJob
{
	public SnackSequenceScriptableObject tweakables;

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
	private VVRConsumable butterEatTrigger;

	[SerializeField]
	private VVRConsumable butterTable;

	public VVRConsumable[] invincibles;

	private SafeCoroutine snackThread;

	private SafeCoroutine vacuumThread;

	private bool toastPendingJudgment;

	private bool vacuumDetect;

	private ObjectToaster.Toast lastToastEvaluation;

	public static VVR_JobSnack instance;

	private float initialFill;

	[HideInInspector]
	public int[] toastCount = new int[3];

	protected override void OnFirstTimeSceneActivated()
	{
		if (instance != null)
		{
			throw new UnityException("Only one instance of Snack in a scene is allowed.");
		}
		instance = this;
		for (int i = 0; i < kitchenToasters.Length; i++)
		{
			kitchenToasters[i].enabled = false;
		}
		for (int j = 0; j < invincibles.Length; j++)
		{
			if (invincibles[j] != null)
			{
				invincibles[j].unconsumable = true;
			}
		}
		initialFill = EagerMaster.instance.vacuumFill;
		base.OnFirstTimeSceneActivated();
		snackThread = GlobalCoroutineContainer.AddThread(SnackCoroutine(), this);
		butterEatTrigger.unconsumable = true;
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_BACKSTAGE_ELEVATOR);
	}

	private void Update()
	{
		if (snackThread != null && snackThread.IsRunning && ((!vacuumDetect && EagerMaster.instance.vacuumFill > initialFill) || (vacuumDetect && ToolManager.IsVacuumOut)))
		{
			snackThread.Stop();
			vacuumThread = GlobalCoroutineContainer.AddThread(VacuumCoroutine(), this);
		}
	}

	private IEnumerator SnackCoroutine()
	{
		yield return new WaitForSeconds(tweakables.waitBeforeFirstLine);
		butterDialogueSource.PlayClip(tweakables.dialogue.OhItsYou);
		while (butterDialogueSource.isPlaying)
		{
			yield return null;
		}
		for (int i = 0; i < kitchenToasters.Length; i++)
		{
			kitchenToasters[i].enabled = true;
		}
		yield return new WaitForSeconds(tweakables.beat);
		butterDialogueSource.PlayClip(tweakables.dialogue.LetsGetStarted);
		while (butterDialogueSource.isPlaying)
		{
			yield return null;
		}
		yield return new WaitForSeconds(tweakables.beat);
		butterDialogueSource.PlayClip(tweakables.dialogue.PutThemOnMe);
		while (butterDialogueSource.isPlaying)
		{
			yield return null;
		}
		yield return new WaitForSeconds(tweakables.beat);
		butterDialogueSource.PlayClip(tweakables.dialogue.HopeForImprovement);
		while (butterDialogueSource.isPlaying)
		{
			yield return null;
		}
		yield return new WaitForSeconds(tweakables.beat);
		toastPendingJudgment = false;
		for (int j = 0; j < tweakables.dialogue.Critiques.Length; j++)
		{
			butterDialogueSource.PlayClip(tweakables.dialogue.Critiques[j]);
			while (butterDialogueSource.isPlaying)
			{
				if (toastPendingJudgment && tweakables.dialogue.Critiques[j].mInterruptability != Interruptability.uninterruptable)
				{
					butterDialogueSource.InterruptWithClip(getToastFeedbackLine(lastToastEvaluation), enqueueIfCantInterrupt: true);
					toastPendingJudgment = false;
				}
				yield return new WaitForSeconds(tweakables.beat);
			}
		}
		butterDialogueSource.PlayClip(tweakables.dialogue.Stop);
		for (int k = 0; k < kitchenToasters.Length; k++)
		{
			if (!kitchenToasters[k].finalToast && kitchenToasters[k].startedToasting)
			{
				kitchenToasters[k].EjectToast(popLastToast: true);
				yield return new WaitForSeconds(Random.Range(0.02f, 0.1f));
			}
		}
		int freezeToastStart = Mathf.Max(0, KitchenToastManager.GetKitchenToast().Count - 30);
		for (int l = freezeToastStart; l < KitchenToastManager.GetKitchenToast().Count; l++)
		{
			Rigidbody component = KitchenToastManager.GetKitchenToast()[l].GetComponent<Rigidbody>();
			if ((bool)component)
			{
				component.useGravity = false;
				component.velocity *= 0.03f;
				component.angularVelocity *= 0.06f;
			}
		}
		while (butterDialogueSource.isPlaying)
		{
			yield return null;
		}
		vacuumDetect = true;
		for (int m = 0; m < tweakables.dialogue.VacuumMonologue.Length; m++)
		{
			butterDialogueSource.PlayClip(tweakables.dialogue.VacuumMonologue[m]);
			while (butterDialogueSource.isPlaying)
			{
				yield return null;
			}
		}
		float t = 0f;
		while (butter != null)
		{
			t += Time.deltaTime;
			if (t > 20f && vacuumThread == null)
			{
				butterDialogueSource.PlayClip(tweakables.dialogue.VacuumMonologue[4]);
				t = 0f;
			}
			yield return null;
		}
		GoToBackstage();
	}

	private IEnumerator VacuumCoroutine()
	{
		butterDialogueSource.ForcePlayClipOverwrite(tweakables.dialogue.WhatsThis);
		while (butterDialogueSource.isPlaying)
		{
			yield return null;
		}
		butterDialogueSource.PlayClip(tweakables.dialogue.YesItIs);
		while (butterDialogueSource.isPlaying)
		{
			yield return null;
		}
		butterDialogueSource.PlayClip(tweakables.dialogue.NoWordOfThis);
		while (butterDialogueSource.isPlaying)
		{
			yield return null;
		}
		butterDialogueSource.PlayClip(tweakables.dialogue.ImReady);
		while (butterDialogueSource.isPlaying)
		{
			yield return null;
		}
		butterEatTrigger.unconsumable = false;
		while (butterEatTrigger != null)
		{
			yield return null;
		}
		butterDialogueSource.PlayClip(tweakables.dialogue.Yells[3]);
		butterTable.unconsumable = false;
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.4f, this);
		butter.GetComponent<VVRConsumable>().unconsumable = false;
		butter.GetComponent<ObjectButter>().mouth.unconsumable = false;
		butter.GetComponent<VVRConsumable>().OneShotTransfer(ToolManager.BestBlackHoleConsumer(butter.transform.position), -5f);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.4f, this);
		for (int i = 0; i < invincibles.Length; i++)
		{
			if (invincibles[i] != null)
			{
				invincibles[i].unconsumable = false;
			}
		}
		GoToBackstage();
	}

	public void NewToastAttachedToButter(ObjectToast toastIn)
	{
		lastToastEvaluation = toastIn.toastState;
		toastPendingJudgment = true;
		if (lastToastEvaluation == ObjectToaster.Toast.Uncooked)
		{
			toastCount[0]++;
		}
		else if (lastToastEvaluation == ObjectToaster.Toast.Toasted)
		{
			toastCount[1]++;
		}
		else if (lastToastEvaluation == ObjectToaster.Toast.Burning)
		{
			toastCount[2]++;
		}
	}

	private AudioClip getToastFeedbackLine(ObjectToaster.Toast toastIn)
	{
		return toastIn switch
		{
			ObjectToaster.Toast.Uncooked => tweakables.dialogue.Underdone.FireFeedbackClip(), 
			ObjectToaster.Toast.Toasted => tweakables.dialogue.Perfect.FireFeedbackClip(), 
			ObjectToaster.Toast.Burning => tweakables.dialogue.Burnt.FireFeedbackClip(), 
			ObjectToaster.Toast.SOS => tweakables.dialogue.Burnt.FireFeedbackClip(), 
			_ => null, 
		};
	}
}
