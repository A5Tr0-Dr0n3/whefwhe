using System.Collections.Generic;
using UnityEngine;

public class GravelRespawn : MonoBehaviour
{
	public float respawnHeight;

	public float respawnFrequency = 0.5f;

	public int activeGravelCount = 10;

	public bool spawning;

	private List<GameObject> spawnPool = new List<GameObject>();

	private List<GameObject> active = new List<GameObject>();

	private float timer;

	private Rigidbody rb;

	private void Start()
	{
		foreach (Transform item in base.transform)
		{
			spawnPool.Add(item.gameObject);
			item.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (spawning && timer > respawnFrequency && active.Count < activeGravelCount && spawnPool != null && spawnPool.Count > 0)
		{
			timer = 0f;
			rb = spawnPool[0].GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.mass = Random.Range(0.05f, 0.9f);
				rb.drag = Random.Range(0f, 0.5f);
			}
			spawnPool[0].SetActive(value: true);
			active.Add(spawnPool[0]);
			spawnPool.RemoveAt(0);
		}
		for (int i = 0; i < active.Count; i++)
		{
			if (active[i].transform.position.y < respawnHeight)
			{
				active[i].transform.position = transform.position;
				active[i].SetActive(value: false);
				spawnPool.Add(active[i]);
				active.RemoveAt(i);
			}
		}
	}
}
