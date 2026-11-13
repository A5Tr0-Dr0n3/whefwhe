using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class GvrControllerReticleVisual : MonoBehaviour
{
	[Serializable]
	public struct FaceCameraData(bool startEnabled)
	{
		public bool alongXAxis = startEnabled;

		public bool alongYAxis = startEnabled;

		public bool alongZAxis = startEnabled;

		public bool IsAnyAxisOff => !alongXAxis || !alongYAxis || !alongZAxis;
	}

	[Tooltip("Determines if the size of the reticle is based on the distance from the camera.")]
	public bool isSizeBasedOnCameraDistance = true;

	[Tooltip("Final size of the reticle in meters when it is 1 meter from the camera.")]
	public float sizeMeters = 0.1f;

	[Tooltip("Determines if the reticle will always face the camera and along what axes.")]
	public FaceCameraData doesReticleFaceCamera = new FaceCameraData(startEnabled: true);

	[Range(-32767f, 32767f)]
	public int sortingOrder;

	protected MeshRenderer meshRenderer;

	protected MeshFilter meshFilter;

	private Vector3 preRenderLocalScale;

	private Quaternion preRenderLocalRotation;

	public float ReticleMeshSizeMeters { get; private set; }

	public float ReticleMeshSizeRatio { get; private set; }

	public void RefreshMesh()
	{
		ReticleMeshSizeMeters = 1f;
		ReticleMeshSizeRatio = 1f;
		if (meshFilter != null && meshFilter.mesh != null)
		{
			ReticleMeshSizeMeters = meshFilter.mesh.bounds.size.x;
			if (ReticleMeshSizeMeters != 0f)
			{
				ReticleMeshSizeRatio = 1f / ReticleMeshSizeMeters;
			}
		}
		if (meshRenderer != null)
		{
			meshRenderer.sortingOrder = sortingOrder;
		}
	}

	protected virtual void Awake()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		meshFilter = GetComponent<MeshFilter>();
	}

	protected virtual void OnEnable()
	{
		RefreshMesh();
	}

	protected virtual void OnWillRenderObject()
	{
		preRenderLocalScale = transform.localScale;
		preRenderLocalRotation = transform.localRotation;
		Camera current = Camera.current;
		UpdateReticleSize(current);
		UpdateReticleOrientation(current);
	}

	protected virtual void OnRenderObject()
	{
		transform.localScale = preRenderLocalScale;
		transform.localRotation = preRenderLocalRotation;
	}

	protected virtual void UpdateReticleSize(Camera camera)
	{
		if (!(camera == null))
		{
			float num = sizeMeters;
			if (isSizeBasedOnCameraDistance)
			{
				float magnitude = (transform.position - camera.transform.position).magnitude;
				num *= ReticleMeshSizeRatio * magnitude;
			}
			transform.localScale = Vector3.one * num;
		}
	}

	protected virtual void UpdateReticleOrientation(Camera camera)
	{
		if (camera == null)
		{
			return;
		}
		Vector3 forward = transform.position - camera.transform.position;
		transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
		if (doesReticleFaceCamera.IsAnyAxisOff)
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
			if (!doesReticleFaceCamera.alongXAxis)
			{
				localEulerAngles.x = 0f;
			}
			if (!doesReticleFaceCamera.alongYAxis)
			{
				localEulerAngles.y = 0f;
			}
			if (!doesReticleFaceCamera.alongZAxis)
			{
				localEulerAngles.z = 0f;
			}
			transform.localEulerAngles = localEulerAngles;
		}
	}

	protected virtual void OnValidate()
	{
		if (Application.isPlaying && isActiveAndEnabled)
		{
			RefreshMesh();
		}
	}
}
