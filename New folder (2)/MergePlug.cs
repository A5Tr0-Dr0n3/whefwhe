using UnityEngine;

public class MergePlug : MonoBehaviour
{
	private void FixedUpdate()
	{
		transform.position = EagerMaster.instance.mainCamera.transform.position + EagerMaster.instance.mainCamera.transform.up * 1f;
	}
}
