using System;
using UnityEngine;

namespace Gvr.Internal;

internal class MouseControllerProvider : IControllerProvider
{
	private const string AXIS_MOUSE_X = "Mouse X";

	private const string AXIS_MOUSE_Y = "Mouse Y";

	private ControllerState state = new ControllerState();

	private Vector2 mouseDelta = default;

	private bool wasClickButton;

	private bool wasAppButton;

	private bool wasHomeButton;

	private bool wasTouching;

	private const float ROTATE_SENSITIVITY = 4.5f;

	private const float TOUCH_SENSITIVITY = 0.12f;

	private static readonly Vector3 INVERT_Y = new Vector3(1f, -1f, 1f);

	public static bool IsMouseAvailable => Input.mousePresent && IsActivateButtonPressed;

	public static bool IsActivateButtonPressed => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

	public static bool IsClickButtonPressed => Input.GetMouseButton(0);

	public static bool IsAppButtonPressed => Input.GetMouseButton(1);

	public static bool IsHomeButtonPressed => Input.GetMouseButton(2);

	public static bool IsTouching => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

	public bool SupportsBatteryStatus => false;

	internal MouseControllerProvider()
	{
	}

	public void ReadState(ControllerState outState)
	{
		lock (state)
		{
			UpdateState();
			outState.CopyFrom(state);
		}
		state.ClearTransientState();
	}

	public void OnPause()
	{
	}

	public void OnResume()
	{
	}

	private void UpdateState()
	{
		GvrCursorHelper.ControllerEmulationActive = IsMouseAvailable;
		if (!IsMouseAvailable)
		{
			ClearState();
			return;
		}
		state.connectionState = GvrConnectionState.Connected;
		state.apiStatus = GvrControllerApiStatus.Ok;
		state.isCharging = false;
		state.batteryLevel = GvrControllerBatteryLevel.Full;
		UpdateButtonStates();
		mouseDelta.Set(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		if (state.isTouching)
		{
			UpdateTouchPos();
		}
		else
		{
			UpdateOrientation();
		}
	}

	private void UpdateTouchPos()
	{
		Vector3 mousePosition = Input.mousePosition;
		Vector2 vector = mouseDelta * 0.12f;
		vector.y *= -1f;
		state.touchPos += vector;
		state.touchPos.x = Mathf.Clamp01(state.touchPos.x);
		state.touchPos.y = Mathf.Clamp01(state.touchPos.y);
	}

	private void UpdateOrientation()
	{
		Vector3 vector = Vector3.Scale(mouseDelta, INVERT_Y) * 4.5f;
		state.gyro = vector * ((float)Math.PI / 180f / Time.deltaTime);
		Quaternion quaternion = Quaternion.AngleAxis(vector.x, Vector3.up);
		Quaternion quaternion2 = Quaternion.AngleAxis(vector.y, Vector3.right);
		state.orientation = state.orientation * quaternion * quaternion2;
	}

	private void UpdateButtonStates()
	{
		state.clickButtonState = IsClickButtonPressed;
		state.clickButtonDown = state.clickButtonState && !wasClickButton;
		state.clickButtonUp = !state.clickButtonState && wasClickButton;
		state.isTouching = IsTouching;
		state.touchDown = state.isTouching && !wasTouching;
		state.touchUp = !state.isTouching && wasTouching;
		if (state.touchUp)
		{
			ClearTouchPos();
		}
		state.appButtonState = IsAppButtonPressed;
		state.appButtonDown = state.appButtonState && !wasAppButton;
		state.appButtonUp = !state.appButtonState && wasAppButton;
		state.homeButtonState = IsHomeButtonPressed;
		state.homeButtonDown = state.homeButtonState && !wasHomeButton;
		if (!state.homeButtonState && wasHomeButton)
		{
			Recenter();
		}
		wasClickButton = state.clickButtonState;
		wasTouching = state.isTouching;
		wasAppButton = state.appButtonState;
		wasHomeButton = state.homeButtonState;
	}

	private void Recenter()
	{
		Quaternion quaternion = Quaternion.AngleAxis(0f - state.orientation.eulerAngles.y, Vector3.up);
		state.orientation *= quaternion;
		state.recentered = true;
	}

	private void ClearTouchPos()
	{
		state.touchPos = new Vector2(0.5f, 0.5f);
	}

	private void ClearState()
	{
		state.connectionState = GvrConnectionState.Disconnected;
		state.clickButtonState = false;
		state.clickButtonDown = false;
		state.clickButtonUp = false;
		state.appButtonState = false;
		state.appButtonDown = false;
		state.appButtonUp = false;
		ClearTouchPos();
	}
}
