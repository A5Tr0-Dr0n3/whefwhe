using System;
using System.Collections.Generic;
using TButt;
using UnityEngine;
using UnityEngine.XR;

public class ToolManager : MonoBehaviour
{
	[Serializable]
	public class ManipulatorConfiguration
	{
		public float MaxGrabLength = 9f;

		public bool UserCanDrop = true;

		public bool UserCanGrab = true;

		public bool UserCanScroll = true;

		public bool SceneCanVacuum;

		public bool HasVacuum;
	}

	[Serializable]
	public class VacuumConfiguration
	{
		public float VacMaxLength = 5f;
	}

	[Serializable]
	public class TeleporterConfiguration
	{
		public bool TeleportEnabled;

		public float MinHoldTime = 0.2f;
	}

	[Serializable]
	public class CursorReticleConfiguration
	{
		public bool HideCursor;
	}

	[SerializeField]
	private ToolGroup[] _toolGroups;

	private static ToolManager G;

	private ManipulatorConfiguration _manipConfig = new ManipulatorConfiguration();

	private VacuumConfiguration _vacuumConfig = new VacuumConfiguration();

	private TeleporterConfiguration _teleporterConfig = new TeleporterConfiguration();

	private CursorReticleConfiguration _cursorReticleConfiguration = new CursorReticleConfiguration();

	private readonly List<ToolGroup> _activeToolGroups = new List<ToolGroup>(2);

	public static ManipulatorConfiguration AllManipulators => G._manipConfig;

	public static VacuumConfiguration AllVacuums => G._vacuumConfig;

	public static TeleporterConfiguration AllTeleporters => G._teleporterConfig;

	public static CursorReticleConfiguration AllCursorReticles => G._cursorReticleConfiguration;

	public static List<ToolGroup> ActiveToolGroups => G._activeToolGroups;

	public static bool IsVacuumOut
	{
		get
		{
			for (int i = 0; i < G._toolGroups.Length; i++)
			{
				if (G._toolGroups[i].IsActive && G._toolGroups[i].Manipulator.CurrentManipulatorMode == Manipulator.manipulatorMode.vacuum)
				{
					return true;
				}
			}
			return false;
		}
	}

	public static bool IsTeleporting
	{
		get
		{
			for (int i = 0; i < G._toolGroups.Length; i++)
			{
				if (G._toolGroups[i].IsActive && G._toolGroups[i].Teleporter.isLerpingToPosition)
				{
					return true;
				}
			}
			return false;
		}
	}

	public static float AdminCheatTimer
	{
		get
		{
			float num = 0f;
			for (int i = 0; i < G._toolGroups.Length; i++)
			{
				if (G._toolGroups[i].IsActive)
				{
					num = Mathf.Max(G._toolGroups[i].AdminCheatTimer, num);
				}
			}
			return num;
		}
	}

	private void Awake()
	{
		G = this;
	}

	private void OnEnable()
	{
		TBTracking.OnNodeConnected = (TBTracking.TrackingEvent)Delegate.Combine(TBTracking.OnNodeConnected, new TBTracking.TrackingEvent(OnNodeConnected));
		for (int i = 0; i < _toolGroups.Length; i++)
		{
			Transform transformForNode = TBTracking.GetTransformForNode(_toolGroups[i].Node);
			if (transformForNode != null)
			{
				_toolGroups[i].Activate(transformForNode);
				_activeToolGroups.Add(_toolGroups[i]);
			}
		}
	}

	private void OnNodeConnected(XRNode node, Transform t)
	{
		for (int i = 0; i < _toolGroups.Length; i++)
		{
			if (node == _toolGroups[i].Node)
			{
				_toolGroups[i].Activate(t);
				_activeToolGroups.Add(_toolGroups[i]);
			}
		}
	}

	public static bool PlayerGrabbingSomething(bool dropIfGrabbing = false, bool playSFX = true)
	{
		bool result = false;
		for (int i = 0; i < G._toolGroups.Length; i++)
		{
			if (!G._toolGroups[i].IsActive)
			{
				continue;
			}
			ObjectBase heldObjectBase = G._toolGroups[i].Manipulator.HeldObjectBase;
			if (heldObjectBase != null)
			{
				if (dropIfGrabbing)
				{
					heldObjectBase.ForceDropByPlayer(playSFX);
				}
				result = true;
			}
		}
		return result;
	}

	public static bool PlayerGrabbingObjectType<T>(bool dropIfGrabbing = false, bool playSFX = true)
	{
		bool result = false;
		for (int i = 0; i < G._toolGroups.Length; i++)
		{
			if (!G._toolGroups[i].IsActive)
			{
				continue;
			}
			ObjectBase heldObjectBase = G._toolGroups[i].Manipulator.HeldObjectBase;
			if (heldObjectBase is T)
			{
				if (dropIfGrabbing)
				{
					heldObjectBase.ForceDropByPlayer(playSFX);
				}
				result = true;
			}
		}
		return result;
	}

	public static bool PlayerGrabbingGameObject(GameObject o, bool dropIfGrabbing = false, bool playSFX = true)
	{
		bool result = false;
		for (int i = 0; i < G._toolGroups.Length; i++)
		{
			if (!G._toolGroups[i].IsActive)
			{
				continue;
			}
			ObjectBase heldObjectBase = G._toolGroups[i].Manipulator.HeldObjectBase;
			if (heldObjectBase != null && heldObjectBase.gameObject == o)
			{
				if (dropIfGrabbing)
				{
					heldObjectBase.ForceDropByPlayer(playSFX);
				}
				result = true;
			}
		}
		return result;
	}

	public static bool PlayerGrabbingObjectWithComponentType<C>(bool dropIfGrabbing = false, bool playSFX = true)
	{
		bool result = false;
		for (int i = 0; i < G._toolGroups.Length; i++)
		{
			if (!G._toolGroups[i].IsActive)
			{
				continue;
			}
			ObjectBase heldObjectBase = G._toolGroups[i].Manipulator.HeldObjectBase;
			if (heldObjectBase != null && heldObjectBase.gameObject.GetComponent<C>() != null)
			{
				if (dropIfGrabbing)
				{
					heldObjectBase.ForceDropByPlayer(playSFX);
				}
				result = true;
			}
		}
		return result;
	}

	public static bool PlayerGrabbingObjectBase(ObjectBase b, bool dropIfGrabbing = false, bool playSFX = true)
	{
		bool result = false;
		for (int i = 0; i < G._toolGroups.Length; i++)
		{
			if (!G._toolGroups[i].IsActive)
			{
				continue;
			}
			ObjectBase heldObjectBase = G._toolGroups[i].Manipulator.HeldObjectBase;
			if (heldObjectBase == b)
			{
				if (dropIfGrabbing)
				{
					heldObjectBase.ForceDropByPlayer(playSFX);
				}
				result = true;
			}
		}
		return result;
	}

	public static HRAICore PlayerGrabbedActiveCore()
	{
		for (int i = 0; i < G._toolGroups.Length; i++)
		{
			if (G._toolGroups[i].IsActive)
			{
				ObjectBase heldObjectBase = G._toolGroups[i].Manipulator.HeldObjectBase;
				if (heldObjectBase != null && (bool)heldObjectBase.GetComponent<HRAICore>() && !heldObjectBase.GetComponent<HRAICore>().isDisconnected)
				{
					return heldObjectBase.GetComponent<HRAICore>();
				}
			}
		}
		return null;
	}

	public static bool AnyControllerGetButtonDown(TBInput.Button button)
	{
		bool result = false;
		for (int i = 0; i < G._toolGroups.Length; i++)
		{
			if (G._toolGroups[i].IsActive && TBInput.GetButtonDown(button, G._toolGroups[i].Controller))
			{
				result = true;
			}
		}
		return result;
	}

	public static bool AnyControllerGetButton(TBInput.Button button)
	{
		bool result = false;
		for (int i = 0; i < G._toolGroups.Length; i++)
		{
			if (G._toolGroups[i].IsActive && TBInput.GetButton(button, G._toolGroups[i].Controller))
			{
				result = true;
			}
		}
		return result;
	}

	public static bool AnyControllerGetButtonUp(TBInput.Button button)
	{
		bool result = false;
		for (int i = 0; i < G._toolGroups.Length; i++)
		{
			if (G._toolGroups[i].IsActive && TBInput.GetButtonUp(button, G._toolGroups[i].Controller))
			{
				result = true;
			}
		}
		return result;
	}

	public static void DropEverything(bool playSFX = true)
	{
		for (int i = 0; i < G._toolGroups.Length; i++)
		{
			if (G._toolGroups[i].IsActive && G._toolGroups[i].Manipulator.CurrentManipulatorMode != Manipulator.manipulatorMode.vacuum)
			{
				ObjectBase heldObjectBase = G._toolGroups[i].Manipulator.HeldObjectBase;
				if (heldObjectBase != null)
				{
					heldObjectBase.ForceDropByPlayer(playSFX);
				}
			}
		}
	}

	public static void OnTeleport()
	{
		for (int i = 0; i < G._toolGroups.Length; i++)
		{
			if (G._toolGroups[i].IsActive)
			{
				ObjectBase heldObjectBase = G._toolGroups[i].Manipulator.HeldObjectBase;
				if (heldObjectBase != null)
				{
					heldObjectBase.onTeleport();
				}
			}
		}
	}

	public static ToolGroup GetToolGroupForController(TBInput.Controller controller)
	{
		for (int i = 0; i < G._toolGroups.Length; i++)
		{
			if (G._toolGroups[i].IsActive && controller == G._toolGroups[i].Controller)
			{
				return G._toolGroups[i];
			}
		}
		return null;
	}

	public static ToolGroup[] GetAllToolgroups()
	{
		return G._toolGroups;
	}

	public static void StopAllOtherTeleport(ToolGroup group)
	{
		for (int i = 0; i < G._toolGroups.Length; i++)
		{
			if (G._toolGroups[i] != group && G._toolGroups[i].IsActive && G._toolGroups[i].Teleporter.isLerpingToPosition)
			{
				G._toolGroups[i].Teleporter.StopTeleport();
			}
		}
	}

	public static ToolGroup GetToolgroupForOtherController(TBInput.Controller controller)
	{
		switch (controller)
		{
		case TBInput.Controller.Mobile3DOFController:
			return GetToolGroupForController(controller);
		case TBInput.Controller.LHandController:
			return GetToolGroupForController(TBInput.Controller.RHandController);
		case TBInput.Controller.RHandController:
			return GetToolGroupForController(TBInput.Controller.LHandController);
		default:
			SafeDebug.LogError("You inputted an invalid controller type to: GetToolgroupForOtherController");
			return null;
		}
	}

	public static VVRConsumer BestBlackHoleConsumer(Vector3 ediblePosition)
	{
		VVRConsumer result = null;
		VVRConsumer vVRConsumer = null;
		float num = -1f;
		float num2 = 0f;
		if (ActiveToolGroups.Count == 0)
		{
			return null;
		}
		if (ActiveToolGroups.Count == 1)
		{
			return ActiveToolGroups[0].BlackHole.vacConsumer;
		}
		for (int i = 0; i < ActiveToolGroups.Count; i++)
		{
			vVRConsumer = ActiveToolGroups[i].BlackHole.vacConsumer;
			num2 = 0f;
			if (vVRConsumer.gameObject.activeInHierarchy)
			{
				num2 += 5f;
			}
			if (vVRConsumer.isTransfering)
			{
				num2 = ((!vVRConsumer.isSucking) ? (num2 + 2f) : (num2 + 3f));
			}
			num2 += 1f - Mathf.Clamp01((vVRConsumer.mouthTransform.position - ediblePosition).sqrMagnitude * 0.01f);
			if (num2 > num)
			{
				num = num2;
				result = vVRConsumer;
			}
		}
		return result;
	}

	public static ChazController.chazMode CurrentChazGrabMode()
	{
		ChazController.chazMode chazMode = ChazController.chazMode.standing;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		for (int i = 0; i < ActiveToolGroups.Count; i++)
		{
			if (!ActiveToolGroups[i].Manipulator.isGrabbing())
			{
				continue;
			}
			ObjectBase heldObjectBase = ActiveToolGroups[i].Manipulator.HeldObjectBase;
			if (heldObjectBase is ObjectChazLeg)
			{
				ObjectChazLeg objectChazLeg = (ObjectChazLeg)heldObjectBase;
				switch (objectChazLeg.grabBehavior)
				{
				case ChazController.chazMode.headGrab:
					flag = true;
					break;
				case ChazController.chazMode.leftLegGrab:
					flag2 = true;
					break;
				case ChazController.chazMode.rightLegGrab:
					flag3 = true;
					break;
				}
			}
		}
		if (flag2 && !flag3)
		{
			chazMode = ChazController.chazMode.leftLegGrab;
		}
		else if (flag3 && !flag2)
		{
			chazMode = ChazController.chazMode.rightLegGrab;
		}
		else if (flag3 && flag2)
		{
			chazMode = ChazController.chazMode.bothLegsGrabbed;
		}
		else if (flag)
		{
			chazMode = ChazController.chazMode.headGrab;
		}
		SafeDebug.Log("chazmode: " + chazMode);
		return chazMode;
	}

	public static void CancelDisableTeleport()
	{
		for (int i = 0; i < G._toolGroups.Length; i++)
		{
			if (G._toolGroups[i].IsActive)
			{
				G._toolGroups[i].Teleporter.CancelDisableTeleport();
			}
		}
	}

	public static void StopTeleport()
	{
		for (int i = 0; i < G._toolGroups.Length; i++)
		{
			if (G._toolGroups[i].IsActive)
			{
				G._toolGroups[i].Teleporter.StopTeleport();
			}
		}
	}

	public static void ToggleManipulator(bool toggle)
	{
		for (int i = 0; i < G._toolGroups.Length; i++)
		{
			if (G._toolGroups[i].IsActive)
			{
				G._toolGroups[i].ToggleManipulator(toggle);
			}
		}
	}

	public static void ResetAdminTimer()
	{
		for (int i = 0; i < G._toolGroups.Length; i++)
		{
			if (G._toolGroups[i].IsActive)
			{
				G._toolGroups[i].ResetAdminCheatTimer();
			}
		}
	}
}
