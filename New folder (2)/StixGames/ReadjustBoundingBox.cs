using UnityEngine;

namespace StixGames;

[RequireComponent(typeof(MeshFilter))]
public class ReadjustBoundingBox : MonoBehaviour
{
	public float maxGrassHeight = 2f;

	private void Start()
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Bounds bounds = mesh.bounds;
		bounds.Expand(maxGrassHeight * 2f);
		mesh.bounds = bounds;
		Object.Destroy(this);
	}
}
