using proto;

namespace Gvr.Internal;

internal struct EmulatorButtonEvent(PhoneEvent.Types.KeyEvent proto)
{
	public enum ButtonCode
	{
		kNone = 0,
		kHome = 3,
		kVolumeUp = 25,
		kVolumeDown = 24,
		kClick = 66,
		kApp = 82
	}

	public readonly ButtonCode code = (ButtonCode)proto.Code;

	public readonly bool down = proto.Action == 0;
}
