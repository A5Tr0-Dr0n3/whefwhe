public class ObjectReflector : ObjectBase
{
	public ObjectReflector reflectObject;

	private bool controller = true;

	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		SetReflector(master: true);
	}

	private void Update()
	{
		if (controller)
		{
			reflectObject.transform.localPosition = transform.localPosition;
			reflectObject.transform.localRotation = transform.localRotation;
		}
	}

	private void SetReflector(bool master)
	{
		if (master)
		{
			controller = true;
			reflectObject.controller = false;
			rb.isKinematic = false;
			reflectObject.rb.isKinematic = true;
		}
	}
}
