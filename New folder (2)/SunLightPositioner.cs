using UnityEngine;

public class SunLightPositioner : MonoBehaviour
{
	public GameObject sun;

	private void Update()
	{
		transform.position = sun.transform.position + Vector3.up * sun.transform.localScale.x * 0.5f;
	}
}
