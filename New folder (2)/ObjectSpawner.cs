using System;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
	public enum Shapes
	{
		FibonacciSphere,
		SemiSphere,
		Point,
		Mesh
	}

	public Shapes spawnShape;

	public int sampleNumber = 20;

	public GameObject[] spawnObjects;

	public ShrinkTrigger faceShrinker;

	private GameObject[] pointsParents;

	private Vector3[] points;

	[HideInInspector]
	public GameObject[] pointObjects;

	private int randObject;

	private float rnd;

	private float x;

	private float y;

	private float z;

	private float offset;

	private float increment;

	private float r;

	private float radius = 10f;

	private float phi;

	private void Start()
	{
		RunShape(spawnShape);
	}

	private void RunShape(Shapes shape)
	{
		points = new Vector3[sampleNumber];
		pointObjects = new GameObject[sampleNumber];
		pointsParents = new GameObject[spawnObjects.Length];
		switch (shape)
		{
		case Shapes.FibonacciSphere:
			points = FibonacciSphere(sampleNumber);
			break;
		}
		for (int i = 0; i < spawnObjects.Length; i++)
		{
			pointsParents[i] = UnityEngine.Object.Instantiate(new GameObject());
			pointsParents[i].name = "Object Parent " + i;
			pointsParents[i].transform.SetParent(transform);
			pointsParents[i].SetActive(value: false);
			for (int j = 0; j < points.Length; j++)
			{
				Quaternion rotation = Quaternion.LookRotation(points[j] - EagerMaster.instance.player.transform.position);
				if (spawnObjects.Length > 0)
				{
					pointObjects[j] = UnityEngine.Object.Instantiate(spawnObjects[i], points[j] + EagerMaster.instance.player.transform.position, rotation);
				}
				else
				{
					pointObjects[j] = UnityEngine.Object.Instantiate(new GameObject(), points[j] + EagerMaster.instance.player.transform.position, rotation);
				}
				pointObjects[j].name = "Point" + (j + 1);
				pointObjects[j].tag = "ShrinkLookAt";
				SphereCollider sphereCollider = pointObjects[j].AddComponent<SphereCollider>();
				sphereCollider.isTrigger = true;
				pointObjects[j].transform.SetParent(pointsParents[i].transform);
			}
		}
		randObject = UnityEngine.Random.Range(0, spawnObjects.Length);
		pointsParents[randObject].SetActive(value: true);
		for (int k = 0; k < points.Length; k++)
		{
			pointObjects[k] = pointsParents[randObject].transform.GetChild(k).gameObject;
		}
	}

	private Vector3[] FibonacciSphere(int samples)
	{
		offset = 2f / (float)samples;
		increment = (float)Math.PI * (3f - Mathf.Sqrt(5f));
		for (int i = 0; i < samples; i++)
		{
			y = (float)i * offset - 1f + offset / 2f;
			r = Mathf.Sqrt(1f - Mathf.Pow(y, 2f));
			phi = ((float)i + 1f) % (float)samples * increment;
			x = Mathf.Cos(phi) * r;
			z = Mathf.Sin(phi) * r;
			ref Vector3 reference = ref points[i];
			reference = radius * new Vector3(x, y, z);
		}
		return points;
	}

	public void ChangeObjects()
	{
		randObject = UnityEngine.Random.Range(0, spawnObjects.Length);
		if (!pointsParents[randObject].activeSelf)
		{
			for (int i = 0; i < spawnObjects.Length; i++)
			{
				pointsParents[i].SetActive(value: false);
			}
			pointsParents[randObject].transform.rotation = UnityEngine.Random.rotation;
			ResetWords();
			pointsParents[randObject].SetActive(value: true);
			for (int j = 0; j < points.Length; j++)
			{
				pointObjects[j] = pointsParents[randObject].transform.GetChild(j).gameObject;
			}
		}
		else
		{
			ChangeObjects();
		}
	}

	public void ResetWords()
	{
		for (int i = 0; i < pointObjects.Length; i++)
		{
			faceShrinker.shrinkObjects[i] = false;
		}
	}
}
