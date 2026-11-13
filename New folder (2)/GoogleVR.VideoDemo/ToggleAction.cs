using UnityEngine;
using UnityEngine.Events;

namespace GoogleVR.VideoDemo;

public class ToggleAction : MonoBehaviour
{
	private float lastUsage;

	private bool on;

	[Tooltip("Event to raise when this is toggled on.")]
	public UnityEvent OnToggleOn;

	[Tooltip("Event to raise when this is toggled off.")]
	public UnityEvent OnToggleOff;

	[Tooltip("Should this initial state be on or off?")]
	public bool InitialState;

	[Tooltip("Should an event be raised for the initial state on Start?")]
	public bool RaiseEventForInitialState;

	[Tooltip("Time required between toggle operations. Operations Toggles within this window will be ignored.")]
	public float Cooldown;

	private void Start()
	{
		on = InitialState;
		if (RaiseEventForInitialState)
		{
			RaiseToggleEvent(on);
		}
	}

	public void Toggle()
	{
		if (!(Time.time - lastUsage < Cooldown))
		{
			lastUsage = Time.time;
			on = !on;
			RaiseToggleEvent(on);
		}
	}

	public void Set(bool on)
	{
		if (this.on != on)
		{
			this.on = on;
			RaiseToggleEvent(on);
		}
	}

	private void RaiseToggleEvent(bool on)
	{
		if (on)
		{
			OnToggleOn.Invoke();
		}
		else
		{
			OnToggleOff.Invoke();
		}
	}
}
