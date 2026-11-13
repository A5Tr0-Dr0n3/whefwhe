using UnityEngine;

namespace GoogleVR.GVRDemo;

[RequireComponent(typeof(Collider))]
public class Teleport : MonoBehaviour
{
	private Vector3 startingPosition;

	public Material inactiveMaterial;

	public Material gazedAtMaterial;

	private void Start()
	{
		startingPosition = transform.localPosition;
		SetGazedAt(gazedAt: false);
	}

	public void SetGazedAt(bool gazedAt)
	{
		if (inactiveMaterial != null && gazedAtMaterial != null)
		{
			GetComponent<Renderer>().material = ((!gazedAt) ? inactiveMaterial : gazedAtMaterial);
		}
		else
		{
			GetComponent<Renderer>().material.color = ((!gazedAt) ? Color.red : Color.green);
		}
	}

	public void Reset()
	{
		transform.localPosition = startingPosition;
	}

	public void Recenter()
	{
		GvrCardboardHelpers.Recenter();
	}

	public void TeleportRandomly()
	{
		Vector3 onUnitSphere = Random.onUnitSphere;
		onUnitSphere.y = Mathf.Clamp(onUnitSphere.y, 0.5f, 1f);
		float num = 2f * Random.value + 1.5f;
		transform.localPosition = onUnitSphere * num;
	}
}
