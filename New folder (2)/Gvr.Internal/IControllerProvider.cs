namespace Gvr.Internal;

internal interface IControllerProvider
{
	bool SupportsBatteryStatus { get; }

	void OnPause();

	void OnResume();

	void ReadState(ControllerState outState);
}
