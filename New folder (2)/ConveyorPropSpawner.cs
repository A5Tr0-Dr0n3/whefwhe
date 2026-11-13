using System.Collections.Generic;
using UnityEngine;

public class ConveyorPropSpawner : MonoBehaviour
{
	public List<GameObject> cactusList;

	public List<GameObject> fenceList;

	public List<GameObject> rockList;

	public GameObject grizzly;

	private bool spawnGrizzly;

	private float propRoll;

	private bool lastFlatHadProp;

	private void OnTriggerEnter(Collider c)
	{
		if (cactusList.Contains(c.gameObject) || fenceList.Contains(c.gameObject) || rockList.Contains(c.gameObject) || c.gameObject == grizzly)
		{
			c.gameObject.SetActive(value: false);
			c.transform.SetParent(JobTumbleweed.instance.transform);
			c.transform.position = Vector3.zero;
		}
	}

	private void OnTriggerExit(Collider c)
	{
		if (spawnGrizzly && !grizzly.activeInHierarchy && c.gameObject.name == "Flat")
		{
			Spawn(grizzly, c.transform, shift: false);
			spawnGrizzly = false;
		}
		else if (!lastFlatHadProp)
		{
			if (c.gameObject.name == "Flat" && Random.Range(0f, 1f) < JobTumbleweed.instance.spawnRate)
			{
				lastFlatHadProp = true;
				propRoll = Random.Range(0f, 1f);
				if (propRoll < JobTumbleweed.instance.cactusRateNormalized)
				{
					Spawn(cactusList, c.transform, shift: true);
				}
				else if (propRoll < JobTumbleweed.instance.fenceRateNormalized + JobTumbleweed.instance.cactusRateNormalized)
				{
					Spawn(fenceList, c.transform, shift: false);
				}
				else
				{
					Spawn(rockList, c.transform, shift: true);
				}
			}
		}
		else
		{
			lastFlatHadProp = false;
		}
	}

	public void SpawnGrizzly()
	{
		spawnGrizzly = true;
	}

	public void Spawn(List<GameObject> list, Transform spawnPoint, bool shift)
	{
		int num = Random.Range(0, list.Count - 1);
		for (int i = 0; i < list.Count; i++)
		{
			num++;
			if (num >= list.Count)
			{
				num = 0;
			}
			if (list[num] != null && !list[num].activeInHierarchy)
			{
				Spawn(list[num], spawnPoint, shift);
				break;
			}
		}
	}

	public void Spawn(GameObject obj, Transform spawnPoint, bool shift)
	{
		obj.transform.position = spawnPoint.position;
		if (shift)
		{
			obj.transform.position = new Vector3(obj.transform.position.x + Random.Range(-2.3f, 2.3f), obj.transform.position.y, obj.transform.position.z);
		}
		obj.transform.SetParent(spawnPoint, worldPositionStays: true);
		obj.SetActive(value: true);
	}
}
