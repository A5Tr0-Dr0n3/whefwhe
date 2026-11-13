using UnityEngine;

namespace Gvr.Internal;

internal class DummyHeadsetProvider : IHeadsetProvider
{
	private HeadsetState dummyState;

	public bool SupportsPositionalTracking => false;

	public void PollEventState(ref HeadsetState state)
	{
	}

	public bool TryGetFloorHeight(ref float floorHeight)
	{
		return false;
	}

	public bool TryGetRecenterTransform(ref Vector3 position, ref Quaternion rotation)
	{
		return false;
	}

	public bool TryGetSafetyRegionType(ref GvrSafetyRegionType safetyType)
	{
		return false;
	}

	public bool TryGetSafetyCylinderInnerRadius(ref float innerRadius)
	{
		return false;
	}

	public bool TryGetSafetyCylinderOuterRadius(ref float outerRadius)
	{
		return false;
	}
}
