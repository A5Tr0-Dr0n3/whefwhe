using UnityEngine;

public class HRBeetleBallLogic : MonoBehaviour
{
	public ChazController chazBeetle;

	private BeetleBall ballMasterScript;

	[SerializeField]
	private GvrAudioSource turnOnSound;

	[SerializeField]
	private Collider playerCollisionBig;

	private bool hasBeenEnabled;

	private Vector3 nextPos = Vector3.zero;

	private float chargeTime = 5f;

	private float chargeCount;

	private float pitchRandomAmount = 0.4f;

	private float pitchRandomSpeed = 3f;

	private float pitchShiftSpeed = 0.1f;

	private float targetPitch;

	private float currentVolume;

	private float targetVolume;

	private Quaternion ballRot;

	private Quaternion lastBallRot;

	private float ballVel;

	[SerializeField]
	private GvrAudioSource ballSounds;

	[SerializeField]
	private Transform ballTransform;

	private void Start()
	{
		ballMasterScript = GetComponent<BeetleBall>();
	}

	private void Update()
	{
		if (ballMasterScript.enabled)
		{
			if (!hasBeenEnabled)
			{
				turnOnSound.Play();
				playerCollisionBig.enabled = true;
				hasBeenEnabled = true;
			}
			SetNextBallTarget();
		}
	}

	private void FixedUpdate()
	{
		if (ballMasterScript.enabled)
		{
			UpdateSound();
		}
	}

	private void SetNextBallTarget()
	{
		playerCollisionBig.transform.position = EagerMaster.instance.mainCamera.transform.position;
		chargeCount -= Time.deltaTime;
		if (!(chargeCount > 0f))
		{
			chargeCount = chargeTime;
			nextPos = EagerMaster.instance.player.transform.position + Vector3.forward * 5f;
			nextPos += Random.onUnitSphere * 5f;
			nextPos.y = EagerMaster.instance.player.transform.position.y;
			chazBeetle.WalkTo(nextPos);
		}
	}

	private void UpdateSound()
	{
		if (!ballSounds.isPlaying)
		{
			ballSounds.Play();
		}
		ballRot = ballTransform.rotation;
		ballVel = Quaternion.Angle(ballRot, lastBallRot);
		ballVel *= 5f;
		lastBallRot = ballRot;
		ballVel = Mathf.Clamp(ballVel, 0.3f, 1.25f);
		targetVolume = ((!(ballVel > 0.4f)) ? 0f : 1f);
		targetPitch = Mathf.Lerp(ballVel, ballVel * Mathf.PerlinNoise(Time.time * pitchRandomSpeed, 0f), pitchRandomAmount);
		currentVolume = Mathf.Lerp(currentVolume, targetVolume, pitchShiftSpeed);
		currentVolume = Mathf.Clamp(currentVolume, 0.25f, 0.5f);
		ballSounds.volume = currentVolume;
		ballSounds.pitch = Mathf.Lerp(ballSounds.pitch, targetPitch, pitchShiftSpeed);
		targetVolume = 1f;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, nextPos);
	}
}
