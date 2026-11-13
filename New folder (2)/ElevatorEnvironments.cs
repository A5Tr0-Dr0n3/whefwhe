using System.Collections.Generic;
using GVR.Gfx;
using TButt;
using UnityEngine;

public class ElevatorEnvironments : MonoBehaviour
{
	[Header("References")]
	[SerializeField]
	private ElizaEnvironmentProperties[] environments;

	[SerializeField]
	private EnvironmentProperties mainEnvironment;

	private float t;

	private float lerpNewVariablesTime;

	private float lerpNewVariablesCount;

	private float enviroT;

	private float fullOpacityDistance_FROM;

	private float fullOpacityDistance_TO;

	private Color nearHorizonColor_FROM;

	private Color nearHorizonColor_TO;

	private Color farHorizonColor_FROM;

	private Color farHorizonColor_TO;

	private float horizonColorPivotDistance_FROM;

	private float horizonColorPivotDistance_TO;

	private float horizonColorBlendLength_FROM;

	private float horizonColorBlendLength_TO;

	private Color nearApexColor_FROM;

	private Color nearApexColor_TO;

	private Color farApexColor_FROM;

	private Color farApexColor_TO;

	private float apexFullOpacityDistance_FROM;

	private float apexFullOpacityDistance_TO;

	private float apexColorPivotDistance_FROM;

	private float apexColorPivotDistance_TO;

	private float apexColorBlendLength_FROM;

	private float apexColorBlendLength_TO;

	private Color sceneLightColor_FROM;

	private Color sceneLightColor_TO;

	private Color sceneShadowColor_FROM;

	private Color sceneShadowColor_TO;

	private bool changeLighting;

	private void Start()
	{
	}

	private void Update()
	{
		if (lerpNewVariablesCount < lerpNewVariablesTime)
		{
			handleValueChange();
		}
	}

	public void MatchEnvironment(float lTime, int envIndex, bool changeLighting = true)
	{
		MatchEnvironment(lTime, environments[envIndex], changeLighting);
	}

	public void MatchEnvironment(float lTime, ElizaEnvironmentProperties newProperties, bool changeLighting = true)
	{
		ChangeValues(lTime, newProperties.fullOpacityDistanceMax, newProperties.nearHorizonColor, newProperties.farHorizonColor, newProperties.horizonColorPivotDistanceMax, newProperties.horizonColorBlendLengthMax, newProperties.nearApexColor, newProperties.farApexColor, newProperties.apexFullOpacityDistanceMax, newProperties.apexColorPivotDistanceMax, newProperties.apexColorBlendLengthMax, newProperties.sceneLightColor, newProperties.sceneShadowColor, newProperties.pointLights, newProperties.desktopLights, changeLighting);
	}

	public void MatchPointLights(List<VVR_Point> newPointLights, List<DesktopLight> newDesktopLights)
	{
		for (int i = 0; i < mainEnvironment.pointLights.Count; i++)
		{
			mainEnvironment.pointLights[i].gameObject.SetActive(value: false);
		}
		if (TBCore.GetActivePlatform() != VRPlatform.OculusMobile)
		{
			for (int j = 0; j < mainEnvironment.desktopLights.Count; j++)
			{
				mainEnvironment.desktopLights[j].gameObject.SetActive(value: false);
			}
			for (int k = 0; k < mainEnvironment.directionalLights.Count; k++)
			{
				mainEnvironment.directionalLights[k].gameObject.SetActive(value: false);
			}
		}
		mainEnvironment.pointLights = newPointLights;
		for (int l = 0; l < mainEnvironment.pointLights.Count; l++)
		{
			mainEnvironment.pointLights[l].gameObject.SetActive(value: true);
		}
		mainEnvironment.desktopLights = newDesktopLights;
		if (TBCore.GetActivePlatform() != VRPlatform.OculusMobile)
		{
			for (int m = 0; m < mainEnvironment.desktopLights.Count; m++)
			{
				mainEnvironment.desktopLights[m].gameObject.SetActive(value: true);
			}
			for (int n = 0; n < mainEnvironment.directionalLights.Count; n++)
			{
				mainEnvironment.directionalLights[n].gameObject.SetActive(value: true);
			}
		}
	}

	private void ChangeValues(float lerpTime, float fullOpacityDistance, Color newHorizonColorNear, Color newHorizonColorFar, float newHorizonColorPivotDistance, float newHorizonColorBlendLength, Color newApexColorNear, Color newApexColorFar, float newApexFullOpacityDistance, float newApexColorPivotDistance, float newApexColorBlendLength, Color newSceneLightColor, Color newSceneShadowColor, List<VVR_Point> newPointLights, List<DesktopLight> newDesktopLights, bool changeLighting)
	{
		lerpNewVariablesTime = lerpTime;
		lerpNewVariablesCount = 0f;
		fullOpacityDistance_FROM = mainEnvironment.fullOpacityDistance;
		fullOpacityDistance_TO = fullOpacityDistance;
		nearHorizonColor_FROM = mainEnvironment.nearHorizonColor;
		nearHorizonColor_TO = newHorizonColorNear;
		farHorizonColor_FROM = mainEnvironment.farHorizonColor;
		farHorizonColor_TO = newHorizonColorFar;
		horizonColorPivotDistance_FROM = mainEnvironment.horizonColorPivotDistance;
		horizonColorPivotDistance_TO = newHorizonColorPivotDistance;
		horizonColorBlendLength_FROM = mainEnvironment.horizonColorBlendLength;
		horizonColorBlendLength_TO = newHorizonColorBlendLength;
		nearApexColor_FROM = mainEnvironment.nearApexColor;
		nearApexColor_TO = newApexColorNear;
		farApexColor_FROM = mainEnvironment.farApexColor;
		farApexColor_TO = newApexColorFar;
		apexFullOpacityDistance_FROM = mainEnvironment.apexFullOpacityDistance;
		apexFullOpacityDistance_TO = newApexFullOpacityDistance;
		apexColorPivotDistance_FROM = mainEnvironment.apexColorPivotDistance;
		apexColorPivotDistance_TO = newApexColorPivotDistance;
		apexColorBlendLength_FROM = mainEnvironment.apexColorBlendLength;
		apexColorBlendLength_TO = newApexColorBlendLength;
		sceneLightColor_FROM = mainEnvironment.sceneLightColor;
		sceneLightColor_TO = newSceneLightColor;
		sceneShadowColor_FROM = mainEnvironment.sceneShadowColor;
		sceneShadowColor_TO = newSceneShadowColor;
		this.changeLighting = changeLighting;
		if (changeLighting)
		{
			MatchPointLights(newPointLights, newDesktopLights);
		}
		if (lerpTime == 0f)
		{
			lerpNewVariablesTime = 1f;
			lerpNewVariablesCount = lerpNewVariablesTime;
			handleValueChange();
		}
	}

	private void handleValueChange()
	{
		lerpNewVariablesCount += Time.deltaTime;
		if (lerpNewVariablesCount > lerpNewVariablesTime)
		{
			lerpNewVariablesCount = lerpNewVariablesTime;
		}
		enviroT = Mathf.Clamp01(lerpNewVariablesCount / lerpNewVariablesTime);
		mainEnvironment.fullOpacityDistance = Mathf.Lerp(fullOpacityDistance_FROM, fullOpacityDistance_TO, enviroT);
		mainEnvironment.nearHorizonColor = Color.Lerp(nearHorizonColor_FROM, nearHorizonColor_TO, enviroT);
		mainEnvironment.farHorizonColor = Color.Lerp(farHorizonColor_FROM, farHorizonColor_TO, enviroT);
		mainEnvironment.horizonColorPivotDistance = Mathf.Lerp(horizonColorPivotDistance_FROM, horizonColorPivotDistance_TO, enviroT);
		mainEnvironment.horizonColorBlendLength = Mathf.Lerp(horizonColorBlendLength_FROM, horizonColorBlendLength_TO, enviroT);
		mainEnvironment.nearApexColor = Color.Lerp(nearApexColor_FROM, nearApexColor_TO, enviroT);
		mainEnvironment.farApexColor = Color.Lerp(farApexColor_FROM, farApexColor_TO, enviroT);
		mainEnvironment.apexFullOpacityDistance = Mathf.Lerp(apexFullOpacityDistance_FROM, apexFullOpacityDistance_TO, enviroT);
		mainEnvironment.apexColorPivotDistance = Mathf.Lerp(apexColorPivotDistance_FROM, apexColorPivotDistance_TO, enviroT);
		mainEnvironment.apexColorBlendLength = Mathf.Lerp(apexColorBlendLength_FROM, apexColorBlendLength_TO, enviroT);
		mainEnvironment.sceneLightColor = Color.Lerp(sceneLightColor_FROM, sceneLightColor_TO, enviroT);
		mainEnvironment.sceneShadowColor = Color.Lerp(sceneShadowColor_FROM, sceneShadowColor_TO, enviroT);
		if (changeLighting)
		{
			for (int i = 0; i < mainEnvironment.pointLights.Count; i++)
			{
				mainEnvironment.pointLights[i].LerpValues(enviroT);
			}
			if (TBCore.GetActivePlatform() != VRPlatform.OculusMobile)
			{
				for (int j = 0; j < mainEnvironment.desktopLights.Count; j++)
				{
					mainEnvironment.desktopLights[j].LerpValues(enviroT);
				}
				for (int k = 0; k < mainEnvironment.directionalLights.Count; k++)
				{
					mainEnvironment.directionalLights[k].LerpValues(enviroT);
				}
			}
			mainEnvironment.pushPointLights();
		}
		mainEnvironment.pushSettings();
	}
}
