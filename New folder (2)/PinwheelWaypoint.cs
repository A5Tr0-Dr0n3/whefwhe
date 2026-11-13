using UnityEngine;

public class PinwheelWaypoint : MonoBehaviour
{
	[SerializeField]
	private int number;

	[HideInInspector]
	public PinwheelGameController gameController;

	private void OnTriggerEnter(Collider obj)
	{
		if (obj.tag == "MainCamera")
		{
			gameController.SetWaypoint(number);
		}
	}
}
