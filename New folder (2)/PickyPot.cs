using UnityEngine;

public class PickyPot : Pot
{
	public ObjectPottable pickedPottable;

	public float plantedSmashVelocityMin = 1f;

	private void Update()
	{
		if (plant == null)
		{
			smashVelocityMin = float.PositiveInfinity;
		}
		else
		{
			smashVelocityMin = plantedSmashVelocityMin;
		}
	}

	protected override void OnTriggerEnter(Collider other)
	{
		if (pickedPottable != null && other.GetComponent<ObjectPottable>() == pickedPottable && !other.GetComponent<ObjectPottable>().isPotted && other.GetComponent<ObjectPottable>().isGrabbed)
		{
			plantObjectBase = other.GetComponent<ObjectPottable>();
			PotThePlant(other.gameObject);
		}
	}
}
