using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelugeInvasion : MonoBehaviour
{
	public List<GameObject> invasionObjects;

	public float invasionRadius;

	public float spawnMax;

	public float spawnMin;

	private int index;

	private Vector3 spawnPoint;

	private Vector2 spawnCircle;

	private GameObject go;

	private void Start()
	{
		StartCoroutine(SpawnSnails());
	}

	private IEnumerator SpawnSnails()
	{
		for (int i = 0; i < invasionObjects.Count; i++)
		{
			yield return new WaitForSeconds(Random.Range(spawnMin, spawnMax));
			if (!invasionObjects[i].GetComponent<ObjectBase>().isGrabbed)
			{
				spawnCircle = Random.insideUnitCircle.normalized * invasionRadius;
				spawnPoint = new Vector3(spawnCircle.x, 0.1f, Mathf.Abs(spawnCircle.y));
				invasionObjects[i].transform.position = spawnPoint;
				invasionObjects[i].transform.rotation = Random.rotation;
				invasionObjects[i].SetActive(value: true);
			}
			if (i == invasionObjects.Count - 1)
			{
				i = 0;
			}
		}
		yield return null;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(Vector3.zero, invasionRadius);
	}

	private void OnTriggerEnter(Collider other)
	{
		go = other.gameObject;
		if (invasionObjects.Contains(go) && !go.GetComponent<ObjectBase>().isGrabbed)
		{
			go.transform.position = transform.position + new Vector3(Random.Range(0f - invasionRadius, invasionRadius), 0f, Random.Range(0f - invasionRadius, invasionRadius));
			go.transform.rotation = Random.rotation;
		}
	}
}
