using System;
using UnityEngine;

[Obsolete("Replaced by GvrControllerInput.")]
[AddComponentMenu("")]
public class GvrController : GvrControllerInput
{
	public new static GvrConnectionState State => GvrControllerInput.State;

	public new static GvrControllerApiStatus ApiStatus => GvrControllerInput.ApiStatus;

	public new static Quaternion Orientation => GvrControllerInput.Orientation;

	public new static Vector3 Gyro => GvrControllerInput.Gyro;

	public new static Vector3 Accel => GvrControllerInput.Accel;

	public new static bool IsTouching => GvrControllerInput.IsTouching;

	public new static bool TouchDown => GvrControllerInput.TouchDown;

	public new static bool TouchUp => GvrControllerInput.TouchUp;

	public new static Vector2 TouchPos => GvrControllerInput.TouchPos;

	public new static bool Recentered => GvrControllerInput.Recentered;

	public new static bool ClickButton => GvrControllerInput.ClickButton;

	public new static bool ClickButtonDown => GvrControllerInput.ClickButtonDown;

	public new static bool ClickButtonUp => GvrControllerInput.ClickButtonUp;

	public new static bool AppButton => GvrControllerInput.AppButton;

	public new static bool AppButtonDown => GvrControllerInput.AppButtonDown;

	public new static bool AppButtonUp => GvrControllerInput.AppButtonUp;

	public new static bool HomeButtonDown => GvrControllerInput.HomeButtonDown;

	public new static bool HomeButtonState => GvrControllerInput.HomeButtonState;

	public new static string ErrorDetails => GvrControllerInput.ErrorDetails;

	public new static IntPtr StatePtr => GvrControllerInput.StatePtr;

	public new static bool IsCharging => GvrControllerInput.IsCharging;

	public new static GvrControllerBatteryLevel BatteryLevel => GvrControllerInput.BatteryLevel;
}
