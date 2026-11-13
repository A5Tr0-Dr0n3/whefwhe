using System;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class GvrBasePointer : MonoBehaviour
{
	public enum RaycastMode
	{
		Camera,
		Direct,
		Hybrid
	}

	public struct PointerRay
	{
		public Ray ray;

		public float distanceFromStart;

		public float distance;
	}

	[Tooltip("Determines which raycast mode to use for this raycaster.\n • Camera - Ray is cast from camera.\n • Direct - Ray is cast from pointer.\n • Hybrid - Transitions from Direct ray to Camera ray.")]
	public RaycastMode raycastMode = RaycastMode.Hybrid;

	[Tooltip("Optional: Use a camera other than Camera.main.")]
	public Camera overridePointerCamera;

	public RaycastResult CurrentRaycastResult => GvrPointerInputModule.CurrentRaycastResult;

	[Obsolete("Replaced by CurrentRaycastResult.worldPosition")]
	public Vector3 PointerIntersection => CurrentRaycastResult.worldPosition;

	[Obsolete("Replaced by CurrentRaycastResult.gameObject != null")]
	public bool IsPointerIntersecting => CurrentRaycastResult.gameObject != null;

	public bool ShouldUseExitRadiusForRaycast { get; set; }

	public float CurrentPointerRadius
	{
		get
		{
			GetPointerRadius(out var enterRadius, out var exitRadius);
			if (ShouldUseExitRadiusForRaycast)
			{
				return exitRadius;
			}
			return enterRadius;
		}
	}

	public virtual Transform PointerTransform => transform;

	public virtual bool TriggerDown => Input.GetMouseButtonDown(0) || GvrControllerInput.ClickButtonDown;

	public virtual bool Triggering => Input.GetMouseButton(0) || GvrControllerInput.ClickButton;

	public virtual bool TriggerUp => Input.GetMouseButtonUp(0) || GvrControllerInput.ClickButtonUp;

	public virtual bool TouchDown => GvrControllerInput.TouchDown;

	public virtual bool IsTouching => GvrControllerInput.IsTouching;

	public virtual bool TouchUp => GvrControllerInput.TouchUp;

	public virtual Vector2 TouchPos => GvrControllerInput.TouchPos;

	public virtual Vector3 MaxPointerEndPoint
	{
		get
		{
			Transform pointerTransform = PointerTransform;
			if (pointerTransform == null)
			{
				return Vector3.zero;
			}
			return GetPointAlongPointer(MaxPointerDistance);
		}
	}

	public virtual bool IsAvailable
	{
		get
		{
			Transform pointerTransform = PointerTransform;
			if (pointerTransform == null)
			{
				return false;
			}
			if (!enabled)
			{
				return false;
			}
			return pointerTransform.gameObject.activeInHierarchy;
		}
	}

	public virtual float CameraRayIntersectionDistance => MaxPointerDistance;

	public Camera PointerCamera
	{
		get
		{
			if (overridePointerCamera != null)
			{
				return overridePointerCamera;
			}
			return Camera.main;
		}
	}

	public abstract float MaxPointerDistance { get; }

	public abstract void OnPointerEnter(RaycastResult raycastResult, bool isInteractive);

	public abstract void OnPointerHover(RaycastResult raycastResultResult, bool isInteractive);

	public abstract void OnPointerExit(GameObject previousObject);

	public abstract void OnPointerClickDown();

	public abstract void OnPointerClickUp();

	public abstract void GetPointerRadius(out float enterRadius, out float exitRadius);

	public Vector3 GetPointAlongPointer(float distance)
	{
		PointerRay rayForDistance = GetRayForDistance(distance);
		return rayForDistance.ray.GetPoint(distance - rayForDistance.distanceFromStart);
	}

	public PointerRay GetRayForDistance(float distance)
	{
		PointerRay pointerRay = default;
		if (raycastMode == RaycastMode.Hybrid)
		{
			float cameraRayIntersectionDistance = CameraRayIntersectionDistance;
			if (distance < cameraRayIntersectionDistance)
			{
				return CalculateHybridRay(this, RaycastMode.Direct);
			}
			return CalculateHybridRay(this, RaycastMode.Camera);
		}
		return CalculateRay(this, raycastMode);
	}

	public static PointerRay CalculateRay(GvrBasePointer pointer, RaycastMode mode)
	{
		PointerRay result = default;
		if (pointer == null || !pointer.IsAvailable)
		{
			Debug.LogError("Cannot calculate ray when the pointer isn't available.");
			return result;
		}
		Transform pointerTransform = pointer.PointerTransform;
		if (pointerTransform == null)
		{
			Debug.LogError("Cannot calculate ray when pointerTransform is null.");
			return result;
		}
		result.distance = pointer.MaxPointerDistance;
		switch (mode)
		{
		case RaycastMode.Camera:
		{
			Camera pointerCamera = pointer.PointerCamera;
			if (pointerCamera == null)
			{
				Debug.LogError("Cannot calculate ray because pointer.PointerCamera is null.To fix this, either tag a Camera as \"MainCamera\" or set overridePointerCamera.");
				return result;
			}
			Vector3 position = pointerTransform.position;
			Vector3 vector = position + pointerTransform.forward * pointer.CameraRayIntersectionDistance;
			Vector3 position2 = pointerCamera.transform.position;
			Vector3 vector2 = vector - position2;
			vector2.Normalize();
			Vector3 origin = position2 + vector2 * pointerCamera.nearClipPlane;
			result.ray = new Ray(origin, vector2);
			break;
		}
		case RaycastMode.Direct:
			result.ray = new Ray(pointerTransform.position, pointerTransform.forward);
			break;
		default:
			throw new UnityException(string.Concat("Invalid RaycastMode ", mode, " passed into CalculateRay."));
		}
		return result;
	}

	public static PointerRay CalculateHybridRay(GvrBasePointer pointer, RaycastMode hybridMode)
	{
		PointerRay result;
		switch (hybridMode)
		{
		case RaycastMode.Direct:
			result = CalculateRay(pointer, hybridMode);
			result.distance = pointer.CameraRayIntersectionDistance;
			break;
		case RaycastMode.Camera:
		{
			result = CalculateRay(pointer, hybridMode);
			PointerRay pointerRay = CalculateHybridRay(pointer, RaycastMode.Direct);
			result.ray.origin = pointerRay.ray.GetPoint(pointerRay.distance);
			result.distanceFromStart = pointerRay.distance;
			result.distance = pointer.MaxPointerDistance - pointerRay.distance;
			break;
		}
		default:
			throw new UnityException(string.Concat("Invalid RaycastMode ", hybridMode, " passed into CalculateHybridRay."));
		}
		return result;
	}

	protected virtual void Start()
	{
		GvrPointerInputModule.OnPointerCreated(this);
	}
}
