using UnityEngine;

public class BillboardText : MonoBehaviour
{
	private void Update()
	{
		transform.LookAt(Camera.main.transform.position, Vector3.up);
	}
}
