using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MultiObjectTrigger : MonoBehaviour
{
	public bool hasObjectInside;

	public List<GameObject> objectChecks;

	private GameObject objectInside;

	public GameObject ObjectInside => objectInside;

	private void OnTriggerEnter(Collider other)
	{
		if (objectChecks != null && objectChecks.Contains(other.gameObject))
		{
			hasObjectInside = true;
			objectInside = other.gameObject;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (objectChecks != null && objectChecks.Contains(other.gameObject))
		{
			hasObjectInside = false;
			objectInside = other.gameObject;
		}
	}

	private void Update()
	{
		if (objectInside == null)
		{
			hasObjectInside = false;
		}
	}
}
