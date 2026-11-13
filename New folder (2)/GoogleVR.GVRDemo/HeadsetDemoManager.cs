using System.Collections;
using UnityEngine;

namespace GoogleVR.GVRDemo;

public class HeadsetDemoManager : MonoBehaviour
{
	public bool enableDebugLog = true;

	private WaitForSeconds waitFourSeconds = new WaitForSeconds(4f);

	public void OnSafetyRegionEvent(bool enter)
	{
		Debug.Log("SafetyRegionEvent: " + ((!enter) ? "exit" : "enter"));
	}

	public void OnRecenterEvent(GvrRecenterEventType recenterType, GvrRecenterFlags recenterFlags, Vector3 recenteredPosition, Quaternion recenteredOrientation)
	{
		Debug.Log($"RecenterEvent: Type {recenterType}, flags {recenterFlags}\nPosition: {recenteredPosition}, Rotation: {recenteredOrientation}");
	}

	public void FindFloorHeight()
	{
		float floorHeight = 0f;
		bool flag = GvrHeadset.TryGetFloorHeight(ref floorHeight);
		Debug.Log("Floor height success " + flag + "; value " + floorHeight);
	}

	public void FindRecenterTransform()
	{
		Vector3 position = Vector3.zero;
		Quaternion rotation = Quaternion.identity;
		bool flag = GvrHeadset.TryGetRecenterTransform(ref position, ref rotation);
		Debug.Log(string.Concat("Recenter transform success ", flag, "; value ", position, "; ", rotation));
	}

	public void FindSafetyRegionType()
	{
		GvrSafetyRegionType safetyType = GvrSafetyRegionType.None;
		bool flag = GvrHeadset.TryGetSafetyRegionType(ref safetyType);
		Debug.Log("Safety region type success " + flag + "; value " + safetyType);
	}

	public void FindSafetyInnerRadius()
	{
		float innerRadius = -1f;
		bool flag = GvrHeadset.TryGetSafetyCylinderInnerRadius(ref innerRadius);
		Debug.Log("Safety region inner radius success " + flag + "; value " + innerRadius);
	}

	public void FindSafetyOuterRadius()
	{
		float outerRadius = -1f;
		bool flag = GvrHeadset.TryGetSafetyCylinderOuterRadius(ref outerRadius);
		Debug.Log("Safety region outer radius success " + flag + "; value " + outerRadius);
	}

	private void OnEnable()
	{
		if (GvrHeadset.SupportsPositionalTracking)
		{
			GvrHeadset.OnSafetyRegionChange += OnSafetyRegionEvent;
			GvrHeadset.OnRecenter += OnRecenterEvent;
			if (enableDebugLog)
			{
				StartCoroutine(StatusUpdateLoop());
			}
		}
	}

	private void OnDisable()
	{
		if (GvrHeadset.SupportsPositionalTracking)
		{
			GvrHeadset.OnSafetyRegionChange -= OnSafetyRegionEvent;
			GvrHeadset.OnRecenter -= OnRecenterEvent;
		}
	}

	private void Start()
	{
		if (GvrHeadset.SupportsPositionalTracking)
		{
			Debug.Log("Device supports positional tracking!");
		}
	}

	private IEnumerator StatusUpdateLoop()
	{
		while (true)
		{
			yield return waitFourSeconds;
			FindFloorHeight();
			FindRecenterTransform();
			FindSafetyOuterRadius();
			FindSafetyInnerRadius();
			FindSafetyRegionType();
		}
	}
}
