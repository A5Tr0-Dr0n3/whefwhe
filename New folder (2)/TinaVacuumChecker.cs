using UnityEngine;
using UnityEngine.Events;

public class TinaVacuumChecker : MonoBehaviour
{
	public UnityEvent OnPlayerUseVacuumEvent;

	private float lastVacValue = -1f;

	private float timeSinceInvoked;

	private static float reactTime = 4f;

	private void Start()
	{
		lastVacValue = EagerMaster.instance.vacuumFill;
		timeSinceInvoked = reactTime;
	}

	private void Update()
	{
		timeSinceInvoked += Time.deltaTime;
		if (EagerMaster.instance.vacuumFill != lastVacValue && timeSinceInvoked > reactTime)
		{
			OnPlayerUseVacuumEvent.Invoke();
			timeSinceInvoked = 0f;
		}
		lastVacValue = EagerMaster.instance.vacuumFill;
	}
}
