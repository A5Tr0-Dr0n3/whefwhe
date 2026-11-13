using UnityEngine;

public class DollRoomTriggerArea : MonoBehaviour
{
	public JobTina sceneRef;

	public DollhouseRoom mRoom;

	private void OnTriggerEnter(Collider other)
	{
		DollhouseDoll component = other.GetComponent<DollhouseDoll>();
		if (component != null)
		{
			component.currentRoom = mRoom;
			if (component.playerDoll && !component.isTinaDoll && component.isGrabbed)
			{
				sceneRef.ReactToPlayerDollInrRoom(mRoom);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		DollhouseDoll component = other.GetComponent<DollhouseDoll>();
		if (component != null && component.currentRoom == mRoom)
		{
			component.currentRoom = DollhouseRoom.Count;
		}
	}
}
