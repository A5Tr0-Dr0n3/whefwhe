using System;
using UnityEngine;

namespace Gvr.Internal;

internal class ControllerState
{
	internal GvrConnectionState connectionState;

	internal GvrControllerApiStatus apiStatus = GvrControllerApiStatus.Unavailable;

	internal Quaternion orientation = Quaternion.identity;

	internal Vector3 gyro = Vector3.zero;

	internal Vector3 accel = Vector3.zero;

	internal bool isTouching;

	internal Vector2 touchPos = Vector2.zero;

	internal bool touchDown;

	internal bool touchUp;

	internal bool recentered;

	internal bool clickButtonState;

	internal bool clickButtonDown;

	internal bool clickButtonUp;

	internal bool appButtonState;

	internal bool appButtonDown;

	internal bool appButtonUp;

	internal bool homeButtonDown;

	internal bool homeButtonUp;

	internal bool homeButtonState;

	internal string errorDetails = string.Empty;

	internal IntPtr gvrPtr = IntPtr.Zero;

	internal bool isCharging;

	internal GvrControllerBatteryLevel batteryLevel;

	public void CopyFrom(ControllerState other)
	{
		connectionState = other.connectionState;
		apiStatus = other.apiStatus;
		orientation = other.orientation;
		gyro = other.gyro;
		accel = other.accel;
		isTouching = other.isTouching;
		touchPos = other.touchPos;
		touchDown = other.touchDown;
		touchUp = other.touchUp;
		recentered = other.recentered;
		clickButtonState = other.clickButtonState;
		clickButtonDown = other.clickButtonDown;
		clickButtonUp = other.clickButtonUp;
		appButtonState = other.appButtonState;
		appButtonDown = other.appButtonDown;
		appButtonUp = other.appButtonUp;
		homeButtonDown = other.homeButtonDown;
		homeButtonUp = other.homeButtonUp;
		homeButtonState = other.homeButtonState;
		errorDetails = other.errorDetails;
		gvrPtr = other.gvrPtr;
		isCharging = other.isCharging;
		batteryLevel = other.batteryLevel;
	}

	public void ClearTransientState()
	{
		touchDown = false;
		touchUp = false;
		recentered = false;
		clickButtonDown = false;
		clickButtonUp = false;
		appButtonDown = false;
		appButtonUp = false;
		homeButtonDown = false;
		homeButtonUp = false;
	}
}
