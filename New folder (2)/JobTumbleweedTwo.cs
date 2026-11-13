using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using TButt;
using UnityEngine;

public class JobTumbleweedTwo : JobTumbleweed
{
	[Header("Scene Two:")]
	[SerializeField]
	private TumbleweedTwoScriptableObject tumbleTwoContent;

	[SerializeField]
	private ObjectTrigger startTrigger;

	public GameObject tumbleweed;

	public GameObject pete;

	public GameObject lilPete;

	[Header("Derail:")]
	public List<Collider> derailColliders;

	public List<Rigidbody> derailRB;

	public List<MeshRenderer> derailMesh;

	[Header("Transition:")]
	public GameObject fallingGO;

	[Header("Transition:")]
	public GameObject dustGO;

	[Header("Transition:")]
	public GameObject sparksGO;

	public TransformShaker shaker;

	private float shakerTargetPosition;

	private float shakerTargetRotation;

	private float shakerTargetPositionFrequency;

	private float shakerTargetRotationFrequency;

	public AudioEvent wiresnap;

	public List<MeshRenderer> meshesTurnOnBackstage;

	private float vacAtStart;

	private VVRDialogueSource tumbleTalk;

	private VVRDialogueSource peteTalk;

	private VVRDialogueSource lilPeteTalk;

	private SafeCoroutine jobThread;

	private SafeCoroutine vacuumThread;

	private float scoreFloat;

	private bool respawn = true;

	private bool respanYells;

	private int tumbleFalls;

	private int lilFalls;

	private int whistleIndex;

	private bool derailed;

	protected override void OnFirstTimeSceneActivated()
	{
		if (JobTumbleweed.instance != null)
		{
			Object.Destroy(this);
		}
		JobTumbleweed.instance = this;
		base.OnFirstTimeSceneActivated();
		tumbleTalk = tumbleweed.GetComponent<VVRDialogueSource>();
		peteTalk = pete.GetComponent<VVRDialogueSource>();
		lilPeteTalk = lilPete.GetComponent<VVRDialogueSource>();
		if (jobThread == null || !jobThread.IsRunning)
		{
			jobThread = GlobalCoroutineContainer.AddThread(LeafblowerCoroutine(), this);
		}
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_BACKSTAGE_ELEVATOR);
		shaker.enabled = false;
		shaker.position.amount = 0.02f;
		shaker.position.frequency = 1f;
		shaker.rotation.amount = 0.1f;
		shaker.rotation.frequency = 1f;
		vacAtStart = EagerMaster.instance.vacuumFill;
	}

	private void Update()
	{
		scoreFloat += currMaxSpeed * Time.deltaTime;
		score = Mathf.FloorToInt(scoreFloat);
		scoreText.text = score.ToString().PadLeft(3, '0') + " m";
		if (jobThread != null && vacuumThread == null)
		{
			for (int i = 0; i < ToolManager.ActiveToolGroups.Count; i++)
			{
				if (ToolManager.ActiveToolGroups[i].Manipulator.CurrentManipulatorMode == Manipulator.manipulatorMode.vacuum && TBInput.GetButtonDown(TBInput.Button.Action1, ToolManager.ActiveToolGroups[i].Controller))
				{
					vacuumThread = GlobalCoroutineContainer.AddThread(VacuumCoroutine(), this);
				}
			}
		}
		if (respawn)
		{
			if (tumbleweed != null && tumbleweed.transform.position.y < 0f)
			{
				Reset(tumbleweed);
			}
			if (pete != null && pete.transform.position.y < 0f)
			{
				Reset(pete, -1.5f);
			}
			if (lilPete != null && lilPete.transform.position.y < 0f)
			{
				Reset(lilPete, 1.5f);
			}
		}
		if (shaker.enabled)
		{
			shaker.position.amount = Mathf.Lerp(shaker.position.amount, shakerTargetPosition, 3f * Time.deltaTime);
			shaker.position.frequency = Mathf.Lerp(shaker.position.frequency, shakerTargetPositionFrequency, 3f * Time.deltaTime);
			shaker.rotation.amount = Mathf.Lerp(shaker.rotation.amount, shakerTargetRotation, 3f * Time.deltaTime);
			shaker.rotation.frequency = Mathf.Lerp(shaker.rotation.frequency, shakerTargetRotationFrequency, 3f * Time.deltaTime);
		}
	}

	private IEnumerator LeafblowerCoroutine()
	{
		whistling = false;
		leafblower.SetActive(value: false);
		pedestal.SetActive(value: false);
		dustGO.SetActive(value: false);
		sparksGO.SetActive(value: false);
		fallingGO.SetActive(value: false);
		while (!startTrigger.playerInside)
		{
			yield return null;
		}
		tumbleweed.GetComponent<Rigidbody>().isKinematic = false;
		pete.GetComponent<Rigidbody>().isKinematic = false;
		lilPete.GetComponent<Rigidbody>().isKinematic = false;
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2.5f, this);
		tumbleTalk.PlayClip(tumbleTwoContent.dialogue.intro);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
		pedestal.SetActive(value: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.2f, this);
		leafblower.SetActive(value: true);
		while (tumbleTalk.isPlaying)
		{
			if (ToolManager.PlayerGrabbingGameObject(leafblower))
			{
				tumbleTalk.ForcePlayClipOverwrite(tumbleTwoContent.dialogue.tooFast);
				break;
			}
			yield return null;
		}
		tumbleTalk.PlayClip(tumbleTwoContent.dialogue.introductions);
		while (tumbleTalk.isPlaying)
		{
			yield return null;
		}
		lilPeteTalk.PlayClip(tumbleTwoContent.dialogue.howdy);
		while (lilPeteTalk.isPlaying)
		{
			yield return null;
		}
		respanYells = true;
		currMaxSpeed = mediumSpeed;
		conveyor.ChangeSpeed(currMaxSpeed, 5f);
		tumbleTalk.PlayClip(tumbleTwoContent.dialogue.letsGetTumblin);
		while (tumbleTalk.isPlaying)
		{
			yield return null;
		}
		EagerScenes.instance.LoadQueue(EagerScenes.BACKSTAGE_LEVEL);
		whistling = true;
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(tumbleTwoContent.tumbleSectionOne, this);
		lilPeteTalk.PlayClip(tumbleTwoContent.dialogue.cantThisThing);
		currMaxSpeed = hardSpeed;
		conveyor.ChangeSpeed(currMaxSpeed, 5f);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
		SetShakerTargets(turnOn: true, 0.05f, 3f, 0.6f, 3f);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
		dustGO.SetActive(value: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(tumbleTwoContent.tumbleSectionTwo, this);
		lilPeteTalk.PlayClip(tumbleTwoContent.dialogue.faster);
		conveyor.ChangeSpeed(8f, 5f);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
		SetShakerTargets(turnOn: true, 0.1f, 10f, 1.5f, 10f);
		sparksGO.SetActive(value: true);
		wiresnap.Play();
		while (lilPeteTalk.isPlaying)
		{
			yield return null;
		}
		wiresnap.Play();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(3f, this);
		wiresnap.Play();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.8f, this);
		wiresnap.Play();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.4f, this);
		wiresnap.Play();
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.2f, this);
		wiresnap.Play();
		whistling = false;
		derailed = true;
		ConveyorTransition();
		SetShakerTargets(turnOn: true, 0f, 3f, 0f, 3f);
		for (int i = 0; i < derailMesh.Count; i++)
		{
			derailMesh[i].enabled = false;
		}
		for (int j = 0; j < derailColliders.Count; j++)
		{
			derailColliders[j].enabled = false;
		}
		conveyor.GetComponent<AudioLoop>().Stop();
		SetShakerTargets(turnOn: false, 0f, 3f, 0f, 3f);
		ToolManager.PlayerGrabbingGameObject(leafblower, dropIfGrabbing: true, playSFX: false);
		leafblower.SetActive(value: false);
		fallingGO.SetActive(value: true);
		dustGO.SetActive(value: false);
		wiresnap.Play();
		for (int k = 0; k < derailRB.Count; k++)
		{
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.3f, this);
			derailRB[k].isKinematic = false;
			derailRB[k].useGravity = true;
		}
		sparksGO.SetActive(value: false);
		JobReviewUtility.PushJobReview(VVR_HUB.JobStates.Tumbleweed2, goodReview.mReview);
		respawn = false;
		if (vacuumThread != null && vacuumThread.IsRunning)
		{
			vacuumThread.Stop();
		}
		GoToBackstage();
	}

	private void SetShakerTargets(bool turnOn, float pos, float posFreq, float rot, float rotFreq)
	{
		shaker.enabled = turnOn;
		shakerTargetPosition = pos;
		shakerTargetPositionFrequency = posFreq;
		shakerTargetRotation = rot;
		shakerTargetRotationFrequency = rotFreq;
	}

	public void Whistle()
	{
		tumbleTalk.ForcePlayClipOverwrite(tumbleTwoContent.dialogue.whistles[whistleIndex]);
		whistleIndex++;
		if (whistleIndex >= tumbleTwoContent.dialogue.whistles.Count)
		{
			whistleIndex = 0;
		}
	}

	public void Reset(GameObject tumbler, float xOffset = 0f)
	{
		if (!derailed && (resetThread == null || !resetThread.IsRunning))
		{
			resetThread = GlobalCoroutineContainer.AddThread(resetCoroutine(tumbler, xOffset), this);
		}
	}

	private IEnumerator resetCoroutine(GameObject tumbler, float xOffset)
	{
		if (respanYells)
		{
			if (tumbler == tumbleweed)
			{
				tumbleTalk.ForcePlayClipOverwrite(tumbleTwoContent.dialogue.mrWeedRespawn[tumbleFalls % tumbleTwoContent.dialogue.mrWeedRespawn.Count]);
				tumbleFalls++;
			}
			else if (tumbler == lilPete)
			{
				lilPeteTalk.PlayClip(tumbleTwoContent.dialogue.lilPeetRespawn[lilFalls % tumbleTwoContent.dialogue.lilPeetRespawn.Count]);
				lilFalls++;
			}
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
		falls++;
		fallText.text = falls.ToString();
		tumbler.GetComponent<Rigidbody>().velocity = Vector3.zero;
		tumbler.transform.position = respawnPoint + Vector3.right * xOffset;
		yield return null;
		tumbler.GetComponent<Rigidbody>().AddForce((Vector3.up * 800f + Vector3.back * 350f) / 60f, ForceMode.Impulse);
	}

	private IEnumerator VacuumCoroutine()
	{
		JobReviewUtility.PushJobReview(VVR_HUB.JobStates.Tumbleweed2, badReview.mReview);
		tumbleTalk.ForcePlayClipOverwrite(tumbleTwoContent.dialogue.vacuumReaction);
		yield return null;
		while (tumbleweed != null && tumbleTalk.isPlaying)
		{
			yield return null;
		}
		while (lilPete != null && tumbleweed != null && pete != null && EagerMaster.instance.vacuumFill - vacAtStart < 80f)
		{
			yield return null;
		}
		respawn = false;
		ToolManager.PlayerGrabbingGameObject(leafblower, dropIfGrabbing: true, playSFX: false);
		leafblower.SetActive(value: false);
		fallingGO.SetActive(value: true);
		dustGO.SetActive(value: false);
		sparksGO.SetActive(value: false);
		ConveyorTransition();
		if (jobThread != null && jobThread.IsRunning)
		{
			jobThread.Stop();
		}
		if (lilPete != null)
		{
			lilPete.SetActive(value: false);
		}
		if (pete != null)
		{
			pete.SetActive(value: false);
		}
		if (tumbleweed != null)
		{
			tumbleweed.SetActive(value: false);
		}
		conveyor.GetComponent<AudioLoop>().Stop();
		SetShakerTargets(turnOn: false, 0f, 3f, 0f, 3f);
		GoToBackstage();
	}

	public void LeafblowerGrab()
	{
		leafblower.GetComponent<Rigidbody>().isKinematic = false;
		pedestal.GetComponent<SpawnScale>().Disable();
	}

	private void ConveyorTransition()
	{
		for (int i = 0; i < conveyor.linkList.Count; i++)
		{
			ConveyorLink conveyorLink = conveyor.linkList[i];
			if ((bool)conveyorLink)
			{
				Rigidbody component = conveyorLink.gameObject.GetComponent<Rigidbody>();
				component.isKinematic = false;
				component.useGravity = true;
				component.AddForce(Vector3.up * Random.Range(5, 15), ForceMode.Impulse);
				component.AddTorque(new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10)), ForceMode.Impulse);
				conveyorLink.broken = true;
			}
		}
		for (int j = 0; j < spawner.cactusList.Count; j++)
		{
			GameObject gameObject = spawner.cactusList[j];
			if ((bool)gameObject)
			{
				gameObject.transform.SetParent(JobTumbleweed.instance.transform);
			}
		}
		for (int k = 0; k < spawner.rockList.Count; k++)
		{
			GameObject gameObject2 = spawner.rockList[k];
			if ((bool)gameObject2)
			{
				gameObject2.transform.SetParent(JobTumbleweed.instance.transform);
			}
		}
		for (int l = 0; l < spawner.fenceList.Count; l++)
		{
			GameObject gameObject3 = spawner.fenceList[l];
			if ((bool)gameObject3)
			{
				gameObject3.transform.SetParent(JobTumbleweed.instance.transform);
			}
		}
	}
}
