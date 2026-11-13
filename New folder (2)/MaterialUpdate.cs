using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MaterialUpdate
{
	public Material sharedMaterial;

	public List<MaterialPropertyValue> property = new List<MaterialPropertyValue>();

	[HideInInspector]
	public Material vacuumVariant;

	public void RefreshVacuumMat()
	{
		if (Application.isPlaying && !(vacuumVariant != null) && MaterialManager.HasVacuumMaterial(sharedMaterial))
		{
			vacuumVariant = MaterialManager.GetMaterialVariation(sharedMaterial, MaterialVariations.Vacuum);
		}
	}
}
