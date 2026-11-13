using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MaterialUpdateController : MonoBehaviour
{
	public List<MaterialUpdate> materialsToUpdate = new List<MaterialUpdate>();

	private MaterialUpdate workingMaterialUpdate;

	private MaterialPropertyValue workingMaterialProperty;

	public void PushMaterials()
	{
		for (int i = 0; i < materialsToUpdate.Count; i++)
		{
			workingMaterialUpdate = materialsToUpdate[i];
			if (workingMaterialUpdate == null)
			{
				continue;
			}
			for (int j = 0; j < workingMaterialUpdate.property.Count; j++)
			{
				workingMaterialProperty = workingMaterialUpdate.property[j];
				workingMaterialUpdate.RefreshVacuumMat();
				if (workingMaterialProperty == null || !(workingMaterialUpdate.sharedMaterial != null))
				{
					continue;
				}
				switch (workingMaterialProperty.type)
				{
				case MaterialPropertyValue.PropertyType.Color:
					workingMaterialUpdate.sharedMaterial.SetVector(workingMaterialProperty.propID, workingMaterialProperty.color);
					if (workingMaterialUpdate.vacuumVariant != null)
					{
						workingMaterialUpdate.vacuumVariant.SetVector(workingMaterialProperty.propID, workingMaterialProperty.color);
					}
					break;
				case MaterialPropertyValue.PropertyType.Float:
					workingMaterialUpdate.sharedMaterial.SetFloat(workingMaterialProperty.propID, workingMaterialProperty.propFloat);
					if (workingMaterialUpdate.vacuumVariant != null)
					{
						workingMaterialUpdate.vacuumVariant.SetFloat(workingMaterialProperty.propID, workingMaterialProperty.propFloat);
					}
					break;
				case MaterialPropertyValue.PropertyType.Keyword:
					if (workingMaterialProperty.attributeBool)
					{
						workingMaterialUpdate.sharedMaterial.EnableKeyword(workingMaterialProperty.keyword);
					}
					else
					{
						workingMaterialUpdate.sharedMaterial.DisableKeyword(workingMaterialProperty.keyword);
					}
					if (workingMaterialUpdate.vacuumVariant != null)
					{
						if (workingMaterialProperty.attributeBool)
						{
							workingMaterialUpdate.vacuumVariant.EnableKeyword(workingMaterialProperty.keyword);
						}
						else
						{
							workingMaterialUpdate.vacuumVariant.DisableKeyword(workingMaterialProperty.keyword);
						}
					}
					break;
				case MaterialPropertyValue.PropertyType.Texture:
					workingMaterialUpdate.sharedMaterial.SetTexture(workingMaterialProperty.propID, workingMaterialProperty.propTexture);
					if (workingMaterialUpdate.vacuumVariant != null)
					{
						workingMaterialUpdate.vacuumVariant.SetTexture(workingMaterialProperty.propID, workingMaterialProperty.propTexture);
					}
					break;
				case MaterialPropertyValue.PropertyType.Vector:
					workingMaterialUpdate.sharedMaterial.SetVector(workingMaterialProperty.propID, workingMaterialProperty.propVector);
					if (workingMaterialUpdate.vacuumVariant != null)
					{
						workingMaterialUpdate.vacuumVariant.SetVector(workingMaterialProperty.propID, workingMaterialProperty.propVector);
					}
					break;
				}
			}
		}
	}
}
