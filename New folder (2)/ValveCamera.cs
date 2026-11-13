using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ValveCamera : MonoBehaviour
{
	[NonSerialized]
	private const float DIRECTIONAL_LIGHT_PULLBACK_DISTANCE = 10000f;

	[NonSerialized]
	private const int MAX_LIGHTS = 18;

	[Header("Lights & Shadows")]
	[Range(1024f, 8192f)]
	public int m_valveShadowTextureWidth = 8192;

	[Range(1024f, 8192f)]
	public int m_valveShadowTextureHeight = 8192;

	[NonSerialized]
	private Camera m_shadowCamera;

	[NonSerialized]
	public RenderTexture m_shadowDepthTexture;

	[NonSerialized]
	public Shader m_shaderCastShadows;

	[NonSerialized]
	private static bool s_bUsingStaticSettings;

	[NonSerialized]
	private static bool m_bAllowFlush = true;

	[NonSerialized]
	private int g_nNumFlushesThisFrame;

	[NonSerialized]
	private int m_nFlushCounterFrameCount;

	[NonSerialized]
	private bool m_bFailedToPackLastTime;

	[NonSerialized]
	private int m_nLastRenderedFrameCount = -1;

	[NonSerialized]
	private int m_nWarnedTooManyLights;

	[NonSerialized]
	private Vector4[] g_vLightColor = new Vector4[18];

	[NonSerialized]
	private Vector4[] g_vLightPosition_flInvRadius = new Vector4[18];

	[NonSerialized]
	private Vector4[] g_vLightDirection = new Vector4[18];

	[NonSerialized]
	private Vector4[] g_vLightShadowIndex_vLightParams = new Vector4[18];

	[NonSerialized]
	private Vector4[] g_vLightFalloffParams = new Vector4[18];

	[NonSerialized]
	private Vector4[] g_vSpotLightInnerOuterConeCosines = new Vector4[18];

	[NonSerialized]
	private Vector4[] g_vShadowMinMaxUv = new Vector4[18];

	[NonSerialized]
	private Matrix4x4[] g_matWorldToShadow = new Matrix4x4[18];

	[NonSerialized]
	private Matrix4x4[] g_matWorldToLightCookie = new Matrix4x4[18];

	public static bool HasCommandLineArg(string argumentName)
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			if (commandLineArgs[i].Equals(argumentName))
			{
				return true;
			}
		}
		return false;
	}

	public static int GetCommandLineArgValue(string argumentName, int nDefaultValue)
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			if (commandLineArgs[i].Equals(argumentName))
			{
				if (i == commandLineArgs.Length - 1)
				{
					return nDefaultValue;
				}
				return int.Parse(commandLineArgs[i + 1]);
			}
		}
		return nDefaultValue;
	}

	public static float GetCommandLineArgValue(string argumentName, float flDefaultValue)
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			if (commandLineArgs[i].Equals(argumentName))
			{
				if (i == commandLineArgs.Length - 1)
				{
					return flDefaultValue;
				}
				return (float)double.Parse(commandLineArgs[i + 1]);
			}
		}
		return flDefaultValue;
	}

	private void OnValidate()
	{
		if (m_valveShadowTextureWidth % 128 != 0)
		{
			m_valveShadowTextureWidth -= m_valveShadowTextureWidth % 128;
		}
		if (m_valveShadowTextureHeight % 128 != 0)
		{
			m_valveShadowTextureHeight -= m_valveShadowTextureHeight % 128;
		}
	}

	private void OnEnable()
	{
		if (HasCommandLineArg("-noflush"))
		{
			m_bAllowFlush = false;
		}
		CreateAssets();
	}

	private void OnDisable()
	{
	}

	private void OnDestroy()
	{
		if ((bool)m_shadowDepthTexture)
		{
			UnityEngine.Object.DestroyImmediate(m_shadowDepthTexture);
			m_shadowDepthTexture = null;
		}
	}

	private void CreateAssets()
	{
		if (!m_shadowCamera)
		{
			m_shadowCamera = CreateRenderCamera("Valve Shadow Camera");
		}
		if (!m_shadowDepthTexture || m_shadowDepthTexture.width != m_valveShadowTextureWidth || m_shadowDepthTexture.height != m_valveShadowTextureHeight)
		{
			if ((bool)m_shadowDepthTexture)
			{
				UnityEngine.Object.DestroyImmediate(m_shadowDepthTexture);
				m_shadowDepthTexture = null;
			}
			m_shadowDepthTexture = new RenderTexture(m_valveShadowTextureWidth, m_valveShadowTextureHeight, 24, RenderTextureFormat.Shadowmap, RenderTextureReadWrite.Linear);
			if ((bool)m_shadowDepthTexture)
			{
				m_shadowDepthTexture.name = "m_shadowDepthTexture";
				m_shadowDepthTexture.hideFlags = HideFlags.HideAndDontSave;
				m_shadowDepthTexture.useMipMap = false;
				m_shadowDepthTexture.filterMode = FilterMode.Bilinear;
				m_shadowDepthTexture.wrapMode = TextureWrapMode.Clamp;
				m_shadowDepthTexture.antiAliasing = 1;
				m_shadowDepthTexture.Create();
				m_shadowDepthTexture.SetGlobalShaderProperty("g_tShadowBuffer");
			}
			else
			{
				Debug.LogWarning("ERROR! Cannot create shadow depth texture!\n");
			}
		}
		if (!m_shaderCastShadows)
		{
			m_shaderCastShadows = Resources.Load("vr_cast_shadows") as Shader;
			if (!m_shaderCastShadows)
			{
				Debug.LogWarning("ERROR! Can't find Resources/vr_cast_shadows!\n");
			}
			else if (!m_shaderCastShadows.isSupported)
			{
				Debug.LogWarning("ERROR! Resources/vr_cast_shadows not supported!\n");
			}
		}
	}

	private static Camera CreateRenderCamera(string name)
	{
		GameObject gameObject = GameObject.Find(name);
		if (!gameObject)
		{
			gameObject = new GameObject(name);
		}
		Camera camera = gameObject.GetComponent<Camera>();
		if (!camera)
		{
			camera = gameObject.AddComponent<Camera>();
		}
		gameObject.hideFlags = HideFlags.HideAndDontSave;
		camera.enabled = false;
		camera.renderingPath = RenderingPath.Forward;
		camera.nearClipPlane = 0.1f;
		camera.farClipPlane = 100f;
		camera.depthTextureMode = DepthTextureMode.None;
		camera.clearFlags = CameraClearFlags.Depth;
		camera.backgroundColor = Color.white;
		camera.orthographic = false;
		camera.hideFlags = HideFlags.HideAndDontSave;
		gameObject.SetActive(value: false);
		return camera;
	}

	private void ValveGLFlush()
	{
		if (m_bAllowFlush)
		{
			if (m_nFlushCounterFrameCount != Time.frameCount)
			{
				m_nFlushCounterFrameCount = Time.frameCount;
				g_nNumFlushesThisFrame = 0;
			}
			if (++g_nNumFlushesThisFrame <= 3)
			{
				GL.Flush();
			}
		}
	}

	private void ValveGLFlushIfNotReprojecting()
	{
		ValveGLFlush();
	}

	private void MyEditorUpdate()
	{
	}

	private void OnPreCull()
	{
		ValveShadowBufferRender();
	}

	private void OnPostRender()
	{
		ValveGLFlush();
	}

	private void ValveShadowBufferRender()
	{
		CullLightsAgainstCameraFrustum();
		RenderShadowBuffer();
		UpdateLightConstants();
	}

	private bool AutoPackLightsIntoShadowTexture()
	{
		ValveRealtimeLight.s_allLights.Sort((ValveRealtimeLight vl1, ValveRealtimeLight vl2) =>
		{
			int num7 = 0;
			bool flag = vl1.CastsShadows();
			bool flag2 = vl2.CastsShadows();
			num7 = ((!flag || !flag2) ? flag2.CompareTo(flag) : vl2.m_shadowResolution.CompareTo(vl1.m_shadowResolution));
			if (num7 == 0)
			{
				num7 = vl2.m_cachedLight.range.CompareTo(vl1.m_cachedLight.range);
			}
			if (num7 == 0)
			{
				num7 = vl2.GetInstanceID().CompareTo(vl1.GetInstanceID());
			}
			return num7;
		});
		int num = 0;
		int num2 = 0;
		int num3 = -1;
		int num4 = 0;
		int num5 = 0;
		for (int num6 = 0; num6 < ValveRealtimeLight.s_allLights.Count; num6++)
		{
			ValveRealtimeLight valveRealtimeLight = ValveRealtimeLight.s_allLights[num6];
			valveRealtimeLight.m_bRenderShadowsThisFrame = false;
			if (!valveRealtimeLight.IsEnabled() || !valveRealtimeLight.CastsShadows())
			{
				continue;
			}
			num++;
			if (num > 18)
			{
				break;
			}
			if (num3 == -1 && (valveRealtimeLight.m_shadowResolution > m_shadowDepthTexture.width || valveRealtimeLight.m_shadowResolution > m_shadowDepthTexture.height))
			{
				Debug.LogError("ERROR! Valve shadow packer ran out of space in the " + m_shadowDepthTexture.width + "x" + m_shadowDepthTexture.height + " texture!\n\n");
				m_bFailedToPackLastTime = true;
				return false;
			}
			if (num3 == -1 || num2 + valveRealtimeLight.m_shadowResolution > m_shadowDepthTexture.width)
			{
				num2 = 0;
				num3 = num4;
				num4 += valveRealtimeLight.m_shadowResolution;
			}
			if (num3 + valveRealtimeLight.m_shadowResolution > m_shadowDepthTexture.height)
			{
				Debug.LogError("ERROR! Valve shadow packer ran out of space in the " + m_shadowDepthTexture.width + "x" + m_shadowDepthTexture.height + " texture!\n\n");
				m_bFailedToPackLastTime = true;
				return false;
			}
			valveRealtimeLight.m_shadowX[num5] = num2;
			valveRealtimeLight.m_shadowY[num5] = num3;
			valveRealtimeLight.m_bRenderShadowsThisFrame = true;
			num2 += valveRealtimeLight.m_shadowResolution;
			if (valveRealtimeLight.m_cachedLight.type == LightType.Point)
			{
				num5++;
				if (num5 < 6)
				{
					num6--;
				}
				else
				{
					num5 = 0;
				}
			}
		}
		if (m_bFailedToPackLastTime)
		{
			m_bFailedToPackLastTime = false;
			Debug.Log("SUCCESS! Valve shadow packer can now fit all lights into the " + m_shadowDepthTexture.width + "x" + m_shadowDepthTexture.height + " texture!\n\n");
		}
		return num != 0;
	}

	private void CullLightsAgainstCameraFrustum()
	{
		if (!m_shadowCamera)
		{
			Debug.LogWarning("ERROR! m_shadowCamera == null!\n");
			return;
		}
		Camera component = gameObject.GetComponent<Camera>();
		Plane[] array = GeometryUtility.CalculateFrustumPlanes(component);
		for (int i = 0; i < ValveRealtimeLight.s_allLights.Count; i++)
		{
			ValveRealtimeLight valveRealtimeLight = ValveRealtimeLight.s_allLights[i];
			Light cachedLight = valveRealtimeLight.m_cachedLight;
			valveRealtimeLight.m_bInCameraFrustum = true;
			if (!valveRealtimeLight.IsEnabled())
			{
				continue;
			}
			valveRealtimeLight.m_bInCameraFrustum = false;
			if (cachedLight.type == LightType.Directional)
			{
				valveRealtimeLight.m_bInCameraFrustum = true;
			}
			if (cachedLight.type == LightType.Point)
			{
				Bounds bounds = new Bounds(cachedLight.transform.position, new Vector3(cachedLight.range * 2f, cachedLight.range * 2f, cachedLight.range * 2f));
				if (GeometryUtility.TestPlanesAABB(array, bounds))
				{
					valveRealtimeLight.m_bInCameraFrustum = true;
					Vector3 position = cachedLight.transform.position;
					for (int j = 0; j < array.Length; j++)
					{
						float distanceToPoint = array[j].GetDistanceToPoint(position);
						if (distanceToPoint < 0f - cachedLight.range)
						{
							valveRealtimeLight.m_bInCameraFrustum = false;
							break;
						}
					}
				}
			}
			if (cachedLight.type != LightType.Spot)
			{
				continue;
			}
			Transform transform = cachedLight.transform;
			Vector3 position2 = transform.position;
			Vector3 forward = transform.forward;
			float range = cachedLight.range;
			Vector3 center = position2 + forward * range * 0.5f;
			Bounds bounds2 = new Bounds(center, new Vector3(range, range, range));
			if (!GeometryUtility.TestPlanesAABB(array, bounds2))
			{
				continue;
			}
			Vector3 right = transform.right;
			Vector3 up = transform.up;
			float spotAngle = cachedLight.spotAngle;
			valveRealtimeLight.m_bInCameraFrustum = true;
			float num = Mathf.Tan(spotAngle * ((float)Math.PI / 180f) * 0.5f) * range;
			Vector3[] array2 = new Vector3[6]
			{
				position2,
				position2 + forward * range,
				default,
				default,
				default,
				default
			};
			ref Vector3 reference = ref array2[2];
			reference = array2[1] + right * num;
			ref Vector3 reference2 = ref array2[3];
			reference2 = array2[1] - right * num;
			ref Vector3 reference3 = ref array2[4];
			reference3 = array2[1] + up * num;
			ref Vector3 reference4 = ref array2[5];
			reference4 = array2[1] - up * num;
			for (int k = 0; k < array.Length; k++)
			{
				bool flag = false;
				for (int l = 0; l < array2.Length; l++)
				{
					if (array[k].GetDistanceToPoint(array2[l]) > 0f)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					valveRealtimeLight.m_bInCameraFrustum = false;
					break;
				}
			}
		}
	}

	private void RenderShadowBuffer()
	{
		if (m_nLastRenderedFrameCount == Time.frameCount)
		{
			return;
		}
		m_nLastRenderedFrameCount = Time.frameCount;
		if (!m_shadowCamera)
		{
			Debug.LogWarning("ERROR! m_shadowCamera == null!\n");
		}
		else if (!m_shadowDepthTexture)
		{
			Debug.LogWarning("ERROR! m_shadowDepthTexture == null!\n");
		}
		else
		{
			if (!AutoPackLightsIntoShadowTexture())
			{
				return;
			}
			m_shadowCamera.targetTexture = m_shadowDepthTexture;
			m_shadowCamera.pixelRect = new Rect(0f, 0f, m_shadowCamera.targetTexture.width, m_shadowCamera.targetTexture.height);
			m_shadowCamera.clearFlags = CameraClearFlags.Depth;
			m_shadowCamera.cullingMask = 0;
			m_shadowCamera.RenderWithShader(m_shaderCastShadows, "DO_NOT_RENDER");
			m_shadowCamera.clearFlags = CameraClearFlags.Nothing;
			m_shadowCamera.cullingMask = -1;
			int num = 0;
			for (int i = 0; i < ValveRealtimeLight.s_allLights.Count; i++)
			{
				ValveRealtimeLight valveRealtimeLight = ValveRealtimeLight.s_allLights[i];
				Light cachedLight = valveRealtimeLight.m_cachedLight;
				if (!valveRealtimeLight.IsEnabled() || !valveRealtimeLight.CastsShadows() || !valveRealtimeLight.m_bRenderShadowsThisFrame)
				{
					continue;
				}
				m_shadowCamera.pixelRect = new Rect(valveRealtimeLight.m_shadowX[num], valveRealtimeLight.m_shadowY[num], valveRealtimeLight.m_shadowResolution, valveRealtimeLight.m_shadowResolution);
				Quaternion rotation = cachedLight.transform.rotation;
				if (cachedLight.type == LightType.Point)
				{
					if (num < 4)
					{
						cachedLight.transform.Rotate(cachedLight.transform.up.normalized, 90f * (float)num, Space.World);
					}
					else
					{
						cachedLight.transform.Rotate(cachedLight.transform.right.normalized, 90f + 180f * (float)(num - 4), Space.World);
					}
				}
				m_shadowCamera.aspect = 1f;
				m_shadowCamera.orthographic = false;
				m_shadowCamera.nearClipPlane = valveRealtimeLight.m_shadowNearClipPlane;
				m_shadowCamera.farClipPlane = cachedLight.range;
				m_shadowCamera.fieldOfView = cachedLight.spotAngle;
				m_shadowCamera.transform.position = cachedLight.transform.position;
				m_shadowCamera.transform.rotation = cachedLight.transform.rotation;
				m_shadowCamera.cullingMask = valveRealtimeLight.m_shadowCastLayerMask;
				m_shadowCamera.useOcclusionCulling = valveRealtimeLight.m_useOcclusionCullingForShadows;
				if (cachedLight.type == LightType.Directional)
				{
					m_shadowCamera.nearClipPlane = 10000f;
					m_shadowCamera.farClipPlane = m_shadowCamera.nearClipPlane + valveRealtimeLight.m_directionalLightShadowRange;
					m_shadowCamera.fieldOfView = 57.29578f * Mathf.Tan(valveRealtimeLight.m_directionalLightShadowRadius / m_shadowCamera.nearClipPlane);
					m_shadowCamera.transform.position = new Vector3(cachedLight.transform.position.x - cachedLight.transform.forward.normalized.x * 10000f, cachedLight.transform.position.y - cachedLight.transform.forward.normalized.y * 10000f, cachedLight.transform.position.z - cachedLight.transform.forward.normalized.z * 10000f);
				}
				if (cachedLight.type == LightType.Point)
				{
					m_shadowCamera.fieldOfView = 90f;
				}
				Matrix4x4 identity = Matrix4x4.identity;
				identity.m00 = 0.5f;
				identity.m11 = 0.5f;
				identity.m22 = 0.5f;
				identity.m03 = 0.5f;
				identity.m13 = 0.5f;
				identity.m23 = 0.5f;
				Matrix4x4 identity2 = Matrix4x4.identity;
				identity2.m00 = (float)valveRealtimeLight.m_shadowResolution / (float)m_shadowDepthTexture.width;
				identity2.m11 = (float)valveRealtimeLight.m_shadowResolution / (float)m_shadowDepthTexture.height;
				identity2.m03 = (float)valveRealtimeLight.m_shadowX[num] / (float)m_shadowDepthTexture.width;
				identity2.m13 = (float)valveRealtimeLight.m_shadowY[num] / (float)m_shadowDepthTexture.height;
				ref Matrix4x4 reference = ref valveRealtimeLight.m_shadowTransform[num];
				reference = identity2 * identity * m_shadowCamera.projectionMatrix * m_shadowCamera.worldToCameraMatrix;
				ref Matrix4x4 reference2 = ref valveRealtimeLight.m_lightCookieTransform[num];
				reference2 = identity * m_shadowCamera.projectionMatrix * m_shadowCamera.worldToCameraMatrix;
				Shader.SetGlobalVector("g_vLightDirWs", new Vector4(cachedLight.transform.forward.normalized.x, cachedLight.transform.forward.normalized.y, cachedLight.transform.forward.normalized.z));
				m_shadowCamera.RenderWithShader(m_shaderCastShadows, "RenderType");
				if (cachedLight.type == LightType.Point)
				{
					cachedLight.transform.rotation = rotation;
					num++;
					if (num < 6)
					{
						i--;
					}
					else
					{
						num = 0;
					}
				}
			}
			ValveGLFlushIfNotReprojecting();
		}
	}

	private void UpdateLightConstants()
	{
		int num = 0;
		int num2 = 0;
		bool flag = false;
		int num3 = 0;
		for (int i = 0; i < ValveRealtimeLight.s_allLights.Count; i++)
		{
			ValveRealtimeLight valveRealtimeLight = ValveRealtimeLight.s_allLights[i];
			Light cachedLight = valveRealtimeLight.m_cachedLight;
			if (!valveRealtimeLight.IsEnabled())
			{
				continue;
			}
			if (cachedLight.type == LightType.Directional)
			{
				num2++;
				if (num2 > 18)
				{
					continue;
				}
				float num4 = ((!(cachedLight.intensity <= 1f)) ? (cachedLight.intensity * cachedLight.intensity) : cachedLight.intensity);
				ref Vector4 reference = ref g_vLightColor[num];
				reference = new Vector4(cachedLight.color.linear.r * num4, cachedLight.color.linear.g * num4, cachedLight.color.linear.b * num4);
				ref Vector4 reference2 = ref g_vLightPosition_flInvRadius[num];
				reference2 = new Vector4(cachedLight.transform.position.x - cachedLight.transform.forward.normalized.x * 10000f, cachedLight.transform.position.y - cachedLight.transform.forward.normalized.y * 10000f, cachedLight.transform.position.z - cachedLight.transform.forward.normalized.z * 10000f, -1f);
				ref Vector4 reference3 = ref g_vLightDirection[num];
				reference3 = new Vector4(cachedLight.transform.forward.normalized.x, cachedLight.transform.forward.normalized.y, cachedLight.transform.forward.normalized.z);
				ref Vector4 reference4 = ref g_vLightShadowIndex_vLightParams[num];
				reference4 = new Vector4(0f, 0f, 1f, 1f);
				ref Vector4 reference5 = ref g_vLightFalloffParams[num];
				reference5 = new Vector4(0f, 0f, float.MaxValue);
				ref Vector4 reference6 = ref g_vSpotLightInnerOuterConeCosines[num];
				reference6 = new Vector4(0f, -1f, 1f);
				if (cachedLight.shadows != LightShadows.None && valveRealtimeLight.m_bRenderShadowsThisFrame)
				{
					g_vLightShadowIndex_vLightParams[num].x = 1f;
					ref Matrix4x4 reference7 = ref g_matWorldToShadow[num];
					reference7 = valveRealtimeLight.m_shadowTransform[num3].transpose;
					ref Vector4 reference8 = ref g_vShadowMinMaxUv[num];
					reference8 = new Vector4(0f, 0f, 1f, 1f);
				}
				num++;
			}
			if (cachedLight.type == LightType.Point)
			{
				num2++;
				if (num2 > 18)
				{
					continue;
				}
				float num5 = ((!(cachedLight.intensity <= 1f)) ? (cachedLight.intensity * cachedLight.intensity) : cachedLight.intensity);
				ref Vector4 reference9 = ref g_vLightColor[num];
				reference9 = new Vector4(cachedLight.color.linear.r * num5, cachedLight.color.linear.g * num5, cachedLight.color.linear.b * num5);
				ref Vector4 reference10 = ref g_vLightPosition_flInvRadius[num];
				reference10 = new Vector4(cachedLight.transform.position.x, cachedLight.transform.position.y, cachedLight.transform.position.z, 1f / cachedLight.range);
				ref Vector4 reference11 = ref g_vLightDirection[num];
				reference11 = new Vector4(0f, 0f, 0f);
				ref Vector4 reference12 = ref g_vLightShadowIndex_vLightParams[num];
				reference12 = new Vector4(0f, 0f, 1f, 1f);
				ref Vector4 reference13 = ref g_vLightFalloffParams[num];
				reference13 = new Vector4(1f, 0f, cachedLight.range * cachedLight.range);
				ref Vector4 reference14 = ref g_vSpotLightInnerOuterConeCosines[num];
				reference14 = new Vector4(0f, -1f, 1f);
				if (cachedLight.shadows != LightShadows.None && valveRealtimeLight.m_bRenderShadowsThisFrame)
				{
					flag = true;
					Quaternion rotation = cachedLight.transform.rotation;
					if (num3 < 4)
					{
						cachedLight.transform.Rotate(cachedLight.transform.up.normalized, 90f * (float)num3, Space.World);
					}
					else
					{
						cachedLight.transform.Rotate(cachedLight.transform.right.normalized, 90f + 180f * (float)(num3 - 4), Space.World);
					}
					ref Vector4 reference15 = ref g_vLightDirection[num];
					reference15 = new Vector4(cachedLight.transform.forward.normalized.x, cachedLight.transform.forward.normalized.y, cachedLight.transform.forward.normalized.z);
					g_vLightShadowIndex_vLightParams[num].x = 1f;
					g_vLightShadowIndex_vLightParams[num].y = 2f;
					ref Matrix4x4 reference16 = ref g_matWorldToShadow[num];
					reference16 = valveRealtimeLight.m_shadowTransform[num3].transpose;
					ref Matrix4x4 reference17 = ref g_matWorldToLightCookie[num];
					reference17 = valveRealtimeLight.m_lightCookieTransform[num3].transpose;
					ref Vector4 reference18 = ref g_vSpotLightInnerOuterConeCosines[num];
					reference18 = new Vector4(0f, 0.574f, 9999999f);
					ref Vector4 reference19 = ref g_vShadowMinMaxUv[num];
					reference19 = new Vector4((float)(valveRealtimeLight.m_shadowX[num3] + 1) / (float)m_shadowDepthTexture.width, (float)(valveRealtimeLight.m_shadowY[num3] + 1) / (float)m_shadowDepthTexture.height, (float)(valveRealtimeLight.m_shadowX[num3] + valveRealtimeLight.m_shadowResolution - 1) / (float)m_shadowDepthTexture.width, (float)(valveRealtimeLight.m_shadowY[num3] + valveRealtimeLight.m_shadowResolution - 1) / (float)m_shadowDepthTexture.height);
					cachedLight.transform.rotation = rotation;
					num3++;
					if (num3 < 6)
					{
						i--;
					}
					else
					{
						num3 = 0;
					}
				}
				num++;
			}
			if (cachedLight.type != LightType.Spot)
			{
				continue;
			}
			num2++;
			if (num2 <= 18)
			{
				float num6 = ((!(cachedLight.intensity <= 1f)) ? (cachedLight.intensity * cachedLight.intensity) : cachedLight.intensity);
				ref Vector4 reference20 = ref g_vLightColor[num];
				reference20 = new Vector4(cachedLight.color.linear.r * num6, cachedLight.color.linear.g * num6, cachedLight.color.linear.b * num6);
				ref Vector4 reference21 = ref g_vLightPosition_flInvRadius[num];
				reference21 = new Vector4(cachedLight.transform.position.x, cachedLight.transform.position.y, cachedLight.transform.position.z, 1f / cachedLight.range);
				ref Vector4 reference22 = ref g_vLightDirection[num];
				reference22 = new Vector4(cachedLight.transform.forward.normalized.x, cachedLight.transform.forward.normalized.y, cachedLight.transform.forward.normalized.z);
				ref Vector4 reference23 = ref g_vLightShadowIndex_vLightParams[num];
				reference23 = new Vector4(0f, 0f, 1f, 1f);
				ref Vector4 reference24 = ref g_vLightFalloffParams[num];
				reference24 = new Vector4(1f, 0f, cachedLight.range * cachedLight.range);
				float num7 = Mathf.Clamp(valveRealtimeLight.m_innerSpotPercent, 0f, 100f) / 100f;
				float num8 = Mathf.Clamp(Mathf.Cos(cachedLight.spotAngle * 0.5f * ((float)Math.PI / 180f)), 0f, 1f);
				float num9 = Mathf.Clamp(Mathf.Cos(cachedLight.spotAngle * 0.5f * num7 * ((float)Math.PI / 180f)), 0f, 1f);
				ref Vector4 reference25 = ref g_vSpotLightInnerOuterConeCosines[num];
				reference25 = new Vector4(num9, num8, 1f / Mathf.Max(0.01f, num9 - num8));
				if (cachedLight.shadows != LightShadows.None && valveRealtimeLight.m_bRenderShadowsThisFrame)
				{
					g_vLightShadowIndex_vLightParams[num].x = 1f;
					ref Matrix4x4 reference26 = ref g_matWorldToShadow[num];
					reference26 = valveRealtimeLight.m_shadowTransform[num3].transpose;
					ref Vector4 reference27 = ref g_vShadowMinMaxUv[num];
					reference27 = new Vector4(0f, 0f, 1f, 1f);
				}
				num++;
			}
		}
		if (num2 > 18)
		{
			if (num2 > m_nWarnedTooManyLights)
			{
				Debug.LogError("ERROR! Found " + num2 + " runtime lights! Valve renderer supports up to " + 18 + " active runtime lights at a time!\nDisabling " + (num2 - 18) + " runtime light" + ((num2 - 18 <= 1) ? string.Empty : "s") + "!\n");
			}
			m_nWarnedTooManyLights = num2;
		}
		else if (m_nWarnedTooManyLights > 0)
		{
			m_nWarnedTooManyLights = 0;
			Debug.Log("SUCCESS! Found " + num2 + " runtime lights which is within the supported number of lights, " + 18 + ".\n\n");
		}
		Shader.SetGlobalInt("g_nNumLights", num);
		Shader.SetGlobalVectorArray("g_vLightPosition_flInvRadius", g_vLightPosition_flInvRadius);
		Shader.SetGlobalVectorArray("g_vLightColor", g_vLightColor);
		Shader.SetGlobalVectorArray("g_vLightDirection", g_vLightDirection);
		Shader.SetGlobalVectorArray("g_vLightShadowIndex_vLightParams", g_vLightShadowIndex_vLightParams);
		Shader.SetGlobalVectorArray("g_vLightFalloffParams", g_vLightFalloffParams);
		Shader.SetGlobalVectorArray("g_vSpotLightInnerOuterConeCosines", g_vSpotLightInnerOuterConeCosines);
		Shader.SetGlobalVectorArray("g_vShadowMinMaxUv", g_vShadowMinMaxUv);
		Shader.SetGlobalMatrixArray("g_matWorldToShadow", g_matWorldToShadow);
		Shader.SetGlobalMatrixArray("g_matWorldToLightCookie", g_matWorldToLightCookie);
		if (flag)
		{
			Shader.EnableKeyword("D_VALVE_SHADOWING_POINT_LIGHTS");
		}
		else
		{
			Shader.DisableKeyword("D_VALVE_SHADOWING_POINT_LIGHTS");
		}
		Shader.SetGlobalFloat("g_flTime", Time.timeSinceLevelLoad);
		if ((bool)m_shadowDepthTexture)
		{
			float num10 = 1f / (float)m_shadowDepthTexture.width;
			float num11 = 1f / (float)m_shadowDepthTexture.height;
			Vector4 value = new Vector4(0.074906364f, 0.123595506f, 0.2059925f, 0f);
			Vector4 value2 = new Vector4(num10, num11, 0f - num10, 0f - num11);
			Vector4 value3 = new Vector4(num10, num11, 0f, 0f);
			Vector4 value4 = new Vector4(0f - num10, 0f - num11, 0f, 0f);
			Shader.SetGlobalVector("g_vShadow3x3PCFTerms0", value);
			Shader.SetGlobalVector("g_vShadow3x3PCFTerms1", value2);
			Shader.SetGlobalVector("g_vShadow3x3PCFTerms2", value3);
			Shader.SetGlobalVector("g_vShadow3x3PCFTerms3", value4);
		}
	}

	public void CopySettingsFromScriptableObject(ValveCameraSettingsObject obj)
	{
		m_valveShadowTextureWidth = obj._valveShadowTextureWidth;
		m_valveShadowTextureHeight = obj._valveShadowTextureHeight;
	}
}
