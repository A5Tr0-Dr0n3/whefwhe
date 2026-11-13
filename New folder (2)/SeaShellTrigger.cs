using UnityEngine;

public class SeaShellTrigger : MonoBehaviour
{
	[HideInInspector]
	public bool playerInRange;

	private SeaShell myShell;

	public void SetShell(SeaShell newShell)
	{
		myShell = newShell;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "MainCamera" && !playerInRange)
		{
			if (myShell.isGrabbed)
			{
				myShell.SetShellFadeState(isOn: true);
			}
			playerInRange = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "MainCamera" && playerInRange)
		{
			if (myShell.isGrabbed)
			{
				myShell.SetShellFadeState(isOn: false);
			}
			playerInRange = false;
		}
	}
}
