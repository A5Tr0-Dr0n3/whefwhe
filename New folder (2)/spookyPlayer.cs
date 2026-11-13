using UnityEngine;

public class spookyPlayer : MonoBehaviour
{
	private bool firstPlay = true;

	public GameObject thelma;

	public GameObject title;

	private void Start()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (firstPlay && other.gameObject == thelma)
		{
			GetComponent<AudioSource>().Play();
			firstPlay = false;
			if ((bool)title)
			{
				title.SetActive(value: false);
			}
		}
	}
}
