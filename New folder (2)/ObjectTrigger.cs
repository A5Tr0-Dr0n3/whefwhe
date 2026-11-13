using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjectTrigger : MonoBehaviour
{
	public bool playerInside;

	public bool objectInside;

	public GameObject objectCheck;

	public GameObject disableObject;

	private void OnTriggerEnter(Collider other)
	{
		if (objectCheck != null && objectCheck == other.gameObject)
		{
			objectInside = true;
		}
		else if (other.tag == "MainCamera")
		{
			playerInside = true;
			if (disableObject != null)
			{
				disableObject.SetActive(value: false);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (objectCheck != null && objectCheck == other.gameObject)
		{
			objectInside = false;
		}
		else if (other.tag == "MainCamera")
		{
			playerInside = false;
		}
	}
}
