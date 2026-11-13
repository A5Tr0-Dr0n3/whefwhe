using UnityEngine;

public class AudioEventOnObjectDestroy : MonoBehaviour
{
	public AudioEvent eventToPlay;

	public GameObject objectTrigger;

	private bool eventTriggered;

	private void Update()
	{
		if (!eventTriggered && objectTrigger == null)
		{
			eventTriggered = true;
			eventToPlay.Play();
		}
	}
}
