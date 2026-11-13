using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

public class GvrPointerInputModuleImpl
{
	private GvrBasePointer pointer;

	private Vector2 lastPose;

	private bool isPointerHovering;

	private bool isActive;

	public IGvrInputModuleController ModuleController { get; set; }

	public IGvrEventExecutor EventExecutor { get; set; }

	public bool VrModeOnly { get; set; }

	public GvrPointerScrollInput ScrollInput { get; set; }

	public PointerEventData CurrentEventData { get; private set; }

	public GvrBasePointer Pointer
	{
		get
		{
			return pointer;
		}
		set
		{
			if (!(pointer == value))
			{
				TryExitPointer();
				pointer = value;
			}
		}
	}

	public bool ShouldActivateModule()
	{
		bool flag = !VrModeOnly;
		flag |= XRSettings.enabled;
		bool flag2 = ModuleController.ShouldActivate() && flag;
		if (flag2 != isActive)
		{
			isActive = flag2;
		}
		return flag2;
	}

	public void DeactivateModule()
	{
		TryExitPointer();
		ModuleController.Deactivate();
		if (CurrentEventData != null)
		{
			HandlePendingClick();
			HandlePointerExitAndEnter(CurrentEventData, null);
			CurrentEventData = null;
		}
		ModuleController.eventSystem.SetSelectedGameObject(null, ModuleController.GetBaseEventData());
	}

	public bool IsPointerOverGameObject(int pointerId)
	{
		return CurrentEventData != null && CurrentEventData.pointerEnter != null;
	}

	public void Process()
	{
		if (!IsPointerActiveAndAvailable())
		{
			TryExitPointer();
		}
		GameObject currentGameObject = GetCurrentGameObject();
		CastRay();
		UpdateCurrentObject(currentGameObject);
		UpdatePointer(currentGameObject);
		bool flag = false;
		bool flag2 = false;
		if (IsPointerActiveAndAvailable())
		{
			flag = Pointer.TriggerDown;
			flag2 = Pointer.Triggering;
		}
		bool flag3 = !flag2;
		if (!flag && flag2)
		{
			HandleDrag();
		}
		else if (flag && !CurrentEventData.eligibleForClick)
		{
			HandleTriggerDown();
		}
		else if (flag3)
		{
			HandlePendingClick();
		}
		ScrollInput.HandleScroll(GetCurrentGameObject(), CurrentEventData, Pointer, EventExecutor);
	}

	private void CastRay()
	{
		Vector2 vector = lastPose;
		if (IsPointerActiveAndAvailable())
		{
			vector = GvrMathHelpers.NormalizedCartesianToSpherical(Pointer.PointerTransform.forward);
		}
		if (CurrentEventData == null)
		{
			CurrentEventData = new PointerEventData(ModuleController.eventSystem);
			lastPose = vector;
		}
		RaycastResult pointerCurrentRaycast = CurrentEventData.pointerCurrentRaycast;
		if (IsPointerActiveAndAvailable())
		{
			Pointer.ShouldUseExitRadiusForRaycast = false;
		}
		CurrentEventData.Reset();
		CurrentEventData.position = GvrVRHelpers.GetViewportCenter();
		bool flag = IsPointerActiveAndAvailable();
		RaycastResult raycastResult;
		if (flag)
		{
			RaycastAll();
			raycastResult = ModuleController.FindFirstRaycast(ModuleController.RaycastResultCache);
		}
		else
		{
			raycastResult = default;
			raycastResult.Clear();
		}
		if (pointerCurrentRaycast.gameObject != null && raycastResult.gameObject != pointerCurrentRaycast.gameObject && flag)
		{
			Pointer.ShouldUseExitRadiusForRaycast = true;
			RaycastAll();
			RaycastResult raycastResult2 = ModuleController.FindFirstRaycast(ModuleController.RaycastResultCache);
			if (raycastResult2.gameObject == pointerCurrentRaycast.gameObject)
			{
				raycastResult = raycastResult2;
			}
		}
		if (raycastResult.gameObject != null && raycastResult.worldPosition == Vector3.zero)
		{
			raycastResult.worldPosition = GvrMathHelpers.GetIntersectionPosition(CurrentEventData.enterEventCamera, raycastResult);
		}
		CurrentEventData.pointerCurrentRaycast = raycastResult;
		if (raycastResult.gameObject != null)
		{
			CurrentEventData.position = raycastResult.screenPosition;
		}
		else if (IsPointerActiveAndAvailable() && CurrentEventData.enterEventCamera != null)
		{
			Vector3 maxPointerEndPoint = Pointer.MaxPointerEndPoint;
			CurrentEventData.position = CurrentEventData.enterEventCamera.WorldToScreenPoint(maxPointerEndPoint);
		}
		ModuleController.RaycastResultCache.Clear();
		CurrentEventData.delta = vector - lastPose;
		lastPose = vector;
		if (raycastResult.module != null && !(raycastResult.module is GvrPointerGraphicRaycaster) && !(raycastResult.module is GvrPointerPhysicsRaycaster))
		{
			Debug.LogWarning(string.Concat("Using Raycaster (Raycaster: ", raycastResult.module.GetType(), ", Object: ", raycastResult.module.name, "). It is recommended to use GvrPointerPhysicsRaycaster or GvrPointerGrahpicRaycaster with GvrPointerInputModule."));
		}
	}

	private void UpdateCurrentObject(GameObject previousObject)
	{
		if (CurrentEventData != null)
		{
			GameObject currentGameObject = GetCurrentGameObject();
			HandlePointerExitAndEnter(CurrentEventData, currentGameObject);
			GameObject eventHandler = EventExecutor.GetEventHandler<ISelectHandler>(currentGameObject);
			if (eventHandler == ModuleController.eventSystem.currentSelectedGameObject)
			{
				EventExecutor.Execute(ModuleController.eventSystem.currentSelectedGameObject, ModuleController.GetBaseEventData(), ExecuteEvents.updateSelectedHandler);
			}
			else
			{
				ModuleController.eventSystem.SetSelectedGameObject(null, CurrentEventData);
			}
			if (currentGameObject != null && currentGameObject == previousObject)
			{
				EventExecutor.ExecuteHierarchy(currentGameObject, CurrentEventData, GvrExecuteEventsExtension.pointerHoverHandler);
			}
		}
	}

	private void UpdatePointer(GameObject previousObject)
	{
		if (CurrentEventData == null)
		{
			return;
		}
		GameObject currentGameObject = GetCurrentGameObject();
		bool flag = IsPointerActiveAndAvailable();
		bool isInteractive = CurrentEventData.pointerPress != null || EventExecutor.GetEventHandler<IPointerClickHandler>(currentGameObject) != null || EventExecutor.GetEventHandler<IDragHandler>(currentGameObject) != null;
		if (isPointerHovering && currentGameObject != null && currentGameObject == previousObject)
		{
			if (flag)
			{
				Pointer.OnPointerHover(CurrentEventData.pointerCurrentRaycast, isInteractive);
			}
			return;
		}
		if (previousObject != null || (currentGameObject == null && isPointerHovering))
		{
			if (flag)
			{
				Pointer.OnPointerExit(previousObject);
			}
			isPointerHovering = false;
		}
		if (currentGameObject != null)
		{
			if (flag)
			{
				Pointer.OnPointerEnter(CurrentEventData.pointerCurrentRaycast, isInteractive);
			}
			isPointerHovering = true;
		}
	}

	private static bool ShouldStartDrag(Vector2 pressPos, Vector2 currentPos, float threshold, bool useDragThreshold)
	{
		if (!useDragThreshold)
		{
			return true;
		}
		return (pressPos - currentPos).sqrMagnitude >= threshold * threshold;
	}

	private void HandleDrag()
	{
		bool flag = CurrentEventData.IsPointerMoving();
		bool flag2 = ShouldStartDrag(CurrentEventData.pressPosition, CurrentEventData.position, ModuleController.eventSystem.pixelDragThreshold, CurrentEventData.useDragThreshold);
		if (flag && flag2 && CurrentEventData.pointerDrag != null && !CurrentEventData.dragging)
		{
			EventExecutor.Execute(CurrentEventData.pointerDrag, CurrentEventData, ExecuteEvents.beginDragHandler);
			CurrentEventData.dragging = true;
		}
		if (CurrentEventData.dragging && flag && CurrentEventData.pointerDrag != null)
		{
			if (CurrentEventData.pointerPress != CurrentEventData.pointerDrag)
			{
				EventExecutor.Execute(CurrentEventData.pointerPress, CurrentEventData, ExecuteEvents.pointerUpHandler);
				CurrentEventData.eligibleForClick = false;
				CurrentEventData.pointerPress = null;
				CurrentEventData.rawPointerPress = null;
			}
			EventExecutor.Execute(CurrentEventData.pointerDrag, CurrentEventData, ExecuteEvents.dragHandler);
		}
	}

	private void HandlePendingClick()
	{
		if (CurrentEventData != null && (CurrentEventData.eligibleForClick || CurrentEventData.dragging))
		{
			if (IsPointerActiveAndAvailable())
			{
				Pointer.OnPointerClickUp();
			}
			GameObject gameObject = CurrentEventData.pointerCurrentRaycast.gameObject;
			EventExecutor.Execute(CurrentEventData.pointerPress, CurrentEventData, ExecuteEvents.pointerUpHandler);
			GameObject eventHandler = EventExecutor.GetEventHandler<IPointerClickHandler>(gameObject);
			if (CurrentEventData.pointerPress == eventHandler && CurrentEventData.eligibleForClick)
			{
				EventExecutor.Execute(CurrentEventData.pointerPress, CurrentEventData, ExecuteEvents.pointerClickHandler);
			}
			if (CurrentEventData.pointerDrag != null && CurrentEventData.dragging)
			{
				EventExecutor.ExecuteHierarchy(gameObject, CurrentEventData, ExecuteEvents.dropHandler);
				EventExecutor.Execute(CurrentEventData.pointerDrag, CurrentEventData, ExecuteEvents.endDragHandler);
			}
			CurrentEventData.pointerPress = null;
			CurrentEventData.rawPointerPress = null;
			CurrentEventData.eligibleForClick = false;
			CurrentEventData.clickCount = 0;
			CurrentEventData.clickTime = 0f;
			CurrentEventData.pointerDrag = null;
			CurrentEventData.dragging = false;
		}
	}

	private void HandleTriggerDown()
	{
		GameObject gameObject = CurrentEventData.pointerCurrentRaycast.gameObject;
		CurrentEventData.pressPosition = CurrentEventData.position;
		CurrentEventData.pointerPressRaycast = CurrentEventData.pointerCurrentRaycast;
		CurrentEventData.pointerPress = EventExecutor.ExecuteHierarchy(gameObject, CurrentEventData, ExecuteEvents.pointerDownHandler) ?? EventExecutor.GetEventHandler<IPointerClickHandler>(gameObject);
		CurrentEventData.rawPointerPress = gameObject;
		CurrentEventData.eligibleForClick = true;
		CurrentEventData.delta = Vector2.zero;
		CurrentEventData.dragging = false;
		CurrentEventData.useDragThreshold = true;
		CurrentEventData.clickCount = 1;
		CurrentEventData.clickTime = Time.unscaledTime;
		CurrentEventData.pointerDrag = EventExecutor.GetEventHandler<IDragHandler>(gameObject);
		if (CurrentEventData.pointerDrag != null)
		{
			EventExecutor.Execute(CurrentEventData.pointerDrag, CurrentEventData, ExecuteEvents.initializePotentialDrag);
		}
		if (IsPointerActiveAndAvailable())
		{
			Pointer.OnPointerClickDown();
		}
	}

	private GameObject GetCurrentGameObject()
	{
		if (CurrentEventData != null)
		{
			return CurrentEventData.pointerCurrentRaycast.gameObject;
		}
		return null;
	}

	private void HandlePointerExitAndEnter(PointerEventData currentPointerData, GameObject newEnterTarget)
	{
		if (newEnterTarget == null || currentPointerData.pointerEnter == null)
		{
			for (int i = 0; i < currentPointerData.hovered.Count; i++)
			{
				EventExecutor.Execute(currentPointerData.hovered[i], currentPointerData, ExecuteEvents.pointerExitHandler);
			}
			currentPointerData.hovered.Clear();
			if (newEnterTarget == null)
			{
				currentPointerData.pointerEnter = newEnterTarget;
				return;
			}
		}
		if ((bool)newEnterTarget && currentPointerData.pointerEnter == newEnterTarget)
		{
			return;
		}
		GameObject gameObject = ModuleController.FindCommonRoot(currentPointerData.pointerEnter, newEnterTarget);
		if (currentPointerData.pointerEnter != null)
		{
			Transform transform = currentPointerData.pointerEnter.transform;
			while (transform != null && (!(gameObject != null) || !(gameObject.transform == transform)))
			{
				EventExecutor.Execute(transform.gameObject, currentPointerData, ExecuteEvents.pointerExitHandler);
				currentPointerData.hovered.Remove(transform.gameObject);
				transform = transform.parent;
			}
		}
		currentPointerData.pointerEnter = newEnterTarget;
		if (newEnterTarget != null)
		{
			Transform transform2 = newEnterTarget.transform;
			while (transform2 != null && transform2.gameObject != gameObject)
			{
				EventExecutor.Execute(transform2.gameObject, currentPointerData, ExecuteEvents.pointerEnterHandler);
				currentPointerData.hovered.Add(transform2.gameObject);
				transform2 = transform2.parent;
			}
		}
	}

	private void TryExitPointer()
	{
		if (!(Pointer == null))
		{
			GameObject currentGameObject = GetCurrentGameObject();
			if ((bool)currentGameObject)
			{
				Pointer.OnPointerExit(currentGameObject);
			}
		}
	}

	private bool IsPointerActiveAndAvailable()
	{
		return pointer != null && pointer.IsAvailable;
	}

	private void RaycastAll()
	{
		ModuleController.RaycastResultCache.Clear();
		ModuleController.eventSystem.RaycastAll(CurrentEventData, ModuleController.RaycastResultCache);
	}
}
