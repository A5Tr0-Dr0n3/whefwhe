using UnityEngine;

public class NametagStickToPlayer : MonoBehaviour
{
	public bool sticking;

	private void Update()
	{
		if (sticking)
		{
			transform.position = EagerMaster.instance.mainCamera.transform.position - Vector3.up * 0.4f + EagerMaster.instance.mainCamera.transform.forward.normalized * 0.1f;
			transform.position = new Vector3(transform.position.x - 0.2f, 0.8f, transform.position.z);
			transform.eulerAngles = new Vector3(23f, EagerMaster.instance.mainCamera.transform.eulerAngles.y - 180f, 0f);
		}
	}
}
