using System;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkTrigger : MonoBehaviour
{
	public ObjectSpawner objSpawner;

	[HideInInspector]
	public List<bool> shrinkObjects;

	private void Start()
	{
		transform.parent = EagerMaster.instance.mainCamera.transform;
		shrinkObjects = new List<bool>();
		for (int i = 0; i < objSpawner.sampleNumber; i++)
		{
			shrinkObjects.Add(item: false);
		}
	}

	private void Update()
	{
		HideWords();
	}

	private void HideWords()
	{
		for (int i = 0; i < objSpawner.pointObjects.Length; i++)
		{
			if (!shrinkObjects[i])
			{
				shrinkObjects.Add(item: false);
			}
			if (shrinkObjects[i])
			{
				objSpawner.pointObjects[i].transform.localScale = Vector3.Lerp(objSpawner.pointObjects[i].transform.localScale, Vector3.zero, 5f * Time.deltaTime);
			}
			else
			{
				objSpawner.pointObjects[i].transform.localScale = Vector3.Lerp(objSpawner.pointObjects[i].transform.localScale, Vector3.one * 4f, 5f * Time.deltaTime);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "ShrinkLookAt")
		{
			shrinkObjects[Array.IndexOf(objSpawner.pointObjects, other.gameObject)] = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "ShrinkLookAt")
		{
			shrinkObjects[Array.IndexOf(objSpawner.pointObjects, other.gameObject)] = false;
		}
	}
}
