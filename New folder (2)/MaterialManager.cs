using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour
{
	public Material GhostedMat;

	public Material DefaultVacMat;

	private Dictionary<Material, Material> toVacuumVars;

	private Dictionary<Material, Material> fromVaccuumVars;

	private static MaterialManager instance;

	private bool unlitOn;

	private bool aliasingOn;

	private bool reflOn;

	private bool vacuumOn;

	private void Awake()
	{
		instance = this;
		toVacuumVars = new Dictionary<Material, Material>();
		fromVaccuumVars = new Dictionary<Material, Material>();
		AddVacuumVar(DefaultVacMat);
	}

	public static bool HasVacuumMaterial(Material staticMaterial)
	{
		return staticMaterial != null && instance.toVacuumVars.ContainsKey(staticMaterial);
	}

	public static Material GetMaterialVariation(Material staticMaterial, MaterialVariations variation)
	{
		if (staticMaterial == null)
		{
			staticMaterial = instance.DefaultVacMat;
		}
		if (!instance.toVacuumVars.ContainsKey(staticMaterial))
		{
			AddVacuumVar(staticMaterial);
		}
		switch (variation)
		{
		case MaterialVariations.Ghost:
			return instance.GhostedMat;
		case MaterialVariations.Static:
			return staticMaterial;
		case MaterialVariations.Vacuum:
			return instance.toVacuumVars[staticMaterial];
		default:
			SafeDebug.LogError("Material Manager: Invalid material variation!");
			return null;
		}
	}

	private static void AddVacuumVar(Material staticMaterial)
	{
		bool flag = staticMaterial.IsKeywordEnabled("VACUUM");
		Material material = staticMaterial;
		if (!flag)
		{
			material = new Material(staticMaterial);
			material.EnableKeyword("VACUUM");
			material.SetFloat("_Vac", 1f);
			material.renderQueue++;
		}
		if (!instance.toVacuumVars.ContainsKey(staticMaterial))
		{
			instance.toVacuumVars.Add(staticMaterial, material);
		}
		if (!instance.fromVaccuumVars.ContainsKey(material))
		{
			instance.fromVaccuumVars.Add(material, staticMaterial);
		}
	}

	public void ToggleUnlit()
	{
		unlitOn = !unlitOn;
		ToggleShaderFeature("UNLIT", unlitOn);
	}

	public void ToggleAliasing()
	{
		aliasingOn = !aliasingOn;
		ToggleShaderFeature("ALIAS", aliasingOn);
	}

	public void ToggleReflection()
	{
		reflOn = !reflOn;
		ToggleShaderFeature("USE_REFLECTION", reflOn);
	}

	public void ToggleVacuum()
	{
		vacuumOn = !vacuumOn;
		ToggleShaderFeature("VACUUM", vacuumOn);
	}

	private void ToggleShaderFeature(string feature, bool turnOn)
	{
		MeshRenderer[] array = Object.FindObjectsOfType<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in array)
		{
			if (turnOn)
			{
				meshRenderer.sharedMaterial.EnableKeyword(feature);
			}
			else
			{
				meshRenderer.sharedMaterial.DisableKeyword(feature);
			}
		}
	}
}
