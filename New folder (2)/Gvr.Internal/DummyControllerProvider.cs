namespace Gvr.Internal;

internal class DummyControllerProvider : IControllerProvider
{
	private ControllerState dummyState = new ControllerState();

	public bool SupportsBatteryStatus => false;

	internal DummyControllerProvider()
	{
	}

	public void ReadState(ControllerState outState)
	{
		outState.CopyFrom(dummyState);
	}

	public void OnPause()
	{
	}

	public void OnResume()
	{
	}
}
