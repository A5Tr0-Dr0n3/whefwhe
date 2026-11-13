using System;
using System.Collections.Generic;
using GVR.Gfx;
using TButt;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ElizaEnvironments : MonoBehaviour
{
	[Header("References")]
	public ElizaEnvironmentProperties enviroPropertiesBeach;

	public ElizaEnvironmentProperties enviroPropertiesBeachSunset;

	public ElizaEnvironmentProperties enviroPropertiesVolcano;

	public ElizaEnvironmentProperties enviroPropertiesVolcanoComplete;

	public ElizaEnvironmentProperties enviroPropertiesCanyon;

	public ElizaEnvironmentProperties enviroPropertiesSpace;

	public ElizaEnvironmentProperties enviroPropertiesCave;

	public ElizaEnvironmentProperties enviroPropertiesCenter;

	public ElizaEnvironmentProperties enviroPropertiesFinalShell;

	public ElizaEnvironmentProperties enviroPropertiesCenterSpotLight;

	public ElizaEnvironmentProperties enviroPropertiesPostGlitch;

	[HideInInspector]
	public ElizaEnvironmentProperties currentPropperties;

	private bool hasSeenBeach;

	private bool hasSeenVolcano;

	private bool hasSeenCenter;

	private bool hasSeenCenterOff;

	private bool hasSeenCanyon;

	private bool hasSeenSpace;

	private bool hasSeenCave;

	private VVR_Eliza elizaScriptReference;

	public GameObject sunReference;

	public EnvironmentProperties envProps;

	[Header("Environment References")]
	public float beachCameraAngle;

	public Transform beachSunPositionUp;

	public Transform beachSunPositionDown;

	public Transform beachSunPositionSet;

	public float volcanoCameraAngle;

	public float canyonCameraAngle;

	public float spaceCameraAngle;

	public float caveCameraAngle;

	private int currentPosition = -1;

	[Header("Tweakables")]
	public float cameraRange = 36f;

	public float sunMoveTime = 1f;

	private float sunMoveCount;

	private Vector3 sunStartPosition;

	private Vector3 sunTargetPosition;

	private float t;

	[HideInInspector]
	public bool useCameraAngle = true;

	private float currentAngle;

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

	public ElizaEnvironmentTrigger centerTrigger;

	private ElizaEnvironmentTrigger _endingTrigger;

	private bool _beachComplete;

	private bool _caveComplete;

	private bool _canyonComplete;

	private bool _spaceComplete;

	private bool _volcanoComplete;

	private bool endScene;

	public VVR_Eliza elizaReference => elizaScriptReference;

	public ElizaEnvironmentTrigger endingTrigger => _endingTrigger;

	public bool beachComplete => _beachComplete;

	public bool caveComplete => _caveComplete;

	public bool canyonComplete => _canyonComplete;

	public bool spaceComplete => _spaceComplete;

	public bool volcanoComplete => _volcanoComplete;

	private void Start()
	{
		StartSunMove(beachSunPositionDown.position, immediate: true);
		elizaScriptReference = GetComponent<VVR_Eliza>();
	}

	private void Update()
	{
		if (sunMoveCount < sunMoveTime)
		{
			handleEnvironmentSwitch();
		}
		if (lerpNewVariablesCount < lerpNewVariablesTime)
		{
			handleValueChange();
		}
		if (!useCameraAngle)
		{
			return;
		}
		currentAngle = EagerMaster.instance.mainCamera.transform.rotation.eulerAngles.y;
		if (currentPosition != 0 && currentAngle >= beachCameraAngle - cameraRange && currentAngle < beachCameraAngle + cameraRange)
		{
			if (_beachComplete)
			{
				changePresetBeach();
				elizaScriptReference.FadeInMusic();
			}
			else
			{
				changePresetCenterOff();
				elizaScriptReference.FadeOutMusic();
			}
		}
		if (currentPosition != 1 && currentAngle >= volcanoCameraAngle - cameraRange && currentAngle < volcanoCameraAngle + cameraRange)
		{
			if (_volcanoComplete)
			{
				changePresetVolcano();
				elizaScriptReference.FadeInMusic();
			}
			else
			{
				changePresetCenterOff();
				elizaScriptReference.FadeOutMusic();
			}
		}
		if (currentPosition != 2 && currentAngle >= canyonCameraAngle - cameraRange && currentAngle < canyonCameraAngle + cameraRange)
		{
			if (_canyonComplete)
			{
				changePresetCanyon();
				elizaScriptReference.FadeInMusic();
			}
			else
			{
				changePresetCenterOff();
				elizaScriptReference.FadeOutMusic();
			}
		}
		if (currentPosition != 3 && currentAngle >= spaceCameraAngle - cameraRange && currentAngle < spaceCameraAngle + cameraRange)
		{
			if (_spaceComplete)
			{
				changePresetSpace();
				elizaScriptReference.FadeInMusic();
			}
			else
			{
				changePresetCenterOff();
				elizaScriptReference.FadeOutMusic();
			}
		}
		if (currentPosition != 4 && currentAngle >= caveCameraAngle - cameraRange && currentAngle < caveCameraAngle + cameraRange)
		{
			if (_caveComplete)
			{
				changePresetCave();
				elizaScriptReference.FadeInMusic();
			}
			else
			{
				changePresetCenterOff();
				elizaScriptReference.FadeOutMusic();
			}
		}
	}

	private void StartSunMove(Vector3 targetPos, bool immediate = false)
	{
		if (!immediate)
		{
			sunTargetPosition = targetPos;
			sunStartPosition = sunReference.transform.position;
			sunMoveCount = 0f;
		}
		else
		{
			sunTargetPosition = targetPos;
			sunStartPosition = sunReference.transform.position;
			sunMoveCount = sunMoveTime - 0.1f;
		}
	}

	private void handleEnvironmentSwitch()
	{
		sunMoveCount += Time.deltaTime;
		if (sunMoveCount >= sunMoveTime)
		{
			sunMoveCount = sunMoveTime;
		}
		t = sunMoveCount / sunMoveTime;
		t = Mathf.Sin(t * (float)Math.PI * 0.5f);
		sunReference.transform.position = Vector3.Lerp(sunStartPosition, sunTargetPosition, t);
	}

	private void changePreset(int newPosition, bool immediate = false, bool noMusic = false)
	{
		switch (newPosition)
		{
		case 0:
			changePresetBeach(noMusic);
			break;
		case 1:
			changePresetVolcano(noMusic);
			break;
		case 2:
			changePresetCanyon(noMusic);
			break;
		case 3:
			changePresetSpace(noMusic);
			break;
		case 4:
			changePresetCave(noMusic);
			break;
		case 5:
			changePresetCenter(immediate, noMusic);
			break;
		case 6:
			changePresetCaveComplete();
			break;
		case 7:
			changePresetVolcanoComplete();
			break;
		case 8:
			changePresetSpaceComplete();
			break;
		case 9:
			changePresetCanyonComplete();
			break;
		case 10:
			changePresetBeachComplete();
			break;
		case 11:
			changePresetCenterOff(immediate);
			break;
		case 12:
			changePresetFinalShell();
			break;
		case 13:
			changePresetStart();
			break;
		case 14:
			changePresetCenterSpotlight();
			break;
		}
	}

	private void changePresetCenter(bool immediate = false, bool noMusic = false)
	{
		if (currentPosition != 5)
		{
			if (currentPosition == 0)
			{
				StartSunMove(beachSunPositionDown.position, immediate);
			}
			MatchEnvironment(sunMoveTime, enviroPropertiesCenter, hasSeenCenter);
			currentPosition = 5;
			hasSeenCenter = true;
			if (elizaScriptReference.musicHandler != null && !noMusic)
			{
				elizaScriptReference.musicHandler.ResetAdBools();
			}
		}
	}

	private void changePresetCenterSpotlight(bool immediate = false, bool noMusic = false)
	{
		if (currentPosition != 5)
		{
			if (currentPosition == 0)
			{
				StartSunMove(beachSunPositionDown.position, immediate);
			}
			MatchEnvironment(sunMoveTime * 2f, enviroPropertiesCenterSpotLight, hasSeenCenter);
			currentPosition = 5;
			hasSeenCenter = true;
			if (elizaScriptReference.musicHandler != null && !noMusic)
			{
				elizaScriptReference.musicHandler.ResetAdBools();
				elizaScriptReference.musicHandler.playCenterLoop = true;
			}
		}
	}

	private void changePresetCenterOff(bool immediate = false)
	{
		if (currentPosition != 11)
		{
			if (currentPosition == 0)
			{
				StartSunMove(beachSunPositionDown.position, immediate);
			}
			MatchEnvironment(sunMoveTime, enviroPropertiesCenter, hasSeenCenterOff);
			currentPosition = 11;
			hasSeenCenterOff = true;
			if (elizaScriptReference == null)
			{
				elizaScriptReference = GetComponent<VVR_Eliza>();
			}
			if (elizaScriptReference.musicHandler != null)
			{
				elizaScriptReference.musicHandler.ResetAdBools();
			}
		}
	}

	private void changePresetBeach(bool noMusic = false)
	{
		if (currentPosition != 0)
		{
			currentPosition = 0;
		}
		if (!VVR_Eliza.instance.allowEnvironmentChange)
		{
			StartSunMove(beachSunPositionSet.position);
			if (!endScene)
			{
				MatchEnvironment(sunMoveTime, enviroPropertiesBeach);
			}
			else
			{
				MatchEnvironment(sunMoveTime * 2.5f, enviroPropertiesBeachSunset, hasSeenBeach);
			}
			if (!noMusic)
			{
				elizaScriptReference.musicHandler.StopAllAds();
			}
			return;
		}
		StartSunMove(beachSunPositionUp.position);
		if (!endScene)
		{
			MatchEnvironment(sunMoveTime, enviroPropertiesBeach, hasSeenBeach);
		}
		else
		{
			MatchEnvironment(sunMoveTime * 2.5f, enviroPropertiesBeachSunset, hasSeenBeach);
		}
		hasSeenBeach = true;
		if (!noMusic)
		{
			elizaScriptReference.musicHandler.ResetAdBools();
			elizaScriptReference.musicHandler.playMermanLoop = true;
		}
	}

	private void changePresetVolcano(bool noMusic = false)
	{
		if (currentPosition != 1)
		{
			if (currentPosition == 0)
			{
				StartSunMove(beachSunPositionDown.position);
			}
			MatchEnvironment(sunMoveTime, enviroPropertiesVolcano, hasSeenVolcano);
			currentPosition = 1;
			hasSeenVolcano = true;
			if (!noMusic)
			{
				elizaScriptReference.musicHandler.ResetAdBools();
				elizaScriptReference.musicHandler.playVesuviusLoop = true;
			}
		}
	}

	private void changePresetCanyon(bool noMusic = false)
	{
		if (currentPosition != 2)
		{
			if (currentPosition == 0)
			{
				StartSunMove(beachSunPositionDown.position);
			}
			MatchEnvironment(sunMoveTime, enviroPropertiesCanyon, hasSeenCanyon);
			currentPosition = 2;
			hasSeenCanyon = true;
			if (!noMusic)
			{
				elizaScriptReference.musicHandler.ResetAdBools();
				elizaScriptReference.musicHandler.playBalloonLoop = true;
			}
		}
	}

	private void changePresetStart()
	{
		if (currentPosition != 2)
		{
			if (currentPosition == 0)
			{
				StartSunMove(beachSunPositionDown.position);
			}
			MatchEnvironment(sunMoveTime, enviroPropertiesCenterSpotLight, resetLights: true);
			currentPosition = 2;
		}
	}

	private void changePresetSpace(bool noMusic = false)
	{
		if (currentPosition != 3)
		{
			if (currentPosition == 0)
			{
				StartSunMove(beachSunPositionDown.position);
			}
			MatchEnvironment(sunMoveTime, enviroPropertiesSpace, hasSeenSpace);
			currentPosition = 3;
			hasSeenSpace = true;
			if (!noMusic)
			{
				elizaScriptReference.musicHandler.ResetAdBools();
				elizaScriptReference.musicHandler.playCosmicLoop = true;
			}
		}
	}

	private void changePresetCave(bool noMusic = false)
	{
		if (currentPosition != 4)
		{
			if (currentPosition == 0)
			{
				StartSunMove(beachSunPositionDown.position);
			}
			MatchEnvironment(sunMoveTime, enviroPropertiesCave, hasSeenCave);
			currentPosition = 4;
			hasSeenCave = true;
			if (!noMusic)
			{
				elizaScriptReference.musicHandler.ResetAdBools();
				elizaScriptReference.musicHandler.playBurrowsLoop = true;
			}
		}
	}

	private void changePresetCaveComplete()
	{
		if (currentPosition != 6)
		{
			if (currentPosition == 0)
			{
				StartSunMove(beachSunPositionDown.position);
			}
			MatchEnvironment(sunMoveTime, enviroPropertiesCave);
			currentPosition = 6;
			elizaScriptReference.musicHandler.ResetAdBools();
			elizaScriptReference.musicHandler.playBurrowsLoop = true;
		}
	}

	private void changePresetVolcanoComplete()
	{
		if (currentPosition != 7)
		{
			if (currentPosition == 0)
			{
				StartSunMove(beachSunPositionDown.position);
			}
			MatchEnvironment(sunMoveTime, enviroPropertiesVolcanoComplete);
			currentPosition = 7;
			elizaScriptReference.musicHandler.ResetAdBools();
			elizaScriptReference.musicHandler.playVesuviusLoop = true;
		}
	}

	private void changePresetSpaceComplete()
	{
		if (currentPosition != 8)
		{
			if (currentPosition == 0)
			{
				StartSunMove(beachSunPositionDown.position);
			}
			MatchEnvironment(sunMoveTime, enviroPropertiesSpace);
			currentPosition = 8;
			elizaScriptReference.musicHandler.ResetAdBools();
			elizaScriptReference.musicHandler.playCosmicLoop = true;
		}
	}

	private void changePresetCanyonComplete()
	{
		if (currentPosition != 9)
		{
			if (currentPosition == 0)
			{
				StartSunMove(beachSunPositionDown.position);
			}
			MatchEnvironment(sunMoveTime, enviroPropertiesCanyon);
			currentPosition = 9;
			elizaScriptReference.musicHandler.ResetAdBools();
			elizaScriptReference.musicHandler.playBalloonLoop = true;
		}
	}

	private void changePresetBeachComplete()
	{
		if (currentPosition != 10)
		{
			if (currentPosition == 0)
			{
				StartSunMove(beachSunPositionDown.position);
			}
			MatchEnvironment(sunMoveTime, enviroPropertiesBeach);
			currentPosition = 10;
			elizaScriptReference.musicHandler.ResetAdBools();
			elizaScriptReference.musicHandler.playMermanLoop = true;
		}
	}

	private void changePresetFinalShell()
	{
		if (currentPosition != 12)
		{
			if (currentPosition == 0)
			{
				StartSunMove(beachSunPositionDown.position);
			}
			MatchEnvironment(sunMoveTime, enviroPropertiesFinalShell);
			currentPosition = 12;
		}
	}

	public void SetNewArea(int newArea, bool immediate = false, bool noMusic = false)
	{
		if (newArea == -1)
		{
			VVR_Eliza.instance.allowEnvironmentChange = false;
			endScene = true;
			changePreset(0);
			SetUseCamera(newC: false);
		}
		else if (VVR_Eliza.instance.allowEnvironmentChange)
		{
			changePreset(newArea, immediate, noMusic);
			SetUseCamera(newC: false);
		}
	}

	public void SetUseCamera(bool newC)
	{
		if (VVR_Eliza.instance.allowEnvironmentChange)
		{
			useCameraAngle = newC;
		}
		else
		{
			useCameraAngle = false;
		}
	}

	public void MatchEnvironment(float lTime, ElizaEnvironmentProperties newProperties, bool resetLights = false)
	{
		if (!(currentPropperties == newProperties))
		{
			currentPropperties = newProperties;
			ChangeValues(lTime, newProperties.fullOpacityDistanceMax, newProperties.nearHorizonColor, newProperties.farHorizonColor, newProperties.horizonColorPivotDistanceMax, newProperties.horizonColorBlendLengthMax, newProperties.nearApexColor, newProperties.farApexColor, newProperties.apexFullOpacityDistanceMax, newProperties.apexColorPivotDistanceMax, newProperties.apexColorBlendLengthMax, newProperties.sceneLightColor, newProperties.sceneShadowColor, newProperties.pointLights, newProperties.desktopLights, newProperties.directionalLights, newProperties.environmentProfile, resetLights);
		}
	}

	public void SetCenterTrigger(ElizaEnvironmentTrigger newC)
	{
		centerTrigger = newC;
	}

	public void SetEndingTrigger(ElizaEnvironmentTrigger newC)
	{
		_endingTrigger = newC;
	}

	public void CompleteBeach()
	{
		_beachComplete = true;
	}

	public void CompleteSpace()
	{
		_spaceComplete = true;
	}

	public void CompleteCanyon()
	{
		_canyonComplete = true;
	}

	public void CompleteCave()
	{
		_caveComplete = true;
	}

	public void CompleteVolcano()
	{
		_volcanoComplete = true;
	}

	private void ChangeValues(float lerpTime, float fullOpacityDistance, Color newHorizonColorNear, Color newHorizonColorFar, float newHorizonColorPivotDistance, float newHorizonColorBlendLength, Color newApexColorNear, Color newApexColorFar, float newApexFullOpacityDistance, float newApexColorPivotDistance, float newApexColorBlendLength, Color newSceneLightColor, Color newSceneShadowColor, List<VVR_Point> newPointLights, List<DesktopLight> newDesktopLights, List<DesktopDirectionalLight> newDirectionalLights, PostProcessProfile newProfile, bool resetLights)
	{
		lerpNewVariablesTime = lerpTime;
		lerpNewVariablesCount = 0f;
		fullOpacityDistance_FROM = envProps.fullOpacityDistance;
		fullOpacityDistance_TO = fullOpacityDistance;
		nearHorizonColor_FROM = envProps.nearHorizonColor;
		nearHorizonColor_TO = newHorizonColorNear;
		farHorizonColor_FROM = envProps.farHorizonColor;
		farHorizonColor_TO = newHorizonColorFar;
		horizonColorPivotDistance_FROM = envProps.horizonColorPivotDistance;
		horizonColorPivotDistance_TO = newHorizonColorPivotDistance;
		horizonColorBlendLength_FROM = envProps.horizonColorBlendLength;
		horizonColorBlendLength_TO = newHorizonColorBlendLength;
		nearApexColor_FROM = envProps.nearApexColor;
		nearApexColor_TO = newApexColorNear;
		farApexColor_FROM = envProps.farApexColor;
		farApexColor_TO = newApexColorFar;
		apexFullOpacityDistance_FROM = envProps.apexFullOpacityDistance;
		apexFullOpacityDistance_TO = newApexFullOpacityDistance;
		apexColorPivotDistance_FROM = envProps.apexColorPivotDistance;
		apexColorPivotDistance_TO = newApexColorPivotDistance;
		apexColorBlendLength_FROM = envProps.apexColorBlendLength;
		apexColorBlendLength_TO = newApexColorBlendLength;
		sceneLightColor_FROM = envProps.sceneLightColor;
		sceneLightColor_TO = newSceneLightColor;
		sceneShadowColor_FROM = envProps.sceneShadowColor;
		sceneShadowColor_TO = newSceneShadowColor;
		for (int i = 0; i < envProps.pointLights.Count; i++)
		{
			envProps.pointLights[i].gameObject.SetActive(value: false);
		}
		for (int j = 0; j < envProps.directionalLights.Count; j++)
		{
			envProps.directionalLights[j].gameObject.SetActive(value: false);
		}
		for (int k = 0; k < envProps.desktopLights.Count; k++)
		{
			envProps.desktopLights[k].gameObject.SetActive(value: false);
		}
		envProps.pointLights = newPointLights;
		envProps.desktopLights = newDesktopLights;
		envProps.directionalLights = newDirectionalLights;
		for (int l = 0; l < envProps.pointLights.Count; l++)
		{
			if (resetLights)
			{
				envProps.pointLights[l].intensity = (envProps.pointLights[l].range = 0f);
			}
			envProps.pointLights[l].gameObject.SetActive(value: true);
		}
		if (TBCore.GetActivePlatform() == VRPlatform.OculusMobile)
		{
			return;
		}
		for (int m = 0; m < envProps.desktopLights.Count; m++)
		{
			envProps.desktopLights[m].gameObject.SetActive(value: true);
			if (resetLights && envProps.desktopLights[m].mLight != null)
			{
				envProps.desktopLights[m].mLight.intensity = 0f;
				envProps.desktopLights[m].mLight.range = 0f;
			}
		}
		for (int n = 0; n < envProps.directionalLights.Count; n++)
		{
			envProps.directionalLights[n].gameObject.SetActive(value: true);
			if (resetLights && envProps.directionalLights[n].mLight != null)
			{
				envProps.directionalLights[n].mLight.intensity = 0f;
				envProps.directionalLights[n].mLight.range = 0f;
			}
		}
		envProps.postProcessProfile = newProfile;
	}

	private void handleValueChange()
	{
		lerpNewVariablesCount += Time.deltaTime;
		if (lerpNewVariablesCount > lerpNewVariablesTime)
		{
			lerpNewVariablesCount = lerpNewVariablesTime;
		}
		enviroT = lerpNewVariablesCount / lerpNewVariablesTime;
		enviroT = Mathf.Sin(enviroT * (float)Math.PI * 0.5f);
		envProps.fullOpacityDistance = Mathf.Lerp(fullOpacityDistance_FROM, fullOpacityDistance_TO, enviroT);
		envProps.nearHorizonColor = Color.Lerp(nearHorizonColor_FROM, nearHorizonColor_TO, enviroT);
		envProps.farHorizonColor = Color.Lerp(farHorizonColor_FROM, farHorizonColor_TO, enviroT);
		envProps.horizonColorPivotDistance = Mathf.Lerp(horizonColorPivotDistance_FROM, horizonColorPivotDistance_TO, enviroT);
		envProps.horizonColorBlendLength = Mathf.Lerp(horizonColorBlendLength_FROM, horizonColorBlendLength_TO, enviroT);
		envProps.nearApexColor = Color.Lerp(nearApexColor_FROM, nearApexColor_TO, enviroT);
		envProps.farApexColor = Color.Lerp(farApexColor_FROM, farApexColor_TO, enviroT);
		envProps.apexFullOpacityDistance = Mathf.Lerp(apexFullOpacityDistance_FROM, apexFullOpacityDistance_TO, enviroT);
		envProps.apexColorPivotDistance = Mathf.Lerp(apexColorPivotDistance_FROM, apexColorPivotDistance_TO, enviroT);
		envProps.apexColorBlendLength = Mathf.Lerp(apexColorBlendLength_FROM, apexColorBlendLength_TO, enviroT);
		envProps.sceneLightColor = Color.Lerp(sceneLightColor_FROM, sceneLightColor_TO, enviroT);
		envProps.sceneShadowColor = Color.Lerp(sceneShadowColor_FROM, sceneShadowColor_TO, enviroT);
		for (int i = 0; i < envProps.pointLights.Count; i++)
		{
			envProps.pointLights[i].LerpValues(enviroT);
		}
		if (TBCore.GetActivePlatform() != VRPlatform.OculusMobile)
		{
			for (int j = 0; j < envProps.desktopLights.Count; j++)
			{
				envProps.desktopLights[j].LerpValues(enviroT);
			}
			for (int k = 0; k < envProps.directionalLights.Count; k++)
			{
				envProps.directionalLights[k].LerpValues(enviroT);
			}
		}
		envProps.pushPointLights();
		envProps.pushSettings();
	}
}
