using UnityEngine;

namespace Gvr.Internal;

internal struct HeadsetState
{
	internal GvrEventType eventType;

	internal int eventFlags;

	internal long eventTimestampNs;

	internal GvrRecenterEventType recenterEventType;

	internal uint recenterEventFlags;

	internal Vector3 recenteredPosition;

	internal Quaternion recenteredRotation;

	public void Initialize()
	{
		eventType = GvrEventType.Invalid;
		eventFlags = 0;
		eventTimestampNs = 0L;
		recenterEventType = GvrRecenterEventType.Invalid;
		recenterEventFlags = 0u;
		recenteredPosition = Vector3.zero;
		recenteredRotation = Quaternion.identity;
	}
}
