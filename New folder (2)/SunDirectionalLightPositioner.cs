using UnityEngine;

public class SunDirectionalLightPositioner : MonoBehaviour
{
	public GameObject sun;

	private void Start()
	{
	}

	private void Update()
	{
		transform.position = Vector3.Lerp(sun.transform.position.Flatten(), EagerMaster.instance.mainCamera.transform.position.Flatten(), 0.95f);
		Vector3 position = transform.position;
		position.y = Mathf.Lerp(sun.transform.position.y, EagerMaster.instance.mainCamera.transform.position.y, 0.75f);
		transform.position = position;
	}
}
