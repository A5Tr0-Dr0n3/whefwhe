using UnityEngine;

public class CursorReticle : MonoBehaviour
{
	private ToolGroup _toolGroup;

	[Header("Scene References")]
	public Transform CursorTransform;

	[Header("Tweakables")]
	public Color noGrabColor;

	[Header("Tweakables")]
	public Color airColor;

	[Header("Tweakables")]
	public Color grabColor;

	[Header("Tweakables")]
	public Color buttonColor;

	private Color cursorColor;

	private Color targetColor;

	[HideInInspector]
	public Vector3 originalScale = Vector3.one * 0.03f;

	[HideInInspector]
	public Vector3 grabScale;

	public Renderer stateRenderer;

	public Renderer centerRenderer;

	[HideInInspector]
	public bool rendering = true;

	public float scaleChangeSpeed = 15f;

	public float colorChangeSpeed = 15f;

	[HideInInspector]
	public float scaleConstant = 1f;

	private float myScaleConstant = 1f;

	public float cursorScalar = 1f;

	private Vector3 workingScale;

	[Header("Trail Settings")]
	public TrailRenderer trail;

	public float trailWidth = 0.1f;

	private Vector3[] trailPositionSamples;

	private float FOVconstant;

	private Plane plane;

	private Camera cam;

	private Transform camT;

	[SerializeField]
	private int trailSampleSize = 4;

	[SerializeField]
	private int trailCurvePoints = 15;

	private float _velocityMagnitude = -1f;

	[HideInInspector]
	public float dist;

	private float distanceLerp;

	[Header("Scale Settings")]
	public float maxScalingDist = 50f;

	public float scaleAtMaxDist = 0.3f;

	private Vector2 lastFrameCursorPos;

	private Vector2 thisFrameCursorPos;

	public float cursorMoveThreshold = 0.00015f;

	public float velocityMagnitude => _velocityMagnitude;

	public void Initialize(ToolGroup toolGroup)
	{
		_toolGroup = toolGroup;
	}

	private void Awake()
	{
		CursorTransform = transform;
	}

	private void Start()
	{
		camT = Camera.main.transform;
		CursorTransform.position = camT.position + camT.forward * 5f;
		workingScale = originalScale;
		grabScale = originalScale * 0.5f;
		stateRenderer.material = stateRenderer.material;
	}

	private void LateUpdate()
	{
		MoveCursor();
		ChangeCursorColor();
	}

	public void ToggleOnOff(bool flag)
	{
		centerRenderer.enabled = flag;
		stateRenderer.enabled = flag;
		rendering = flag;
	}

	private void UpdateScaleConstant()
	{
		plane = new Plane(camT.forward, camT.position);
		dist = plane.GetDistanceToPoint(CursorTransform.position);
		scaleConstant = dist * cursorScalar;
		distanceLerp = Mathf.InverseLerp(0f, maxScalingDist, dist);
		if (PlayerScaler.instance.scaled)
		{
			myScaleConstant = Mathf.Clamp(scaleConstant * Mathf.Lerp(1f, scaleAtMaxDist, distanceLerp), 0.5f, 99999f) / (PlayerScaler.instance.currentScale.x * 0.5f);
		}
		else
		{
			myScaleConstant = Mathf.Clamp(scaleConstant * Mathf.Lerp(1f, scaleAtMaxDist, distanceLerp), 0.5f, 99999f);
		}
		trail.widthMultiplier = trailWidth * myScaleConstant;
	}

	private void MoveCursor()
	{
		if (_toolGroup.Manipulator.isGrabbing())
		{
			if (rendering)
			{
				stateRenderer.enabled = true;
			}
			if (rendering)
			{
				centerRenderer.enabled = true;
			}
			if (targetColor != grabColor)
			{
				targetColor = grabColor;
			}
			if ((bool)trail)
			{
				trail.Clear();
				trail.enabled = false;
			}
			transform.position = _toolGroup.Manipulator.grabLocation;
			UpdateScaleConstant();
			workingScale = Vector3.Lerp(workingScale, grabScale, scaleChangeSpeed * Time.deltaTime);
			if (!ToolManager.AllCursorReticles.HideCursor)
			{
				transform.localScale = workingScale * myScaleConstant;
			}
			return;
		}
		thisFrameCursorPos = Camera.main.WorldToViewportPoint(CursorTransform.position);
		if ((bool)trail && rendering && Vector2.SqrMagnitude(lastFrameCursorPos - thisFrameCursorPos) > cursorMoveThreshold)
		{
			trail.enabled = true;
			_velocityMagnitude = Vector2.SqrMagnitude(lastFrameCursorPos - thisFrameCursorPos);
		}
		else
		{
			trail.Clear();
			trail.enabled = false;
			_velocityMagnitude = -1f;
		}
		lastFrameCursorPos = thisFrameCursorPos;
		if (!ToolManager.IsTeleporting)
		{
			transform.position = Vector3.Lerp(transform.position, _toolGroup.Manipulator.grabLocation, 2f * scaleChangeSpeed * Time.deltaTime);
			UpdateScaleConstant();
		}
		switch (_toolGroup.Manipulator.currentGrabbableStatus)
		{
		case Manipulator.grabbableStatus.grabbable:
			targetColor = grabColor;
			break;
		case Manipulator.grabbableStatus.button:
			targetColor = buttonColor;
			break;
		case Manipulator.grabbableStatus.neutral:
			targetColor = noGrabColor;
			break;
		case Manipulator.grabbableStatus.noHit:
			targetColor = airColor;
			break;
		default:
			targetColor = noGrabColor;
			break;
		}
		bool flag = _toolGroup.Manipulator.currentGrabbableStatus == Manipulator.grabbableStatus.noHit;
		if (EagerMaster.instance.isSwitchingScenes || ToolManager.AllCursorReticles.HideCursor || flag)
		{
			transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 15f * Time.deltaTime);
			if (transform.localScale.x < 0.025f)
			{
				stateRenderer.enabled = false;
				centerRenderer.enabled = false;
			}
			if ((bool)trail && (EagerMaster.instance.isSwitchingScenes || ToolManager.AllCursorReticles.HideCursor))
			{
				trail.Clear();
				trail.enabled = false;
			}
			return;
		}
		workingScale = Vector3.Lerp(workingScale, originalScale, 10f * Time.deltaTime);
		transform.localScale = workingScale * myScaleConstant;
		if (rendering)
		{
			stateRenderer.enabled = true;
		}
		if (rendering)
		{
			centerRenderer.enabled = true;
		}
		if ((bool)trail)
		{
			trail.enabled = true;
		}
	}

	private void ChangeCursorColor()
	{
		cursorColor = Color.Lerp(cursorColor, targetColor, colorChangeSpeed * Time.deltaTime);
		stateRenderer.sharedMaterial.SetColor("_Color", cursorColor);
	}
}
