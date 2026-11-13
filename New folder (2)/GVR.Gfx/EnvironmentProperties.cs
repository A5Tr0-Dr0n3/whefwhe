using System.Collections.Generic;
using TButt;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;

namespace GVR.Gfx;

[ExecuteInEditMode]
public class EnvironmentProperties : MonoBehaviour
{
	[Header("Update Frequency")]
	[SerializeField]
	private bool hasMovingLights;

	[Header("Fog Properties")]
	[FormerlySerializedAs("horizonFogDistance")]
	public float fullOpacityDistance;

	[Space]
	[FormerlySerializedAs("fogHorizonColor")]
	public Color nearHorizonColor;

	[FormerlySerializedAs("horizonColor")]
	public Color farHorizonColor;

	[FormerlySerializedAs("horizonFogPoint")]
	public float horizonColorPivotDistance;

	[FormerlySerializedAs("horizonFogBlendDistance")]
	public float horizonColorBlendLength;

	[Space]
	[FormerlySerializedAs("fogApexColor")]
	public Color nearApexColor;

	[FormerlySerializedAs("apexColor")]
	public Color farApexColor;

	[FormerlySerializedAs("apexFogDistance")]
	public float apexFullOpacityDistance;

	[FormerlySerializedAs("apexFogPoint")]
	public float apexColorPivotDistance;

	[FormerlySerializedAs("apexFogBlendDistance")]
	public float apexColorBlendLength;

	[Space]
	public float fogDensity;

	private int id_nearHorizonColor;

	private int id_nearApexColor;

	private int id_params;

	private int id_farHorizonColor;

	private int id_farApexColor;

	public Texture2D skyTexture;

	[Header("FishEye")]
	public static float _fishEyeStrength;

	private static int id_fishEyeStrength;

	[Header("Lighting Properties")]
	public Color sceneLightColor;

	public Color sceneShadowColor;

	private int id_VVRLightColor;

	private int id_VVRShadowColor;

	private int id_VVR_Light0;

	private int id_VVR_Light1;

	private int id_VVR_Light2;

	private int id_VVR_Light3;

	private int id_VVR_ReflectionMap;

	private int id_VVR_LightAtten;

	public Cubemap reflectionMap;

	public List<VVR_Point> pointLights = new List<VVR_Point>(4);

	[SerializeField]
	[HideInInspector]
	private Shader skyShader;

	public List<DesktopLight> desktopLights = new List<DesktopLight>();

	public List<DesktopDirectionalLight> directionalLights = new List<DesktopDirectionalLight>();

	private static EnvironmentProperties _currentActiveProperties;

	private Vector4 lightAtten = new Vector4(0f, 0f, 0f, 0f);

	private int currentNumLights;

	private Vector4 emptyLight = new Vector4(0f, 0f, 0f, 0f);

	private static bool firstFishPush = true;

	[Header("Post Processing Settings")]
	public PostProcessProfile postProcessProfile;

	public static float fishEyeStrength
	{
		get
		{
			return _fishEyeStrength;
		}
		set
		{
			_fishEyeStrength = value;
			pushFishEye();
		}
	}

	private void Start()
	{
	}

	private void Awake()
	{
		id_nearHorizonColor = Shader.PropertyToID("_NearHorizonColor");
		id_nearApexColor = Shader.PropertyToID("_NearApexColor");
		id_farHorizonColor = Shader.PropertyToID("_FarHorizonColor");
		id_farApexColor = Shader.PropertyToID("_FarApexColor");
		id_params = Shader.PropertyToID("_FogParams");
		id_VVRLightColor = Shader.PropertyToID("_VVRLightColor");
		id_VVRShadowColor = Shader.PropertyToID("_VVRShadowColor");
		id_VVR_Light0 = Shader.PropertyToID("_VVR_Light0");
		id_VVR_Light1 = Shader.PropertyToID("_VVR_Light1");
		id_VVR_Light2 = Shader.PropertyToID("_VVR_Light2");
		id_VVR_Light3 = Shader.PropertyToID("_VVR_Light3");
		id_VVR_LightAtten = Shader.PropertyToID("_VVR_LightAtten");
		id_VVR_ReflectionMap = Shader.PropertyToID("_VVR_ReflectionMap");
		id_fishEyeStrength = Shader.PropertyToID("_FishEyeStrength");
	}

	private void ValidateSkybox()
	{
		if (skyShader == null)
		{
			skyShader = Shader.Find("GVR/Sky");
		}
		if (RenderSettings.skybox == null || RenderSettings.skybox.shader != skyShader)
		{
			SafeDebug.LogWarning("Auto-Generated Sky Material.");
			Material material = new Material(skyShader);
			material.name = "Auto-Generated Sky Material";
			RenderSettings.skybox = material;
		}
	}

	private void Update()
	{
		if (hasMovingLights && _currentActiveProperties == this)
		{
			pushPointLights();
		}
	}

	public void Activate()
	{
		FoolProofLightList();
		pushAllProperties();
		if (Application.isPlaying && EagerMaster.instance != null)
		{
			EagerMaster.instance.globalPostProcessVolume.profile = postProcessProfile;
		}
		_currentActiveProperties = this;
	}

	public static void UpdateActivePointLights()
	{
		if (_currentActiveProperties != null)
		{
			_currentActiveProperties.pushPointLights();
		}
	}

	public static void AddPointLightToActiveEnvironment(VVR_Point point, bool forceIndex0 = false)
	{
		if (_currentActiveProperties != null)
		{
			_currentActiveProperties.addPointLight(point, forceIndex0);
		}
	}

	public static void RemovePointLightFromActiveEnvironment(VVR_Point point)
	{
		if (_currentActiveProperties != null)
		{
			_currentActiveProperties.removePointLight(point);
		}
	}

	private void addPointLight(VVR_Point point, bool forceIndex0 = false)
	{
		if (!_currentActiveProperties.pointLights.Contains(point))
		{
			if (forceIndex0)
			{
				_currentActiveProperties.pointLights.Insert(0, point);
			}
			else
			{
				_currentActiveProperties.pointLights.Add(point);
			}
		}
	}

	private void removePointLight(VVR_Point point)
	{
		pointLights.Remove(point);
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

	public void pushAllProperties()
	{
		pushFishEye();
		pushSettings();
		pushPointLights();
		pushSkyTex();
	}

	private static void pushFishEye()
	{
		if (fishEyeStrength != 0f)
		{
			Shader.EnableKeyword("FISHEYE");
			if (firstFishPush)
			{
				id_fishEyeStrength = Shader.PropertyToID("_FishEyeStrength");
				firstFishPush = false;
			}
			Shader.SetGlobalFloat(id_fishEyeStrength, fishEyeStrength);
		}
		else
		{
			Shader.DisableKeyword("FISHEYE");
		}
	}

	public void pushSkyTex()
	{
		Shader.SetGlobalTexture("_SkyTexture", skyTexture);
	}

	public void pushSettings()
	{
		float num = (RenderSettings.fog ? 1 : 0);
		Shader.SetGlobalVector(id_farHorizonColor, new Vector4(farHorizonColor.r, farHorizonColor.g, farHorizonColor.b, 1f / (0.1f + fullOpacityDistance) * num));
		Shader.SetGlobalVector(id_farApexColor, new Vector4(farApexColor.r, farApexColor.g, farApexColor.b, 1f / (0.1f + apexFullOpacityDistance) * num));
		Shader.SetGlobalVector(id_nearHorizonColor, new Vector4(nearHorizonColor.r, nearHorizonColor.g, nearHorizonColor.b, horizonColorPivotDistance));
		Shader.SetGlobalVector(id_nearApexColor, new Vector4(nearApexColor.r, nearApexColor.g, nearApexColor.b, apexColorPivotDistance));
		Shader.SetGlobalVector(id_params, new Vector4(1f / (0.1f + horizonColorBlendLength), 1f / (0.1f + apexColorBlendLength), fogDensity, 0f));
		Shader.SetGlobalVector(id_VVRLightColor, sceneLightColor);
		Shader.SetGlobalVector(id_VVRShadowColor, sceneShadowColor);
		Shader.SetGlobalTexture(id_VVR_ReflectionMap, reflectionMap);
	}

	public void pushPointLights()
	{
		currentNumLights = pointLights.Count;
		if (currentNumLights > 0)
		{
			Shader.SetGlobalVector(id_VVR_Light0, new Vector4(pointLights[0].transform.position.x, pointLights[0].transform.position.y, pointLights[0].transform.position.z, pointLights[0].intensity));
			lightAtten.x = ((!pointLights[0].noAttenuation) ? (1f / Mathf.Pow(pointLights[0].range, 2f)) : 0f);
		}
		else
		{
			Shader.SetGlobalVector(id_VVR_Light0, emptyLight);
			lightAtten.x = 1f;
		}
		if (currentNumLights > 1)
		{
			Shader.SetGlobalVector(id_VVR_Light1, new Vector4(pointLights[1].transform.position.x, pointLights[1].transform.position.y, pointLights[1].transform.position.z, pointLights[1].intensity));
			lightAtten.y = ((!pointLights[1].noAttenuation) ? (1f / Mathf.Pow(pointLights[1].range, 2f)) : 0f);
		}
		else
		{
			Shader.SetGlobalVector(id_VVR_Light1, emptyLight);
			lightAtten.y = 1f;
		}
		if (currentNumLights > 2)
		{
			Shader.SetGlobalVector(id_VVR_Light2, new Vector4(pointLights[2].transform.position.x, pointLights[2].transform.position.y, pointLights[2].transform.position.z, pointLights[2].intensity));
			lightAtten.z = ((!pointLights[2].noAttenuation) ? (1f / Mathf.Pow(pointLights[2].range, 2f)) : 0f);
		}
		else
		{
			Shader.SetGlobalVector(id_VVR_Light2, emptyLight);
			lightAtten.z = 1f;
		}
		if (currentNumLights > 3)
		{
			Shader.SetGlobalVector(id_VVR_Light3, new Vector4(pointLights[3].transform.position.x, pointLights[3].transform.position.y, pointLights[3].transform.position.z, pointLights[3].intensity));
			lightAtten.w = ((!pointLights[3].noAttenuation) ? (1f / Mathf.Pow(pointLights[3].range, 2f)) : 0f);
		}
		else
		{
			Shader.SetGlobalVector(id_VVR_Light3, emptyLight);
			lightAtten.w = 1f;
		}
		Shader.SetGlobalVector(id_VVR_LightAtten, lightAtten);
	}

	private void OnEnable()
	{
		if (TBCore.GetActivePlatform() == VRPlatform.OculusMobile)
		{
			return;
		}
		for (int i = 0; i < desktopLights.Count; i++)
		{
			if (desktopLights[i] != null)
			{
				desktopLights[i].gameObject.SetActive(value: true);
			}
		}
	}

	private void OnDisable()
	{
		if (TBCore.GetActivePlatform() == VRPlatform.OculusMobile)
		{
			return;
		}
		for (int i = 0; i < desktopLights.Count; i++)
		{
			if (desktopLights[i] != null)
			{
				desktopLights[i].gameObject.SetActive(value: false);
			}
		}
	}
}
