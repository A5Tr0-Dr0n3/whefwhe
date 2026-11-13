using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("GoogleVR/GvrPointerGraphicRaycaster")]
[RequireComponent(typeof(Canvas))]
public class GvrPointerGraphicRaycaster : GvrBasePointerRaycaster
{
	public enum BlockingObjects
	{
		None,
		TwoD,
		ThreeD,
		All
	}

	private const int NO_EVENT_MASK_SET = -1;

	public bool ignoreReversedGraphics = true;

	public BlockingObjects blockingObjects;

	public LayerMask blockingMask = -1;

	private Canvas targetCanvas;

	private List<Graphic> raycastResults = new List<Graphic>();

	private Camera cachedPointerEventCamera;

	private static readonly List<Graphic> sortedGraphics = new List<Graphic>();

	public override Camera eventCamera
	{
		get
		{
			GvrBasePointer pointer = GvrPointerInputModule.Pointer;
			if (pointer == null)
			{
				return null;
			}
			if (pointer.raycastMode == GvrBasePointer.RaycastMode.Hybrid)
			{
				return GetCameraForRaycastMode(pointer, CurrentRaycastModeForHybrid);
			}
			return GetCameraForRaycastMode(pointer, pointer.raycastMode);
		}
	}

	private Canvas canvas
	{
		get
		{
			if (targetCanvas != null)
			{
				return targetCanvas;
			}
			targetCanvas = GetComponent<Canvas>();
			return targetCanvas;
		}
	}

	protected GvrPointerGraphicRaycaster()
	{
	}

	protected override bool PerformRaycast(GvrBasePointer.PointerRay pointerRay, float radius, PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		if (canvas == null)
		{
			return false;
		}
		if (eventCamera == null)
		{
			return false;
		}
		if (canvas.renderMode != RenderMode.WorldSpace)
		{
			Debug.LogError("GvrPointerGraphicRaycaster requires that the canvas renderMode is set to WorldSpace.");
			return false;
		}
		float num = float.MaxValue;
		if (blockingObjects != BlockingObjects.None)
		{
			float distance = pointerRay.distance;
			if ((blockingObjects == BlockingObjects.ThreeD || blockingObjects == BlockingObjects.All) && Physics.Raycast(pointerRay.ray, out var hitInfo, distance, blockingMask))
			{
				num = hitInfo.distance;
			}
			if (blockingObjects == BlockingObjects.TwoD || blockingObjects == BlockingObjects.All)
			{
				RaycastHit2D raycastHit2D = Physics2D.Raycast(pointerRay.ray.origin, pointerRay.ray.direction, distance, blockingMask);
				if (raycastHit2D.collider != null)
				{
					num = raycastHit2D.fraction * distance;
				}
			}
		}
		raycastResults.Clear();
		Raycast(canvas, pointerRay.ray, eventCamera, pointerRay.distance, raycastResults, out var _);
		bool result = false;
		for (int i = 0; i < raycastResults.Count; i++)
		{
			GameObject gameObject = raycastResults[i].gameObject;
			bool flag = true;
			if (ignoreReversedGraphics)
			{
				Vector3 lhs = eventCamera.transform.rotation * Vector3.forward;
				Vector3 rhs = gameObject.transform.rotation * Vector3.forward;
				flag = Vector3.Dot(lhs, rhs) > 0f;
			}
			if (!flag)
			{
				continue;
			}
			float num2 = 0f;
			Transform transform = gameObject.transform;
			Vector3 forward = transform.forward;
			float num3 = Vector3.Dot(forward, transform.position - pointerRay.ray.origin);
			float num4 = Vector3.Dot(forward, pointerRay.ray.direction);
			num2 = num3 / num4;
			Vector3 vector = pointerRay.ray.origin + pointerRay.ray.direction * num2;
			num2 += pointerRay.distanceFromStart;
			if (!(num2 < 0f) && !(num2 >= num) && !(num2 > pointerRay.distance))
			{
				Transform pointerTransform = GvrPointerInputModule.Pointer.PointerTransform;
				float magnitude = (vector - pointerTransform.position).magnitude;
				if (!(magnitude < pointerRay.distanceFromStart))
				{
					RaycastResult item = new RaycastResult
					{
						gameObject = gameObject,
						module = this,
						distance = num2,
						worldPosition = vector,
						screenPosition = eventCamera.WorldToScreenPoint(vector),
						index = resultAppendList.Count,
						depth = raycastResults[i].depth,
						sortingLayer = canvas.sortingLayerID,
						sortingOrder = canvas.sortingOrder
					};
					resultAppendList.Add(item);
					result = true;
				}
			}
		}
		return result;
	}

	private Camera GetCameraForRaycastMode(GvrBasePointer pointer, GvrBasePointer.RaycastMode mode)
	{
		switch (mode)
		{
		case GvrBasePointer.RaycastMode.Direct:
			if (cachedPointerEventCamera == null)
			{
				Transform pointerTransform = GvrPointerInputModule.Pointer.PointerTransform;
				cachedPointerEventCamera = pointerTransform.GetComponent<Camera>();
			}
			if (cachedPointerEventCamera == null)
			{
				cachedPointerEventCamera = AddDummyCameraToPointer(pointer);
				return null;
			}
			return cachedPointerEventCamera;
		default:
			return pointer.PointerCamera;
		}
	}

	private Camera AddDummyCameraToPointer(GvrBasePointer pointer)
	{
		Camera camera = pointer.PointerTransform.gameObject.AddComponent<Camera>();
		camera.enabled = false;
		camera.nearClipPlane = 0.01f;
		return camera;
	}

	private static void Raycast(Canvas canvas, Ray ray, Camera cam, float distance, List<Graphic> results, out Ray finalRay)
	{
		Vector3 vector = cam.WorldToScreenPoint(ray.GetPoint(distance));
		finalRay = cam.ScreenPointToRay(vector);
		IList<Graphic> graphicsForCanvas = GraphicRegistry.GetGraphicsForCanvas(canvas);
		for (int i = 0; i < graphicsForCanvas.Count; i++)
		{
			Graphic graphic = graphicsForCanvas[i];
			if (graphic.depth != -1 && graphic.raycastTarget && RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, vector, cam) && graphic.Raycast(vector, cam))
			{
				sortedGraphics.Add(graphic);
			}
		}
		sortedGraphics.Sort((Graphic g1, Graphic g2) => g2.depth.CompareTo(g1.depth));
		for (int num = 0; num < sortedGraphics.Count; num++)
		{
			results.Add(sortedGraphics[num]);
		}
		sortedGraphics.Clear();
	}
}
