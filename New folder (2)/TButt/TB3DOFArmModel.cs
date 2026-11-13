using System;
using UnityEngine;

namespace TButt;

public class TB3DOFArmModel : MonoBehaviour
{
	public enum GazeBehavior
	{
		Never,
		DuringMotion,
		Always
	}

	public delegate void OnArmModelUpdateEvent();

	public static TB3DOFArmModel instance = null;

	private static readonly Vector3 DEFAULT_SHOULDER_RIGHT = new Vector3(0.19f, -0.1f, -0.03f);

	private static readonly Vector3 ELBOW_MIN_RANGE = new Vector3(-0.05f, -0.1f, 0f) * 0.25f;

	private static readonly Vector3 ELBOW_MAX_RANGE = new Vector3(0.05f, 0.1f, 0.2f) * 0.25f;

	private static readonly Vector3 POINTER_OFFSET = new Vector3(0f, -0.009f, 0.099f);

	private static readonly Vector3 ELBOW_POSITION = new Vector3(0.195f, -0.4f, -0.075f);

	private static readonly Vector3 WRIST_POSITION = new Vector3(0f, 0f, 0.25f);

	private static readonly Vector3 ARM_EXTENSION_OFFSET = new Vector3(-0.13f, 0.14f, 0.08f) * 0.25f;

	private const float GRAVITY_CALIB_STRENGTH = 0.999f;

	private const float VELOCITY_FILTER_SUPPRESS = 0.99f;

	private const float LOW_ACCEL_VELOCITY_SUPPRESS = 0.9f;

	private const float LOW_VELOCITY_ACCEL_SUPPRESS = 0.5f;

	private const float MIN_ACCEL = 1f;

	private const float GRAVITY_FORCE = 9.807f;

	private const float DELTA_ALPHA = 4f;

	private const float MIN_EXTENSION_ANGLE = 7f;

	private const float MAX_EXTENSION_ANGLE = 60f;

	private const float EXTENSION_WEIGHT = 0.4f;

	private Vector3 elbowOffset;

	private Vector3 torsoDirection;

	private Vector3 filteredVelocity;

	private Vector3 filteredAccel;

	private Vector3 zeroAccel;

	private bool firstUpdate;

	private Vector3 handedMultiplier;

	[Range(0f, 0.2f)]
	public float addedElbowHeight = 0.02f;

	[Range(0f, 0.2f)]
	public float addedElbowDepth = 0.07f;

	[Range(0f, 30f)]
	public float pointerTiltAngle = 10f;

	[Range(0f, 0.4f)]
	public float fadeDistanceFromFace = 0.32f;

	[Range(0.4f, 0.6f)]
	public float tooltipMinDistanceFromFace = 0.45f;

	[Range(0f, 180f)]
	public int tooltipMaxAngleFromCamera = 80;

	public GazeBehavior followGaze = GazeBehavior.DuringMotion;

	public bool useAccelerometer = true;

	public bool centered;

	private float handednessTimer;

	private bool handednessSetup;

	public Vector3 pointerPosition { get; private set; }

	public Quaternion pointerRotation { get; private set; }

	public Vector3 wristPosition { get; private set; }

	public Quaternion wristRotation { get; private set; }

	public Vector3 elbowPosition { get; private set; }

	public Quaternion elbowRotation { get; private set; }

	public Vector3 shoulderPosition { get; private set; }

	public Quaternion shoulderRotation { get; private set; }

	public float preferredAlpha { get; private set; }

	public float tooltipAlphaValue { get; private set; }

	public event OnArmModelUpdateEvent OnArmModelUpdate;

	public void Initialize()
	{
		TBInput.Mobile3DOFHandedness handedness = TBInput.Get3DOFHandedness();
		SetHandedness(handedness);
		instance = this;
		TBCore.OnUpdate = (TBCore.TBCoreEvent)Delegate.Combine(TBCore.OnUpdate, new TBCore.TBCoreEvent(OnControllerUpdate));
		TBCore.Events.OnTrackingReset = (TBCore.TBCoreEvent)Delegate.Combine(TBCore.Events.OnTrackingReset, new TBCore.TBCoreEvent(ResetState));
		TBCore.Events.OnSystemMenu = (TBCore.Events.TBCoreSystemEvent)Delegate.Combine(TBCore.Events.OnSystemMenu, new TBCore.Events.TBCoreSystemEvent(UpdateHandednessFromSystemMenu));
		firstUpdate = true;
		elbowOffset = Vector3.zero;
		zeroAccel.Set(0f, 9.807f, 0f);
	}

	private void Update()
	{
		if (!handednessSetup)
		{
			TBInput.Mobile3DOFHandedness handedness = TBInput.Get3DOFHandedness(out handednessSetup);
			SetHandedness(handedness);
		}
	}

	private void OnDestroy()
	{
		TBCore.OnUpdate = (TBCore.TBCoreEvent)Delegate.Remove(TBCore.OnUpdate, new TBCore.TBCoreEvent(OnControllerUpdate));
		TBCore.Events.OnTrackingReset = (TBCore.TBCoreEvent)Delegate.Remove(TBCore.Events.OnTrackingReset, new TBCore.TBCoreEvent(ResetState));
		TBCore.Events.OnSystemMenu = (TBCore.Events.TBCoreSystemEvent)Delegate.Remove(TBCore.Events.OnSystemMenu, new TBCore.Events.TBCoreSystemEvent(UpdateHandednessFromSystemMenu));
		instance = null;
	}

	private void OnControllerUpdate()
	{
		UpdateTorsoDirection();
		UpdateFromController();
		if (useAccelerometer)
		{
			UpdateVelocity();
			TransformElbow();
		}
		else
		{
			elbowOffset = Vector3.zero;
		}
		ApplyArmModel();
		UpdateTransparency();
		UpdatePointer();
		firstUpdate = false;
		if (OnArmModelUpdate != null)
		{
			OnArmModelUpdate();
		}
	}

	private void UpdateHandednessFromSystemMenu(bool on)
	{
		if (!centered && on)
		{
			SetHandedness(TBInput.Get3DOFHandedness());
		}
		handednessSetup = false;
	}

	public void SetHandedness(TBInput.Mobile3DOFHandedness handedness)
	{
		handedMultiplier.Set(0f, 1f, 1f);
		switch (handedness)
		{
		case TBInput.Mobile3DOFHandedness.Left:
			handedMultiplier.x = -1f;
			break;
		case TBInput.Mobile3DOFHandedness.Right:
			handedMultiplier.x = 1f;
			break;
		case TBInput.Mobile3DOFHandedness.Center:
			handedMultiplier.x = 0f;
			break;
		}
		shoulderRotation = Quaternion.identity;
		shoulderPosition = Vector3.Scale(DEFAULT_SHOULDER_RIGHT, handedMultiplier);
	}

	private void UpdateTorsoDirection()
	{
		if (followGaze != GazeBehavior.Never)
		{
			Vector3 b = transform.rotation * TBCameraRig.instance.GetCenter().forward;
			b.y = 0f;
			b.Normalize();
			if (followGaze == GazeBehavior.Always || firstUpdate)
			{
				torsoDirection = b;
			}
			else if (followGaze == GazeBehavior.DuringMotion)
			{
				float magnitude = TBInput.GetAngularVelocity(TBInput.Controller.Mobile3DOFController).magnitude;
				float t = Mathf.Clamp((magnitude - 0.2f) / 45f, 0f, 0.1f);
				torsoDirection = Vector3.Slerp(torsoDirection, b, t);
			}
			Quaternion quaternion = (shoulderRotation = Quaternion.FromToRotation(Vector3.forward, torsoDirection));
			shoulderPosition = quaternion * shoulderPosition;
		}
	}

	private void UpdateFromController()
	{
		Vector3 vector = TBInput.GetRotation(TBInput.Controller.Mobile3DOFController) * TBInput.GetAcceleration(TBInput.Controller.Mobile3DOFController);
		zeroAccel = zeroAccel * 0.999f + vector * 0.0009999871f;
		filteredAccel = vector - zeroAccel;
		if (firstUpdate)
		{
			filteredVelocity = Vector3.zero;
		}
		if (filteredAccel.magnitude < 1f)
		{
			filteredAccel = Vector3.zero;
			filteredVelocity *= 0.9f;
		}
		else if ((filteredVelocity + filteredAccel * Time.deltaTime).sqrMagnitude < filteredVelocity.sqrMagnitude)
		{
			filteredAccel *= 0.5f;
		}
	}

	private void UpdateVelocity()
	{
		filteredVelocity += filteredAccel * Time.deltaTime;
		filteredVelocity *= 0.99f;
	}

	private void ResetState()
	{
		filteredVelocity *= 0.5f;
		filteredAccel *= 0.5f;
		firstUpdate = true;
	}

	private void TransformElbow()
	{
		if (useAccelerometer)
		{
			elbowOffset += filteredVelocity * Time.deltaTime;
			ref Vector3 reference = ref elbowOffset;
			float x = elbowOffset.x;
			Vector3 eLBOW_MIN_RANGE = ELBOW_MIN_RANGE;
			float x2 = eLBOW_MIN_RANGE.x;
			Vector3 eLBOW_MAX_RANGE = ELBOW_MAX_RANGE;
			reference.x = Mathf.Clamp(x, x2, eLBOW_MAX_RANGE.x);
			ref Vector3 reference2 = ref elbowOffset;
			float y = elbowOffset.y;
			Vector3 eLBOW_MIN_RANGE2 = ELBOW_MIN_RANGE;
			float y2 = eLBOW_MIN_RANGE2.y;
			Vector3 eLBOW_MAX_RANGE2 = ELBOW_MAX_RANGE;
			reference2.y = Mathf.Clamp(y, y2, eLBOW_MAX_RANGE2.y);
			ref Vector3 reference3 = ref elbowOffset;
			float z = elbowOffset.z;
			Vector3 eLBOW_MIN_RANGE3 = ELBOW_MIN_RANGE;
			float z2 = eLBOW_MIN_RANGE3.z;
			Vector3 eLBOW_MAX_RANGE3 = ELBOW_MAX_RANGE;
			reference3.z = Mathf.Clamp(z, z2, eLBOW_MAX_RANGE3.z);
		}
	}

	private void ApplyArmModel()
	{
		Quaternion quaternion = Quaternion.Inverse(shoulderRotation) * TBInput.GetRotation(TBInput.Controller.Mobile3DOFController);
		elbowPosition = ELBOW_POSITION + new Vector3(0f, addedElbowHeight, addedElbowDepth);
		elbowPosition = Vector3.Scale(elbowPosition, handedMultiplier) + elbowOffset;
		wristPosition = Vector3.Scale(WRIST_POSITION, handedMultiplier);
		Vector3 vector = Vector3.Scale(ARM_EXTENSION_OFFSET, handedMultiplier);
		Vector3 vector2 = quaternion * Vector3.forward;
		float num = 90f - Vector3.Angle(vector2, Vector3.up);
		Quaternion quaternion2 = Quaternion.FromToRotation(Vector3.forward, vector2);
		float value = (num - 7f) / 53f;
		float num2 = Mathf.Clamp(value, 0f, 1f);
		if (!useAccelerometer)
		{
			elbowPosition += vector * num2;
		}
		float num3 = Quaternion.Angle(quaternion2, Quaternion.identity);
		float num4 = 1f - Mathf.Pow(num3 / 180f, 6f);
		float t = num4 * (0.4f + 0.6f * num2 * 0.4f);
		Quaternion rotation = Quaternion.Lerp(Quaternion.identity, quaternion2, t);
		elbowRotation = shoulderRotation * Quaternion.Inverse(rotation) * quaternion;
		wristRotation = shoulderRotation * quaternion;
		elbowPosition = shoulderRotation * elbowPosition;
		wristPosition = elbowPosition + elbowRotation * wristPosition;
	}

	private void UpdateTransparency()
	{
		float num = 4f * Time.deltaTime;
		float num2 = Vector3.Distance(wristPosition, Vector3.zero);
		if (num2 < fadeDistanceFromFace)
		{
			preferredAlpha = Mathf.Max(0f, preferredAlpha - num);
		}
		else
		{
			preferredAlpha = Mathf.Min(1f, preferredAlpha + num);
		}
		float num3 = Vector3.Dot(wristRotation * Vector3.up, -wristPosition.normalized);
		float num4 = ((float)tooltipMaxAngleFromCamera - 90f) / -90f;
		if (num2 < fadeDistanceFromFace || num2 > tooltipMinDistanceFromFace || num3 < num4)
		{
			tooltipAlphaValue = Mathf.Max(0f, tooltipAlphaValue - num);
		}
		else
		{
			tooltipAlphaValue = Mathf.Min(1f, tooltipAlphaValue + num);
		}
	}

	private void UpdatePointer()
	{
		pointerPosition = wristPosition + wristRotation * POINTER_OFFSET;
		pointerRotation = wristRotation * Quaternion.AngleAxis(pointerTiltAngle, Vector3.right);
	}
}
