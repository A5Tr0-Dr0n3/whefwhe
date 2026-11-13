using UnityEngine;

public class BorderExitVacuumDrop : MonoBehaviour
{
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.name == "nearbyTrigger")
		{
			ToolManager.DropEverything();
			other.transform.parent.gameObject.SetActive(value: false);
		}
	}
}
