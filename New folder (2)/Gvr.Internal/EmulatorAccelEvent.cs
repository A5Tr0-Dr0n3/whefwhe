using proto;
using UnityEngine;

namespace Gvr.Internal;

internal struct EmulatorAccelEvent(PhoneEvent.Types.AccelerometerEvent proto)
{
	public readonly long timestamp = proto.Timestamp;

	public readonly Vector3 value = new Vector3(proto.X, proto.Y, proto.Z);
}
