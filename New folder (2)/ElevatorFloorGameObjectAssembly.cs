using UnityEngine;

public class ElevatorFloorGameObjectAssembly : MonoBehaviour
{
	public GvrAudioSource mAudioSource;

	[SerializeField]
	private Renderer floorTop;

	[SerializeField]
	private Renderer floorBottom;

	public float GetFloorHeight()
	{
		if (floorTop != null && floorBottom != null)
		{
			return floorTop.bounds.max.y - floorBottom.bounds.min.y;
		}
		return 0f;
	}

	public Vector3 GetFloorTopPoint()
	{
		if (floorTop != null)
		{
			return new Vector3
			{
				x = floorTop.bounds.min.x + floorTop.bounds.max.x / 2f,
				y = floorTop.bounds.max.y,
				z = floorTop.bounds.min.z + floorTop.bounds.max.z / 2f
			};
		}
		return Vector2.zero;
	}

	public Vector3 GetFloorBottomPoint()
	{
		if (floorTop != null)
		{
			return new Vector3
			{
				x = floorBottom.bounds.min.x + floorBottom.bounds.max.x / 2f,
				y = floorBottom.bounds.min.y,
				z = floorBottom.bounds.min.z + floorBottom.bounds.max.z / 2f
			};
		}
		return Vector2.zero;
	}
}
