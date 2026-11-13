using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TButt.InputMaps;

public abstract class TBSDKInputBase<T>
{
	protected TBInput.Controller _activeController;

	protected bool _usesXInputForGamepads;

	protected TBInput.ButtonLookupTable<T> emptyLookupTable;

	protected TBControllerBase<T> controller_LHand;

	protected TBControllerBase<T> controller_RHand;

	protected TBControllerBase<T> controller_3DOF;

	protected TBControllerBase<T> controller_Gamepad;

	protected TBControllerBase<T> controller_ClickRemote;

	protected Coroutine leftRumbleRoutine;

	protected Coroutine rightRumbleRoutine;

	public virtual void Initialize()
	{
		emptyLookupTable = default;
		_activeController = GetActiveController();
		if (TBSettings.GetControlSettings().supportsHandControllers)
		{
			LoadHandControllers();
		}
		if (TBSettings.GetControlSettings().supports3DOFControllers)
		{
			Load3DOFControllers();
		}
		if (TBSettings.GetControlSettings().supportsClickRemote)
		{
			LoadClickRemotes();
		}
		if (TBSettings.GetControlSettings().supportsGamepad)
		{
			LoadGamepads();
		}
	}

	protected virtual void LoadHandControllers()
	{
	}

	protected virtual void Load3DOFControllers()
	{
	}

	protected virtual void LoadClickRemotes()
	{
	}

	protected virtual void LoadGamepads()
	{
	}

	public virtual HandTrackingOffsets GetControllerTrackingOffsets(TBInput.Controller controller)
	{
		if (controller == TBInput.Controller.Active)
		{
			controller = GetActiveController();
		}
		return controller switch
		{
			TBInput.Controller.LHandController => controller_LHand.GetTrackingOffsets(), 
			TBInput.Controller.RHandController => controller_RHand.GetTrackingOffsets(), 
			TBInput.Controller.Mobile3DOFController => controller_3DOF.GetTrackingOffsets(), 
			TBInput.Controller.ClickRemote => controller_ClickRemote.GetTrackingOffsets(), 
			TBInput.Controller.Gamepad => controller_Gamepad.GetTrackingOffsets(), 
			_ => default, 
		};
	}

	public virtual void Update()
	{
	}

	public virtual void FixedUpdate()
	{
	}

	public virtual TBInput.Controller GetActiveController()
	{
		return TBInput.GetActiveControlType() switch
		{
			TBInput.ControlType.ClickRemote => TBInput.Controller.ClickRemote, 
			TBInput.ControlType.Gamepad => TBInput.Controller.Gamepad, 
			TBInput.ControlType.HandControllers => TBInput.Controller.RHandController, 
			TBInput.ControlType.Mobile3DOFController => TBInput.Controller.Mobile3DOFController, 
			_ => TBInput.Controller.None, 
		};
	}

	public virtual string GetControllerName(TBInput.Controller controller)
	{
		if (controller == TBInput.Controller.Active)
		{
			controller = GetActiveController();
		}
		if (GetControllerForType(controller) != null)
		{
			return GetControllerForType(controller).GetName();
		}
		return "Undefined";
	}

	public virtual bool ControllerHasRumbleSupport(TBInput.Controller controller)
	{
		if (GetControllerForType(controller) != null)
		{
			return GetControllerForType(controller).HasRumbleSupport();
		}
		return false;
	}

	public static List<TBInput.ButtonDef<T>> LoadMappingsForController(TBControllerBase<T> controller)
	{
		return TBInput.LoadButtonDefs(controller.GetDefaultDefs(), controller.GetFileName());
	}

	protected TBControllerBase<T> GetControllerForType(TBInput.Controller controller)
	{
		return controller switch
		{
			TBInput.Controller.LHandController => controller_LHand, 
			TBInput.Controller.RHandController => controller_RHand, 
			TBInput.Controller.Mobile3DOFController => controller_3DOF, 
			TBInput.Controller.ClickRemote => controller_ClickRemote, 
			TBInput.Controller.Gamepad => controller_Gamepad, 
			_ => null, 
		};
	}

	protected TBInput.ButtonLookupTable<T> GetButtonLookupTableForController(TBInput.Controller controller)
	{
		if (GetControllerForType(controller) != null)
		{
			return GetControllerForType(controller).GetLookupTable();
		}
		return emptyLookupTable;
	}

	public virtual T[] GetButtonArray(TBInput.Button button, ref TBInput.Controller controller)
	{
		if (controller == TBInput.Controller.Active)
		{
			controller = TBInput.GetActiveController();
		}
		return TBInput.GetRawButtons(button, GetButtonLookupTableForController(controller));
	}

	public virtual TBInput.Mobile3DOFHandedness Get3DOFHandedness()
	{
		return TBInput.Mobile3DOFHandedness.Center;
	}

	public virtual float GetPointerTiltAngle(TBInput.Controller controller)
	{
		if (controller == TBInput.Controller.Active)
		{
			controller = TBInput.GetActiveController();
		}
		return GetControllerForType(controller)?.PointerTiltAngle ?? 0f;
	}

	public virtual bool GetButtonDown(TBInput.Button button, TBInput.Controller controller)
	{
		bool result = false;
		T[] buttonArray = GetButtonArray(button, ref controller);
		if (buttonArray == null)
		{
			return false;
		}
		for (int i = 0; i < buttonArray.Length; i++)
		{
			if (ResolveButtonDown(buttonArray[i], controller))
			{
				result = true;
			}
		}
		return result;
	}

	public virtual bool GetButtonUp(TBInput.Button button, TBInput.Controller controller)
	{
		bool result = false;
		T[] buttonArray = GetButtonArray(button, ref controller);
		if (buttonArray == null)
		{
			return false;
		}
		for (int i = 0; i < buttonArray.Length; i++)
		{
			if (ResolveButtonUp(buttonArray[i], controller))
			{
				result = true;
			}
		}
		return result;
	}

	public virtual bool GetButton(TBInput.Button button, TBInput.Controller controller)
	{
		bool result = false;
		T[] buttonArray = GetButtonArray(button, ref controller);
		if (buttonArray == null)
		{
			return false;
		}
		for (int i = 0; i < buttonArray.Length; i++)
		{
			if (ResolveButton(buttonArray[i], controller))
			{
				result = true;
			}
		}
		return result;
	}

	public virtual bool GetTouchDown(TBInput.Button button, TBInput.Controller controller)
	{
		bool result = false;
		T[] buttonArray = GetButtonArray(button, ref controller);
		if (buttonArray == null)
		{
			return false;
		}
		for (int i = 0; i < buttonArray.Length; i++)
		{
			if (ResolveTouchDown(buttonArray[i], controller))
			{
				result = true;
			}
		}
		return result;
	}

	public virtual bool GetTouchUp(TBInput.Button button, TBInput.Controller controller)
	{
		bool result = false;
		T[] buttonArray = GetButtonArray(button, ref controller);
		if (buttonArray == null)
		{
			return false;
		}
		for (int i = 0; i < buttonArray.Length; i++)
		{
			if (ResolveTouchUp(buttonArray[i], controller))
			{
				result = true;
			}
		}
		return result;
	}

	public virtual bool GetTouch(TBInput.Button button, TBInput.Controller controller)
	{
		bool result = false;
		T[] buttonArray = GetButtonArray(button, ref controller);
		if (buttonArray == null)
		{
			return false;
		}
		for (int i = 0; i < buttonArray.Length; i++)
		{
			if (ResolveTouch(buttonArray[i], controller))
			{
				result = true;
			}
		}
		return result;
	}

	public virtual float GetAxis1D(TBInput.Button button, TBInput.Controller controller)
	{
		float num = 0f;
		T[] buttonArray = GetButtonArray(button, ref controller);
		if (buttonArray == null)
		{
			return num;
		}
		for (int i = 0; i < buttonArray.Length; i++)
		{
			float num2 = ResolveAxis1D(buttonArray[i], controller);
			if (num2 > num)
			{
				num = num2;
			}
		}
		return num;
	}

	public virtual Vector2 GetAxis2D(TBInput.Button button, TBInput.Controller controller)
	{
		Vector2 result = Vector2.zero;
		T[] buttonArray = GetButtonArray(button, ref controller);
		if (buttonArray == null)
		{
			return result;
		}
		for (int i = 0; i < buttonArray.Length; i++)
		{
			Vector2 vector = ResolveAxis2D(buttonArray[i], controller);
			if (!(vector == Vector2.zero) && vector.sqrMagnitude > result.sqrMagnitude)
			{
				result = vector;
			}
		}
		return result;
	}

	public virtual Vector3 GetRawPosition(TBInput.Controller controller)
	{
		return Vector3.zero;
	}

	public virtual Quaternion GetRawRotation(TBInput.Controller controller)
	{
		return Quaternion.identity;
	}

	public virtual Vector3 GetVelocity(TBInput.Controller controller)
	{
		return Vector3.zero;
	}

	public virtual Vector3 GetAngularVelocity(TBInput.Controller controller)
	{
		return Vector3.zero;
	}

	public virtual Vector3 GetAcceleration(TBInput.Controller controller)
	{
		return Vector3.zero;
	}

	public virtual bool SetRumble(TBInput.Controller controller, float strength)
	{
		return false;
	}

	public virtual bool StartRumbleClip(TBInput.Controller controller, AudioClip clip)
	{
		return false;
	}

	public virtual void StartRumblePulse(TBInput.Controller controller, float strength, float length)
	{
		if (controller == TBInput.Controller.Active)
		{
			controller = GetActiveController();
		}
		if (!SetRumble(controller, strength))
		{
			return;
		}
		switch (controller)
		{
		case TBInput.Controller.LHandController:
			if (leftRumbleRoutine != null)
			{
				TBCore.instance.StopCoroutine(leftRumbleRoutine);
			}
			leftRumbleRoutine = TBCore.instance.StartCoroutine(RumblePulseRoutine(controller, strength, length));
			break;
		case TBInput.Controller.RHandController:
			if (rightRumbleRoutine != null)
			{
				TBCore.instance.StopCoroutine(rightRumbleRoutine);
			}
			rightRumbleRoutine = TBCore.instance.StartCoroutine(RumblePulseRoutine(controller, strength, length));
			break;
		case TBInput.Controller.ClickRemote:
		case TBInput.Controller.Mobile3DOFController:
		case TBInput.Controller.Gamepad:
			if (leftRumbleRoutine != null)
			{
				TBCore.instance.StopCoroutine(leftRumbleRoutine);
			}
			leftRumbleRoutine = TBCore.instance.StartCoroutine(RumblePulseRoutine(controller, strength, length));
			break;
		}
	}

	protected virtual IEnumerator RumblePulseRoutine(TBInput.Controller controller, float strength, float length)
	{
		yield return null;
		float t = 0f;
		while (t < length)
		{
			SetRumble(controller, strength);
			t += Time.unscaledDeltaTime;
			yield return null;
		}
	}

	public virtual bool ResolveButtonDown(T button, TBInput.Controller controller)
	{
		return false;
	}

	public virtual bool ResolveButtonUp(T button, TBInput.Controller controller)
	{
		return false;
	}

	public virtual bool ResolveButton(T button, TBInput.Controller controller)
	{
		return false;
	}

	public virtual bool ResolveTouchDown(T button, TBInput.Controller controller)
	{
		return false;
	}

	public virtual bool ResolveTouchUp(T button, TBInput.Controller controller)
	{
		return false;
	}

	public virtual bool ResolveTouch(T button, TBInput.Controller controller)
	{
		return false;
	}

	public virtual float ResolveAxis1D(T button, TBInput.Controller controller)
	{
		return 0f;
	}

	public virtual Vector2 ResolveAxis2D(T button, TBInput.Controller controller)
	{
		return Vector2.zero;
	}

	public virtual Vector3 ResolveRawPosition(TBInput.Controller controller)
	{
		return Vector3.zero;
	}

	public virtual Quaternion ResolveRawRotation(TBInput.Controller controller)
	{
		return Quaternion.identity;
	}

	protected virtual void SetActiveController(TBInput.Controller controller)
	{
		if (_activeController != controller)
		{
			_activeController = controller;
		}
	}

	protected virtual bool SupportsInputType(TBInput.Button button, TBInput.Controller controller)
	{
		T[] buttonArray = GetButtonArray(button, ref controller);
		if (buttonArray == null)
		{
			return false;
		}
		return true;
	}

	public virtual bool SupportsButton(TBInput.Button button, TBInput.Controller controller)
	{
		T[] buttonArray = GetButtonArray(button, ref controller);
		if (buttonArray == null)
		{
			return false;
		}
		bool result = false;
		for (int i = 0; i < buttonArray.Length; i++)
		{
			if (SupportsRawButton(buttonArray[i], controller))
			{
				result = true;
			}
		}
		return result;
	}

	public virtual bool SupportsRawButton(T button, TBInput.Controller controller)
	{
		TBControllerBase<T> controllerForType = GetControllerForType(controller);
		for (int i = 0; i < controllerForType.GetLoadedButtonDefs().Count; i++)
		{
			if (EqualityComparer<T>.Default.Equals(button, controllerForType.GetLoadedButtonDefs()[i].rawButton))
			{
				return controllerForType.GetLoadedButtonDefs()[i].supportsButton;
			}
		}
		return false;
	}

	public virtual bool SupportsTouch(TBInput.Button button, TBInput.Controller controller)
	{
		T[] buttonArray = GetButtonArray(button, ref controller);
		if (buttonArray == null)
		{
			return false;
		}
		bool result = false;
		for (int i = 0; i < buttonArray.Length; i++)
		{
			if (SupportsRawTouch(buttonArray[i], controller))
			{
				result = true;
			}
		}
		return result;
	}

	public virtual bool SupportsRawTouch(T button, TBInput.Controller controller)
	{
		TBControllerBase<T> controllerForType = GetControllerForType(controller);
		for (int i = 0; i < controllerForType.GetLoadedButtonDefs().Count; i++)
		{
			if (EqualityComparer<T>.Default.Equals(button, controllerForType.GetLoadedButtonDefs()[i].rawButton))
			{
				return controllerForType.GetLoadedButtonDefs()[i].supportsTouch;
			}
		}
		return false;
	}

	public virtual bool SupportsAxis1D(TBInput.Button button, TBInput.Controller controller)
	{
		T[] buttonArray = GetButtonArray(button, ref controller);
		if (buttonArray == null)
		{
			return false;
		}
		bool result = false;
		for (int i = 0; i < buttonArray.Length; i++)
		{
			if (SupportsRawAxis1D(buttonArray[i], controller))
			{
				result = true;
			}
		}
		return result;
	}

	public virtual bool SupportsRawAxis1D(T button, TBInput.Controller controller)
	{
		TBControllerBase<T> controllerForType = GetControllerForType(controller);
		for (int i = 0; i < controllerForType.GetLoadedButtonDefs().Count; i++)
		{
			if (EqualityComparer<T>.Default.Equals(button, controllerForType.GetLoadedButtonDefs()[i].rawButton))
			{
				return controllerForType.GetLoadedButtonDefs()[i].supportsAxis1D;
			}
		}
		return false;
	}

	public virtual bool SupportsAxis2D(TBInput.Button button, TBInput.Controller controller)
	{
		T[] buttonArray = GetButtonArray(button, ref controller);
		if (buttonArray == null)
		{
			return false;
		}
		bool result = false;
		for (int i = 0; i < buttonArray.Length; i++)
		{
			if (SupportsRawAxis2D(buttonArray[i], controller))
			{
				result = true;
			}
		}
		return result;
	}

	public virtual bool SupportsRawAxis2D(T button, TBInput.Controller controller)
	{
		TBControllerBase<T> controllerForType = GetControllerForType(controller);
		for (int i = 0; i < controllerForType.GetLoadedButtonDefs().Count; i++)
		{
			if (EqualityComparer<T>.Default.Equals(button, controllerForType.GetLoadedButtonDefs()[i].rawButton))
			{
				return controllerForType.GetLoadedButtonDefs()[i].supportsAxis2D;
			}
		}
		return false;
	}
}
