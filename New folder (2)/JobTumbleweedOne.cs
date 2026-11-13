using System.Collections;
using UnityEngine;

public class JobTumbleweedOne : JobTumbleweed
{
	[Header("Scene One:")]
	[SerializeField]
	private TumbleweedOneScriptableObject tumbleOneContent;

	public GameObject tumbleweed;

	public GravelRespawn gravel;

	[Header("Audio:")]
	public AudioLoop wind;

	public AudioEvent eagle;

	public AudioLoop spacialMusic;

	public AudioLoop flatMusic;

	public GameObject radioSubtitle;

	private VVRDialogueSource tumbleTalk;

	private float eagleTimer;

	private float eagleThreshhold = 20f;

	private float scoreFloat;

	private bool intro = true;

	private int whistleIndex;

	private int warningFrontIndex;

	private int warningSideIndex;

	private int screamIndex;

	private int[] screamIndicies;

	private int grumbleIndex;

	private int[] grumbleIndicies;

	private void Update()
	{
		eagleTimer += Time.deltaTime;
		if (eagleTimer > eagleThreshhold)
		{
			eagle.Play();
			eagleThreshhold = Random.Range(5f, 50f);
			eagleTimer = 0f;
		}
		if (!intro)
		{
			if (tumbleweed.transform.position.y < 0f || Mathf.Abs(tumbleweed.transform.position.x) > 3.5f)
			{
				Reset();
			}
			else
			{
				scoreFloat += currMaxSpeed * Time.deltaTime;
				score = Mathf.FloorToInt(scoreFloat);
				scoreText.text = score.ToString().PadLeft(3, '0') + " m";
				if (whistling && !tumbleTalk.isPlaying && !(tumbleweed.transform.position.z < 0f) && !(Mathf.Abs(tumbleweed.transform.position.x) > 2.5f))
				{
				}
			}
		}
		tumblePosLastFrame = tumbleweed.transform.position;
	}

	protected override void OnFirstTimeSceneActivated()
	{
		if (JobTumbleweed.instance != null)
		{
			Object.Destroy(this);
		}
		JobTumbleweed.instance = this;
		SafeDebug.Log("Instance: " + JobTumbleweed.instance);
		wind.Play();
		base.OnFirstTimeSceneActivated();
		tumbleTalk = tumbleweed.GetComponent<VVRDialogueSource>();
		GlobalCoroutineContainer.AddThread(LeafblowerCoroutine(), this);
	}

	private IEnumerator LeafblowerCoroutine()
	{
		leafblower.SetActive(value: false);
		pedestal.SetActive(value: false);
		whistling = false;
		conveyor.speed = 0f;
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(tumbleOneContent.firstWait, this);
		tumbleTalk.PlayClip(tumbleOneContent.dialogue.Yeehaw);
		while (tumbleTalk.isPlaying)
		{
			yield return null;
		}
		intro = false;
		tumbleTalk.PlayClip(tumbleOneContent.dialogue.Intro);
		while (tumbleTalk.isPlaying)
		{
			yield return null;
		}
		tumbleTalk.PlayClip(tumbleOneContent.dialogue.StartOffGentle);
		while (tumbleTalk.isPlaying)
		{
			yield return null;
		}
		leafblower.SetActive(value: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		pedestal.SetActive(value: true);
		tumbleTalk.PlayClip(tumbleOneContent.dialogue.WrapYerMitts);
		while (!ToolManager.PlayerGrabbingGameObject(leafblower))
		{
			yield return null;
		}
		leafblower.GetComponent<Rigidbody>().isKinematic = false;
		pedestal.GetComponent<SpawnScale>().Disable();
		spacialMusic.Play();
		flatMusic.Play();
		radioSubtitle.SetActive(value: true);
		if (tumbleTalk.isPlaying)
		{
			tumbleTalk.ForcePlayClipOverwrite(tumbleOneContent.dialogue.TooFast);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(4f, this);
		}
		currMaxSpeed = easySpeed;
		while (conveyor.changingSpeed)
		{
			yield return null;
		}
		conveyor.ChangeSpeed(currMaxSpeed, 5f);
		gravel.spawning = true;
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
		tumbleTalk.PlayClip(tumbleOneContent.dialogue.NowWereCookin);
		while (tumbleTalk.isPlaying)
		{
			yield return null;
		}
		whistling = true;
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(tumbleOneContent.tumbleSectionOne, this);
		currMaxSpeed = mediumSpeed;
		while (conveyor.changingSpeed)
		{
			yield return null;
		}
		conveyor.ChangeSpeed(currMaxSpeed, 5f);
		whistling = false;
		tumbleTalk.PlayClip(tumbleOneContent.dialogue.GrizzlyTerritory);
		while (tumbleTalk.isPlaying)
		{
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(tumbleOneContent.grizzlyWait, this);
		spawner.SpawnGrizzly();
		tumbleTalk.ForcePlayClipOverwrite(tumbleOneContent.dialogue.GrizzlyAttack);
		while (tumbleTalk.isPlaying)
		{
			yield return null;
		}
		whistling = true;
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(tumbleOneContent.tumbleSectionTwo, this);
		whistling = false;
		tumbleTalk.PlayClip(tumbleOneContent.dialogue.ThatsTheTicket);
		while (tumbleTalk.isPlaying)
		{
			yield return null;
		}
		whistling = true;
		spawnRate = 0.25f;
		currMaxSpeed = hardSpeed;
		while (conveyor.changingSpeed)
		{
			yield return null;
		}
		conveyor.ChangeSpeed(currMaxSpeed, 5f);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(tumbleOneContent.tumbleSectionThree, this);
		currMaxSpeed = insaneSpeed;
		while (conveyor.changingSpeed)
		{
			yield return null;
		}
		conveyor.ChangeSpeed(currMaxSpeed, 5f);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(10f, this);
		while (conveyor.changingSpeed)
		{
			yield return null;
		}
		whistling = false;
		SafeDebug.Log("time to end");
		gravel.spawning = false;
		currMaxSpeed = 0f;
		conveyor.ChangeSpeed(0f, 5f);
		while (conveyor.changingSpeed)
		{
			yield return null;
		}
		ObjectBase leafblowerOB = leafblower.GetComponent<ObjectBase>();
		leafblowerOB.ForceDropByPlayer();
		leafblower.SetActive(value: false);
		if (falls > 6)
		{
			tumbleTalk.PlayClip(tumbleOneContent.dialogue.BadReview);
			JobReviewUtility.PushJobReview(VVR_HUB.JobStates.Tumbleweed1, badReview.mReview);
			while (tumbleTalk.isPlaying)
			{
				yield return null;
			}
		}
		else
		{
			tumbleTalk.PlayClip(tumbleOneContent.dialogue.GoodReview);
			JobReviewUtility.PushJobReview(VVR_HUB.JobStates.Tumbleweed1, goodReview.mReview);
			while (tumbleTalk.isPlaying)
			{
				yield return null;
			}
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		CompleteJob();
		yield return null;
	}

	public void Whistle()
	{
		tumbleTalk.ForcePlayClipOverwrite(tumbleOneContent.dialogue.whistles[whistleIndex]);
		whistleIndex++;
		if (whistleIndex >= tumbleOneContent.dialogue.whistles.Count)
		{
			whistleIndex = 0;
		}
	}

	public void WarnFront()
	{
		tumbleTalk.ForcePlayClipOverwrite(tumbleOneContent.dialogue.warningsFront[warningFrontIndex]);
		warningFrontIndex++;
		if (warningFrontIndex >= tumbleOneContent.dialogue.warningsFront.Count)
		{
			warningFrontIndex = 0;
		}
	}

	public void WarnSide()
	{
		tumbleTalk.ForcePlayClipOverwrite(tumbleOneContent.dialogue.warningsSide[warningSideIndex]);
		warningSideIndex++;
		if (warningSideIndex >= tumbleOneContent.dialogue.warningsSide.Count)
		{
			warningSideIndex = 0;
		}
	}

	public void Scream()
	{
		if (screamIndicies == null || screamIndicies.Length == 0)
		{
			screamIndicies = new int[tumbleOneContent.dialogue.screamClips.Count];
			for (int i = 0; i < tumbleOneContent.dialogue.screamClips.Count; i++)
			{
				screamIndicies[i] = i;
			}
			reshuffle(screamIndicies);
		}
		if (screamIndex >= tumbleOneContent.dialogue.screamClips.Count)
		{
			screamIndex = 0;
		}
		if (whistling)
		{
			tumbleTalk.ForcePlayClipOverwrite(tumbleOneContent.dialogue.screamClips[screamIndicies[screamIndex]]);
		}
		else
		{
			tumbleTalk.InterruptWithClip(tumbleOneContent.dialogue.screamClips[screamIndicies[screamIndex]]);
		}
		screamIndex++;
	}

	private IEnumerator Grumble()
	{
		if (grumbleIndicies == null || grumbleIndicies.Length == 0)
		{
			grumbleIndicies = new int[tumbleOneContent.dialogue.respawnLines.Count];
			for (int i = 0; i < tumbleOneContent.dialogue.respawnLines.Count; i++)
			{
				grumbleIndicies[i] = i;
			}
			reshuffle(grumbleIndicies);
		}
		if (grumbleIndex >= tumbleOneContent.dialogue.respawnLines.Count)
		{
			grumbleIndex = 0;
		}
		if (whistling)
		{
			tumbleTalk.ForcePlayClipOverwrite(tumbleOneContent.dialogue.respawnLines[grumbleIndicies[grumbleIndex]]);
		}
		else
		{
			tumbleTalk.InterruptWithClip(tumbleOneContent.dialogue.respawnLines[grumbleIndicies[grumbleIndex]]);
		}
		grumbleIndex++;
		while (tumbleTalk.isPlaying)
		{
			yield return null;
		}
	}

	public void Reset()
	{
		if (resetThread == null || !resetThread.IsRunning)
		{
			resetThread = GlobalCoroutineContainer.AddThread(resetCoroutine(), this);
		}
	}

	private IEnumerator resetCoroutine()
	{
		float originalSpeed = currMaxSpeed;
		JobTumbleweed.instance.conveyor.ChangeSpeed(0f, 2f);
		Scream();
		falls++;
		fallText.text = falls.ToString();
		while (conveyor.changingSpeed)
		{
			yield return null;
		}
		JobTumbleweed.instance.conveyor.ChangeSpeed(originalSpeed, 2f);
		tumbleweed.GetComponent<Rigidbody>().velocity = Vector3.zero;
		tumbleweed.transform.position = respawnPoint;
		yield return null;
		tumbleweed.GetComponent<Rigidbody>().AddForce((Vector3.up * 800f + Vector3.back * 300f) / 60f, ForceMode.Impulse);
		GlobalCoroutineContainer.AddThread(Grumble(), this);
	}

	private void reshuffle(int[] numbers)
	{
		for (int i = 0; i < numbers.Length; i++)
		{
			int num = numbers[i];
			int num2 = Random.Range(i, numbers.Length);
			numbers[i] = numbers[num2];
			numbers[num2] = num;
		}
	}
}
