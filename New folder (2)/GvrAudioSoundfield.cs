using System;
using UnityEngine;
using UnityEngine.Audio;

[Obsolete("GvrAudioSoundfield is deprecated. Please upgrade to Resonance Audio (https://developers.google.com/resonance-audio/migrate).")]
[AddComponentMenu("GoogleVR/Audio/Deprecated/GvrAudioSoundfield")]
public class GvrAudioSoundfield : MonoBehaviour
{
	public bool bypassRoomEffects = true;

	public float gainDb;

	public bool playOnAwake = true;

	[SerializeField]
	private AudioClip soundfieldClip0102;

	[SerializeField]
	private AudioClip soundfieldClip0304;

	[SerializeField]
	private bool soundfieldLoop;

	[SerializeField]
	private bool soundfieldMute;

	[SerializeField]
	[Range(-3f, 3f)]
	private float soundfieldPitch = 1f;

	[SerializeField]
	[Range(0f, 256f)]
	private int soundfieldPriority = 32;

	[SerializeField]
	[Range(0f, 1f)]
	private float soundfieldSpatialBlend;

	[SerializeField]
	[Range(0f, 5f)]
	private float soundfieldDopplerLevel;

	[SerializeField]
	[Range(0f, 1f)]
	private float soundfieldVolume = 1f;

	[SerializeField]
	private AudioRolloffMode soundfieldRolloffMode;

	[SerializeField]
	private float soundfieldMaxDistance = 500f;

	[SerializeField]
	private float soundfieldMinDistance = 1f;

	private int id = -1;

	private AudioSource[] audioSources;

	private bool isPaused;

	public AudioClip clip0102
	{
		get
		{
			return soundfieldClip0102;
		}
		set
		{
			soundfieldClip0102 = value;
			if (audioSources != null && audioSources.Length > 0)
			{
				audioSources[0].clip = soundfieldClip0102;
			}
		}
	}

	public AudioClip clip0304
	{
		get
		{
			return soundfieldClip0304;
		}
		set
		{
			soundfieldClip0304 = value;
			if (audioSources != null && audioSources.Length > 0)
			{
				audioSources[1].clip = soundfieldClip0304;
			}
		}
	}

	public bool isPlaying
	{
		get
		{
			if (audioSources != null && audioSources.Length > 0)
			{
				return audioSources[0].isPlaying;
			}
			return false;
		}
	}

	public bool loop
	{
		get
		{
			return soundfieldLoop;
		}
		set
		{
			soundfieldLoop = value;
			if (audioSources != null)
			{
				for (int i = 0; i < audioSources.Length; i++)
				{
					audioSources[i].loop = soundfieldLoop;
				}
			}
		}
	}

	public bool mute
	{
		get
		{
			return soundfieldMute;
		}
		set
		{
			soundfieldMute = value;
			if (audioSources != null)
			{
				for (int i = 0; i < audioSources.Length; i++)
				{
					audioSources[i].mute = soundfieldMute;
				}
			}
		}
	}

	public float pitch
	{
		get
		{
			return soundfieldPitch;
		}
		set
		{
			soundfieldPitch = value;
			if (audioSources != null)
			{
				for (int i = 0; i < audioSources.Length; i++)
				{
					audioSources[i].pitch = soundfieldPitch;
				}
			}
		}
	}

	public int priority
	{
		get
		{
			return soundfieldPriority;
		}
		set
		{
			soundfieldPriority = value;
			if (audioSources != null)
			{
				for (int i = 0; i < audioSources.Length; i++)
				{
					audioSources[i].priority = soundfieldPriority;
				}
			}
		}
	}

	public float spatialBlend
	{
		get
		{
			return soundfieldSpatialBlend;
		}
		set
		{
			soundfieldSpatialBlend = value;
			if (audioSources != null)
			{
				for (int i = 0; i < audioSources.Length; i++)
				{
					audioSources[i].spatialBlend = soundfieldSpatialBlend;
				}
			}
		}
	}

	public float dopplerLevel
	{
		get
		{
			return soundfieldDopplerLevel;
		}
		set
		{
			soundfieldDopplerLevel = value;
			if (audioSources != null)
			{
				for (int i = 0; i < audioSources.Length; i++)
				{
					audioSources[i].dopplerLevel = soundfieldDopplerLevel;
				}
			}
		}
	}

	public float time
	{
		get
		{
			if (audioSources != null && audioSources.Length > 0)
			{
				return audioSources[0].time;
			}
			return 0f;
		}
		set
		{
			if (audioSources != null)
			{
				for (int i = 0; i < audioSources.Length; i++)
				{
					audioSources[i].time = value;
				}
			}
		}
	}

	public int timeSamples
	{
		get
		{
			if (audioSources != null && audioSources.Length > 0)
			{
				return audioSources[0].timeSamples;
			}
			return 0;
		}
		set
		{
			if (audioSources != null)
			{
				for (int i = 0; i < audioSources.Length; i++)
				{
					audioSources[i].timeSamples = value;
				}
			}
		}
	}

	public float volume
	{
		get
		{
			return soundfieldVolume;
		}
		set
		{
			soundfieldVolume = value;
			if (audioSources != null)
			{
				for (int i = 0; i < audioSources.Length; i++)
				{
					audioSources[i].volume = soundfieldVolume;
				}
			}
		}
	}

	public AudioRolloffMode rolloffMode
	{
		get
		{
			return soundfieldRolloffMode;
		}
		set
		{
			soundfieldRolloffMode = value;
			if (audioSources == null)
			{
				return;
			}
			for (int i = 0; i < audioSources.Length; i++)
			{
				audioSources[i].rolloffMode = soundfieldRolloffMode;
				if (rolloffMode == AudioRolloffMode.Custom)
				{
					audioSources[i].SetCustomCurve(AudioSourceCurveType.CustomRolloff, AnimationCurve.Linear(soundfieldMinDistance, 1f, soundfieldMaxDistance, 1f));
				}
			}
		}
	}

	public float maxDistance
	{
		get
		{
			return soundfieldMaxDistance;
		}
		set
		{
			soundfieldMaxDistance = Mathf.Clamp(value, soundfieldMinDistance + 0.01f, 1000000f);
			if (audioSources != null)
			{
				for (int i = 0; i < audioSources.Length; i++)
				{
					audioSources[i].maxDistance = soundfieldMaxDistance;
				}
			}
		}
	}

	public float minDistance
	{
		get
		{
			return soundfieldMinDistance;
		}
		set
		{
			soundfieldMinDistance = Mathf.Clamp(value, 0f, 990099f);
			if (audioSources != null)
			{
				for (int i = 0; i < audioSources.Length; i++)
				{
					audioSources[i].minDistance = soundfieldMinDistance;
				}
			}
		}
	}

	private void Awake()
	{
		AudioMixer audioMixer = Resources.Load("GvrAudioMixer") as AudioMixer;
		if (audioMixer == null)
		{
			Debug.LogError("GVRAudioMixer could not be found in Resources. Make sure that the GVR SDKUnity package is imported properly.");
			return;
		}
		audioSources = new AudioSource[2];
		for (int i = 0; i < audioSources.Length; i++)
		{
			GameObject gameObject = new GameObject("Channel Set " + i);
			gameObject.transform.parent = base.gameObject.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			audioSources[i] = gameObject.AddComponent<AudioSource>();
			audioSources[i].enabled = false;
			audioSources[i].playOnAwake = false;
			audioSources[i].bypassReverbZones = true;
			audioSources[i].spatializePostEffects = true;
			audioSources[i].outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[0];
		}
		OnValidate();
	}

	private void OnEnable()
	{
		for (int i = 0; i < audioSources.Length; i++)
		{
			audioSources[i].enabled = true;
		}
		if (playOnAwake && !isPlaying && InitializeSoundfield())
		{
			Play();
		}
	}

	private void Start()
	{
		if (playOnAwake && !isPlaying)
		{
			Play();
		}
	}

	private void OnDisable()
	{
		Stop();
		for (int i = 0; i < audioSources.Length; i++)
		{
			audioSources[i].enabled = false;
		}
	}

	private void OnDestroy()
	{
		for (int i = 0; i < audioSources.Length; i++)
		{
			UnityEngine.Object.Destroy(audioSources[i].gameObject);
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			Pause();
		}
		else
		{
			UnPause();
		}
	}

	private void Update()
	{
		if (!isPlaying && !isPaused)
		{
			Stop();
			return;
		}
		for (int i = 0; i < audioSources.Length; i++)
		{
			audioSources[i].SetSpatializerFloat(4, GvrAudio.ConvertAmplitudeFromDb(gainDb));
			audioSources[i].SetSpatializerFloat(6, soundfieldMinDistance);
		}
		GvrAudio.UpdateAudioSoundfield(id, this);
	}

	public void Pause()
	{
		if (audioSources != null)
		{
			isPaused = true;
			for (int i = 0; i < audioSources.Length; i++)
			{
				audioSources[i].Pause();
			}
		}
	}

	public void Play()
	{
		double dspTime = AudioSettings.dspTime;
		PlayScheduled(dspTime);
	}

	public void PlayDelayed(float delay)
	{
		double num = AudioSettings.dspTime + (double)delay;
		PlayScheduled(num);
	}

	public void PlayScheduled(double time)
	{
		if (audioSources != null && InitializeSoundfield())
		{
			for (int i = 0; i < audioSources.Length; i++)
			{
				audioSources[i].PlayScheduled(time);
			}
			isPaused = false;
		}
		else
		{
			Debug.LogWarning("GVR Audio soundfield not initialized. Audio playback not supported until after Awake() and OnEnable(). Try calling from Start() instead.");
		}
	}

	public void SetScheduledEndTime(double time)
	{
		if (audioSources != null)
		{
			for (int i = 0; i < audioSources.Length; i++)
			{
				audioSources[i].SetScheduledEndTime(time);
			}
		}
	}

	public void SetScheduledStartTime(double time)
	{
		if (audioSources != null)
		{
			for (int i = 0; i < audioSources.Length; i++)
			{
				audioSources[i].SetScheduledStartTime(time);
			}
		}
	}

	public void Stop()
	{
		if (audioSources != null)
		{
			for (int i = 0; i < audioSources.Length; i++)
			{
				audioSources[i].Stop();
			}
			ShutdownSoundfield();
			isPaused = false;
		}
	}

	public void UnPause()
	{
		if (audioSources != null)
		{
			for (int i = 0; i < audioSources.Length; i++)
			{
				audioSources[i].UnPause();
			}
			isPaused = false;
		}
	}

	private bool InitializeSoundfield()
	{
		if (id < 0)
		{
			id = GvrAudio.CreateAudioSoundfield();
			if (id >= 0)
			{
				GvrAudio.UpdateAudioSoundfield(id, this);
				for (int i = 0; i < audioSources.Length; i++)
				{
					InitializeChannelSet(audioSources[i], i);
				}
			}
		}
		return id >= 0;
	}

	private void ShutdownSoundfield()
	{
		if (id >= 0)
		{
			for (int i = 0; i < audioSources.Length; i++)
			{
				ShutdownChannelSet(audioSources[i], i);
			}
			GvrAudio.DestroyAudioSource(id);
			id = -1;
		}
	}

	private void InitializeChannelSet(AudioSource source, int channelSet)
	{
		source.spatialize = true;
		source.SetSpatializerFloat(1, 1f);
		source.SetSpatializerFloat(2, 4f);
		source.SetSpatializerFloat(3, channelSet);
		source.SetSpatializerFloat(4, GvrAudio.ConvertAmplitudeFromDb(gainDb));
		source.SetSpatializerFloat(6, soundfieldMinDistance);
		source.SetSpatializerFloat(7, 0f);
		source.SetSpatializerFloat(0, id);
	}

	private void ShutdownChannelSet(AudioSource source, int channelSet)
	{
		source.SetSpatializerFloat(0, -1f);
		source.SetSpatializerFloat(7, 1f);
		source.spatialize = false;
	}

	private void OnDidApplyAnimationProperties()
	{
		OnValidate();
	}

	private void OnValidate()
	{
		clip0102 = soundfieldClip0102;
		clip0304 = soundfieldClip0304;
		loop = soundfieldLoop;
		mute = soundfieldMute;
		pitch = soundfieldPitch;
		priority = soundfieldPriority;
		spatialBlend = soundfieldSpatialBlend;
		volume = soundfieldVolume;
		dopplerLevel = soundfieldDopplerLevel;
		minDistance = soundfieldMinDistance;
		maxDistance = soundfieldMaxDistance;
		rolloffMode = soundfieldRolloffMode;
	}
}
