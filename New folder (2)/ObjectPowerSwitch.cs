using UnityEngine;

public class ObjectPowerSwitch : ObjectBase
{
	private enum leverType
	{
		horizontal,
		vertical
	}

	public enum leverState
	{
		on,
		idle,
		off
	}

	public HingeJoint hinge;

	private GvrAudioSource switchSound;

	public leverState currentLeverState = leverState.idle;

	[SerializeField]
	private leverType mLeverType = leverType.vertical;

	private ParticleSystem powerSparks;

	private float grabForce;

	private float leverDelay;

	private Vector3 manipulatorTarget;

	public JointSpring jointSpring;

	protected override void Awake()
	{
		base.Awake();
		hinge = GetComponent<HingeJoint>();
		switchSound = GetComponent<GvrAudioSource>();
		powerSparks = GetComponentInChildren<ParticleSystem>();
		ResetHandle();
	}

	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		ungrabbable = true;
	}

	public void ResetHandle()
	{
		jointSpring = hinge.spring;
		jointSpring.targetPosition = 0f;
		hinge.spring = jointSpring;
		currentLeverState = leverState.idle;
	}

	public override void GrabUpdate(Manipulator grabber)
	{
		if (grabber != grabbedBy[0])
		{
			return;
		}
		base.GrabUpdate(grabber);
		manipulatorTarget = Quaternion.identity * grabber.transform.forward * Mathf.Max(2f, grabber.grabTargetDistance) + grabber.transform.position;
		if (mLeverType == leverType.horizontal)
		{
			grabForce = manipulatorTarget.z - grabber.grabLocation.z;
		}
		else
		{
			grabForce = manipulatorTarget.y - grabber.grabLocation.y;
		}
		leverDelay -= Time.deltaTime;
		jointSpring = hinge.spring;
		if (!(leverDelay <= 0f))
		{
			return;
		}
		switch (currentLeverState)
		{
		case leverState.on:
			if (grabForce < -1f)
			{
				leverDelay = 0.5f;
				jointSpring.targetPosition = 0f;
				currentLeverState = leverState.idle;
				switchSound.Play();
				if (powerSparks != null)
				{
					powerSparks.Play();
				}
			}
			break;
		case leverState.idle:
			if (grabForce < -1f)
			{
				leverDelay = 0.5f;
				jointSpring.targetPosition = 90f;
				currentLeverState = leverState.off;
				switchSound.Play();
				if (powerSparks != null)
				{
					powerSparks.Play();
				}
			}
			else if (grabForce > 1f)
			{
				leverDelay = 0.5f;
				jointSpring.targetPosition = -90f;
				currentLeverState = leverState.on;
				switchSound.Play();
				if (powerSparks != null)
				{
					powerSparks.Play();
				}
			}
			break;
		case leverState.off:
			if (grabForce > 1f)
			{
				leverDelay = 0.5f;
				jointSpring.targetPosition = 0f;
				currentLeverState = leverState.idle;
				switchSound.Play();
				if (powerSparks != null)
				{
					powerSparks.Play();
				}
			}
			break;
		}
		hinge.spring = jointSpring;
	}

	public override void GrabRelease(Manipulator grabber)
	{
		base.GrabRelease(grabber);
		ungrabbable = false;
	}
}
