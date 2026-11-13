using UnityEngine;

public class ElevatorFloorSpecialPlace : MonoBehaviour
{
	[SerializeField]
	private Collider propDoor;

	public void EnablePropDoor(bool shouldEnable)
	{
		propDoor.enabled = shouldEnable;
	}
}
