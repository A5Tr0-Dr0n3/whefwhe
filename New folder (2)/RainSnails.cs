using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainSnails : MonoBehaviour
{
	public List<GameObject> rainObjects;

	public float rainWidth;

	public float rainMax;

	public float rainMin;

	private int index;

	private GameObject go;

	private void Start()
	{
		StartCoroutine(SpawnSnails());
	}

	private IEnumerator SpawnSnails()
	{
		for (int i = 0; i < rainObjects.Count; i++)
		{
			yield return new WaitForSeconds(Random.Range(rainMin, rainMax));
			if (!rainObjects[i].GetComponent<ObjectBase>().isGrabbed)
			{
				rainObjects[i].transform.position = transform.position + new Vector3(Random.Range(0f - rainWidth, rainWidth), 0f, Random.Range(0f - rainWidth, rainWidth));
				rainObjects[i].transform.rotation = Random.rotation;
				rainObjects[i].SetActive(value: true);
				rainObjects[i].GetComponent<Rigidbody>().AddForce(transform.up * -100f);
			}
			if (i == rainObjects.Count - 1)
			{
				i = 0;
			}
		}
		yield return null;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(transform.position + Vector3.right * rainWidth + Vector3.forward * rainWidth, transform.position + Vector3.left * rainWidth + Vector3.forward * rainWidth);
		Gizmos.DrawLine(transform.position + Vector3.left * rainWidth + Vector3.forward * rainWidth, transform.position + Vector3.left * rainWidth + Vector3.back * rainWidth);
		Gizmos.DrawLine(transform.position + Vector3.right * rainWidth + Vector3.forward * rainWidth, transform.position + Vector3.right * rainWidth + Vector3.back * rainWidth);
		Gizmos.DrawLine(transform.position + Vector3.back * rainWidth + Vector3.right * rainWidth, transform.position + Vector3.back * rainWidth + Vector3.left * rainWidth);
	}

	private void OnTriggerEnter(Collider other)
	{
		go = other.gameObject;
		if (rainObjects.Contains(go) && !go.GetComponent<ObjectBase>().isGrabbed)
		{
			go.transform.position = transform.position + new Vector3(Random.Range(0f - rainWidth, rainWidth), 0f, Random.Range(0f - rainWidth, rainWidth));
			go.transform.rotation = Random.rotation;
		}
	}
}
