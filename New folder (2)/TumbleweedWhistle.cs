using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using UnityEngine;

public class TumbleweedWhistle : MonoBehaviour
{
	[SerializeField]
	private TumbleweedOneScriptableObject tumbleOneContent;

	public Vector3 border = Vector3.zero;

	public Vector3 respawnPoint = Vector3.zero;

	public bool whistling;

	public List<AudioClip> whistles = new List<AudioClip>();

	public List<AudioClip> warnings = new List<AudioClip>();

	public List<AudioClip> complaints = new List<AudioClip>();

	public AudioClip screamClip;

	private VVRDialogueSource audioSource;

	private Vector3 positionLastFrame;

	private SafeCoroutine resetThread;

	private int whIndex;

	private int waIndex;

	private void Start()
	{
		audioSource = GetComponent<VVRDialogueSource>();
		positionLastFrame = transform.position;
	}

	private void Update()
	{
		if (transform.position.y < border.y)
		{
			Reset();
		}
		else if (transform.position.z > border.z)
		{
			if (positionLastFrame.z - transform.position.z < -0.0001f * Time.deltaTime)
			{
				JobTumbleweed.instance.score += (int)Mathf.Clamp(0f, 2f, 1000f * (transform.position.z - positionLastFrame.z));
				if (!audioSource.isPlaying && whistling)
				{
					Whistle();
				}
			}
		}
		else if (!audioSource.isPlaying && whistling)
		{
			Warn();
		}
		positionLastFrame = transform.position;
	}

	public void Whistle()
	{
		audioSource.ForcePlayClipOverwrite(tumbleOneContent.dialogue.whistles[whIndex]);
		whIndex++;
		if (whIndex >= tumbleOneContent.dialogue.whistles.Count)
		{
			whIndex = 0;
		}
	}

	public void Warn()
	{
		audioSource.ForcePlayClipOverwrite(tumbleOneContent.dialogue.warningsFront[waIndex]);
		waIndex++;
		if (waIndex >= tumbleOneContent.dialogue.warningsFront.Count)
		{
			waIndex = 0;
		}
	}

	public void Scream()
	{
		audioSource.ForcePlayClipOverwrite(tumbleOneContent.dialogue.screamClips[0]);
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
		float originalSpeed = JobTumbleweed.instance.currMaxSpeed;
		JobTumbleweed.instance.conveyor.ChangeSpeed(0f, 2f);
		yield return new WaitForSeconds(1f);
		Scream();
		yield return new WaitForSeconds(1f);
		JobTumbleweed.instance.conveyor.ChangeSpeed(originalSpeed, 2f);
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		transform.position = respawnPoint;
	}
}
