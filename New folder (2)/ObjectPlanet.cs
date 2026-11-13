public class ObjectPlanet : ObjectBase
{
	private float oldDrag;

	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		oldDrag = rb.drag;
		rb.drag = 0.5f;
	}

	public override void GrabRelease(Manipulator grabber)
	{
		base.GrabRelease(grabber);
		rb.drag = oldDrag;
	}
}
