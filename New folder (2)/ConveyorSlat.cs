using UnityEngine;

public class ConveyorSlat : MonoBehaviour
{
	public GameObject[] slats;

	public float slatSpeed = 2f;

	private Vector3 startPos;

	private int i;

	private void Start()
	{
		startPos = slats[0].transform.position;
	}

	private void Update()
	{
		i = 0;
		for (i = 0; i < slats.Length; i++)
		{
			slats[i].transform.position += Vector3.forward * (0f - slatSpeed) * Time.deltaTime;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		other.transform.position = startPos;
	}
}
