using System.Collections.Generic;
using UnityEngine;

public class WhaleTalkerController : MonoBehaviour
{
	public AudioSourceLoudness Loudness;

	public AnimationCurve LoudnessCurve;

	public Material ScreenMat;

	public Material GlowMat;

	private int volId;

	private void Awake()
	{
		volId = Shader.PropertyToID("_Volume");
	}

	private void Update()
	{
		float value = LoudnessCurve.Evaluate(Loudness.lerpLoudness);
		ScreenMat.SetFloat(volId, value);
		GlowMat.SetFloat(volId, value);
	}

	public void DoRandomize()
	{
		Dictionary<Transform, Color> dictionary = new Dictionary<Transform, Color>();
		MeshRenderer[] array = Object.FindObjectsOfType<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in array)
		{
			if (meshRenderer.sharedMaterial == GlowMat || meshRenderer.sharedMaterial == ScreenMat)
			{
				Transform parent = meshRenderer.transform;
				while (parent.parent != null && parent.parent != transform)
				{
					parent = parent.parent;
				}
				SafeDebug.Log("Sync transform = " + parent);
				Color color = Random.ColorHSV();
				if (dictionary.ContainsKey(parent))
				{
					color = dictionary[parent];
				}
				else
				{
					dictionary.Add(parent, color);
				}
				MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
				Mesh mesh = new Mesh();
				Vector3[] array2 = new Vector3[component.sharedMesh.vertexCount];
				int[] array3 = new int[component.sharedMesh.triangles.Length];
				Vector3[] array4 = new Vector3[component.sharedMesh.vertexCount];
				Color[] array5 = new Color[component.sharedMesh.vertexCount];
				Vector2[] array6 = new Vector2[component.sharedMesh.uv.Length];
				for (int j = 0; j < component.sharedMesh.vertexCount; j++)
				{
					ref Vector3 reference = ref array2[j];
					reference = component.sharedMesh.vertices[j];
					ref Vector3 reference2 = ref array4[j];
					reference2 = component.sharedMesh.normals[j];
					array5[j] = color;
				}
				for (int k = 0; k < component.sharedMesh.uv.Length; k++)
				{
					ref Vector2 reference3 = ref array6[k];
					reference3 = component.sharedMesh.uv[k];
				}
				for (int l = 0; l < component.sharedMesh.triangles.Length; l++)
				{
					array3[l] = component.sharedMesh.triangles[l];
				}
				mesh.vertices = array2;
				mesh.triangles = array3;
				mesh.normals = array4;
				mesh.colors = array5;
				mesh.uv = array6;
				component.sharedMesh = mesh;
			}
		}
	}
}
