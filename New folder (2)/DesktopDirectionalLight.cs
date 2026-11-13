using TButt;
using UnityEngine;

[ExecuteInEditMode]
public class DesktopDirectionalLight : MonoBehaviour
{
	[Header("Graphics settings")]
	public StandaloneGraphicsMode minGraphicsLevel;

	[Header("Light Settings")]
	public float _range = 10f;

	public float _spotAngle = 30f;

	public Color _color = Color.white;

	public float _intensity = 1f;

	[Header("Shadow Settings")]
	public LightShadows _shadows = LightShadows.Soft;

	[Header("Misc light Settings")]
	public LayerMask _cullingMask;

	[Header("Rotation Settings")]
	public Vector3 rotationEuler;

	[Header("Valve Light Settings")]
	[Range(0f, 100f)]
	public float m_innerSpotPercent = 50f;

	[Range(128f, 8192f)]
	public int m_shadowResolution = 1024;

	public float m_shadowNearClipPlane = 1f;

	public LayerMask m_shadowCastLayerMask = -1;

	public float m_directionalLightShadowRadius = 100f;

	public float m_directionalLightShadowRange = 100f;

	[Header("Light Reference")]
	public Light mLight;

	private ValveRealtimeLight valveLight;

	private Vector3 startRotationEulers;

	private Quaternion startRotation;

	private bool lightsDestroyed;

	private bool dontUpdateInEditor;

	private float startLerpIntensity;

	private float startLerpRange;

	private void Start()
	{
		if (TBCore.GetActivePlatform() != VRPlatform.OculusMobile)
		{
			if (minGraphicsLevel > PersistentDataManager.GetGraphicsMode())
			{
				gameObject.SetActive(value: false);
				mLight.gameObject.SetActive(value: false);
			}
			else
			{
				EnhanceLight();
			}
		}
	}

	private void Update()
	{
		if (dontUpdateInEditor)
		{
			dontUpdateInEditor = false;
		}
	}

	private void EnhanceLight()
	{
		lightsDestroyed = false;
		if (mLight == null)
		{
			mLight = GetComponent<Light>();
		}
		if (mLight == null)
		{
			SafeDebug.LogWarning("You have a 'DesktopDiectionalLight' script on an object with no light make sure this is what you want");
			mLight = gameObject.AddComponent<Light>();
		}
		mLight.color = _color;
		mLight.cullingMask = _cullingMask;
		mLight.intensity = _intensity;
		mLight.range = _range;
		mLight.shadows = _shadows;
		mLight.spotAngle = _spotAngle;
		mLight.type = LightType.Directional;
		valveLight = GetComponent<ValveRealtimeLight>();
		if (valveLight == null)
		{
			valveLight = gameObject.AddComponent<ValveRealtimeLight>();
		}
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
		valveLight.m_directionalLightShadowRadius = m_directionalLightShadowRadius;
		valveLight.m_directionalLightShadowRange = m_directionalLightShadowRange;
		transform.localRotation = Quaternion.Euler(rotationEuler);
	}

	private void EditorUpdateLight()
	{
		lightsDestroyed = false;
		if (mLight == null)
		{
			mLight = GetComponent<Light>();
		}
		if (mLight == null)
		{
			SafeDebug.LogWarning("You have a 'DesktopDiectionalLight' script on an object with no light make sure this is what you want");
			mLight = gameObject.AddComponent<Light>();
		}
		mLight.color = _color;
		mLight.cullingMask = _cullingMask;
		mLight.intensity = _intensity;
		mLight.range = _range;
		mLight.shadows = _shadows;
		mLight.spotAngle = _spotAngle;
		mLight.type = LightType.Directional;
		valveLight = GetComponent<ValveRealtimeLight>();
		if (valveLight == null)
		{
			valveLight = gameObject.AddComponent<ValveRealtimeLight>();
		}
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
		valveLight.m_directionalLightShadowRadius = m_directionalLightShadowRadius;
		valveLight.m_directionalLightShadowRange = m_directionalLightShadowRange;
		transform.localRotation = Quaternion.Euler(rotationEuler);
	}

	public void DestroyLights()
	{
		if (!lightsDestroyed)
		{
			transform.localRotation = startRotation;
			Object.DestroyImmediate(valveLight);
			valveLight = null;
			lightsDestroyed = true;
			dontUpdateInEditor = true;
		}
	}

	public void LerpValues(float t)
	{
		startLerpIntensity = _intensity;
		startLerpRange = _range;
		mLight.intensity = startLerpIntensity * t;
		mLight.range = startLerpRange * t;
	}
}
