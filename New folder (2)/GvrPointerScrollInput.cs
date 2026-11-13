using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class GvrPointerScrollInput
{
	private class ScrollInfo
	{
		public bool isScrollingX;

		public bool isScrollingY;

		public Vector2 initScroll = Vector2.zero;

		public Vector2 lastScroll = Vector2.zero;

		public Vector2 scrollVelocity = Vector2.zero;

		public IGvrScrollSettings scrollSettings;

		public bool IsScrolling => isScrollingX || isScrollingY;
	}

	public const string PROPERTY_NAME_INERTIA = "inertia";

	public const string PROPERTY_NAME_DECELERATION_RATE = "decelerationRate";

	[Tooltip("Determines if movement inertia is enabled.")]
	public bool inertia = true;

	[Tooltip("The rate at which movement slows down.")]
	public float decelerationRate = 0.05f;

	public const float SCROLL_DELTA_MULTIPLIER = 1000f;

	private const float CUTOFF_HZ = 10f;

	private const float RC = 1f / (20f * (float)Math.PI);

	private const float SPEED_CLAMP_RATIO = 0.05f;

	private const float SPEED_CLAMP = 50f;

	private const float SPEED_CLAMP_SQUARED = 2500f;

	private const float INERTIA_THRESHOLD_RATIO = 0.2f;

	private const float INERTIA_THRESHOLD = 200f;

	private const float INERTIA_THRESHOLD_SQUARED = 40000f;

	private const float SLOP_VERTICAL = 165f;

	private const float SLOP_HORIZONTAL = 150f;

	private Dictionary<GameObject, ScrollInfo> scrollHandlers = new Dictionary<GameObject, ScrollInfo>();

	private List<GameObject> scrollingObjects = new List<GameObject>();

	public void HandleScroll(GameObject currentGameObject, PointerEventData pointerData, GvrBasePointer pointer, IGvrEventExecutor eventExecutor)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		Vector2 currentScroll = Vector2.zero;
		if (pointer != null && pointer.IsAvailable)
		{
			flag = pointer.TouchDown;
			flag2 = pointer.IsTouching;
			flag3 = pointer.TouchUp;
			currentScroll = pointer.TouchPos * 1000f;
		}
		GameObject eventHandler = eventExecutor.GetEventHandler<IScrollHandler>(currentGameObject);
		if (flag)
		{
			RemoveScrollHandler(eventHandler);
		}
		if (eventHandler != null && (flag || flag2))
		{
			OnTouchingScrollHandler(eventHandler, pointerData, currentScroll, eventExecutor);
		}
		else if (flag3 && eventHandler != null)
		{
			OnReleaseScrollHandler(eventHandler);
		}
		StopScrollingIfNecessary(flag2, eventHandler);
		UpdateInertiaScrollHandlers(flag2, eventHandler, pointerData, eventExecutor);
	}

	private void OnTouchingScrollHandler(GameObject currentScrollHandler, PointerEventData pointerData, Vector2 currentScroll, IGvrEventExecutor eventExecutor)
	{
		ScrollInfo scrollInfo = null;
		scrollInfo = (scrollHandlers.ContainsKey(currentScrollHandler) ? scrollHandlers[currentScrollHandler] : AddScrollHandler(currentScrollHandler, currentScroll));
		if (CanScrollStartX(scrollInfo, currentScroll))
		{
			scrollInfo.isScrollingX = true;
		}
		if (CanScrollStartY(scrollInfo, currentScroll))
		{
			scrollInfo.isScrollingY = true;
		}
		if (scrollInfo.IsScrolling)
		{
			Vector2 vector = currentScroll;
			Vector2 lastScroll = scrollInfo.lastScroll;
			if (!scrollInfo.isScrollingX)
			{
				vector.x = 0f;
				lastScroll.x = 0f;
			}
			if (!scrollInfo.isScrollingY)
			{
				vector.y = 0f;
				lastScroll.y = 0f;
			}
			Vector2 vector2 = vector - lastScroll;
			UpdateVelocity(scrollInfo, vector2);
			if (!ShouldUseInertia(scrollInfo))
			{
				pointerData.scrollDelta = vector2;
				eventExecutor.ExecuteHierarchy(currentScrollHandler, pointerData, ExecuteEvents.scrollHandler);
				pointerData.scrollDelta = Vector2.zero;
			}
		}
		scrollInfo.lastScroll = currentScroll;
	}

	private void OnReleaseScrollHandler(GameObject currentScrollHandler)
	{
		if (scrollHandlers.TryGetValue(currentScrollHandler, out var value) && (!value.IsScrolling || value.scrollVelocity.sqrMagnitude <= 40000f))
		{
			RemoveScrollHandler(currentScrollHandler);
		}
	}

	private void UpdateVelocity(ScrollInfo scrollInfo, Vector2 scrollDisplacement)
	{
		Vector2 b = scrollDisplacement / Time.deltaTime;
		float t = Time.deltaTime / (1f / (20f * (float)Math.PI) + Time.deltaTime);
		scrollInfo.scrollVelocity = Vector2.Lerp(scrollInfo.scrollVelocity, b, t);
	}

	private void StopScrollingIfNecessary(bool touching, GameObject currentScrollHandler)
	{
		if (scrollHandlers.Count == 0)
		{
			return;
		}
		for (int num = scrollingObjects.Count - 1; num >= 0; num--)
		{
			GameObject gameObject = scrollingObjects[num];
			ScrollInfo scrollInfo = scrollHandlers[gameObject];
			bool isScrolling = scrollInfo.IsScrolling;
			bool flag = isScrolling && scrollInfo.scrollVelocity.sqrMagnitude <= 2500f;
			bool flag2 = touching && gameObject == currentScrollHandler;
			bool flag3 = ShouldUseInertia(scrollInfo);
			if (flag || ((!flag3 || !isScrolling) && !flag2))
			{
				RemoveScrollHandler(gameObject);
			}
		}
	}

	private void UpdateInertiaScrollHandlers(bool touching, GameObject currentScrollHandler, PointerEventData pointerData, IGvrEventExecutor eventExecutor)
	{
		if (pointerData == null)
		{
			return;
		}
		for (int i = 0; i < scrollingObjects.Count; i++)
		{
			GameObject gameObject = scrollingObjects[i];
			ScrollInfo scrollInfo = scrollHandlers[gameObject];
			if (ShouldUseInertia(scrollInfo) && scrollInfo.IsScrolling)
			{
				if (!touching || gameObject != currentScrollHandler)
				{
					float f = GetDecelerationRate(scrollInfo);
					scrollInfo.scrollVelocity *= Mathf.Pow(f, Time.deltaTime);
				}
				pointerData.scrollDelta = scrollInfo.scrollVelocity * Time.deltaTime;
				eventExecutor.ExecuteHierarchy(gameObject, pointerData, ExecuteEvents.scrollHandler);
			}
		}
		pointerData.scrollDelta = Vector2.zero;
	}

	private ScrollInfo AddScrollHandler(GameObject scrollHandler, Vector2 currentScroll)
	{
		ScrollInfo scrollInfo = new ScrollInfo();
		scrollInfo.initScroll = currentScroll;
		scrollInfo.lastScroll = currentScroll;
		scrollInfo.scrollSettings = scrollHandler.GetComponent<IGvrScrollSettings>();
		scrollHandlers[scrollHandler] = scrollInfo;
		scrollingObjects.Add(scrollHandler);
		return scrollInfo;
	}

	private void RemoveScrollHandler(GameObject scrollHandler)
	{
		if (!object.Equals(scrollHandler, null) && scrollHandlers.ContainsKey(scrollHandler))
		{
			scrollHandlers.Remove(scrollHandler);
			scrollingObjects.Remove(scrollHandler);
		}
	}

	private bool ShouldUseInertia(ScrollInfo scrollInfo)
	{
		if (scrollInfo != null && scrollInfo.scrollSettings != null)
		{
			return scrollInfo.scrollSettings.InertiaOverride;
		}
		return inertia;
	}

	private float GetDecelerationRate(ScrollInfo scrollInfo)
	{
		if (scrollInfo != null && scrollInfo.scrollSettings != null)
		{
			return scrollInfo.scrollSettings.DecelerationRateOverride;
		}
		return decelerationRate;
	}

	private static bool CanScrollStartX(ScrollInfo scrollInfo, Vector2 currentScroll)
	{
		if (scrollInfo == null)
		{
			return false;
		}
		return Mathf.Abs(currentScroll.x - scrollInfo.initScroll.x) >= 150f;
	}

	private static bool CanScrollStartY(ScrollInfo scrollInfo, Vector2 currentScroll)
	{
		if (scrollInfo == null)
		{
			return false;
		}
		return Mathf.Abs(currentScroll.y - scrollInfo.initScroll.y) >= 165f;
	}
}
