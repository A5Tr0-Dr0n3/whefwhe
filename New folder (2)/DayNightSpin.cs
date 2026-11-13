using UnityEngine;

public class DayNightSpin : MonoBehaviour
{
	private Material myMaterial;

	private int huePropertyID;

	private int satPropertyID;

	private int brightPropertyID;

	private int contrastPropertyID;

	public GameObject turbine;

	public GameObject celestialBodies;

	[CurveRange(0f, 360f, 0f, 1f)]
	public AnimationCurve _hueCurve;

	[CurveRange(0f, 360f, 0f, 1f)]
	public AnimationCurve _satCurve;

	[CurveRange(0f, 360f, 0f, 1f)]
	public AnimationCurve _brightCurve;

	[CurveRange(0f, 360f, 0f, 1f)]
	public AnimationCurve _contrastCurve;

	private void Start()
	{
		myMaterial = turbine.GetComponent<Renderer>().sharedMaterial;
		huePropertyID = Shader.PropertyToID("_Hue");
		satPropertyID = Shader.PropertyToID("_Saturation");
		brightPropertyID = Shader.PropertyToID("_Brightness");
		contrastPropertyID = Shader.PropertyToID("_Contrast");
	}

	private void Update()
	{
		float z = transform.localEulerAngles.z;
		myMaterial.SetFloat(huePropertyID, _hueCurve.Evaluate(z));
		myMaterial.SetFloat(satPropertyID, _satCurve.Evaluate(z));
		myMaterial.SetFloat(brightPropertyID, _brightCurve.Evaluate(z));
		myMaterial.SetFloat(contrastPropertyID, _contrastCurve.Evaluate(z));
		celestialBodies.transform.localEulerAngles = new Vector3(0f - z - 50f, 20f, 0f);
	}
}
