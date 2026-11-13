using UnityEngine;

public class GvrArmModel : GvrBaseArmModel
{
	public Vector3 elbowRestPosition = DEFAULT_ELBOW_REST_POSITION;

	public Vector3 wristRestPosition = DEFAULT_WRIST_REST_POSITION;

	public Vector3 controllerRestPosition = DEFAULT_CONTROLLER_REST_POSITION;

	public Vector3 armExtensionOffset = DEFAULT_ARM_EXTENSION_OFFSET;

	[Range(0f, 1f)]
	public float elbowBendRatio = 0.6f;

	[Range(0f, 0.4f)]
	public float fadeControllerOffset;

	[Range(0f, 0.4f)]
	public float fadeDistanceFromHeadForward = 0.25f;

	[Range(0f, 0.4f)]
	public float fadeDistanceFromHeadSide = 0.15f;

	[Range(0.4f, 0.6f)]
	public float tooltipMinDistanceFromFace = 0.45f;

	[Range(0f, 180f)]
	public int tooltipMaxAngleFromCamera = 80;

	public bool isLockedToNeck;

	protected Vector3 neckPosition;

	protected Vector3 elbowPosition;

	protected Quaternion elbowRotation;

	protected Vector3 wristPosition;

	protected Quaternion wristRotation;

	protected Vector3 controllerPosition;

	protected Quaternion controllerRotation;

	protected float preferredAlpha;

	protected float tooltipAlphaValue;

	protected Vector3 handedMultiplier;

	protected Vector3 torsoDirection;

	protected Quaternion torsoRotation;

	public static readonly Vector3 DEFAULT_ELBOW_REST_POSITION = new Vector3(0.195f, -0.5f, 0.005f);

	public static readonly Vector3 DEFAULT_WRIST_REST_POSITION = new Vector3(0f, 0f, 0.25f);

	public static readonly Vector3 DEFAULT_CONTROLLER_REST_POSITION = new Vector3(0f, 0f, 0.05f);

	public static readonly Vector3 DEFAULT_ARM_EXTENSION_OFFSET = new Vector3(-0.13f, 0.14f, 0.08f);

	public const float DEFAULT_ELBOW_BEND_RATIO = 0.6f;

	protected const float EXTENSION_WEIGHT = 0.4f;

	protected static readonly Vector3 SHOULDER_POSITION = new Vector3(0.17f, -0.2f, -0.03f);

	protected static readonly Vector3 NECK_OFFSET = new Vector3(0f, 0.075f, 0.08f);

	protected const float DELTA_ALPHA = 4f;

	protected const float MIN_EXTENSION_ANGLE = 7f;

	protected const float MAX_EXTENSION_ANGLE = 60f;

	public override Vector3 ControllerPositionFromHead => controllerPosition;

	public override Quaternion ControllerRotationFromHead => controllerRotation;

	public override float PreferredAlpha => preferredAlpha;

	public override float TooltipAlphaValue => tooltipAlphaValue;

	public Vector3 NeckPosition => neckPosition;

	public Vector3 ShoulderPosition => neckPosition + torsoRotation * Vector3.Scale(SHOULDER_POSITION, handedMultiplier);

	public Quaternion ShoulderRotation => torsoRotation;

	public Vector3 ElbowPosition => elbowPosition;

	public Quaternion ElbowRotation => elbowRotation;

	public Vector3 WristPosition => wristPosition;

	public Quaternion WristRotation => wristRotation;

	protected virtual void OnEnable()
	{
		GvrControllerInput.OnControllerInputUpdated += OnControllerInputUpdated;
		UpdateTorsoDirection(forceImmediate: true);
		OnControllerInputUpdated();
	}

	protected virtual void OnDisable()
	{
		GvrControllerInput.OnControllerInputUpdated -= OnControllerInputUpdated;
	}

	protected virtual void OnControllerInputUpdated()
	{
		UpdateHandedness();
		UpdateTorsoDirection(forceImmediate: false);
		UpdateNeckPosition();
		ApplyArmModel();
		UpdateTransparency();
	}

	protected virtual void UpdateHandedness()
	{
		GvrSettings.UserPrefsHandedness handedness = GvrSettings.Handedness;
		handedMultiplier.Set(0f, 1f, 1f);
		switch (handedness)
		{
		case GvrSettings.UserPrefsHandedness.Right:
			handedMultiplier.x = 1f;
			break;
		case GvrSettings.UserPrefsHandedness.Left:
			handedMultiplier.x = -1f;
			break;
		}
	}

	protected virtual void UpdateTorsoDirection(bool forceImmediate)
	{
		Vector3 headForward = GvrVRHelpers.GetHeadForward();
		headForward.y = 0f;
		headForward.Normalize();
		if (GvrControllerInput.Recentered || forceImmediate)
		{
			torsoDirection = headForward;
		}
		else
		{
			float magnitude = GvrControllerInput.Gyro.magnitude;
			float t = Mathf.Clamp((magnitude - 0.2f) / 45f, 0f, 0.1f);
			torsoDirection = Vector3.Slerp(torsoDirection, headForward, t);
		}
		torsoRotation = Quaternion.FromToRotation(Vector3.forward, torsoDirection);
	}

	protected virtual void UpdateNeckPosition()
	{
		if (isLockedToNeck)
		{
			neckPosition = GvrVRHelpers.GetHeadPosition();
			neckPosition = ApplyInverseNeckModel(neckPosition);
		}
		else
		{
			neckPosition = Vector3.zero;
		}
	}

	protected virtual void ApplyArmModel()
	{
		SetUntransformedJointPositions();
		GetControllerRotation(out var rotation, out var xyRotation, out var xAngle);
		float extensionRatio = CalculateExtensionRatio(xAngle);
		ApplyExtensionOffset(extensionRatio);
		Quaternion lerpRotation = CalculateLerpRotation(xyRotation, extensionRatio);
		CalculateFinalJointRotations(rotation, xyRotation, lerpRotation);
		ApplyRotationToJoints();
	}

	protected virtual void SetUntransformedJointPositions()
	{
		elbowPosition = Vector3.Scale(elbowRestPosition, handedMultiplier);
		wristPosition = Vector3.Scale(wristRestPosition, handedMultiplier);
		controllerPosition = Vector3.Scale(controllerRestPosition, handedMultiplier);
	}

	protected virtual float CalculateExtensionRatio(float xAngle)
	{
		float value = (xAngle - 7f) / 53f;
		return Mathf.Clamp(value, 0f, 1f);
	}

	protected virtual void ApplyExtensionOffset(float extensionRatio)
	{
		Vector3 vector = Vector3.Scale(armExtensionOffset, handedMultiplier);
		elbowPosition += vector * extensionRatio;
	}

	protected virtual Quaternion CalculateLerpRotation(Quaternion xyRotation, float extensionRatio)
	{
		float num = Quaternion.Angle(xyRotation, Quaternion.identity);
		float num2 = 1f - Mathf.Pow(num / 180f, 6f);
		float num3 = 1f - elbowBendRatio;
		float num4 = num3 + elbowBendRatio * extensionRatio * 0.4f;
		num4 *= num2;
		return Quaternion.Lerp(Quaternion.identity, xyRotation, num4);
	}

	protected virtual void CalculateFinalJointRotations(Quaternion controllerOrientation, Quaternion xyRotation, Quaternion lerpRotation)
	{
		elbowRotation = torsoRotation * Quaternion.Inverse(lerpRotation) * xyRotation;
		wristRotation = elbowRotation * lerpRotation;
		controllerRotation = torsoRotation * controllerOrientation;
	}

	protected virtual void ApplyRotationToJoints()
	{
		elbowPosition = neckPosition + torsoRotation * elbowPosition;
		wristPosition = elbowPosition + elbowRotation * wristPosition;
		controllerPosition = wristPosition + wristRotation * controllerPosition;
	}

	protected virtual Vector3 ApplyInverseNeckModel(Vector3 headPosition)
	{
		Quaternion headRotation = GvrVRHelpers.GetHeadRotation();
		Vector3 vector = headRotation * NECK_OFFSET;
		Vector3 nECK_OFFSET = NECK_OFFSET;
		Vector3 vector2 = vector - nECK_OFFSET.y * Vector3.up;
		headPosition -= vector2;
		return headPosition;
	}

	protected virtual void UpdateTransparency()
	{
		Vector3 vector = controllerRotation * Vector3.forward;
		Vector3 vector2 = controllerPosition + vector * fadeControllerOffset;
		Vector3 a = vector2 - neckPosition;
		Vector3 headForward = GvrVRHelpers.GetHeadForward();
		float magnitude = Vector3.Scale(a, headForward).magnitude;
		Vector3 b = Vector3.Cross(headForward, Vector3.up);
		float magnitude2 = Vector3.Scale(a, b).magnitude;
		float num = Mathf.Abs(a.y);
		bool flag = magnitude < fadeDistanceFromHeadForward && num < fadeDistanceFromHeadForward && magnitude2 < fadeDistanceFromHeadSide;
		float num2 = 4f * Time.unscaledDeltaTime;
		if (flag)
		{
			preferredAlpha = Mathf.Max(0f, preferredAlpha - num2);
		}
		else
		{
			preferredAlpha = Mathf.Min(1f, preferredAlpha + num2);
		}
		float num3 = Vector3.Dot(controllerRotation * Vector3.up, -a.normalized);
		float num4 = ((float)tooltipMaxAngleFromCamera - 90f) / -90f;
		float num5 = Vector3.Distance(a, Vector3.zero);
		if (flag || num5 > tooltipMinDistanceFromFace || num3 < num4)
		{
			tooltipAlphaValue = Mathf.Max(0f, tooltipAlphaValue - num2);
		}
		else
		{
			tooltipAlphaValue = Mathf.Min(1f, tooltipAlphaValue + num2);
		}
	}

	protected void GetControllerRotation(out Quaternion rotation, out Quaternion xyRotation, out float xAngle)
	{
		rotation = GvrControllerInput.Orientation;
		rotation = Quaternion.Inverse(torsoRotation) * rotation;
		Vector3 vector = rotation * Vector3.forward;
		xAngle = 90f - Vector3.Angle(vector, Vector3.up);
		xyRotation = Quaternion.FromToRotation(Vector3.forward, vector);
	}
}
