using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
[ExecuteInEditMode]
public class GvrTooltip : MonoBehaviour, IGvrArmModelReceiver
{
	private enum Location
	{
		TouchPadOutside,
		TouchPadInside,
		AppButtonOutside,
		AppButtonInside,
		Custom
	}

	protected static readonly Quaternion RIGHT_SIDE_ROTATION = Quaternion.Euler(0f, 0f, 0f);

	protected static readonly Quaternion LEFT_SIDE_ROTATION = Quaternion.Euler(0f, 0f, 180f);

	protected static readonly Vector2 SQUARE_CENTER = new Vector2(0.5f, 0.5f);

	protected static readonly Vector2 PIVOT = new Vector2(-0.5f, 0.5f);

	protected const float TOUCH_PAD_Y_POSITION_METERS = 0.0385f;

	protected const float APP_BUTTON_Y_POSITION_METERS = 0.0105f;

	protected const float TOOLTIP_Z_POSITION_METERS = 0.0098f;

	[Tooltip("The location to display the tooltip at relative to the controller visual.")]
	[SerializeField]
	private Location location;

	[Tooltip("The text field for this tooltip.")]
	[SerializeField]
	private Text text;

	[Tooltip("Determines if the tooltip is always visible regardless of the controller's location.")]
	[SerializeField]
	private bool alwaysVisible;

	private bool isOnLeft;

	private RectTransform rectTransform;

	private CanvasGroup canvasGroup;

	public Text TooltipText => text;

	public GvrBaseArmModel ArmModel { get; set; }

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();
		isOnLeft = IsTooltipOnLeft();
		RefreshTooltip();
	}

	private void OnEnable()
	{
		if (Application.isPlaying)
		{
			GvrControllerInput.OnPostControllerInputUpdated += OnPostControllerInputUpdated;
		}
	}

	private void OnDisable()
	{
		GvrControllerInput.OnPostControllerInputUpdated -= OnPostControllerInputUpdated;
	}

	private void OnPostControllerInputUpdated()
	{
		CheckTooltipSide();
		if (canvasGroup != null && ArmModel != null)
		{
			canvasGroup.alpha = ((!alwaysVisible) ? ArmModel.TooltipAlphaValue : 1f);
		}
	}

	private void OnValidate()
	{
		rectTransform = GetComponent<RectTransform>();
		RefreshTooltip();
	}

	public bool IsTooltipInside()
	{
		switch (location)
		{
		case Location.TouchPadInside:
		case Location.AppButtonInside:
		case Location.Custom:
			return true;
		default:
			return false;
		}
	}

	public bool IsTooltipOnLeft()
	{
		bool flag = IsTooltipInside();
		GvrSettings.UserPrefsHandedness handedness = GvrSettings.Handedness;
		if (handedness == GvrSettings.UserPrefsHandedness.Left)
		{
			return !flag;
		}
		return flag;
	}

	protected virtual void OnSideChanged(bool IsLocationOnLeft)
	{
		transform.localRotation = ((!isOnLeft) ? RIGHT_SIDE_ROTATION : LEFT_SIDE_ROTATION);
		if (text != null)
		{
			text.transform.localRotation = ((!IsLocationOnLeft) ? RIGHT_SIDE_ROTATION : LEFT_SIDE_ROTATION);
			text.alignment = ((!IsLocationOnLeft) ? TextAnchor.MiddleLeft : TextAnchor.MiddleRight);
		}
	}

	protected float GetMetersToCanvasScale()
	{
		return GvrUIHelpers.GetMetersToCanvasScale(transform);
	}

	private Vector3 GetLocalPosition()
	{
		float metersToCanvasScale = GetMetersToCanvasScale();
		if (metersToCanvasScale == 0f)
		{
			return rectTransform.anchoredPosition3D;
		}
		float z = 0.0098f / metersToCanvasScale;
		switch (location)
		{
		case Location.TouchPadOutside:
		case Location.TouchPadInside:
		{
			float y2 = 0.0385f / metersToCanvasScale;
			return new Vector3(0f, y2, z);
		}
		case Location.AppButtonOutside:
		case Location.AppButtonInside:
		{
			float y = 0.0105f / metersToCanvasScale;
			return new Vector3(0f, y, z);
		}
		default:
			return rectTransform.anchoredPosition3D;
		}
	}

	private void CheckTooltipSide()
	{
		bool flag = IsTooltipOnLeft();
		if (flag != isOnLeft)
		{
			isOnLeft = flag;
			RefreshTooltip();
		}
	}

	private void RefreshTooltip()
	{
		rectTransform.anchorMax = SQUARE_CENTER;
		rectTransform.anchorMax = SQUARE_CENTER;
		rectTransform.pivot = PIVOT;
		rectTransform.anchoredPosition3D = GetLocalPosition();
		OnSideChanged(isOnLeft);
	}
}
