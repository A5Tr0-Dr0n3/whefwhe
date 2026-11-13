public class ObjectPlugCord : ObjectBase
{
	public ObjectPlug parentPlug;

	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		if (parentPlug.PlugJointReference != null)
		{
			parentPlug.PlugJointReference.breakForce = 200f;
			parentPlug.PlugJointReference.breakTorque = 200f;
			parentPlug.rb.isKinematic = false;
		}
		parentPlug.SetExtPlugIn(p: true);
	}

	public override void GrabRelease(Manipulator grabber)
	{
		base.GrabRelease(grabber);
		parentPlug.SetExtPlugIn(p: false);
	}
}
