using UnityEngine;

public abstract class GvrBaseArmModel : MonoBehaviour
{
	public abstract Vector3 ControllerPositionFromHead { get; }

	public abstract Quaternion ControllerRotationFromHead { get; }

	public abstract float PreferredAlpha { get; }

	public abstract float TooltipAlphaValue { get; }
}
