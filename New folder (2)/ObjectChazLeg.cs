using UnityEngine;
using UnityEngine.Events;

public class ObjectChazLeg : ObjectBase
{
	public ChazController chaz;

	private SpringJoint chazSpring;

	public ChazController.chazMode grabBehavior;

	public bool makeSteppingNoise;

	public bool weakSpringCollisions = true;

	public bool powerDownCollisions;

	public GvrAudioSource powerDownNoise;

	public bool angryCollisionEye = true;

	[SerializeField]
	public UnityEvent onImpactEvent;

	private Rigidbody tempRB;

	private float deadTime;

	private float stepVolume;

	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		chaz.SetChazMode(ToolManager.CurrentChazGrabMode());
		chaz.EyeLookAt(grabber.GrabbingTransform, 1.5f, ChazController.eyeColor.Angry, immediately: false);
	}

	public override void GrabRelease(Manipulator grabber)
	{
		base.GrabRelease(grabber);
		chaz.SetChazMode(ToolManager.CurrentChazGrabMode());
		chaz.GoLimpDrag();
		if (chaz.poweredUp)
		{
			chaz.PowerUp(pitch: false, 1f);
		}
	}

	protected override void OnCollisionEnter(Collision collision)
	{
		if (grabBehavior == ChazController.chazMode.leftLegGrab || grabBehavior == ChazController.chazMode.rightLegGrab)
		{
			if (!makeSteppingNoise)
			{
				return;
			}
			stepVolume = Mathf.Clamp01(collision.relativeVelocity.magnitude / 5f);
			if (grabBehavior == ChazController.chazMode.leftLegGrab)
			{
				chaz.leftStepSFX.randVolume = stepVolume;
				chaz.leftStepSFX.Play();
			}
			else
			{
				chaz.rightStepSFX.randVolume = stepVolume;
				chaz.rightStepSFX.Play();
			}
			chaz.bodyStepSFX.randVolume = stepVolume;
			chaz.bodyStepSFX.Play();
		}
		if (!collision.rigidbody)
		{
			return;
		}
		deadTime = collision.relativeVelocity.magnitude * collision.rigidbody.mass / 25f;
		if (weakSpringCollisions)
		{
			if (deadTime > 0.1f)
			{
				chaz.GoLimpDrag();
				chaz.GoLimpSpring();
				if (chaz.poweredUp)
				{
					chaz.PowerUp(pitch: false, Mathf.Clamp(deadTime, 0.1f, 2f));
				}
			}
			if (deadTime > 0.25f)
			{
				if (angryCollisionEye)
				{
					chaz.EyeLookAt(EagerMaster.instance.mainCamera.transform, 0.5f, ChazController.eyeColor.Angry, immediately: false);
				}
				if (onImpactEvent != null && (ToolManager.PlayerGrabbingObjectBase(this) || ToolManager.PlayerGrabbingGameObject(collision.gameObject)))
				{
					onImpactEvent.Invoke();
				}
			}
		}
		if (powerDownCollisions && deadTime > 0.25f)
		{
			if (powerDownNoise != null && chaz.poweredUp)
			{
				powerDownNoise.Play();
			}
			chaz.PowerDown(pitch: false, Random.Range(3.4f, 6f));
		}
	}

	private void OnJointBreak(float breakForce)
	{
		chaz.JointBroke();
	}
}
