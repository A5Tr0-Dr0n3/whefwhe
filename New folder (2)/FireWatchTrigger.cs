using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FireWatchTrigger : MonoBehaviour
{
	public bool hasObjectInside;

	public List<GameObject> objectChecks;

	private List<GameObject> objectsInside = new List<GameObject>();

	private List<GameObject> ignoreObjs = new List<GameObject>();

	private List<float> objectsTimeInside = new List<float>();

	public List<GameObject> ObjectsInside => objectsInside;

	public List<float> ObjectsTimeInside => objectsTimeInside;

	private void OnTriggerEnter(Collider other)
	{
		if (objectChecks != null && objectChecks.Contains(other.gameObject) && !ignoreObjs.Contains(other.gameObject))
		{
			hasObjectInside = true;
			if (!objectsInside.Contains(other.gameObject))
			{
				objectsInside.Add(other.gameObject);
				objectsTimeInside.Add(0f);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (objectChecks != null && objectChecks.Contains(other.gameObject) && objectsInside.Contains(other.gameObject))
		{
			objectsTimeInside.RemoveAt(objectsInside.IndexOf(other.gameObject));
			objectsInside.Remove(other.gameObject);
			if (objectsInside.Count <= 0)
			{
				hasObjectInside = false;
			}
		}
	}

	private void Update()
	{
		UpdateObjectList();
	}

	private void UpdateObjectList()
	{
		for (int num = objectsInside.Count - 1; num >= 0; num--)
		{
			if (objectsInside[num] == null)
			{
				objectsInside.RemoveAt(num);
				objectsTimeInside.RemoveAt(num);
			}
			else
			{
				objectsTimeInside[num] += Time.deltaTime;
			}
		}
	}

	public void AddObjectIgnore(GameObject ignoreObj)
	{
		ignoreObjs.Add(ignoreObj);
		if (objectsInside.Contains(ignoreObj))
		{
			objectsTimeInside.RemoveAt(objectsInside.IndexOf(ignoreObj));
			objectsInside.Remove(ignoreObj);
		}
	}
}
