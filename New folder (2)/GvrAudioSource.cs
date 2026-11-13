using UnityEngine;
using UnityEngine.Audio;

[AddComponentMenu("GoogleVR/Audio/Deprecated/GvrAudioSource")]
public class GvrAudioSource : MonoBehaviour
{
	public bool bypassRoomEffects;

	public float directivityAlpha;

	public float directivitySharpness = 1f;

	public float listenerDirectivityAlpha;

	public float listenerDirectivitySharpness = 1f;

	public float gainDb;

	public bool occlusionEnabled;

	public bool playOnAwake = true;

	public bool resumeOnReenable;

	[SerializeField]
	private AudioClip sourceClip;

	[SerializeField]
	private bool sourceLoop;

	[SerializeField]
	private bool sourceMute;

	[SerializeField]
	[Range(-3f, 3f)]
	private float sourcePitch = 1f;

	[SerializeField]
	[Range(0f, 256f)]
	private int sourcePriority = 128;

	[SerializeField]
	[Range(0f, 1f)]
	private float sourceSpatialBlend = 1f;

	[SerializeField]
	[Range(0f, 5f)]
	private float sourceDopplerLevel = 1f;

	[SerializeField]
	[Range(0f, 360f)]
	private float sourceSpread;

	[SerializeField]
	[Range(0f, 1f)]
	private float sourceVolume = 1f;

	[SerializeField]
	private AudioRolloffMode sourceRolloffMode;

	[SerializeField]
	private float sourceMaxDistance = 500f;

	[SerializeField]
	private float sourceMinDistance = 1f;

	[SerializeField]
	private bool hrtfEnabled = true;

	[SerializeField]
	private AudioSource audioSource;

	private int id = -1;

	private float currentOcclusion;

	private float nextOcclusionUpdate;

	private bool isPaused;

	private int _disableSamplePoint;

	private bool _disabledWhilePlaying;

	public AudioClip clip
	{
		get
		{
			return sourceClip;
		}
		set
		{
			sourceClip = value;
			if (audioSource != null)
			{
				audioSource.clip = sourceClip;
			}
		}
	}

	public bool isPlaying
	{
		get
		{
			if (audioSource != null)
			{
				return audioSource.isPlaying;
			}
			return false;
		}
	}

	public bool loop
	{
		get
		{
			return sourceLoop;
		}
		set
		{
			sourceLoop = value;
			if (audioSource != null)
			{
				audioSource.loop = sourceLoop;
			}
		}
	}

	public bool mute
	{
		get
		{
			return sourceMute;
		}
		set
		{
			sourceMute = value;
			if (audioSource != null)
			{
				audioSource.mute = sourceMute;
			}
		}
	}

	public float pitch
	{
		get
		{
			return sourcePitch;
		}
		set
		{
			sourcePitch = value;
			if (audioSource != null)
			{
				audioSource.pitch = sourcePitch;
			}
		}
	}

	public int priority
	{
		get
		{
			return sourcePriority;
		}
		set
		{
			sourcePriority = value;
			if (audioSource != null)
			{
				audioSource.priority = sourcePriority;
			}
		}
	}

	public float spatialBlend
	{
		get
		{
			return sourceSpatialBlend;
		}
		set
		{
			sourceSpatialBlend = value;
			if (audioSource != null)
			{
				audioSource.spatialBlend = sourceSpatialBlend;
			}
		}
	}

	public float dopplerLevel
	{
		get
		{
			return sourceDopplerLevel;
		}
		set
		{
			sourceDopplerLevel = value;
			if (audioSource != null)
			{
				audioSource.dopplerLevel = sourceDopplerLevel;
			}
		}
	}

	public float spread
	{
		get
		{
			return sourceSpread;
		}
		set
		{
			sourceSpread = value;
			if (audioSource != null)
			{
				audioSource.spread = sourceSpread;
			}
		}
	}

	public float time
	{
		get
		{
			if (audioSource != null)
			{
				return audioSource.time;
			}
			return 0f;
		}
		set
		{
			if (audioSource != null)
			{
				audioSource.time = value;
			}
		}
	}

	public int timeSamples
	{
		get
		{
			if (audioSource != null)
			{
				return audioSource.timeSamples;
			}
			return 0;
		}
		set
		{
			if (audioSource != null)
			{
				audioSource.timeSamples = value;
			}
		}
	}

	public float volume
	{
		get
		{
			return sourceVolume;
		}
		set
		{
			sourceVolume = value;
			if (audioSource != null)
			{
				audioSource.volume = sourceVolume;
			}
		}
	}

	public AudioRolloffMode rolloffMode
	{
		get
		{
			return sourceRolloffMode;
		}
		set
		{
			sourceRolloffMode = value;
			if (audioSource != null)
			{
				audioSource.rolloffMode = sourceRolloffMode;
				if (rolloffMode == AudioRolloffMode.Custom)
				{
					audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, AnimationCurve.Linear(sourceMinDistance, 1f, sourceMaxDistance, 1f));
				}
			}
		}
	}

	public float maxDistance
	{
		get
		{
			return sourceMaxDistance;
		}
		set
		{
			sourceMaxDistance = Mathf.Clamp(value, sourceMinDistance + 0.01f, 1000000f);
			if (audioSource != null)
			{
				audioSource.maxDistance = sourceMaxDistance;
			}
		}
	}

	public float minDistance
	{
		get
		{
			return sourceMinDistance;
		}
		set
		{
			sourceMinDistance = Mathf.Clamp(value, 0f, 990099f);
			if (audioSource != null)
			{
				audioSource.minDistance = sourceMinDistance;
			}
		}
	}

	private void Awake()
	{
		if (audioSource == null)
		{
			audioSource = gameObject.AddComponent<AudioSource>();
		}
		audioSource.enabled = false;
		audioSource.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
		audioSource.playOnAwake = false;
		audioSource.bypassReverbZones = true;
		audioSource.spatializePostEffects = true;
		OnValidate();
		AudioMixer audioMixer = Resources.Load("GvrAudioMixer") as AudioMixer;
		if (audioMixer != null)
		{
			audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master")[0];
		}
		else
		{
			Debug.LogError("GVRAudioMixer could not be found in Resources. Make sure that the GVR SDK Unity package is imported properly.");
		}
	}

	private void OnEnable()
	{
		audioSource.enabled = true;
		if (resumeOnReenable && _disabledWhilePlaying)
		{
			_disabledWhilePlaying = false;
			timeSamples = _disableSamplePoint;
			Play();
		}
		else if (playOnAwake && !isPlaying && InitializeSource())
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
		if (resumeOnReenable && isPlaying)
		{
			_disableSamplePoint = timeSamples;
			_disabledWhilePlaying = true;
		}
		Stop();
		audioSource.enabled = false;
	}

	private void OnDestroy()
	{
		Object.Destroy(audioSource);
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
		if (!occlusionEnabled)
		{
			currentOcclusion = 0f;
		}
		else if (Time.time >= nextOcclusionUpdate)
		{
			nextOcclusionUpdate = Time.time + 0.2f;
			currentOcclusion = GvrAudio.ComputeOcclusion(transform);
		}
		if (!isPlaying && !isPaused)
		{
			Stop();
			return;
		}
		audioSource.SetSpatializerFloat(4, GvrAudio.ConvertAmplitudeFromDb(gainDb));
		audioSource.SetSpatializerFloat(6, sourceMinDistance);
		GvrAudio.UpdateAudioSource(id, this, currentOcclusion);
	}

	public void GetOutputData(float[] samples, int channel)
	{
		if (audioSource != null)
		{
			audioSource.GetOutputData(samples, channel);
		}
	}

	public void GetSpectrumData(float[] samples, int channel, FFTWindow window)
	{
		if (audioSource != null)
		{
			audioSource.GetSpectrumData(samples, channel, window);
		}
	}

	public void Pause()
	{
		if (audioSource != null)
		{
			isPaused = true;
			audioSource.Pause();
		}
	}

	public void Play()
	{
		if (audioSource != null && InitializeSource())
		{
			audioSource.Play();
			isPaused = false;
		}
		else
		{
			Debug.LogWarning("GVR Audio source not initialized. Audio playback not supported until after Awake() and OnEnable(). Try calling from Start() instead.");
		}
	}

	public void PlayDelayed(float delay)
	{
		if (audioSource != null && InitializeSource())
		{
			audioSource.PlayDelayed(delay);
			isPaused = false;
		}
		else
		{
			Debug.LogWarning("GVR Audio source not initialized. Audio playback not supported until after Awake() and OnEnable(). Try calling from Start() instead.");
		}
	}

	public void PlayOneShot(AudioClip clip)
	{
		PlayOneShot(clip, 1f);
	}

	public void PlayOneShot(AudioClip clip, float volume)
	{
		if (audioSource != null && InitializeSource())
		{
			audioSource.PlayOneShot(clip, volume);
			isPaused = false;
		}
		else
		{
			Debug.LogWarning("GVR Audio source not initialized. Audio playback not supported until after Awake() and OnEnable(). Try calling from Start() instead.");
		}
	}

	public void PlayScheduled(double time)
	{
		if (audioSource != null && InitializeSource())
		{
			audioSource.PlayScheduled(time);
			isPaused = false;
		}
		else
		{
			Debug.LogWarning("GVR Audio source not initialized. Audio playback not supported until after Awake() and OnEnable(). Try calling from Start() instead.");
		}
	}

	public void SetScheduledEndTime(double time)
	{
		if (audioSource != null)
		{
			audioSource.SetScheduledEndTime(time);
		}
	}

	public void SetScheduledStartTime(double time)
	{
		if (audioSource != null)
		{
			audioSource.SetScheduledStartTime(time);
		}
	}

	public void Stop()
	{
		if (audioSource != null)
		{
			audioSource.Stop();
			ShutdownSource();
			isPaused = true;
		}
	}

	public void UnPause()
	{
		if (audioSource != null)
		{
			audioSource.UnPause();
			isPaused = false;
		}
	}

	private bool InitializeSource()
	{
		if (id < 0)
		{
			id = GvrAudio.CreateAudioSource(hrtfEnabled);
			if (id >= 0)
			{
				GvrAudio.UpdateAudioSource(id, this, currentOcclusion);
				audioSource.spatialize = true;
				audioSource.SetSpatializerFloat(1, 0f);
				audioSource.SetSpatializerFloat(4, GvrAudio.ConvertAmplitudeFromDb(gainDb));
				audioSource.SetSpatializerFloat(6, sourceMinDistance);
				audioSource.SetSpatializerFloat(7, 0f);
				audioSource.SetSpatializerFloat(0, id);
			}
		}
		return id >= 0;
	}

	private void ShutdownSource()
	{
		if (id >= 0)
		{
			audioSource.SetSpatializerFloat(0, -1f);
			audioSource.SetSpatializerFloat(7, 1f);
			audioSource.spatialize = false;
			GvrAudio.DestroyAudioSource(id);
			id = -1;
		}
	}

	private void OnDidApplyAnimationProperties()
	{
		OnValidate();
	}

	private void OnValidate()
	{
		clip = sourceClip;
		loop = sourceLoop;
		mute = sourceMute;
		pitch = sourcePitch;
		priority = sourcePriority;
		spatialBlend = sourceSpatialBlend;
		volume = sourceVolume;
		dopplerLevel = sourceDopplerLevel;
		spread = sourceSpread;
		minDistance = sourceMinDistance;
		maxDistance = sourceMaxDistance;
		rolloffMode = sourceRolloffMode;
	}

	private void OnDrawGizmosSelected()
	{
		GvrAudioListener gvrAudioListener = Object.FindObjectOfType<GvrAudioListener>();
		if (gvrAudioListener != null)
		{
			Gizmos.color = GvrAudio.listenerDirectivityColor;
			DrawDirectivityGizmo(gvrAudioListener.transform, listenerDirectivityAlpha, listenerDirectivitySharpness, 180);
		}
		Gizmos.color = GvrAudio.sourceDirectivityColor;
		DrawDirectivityGizmo(transform, directivityAlpha, directivitySharpness, 180);
	}

	private void DrawDirectivityGizmo(Transform target, float alpha, float sharpness, int resolution)
	{
		Vector2[] array = GvrAudio.Generate2dPolarPattern(alpha, sharpness, resolution);
		int num = resolution + 1;
		Vector3[] array2 = new Vector3[num];
		ref Vector3 reference = ref array2[0];
		reference = Vector3.zero;
		for (int i = 0; i < array.Length; i++)
		{
			ref Vector3 reference2 = ref array2[i + 1];
			reference2 = new Vector3(array[i].x, 0f, array[i].y);
		}
		int[] array3 = new int[6 * num];
		for (int j = 0; j < num - 1; j++)
		{
			int num2 = 6 * j;
			if (j < num - 2)
			{
				array3[num2] = 0;
				array3[num2 + 1] = j + 1;
				array3[num2 + 2] = j + 2;
			}
			else
			{
				array3[num2] = 0;
				array3[num2 + 1] = num - 1;
				array3[num2 + 2] = 1;
			}
			array3[num2 + 3] = array3[num2];
			array3[num2 + 4] = array3[num2 + 2];
			array3[num2 + 5] = array3[num2 + 1];
		}
		Mesh mesh = new Mesh();
		mesh.hideFlags = HideFlags.DontSaveInEditor;
		mesh.vertices = array2;
		mesh.triangles = array3;
		mesh.RecalculateNormals();
		Vector3 scale = 2f * Mathf.Max(target.lossyScale.x, target.lossyScale.z) * Vector3.one;
		Gizmos.DrawMesh(mesh, target.position, target.rotation, scale);
	}
}
