using System;
using GVR.Gfx;
using UnityEngine;

public class LoginDayNightColors : MonoBehaviour
{
	[SerializeField]
	private EnvironmentProperties properties;

	[Header("EnvironmentProperty Gradients:")]
	[SerializeField]
	private Gradient nearHorizonGradient = new Gradient();

	[SerializeField]
	private Gradient farHorizonGradient = new Gradient();

	[SerializeField]
	private Gradient nearApexGradient = new Gradient();

	[SerializeField]
	private Gradient farApexGradient = new Gradient();

	[SerializeField]
	private Gradient sceneLightGradient = new Gradient();

	[SerializeField]
	private Gradient sceneShadowGradient = new Gradient();

	[Header("Material Gradients:")]
	[SerializeField]
	private Gradient glowCardGradient = new Gradient();

	[SerializeField]
	private Material glowCardMaterial;

	[Header("Hactivitude Properties:")]
	[SerializeField]
	private Color hacktivitudeNearHorizon = Color.white;

	[SerializeField]
	private Color hacktivitudeFarHorizon = Color.white;

	[SerializeField]
	private Color hacktivitudeNearApex = Color.white;

	[SerializeField]
	private Color hacktivitudeFarApex = Color.white;

	[SerializeField]
	private Color hacktivitudeSceneLight = Color.white;

	[SerializeField]
	private Color hacktivitudeSceneShadow = Color.white;

	[SerializeField]
	private Color hacktivitudeGlowCard = Color.white;

	[SerializeField]
	private VVR_Point hacktivitudePointLight;

	[SerializeField]
	private Transform hacktivitudePointLightAnchor;

	[SerializeField]
	private float hacktivitudePointLightIntensity = 1f;

	[SerializeField]
	private float hacktivitudePointLightRange = 1f;

	[Header("Time Debug")]
	[SerializeField]
	private bool useDebugT;

	[SerializeField]
	private float debugT;

	private float dayNightOffsetT;

	private bool hacktivitudeOverrideMode;

	public void ActivateHacktivitudeOverride()
	{
		hacktivitudeOverrideMode = true;
		hacktivitudePointLight.transform.position = hacktivitudePointLightAnchor.position;
		hacktivitudePointLight.transform.SetParent(hacktivitudePointLightAnchor);
		hacktivitudePointLight.intensity = hacktivitudePointLightIntensity;
		hacktivitudePointLight.range = hacktivitudePointLightRange;
	}

	public void ApplyGradientColors(float normalizedTime)
	{
		if (properties != null)
		{
			if (hacktivitudeOverrideMode)
			{
				properties.nearHorizonColor = hacktivitudeNearHorizon;
				properties.farHorizonColor = hacktivitudeFarHorizon;
				properties.nearApexColor = hacktivitudeNearApex;
				properties.farApexColor = hacktivitudeFarApex;
				properties.sceneLightColor = hacktivitudeSceneLight;
				properties.sceneShadowColor = hacktivitudeSceneShadow;
			}
			else
			{
				properties.nearHorizonColor = nearHorizonGradient.Evaluate(normalizedTime);
				properties.farHorizonColor = farHorizonGradient.Evaluate(normalizedTime);
				properties.nearApexColor = nearApexGradient.Evaluate(normalizedTime);
				properties.farApexColor = farApexGradient.Evaluate(normalizedTime);
				properties.sceneLightColor = sceneLightGradient.Evaluate(normalizedTime);
				properties.sceneShadowColor = sceneShadowGradient.Evaluate(normalizedTime);
			}
			properties.Activate();
		}
		if (glowCardMaterial != null)
		{
			if (hacktivitudeOverrideMode)
			{
				glowCardMaterial.SetColor("_Color", hacktivitudeGlowCard);
			}
			else
			{
				glowCardMaterial.SetColor("_Color", glowCardGradient.Evaluate(normalizedTime));
			}
		}
	}

	public void SetTimeOffset(float offsetTIn)
	{
		dayNightOffsetT = offsetTIn;
	}

	private void Update()
	{
		float normalizedTime = ((!useDebugT) ? (RealWorldTimeUtility.NormalizeTime(DateTime.Now) + dayNightOffsetT) : debugT) % 1f;
		ApplyGradientColors(normalizedTime);
	}
}
