using UnityEngine;

public class Dollhouse : MonoBehaviour
{
	internal Collider[] colliders;

	[SerializeField]
	private VVRConsumable[] consumableObjects;

	private void Awake()
	{
		if (colliders == null)
		{
			colliders = GetComponentsInChildren<Collider>();
		}
		if (consumableObjects == null || consumableObjects.Length == 0)
		{
			consumableObjects = GetComponentsInChildren<VVRConsumable>();
		}
		for (int i = 0; i < consumableObjects.Length; i++)
		{
			consumableObjects[i].unconsumable = true;
		}
	}

	public void BecomeConsumable()
	{
		for (int i = 0; i < consumableObjects.Length; i++)
		{
			consumableObjects[i].unconsumable = false;
		}
	}
}
