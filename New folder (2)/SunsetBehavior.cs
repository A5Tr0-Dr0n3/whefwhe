using System;
using System.Collections;
using GVR.Gfx;
using TButt;
using UnityEngine;

[RequireComponent(typeof(ObjectSun))]
public class SunsetBehavior : MonoBehaviour
{
	[Header("References")]
	[SerializeField]
	public EnvironmentProperties environment;

	[SerializeField]
	public VVR_Point sunPointLight;

	[SerializeField]
	public FireFlicker firePointLight;

	[SerializeField]
	private DesktopDirectionalLight sunDesktopLight;

	[SerializeField]
	private DesktopFireFlicker fireDesktopLight;

	[SerializeField]
	public Material[] stars;

	[SerializeField]
	public Transform directionLight;

	[SerializeField]
	public Transform pathCenter;

	[SerializeField]
	private Transform topOfSun;

	[SerializeField]
	private ParticleSystem fireParticles;

	private ObjectSun sunObject;

	[Header("Tweakables")]
	[SerializeField]
	private float sunsetSpeed = 0.5f;

	[SerializeField]
	private AnimationCurve angrySequence;

	private float sequenceT;

	private float dot;

	private float radiusFromCenter;

	private float lastDot = -1f;

	private float positionValue;

	private float sunsetTheta;

	private Vector3 centerPoint;

	private float sunsetXAxis;

	private bool isInOrbit = true;

	public bool isAngry;

	private bool isMoving;

	[Header("Environment Fog:")]
	public float fullOpacityDistanceMax = 50f;

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve fullOpacityDistance;

	public Gradient nearHorizonColor;

	public Gradient farHorizonColor;

	public float horizonColorPivotDistanceMax = 5f;

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve horizonColorPivotDistance;

	public float horizonColorBlendLengthMax = 5f;

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve horizonColorBlendLength;

	public Gradient nearApexColor;

	public Gradient farApexColor;

	public float apexFullOpacityDistanceMax = 250f;

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve apexFullOpacityDistance;

	public float apexColorPivotDistanceMax = 250f;

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve apexColorPivotDistance;

	public float apexColorBlendLengthMax = 400f;

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve apexColorBlendLength;

	[Header("Environment Lighting:")]
	public Gradient sceneLightColor;

	public Gradient sceneShadowColor;

	[Header("Sun Point Light:")]
	public float sunIntensityMax = 50f;

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve sunIntensity;

	public float sunRangeMax = 200f;

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve sunRange;

	[Header("Bonfire Point Light:")]
	public float fireIntensityMax = 50f;

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve fireIntensity;

	public float fireRangeMax = 200f;

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve fireRange;

	private float lerpNewVariablesTime;

	private float lerpNewVariablesCount;

	private float t;

	private void Awake()
	{
		init();
	}

	protected void Update()
	{
		updateSunVisuals();
	}

	protected void FixedUpdate()
	{
		if (isInOrbit)
		{
			if (!isAngry)
			{
				processSunsetPath();
			}
			else
			{
				processSunsetPath(forceSnap: true);
			}
		}
		if (isAngry)
		{
			isInOrbit = true;
			StepAngrySequence();
		}
		if (!isInOrbit && !sunObject.isGrabbed)
		{
			resetOutOfBounds();
		}
	}

	private void processSunsetPath(bool forceSnap = false)
	{
		if (!isAngry)
		{
			sunsetTheta -= Time.fixedDeltaTime * sunsetSpeed * (float)Math.PI;
		}
		Vector2 vector = findPointOnCircle(sunsetTheta, radiusFromCenter, centerPoint);
		Vector3 b = new Vector3(sunsetXAxis, vector.y, vector.x);
		if (!forceSnap)
		{
			transform.position = Vector3.Slerp(transform.position, b, 3f * Time.deltaTime);
		}
		else
		{
			transform.position = Vector3.Slerp(transform.position, b, 0.5f);
		}
	}

	public void MoveSun(Vector3 newPos, float time, bool force = false)
	{
		if (!isInOrbit || force)
		{
			isMoving = true;
			GlobalCoroutineContainer.AddThread(MoveSunCoroutine(newPos, time), this);
		}
		else
		{
			SafeDebug.LogWarning("Sun is in orbit not and/or letting anyone else move it");
		}
	}

	public IEnumerator MoveSunCoroutine(Vector3 newPos, float speed)
	{
		float currentLerpTime = 0f;
		while (Mathf.Abs(Vector3.Distance(transform.position, newPos)) > 0.03f && isMoving)
		{
			if (sunObject.isGrabbed)
			{
				isMoving = false;
				break;
			}
			currentLerpTime = Mathf.MoveTowards(currentLerpTime, speed, Time.deltaTime);
			Vector3 targetPos = Vector3.MoveTowards(transform.position, newPos, currentLerpTime);
			transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.5f);
			yield return null;
		}
		isMoving = false;
	}

	public void StepAngrySequence()
	{
		isInOrbit = true;
		isAngry = true;
		sequenceT += Time.deltaTime;
		sunsetTheta = angrySequence.Evaluate(sequenceT);
	}

	private void resetOutOfBounds()
	{
		float num = Vector3.Distance(transform.position, EagerMaster.instance.mainCamera.transform.position);
		if (num >= 100f)
		{
			Vector3 newPos = Vector3.Slerp(transform.position, EagerMaster.instance.mainCamera.transform.position, 0.1f);
			if (!isMoving)
			{
				MoveSun(newPos, 0.15f);
			}
		}
		else if (transform.position.y <= -2f && !sunObject.isGrabbed && !isMoving && topOfSun.position.y < 5f)
		{
			Vector3 position = transform.position;
			position.y += 0.1f;
			MoveSun(position, 0.1f);
		}
	}

	private void updateSunVisuals()
	{
		dot = Vector3.Dot(Vector3.up, sunPointLight.transform.position.normalized);
		if (Mathf.Abs(dot - lastDot) > 1E-05f)
		{
			positionValue = MapValue(Mathf.Clamp(dot, -0.4f, 1f), -0.4f, 1f, 1f, 0f);
			updateSunsettings(positionValue);
		}
		lastDot = dot;
		directionLight.rotation = Quaternion.LookRotation(-transform.position);
	}

	private void updateSunsettings(float val)
	{
		environment.fullOpacityDistance = fullOpacityDistanceMax * fullOpacityDistance.Evaluate(val);
		environment.nearHorizonColor = nearHorizonColor.Evaluate(val);
		environment.farHorizonColor = farHorizonColor.Evaluate(val);
		environment.horizonColorPivotDistance = horizonColorPivotDistanceMax * horizonColorPivotDistance.Evaluate(val);
		environment.horizonColorBlendLength = horizonColorBlendLengthMax * horizonColorBlendLength.Evaluate(val);
		environment.nearApexColor = nearApexColor.Evaluate(val);
		environment.farApexColor = farApexColor.Evaluate(val);
		environment.apexFullOpacityDistance = apexFullOpacityDistanceMax * apexFullOpacityDistance.Evaluate(val);
		environment.apexColorPivotDistance = apexColorPivotDistanceMax * apexColorPivotDistance.Evaluate(val);
		environment.apexColorBlendLength = apexColorBlendLengthMax * apexColorBlendLength.Evaluate(val);
		environment.sceneLightColor = sceneLightColor.Evaluate(val);
		environment.sceneShadowColor = sceneShadowColor.Evaluate(val);
		sunPointLight.intensity = sunIntensityMax * sunIntensity.Evaluate(val);
		sunPointLight.range = sunRangeMax * sunRange.Evaluate(val);
		if (TBCore.GetActivePlatform() != VRPlatform.OculusMobile && sunDesktopLight != null)
		{
			sunDesktopLight.mLight.intensity = sunIntensityMax * sunIntensity.Evaluate(val);
			sunDesktopLight.mLight.range = sunRangeMax * sunRange.Evaluate(val);
		}
		firePointLight.baseIntensity = fireIntensityMax * fireIntensity.Evaluate(val);
		firePointLight.baseRange = fireRangeMax * fireRange.Evaluate(val);
		if (TBCore.GetActivePlatform() != VRPlatform.OculusMobile)
		{
			fireDesktopLight.startIntensity = fireIntensityMax * fireIntensity.Evaluate(val);
			fireDesktopLight.startRange = fireRangeMax * fireRange.Evaluate(val);
			if (firePointLight.baseIntensity > 0f && !fireParticles.emission.enabled)
			{
				ParticleSystem.EmissionModule emission = fireParticles.emission;
				emission.enabled = true;
			}
			else
			{
				ParticleSystem.EmissionModule emission2 = fireParticles.emission;
				emission2.enabled = false;
			}
		}
		environment.pushSettings();
	}

	public float MapValue(float oldVal, float oldMin, float oldMax, float newMin, float newMax)
	{
		return (oldVal - oldMin) * (newMax - newMin) / (oldMax - oldMin) + newMin;
	}

	private void init()
	{
		centerPoint = pathCenter.position;
		sunsetXAxis = transform.position.x;
		sunObject = GetComponent<ObjectSun>();
		radiusFromCenter = Mathf.Abs(Vector3.Distance(transform.position, pathCenter.transform.position));
		if (topOfSun == null)
		{
			SafeDebug.LogError("Top of sun isn't assigned in scene, issues will occur");
			topOfSun = transform;
		}
		sunIntensityMax = sunDesktopLight.mLight.intensity;
	}

	private Vector2 findPointOnCircle(float tValue, float radius, Vector3 centerPoint)
	{
		float x = centerPoint.x + radius * Mathf.Cos(tValue);
		float y = centerPoint.y + radius * Mathf.Sin(tValue);
		return new Vector2(x, y);
	}

	public void SetSunT(float value)
	{
		sunsetTheta = value;
	}

	public void SetIsInOrbit(bool value)
	{
		isInOrbit = value;
	}

	public void SetSunsetSpeed(float value)
	{
		sunsetSpeed = value;
	}

	public float GetSunT()
	{
		return sunsetTheta;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
		if (radiusFromCenter != 0f)
		{
			Gizmos.DrawWireSphere(pathCenter.position, radiusFromCenter);
		}
		else
		{
			Gizmos.DrawWireSphere(pathCenter.position, Mathf.Abs(Vector3.Distance(transform.position, pathCenter.transform.position)));
		}
	}
}
