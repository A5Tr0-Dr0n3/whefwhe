using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(GvrLaserVisual))]
public class GvrLaserPointer : GvrBasePointer
{
	[Tooltip("Distance from the pointer that raycast hits will be detected.")]
	public float maxPointerDistance = 20f;

	[Tooltip("Distance from the pointer that the reticle will be drawn at when hitting nothing.")]
	public float defaultReticleDistance = 20f;

	[Tooltip("By default, the length of the laser is used as the CameraRayIntersectionDistance. Set this field to a non-zero value to override it.")]
	public float overrideCameraRayIntersectionDistance;

	private const float RETICLE_VISUAL_RATIO = 0.1f;

	private bool isHittingTarget;

	public GvrLaserVisual LaserVisual { get; private set; }

	public override float MaxPointerDistance => maxPointerDistance;

	public override float CameraRayIntersectionDistance
	{
		get
		{
			if (overrideCameraRayIntersectionDistance != 0f)
			{
				return overrideCameraRayIntersectionDistance;
			}
			return (!(LaserVisual != null)) ? overrideCameraRayIntersectionDistance : LaserVisual.maxLaserDistance;
		}
	}

	public override void OnPointerEnter(RaycastResult raycastResult, bool isInteractive)
	{
		LaserVisual.SetDistance(raycastResult.distance);
		isHittingTarget = true;
	}

	public override void OnPointerHover(RaycastResult raycastResult, bool isInteractive)
	{
		LaserVisual.SetDistance(raycastResult.distance);
		isHittingTarget = true;
	}

	public override void OnPointerExit(GameObject previousObject)
	{
		isHittingTarget = false;
	}

	public override void OnPointerClickDown()
	{
	}

	public override void OnPointerClickUp()
	{
	}

	public override void GetPointerRadius(out float enterRadius, out float exitRadius)
	{
		if (LaserVisual.reticle != null)
		{
			float x = LaserVisual.reticle.transform.localScale.x;
			enterRadius = LaserVisual.reticle.sizeMeters * 0.5f * 0.1f;
			exitRadius = x * LaserVisual.reticle.ReticleMeshSizeMeters * 0.1f;
		}
		else
		{
			enterRadius = 0f;
			exitRadius = 0f;
		}
	}

	private void Awake()
	{
		LaserVisual = GetComponent<GvrLaserVisual>();
	}

	protected override void Start()
	{
		base.Start();
		LaserVisual.GetPointForDistanceFunction = GetPointAlongPointer;
		LaserVisual.SetDistance(defaultReticleDistance, immediate: true);
	}

	private void Update()
	{
		if (!isHittingTarget)
		{
			LaserVisual.SetDistance(defaultReticleDistance);
		}
	}
}
