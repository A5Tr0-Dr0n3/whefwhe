using System.Collections.Generic;
using UnityEngine;

public class PanopticonController : MonoBehaviour
{
	public float rotationScalar = 1f;

	public ElevatorKey keyGO;

	public List<GvrAudioSource> buzzAudio;

	private float targetVolume = 0.2f;

	public BoidController flockA;

	public BoidController flockB;

	public AggroTimer timerA;

	public AggroTimer timerB;

	private bool firstGrab = true;

	public AudioSource grabMusic;

	public ElevatorEnvironments environmentController;

	public ElizaEnvironmentProperties keyEnv;

	public ElizaEnvironmentProperties noKeyEnv;

	public GameObject broomGlow;

	public ObjectTrigger trigger;

	private float t;

	private bool timerTriggered;

	private bool keyInside;

	private bool _keyGrabbed;

	public bool keyGrabbed
	{
		get
		{
			return _keyGrabbed;
		}
		set
		{
			if (_keyGrabbed == value)
			{
				return;
			}
			if (value)
			{
				if (firstGrab)
				{
					grabMusic.Play();
					firstGrab = false;
				}
				environmentController.MatchEnvironment(1f, keyEnv);
				broomGlow.SetActive(value: true);
			}
			else
			{
				if (!keyInside)
				{
					flockA.SetSwarmBehavior(ObjectBoid.behaviorMode.ambient);
					flockB.SetSwarmBehavior(ObjectBoid.behaviorMode.ambient);
					t = 0f;
					timerTriggered = false;
				}
				environmentController.MatchEnvironment(1f, noKeyEnv);
				broomGlow.SetActive(value: false);
			}
			_keyGrabbed = value;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (keyInside)
		{
			targetVolume = Mathf.Clamp(Mathf.Lerp(targetVolume, 0.7f, 5f * Time.deltaTime), 0.1f, 0.99f);
			flockA.velocity = Mathf.Lerp(flockA.velocity, 8f, 10f * Time.deltaTime);
			flockB.velocity = Mathf.Lerp(flockA.velocity, 9f, 10f * Time.deltaTime);
			rotationScalar = Mathf.Lerp(rotationScalar, 4f, 10f * Time.deltaTime);
			broomGlow.SetActive(value: false);
		}
		else
		{
			keyGrabbed = ToolManager.PlayerGrabbingGameObject(keyGO.gameObject);
			if (keyGrabbed)
			{
				targetVolume = Mathf.Clamp(Mathf.Lerp(targetVolume, 1f, 5f * Time.deltaTime), 0.21f, 0.99f);
				flockA.velocity = Mathf.Lerp(flockA.velocity, 6f, 10f * Time.deltaTime);
				flockB.velocity = Mathf.Lerp(flockA.velocity, 7f, 10f * Time.deltaTime);
				rotationScalar = Mathf.Lerp(rotationScalar, 3f, 10f * Time.deltaTime);
				t += Time.deltaTime;
				if (t > 20f && !timerTriggered)
				{
					timerA.gameObject.SetActive(value: true);
					timerB.gameObject.SetActive(value: true);
					timerTriggered = true;
				}
			}
			else
			{
				targetVolume = Mathf.Clamp(Mathf.Lerp(targetVolume, 0.2f, 2f * Time.deltaTime), 0.21f, 0.99f);
				flockA.velocity = Mathf.Lerp(flockA.velocity, 3.4f, 10f * Time.deltaTime);
				flockB.velocity = Mathf.Lerp(flockA.velocity, 2.4f, 10f * Time.deltaTime);
				rotationScalar = Mathf.Lerp(rotationScalar, 0.5f, 10f * Time.deltaTime);
			}
			if (trigger.objectInside && ToolManager.PlayerGrabbingGameObject(keyGO.gameObject, dropIfGrabbing: true, playSFX: false))
			{
				keyInside = true;
				PersistentDataManager.SetElevatorKey(ElevatorSaveData.KeyStates.Elevator);
				if (keyGO.GetComponent<ObjectBase>() != null)
				{
					keyGO.GetComponent<ObjectBase>().ungrabbable = true;
				}
				if (keyGO.GetComponent<ObjectBoid>() != null)
				{
					keyGO.GetComponent<ObjectBoid>().enabled = false;
				}
				if (keyGO.GetComponent<Rigidbody>() != null)
				{
					keyGO.GetComponent<Rigidbody>().useGravity = false;
					keyGO.GetComponent<Rigidbody>().isKinematic = true;
				}
				if (!timerTriggered)
				{
					timerA.gameObject.SetActive(value: true);
					timerB.gameObject.SetActive(value: true);
					timerTriggered = true;
				}
				keyGO.animateToElevator();
			}
		}
		if (targetVolume > 0.21f && targetVolume < 0.99f)
		{
			for (int i = 0; i < buzzAudio.Count; i++)
			{
				buzzAudio[i].volume = targetVolume - 0.1f;
			}
			grabMusic.volume = targetVolume - 0.21f;
		}
	}
}
