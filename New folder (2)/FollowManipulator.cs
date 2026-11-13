using UnityEngine;

public class FollowManipulator : MonoBehaviour
{
	public float speed = 6f;

	private Transform t;

	private void Start()
	{
		t = transform;
	}

	private void Update()
	{
		t.position = Vector3.Lerp(t.position, ToolManager.ActiveToolGroups[0].Manipulator.grabLocation, speed * Time.deltaTime);
	}
}
