using System;
using UnityEngine;

[Serializable]
public class TrackedItem
{
	public GameObject item;

	[HideInInspector]
	public Rigidbody[] rigidBodies;

	[HideInInspector]
	public TrackedCollider[] colliders;

	[HideInInspector]
	public Transform originalParent;

	[HideInInspector]
	public bool originallyKinematic;

	internal bool originallTrigger;

	public TrackedItem(GameObject obj)
	{
		item = obj;
		rigidBodies = obj.GetComponentsInChildren<Rigidbody>();
		Collider[] componentsInChildren = obj.GetComponentsInChildren<Collider>();
		TrackedCollider[] array = new TrackedCollider[componentsInChildren.Length];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			array[i] = new TrackedCollider(componentsInChildren[i]);
		}
		originalParent = obj.transform.parent;
		if (obj.GetComponent<Rigidbody>() != null)
		{
			originallyKinematic = obj.GetComponent<Rigidbody>().isKinematic;
		}
	}

	public TrackedItem(GameObject obj, Transform parent)
	{
		item = obj;
		rigidBodies = obj.GetComponentsInChildren<Rigidbody>();
		Collider[] componentsInChildren = obj.GetComponentsInChildren<Collider>();
		TrackedCollider[] array = new TrackedCollider[componentsInChildren.Length];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			array[i] = new TrackedCollider(componentsInChildren[i]);
		}
		originalParent = parent;
		if (obj.GetComponent<Rigidbody>() != null)
		{
			originallyKinematic = obj.GetComponent<Rigidbody>().isKinematic;
		}
	}
}
