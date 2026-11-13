using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GvrLaserVisual : MonoBehaviour, IGvrArmModelReceiver
{
	public delegate Vector3 GetPointForDistanceDelegate(float distance);

	[Tooltip("Used to position the reticle at the current position.")]
	public GvrControllerReticleVisual reticle;

	[Tooltip("Used to rotate the controller to face the current position.")]
	public Transform controller;

	[Tooltip("Start color of the laser pointer including alpha transparency.")]
	public Color laserColor = new Color(1f, 1f, 1f, 0.25f);

	[Tooltip("End color of the laser pointer including alpha transparency.")]
	public Color laserColorEnd = new Color(1f, 1f, 1f, 0f);

	[Tooltip("Maximum distance of the laser (meters).")]
	[Range(0f, 20f)]
	public float maxLaserDistance = 1f;

	[Tooltip("The rate that the current position moves towards the target position.")]
	public float lerpSpeed = 20f;

	[Tooltip("If the target position is greater than this threshold, then the position changes immediately instead of lerping.")]
	public float lerpThreshold = 1f;

	[Tooltip("Determines if the laser will shrink when it isn't facing in the forward direction of the transform.")]
	public bool shrinkLaser = true;

	[Range(0f, 1f)]
	[Tooltip("Amount to shrink the laser when it is fully shrunk.")]
	public float shrunkScale = 0.2f;

	[Range(0f, 15f)]
	[Tooltip("Begin shrinking the laser when the angle between transform.forward and the reticle is greater than this value.")]
	public float beginShrinkAngleDegrees;

	[Range(0f, 15f)]
	[Tooltip("Finish shrinking the laser when the angle between transform.forward and the reticle is greater than this value.")]
	public float endShrinkAngleDegrees = 2f;

	private const float LERP_CLAMP_THRESHOLD = 0.02f;

	protected float shrinkRatio;

	protected float targetDistance;

	protected float currentDistance;

	protected Vector3 currentPosition;

	protected Vector3 currentLocalPosition;

	protected Quaternion currentLocalRotation;

	public GvrBaseArmModel ArmModel { get; set; }

	public LineRenderer Laser { get; private set; }

	public GetPointForDistanceDelegate GetPointForDistanceFunction { get; set; }

	public virtual void SetDistance(float distance, bool immediate = false)
	{
		targetDistance = distance;
		if (immediate)
		{
			currentDistance = targetDistance;
		}
		if (targetDistance > lerpThreshold)
		{
			currentDistance = targetDistance;
		}
	}

	protected virtual void Awake()
	{
		Laser = GetComponent<LineRenderer>();
	}

	protected virtual void LateUpdate()
	{
		UpdateCurrentPosition();
		UpdateControllerOrientation();
		UpdateReticlePosition();
		UpdateLaserEndPoint();
		UpdateLaserAlpha();
	}

	protected virtual void UpdateCurrentPosition()
	{
		if (currentDistance != targetDistance)
		{
			float speed = GetSpeed();
			currentDistance = Mathf.Lerp(currentDistance, targetDistance, speed);
			float num = Mathf.Abs(targetDistance - currentDistance);
			if (num < 0.02f)
			{
				currentDistance = targetDistance;
			}
		}
		if (GetPointForDistanceFunction != null)
		{
			currentPosition = GetPointForDistanceFunction(currentDistance);
		}
		else
		{
			Vector3 position = transform.position;
			currentPosition = position + transform.forward * currentDistance;
		}
		currentLocalPosition = transform.InverseTransformPoint(currentPosition);
		currentLocalRotation = Quaternion.FromToRotation(Vector3.forward, currentLocalPosition);
	}

	protected virtual void UpdateControllerOrientation()
	{
		if (!(controller == null))
		{
			controller.localRotation = currentLocalRotation;
		}
	}

	protected virtual void UpdateReticlePosition()
	{
		if (!(reticle == null))
		{
			reticle.transform.position = currentPosition;
		}
	}

	protected virtual void UpdateLaserEndPoint()
	{
		if (!(Laser == null))
		{
			Vector3 vector = Vector3.zero;
			if (controller != null)
			{
				Vector3 position = transform.position;
				Vector3 vector2 = controller.InverseTransformPoint(position);
				vector2 = currentLocalRotation * vector2;
				vector = controller.TransformPoint(vector2);
				vector = transform.InverseTransformPoint(vector);
			}
			Vector3 vector3 = Vector3.ClampMagnitude(currentLocalPosition, maxLaserDistance);
			if (shrinkLaser)
			{
				float num = Vector3.Angle(Vector3.forward, currentLocalPosition);
				float num2 = endShrinkAngleDegrees - beginShrinkAngleDegrees;
				float num3 = Mathf.Clamp(num - beginShrinkAngleDegrees, 0f, num2);
				shrinkRatio = num3 / num2;
				float num4 = GvrMathHelpers.EaseOutCubic(shrunkScale, 1f, 1f - shrinkRatio);
				Vector3 vector4 = vector - currentLocalPosition;
				Vector3 normalized = vector4.normalized;
				float num5 = Mathf.Min(vector4.magnitude, maxLaserDistance) * num4;
				vector3 = currentLocalPosition;
				vector = vector3 + normalized * num5;
			}
			Laser.useWorldSpace = false;
			Laser.SetPosition(0, vector);
			Laser.SetPosition(1, vector3);
		}
	}

	protected virtual void UpdateLaserAlpha()
	{
		float t = ((!(ArmModel != null)) ? 1f : ArmModel.PreferredAlpha);
		Color color = Color.Lerp(Color.clear, laserColor, t);
		Color color2 = laserColorEnd;
		if (shrinkLaser)
		{
			float t2 = GvrMathHelpers.EaseOutCubic(0f, 1f, shrinkRatio);
			color2 = Color.Lerp(color2, color, t2);
			color = Color.Lerp(color, laserColorEnd, t2);
		}
		Laser.startColor = color;
		Laser.endColor = color2;
	}

	protected virtual float GetSpeed()
	{
		return (!(lerpSpeed > 0f)) ? 1f : (lerpSpeed * Time.unscaledDeltaTime);
	}
}
