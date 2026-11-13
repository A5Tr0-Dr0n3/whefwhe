using TButt;
using UnityEngine;

[ExecuteInEditMode]
public class DesktopLight : MonoBehaviour
{
	[Header("Graphics settings")]
	public StandaloneGraphicsMode minGraphicsLevel;

	[Header("Light Settings")]
	public LightType _type;

	public float _range = 10f;

	public float _spotAngle = 30f;

	public Color _color = Color.white;

	public float _intensity = 1f;

	[Header("Shadow Settings")]
	public LightShadows _shadows = LightShadows.Soft;

	[Header("Misc light Settings")]
	public LayerMask _cullingMask;

	[Header("Valve Light Settings")]
	[Range(0f, 100f)]
	public float m_innerSpotPercent = 50f;

	[Range(128f, 8192f)]
	public int m_shadowResolution = 1024;

	public float m_shadowNearClipPlane = 1f;

	public LayerMask m_shadowCastLayerMask = -1;

	[Header("Light Reference")]
	internal Light mLight;

	private ValveRealtimeLight valveLight;

	private bool dontUpdateInEditor;

	private float startLerpIntensity;

	private float startLerpRange;

	private void Start()
	{
		if (TBCore.GetActivePlatform() != VRPlatform.OculusPC && TBCore.GetActivePlatform() != VRPlatform.SteamVR && TBCore.GetActivePlatform() != VRPlatform.PlayStationVR)
		{
			Object.Destroy(gameObject);
		}
	}

	private void OnEnable()
	{
		if (!Application.isPlaying || TBCore.GetActivePlatform() == VRPlatform.OculusMobile)
		{
			return;
		}
		if (minGraphicsLevel > PersistentDataManager.GetGraphicsMode())
		{
			gameObject.SetActive(value: false);
			if (mLight != null)
			{
				mLight.gameObject.SetActive(value: false);
			}
		}
		else
		{
			CreateLight();
		}
	}

	private void CreateLight()
	{
		if (mLight == null)
		{
			ValveRealtimeLight[] components = GetComponents<ValveRealtimeLight>();
			for (int i = 0; i < components.Length; i++)
			{
				Object.DestroyImmediate(components[i]);
			}
			Light[] components2 = GetComponents<Light>();
			for (int j = 0; j < components2.Length; j++)
			{
				Object.DestroyImmediate(components2[j]);
			}
			mLight = gameObject.AddComponent<Light>();
		}
		if (mLight != null)
		{
			mLight.color = _color;
			mLight.cullingMask = _cullingMask;
			mLight.intensity = _intensity;
			mLight.range = _range;
			mLight.shadows = _shadows;
			mLight.spotAngle = _spotAngle;
			mLight.type = _type;
		}
		if (valveLight == null)
		{
			valveLight = gameObject.AddComponent<ValveRealtimeLight>();
		}
		if (valveLight != null)
		{
			valveLight.m_innerSpotPercent = m_innerSpotPercent;
			if (EagerMaster.instance != null)
			{
				valveLight.m_shadowResolution = m_shadowResolution / EagerMaster.instance.shadowDivider;
			}
			else
			{
				valveLight.m_shadowResolution = m_shadowResolution;
			}
			valveLight.m_shadowNearClipPlane = m_shadowNearClipPlane;
			valveLight.m_shadowCastLayerMask = m_shadowCastLayerMask;
			valveLight.m_useOcclusionCullingForShadows = true;
		}
	}

	private void CreateEditorLight()
	{
		if (mLight == null)
		{
			CreateLight();
		}
	}

	private void UpdateEditorLight()
	{
		mLight.color = _color;
		mLight.cullingMask = _cullingMask;
		mLight.intensity = _intensity;
		mLight.range = _range;
		mLight.shadows = _shadows;
		mLight.spotAngle = _spotAngle;
		mLight.type = _type;
		valveLight.m_innerSpotPercent = m_innerSpotPercent;
		if (EagerMaster.instance != null)
		{
			valveLight.m_shadowResolution = m_shadowResolution / EagerMaster.instance.shadowDivider;
		}
		else
		{
			valveLight.m_shadowResolution = m_shadowResolution;
		}
		valveLight.m_shadowNearClipPlane = m_shadowNearClipPlane;
		valveLight.m_shadowCastLayerMask = m_shadowCastLayerMask;
		valveLight.m_useOcclusionCullingForShadows = true;
	}

	public void DestroyLights()
	{
		ValveRealtimeLight[] components = GetComponents<ValveRealtimeLight>();
		for (int i = 0; i < components.Length; i++)
		{
			Object.DestroyImmediate(components[i]);
		}
		Light[] components2 = GetComponents<Light>();
		for (int j = 0; j < components2.Length; j++)
		{
			Object.DestroyImmediate(components2[j]);
		}
		if (valveLight != null)
		{
			Object.DestroyImmediate(valveLight);
		}
		if (mLight != null)
		{
			Object.DestroyImmediate(mLight);
		}
		dontUpdateInEditor = true;
	}

	public void LerpValues(float t)
	{
		startLerpIntensity = _intensity;
		startLerpRange = _range;
		if (mLight != null)
		{
			mLight.intensity = startLerpIntensity * t;
			mLight.range = startLerpRange * t;
		}
	}
}
