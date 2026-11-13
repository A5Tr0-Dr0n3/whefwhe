using UnityEngine;

public class FlipperLever : ObjectBase
{
	public HingeJoint hinge;

	public JointSpring spring;

	public GvrAudioSource switchSound;

	private FlipperLeverState currentState;

	public ItemDisplaySwapper flipper;

	public ParticleSystem powerSparks;

	private float grabForce;

	private Vector3 manipulatorTarget;

	private float leverDelay;

	private bool introSequence;

	protected override void Awake()
	{
		base.Awake();
		ResetHandle();
	}

	public void ResetHandle()
	{
		spring = hinge.spring;
		spring.targetPosition = 75f;
		hinge.spring = spring;
		currentState = FlipperLeverState.idle;
	}

	public override void GrabUpdate(Manipulator grabber)
	{
		if (grabber != grabbedBy[0])
		{
			return;
		}
		base.GrabUpdate(grabber);
		if (introSequence)
		{
			return;
		}
		manipulatorTarget = Quaternion.identity * grabber.transform.forward * Mathf.Max(2f, grabber.grabTargetDistance) + grabber.transform.position;
		grabForce = manipulatorTarget.z - grabber.grabLocation.z;
		leverDelay -= Time.deltaTime;
		if (!(leverDelay <= 0f))
		{
			return;
		}
		switch (currentState)
		{
		case FlipperLeverState.idle:
			if (grabForce < -1f)
			{
				FlipToSnackZone(right: false);
			}
			break;
		case FlipperLeverState.snack:
			if (grabForce < -1f)
			{
				FlipToRelaxationZone(right: false);
			}
			if (grabForce > 1f)
			{
				FlipToNothing(right: true);
			}
			break;
		case FlipperLeverState.relax:
			if (grabForce < -1f)
			{
				FlipToVrZone(right: false);
			}
			if (grabForce > 1f)
			{
				FlipToSnackZone(right: true);
			}
			break;
		case FlipperLeverState.vr:
			if (grabForce > 1f)
			{
				FlipToRelaxationZone(right: true);
			}
			break;
		}
	}

	public void FlipToSnackZone(bool right)
	{
		spring = hinge.spring;
		leverDelay = flipper.rotationTime + 0.5f;
		spring.targetPosition = 25f;
		currentState = FlipperLeverState.snack;
		powerSparks.Play();
		switchSound.Play();
		if (flipper.currentGroup != 1)
		{
			flipper.FlipToSnackZone(right);
		}
		hinge.spring = spring;
	}

	public void FlipToRelaxationZone(bool right)
	{
		spring = hinge.spring;
		leverDelay = flipper.rotationTime + 0.5f;
		spring.targetPosition = -25f;
		currentState = FlipperLeverState.relax;
		powerSparks.Play();
		switchSound.Play();
		if (flipper.currentGroup != 2)
		{
			flipper.FlipToRelaxationZone(right);
		}
		hinge.spring = spring;
	}

	public void FlipToNothing(bool right)
	{
		spring = hinge.spring;
		leverDelay = flipper.rotationTime + 0.5f;
		spring.targetPosition = 75f;
		currentState = FlipperLeverState.idle;
		powerSparks.Play();
		switchSound.Play();
		if (flipper.currentGroup != 0)
		{
			flipper.FlipToNothing(right);
		}
		hinge.spring = spring;
	}

	public void FlipToVrZone(bool right)
	{
		spring = hinge.spring;
		leverDelay = flipper.rotationTime + 0.5f;
		spring.targetPosition = -75f;
		currentState = FlipperLeverState.vr;
		powerSparks.Play();
		switchSound.Play();
		if (flipper.currentGroup != 3)
		{
			flipper.FlipToVrZone(right: false);
		}
		hinge.spring = spring;
	}

	public void SetIntroSequence(bool s)
	{
		introSequence = s;
		ungrabbable = s;
	}
}
