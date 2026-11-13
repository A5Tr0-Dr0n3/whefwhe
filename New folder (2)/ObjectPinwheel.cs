using UnityEngine;

public class ObjectPinwheel : ObjectBase
{
	[SerializeField]
	private RotateObject rotator;

	[SerializeField]
	private AudioSourceLoudness asl;

	private Vector3 lookSwivel;

	private Vector3 rotationSpeed;

	private Vector3 targetPosition;

	[HideInInspector]
	public Transform lookTarget;

	public bool smoothLook = true;

	[HideInInspector]
	public float rbVelMag;

	protected override void Awake()
	{
		base.Awake();
		if (lookTarget == null && EagerMaster.instance != null)
		{
			lookTarget = EagerMaster.instance.mainCamera.transform;
		}
	}

	private void Update()
	{
		if (lookTarget == null)
		{
			lookTarget = EagerMaster.instance.mainCamera.transform;
		}
		if (!isGrabbed)
		{
			rotator.rotation.z = 5000f * asl.lerpLoudness;
		}
		if (smoothLook)
		{
			targetPosition = Vector3.Lerp(targetPosition, lookTarget.position, 0.3f * Time.deltaTime);
			targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
		}
		else
		{
			targetPosition = new Vector3(lookTarget.position.x, transform.position.y, lookTarget.position.z);
		}
		transform.LookAt(targetPosition);
	}

	public override void GrabUpdate(Manipulator grabber)
	{
		if (!(grabber != grabbedBy[0]))
		{
			base.GrabUpdate(grabber);
			rbVelMag = 0.99f * rbVelMag + 0.01f * rb.velocity.sqrMagnitude;
			rotator.rotation.z = Mathf.Clamp(50f * rbVelMag, 0f, 1200f);
		}
	}

	public override void GrabWith(Manipulator grabber)
	{
		ForceDropByPlayer(playSFX: false);
		base.GrabWith(grabber);
		grabber.orientDuringGrab = true;
		grabber.heldHighVector = new Vector3(0f, 0.1f, 0f);
	}

	public override void GrabRelease(Manipulator grabber)
	{
		rotator.rotation = Vector3.zero;
		base.GrabRelease(grabber);
	}

	public override void OnHoverEnter()
	{
		base.OnHoverEnter();
		if ((bool)JobGarden.instance)
		{
			JobGarden.instance.hasHoveredOver = true;
		}
	}
}
