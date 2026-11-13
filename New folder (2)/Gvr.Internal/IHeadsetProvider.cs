using UnityEngine;

namespace Gvr.Internal;

internal interface IHeadsetProvider
{
	bool SupportsPositionalTracking { get; }

	void PollEventState(ref HeadsetState outState);

	bool TryGetFloorHeight(ref float floorHeight);

	bool TryGetRecenterTransform(ref Vector3 position, ref Quaternion rotation);

	bool TryGetSafetyRegionType(ref GvrSafetyRegionType safetyType);

	bool TryGetSafetyCylinderInnerRadius(ref float innerRadius);

	bool TryGetSafetyCylinderOuterRadius(ref float outerRadius);
}
