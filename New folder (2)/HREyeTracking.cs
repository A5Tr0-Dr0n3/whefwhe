using UnityEngine;

public class HREyeTracking : MonoBehaviour
{
	public bool lookAtBee;

	public float lookMoveSpeed = 5f;

	public float driftMoveSpeed = 0.1f;

	public float maxAngleDiff = 120f;

	private Vector3 lookV3;

	private Quaternion lookRotation;

	private bool isTurning;

	[HideInInspector]
	public bool fastTurning;

	[HideInInspector]
	private bool isPanicLooking;

	[HideInInspector]
	private bool customLook;

	private float randomLook;

	private Quaternion originalRot;

	private Quaternion randomRot;

	private float maxTimer = 1.5f;

	private float minTimer = 0.5f;

	private float timer;

	private float maxPanicTimer = 0.75f;

	private float minPanicTimer = 0.4f;

	private float panicTimer;

	private float maxPanicTimerStart = 0.75f;

	private float minPanicTimerStart = 0.4f;

	private void Update()
	{
		if (isPanicLooking)
		{
			PanicLooking();
		}
		else if (customLook)
		{
			LookAtButtFlap();
		}
		else if (lookAtBee)
		{
			LookAtBee();
		}
		else
		{
			transform.Rotate(Vector3.up, 4f * Time.deltaTime);
		}
	}

	public void LookAtButtFlap()
	{
		if (timer < 0f)
		{
			timer = Random.Range(minTimer, maxTimer);
			randomLook = Random.Range(-25f, 25f);
		}
		timer -= Time.deltaTime;
		randomRot = Quaternion.Euler(0f, originalRot.eulerAngles.y + randomLook, 0f);
		transform.localRotation = Quaternion.Lerp(transform.localRotation, randomRot, lookMoveSpeed * 20f * Time.deltaTime);
	}

	public void StartLookingAtButtFlap()
	{
		lookV3 = new Vector3(EagerMaster.instance.mainCamera.transform.position.x, transform.position.y, EagerMaster.instance.mainCamera.transform.position.z);
		randomLook = 0f;
		customLook = true;
		isPanicLooking = false;
		fastTurning = false;
		originalRot = Quaternion.LookRotation(lookV3);
		originalRot = Quaternion.Euler(0f, originalRot.eulerAngles.y, 0f);
	}

	private void LookAtBee()
	{
		lookV3 = new Vector3(EagerMaster.instance.mainCamera.transform.position.x, transform.position.y, EagerMaster.instance.mainCamera.transform.position.z);
		lookRotation = Quaternion.LookRotation(lookV3);
		lookRotation = Quaternion.Euler(0f, lookRotation.eulerAngles.y + 180f, 0f);
		if (fastTurning)
		{
			isTurning = true;
			transform.localRotation = Quaternion.Lerp(transform.localRotation, lookRotation, lookMoveSpeed * 15f * Time.deltaTime);
		}
		if (isTurning || Quaternion.Angle(lookRotation, transform.localRotation) > maxAngleDiff)
		{
			isTurning = true;
			if (Quaternion.Angle(lookRotation, transform.localRotation) < 5f)
			{
				isTurning = false;
				fastTurning = false;
			}
			if (!fastTurning)
			{
				transform.localRotation = Quaternion.Lerp(transform.localRotation, lookRotation, lookMoveSpeed * Time.deltaTime);
			}
		}
		else
		{
			transform.localRotation = Quaternion.Lerp(transform.localRotation, lookRotation, driftMoveSpeed * Time.deltaTime);
		}
	}

	public void StartLookingAtBee()
	{
		lookAtBee = true;
		isTurning = true;
		fastTurning = true;
		customLook = false;
		isPanicLooking = false;
	}

	public void StartPanicLooking()
	{
		lookV3 = new Vector3(EagerMaster.instance.mainCamera.transform.position.x, transform.position.y, EagerMaster.instance.mainCamera.transform.position.z);
		randomLook = 0f;
		isPanicLooking = true;
		customLook = false;
		lookAtBee = false;
		fastTurning = false;
		minPanicTimer = minPanicTimerStart;
		maxPanicTimer = maxPanicTimerStart;
		originalRot = Quaternion.LookRotation(lookV3);
		originalRot = Quaternion.Euler(0f, originalRot.eulerAngles.y + 180f, 0f);
	}

	private void PanicLooking()
	{
		if (panicTimer < 0f)
		{
			panicTimer = Random.Range(minPanicTimer, maxPanicTimer);
			randomLook = Random.Range(-15f, 15f);
			maxPanicTimer = Mathf.Clamp(maxPanicTimer - 0.025f, minPanicTimer, float.PositiveInfinity);
			minPanicTimer = Mathf.Clamp(minPanicTimer - 0.025f, 0.2f, float.PositiveInfinity);
		}
		panicTimer -= Time.deltaTime;
		randomRot = Quaternion.Euler(0f, originalRot.eulerAngles.y + randomLook, 0f);
		transform.localRotation = Quaternion.Lerp(transform.localRotation, randomRot, lookMoveSpeed * 30f * Time.deltaTime);
	}
}
