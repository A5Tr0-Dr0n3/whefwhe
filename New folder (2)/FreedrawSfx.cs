using UnityEngine;

public class FreedrawSfx : MonoBehaviour
{
	[SerializeField]
	private GvrAudioSource loopingSource;

	[SerializeField]
	private GvrAudioSource accentSource;

	[SerializeField]
	private AudioClip loopingStrokeAudio;

	[SerializeField]
	private AudioClip[] strokeAccents;

	[SerializeField]
	private float maxLoopVolume = 0.5f;

	[SerializeField]
	private float minAccentPitch = 0.8f;

	[SerializeField]
	private float maxAccentPitch = 1.2f;

	[SerializeField]
	private float minAccentDelta = 0.4f;

	[SerializeField]
	private float angVelSmoothingSpeed = 5f;

	[SerializeField]
	private float maxVolumeAngVel = 180f;

	[SerializeField]
	private float accentDeltaAngle = 10f;

	private Vector3 lastControllerDir;

	private Vector3 lastAccentDir;

	private Vector3 lastAccentDirectionDirection;

	private Vector3 lastAccentPoint;

	private float rawControllerAngVel;

	private float smoothedControllerAngVel;

	private float lastAccentFireTime;

	private bool firstAccentIgnored;

	private void Start()
	{
		loopingSource.volume = 0f;
		loopingSource.clip = loopingStrokeAudio;
		loopingSource.loop = true;
		loopingSource.Play();
	}

	public void StartEndStroke(Transform grabberT)
	{
		rawControllerAngVel = 0f;
		lastControllerDir = grabberT.forward;
		lastAccentDir = lastControllerDir;
		lastAccentDirectionDirection = lastControllerDir;
		firstAccentIgnored = false;
	}

	public void ActivateUndrawSound()
	{
		loopingSource.pitch = -1f;
		rawControllerAngVel = maxVolumeAngVel;
	}

	public void SetSoundsEnabled(bool enabled)
	{
		loopingSource.mute = !enabled;
		accentSource.mute = !enabled;
	}

	private void Update()
	{
		smoothedControllerAngVel = Mathf.Lerp(smoothedControllerAngVel, rawControllerAngVel, Time.deltaTime * angVelSmoothingSpeed);
		loopingSource.volume = maxLoopVolume * Mathf.InverseLerp(0f, maxVolumeAngVel, smoothedControllerAngVel);
	}

	public void Tick(Transform grabberT)
	{
		Vector3 forward = grabberT.forward;
		float num = Vector3.Angle(forward, lastControllerDir);
		rawControllerAngVel = num / Time.deltaTime;
		lastControllerDir = forward;
		Vector3 normalized = (lastControllerDir - lastAccentDir).normalized;
		if (!(Vector3.Angle(lastAccentDir, lastControllerDir) > accentDeltaAngle) || !(Vector3.Dot(normalized, lastAccentDirectionDirection) < 0f))
		{
			return;
		}
		if (smoothedControllerAngVel > maxVolumeAngVel / 8f)
		{
			if (firstAccentIgnored)
			{
				fireAccent();
			}
			else
			{
				firstAccentIgnored = true;
			}
		}
		lastAccentDirectionDirection = (lastControllerDir - lastAccentDir).normalized;
		lastAccentDir = lastControllerDir;
	}

	private void fireAccent()
	{
		if (!accentSource.isPlaying && Time.time - lastAccentFireTime > minAccentDelta)
		{
			accentSource.clip = strokeAccents[Random.Range(0, strokeAccents.Length)];
			accentSource.pitch = Random.Range(minAccentPitch, maxAccentPitch);
			accentSource.Play();
			lastAccentFireTime = Time.time;
		}
	}
}
