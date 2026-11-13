using proto;
using UnityEngine;

namespace Gvr.Internal;

internal struct EmulatorOrientationEvent(PhoneEvent.Types.OrientationEvent proto)
{
	public readonly long timestamp = proto.Timestamp;

	public readonly Quaternion orientation = new Quaternion(proto.X, proto.Y, 0f - proto.Z, proto.W);
}
