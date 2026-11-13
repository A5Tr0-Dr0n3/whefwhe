using System.Collections.Generic;
using TButt;
using UnityEngine;

public class EagerTouch : MonoBehaviour
{
	public enum VacInputStatus
	{
		previewOut,
		Off,
		PreviewIn,
		Out,
		In
	}

	[HideInInspector]
	public Vector2 delta = new Vector2(0f, 0f);

	[SerializeField]
	private ToolGroup _toolGroup;

	private Vector2 _touchpadVelocity = new Vector2(0f, 0f);

	private Vector2 touchPos = new Vector2(0f, 0f);

	private Vector2 startPosition = new Vector2(0f, 0f);

	private float velocityFalloff = 2f;

	private List<Vector3> touchpadVelocityList = new List<Vector3>();

	private Vector3 swipeVelocityRaw = Vector3.zero;

	private TBInput.Button lastVacuumButton;

	public Vector3 GetRayCastPosition()
	{
		switch (TBCore.GetActivePlatform())
		{
		case VRPlatform.OculusMobile:
		case VRPlatform.Daydream:
		case VRPlatform.Cardboard:
			return EagerMaster.instance.mainCamera.transform.position;
		default:
			return _toolGroup.Manipulator.transform.position;
		}
	}

	private void Update()
	{
		if (TBInput.GetButtonDown(TBInput.Button.Action1, _toolGroup.Controller))
		{
			lastVacuumButton = TBInput.Button.Action1;
		}
		else if (TBInput.GetButtonDown(TBInput.Button.Action4, _toolGroup.Controller))
		{
			lastVacuumButton = TBInput.Button.Action4;
		}
		if (TBInput.GetButtonUp(TBInput.Button.Action1, _toolGroup.Controller))
		{
			if (lastVacuumButton == TBInput.Button.Action1)
			{
				lastVacuumButton = TBInput.Button.None;
			}
		}
		else if (TBInput.GetButtonUp(TBInput.Button.Action4, _toolGroup.Controller) && lastVacuumButton == TBInput.Button.Action4)
		{
			lastVacuumButton = TBInput.Button.None;
		}
		if (TBInput.GetTouchDown(TBInput.Button.Action1, _toolGroup.Controller))
		{
			delta = Vector2.zero;
			startPosition = TBInput.GetAxis2D(TBInput.Button.Action1, _toolGroup.Controller);
			touchpadVelocityList.Clear();
		}
		if (TBInput.GetTouch(TBInput.Button.Action1, _toolGroup.Controller))
		{
			touchPos = TBInput.GetAxis2D(TBInput.Button.Action1, _toolGroup.Controller);
			delta = touchPos - startPosition;
			startPosition = touchPos;
			touchpadVelocityList.Add(new Vector3(touchPos.x, touchPos.y, Time.time));
			if (touchpadVelocityList.Count > 6)
			{
				touchpadVelocityList.RemoveAt(0);
			}
			swipeVelocityRaw = ((touchpadVelocityList.Count <= 2) ? (swipeVelocityRaw = Vector3.zero) : (touchpadVelocityList[touchpadVelocityList.Count - 1] - touchpadVelocityList[0]));
			if (swipeVelocityRaw.z > 0f)
			{
				_touchpadVelocity = Vector2.Lerp(_touchpadVelocity, new Vector2(swipeVelocityRaw.x / swipeVelocityRaw.z, swipeVelocityRaw.y / swipeVelocityRaw.z), 0.4f);
			}
		}
		else
		{
			delta = new Vector2(0f, 0f);
			_touchpadVelocity = Vector2.Lerp(_touchpadVelocity, Vector2.zero, velocityFalloff * Time.deltaTime);
		}
	}

	public void ClearVelocity()
	{
		touchpadVelocityList.Clear();
		swipeVelocityRaw = Vector3.zero;
		_touchpadVelocity = Vector2.zero;
	}

	public Vector2 LeashLengthVelocity()
	{
		if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
		{
			return 6f * TBInput.GetAxis2D(TBInput.Button.Joystick, _toolGroup.Controller);
		}
		return _touchpadVelocity;
	}

	public bool GetToolSwitchDetected(bool left = true, Manipulator.manipulatorMode currentMode = Manipulator.manipulatorMode.grab)
	{
		if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
		{
			return TBInput.GetButtonDown(TBInput.Button.Action3, _toolGroup.Controller);
		}
		if (TBInput.GetTouchUp(TBInput.Button.Action1, _toolGroup.Controller))
		{
			if (currentMode == Manipulator.manipulatorMode.vacuum)
			{
				if (Mathf.Abs(_touchpadVelocity.x) > 2f * Mathf.Abs(_touchpadVelocity.y))
				{
					if (!left && _touchpadVelocity.x > 4.2f)
					{
						return true;
					}
					if (left && _touchpadVelocity.x < -4.2f)
					{
						return true;
					}
				}
			}
			else if (Mathf.Abs(_touchpadVelocity.x) > 0.75f * Mathf.Abs(_touchpadVelocity.y))
			{
				if (!left && _touchpadVelocity.x > 4.2f)
				{
					return true;
				}
				if (left && _touchpadVelocity.x < -4.2f)
				{
					return true;
				}
			}
		}
		return false;
	}

	public VacInputStatus CurrentVacInputStatus()
	{
		if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
		{
			if (TBInput.GetButton(TBInput.Button.Action1, _toolGroup.Controller))
			{
				if (lastVacuumButton == TBInput.Button.Action4)
				{
					return VacInputStatus.Out;
				}
				return VacInputStatus.In;
			}
			if (TBInput.GetButton(TBInput.Button.Action4, _toolGroup.Controller))
			{
				if (lastVacuumButton == TBInput.Button.Action1)
				{
					return VacInputStatus.In;
				}
				return VacInputStatus.Out;
			}
			return VacInputStatus.Off;
		}
		if (TBInput.GetTouch(TBInput.Button.Action1, _toolGroup.Controller))
		{
			if (TBInput.GetButton(TBInput.Button.Action1, _toolGroup.Controller))
			{
				return (!(TBInput.GetAxis2D(TBInput.Button.Action1, _toolGroup.Controller).y > 0f)) ? VacInputStatus.In : VacInputStatus.Out;
			}
			return (!(TBInput.GetAxis2D(TBInput.Button.Action1, _toolGroup.Controller).y > 0f)) ? VacInputStatus.PreviewIn : VacInputStatus.previewOut;
		}
		return VacInputStatus.Off;
	}
}
