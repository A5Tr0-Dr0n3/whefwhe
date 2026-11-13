public class ObjectKinematic : ObjectBase
{
	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		if ((bool)rb)
		{
			rb.isKinematic = false;
		}
	}
}
