using System.Collections.Generic;
using UnityEngine;

public class SunsetWater : MonoBehaviour
{
	public List<BuoyantObject> floatingObjects;

	private void Awake()
	{
		floatingObjects = new List<BuoyantObject>();
	}

	private void FixedUpdate()
	{
	}

	private void OnTriggerEnter(Collider obj)
	{
		BuoyantObject component = obj.GetComponent<BuoyantObject>();
		if (component != null)
		{
			floatingObjects.Add(component);
			component.SetWater(this);
		}
	}

	private void OnTriggerExit(Collider obj)
	{
	}

	private void SureWhyNOt()
	{
	}
}
