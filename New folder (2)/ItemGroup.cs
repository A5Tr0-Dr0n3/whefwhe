using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemGroup
{
	public List<TrackedItem> objects;

	public Transform root;

	public void AddObject(ObjectBase toAdd)
	{
		if (objects == null)
		{
			objects = new List<TrackedItem>();
		}
		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i].item == toAdd.gameObject)
			{
				return;
			}
		}
		if (!toAdd.gameObject.GetComponent<ObjectBlackHole>() && !toAdd.GetComponent<Headset>() && !toAdd.GetComponent<CageEnforcer>())
		{
			objects.Add(new TrackedItem(toAdd.gameObject));
			toAdd.transform.parent = root;
		}
	}

	public void removeObject(ObjectBase obj)
	{
		ConfigurableJoint componentInChildren = obj.GetComponentInChildren<ConfigurableJoint>();
		if ((bool)componentInChildren && (bool)componentInChildren.connectedBody.GetComponentInChildren<ObjectBase>())
		{
			ObjectBase componentInChildren2 = componentInChildren.connectedBody.GetComponentInChildren<ObjectBase>();
			removeObject(componentInChildren2);
		}
		for (int num = objects.Count - 1; num > -1; num--)
		{
			if (objects[num].item == obj.gameObject)
			{
				objects[num].item.transform.SetParent(objects[num].originalParent, worldPositionStays: true);
				objects.Remove(objects[num]);
			}
		}
	}

	public void ResetVelocity()
	{
		for (int i = 0; i < objects.Count; i++)
		{
			for (int j = 0; j < objects[i].rigidBodies.Length; j++)
			{
				if (objects[i].rigidBodies[j] != null)
				{
					objects[i].rigidBodies[j].velocity = Vector3.zero;
				}
			}
		}
	}

	public void EnableAllObjects()
	{
		for (int i = 0; i < objects.Count; i++)
		{
			objects[i].item.SetActive(value: true);
		}
		root.gameObject.SetActive(value: true);
	}

	public void DisableAllObjects()
	{
		for (int i = 0; i < objects.Count; i++)
		{
			objects[i].item.SetActive(value: false);
		}
		root.gameObject.SetActive(value: false);
	}

	public void SetObjectsKinematic(bool s)
	{
		for (int i = 0; i < objects.Count; i++)
		{
			for (int j = 0; j < objects[i].rigidBodies.Length; j++)
			{
				if (objects[i].rigidBodies[j] != null)
				{
					if (!objects[i].originallyKinematic)
					{
						objects[i].rigidBodies[j].isKinematic = s;
					}
					else
					{
						objects[i].rigidBodies[j].isKinematic = true;
					}
				}
			}
		}
	}

	public void SetObjectsTrigger(bool s)
	{
		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i].colliders == null)
			{
				continue;
			}
			for (int j = 0; j < objects[i].colliders.Length; j++)
			{
				if (!objects[i].colliders[j].originallyTrigger)
				{
					objects[i].colliders[j].col.isTrigger = s;
				}
			}
		}
	}

	public void SetObjectsGravity(bool s)
	{
		for (int i = 0; i < objects.Count; i++)
		{
			for (int j = 0; j < objects[i].rigidBodies.Length; j++)
			{
				objects[i].rigidBodies[j].useGravity = s;
			}
		}
	}

	public bool listContains(GameObject obj)
	{
		bool result = false;
		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i].item.GetInstanceID() == obj.GetInstanceID())
			{
				result = true;
			}
		}
		return result;
	}
}
