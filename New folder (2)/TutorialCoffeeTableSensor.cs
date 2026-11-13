using UnityEngine;

public class TutorialCoffeeTableSensor : MonoBehaviour
{
	private Collider[] targetObjectColliders;

	private bool playerPlacedObject;

	public void SetTargetObject(Collider[] obj)
	{
		targetObjectColliders = obj;
	}

	private void OnTriggerEnter(Collider c)
	{
		if (targetObjectColliders == null)
		{
			return;
		}
		Collider[] array = targetObjectColliders;
		foreach (Collider collider in array)
		{
			if (c == collider)
			{
				playerPlacedObject = true;
			}
		}
	}

	public bool DidPlayerPutObjectOnCoffeeTable()
	{
		return playerPlacedObject;
	}
}
