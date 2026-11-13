using UnityEngine;

public class ManipulatorLaser : MonoBehaviour
{
	[Header("References")]
	[SerializeField]
	private Manipulator _manipulator;

	[SerializeField]
	private GameObject handCursor;

	[SerializeField]
	private GameObject mouseCursor;

	[SerializeField]
	private LineRenderer _line;

	[SerializeField]
	private Transform _start;

	[SerializeField]
	private Transform _end;

	[Header("Values")]
	[SerializeField]
	[Range(0f, 1f)]
	private float dist = 0.2f;

	[SerializeField]
	private Gradient _active;

	[SerializeField]
	private Gradient _inactive;

	[SerializeField]
	[Range(0f, 0.05f)]
	private float _activeWidth = 0.015f;

	[SerializeField]
	[Range(0f, 0.05f)]
	private float _inactiveWidth = 0.01f;

	private float laserAlphaMin = 0.05f;

	private float laserAlphaMax = 1f;

	private float laserSpeedThreshold = 0.001f;

	private float laserSpeedT = 1f;

	private Vector3[] positions;

	private ToolGroup _toolGroup;

	private void Start()
	{
		positions = new Vector3[2];
		_toolGroup = _manipulator.toolGroup;
	}

	private void LateUpdate()
	{
		if ((!_manipulator.isGrabbing() && _manipulator.CurrentManipulatorMode == Manipulator.manipulatorMode.grab && _toolGroup.CursorReticle.rendering && !ToolManager.AllCursorReticles.HideCursor) || handCursor.activeInHierarchy || mouseCursor.activeInHierarchy)
		{
			_line.enabled = true;
			if (_manipulator.currentGrabbableStatus == Manipulator.grabbableStatus.button || _manipulator.currentGrabbableStatus == Manipulator.grabbableStatus.grabbable)
			{
				_line.colorGradient = _active;
				_line.widthMultiplier = _activeWidth;
			}
			else
			{
				_line.colorGradient = _inactive;
				_line.widthMultiplier = _inactiveWidth;
			}
			if (_manipulator.toolGroup.CursorReticle.velocityMagnitude > 0f)
			{
				Color startColor = _line.startColor;
				laserSpeedT = _manipulator.toolGroup.CursorReticle.velocityMagnitude / laserSpeedThreshold;
				if (laserSpeedT > 1f)
				{
					laserSpeedT = 1f;
				}
				startColor.a = Mathf.Lerp(laserAlphaMax, laserAlphaMin, laserSpeedT);
				_line.startColor = startColor;
				startColor = _line.endColor;
				startColor.a = _line.startColor.a;
				_line.endColor = startColor;
			}
			ref Vector3 reference = ref positions[0];
			reference = _start.position;
			ref Vector3 reference2 = ref positions[1];
			reference2 = Vector3.Lerp(_start.position, _end.position, dist);
			_line.SetPositions(positions);
		}
		else
		{
			_line.enabled = false;
		}
	}
}
