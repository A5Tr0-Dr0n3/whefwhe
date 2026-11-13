using UnityEngine;

public class ObjectFixedJoint : ObjectBase
{
	public AudioClip jointSnap;

	public override void GrabUpdate(Manipulator grabber)
	{
		if (!(grabber != grabbedBy[0]))
		{
			base.GrabUpdate(grabber);
			if (!isSatisfied && !GetComponent<FixedJoint>())
			{
				AudioSource.PlayClipAtPoint(jointSnap, transform.position);
				Satisfaction();
			}
		}
	}
}
