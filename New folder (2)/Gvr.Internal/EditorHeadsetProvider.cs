using UnityEngine;

namespace Gvr.Internal;

internal class EditorHeadsetProvider : IHeadsetProvider
{
	private HeadsetState dummyState;

	public bool SupportsPositionalTracking => true;

	public void PollEventState(ref HeadsetState state)
	{
	}

	public bool TryGetFloorHeight(ref float floorHeight)
	{
		floorHeight = -1.6f;
		return true;
	}

	public bool TryGetRecenterTransform(ref Vector3 position, ref Quaternion rotation)
	{
		return true;
	}

	public bool TryGetSafetyRegionType(ref GvrSafetyRegionType safetyType)
	{
		safetyType = GvrSafetyRegionType.Cylinder;
		return true;
	}

	public bool TryGetSafetyCylinderInnerRadius(ref float innerRadius)
	{
		innerRadius = 0.6f;
		return true;
	}

	public bool TryGetSafetyCylinderOuterRadius(ref float outerRadius)
	{
		outerRadius = 0.7f;
		return true;
	}
}
