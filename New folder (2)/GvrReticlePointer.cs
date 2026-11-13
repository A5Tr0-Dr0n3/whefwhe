using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GvrReticlePointer : GvrBasePointer
{
	public const float RETICLE_MIN_INNER_ANGLE = 0f;

	public const float RETICLE_MIN_OUTER_ANGLE = 0.5f;

	public const float RETICLE_GROWTH_ANGLE = 1.5f;

	public const float RETICLE_DISTANCE_MIN = 0.45f;

	public float maxReticleDistance = 10f;

	public int reticleSegments = 20;

	public float reticleGrowthSpeed = 8f;

	[Range(-32767f, 32767f)]
	public int reticleSortingOrder = 32767;

	public Material MaterialComp { private get; set; }

	public float ReticleInnerAngle { get; private set; }

	public float ReticleOuterAngle { get; private set; }

	public float ReticleDistanceInMeters { get; private set; }

	public float ReticleInnerDiameter { get; private set; }

	public float ReticleOuterDiameter { get; private set; }

	public override float MaxPointerDistance => maxReticleDistance;

	public override void OnPointerEnter(RaycastResult raycastResultResult, bool isInteractive)
	{
		SetPointerTarget(raycastResultResult.worldPosition, isInteractive);
	}

	public override void OnPointerHover(RaycastResult raycastResultResult, bool isInteractive)
	{
		SetPointerTarget(raycastResultResult.worldPosition, isInteractive);
	}

	public override void OnPointerExit(GameObject previousObject)
	{
		ReticleDistanceInMeters = maxReticleDistance;
		ReticleInnerAngle = 0f;
		ReticleOuterAngle = 0.5f;
	}

	public override void OnPointerClickDown()
	{
	}

	public override void OnPointerClickUp()
	{
	}

	public override void GetPointerRadius(out float enterRadius, out float exitRadius)
	{
		float f = 0f;
		float f2 = (float)Math.PI / 120f;
		enterRadius = 2f * Mathf.Tan(f);
		exitRadius = 2f * Mathf.Tan(f2);
	}

	public void UpdateDiameters()
	{
		ReticleDistanceInMeters = Mathf.Clamp(ReticleDistanceInMeters, 0.45f, maxReticleDistance);
		if (ReticleInnerAngle < 0f)
		{
			ReticleInnerAngle = 0f;
		}
		if (ReticleOuterAngle < 0.5f)
		{
			ReticleOuterAngle = 0.5f;
		}
		float f = (float)Math.PI / 180f * ReticleInnerAngle * 0.5f;
		float f2 = (float)Math.PI / 180f * ReticleOuterAngle * 0.5f;
		float b = 2f * Mathf.Tan(f);
		float b2 = 2f * Mathf.Tan(f2);
		ReticleInnerDiameter = Mathf.Lerp(ReticleInnerDiameter, b, Time.deltaTime * reticleGrowthSpeed);
		ReticleOuterDiameter = Mathf.Lerp(ReticleOuterDiameter, b2, Time.deltaTime * reticleGrowthSpeed);
		MaterialComp.SetFloat("_InnerDiameter", ReticleInnerDiameter * ReticleDistanceInMeters);
		MaterialComp.SetFloat("_OuterDiameter", ReticleOuterDiameter * ReticleDistanceInMeters);
		MaterialComp.SetFloat("_DistanceInMeters", ReticleDistanceInMeters);
	}

	private void Awake()
	{
		ReticleInnerAngle = 0f;
		ReticleOuterAngle = 0.5f;
	}

	protected override void Start()
	{
		base.Start();
		Renderer component = GetComponent<Renderer>();
		component.sortingOrder = reticleSortingOrder;
		MaterialComp = component.material;
		CreateReticleVertices();
	}

	private void Update()
	{
		UpdateDiameters();
	}

	private bool SetPointerTarget(Vector3 target, bool interactive)
	{
		if (base.PointerTransform == null)
		{
			Debug.LogWarning("Cannot operate on a null pointer transform");
			return false;
		}
		ReticleDistanceInMeters = Mathf.Clamp(base.PointerTransform.InverseTransformPoint(target).z, 0.45f, maxReticleDistance);
		if (interactive)
		{
			ReticleInnerAngle = 1.5f;
			ReticleOuterAngle = 2f;
		}
		else
		{
			ReticleInnerAngle = 0f;
			ReticleOuterAngle = 0.5f;
		}
		return true;
	}

	private void CreateReticleVertices()
	{
		Mesh mesh = new Mesh();
		gameObject.AddComponent<MeshFilter>();
		GetComponent<MeshFilter>().mesh = mesh;
		int num = reticleSegments;
		int num2 = (num + 1) * 2;
		Vector3[] array = new Vector3[num2];
		int num3 = 0;
		for (int i = 0; i <= num; i++)
		{
			float f = (float)i / (float)num * ((float)Math.PI * 2f);
			float x = Mathf.Sin(f);
			float y = Mathf.Cos(f);
			ref Vector3 reference = ref array[num3++];
			reference = new Vector3(x, y, 0f);
			ref Vector3 reference2 = ref array[num3++];
			reference2 = new Vector3(x, y, 1f);
		}
		int num4 = (num + 1) * 3 * 2;
		int[] array2 = new int[num4];
		int num5 = 0;
		int num6 = 0;
		for (int j = 0; j < num; j++)
		{
			array2[num6++] = num5 + 1;
			array2[num6++] = num5;
			array2[num6++] = num5 + 2;
			array2[num6++] = num5 + 1;
			array2[num6++] = num5 + 2;
			array2[num6++] = num5 + 3;
			num5 += 2;
		}
		mesh.vertices = array;
		mesh.triangles = array2;
		mesh.RecalculateBounds();
	}
}
