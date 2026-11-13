using UnityEngine;

public class ObjectBreakJoint : ObjectBase
{
	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		if ((bool)GetComponent<FixedJoint>())
		{
			Object.Destroy(GetComponent<FixedJoint>());
		}
	}
}
