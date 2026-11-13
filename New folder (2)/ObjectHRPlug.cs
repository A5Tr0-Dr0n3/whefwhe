using UnityEngine;

public class ObjectHRPlug : ObjectBase
{
	private enum PlugType
	{
		up,
		down
	}

	private enum PlugState
	{
		closed,
		open
	}

	[SerializeField]
	private PlugType mPlugType;

	private float grabForce;

	private Vector3 manipulatorTarget;

	private JointSpring mJointSpring;

	private HingeJoint mHinge;

	protected override void Awake()
	{
		base.Awake();
		mHinge = GetComponent<HingeJoint>();
	}

	private void Update()
	{
	}

	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		ungrabbable = true;
	}

	public override void GrabRelease(Manipulator grabber)
	{
		base.GrabRelease(grabber);
		ungrabbable = false;
	}

	public override void GrabUpdate(Manipulator grabber)
	{
		if (grabber != grabbedBy[0])
		{
			return;
		}
		base.GrabUpdate(grabber);
		manipulatorTarget = Quaternion.identity * grabber.transform.forward * Mathf.Max(2f, grabber.grabTargetDistance) + grabber.transform.position;
		grabForce = manipulatorTarget.y - grabber.grabLocation.y;
		mJointSpring = mHinge.spring;
		if (Mathf.Abs(grabForce) > 1f)
		{
			if (mPlugType == PlugType.up)
			{
				mJointSpring.targetPosition = 90f;
			}
			else
			{
				mJointSpring.targetPosition = -90f;
			}
		}
		mHinge.spring = mJointSpring;
	}
}
