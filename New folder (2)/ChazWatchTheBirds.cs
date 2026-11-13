using UnityEngine;

public class ChazWatchTheBirds : MonoBehaviour
{
	private ChazController chaz;

	public Transform birdToWatch;

	private void Start()
	{
	}

	private void OnEnable()
	{
		chaz = gameObject.GetComponent<ChazController>();
		chaz.EyeLookAt(birdToWatch);
	}
}
