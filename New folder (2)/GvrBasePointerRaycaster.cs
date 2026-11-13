using System.Collections.Generic;
using UnityEngine.EventSystems;

public abstract class GvrBasePointerRaycaster : BaseRaycaster
{
	private GvrBasePointer.PointerRay lastRay;

	protected GvrBasePointer.RaycastMode CurrentRaycastModeForHybrid { get; private set; }

	public GvrBasePointer.PointerRay GetLastRay()
	{
		return lastRay;
	}

	public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		GvrBasePointer pointer = GvrPointerInputModule.Pointer;
		if (!(pointer == null) && pointer.IsAvailable)
		{
			if (pointer.raycastMode == GvrBasePointer.RaycastMode.Hybrid)
			{
				RaycastHybrid(pointer, eventData, resultAppendList);
			}
			else
			{
				RaycastDefault(pointer, eventData, resultAppendList);
			}
		}
	}

	protected abstract bool PerformRaycast(GvrBasePointer.PointerRay pointerRay, float radius, PointerEventData eventData, List<RaycastResult> resultAppendList);

	private void RaycastHybrid(GvrBasePointer pointer, PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		CurrentRaycastModeForHybrid = GvrBasePointer.RaycastMode.Direct;
		lastRay = GvrBasePointer.CalculateHybridRay(pointer, CurrentRaycastModeForHybrid);
		float currentPointerRadius = pointer.CurrentPointerRadius;
		if (!PerformRaycast(lastRay, currentPointerRadius, eventData, resultAppendList))
		{
			CurrentRaycastModeForHybrid = GvrBasePointer.RaycastMode.Camera;
			lastRay = GvrBasePointer.CalculateHybridRay(pointer, CurrentRaycastModeForHybrid);
			PerformRaycast(lastRay, currentPointerRadius, eventData, resultAppendList);
		}
	}

	private void RaycastDefault(GvrBasePointer pointer, PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		lastRay = GvrBasePointer.CalculateRay(pointer, pointer.raycastMode);
		float currentPointerRadius = pointer.CurrentPointerRadius;
		PerformRaycast(lastRay, currentPointerRadius, eventData, resultAppendList);
	}
}
