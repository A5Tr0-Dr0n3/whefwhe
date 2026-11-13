using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
	private void Update()
	{
		transform.position = EagerMaster.instance.mainCamera.transform.position;
	}
}
