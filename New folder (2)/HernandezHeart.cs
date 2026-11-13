using UnityEngine;

public class HernandezHeart : MonoBehaviour
{
	private float fillPercentage = 1f;

	private float seed = 1f;

	private static int seedPropID;

	private static int suckPropID;

	private static int vacPowerPropID;

	private static int vacRangePropID;

	private static int vacPosPropID;

	private static int glimmerTransPropID;

	private MaterialPropertyBlock block;

	public Renderer oRenderer { get; private set; }

	public void SetFill(float newFill)
	{
		fillPercentage = newFill;
	}

	private void Start()
	{
		oRenderer = GetComponent<Renderer>();
		block = new MaterialPropertyBlock();
		seedPropID = Shader.PropertyToID("_Seed");
		suckPropID = Shader.PropertyToID("_SuckUp");
		vacPowerPropID = Shader.PropertyToID("_VacuumPower");
		vacPosPropID = Shader.PropertyToID("_VacuumPosition");
		vacRangePropID = Shader.PropertyToID("_VacuumRange");
		glimmerTransPropID = Shader.PropertyToID("_Alpha");
		block.SetVector(vacPosPropID, Vector3.up * 50f);
		block.SetFloat(vacPowerPropID, 1f);
		block.SetFloat(vacRangePropID, 1f);
		block.SetFloat(seedPropID, seed);
	}

	private void Update()
	{
		block.SetFloat(suckPropID, fillPercentage);
		block.SetFloat(glimmerTransPropID, 1.1f * fillPercentage);
		oRenderer.SetPropertyBlock(block);
	}
}
