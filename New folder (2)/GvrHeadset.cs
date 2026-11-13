using System;
using System.Collections;
using System.ComponentModel;
using Gvr.Internal;
using UnityEngine;

public class GvrHeadset : MonoBehaviour
{
	public delegate void OnSafetyRegionEvent(bool enter);

	public delegate void OnRecenterEvent(GvrRecenterEventType recenterType, GvrRecenterFlags recenterFlags, Vector3 recenteredPosition, Quaternion recenteredOrientation);

	private static GvrHeadset instance;

	private IHeadsetProvider headsetProvider;

	private HeadsetState headsetState;

	private IEnumerator standaloneUpdate;

	private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

	private OnSafetyRegionEvent safetyRegionDelegate;

	private OnRecenterEvent recenterDelegate;

	public static bool SupportsPositionalTracking
	{
		get
		{
			if (instance == null)
			{
				return false;
			}
			return instance.headsetProvider.SupportsPositionalTracking;
		}
	}

	public static event OnSafetyRegionEvent OnSafetyRegionChange
	{
		add
		{
			if (instance != null)
			{
				GvrHeadset gvrHeadset = instance;
				gvrHeadset.safetyRegionDelegate = (OnSafetyRegionEvent)Delegate.Combine(gvrHeadset.safetyRegionDelegate, value);
			}
		}
		remove
		{
			if (instance != null)
			{
				GvrHeadset gvrHeadset = instance;
				gvrHeadset.safetyRegionDelegate = (OnSafetyRegionEvent)Delegate.Remove(gvrHeadset.safetyRegionDelegate, value);
			}
		}
	}

	public static event OnRecenterEvent OnRecenter
	{
		add
		{
			if (instance != null)
			{
				GvrHeadset gvrHeadset = instance;
				gvrHeadset.recenterDelegate = (OnRecenterEvent)Delegate.Combine(gvrHeadset.recenterDelegate, value);
			}
		}
		remove
		{
			if (instance != null)
			{
				GvrHeadset gvrHeadset = instance;
				gvrHeadset.recenterDelegate = (OnRecenterEvent)Delegate.Remove(gvrHeadset.recenterDelegate, value);
			}
		}
	}

	private GvrHeadset()
	{
		headsetState.Initialize();
	}

	public static bool TryGetFloorHeight(ref float floorHeight)
	{
		if (instance == null)
		{
			return false;
		}
		return instance.headsetProvider.TryGetFloorHeight(ref floorHeight);
	}

	public static bool TryGetRecenterTransform(ref Vector3 position, ref Quaternion rotation)
	{
		if (instance == null)
		{
			return false;
		}
		return instance.headsetProvider.TryGetRecenterTransform(ref position, ref rotation);
	}

	public static bool TryGetSafetyRegionType(ref GvrSafetyRegionType safetyType)
	{
		if (instance == null)
		{
			return false;
		}
		return instance.headsetProvider.TryGetSafetyRegionType(ref safetyType);
	}

	public static bool TryGetSafetyCylinderInnerRadius(ref float innerRadius)
	{
		if (instance == null)
		{
			return false;
		}
		return instance.headsetProvider.TryGetSafetyCylinderInnerRadius(ref innerRadius);
	}

	public static bool TryGetSafetyCylinderOuterRadius(ref float outerRadius)
	{
		if (instance == null)
		{
			return false;
		}
		return instance.headsetProvider.TryGetSafetyCylinderOuterRadius(ref outerRadius);
	}

	private void Awake()
	{
		if (instance != null)
		{
			Debug.LogError("More than one GvrHeadset instance was found in your scene. Ensure that there is only one GvrHeadset.");
			enabled = false;
			return;
		}
		instance = this;
		if (headsetProvider == null)
		{
			headsetProvider = HeadsetProviderFactory.CreateProvider();
		}
	}

	private void OnEnable()
	{
		if (SupportsPositionalTracking)
		{
			standaloneUpdate = EndOfFrame();
			StartCoroutine(standaloneUpdate);
		}
	}

	private void OnDisable()
	{
		if (SupportsPositionalTracking)
		{
			StopCoroutine(standaloneUpdate);
		}
	}

	private void OnDestroy()
	{
		if (SupportsPositionalTracking)
		{
			instance = null;
		}
	}

	private void UpdateStandalone()
	{
		headsetProvider.PollEventState(ref headsetState);
		while (headsetState.eventType != GvrEventType.Invalid)
		{
			switch (headsetState.eventType)
			{
			case GvrEventType.Recenter:
				if (recenterDelegate != null)
				{
					recenterDelegate(headsetState.recenterEventType, (GvrRecenterFlags)headsetState.recenterEventFlags, headsetState.recenteredPosition, headsetState.recenteredRotation);
				}
				break;
			case GvrEventType.SafetyRegionEnter:
				if (safetyRegionDelegate != null)
				{
					safetyRegionDelegate(enter: true);
				}
				break;
			case GvrEventType.SafetyRegionExit:
				if (safetyRegionDelegate != null)
				{
					safetyRegionDelegate(enter: false);
				}
				break;
			case GvrEventType.Invalid:
				throw new InvalidEnumArgumentException("Invalid headset event: " + headsetState.eventType);
			}
			headsetProvider.PollEventState(ref headsetState);
		}
	}

	private IEnumerator EndOfFrame()
	{
		while (true)
		{
			yield return waitForEndOfFrame;
			UpdateStandalone();
		}
	}
}
