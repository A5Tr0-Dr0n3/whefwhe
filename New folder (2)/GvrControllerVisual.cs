using System;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class GvrControllerVisual : MonoBehaviour, IGvrArmModelReceiver
{
	[Serializable]
	public struct ControllerDisplayState
	{
		public GvrControllerBatteryLevel batteryLevel;

		public bool batteryCharging;

		public bool clickButton;

		public bool appButton;

		public bool homeButton;

		public bool touching;

		public Vector2 touchPos;
	}

	[SerializeField]
	private GameObject[] attachmentPrefabs;

	[SerializeField]
	private Color touchPadColor = new Color(200f / 255f, 200f / 255f, 200f / 255f, 1f);

	[SerializeField]
	private Color appButtonColor = new Color(200f / 255f, 200f / 255f, 200f / 255f, 1f);

	[SerializeField]
	private Color systemButtonColor = new Color(20f / 255f, 20f / 255f, 20f / 255f, 1f);

	[Tooltip("Determines if the displayState is set from GvrControllerInput.")]
	public bool readControllerState = true;

	public ControllerDisplayState displayState;

	[Range(0f, 1f)]
	public float maximumAlpha = 1f;

	private Renderer controllerRenderer;

	private MaterialPropertyBlock materialPropertyBlock;

	private int alphaId;

	private int touchId;

	private int touchPadId;

	private int appButtonId;

	private int systemButtonId;

	private int batteryColorId;

	private bool wasTouching;

	private float touchTime;

	private Vector4 controllerShaderData;

	private Vector4 controllerShaderData2;

	private Color currentBatteryColor;

	public const float APP_BUTTON_ACTIVE_DURATION_SECONDS = 0.111f;

	public const float APP_BUTTON_RELEASE_DURATION_SECONDS = 0.0909f;

	public const float SYSTEM_BUTTON_ACTIVE_DURATION_SECONDS = 0.111f;

	public const float SYSTEM_BUTTON_RELEASE_DURATION_SECONDS = 0.0909f;

	public const float TOUCHPAD_CLICK_DURATION_SECONDS = 0.111f;

	public const float TOUCHPAD_RELEASE_DURATION_SECONDS = 0.0909f;

	public const float TOUCHPAD_CLICK_SCALE_DURATION_SECONDS = 0.075f;

	public const float TOUCHPAD_POINT_SCALE_DURATION_SECONDS = 0.15f;

	private const float BATTERY_FULL = 0f;

	private const float BATTERY_ALMOST_FULL = 0.125f;

	private const float BATTERY_MEDIUM = 0.225f;

	private const float BATTERY_LOW = 0.325f;

	private const float BATTERY_CRITICAL = 0.425f;

	private const float BATTERY_HIDDEN = 0.525f;

	private readonly Color GVR_BATTERY_CRITICAL_COLOR = new Color(1f, 0f, 0f, 1f);

	private readonly Color GVR_BATTERY_LOW_COLOR = new Color(1f, 0.6823f, 0f, 1f);

	private readonly Color GVR_BATTERY_MED_COLOR = new Color(0f, 1f, 0.588f, 1f);

	private readonly Color GVR_BATTERY_FULL_COLOR = new Color(0f, 1f, 0.588f, 1f);

	private const float IMMEDIATE_UPDATE_TIME = 10f;

	public GvrBaseArmModel ArmModel { get; set; }

	public float PreferredAlpha => (!(ArmModel != null)) ? maximumAlpha : (maximumAlpha * ArmModel.PreferredAlpha);

	public Color TouchPadColor
	{
		get
		{
			return touchPadColor;
		}
		set
		{
			touchPadColor = value;
			if (materialPropertyBlock != null)
			{
				materialPropertyBlock.SetColor(touchPadId, touchPadColor);
			}
		}
	}

	public Color AppButtonColor
	{
		get
		{
			return appButtonColor;
		}
		set
		{
			appButtonColor = value;
			if (materialPropertyBlock != null)
			{
				materialPropertyBlock.SetColor(appButtonId, appButtonColor);
			}
		}
	}

	public Color SystemButtonColor
	{
		get
		{
			return systemButtonColor;
		}
		set
		{
			systemButtonColor = value;
			if (materialPropertyBlock != null)
			{
				materialPropertyBlock.SetColor(systemButtonId, systemButtonColor);
			}
		}
	}

	private void Awake()
	{
		Initialize();
		CreateAttachments();
	}

	private void OnEnable()
	{
		GvrControllerInput.OnPostControllerInputUpdated += OnPostControllerInputUpdated;
	}

	private void OnDisable()
	{
		GvrControllerInput.OnPostControllerInputUpdated -= OnPostControllerInputUpdated;
	}

	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			Initialize();
			OnVisualUpdate(updateImmediately: true);
		}
	}

	private void OnPostControllerInputUpdated()
	{
		OnVisualUpdate();
	}

	private void CreateAttachments()
	{
		if (attachmentPrefabs != null)
		{
			for (int i = 0; i < attachmentPrefabs.Length; i++)
			{
				GameObject original = attachmentPrefabs[i];
				GameObject gameObject = UnityEngine.Object.Instantiate(original);
				gameObject.transform.SetParent(transform, worldPositionStays: false);
			}
		}
	}

	private void Initialize()
	{
		if (controllerRenderer == null)
		{
			controllerRenderer = GetComponent<Renderer>();
		}
		if (materialPropertyBlock == null)
		{
			materialPropertyBlock = new MaterialPropertyBlock();
		}
		alphaId = Shader.PropertyToID("_GvrControllerAlpha");
		touchId = Shader.PropertyToID("_GvrTouchInfo");
		touchPadId = Shader.PropertyToID("_GvrTouchPadColor");
		appButtonId = Shader.PropertyToID("_GvrAppButtonColor");
		systemButtonId = Shader.PropertyToID("_GvrSystemButtonColor");
		batteryColorId = Shader.PropertyToID("_GvrBatteryColor");
		materialPropertyBlock.SetColor(appButtonId, appButtonColor);
		materialPropertyBlock.SetColor(systemButtonId, systemButtonColor);
		materialPropertyBlock.SetColor(touchPadId, touchPadColor);
		controllerRenderer.SetPropertyBlock(materialPropertyBlock);
	}

	private void UpdateControllerState()
	{
		displayState.batteryLevel = GvrControllerInput.BatteryLevel;
		displayState.batteryCharging = GvrControllerInput.IsCharging;
		displayState.clickButton = GvrControllerInput.ClickButton;
		displayState.appButton = GvrControllerInput.AppButton;
		displayState.homeButton = GvrControllerInput.HomeButtonState;
		displayState.touching = GvrControllerInput.IsTouching;
		displayState.touchPos = GvrControllerInput.TouchPosCentered;
	}

	private void OnVisualUpdate(bool updateImmediately = false)
	{
		if (readControllerState)
		{
			UpdateControllerState();
		}
		float num = Time.deltaTime;
		if (updateImmediately)
		{
			num = 10f;
		}
		if (displayState.clickButton)
		{
			controllerShaderData2.y = Mathf.Min(1f, controllerShaderData2.y + num / 0.111f);
		}
		else
		{
			controllerShaderData2.y = Mathf.Max(0f, controllerShaderData2.y - num / 0.0909f);
		}
		if (displayState.appButton)
		{
			controllerShaderData2.z = Mathf.Min(1f, controllerShaderData2.z + num / 0.111f);
		}
		else
		{
			controllerShaderData2.z = Mathf.Max(0f, controllerShaderData2.z - num / 0.0909f);
		}
		if (displayState.homeButton)
		{
			controllerShaderData2.w = Mathf.Min(1f, controllerShaderData2.w + num / 0.111f);
		}
		else
		{
			controllerShaderData2.w = Mathf.Max(0f, controllerShaderData2.w - num / 0.0909f);
		}
		controllerShaderData2.x = PreferredAlpha;
		materialPropertyBlock.SetVector(alphaId, controllerShaderData2);
		controllerShaderData.x = displayState.touchPos.x;
		controllerShaderData.y = displayState.touchPos.y;
		if (displayState.touching || displayState.clickButton)
		{
			if (!wasTouching)
			{
				wasTouching = true;
			}
			if (touchTime < 1f)
			{
				touchTime = Mathf.Min(touchTime + num / 0.15f, 1f);
			}
		}
		else
		{
			wasTouching = false;
			if (touchTime > 0f)
			{
				touchTime = Mathf.Max(touchTime - num / 0.15f, 0f);
			}
		}
		controllerShaderData.z = touchTime;
		UpdateBatteryIndicator();
		materialPropertyBlock.SetVector(touchId, controllerShaderData);
		materialPropertyBlock.SetColor(batteryColorId, currentBatteryColor);
		controllerRenderer.SetPropertyBlock(materialPropertyBlock);
	}

	private void UpdateBatteryIndicator()
	{
		GvrControllerBatteryLevel batteryLevel = displayState.batteryLevel;
		bool batteryCharging = displayState.batteryCharging;
		switch (batteryLevel)
		{
		case GvrControllerBatteryLevel.Full:
			controllerShaderData.w = 0f;
			currentBatteryColor = GVR_BATTERY_FULL_COLOR;
			break;
		case GvrControllerBatteryLevel.AlmostFull:
			controllerShaderData.w = 0.125f;
			currentBatteryColor = GVR_BATTERY_FULL_COLOR;
			break;
		case GvrControllerBatteryLevel.Medium:
			controllerShaderData.w = 0.225f;
			currentBatteryColor = GVR_BATTERY_MED_COLOR;
			break;
		case GvrControllerBatteryLevel.Low:
			controllerShaderData.w = 0.325f;
			currentBatteryColor = GVR_BATTERY_LOW_COLOR;
			break;
		case GvrControllerBatteryLevel.CriticalLow:
			controllerShaderData.w = 0.425f;
			currentBatteryColor = GVR_BATTERY_CRITICAL_COLOR;
			break;
		default:
			controllerShaderData.w = 0.525f;
			currentBatteryColor.a = 0f;
			break;
		}
		if (batteryCharging)
		{
			controllerShaderData.w = 0f - controllerShaderData.w;
			currentBatteryColor = GVR_BATTERY_FULL_COLOR;
		}
	}
}
