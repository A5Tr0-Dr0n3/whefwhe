using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("GoogleVR/GvrPointerPhysicsRaycaster")]
public class GvrPointerPhysicsRaycaster : GvrBasePointerRaycaster
{
	private class HitComparer : IComparer<RaycastHit>
	{
		public int Compare(RaycastHit lhs, RaycastHit rhs)
		{
			return lhs.distance.CompareTo(rhs.distance);
		}
	}

	protected const int NO_EVENT_MASK_SET = -1;

	private const int MAX_RAYCAST_HITS_MAX = 512;

	[SerializeField]
	protected LayerMask raycasterEventMask = -1;

	[SerializeField]
	[Range(1f, 512f)]
	private int maxRaycastHits = 64;

	private RaycastHit[] hits;

	private HitComparer hitComparer = new HitComparer();

	public int MaxRaycastHits
	{
		get
		{
			return maxRaycastHits;
		}
		set
		{
			maxRaycastHits = Mathf.Min(value, 512);
			if (Application.isPlaying && hits != null && hits.Length != maxRaycastHits)
			{
				hits = new RaycastHit[maxRaycastHits];
			}
		}
	}

	public override Camera eventCamera
	{
		get
		{
			GvrBasePointer pointer = GvrPointerInputModule.Pointer;
			if (pointer == null)
			{
				return null;
			}
			return pointer.PointerCamera;
		}
	}

	public int finalEventMask => (!(eventCamera != null)) ? (-1) : (eventCamera.cullingMask & (int)eventMask);

	public LayerMask eventMask
	{
		get
		{
			return raycasterEventMask;
		}
		set
		{
			raycasterEventMask = value;
		}
	}

	protected GvrPointerPhysicsRaycaster()
	{
	}

	protected override void Awake()
	{
		base.Awake();
		hits = new RaycastHit[maxRaycastHits];
	}

	protected override bool PerformRaycast(GvrBasePointer.PointerRay pointerRay, float radius, PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		if (eventCamera == null)
		{
			return false;
		}
		int num = ((!(radius > 0f)) ? Physics.RaycastNonAlloc(pointerRay.ray, hits, pointerRay.distance, finalEventMask) : Physics.SphereCastNonAlloc(pointerRay.ray, radius, hits, pointerRay.distance, finalEventMask));
		if (num == 0)
		{
			return false;
		}
		if (num == MaxRaycastHits)
		{
			MaxRaycastHits *= 2;
			Debug.LogWarningFormat("Physics Raycast/Spherecast returned {0} hits, which is the current maximum and means that some hits may have been lost. Setting maxRaycastHits to {1}. Please set maxRaycastHits to a sufficiently high value for your scene.", num, MaxRaycastHits);
		}
		Array.Sort(hits, 0, num, hitComparer);
		for (int i = 0; i < num; i++)
		{
			Vector3 vector = Vector3.Project(hits[i].point - pointerRay.ray.origin, pointerRay.ray.direction);
			Vector3 vector2 = vector + pointerRay.ray.origin;
			float distance = hits[i].distance + pointerRay.distanceFromStart;
			Transform pointerTransform = GvrPointerInputModule.Pointer.PointerTransform;
			float magnitude = (vector2 - pointerTransform.position).magnitude;
			if (!(magnitude < pointerRay.distanceFromStart))
			{
				RaycastResult item = new RaycastResult
				{
					gameObject = hits[i].collider.gameObject,
					module = this,
					distance = distance,
					worldPosition = vector2,
					worldNormal = hits[i].normal,
					screenPosition = eventCamera.WorldToScreenPoint(vector2),
					index = resultAppendList.Count,
					sortingLayer = 0,
					sortingOrder = 0
				};
				resultAppendList.Add(item);
			}
		}
		return true;
	}
}
