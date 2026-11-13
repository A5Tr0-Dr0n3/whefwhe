using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("GoogleVR/GvrPointerInputModule")]
public class GvrPointerInputModule : BaseInputModule, IGvrInputModuleController
{
	[Tooltip("Whether Pointer input is active in VR Mode only (true), or all the time (false).")]
	public bool vrModeOnly;

	[Tooltip("Manages scroll events for the input module.")]
	public GvrPointerScrollInput scrollInput = new GvrPointerScrollInput();

	public GvrPointerInputModuleImpl Impl { get; private set; }

	public GvrEventExecutor EventExecutor { get; private set; }

	public new EventSystem eventSystem => base.eventSystem;

	public List<RaycastResult> RaycastResultCache => m_RaycastResultCache;

	public static GvrBasePointer Pointer
	{
		get
		{
			GvrPointerInputModule gvrPointerInputModule = FindInputModule();
			if (gvrPointerInputModule == null || gvrPointerInputModule.Impl == null)
			{
				return null;
			}
			return gvrPointerInputModule.Impl.Pointer;
		}
		set
		{
			GvrPointerInputModule gvrPointerInputModule = FindInputModule();
			if (!(gvrPointerInputModule == null) && gvrPointerInputModule.Impl != null)
			{
				gvrPointerInputModule.Impl.Pointer = value;
			}
		}
	}

	public static RaycastResult CurrentRaycastResult
	{
		get
		{
			GvrPointerInputModule gvrPointerInputModule = FindInputModule();
			if (gvrPointerInputModule == null)
			{
				return default;
			}
			if (gvrPointerInputModule.Impl == null)
			{
				return default;
			}
			if (gvrPointerInputModule.Impl.CurrentEventData == null)
			{
				return default;
			}
			return gvrPointerInputModule.Impl.CurrentEventData.pointerCurrentRaycast;
		}
	}

	public static void OnPointerCreated(GvrBasePointer createdPointer)
	{
		GvrPointerInputModule gvrPointerInputModule = FindInputModule();
		if (!(gvrPointerInputModule == null) && gvrPointerInputModule.Impl != null && gvrPointerInputModule.Impl.Pointer == null)
		{
			gvrPointerInputModule.Impl.Pointer = createdPointer;
		}
	}

	public static GvrEventExecutor FindEventExecutor()
	{
		GvrPointerInputModule gvrPointerInputModule = FindInputModule();
		if (gvrPointerInputModule == null)
		{
			return null;
		}
		return gvrPointerInputModule.EventExecutor;
	}

	public static GvrPointerInputModule FindInputModule()
	{
		if (EventSystem.current == null)
		{
			return null;
		}
		EventSystem current = EventSystem.current;
		if (current == null)
		{
			return null;
		}
		return current.GetComponent<GvrPointerInputModule>();
	}

	public override bool ShouldActivateModule()
	{
		return Impl.ShouldActivateModule();
	}

	public override void DeactivateModule()
	{
		Impl.DeactivateModule();
	}

	public override bool IsPointerOverGameObject(int pointerId)
	{
		return Impl.IsPointerOverGameObject(pointerId);
	}

	public override void Process()
	{
		UpdateImplProperties();
		Impl.Process();
	}

	protected override void Awake()
	{
		base.Awake();
		Impl = new GvrPointerInputModuleImpl();
		EventExecutor = new GvrEventExecutor();
		UpdateImplProperties();
	}

	public bool ShouldActivate()
	{
		return base.ShouldActivateModule();
	}

	public void Deactivate()
	{
		base.DeactivateModule();
	}

	public new GameObject FindCommonRoot(GameObject g1, GameObject g2)
	{
		return BaseInputModule.FindCommonRoot(g1, g2);
	}

	public new BaseEventData GetBaseEventData()
	{
		return base.GetBaseEventData();
	}

	public new RaycastResult FindFirstRaycast(List<RaycastResult> candidates)
	{
		return BaseInputModule.FindFirstRaycast(candidates);
	}

	private void UpdateImplProperties()
	{
		if (Impl != null)
		{
			Impl.ScrollInput = scrollInput;
			Impl.VrModeOnly = vrModeOnly;
			Impl.ModuleController = this;
			Impl.EventExecutor = EventExecutor;
		}
	}
}
