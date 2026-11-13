using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class TBPlayArea : MonoBehaviour
{
	public struct Quad
	{
		public Vector3 corner0;

		public Vector3 corner1;

		public Vector3 corner2;

		public Vector3 corner3;
	}

	public enum Size
	{
		_200x150,
		_300x225,
		_400x300
	}

	public Size size;

	public Color color = new Color(1f, 0.78f, 0.3f);

	public float heightOffset;

	public float edgeHeight = 0.5f;

	public bool drawInGame = true;

	[HideInInspector]
	public Vector3[] vertices;

	[HideInInspector]
	public Vector3[] edgeVertices;

	[HideInInspector]
	public float width;

	[HideInInspector]
	public float depth;

	public void OnEnable()
	{
		if (Application.isPlaying)
		{
			GetComponent<MeshRenderer>().enabled = drawInGame;
			enabled = false;
		}
	}

	private void BuildMesh()
	{
		Mesh mesh = BuildGround();
		Mesh mesh2 = BuildEdges();
		CombineInstance[] array = new CombineInstance[2];
		array[0].mesh = mesh;
		array[0].transform = Matrix4x4.identity;
		array[1].mesh = mesh2;
		array[1].transform = Matrix4x4.identity;
		MeshFilter component = GetComponent<MeshFilter>();
		component.mesh = new Mesh();
		component.sharedMesh.CombineMeshes(array, mergeSubMeshes: false);
		Renderer component2 = GetComponent<MeshRenderer>();
		Material material = new Material(Shader.Find("Hidden/TB Play Area Ground"));
		material.SetTexture("_MainTex", Resources.Load("TBGrid") as Texture);
		material.SetTexture("_MaskTex", Resources.Load("TBGrid_Mask") as Texture);
		Material material2 = new Material(Shader.Find("Hidden/TB Play Area Edges"));
		component2.reflectionProbeUsage = ReflectionProbeUsage.Off;
		component2.shadowCastingMode = ShadowCastingMode.Off;
		component2.receiveShadows = false;
		component2.lightProbeUsage = LightProbeUsage.Off;
		Material[] materials = new Material[2] { material, material2 };
		component2.materials = materials;
	}

	private Mesh BuildGround()
	{
		Quad quad = default;
		if (!GetBounds(size, ref quad))
		{
			return null;
		}
		vertices = new Vector3[4] { quad.corner0, quad.corner1, quad.corner2, quad.corner3 };
		int[] triangles = new int[6] { 2, 1, 0, 1, 2, 3 };
		Vector2[] uv = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(width, 0f),
			new Vector2(0f, depth),
			new Vector2(width, depth)
		};
		Color[] colors = new Color[4] { color, color, color, color };
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.colors = colors;
		mesh.triangles = triangles;
		return mesh;
	}

	private Mesh BuildEdges()
	{
		if (Mathf.Approximately(edgeHeight, 0f))
		{
			return null;
		}
		Quad quad = default;
		if (!GetBounds(size, ref quad))
		{
			return null;
		}
		Vector3[] array = new Vector3[4] { quad.corner0, quad.corner1, quad.corner2, quad.corner3 };
		edgeVertices = new Vector3[array.Length * 2];
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 vector = array[i];
			ref Vector3 reference = ref edgeVertices[i];
			reference = new Vector3(vector.x, heightOffset, vector.z);
			ref Vector3 reference2 = ref edgeVertices[i + 4];
			reference2 = new Vector3(vector.x, heightOffset + edgeHeight, vector.z);
		}
		int[] triangles = new int[24]
		{
			1, 4, 0, 5, 4, 1, 3, 5, 1, 3,
			7, 5, 6, 7, 3, 3, 2, 6, 4, 2,
			0, 2, 4, 6
		};
		Vector2[] uv = new Vector2[8]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		Color[] colors = new Color[8]
		{
			color,
			color,
			color,
			color,
			new Color(color.r, color.g, color.b, 0f),
			new Color(color.r, color.g, color.b, 0f),
			new Color(color.r, color.g, color.b, 0f),
			new Color(color.r, color.g, color.b, 0f)
		};
		Mesh mesh = new Mesh();
		mesh.vertices = edgeVertices;
		mesh.uv = uv;
		mesh.colors = colors;
		mesh.triangles = triangles;
		return mesh;
	}

	private bool GetBounds(Size size, ref Quad quad)
	{
		try
		{
			string text = size.ToString().Substring(1);
			string[] array = text.Split(new char[1] { 'x' }, 2);
			float num = float.Parse(array[0]) / 200f;
			float num2 = float.Parse(array[1]) / 200f;
			width = num * 2f;
			depth = num2 * 2f;
			quad.corner0 = new Vector3(0f - num, heightOffset, 0f - num2);
			quad.corner1 = new Vector3(num, heightOffset, 0f - num2);
			quad.corner2 = new Vector3(0f - num, heightOffset, num2);
			quad.corner3 = new Vector3(num, heightOffset, num2);
			return true;
		}
		catch
		{
		}
		return false;
	}
}
