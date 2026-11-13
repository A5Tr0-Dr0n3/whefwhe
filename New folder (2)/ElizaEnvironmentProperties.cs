using System.Collections.Generic;
using GVR.Gfx;
using TButt;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ElizaEnvironmentProperties : MonoBehaviour
{
	[Header("Environment Fog:")]
	public float fullOpacityDistanceMax = 50f;

	public Color nearHorizonColor;

	public Color farHorizonColor;

	public float horizonColorPivotDistanceMax = 5f;

	public float horizonColorBlendLengthMax = 5f;

	public Color nearApexColor;

	public Color farApexColor;

	public float apexFullOpacityDistanceMax = 250f;

	public float apexColorPivotDistanceMax = 250f;

	public float apexColorBlendLengthMax = 400f;

	[Header("Lighting Properties")]
	public Color sceneLightColor;

	public Color sceneShadowColor;

	[Header("Rift Lighting Properties")]
	public List<DesktopLight> desktopLights;

	public List<DesktopDirectionalLight> directionalLights;

	public List<VVR_Point> pointLights = new List<VVR_Point>();

	[Header("Desktop Post Processing")]
	public PostProcessProfile environmentProfile;

	public void MatchSettings(EnvironmentProperties matchProperties)
	{
		fullOpacityDistanceMax = matchProperties.fullOpacityDistance;
		nearHorizonColor = matchProperties.nearHorizonColor;
		farHorizonColor = matchProperties.farHorizonColor;
		horizonColorPivotDistanceMax = matchProperties.horizonColorPivotDistance;
		horizonColorBlendLengthMax = matchProperties.horizonColorBlendLength;
		nearApexColor = matchProperties.nearApexColor;
		farApexColor = matchProperties.farApexColor;
		apexFullOpacityDistanceMax = matchProperties.apexFullOpacityDistance;
		apexColorPivotDistanceMax = matchProperties.apexColorPivotDistance;
		apexColorBlendLengthMax = matchProperties.apexColorBlendLength;
		sceneLightColor = matchProperties.sceneLightColor;
		sceneShadowColor = matchProperties.sceneShadowColor;
		if (TBCore.GetActivePlatform() != VRPlatform.OculusMobile)
		{
			environmentProfile = matchProperties.postProcessProfile;
			if (environmentProfile != null && EagerMaster.instance != null)
			{
				EagerMaster.instance.globalPostProcessVolume.profile = environmentProfile;
			}
		}
	}

	public void FoolProofLightList()
	{
		for (int i = 0; i < pointLights.Count; i++)
		{
			if (pointLights[i] == null)
			{
				pointLights.RemoveAt(i);
				i--;
			}
		}
	}
}
