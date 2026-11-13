using System;
using System.Collections;
using UnityEngine;

public class WindWakerWindSpawner : MonoBehaviour
{
	public GameObject waftPrefab;

	public bool windBlowing = true;

	public bool tornado;

	public Transform spawnCenterTransform;

	public float spawnTimeRangeSecondsMin = 0.5f;

	public float spawnTimeRangeSecondsMax = 5f;

	[SerializeField]
	private Vector3 _windSpawnBox = new Vector3(50f, 20f, 50f);

	public Vector3 _windVector = new Vector3(0f, 0f, 1f);

	[SerializeField]
	private Vector3 _windDirectionRandomness = new Vector3(0f, 0f, 0f);

	public float _windSpeedMin = 2f;

	public float _windSpeedMax = 4f;

	public float lifespanMin = 2f;

	public float lifespanMax = 12f;

	public float asynchronicity;

	public Vector3 curvinessMin = new Vector3(0f, 0.5f, 0.25f);

	public Vector3 curvinessMax = new Vector3(0.5f, 2f, 1f);

	private Coroutine windCoroutine;

	private void OnEnable()
	{
		windCoroutine = StartCoroutine(SpawnRoutine());
	}

	private IEnumerator SpawnRoutine()
	{
		while (windBlowing)
		{
			float sampleTime = UnityEngine.Random.Range(spawnTimeRangeSecondsMin, spawnTimeRangeSecondsMax);
			yield return new WaitForSeconds(sampleTime);
			Vector3 pos = spawnCenterTransform.position + new Vector3(UnityEngine.Random.Range(_windSpawnBox.x * -0.5f, _windSpawnBox.x * 0.5f), UnityEngine.Random.Range(_windSpawnBox.y * -0.5f, _windSpawnBox.y * 0.5f), UnityEngine.Random.Range(_windSpawnBox.z * -0.5f, _windSpawnBox.z * 0.5f));
			Vector3 randomWindVector = new Vector3(_windVector.x + (UnityEngine.Random.value - 0.5f) * _windDirectionRandomness.x, _windVector.y + (UnityEngine.Random.value - 0.5f) * _windDirectionRandomness.y, _windVector.z + (UnityEngine.Random.value - 0.5f) * _windDirectionRandomness.z).normalized * UnityEngine.Random.Range(_windSpeedMin, _windSpeedMax);
			float radius = 1f;
			float angle = 0f;
			if (tornado)
			{
				radius = pos.magnitude;
				angle = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
				pos = new Vector3(radius * Mathf.Cos(angle), pos.y, radius * Mathf.Sin(angle));
				randomWindVector = randomWindVector.normalized * radius;
			}
			GameObject waftling = UnityEngine.Object.Instantiate(waftPrefab, pos, Quaternion.identity);
			waftling.transform.parent = EagerScenes.instance.ActiveScene.transform;
			waftling.GetComponent<Waft>().windVector = randomWindVector;
			waftling.GetComponent<Waft>().lifeSpan = UnityEngine.Random.Range(lifespanMin, lifespanMax);
			waftling.GetComponent<Waft>().timeOffset = UnityEngine.Random.Range(0f, asynchronicity) + angle;
			waftling.GetComponent<Waft>().curviness = new Vector3(UnityEngine.Random.Range(curvinessMin.x, curvinessMax.x) + radius, UnityEngine.Random.Range(curvinessMin.y, curvinessMax.y), UnityEngine.Random.Range(curvinessMin.z, curvinessMax.z) + radius);
		}
	}

	private void OnDisable()
	{
		StopCoroutine(windCoroutine);
	}
}
