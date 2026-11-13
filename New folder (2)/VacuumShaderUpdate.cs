using UnityEngine;

[ExecuteInEditMode]
public class VacuumShaderUpdate : MonoBehaviour
{
	public Material[] windMaterials;

	private Material currentMaterial;

	private void LateUpdate()
	{
		if (Application.isPlaying)
		{
			return;
		}
		EagerMaster.instance.vacuuumPosition = transform.position;
		if (windMaterials.Length > 0)
		{
			for (int i = 0; i < windMaterials.Length; i++)
			{
				windMaterials[i].SetVector("_VacuumPosition", EagerMaster.instance.vacuuumPosition);
			}
		}
	}

	private void OnApplicationQuit()
	{
		enabled = true;
	}
}
