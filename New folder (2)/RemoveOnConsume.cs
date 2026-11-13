using UnityEngine;

[RequireComponent(typeof(VVRConsumable))]
public class RemoveOnConsume : MonoBehaviour
{
	public GameObject[] toRemove;

	private VVRConsumable consumable;

	private void Start()
	{
		consumable = GetComponent<VVRConsumable>();
	}

	private void Update()
	{
		if (toRemove == null)
		{
		}
		if (consumable.currentObjectState == VVRConsumable.objectState.partiallyConsumed)
		{
			for (int i = 0; i < toRemove.Length; i++)
			{
				toRemove[i].SetActive(value: false);
			}
		}
	}
}
