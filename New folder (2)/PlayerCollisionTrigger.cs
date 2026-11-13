using UnityEngine;
using UnityEngine.Events;

public class PlayerCollisionTrigger : MonoBehaviour
{
	public UnityEvent OnPlayerCollisionEnterEvent;

	public UnityEvent OnPlayerCollisionExitEvent;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "MainCamera" && OnPlayerCollisionEnterEvent != null)
		{
			OnPlayerCollisionEnterEvent.Invoke();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "MainCamera" && OnPlayerCollisionExitEvent != null)
		{
			OnPlayerCollisionExitEvent.Invoke();
		}
	}
}
