using UnityEngine;

public class PinwheelGameController : MonoBehaviour
{
	[SerializeField]
	private PinwheelWaypoint[] waypoints;

	[SerializeField]
	private SwapperObjectPinwheel pinWheel;

	private float waypointIndex;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetUpGame()
	{
		pinWheel.gameObject.SetActive(value: false);
		waypointIndex = 0f;
	}

	public void SetWaypoint(int index)
	{
		waypointIndex = index;
		if (index < waypoints.Length)
		{
			pinWheel.MoveToPosition(waypoints[index + 1].transform);
		}
	}
}
