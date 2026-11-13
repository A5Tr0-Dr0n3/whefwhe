using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BoidStraps : MonoBehaviour
{
	public Material StrapMat;

	public Color VertColor;

	private static Mesh m;

	[SerializeField]
	private MeshRenderer _renderer;

	[SerializeField]
	private MeshFilter _filter;

	public int NLengthSubdivs;

	public float StrapLength;

	public float StrapThickness;

	public float StrapHeight;

	public Vector3 RightStrapOrigin;

	public AnimationCurve ExtensionAnimCurve;

	public AnimationCurve FlareAnimCurve;

	public AnimationCurve ConvergenceAnimCurve;

	private MaterialPropertyBlock _block;

	private int _extensionAmtId;

	private int _convergenceAmtId;

	private int _flareAmtId;

	[Range(0f, 1f)]
	public float ExtendProgress;

	[Range(0f, 1f)]
	public float GrabProgress;

	private bool isActive;

	private float extendStartDistance;

	private float grabStartDistance;

	private bool removing;

	private float removingProgress;

	public void BuildStraps()
	{
		_renderer = GetComponent<MeshRenderer>();
		if (_renderer == null)
		{
			_renderer = gameObject.AddComponent<MeshRenderer>();
		}
		_filter = GetComponent<MeshFilter>();
		if (_filter == null)
		{
			_filter = gameObject.AddComponent<MeshFilter>();
		}
		if (m != null)
		{
			Object.DestroyImmediate(m);
		}
		GenerateMesh();
		_renderer.sharedMaterial = StrapMat;
		_filter.sharedMesh = m;
		StrapMat.SetVector("_RightStrapOrigin", new Vector4(RightStrapOrigin.x, RightStrapOrigin.y, RightStrapOrigin.z, 1f));
		StrapMat.SetVector("_LeftStrapOrigin", new Vector4(RightStrapOrigin.x * -1f, RightStrapOrigin.y, RightStrapOrigin.z, 1f));
	}

	private void GenerateMesh()
	{
		m = new Mesh();
		List<Vector3> list = new List<Vector3>();
		List<int> list2 = new List<int>();
		List<Vector2> list3 = new List<Vector2>();
		for (int i = 0; i < 2; i++)
		{
			Vector3 rightStrapOrigin = RightStrapOrigin;
			if (i == 1)
			{
				rightStrapOrigin.x *= -1f;
			}
			for (int j = 0; j < NLengthSubdivs; j++)
			{
				float num = (float)j / ((float)NLengthSubdivs - 1f);
				Vector3 vector = rightStrapOrigin + num * Vector3.back * StrapLength;
				int count = list.Count;
				Vector3 vector2 = Vector3.right * StrapThickness * 0.5f;
				Vector3 vector3 = Vector3.up * StrapHeight * 0.5f;
				list.Add(vector - vector2 - vector3);
				list.Add(vector - vector2 - vector3);
				list.Add(vector - vector2 + vector3);
				list.Add(vector - vector2 + vector3);
				list.Add(vector + vector2 + vector3);
				list.Add(vector + vector2 + vector3);
				list.Add(vector + vector2 - vector3);
				list.Add(vector + vector2 - vector3);
				list3.Add(new Vector2((float)i * 0.6f, num));
				list3.Add(new Vector2((float)i * 0.6f, num));
				list3.Add(new Vector2((float)i * 0.6f, num));
				list3.Add(new Vector2((float)i * 0.6f, num));
				list3.Add(new Vector2((float)i * 0.6f, num));
				list3.Add(new Vector2((float)i * 0.6f, num));
				list3.Add(new Vector2((float)i * 0.6f, num));
				list3.Add(new Vector2((float)i * 0.6f, num));
				for (int k = 0; k < 8; k++)
				{
					int num2 = count + k;
					int num3 = count + (k + 1) % 8;
					if (j == NLengthSubdivs - 1)
					{
						list2.Add(num2);
						list2.Add(num3);
						list2.Add(count + 8);
						continue;
					}
					list2.Add(num2);
					list2.Add(num3);
					list2.Add(num2 + 8);
					list2.Add(num3);
					list2.Add(num3 + 8);
					list2.Add(num2 + 8);
				}
			}
			list.Add(rightStrapOrigin + Vector3.back * StrapLength);
			list3.Add(new Vector2((float)i * 0.5f, 1f));
		}
		m.vertices = list.ToArray();
		m.triangles = list2.ToArray();
		m.uv = list3.ToArray();
		Color[] array = new Color[list.Count];
		for (int l = 0; l < array.Length; l++)
		{
			ref Color reference = ref array[l];
			reference = VertColor;
		}
		m.colors = array;
		m.RecalculateBounds();
		m.RecalculateNormals();
	}

	private void Awake()
	{
		if (m == null)
		{
			GenerateMesh();
		}
		if (_filter != null)
		{
			_filter.sharedMesh = m;
		}
		_extensionAmtId = Shader.PropertyToID("_StrapExtension");
		_convergenceAmtId = Shader.PropertyToID("_EndConvergence");
		_flareAmtId = Shader.PropertyToID("_FlareAmt");
		_block = new MaterialPropertyBlock();
		if (_renderer != null)
		{
			_renderer.enabled = false;
		}
	}

	public void Activate()
	{
		_renderer.enabled = true;
		isActive = true;
		extendStartDistance = Vector3.Distance(transform.position, Camera.main.transform.position);
		StrapsUpdate();
	}

	public void Deactivate()
	{
		isActive = false;
		removing = true;
		removingProgress = 0f;
		StrapsUpdate();
	}

	public void StartGrab()
	{
		grabStartDistance = Vector3.Distance(transform.position, Camera.main.transform.position);
	}

	private void Update()
	{
		StrapsUpdate();
	}

	private void StrapsUpdate()
	{
		if (isActive)
		{
			float num = Vector3.Distance(transform.position, Camera.main.transform.position);
			ExtendProgress = 1f - Mathf.Clamp01(num / extendStartDistance);
			GrabProgress = 1f - Mathf.Clamp01(num / grabStartDistance);
		}
		else
		{
			if (!removing)
			{
				return;
			}
			removingProgress += Time.deltaTime * 3f;
			if (removingProgress > 1f)
			{
				removing = false;
				_renderer.enabled = false;
			}
			ExtendProgress = 1f - Mathf.Clamp01(removingProgress);
			GrabProgress = 0.5f - ExtendProgress * 0.5f;
		}
		_block.SetFloat(_extensionAmtId, ExtensionAnimCurve.Evaluate(ExtendProgress));
		_block.SetFloat(_flareAmtId, FlareAnimCurve.Evaluate(ExtendProgress));
		_block.SetFloat(_convergenceAmtId, ConvergenceAnimCurve.Evaluate(GrabProgress));
		_renderer.SetPropertyBlock(_block);
	}
}
