using UnityEngine;

public class hsbglitchfix : MonoBehaviour
{
	private Material myMaterial;

	private int huePropertyID;

	private int satPropertyID;

	private int brightPropertyID;

	private int contrastPropertyID;

	public GameObject celestialBodies;

	public bool alwaysFix;

	private void Start()
	{
		myMaterial = GetComponent<Renderer>().sharedMaterial;
		huePropertyID = Shader.PropertyToID("_Hue");
		satPropertyID = Shader.PropertyToID("_Saturation");
		brightPropertyID = Shader.PropertyToID("_Brightness");
		contrastPropertyID = Shader.PropertyToID("_Contrast");
		myMaterial.SetFloat(huePropertyID, 0f);
		myMaterial.SetFloat(satPropertyID, 0.5f);
		myMaterial.SetFloat(brightPropertyID, 0.5f);
		myMaterial.SetFloat(contrastPropertyID, 0.5f);
	}

	private void Update()
	{
		if (alwaysFix)
		{
			myMaterial.SetFloat(huePropertyID, 0f);
			myMaterial.SetFloat(satPropertyID, 0.5f);
			myMaterial.SetFloat(brightPropertyID, 0.5f);
			myMaterial.SetFloat(contrastPropertyID, 0.5f);
		}
		if ((bool)celestialBodies)
		{
			celestialBodies.transform.localEulerAngles = new Vector3(360f * myMaterial.GetFloat("_Hue") + 70f, -76f, 0f);
		}
	}
}
