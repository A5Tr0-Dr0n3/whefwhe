using UnityEngine;

public class ObjectWater : ObjectBase
{
	private float waterTime = 1.25f;

	private float waterCount;

	public virtual void WaterObject()
	{
		waterCount += Time.deltaTime;
		if (waterCount > waterTime && !isSatisfied && !(this is ObjectPlant))
		{
			Satisfaction();
		}
	}
}
