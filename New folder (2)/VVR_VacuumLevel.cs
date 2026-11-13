using System.Collections;
using TButt;
using UnityEngine;

public class VVR_VacuumLevel : SceneBehaviorJob
{
	[Header("Scene References")]
	public GameObject outsideParent;

	public VVRDialogueSource vocalPoint;

	public GameObject cleanupTool;

	public GameObject hacktivitudeParent;

	public GameObject leftLight;

	public GameObject rightLight;

	public GameObject swipeText;

	public GameObject activitudeLogo;

	public GameObject foamShaker;

	public VVRConsumable[] consumableCheck;

	public GameObject humanUnionLogo;

	public GvrAudioSource dingSound;

	public GvrAudioSource lightSound;

	public GvrAudioSource windSound;

	public GameObject[] bossParents;

	public Rigidbody[] stars;

	public GameObject[] emptyStars;

	public VVRConsumable[] mugs;

	public VVRConsumable[] flamingos;

	public ObjectTrigger outsideTrigger;

	public Transform insideVocalPos;

	public VacuumTutorialSwapper firstController;

	[Header("Sequence Object")]
	public VaccumSequenceScriptableObject tweakables;

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
		StartVacuum();
		EagerHeadsetManager.SetCanRemoveHeadset(canRemove: false);
		PersistentDataManager.OverwriteHasVaccum(s: false);
		EagerMaster.instance.vacuumFill = 0f;
		PersistentDataManager.OverwriteVacuumValue(0f);
	}

	private void StartVacuum()
	{
		GlobalCoroutineContainer.AddThread(VacuumCoroutine(), this);
	}

	private IEnumerator VacuumCoroutine()
	{
		leftLight.SetActive(value: false);
		rightLight.SetActive(value: false);
		bool windIsPlaying = false;
		lightSound.Play();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(tweakables.startDelay, this);
		vocalPoint.transform.position = insideVocalPos.position;
		vocalPoint.PlayClip(tweakables.dialogueOptions.weAreHereForYou);
		while (vocalPoint.isPlaying)
		{
			yield return null;
		}
		vocalPoint.PlayClip(tweakables.dialogueOptions.hadEnoughOfBadReviews);
		Vector3 logoScale = humanUnionLogo.transform.localScale;
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1.45f, this);
		float t = 0f;
		while (t < 0.3f)
		{
			t += Time.deltaTime;
			float lerp = t / 0.3f;
			lerp *= lerp;
			humanUnionLogo.transform.localScale = logoScale * (1f - lerp);
			yield return null;
		}
		humanUnionLogo.SetActive(value: false);
		while (vocalPoint.isPlaying)
		{
			if (vocalPoint.GetClipQueue() != null)
			{
				float length = tweakables.dialogueOptions.hadEnoughOfBadReviews.clip.length;
				float num = length;
				if (vocalPoint.GetClipQueue().Count > 0)
				{
					num = vocalPoint.GetClipQueue()[0].progressTime;
				}
				if (num > 2f)
				{
					setBossActive(0);
				}
			}
			yield return null;
		}
		vocalPoint.PlayClip(tweakables.dialogueOptions.struggledToSatisfy);
		while (vocalPoint.isPlaying)
		{
			if (vocalPoint.GetClipQueue() != null)
			{
				float length2 = tweakables.dialogueOptions.struggledToSatisfy.clip.length;
				float num2 = length2;
				if (vocalPoint.GetClipQueue().Count > 0)
				{
					num2 = vocalPoint.GetClipQueue()[0].progressTime;
				}
				float num3 = num2 / length2;
				if (num3 < 0.25f)
				{
					setBossActive(1);
				}
				if (num3 > 0.25f && num3 < 0.5f)
				{
					setBossActive(2);
				}
				if (num3 > 0.5f && num3 < 0.75f)
				{
					setBossActive(3);
				}
				if (num3 > 0.75f)
				{
					setBossActive(4);
				}
			}
			yield return null;
		}
		setBossActive(-1);
		vocalPoint.PlayClip(tweakables.dialogueOptions.averageRating);
		for (int i = 0; i < stars.Length; i++)
		{
			stars[i].gameObject.SetActive(value: true);
			emptyStars[i].SetActive(value: true);
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2.5f, this);
		GlobalCoroutineContainer.AddThread(DropStarsOnDelay(0.5f), this);
		while (vocalPoint.isPlaying)
		{
			yield return null;
		}
		vocalPoint.PlayClip(tweakables.dialogueOptions.payDependsOnRating);
		while (vocalPoint.isPlaying)
		{
			yield return null;
		}
		vocalPoint.PlayClip(tweakables.dialogueOptions.togetherWeCanReset);
		while (vocalPoint.isPlaying)
		{
			yield return null;
		}
		for (int j = 0; j < stars.Length; j++)
		{
			stars[stars.Length - 1].gameObject.SetActive(value: false);
			emptyStars[j].SetActive(value: false);
		}
		dingSound.Play();
		swipeText.SetActive(value: false);
		activitudeLogo.SetActive(value: true);
		cleanupTool.SetActive(value: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(dingSound.clip.length, this);
		vocalPoint.PlayClip(tweakables.dialogueOptions.weHacked);
		while (vocalPoint.isPlaying)
		{
			yield return null;
		}
		outsideParent.SetActive(value: true);
		ToolManager.AllManipulators.HasVacuum = true;
		if (PersistentDataManager.GetHasAdmin())
		{
			PersistentDataManager.OverwriteHasVaccum(s: true);
			if (VVR_HUB.instance != null)
			{
				VVR_HUB.instance.hasCompletedJob = true;
				VVR_HUB.instance.wasInUnion = true;
			}
		}
		DialoguePlaybackInstance purgeClip = vocalPoint.PlayClip(tweakables.dialogueOptions.simplySwipe);
		if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
		{
			firstController.controllerRefs.GetWorldControllerModel().GetComponentInChildren<VacuumController>().topButton.sharedMaterial = firstController.controllerRefs.GetWorldControllerModel().GetComponentInChildren<VacuumController>().highlightMat;
		}
		bool hasPlayedDing = false;
		while (vocalPoint.isPlaying)
		{
			if (ToolManager.IsVacuumOut && !hasPlayedDing)
			{
				dingSound.Play();
				hasPlayedDing = true;
				foamShaker.transform.position = Vector3.one * 1000f;
				vocalPoint.PurgePlaybackClipFromQueue(purgeClip);
			}
			yield return null;
		}
		float count = 0f;
		bool reminderPlayed = false;
		while (!ToolManager.IsVacuumOut)
		{
			count += Time.deltaTime;
			if (count > 10f)
			{
				if (reminderPlayed)
				{
					swipeText.SetActive(value: true);
				}
				count = 0f;
				vocalPoint.PlayClip(tweakables.dialogueOptions.swipeLeft);
				reminderPlayed = true;
			}
			yield return null;
		}
		if (!hasPlayedDing)
		{
			dingSound.Play();
		}
		swipeText.SetActive(value: false);
		if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
		{
			firstController.controllerRefs.GetWorldControllerModel().GetComponentInChildren<VacuumController>().topButton.sharedMaterial = firstController.controllerRefs.GetWorldControllerModel().GetComponentInChildren<VacuumController>().startMat;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(dingSound.clip.length + 0.5f, this);
		hasPlayedDing = false;
		vocalPoint.PlayClip(tweakables.dialogueOptions.nowYou);
		EagerMaster.instance.vacuumFill = 0f;
		while (vocalPoint.isPlaying)
		{
			if (EagerMaster.instance.vacuumFill > 0f)
			{
				if (!hasPlayedDing)
				{
					dingSound.Play();
					hasPlayedDing = true;
				}
				hacktivitudeParent.SetActive(value: false);
				if (!windIsPlaying)
				{
					windSound.Play();
					windIsPlaying = true;
				}
			}
			yield return null;
		}
		float time = 0f;
		while (EagerMaster.instance.vacuumFill <= 0f)
		{
			time += Time.deltaTime;
			if (time > 8f)
			{
				time = 0f;
				vocalPoint.PlayClip(tweakables.dialogueOptions.nowYou);
			}
			yield return null;
		}
		vocalPoint.PlayClip(tweakables.dialogueOptions.itsTime);
		if (!hasPlayedDing)
		{
			dingSound.Play();
			hasPlayedDing = true;
		}
		hacktivitudeParent.SetActive(value: false);
		if (!windIsPlaying)
		{
			windSound.Play();
			windIsPlaying = true;
		}
		vocalPoint.mSubtitleRenderer.SetForceToHead(s: true);
		bool continueSequence = false;
		while (!outsideTrigger.playerInside && !continueSequence)
		{
			continueSequence = true;
			for (int k = 0; k < consumableCheck.Length; k++)
			{
				if (consumableCheck[k] != null)
				{
					continueSequence = false;
				}
			}
			for (int l = 0; l < mugs.Length; l++)
			{
				if (mugs[l].currentObjectState != VVRConsumable.objectState.wholeStatic)
				{
					continueSequence = true;
				}
			}
			yield return null;
		}
		bool vaccumedMugs = false;
		vocalPoint.transform.position = new Vector3(0f, 0f, 13f);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		for (int m = 0; m < mugs.Length; m++)
		{
			vaccumedMugs = true;
			if (mugs[m].currentObjectState != VVRConsumable.objectState.fullyConsumedGlimmer && mugs[m].currentObjectState != VVRConsumable.objectState.fullyConsumedStatic)
			{
				vaccumedMugs = false;
			}
		}
		leftLight.SetActive(value: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		for (int n = 0; n < mugs.Length; n++)
		{
			vaccumedMugs = true;
			if (mugs[n].currentObjectState != VVRConsumable.objectState.fullyConsumedGlimmer && mugs[n].currentObjectState != VVRConsumable.objectState.fullyConsumedStatic)
			{
				vaccumedMugs = false;
			}
		}
		vocalPoint.PlayClip(tweakables.dialogueOptions.letsRedistribute);
		while (vocalPoint.isPlaying)
		{
			if (vaccumedMugs)
			{
				vocalPoint.PurgeAllDialogue();
				break;
			}
			for (int num4 = 0; num4 < mugs.Length; num4++)
			{
				vaccumedMugs = true;
				if (mugs[num4].currentObjectState != VVRConsumable.objectState.fullyConsumedGlimmer && mugs[num4].currentObjectState != VVRConsumable.objectState.fullyConsumedStatic)
				{
					vaccumedMugs = false;
				}
			}
			yield return null;
		}
		count = 0f;
		bool lastVacuumState = ToolManager.IsVacuumOut;
		while (!vaccumedMugs)
		{
			count += Time.deltaTime;
			if (count > 10f)
			{
				if (!ToolManager.IsVacuumOut)
				{
					vocalPoint.PlayClip(tweakables.dialogueOptions.simplySwipe);
				}
				else
				{
					vocalPoint.PlayClip(tweakables.dialogueOptions.inhaleSwag);
				}
				count = 0f;
			}
			if (lastVacuumState != ToolManager.IsVacuumOut && !vocalPoint.isPlaying)
			{
				vocalPoint.PlayClip(tweakables.dialogueOptions.inhaleSwag);
				count = 0f;
			}
			for (int num5 = 0; num5 < mugs.Length; num5++)
			{
				vaccumedMugs = true;
				if (mugs[num5].currentObjectState != VVRConsumable.objectState.fullyConsumedGlimmer && mugs[num5].currentObjectState != VVRConsumable.objectState.fullyConsumedStatic)
				{
					vaccumedMugs = false;
				}
			}
			lastVacuumState = ToolManager.IsVacuumOut;
			yield return null;
		}
		dingSound.Play();
		rightLight.SetActive(value: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		vocalPoint.PlayClip(tweakables.dialogueOptions.bitRot);
		while (vocalPoint.isPlaying)
		{
			yield return null;
		}
		bool filledFlamingos = false;
		count = 0f;
		while (!filledFlamingos)
		{
			count += Time.deltaTime;
			if (count > 10f)
			{
				vocalPoint.PlayClip(tweakables.dialogueOptions.clickDown);
				count = 0f;
			}
			filledFlamingos = false;
			for (int num6 = 0; num6 < flamingos.Length; num6++)
			{
				if (flamingos[num6].currentObjectState == VVRConsumable.objectState.wholeShaking || flamingos[num6].currentObjectState == VVRConsumable.objectState.wholeStatic)
				{
					filledFlamingos = true;
				}
			}
			yield return null;
		}
		dingSound.Play();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2.5f, this);
		vocalPoint.PlayClip(tweakables.dialogueOptions.thePower);
		while (vocalPoint.isPlaying)
		{
			yield return null;
		}
		if (ToolManager.IsVacuumOut)
		{
			purgeClip = vocalPoint.PlayClip(tweakables.dialogueOptions.putAwayVacuum[0]);
		}
		bool awayBool = false;
		while (vocalPoint.isPlaying)
		{
			if (!ToolManager.IsVacuumOut)
			{
				vocalPoint.PurgePlaybackClipFromQueue(purgeClip);
				awayBool = true;
				dingSound.Play();
			}
			yield return null;
		}
		float countTime = 0f;
		while (ToolManager.IsVacuumOut)
		{
			countTime += Time.deltaTime;
			if (countTime > 5f)
			{
				vocalPoint.PlayClip(tweakables.dialogueOptions.putAwayVacuum[1]);
				countTime = 0f - tweakables.dialogueOptions.putAwayVacuum[1].clip.length;
			}
			yield return null;
		}
		if (!awayBool)
		{
			dingSound.Play();
		}
		vocalPoint.PlayClip(tweakables.dialogueOptions.atOurSignal);
		while (vocalPoint.isPlaying)
		{
			yield return null;
		}
		vocalPoint.PlayClip(tweakables.dialogueOptions.yourTask);
		while (vocalPoint.isPlaying)
		{
			yield return null;
		}
		EagerHeadsetManager.SetCanRemoveHeadset(canRemove: true);
		if (VVR_HUB.instance != null)
		{
			VVR_HUB.instance.wasInUnion = true;
		}
		PersistentDataManager.OverwriteHasVaccum(s: true);
		CompleteJob();
	}

	private IEnumerator DropStarsOnDelay(float d)
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(d, this);
		stars[0].isKinematic = false;
		stars[0].AddForce(Vector3.forward * 2f, ForceMode.Impulse);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(d, this);
		stars[1].isKinematic = false;
		stars[1].AddForce(Vector3.forward * -2.5f, ForceMode.Impulse);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(d, this);
		stars[2].isKinematic = false;
		stars[2].AddForce(Vector3.forward * 2f, ForceMode.Impulse);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(d, this);
		stars[3].isKinematic = false;
		stars[3].AddForce(Vector3.forward * -5f, ForceMode.Impulse);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(d, this);
		stars[4].isKinematic = false;
	}

	private void setBossActive(int n)
	{
		if (n == -1)
		{
			for (int i = 0; i < bossParents.Length; i++)
			{
				bossParents[i].SetActive(value: false);
			}
			return;
		}
		for (int j = 0; j < bossParents.Length; j++)
		{
			if (j == n)
			{
				bossParents[j].SetActive(value: true);
			}
			else
			{
				bossParents[j].SetActive(value: false);
			}
		}
	}
}
