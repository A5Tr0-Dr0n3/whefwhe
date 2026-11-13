using System;
using UnityEngine;

public class SeaShell : ObjectBase
{
	private Vector3 startPosition;

	private Vector3 sinkPosition;

	private Vector3 dropSinkPosition;

	private float initialShellSinkDistance = 0.4f;

	private float onDropSinkDistance = 0.25f;

	private bool isRising;

	private bool isSinking;

	private bool hasSunk;

	private float moveTime;

	private float moveTimeMax = 2f;

	private bool beingHeld;

	private float moveT;

	public Transform floatTarget;

	private Vector3 floatEuler;

	private Vector3 floatPosition;

	private float floatRate = 0.1f;

	private Vector3 slowFloatStartPosition;

	private Vector3 slowFloatStartEuler;

	private float letGoFloatTimeMax = 1.5f;

	private float letGoFloatTime;

	public VVR_Eliza elizaManagerReference;

	private bool _beenGrabbed;

	public GvrAudioSource shellSource;

	public GvrAudioSource oceanSource;

	public float oceanFadeRate = 1f;

	private float oceanMinVolume = 0.2f;

	private float oceanMaxVolume = 1f;

	private float oceanStartVolume;

	private float startVolume;

	public NPCDialogueClip elizaIntro;

	public NPCDialogueClip elizaResponse;

	private SeaShellTrigger playTrigger;

	private bool fadingIn;

	private bool fadingOut;

	private float fadeTime = 0.8f;

	private float fadeCount;

	private float fadeT;

	private bool currentClipCanBeSkipped;

	private float allowSkipPercent = 0.5f;

	private bool doSink;

	private bool skipOnDrop;

	private bool hasStartedPlaying;

	private bool shellPaused;

	public Collider playerHeadCollider;

	private bool sentFinishedMessage;

	[SerializeField]
	private SubtitleRenderer subtitles;

	[SerializeField]
	private SubtitleRenderer elizaSubtitles;

	public bool beenGrabbed => _beenGrabbed;

	public bool sentFinishedReference => sentFinishedMessage;

	protected override void Start()
	{
		base.Start();
		shellSource = GetComponent<GvrAudioSource>();
		playTrigger = GetComponentInChildren<SeaShellTrigger>();
		playTrigger.SetShell(this);
		rb.isKinematic = true;
		startPosition = transform.position;
		dropSinkPosition = startPosition;
		dropSinkPosition.y -= onDropSinkDistance;
		sinkPosition = startPosition;
		sinkPosition.y -= initialShellSinkDistance;
		transform.position = sinkPosition;
		isRising = true;
		startVolume = shellSource.volume;
		oceanSource.volume = oceanMinVolume;
	}

	private void Update()
	{
		if (fadingIn)
		{
			fadeCount += Time.deltaTime;
			if (fadeCount > fadeTime)
			{
				fadingIn = false;
				elizaManagerReference.SetAllowElizaToContinue(newContinue: false);
				subtitles.SetIsPlaying(s: true);
				if ((bool)elizaSubtitles)
				{
					elizaSubtitles.gameObject.SetActive(value: false);
				}
			}
			fadeT = fadeCount / fadeTime;
			fadeT = Mathf.Sin(fadeT * (float)Math.PI * 0.5f);
			shellSource.volume = Mathf.Lerp(0f, startVolume, fadeT);
			if (!shellSource.isPlaying)
			{
				oceanSource.volume = Mathf.Lerp(0f, oceanMaxVolume, fadeT);
			}
			else
			{
				oceanSource.volume = Mathf.Lerp(0f, oceanMinVolume, fadeT);
			}
			subtitles.SetIsPlaying(s: true);
			if ((bool)elizaSubtitles)
			{
				elizaSubtitles.gameObject.SetActive(value: false);
			}
		}
		if (fadingOut)
		{
			fadeCount += Time.deltaTime;
			if (fadeCount > fadeTime)
			{
				fadingOut = false;
				if (shellSource.isPlaying)
				{
					shellSource.Pause();
					subtitles.SetIsPlaying(s: false);
					if (elizaSubtitles != null)
					{
						elizaSubtitles.gameObject.SetActive(value: true);
					}
					oceanSource.Pause();
					shellPaused = true;
				}
				else
				{
					shellPaused = false;
					skipOnDrop = true;
				}
			}
			fadeT = fadeCount / fadeTime;
			fadeT = Mathf.Sin(fadeT * (float)Math.PI * 0.5f);
			shellSource.volume = Mathf.Lerp(startVolume, 0f, fadeT);
			oceanSource.volume = Mathf.Lerp(oceanStartVolume, 0f, fadeT);
		}
		if (!beingHeld)
		{
			if (isRising)
			{
				moveTime += Time.deltaTime;
				moveT = moveTime / moveTimeMax;
				moveT = Mathf.Sin(moveT * (float)Math.PI * 0.5f);
				rb.MovePosition(Vector3.Lerp(sinkPosition, startPosition, moveT));
				if (moveTime > moveTimeMax)
				{
					isRising = false;
					moveTime = 0f;
				}
			}
			else if (isSinking)
			{
				moveTime += Time.deltaTime;
				moveT = moveTime / moveTimeMax;
				moveT = Mathf.Sin(moveT * (float)Math.PI * 0.5f);
				rb.MovePosition(Vector3.Lerp(startPosition, dropSinkPosition, moveT));
				if (moveTime > moveTimeMax)
				{
					isSinking = false;
					moveTime = 0f;
				}
			}
			else if (!hasSunk && !doSink)
			{
				floatPosition = floatTarget.position;
				floatEuler = floatTarget.rotation.eulerAngles;
				letGoFloatTime -= Time.deltaTime;
				if (letGoFloatTime > 0f)
				{
					float num = letGoFloatTime / letGoFloatTimeMax;
					num = Mathf.Sin(num * (float)Math.PI * 0.5f);
					floatPosition = Vector3.Lerp(floatPosition, slowFloatStartPosition, num);
					rb.MoveRotation(Quaternion.Euler(Vector3.Lerp(floatEuler, slowFloatStartEuler, num)));
					rb.detectCollisions = false;
				}
				else
				{
					floatPosition.x = (1f - floatRate) * floatPosition.x + floatRate * floatPosition.x;
					floatPosition.y = (1f - floatRate) * floatPosition.y + floatRate * floatPosition.y;
					floatPosition.z = (1f - floatRate) * floatPosition.z + floatRate * floatPosition.z;
					rb.MoveRotation(floatTarget.rotation);
					rb.detectCollisions = true;
				}
				rb.MovePosition(floatPosition);
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
			}
		}
		else
		{
			playerHeadCollider.transform.position = EagerMaster.instance.mainCamera.transform.position;
			if (!shellSource.isPlaying && oceanSource.isPlaying && oceanSource.volume < oceanMaxVolume)
			{
				oceanSource.volume += oceanFadeRate * Time.deltaTime;
			}
		}
		if (ShellIsPlaying())
		{
			subtitles.SetIsPlaying(s: true);
			if ((bool)elizaSubtitles)
			{
				elizaSubtitles.gameObject.SetActive(value: false);
			}
		}
	}

	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		rb.isKinematic = false;
		beingHeld = true;
		isRising = false;
		isSinking = false;
		hasSunk = false;
		moveTime = 0f;
		playerHeadCollider.enabled = true;
		playerHeadCollider.transform.position = EagerMaster.instance.mainCamera.transform.position;
		shellSource.clip = elizaManagerReference.GetNextShellDialogue();
		shellSource.timeSamples = elizaManagerReference.GetShellDialoguePosition();
		currentClipCanBeSkipped = elizaManagerReference.GetCurrentShellSkipState();
		allowSkipPercent = elizaManagerReference.GetCurrentShellSkipPercent();
		elizaManagerReference.SetShellsUngrabbable(newGrab: true);
		hasStartedPlaying = false;
		if (elizaManagerReference.GetShellDialoguePosition() > 0)
		{
			shellPaused = true;
		}
		else
		{
			shellPaused = false;
		}
		if (playTrigger.playerInRange)
		{
			SetShellFadeState(isOn: true);
		}
	}

	public override void GrabRelease(Manipulator grabber)
	{
		base.GrabRelease(grabber);
		beingHeld = false;
		isSinking = false;
		playerHeadCollider.enabled = false;
		elizaManagerReference.SetAllowElizaToContinue(newContinue: true);
		letGoFloatTime = letGoFloatTimeMax;
		slowFloatStartPosition = transform.position;
		slowFloatStartEuler = transform.rotation.eulerAngles;
		elizaManagerReference.SetShellsUngrabbable(newGrab: false);
		if (oceanSource.isPlaying)
		{
			oceanSource.Pause();
		}
		if (hasStartedPlaying)
		{
			if (shellSource.isPlaying)
			{
				if (skipOnDrop || currentClipCanBeSkipped || (!currentClipCanBeSkipped && shellSource.timeSamples > Mathf.FloorToInt((float)shellSource.clip.samples * allowSkipPercent)))
				{
					if (!sentFinishedMessage)
					{
						elizaManagerReference.AddShellListened();
						elizaManagerReference.SetShellAudioSamples(0);
						sentFinishedMessage = true;
					}
					shellPaused = false;
					doSink = true;
				}
				else
				{
					shellPaused = true;
					elizaManagerReference.SetShellAudioSamples(shellSource.timeSamples);
					rb.velocity = Vector3.zero;
					rb.angularVelocity = Vector3.zero;
					rb.isKinematic = true;
				}
				shellSource.Pause();
				subtitles.SetIsPlaying(s: false);
				if ((bool)elizaSubtitles)
				{
					elizaSubtitles.gameObject.SetActive(value: true);
				}
			}
			else if (!shellPaused)
			{
				if (!doSink && !sentFinishedMessage)
				{
					elizaManagerReference.AddShellListened();
					elizaManagerReference.SetShellAudioSamples(0);
					sentFinishedMessage = true;
				}
				doSink = true;
			}
			else if (skipOnDrop || currentClipCanBeSkipped || (!currentClipCanBeSkipped && shellSource.timeSamples > Mathf.FloorToInt((float)shellSource.clip.samples * allowSkipPercent)))
			{
				if (!doSink && !sentFinishedMessage)
				{
					elizaManagerReference.AddShellListened();
					elizaManagerReference.SetShellAudioSamples(0);
					sentFinishedMessage = true;
				}
				shellPaused = false;
				doSink = true;
			}
			else
			{
				elizaManagerReference.SetShellAudioSamples(shellSource.timeSamples);
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
				rb.isKinematic = true;
			}
		}
		hasStartedPlaying = false;
		if (!_beenGrabbed)
		{
			_beenGrabbed = true;
			if (elizaResponse != null)
			{
				elizaManagerReference.PlayShellResponse(elizaResponse);
			}
		}
		subtitles.SetIsPlaying(s: false);
		if ((bool)elizaSubtitles)
		{
			elizaSubtitles.gameObject.SetActive(value: true);
		}
	}

	public bool ShellIsPlaying()
	{
		return shellSource.isPlaying;
	}

	public void SetShellFadeState(bool isOn)
	{
		fadeCount = 0f;
		fadeT = 0f;
		fadingIn = (fadingOut = false);
		if (isOn)
		{
			if (doSink)
			{
				return;
			}
			fadingIn = true;
			if (!oceanSource.isPlaying)
			{
				oceanSource.Play();
			}
			if (shellPaused)
			{
				shellPaused = false;
				if (hasStartedPlaying)
				{
					shellSource.UnPause();
					subtitles.SetIsPlaying(s: true);
					if (elizaSubtitles != null)
					{
						elizaSubtitles.gameObject.SetActive(value: false);
					}
				}
				else
				{
					shellSource.Play();
					subtitles.SetIsPlaying(s: true);
					if (elizaSubtitles != null)
					{
						elizaSubtitles.gameObject.SetActive(value: false);
					}
				}
			}
			else if (!skipOnDrop && !shellSource.isPlaying)
			{
				shellSource.Play();
				subtitles.SetIsPlaying(s: true);
				if (elizaSubtitles != null)
				{
					elizaSubtitles.gameObject.SetActive(value: false);
				}
			}
			hasStartedPlaying = true;
		}
		else
		{
			fadingOut = true;
			oceanStartVolume = oceanSource.volume;
			elizaManagerReference.SetAllowElizaToContinue(newContinue: true);
			subtitles.SetIsPlaying(s: false);
			if (elizaSubtitles != null)
			{
				elizaSubtitles.gameObject.SetActive(value: true);
			}
		}
	}
}
