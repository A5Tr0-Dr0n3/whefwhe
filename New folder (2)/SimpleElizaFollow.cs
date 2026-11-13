using UnityEngine;

public class SimpleElizaFollow : MonoBehaviour
{
	private Vector3 lookPosition;

	private Vector3 lookRotation;

	public Vector3 extraTurn = new Vector3(0f, 180f, 0f);

	private float lookStartY;

	public float sideEyeAngleMax = 45f;

	public float sideEyeXRange = 0.2f;

	public float lookEasing = 0.1f;

	public float slowLookEasing = 0.04f;

	public float eyeEasing = 0.5f;

	public GameObject eyeTarget;

	private Vector3 eyeStartPosition;

	private Vector3 eyeLookPosition;

	private float eyeXOffset;

	private bool doEyeLookOnly;

	private Transform _lookTarget;

	public bool lookEnabled;

	public Transform lookTarget => _lookTarget;

	private void Start()
	{
		eyeStartPosition = eyeTarget.transform.localPosition;
		ReevaluateTarget();
	}

	private void OnEnable()
	{
		Teleporter.OnFinishTeleportEvent += playerTeleported;
	}

	private void OnDisable()
	{
		Teleporter.OnFinishTeleportEvent -= playerTeleported;
	}

	private void Update()
	{
		if (!lookEnabled)
		{
			return;
		}
		lookPosition = _lookTarget.position;
		lookPosition.y = transform.position.y;
		lookStartY = transform.rotation.eulerAngles.y;
		transform.LookAt(lookPosition);
		transform.Rotate(extraTurn);
		lookRotation = transform.rotation.eulerAngles;
		eyeLookPosition = eyeStartPosition;
		if (doEyeLookOnly)
		{
			if (lookStartY - lookRotation.y > 180f)
			{
				lookStartY -= 360f;
			}
			else if (lookRotation.y - lookStartY > 180f)
			{
				lookStartY += 360f;
			}
			eyeXOffset = (1f - eyeEasing) * eyeXOffset + eyeEasing * (Mathf.Abs(lookRotation.y - lookStartY) / sideEyeAngleMax * sideEyeXRange);
			if (eyeXOffset > sideEyeXRange)
			{
				eyeXOffset = sideEyeXRange;
			}
			if (eyeXOffset < 0f - sideEyeXRange)
			{
				eyeXOffset = 0f - sideEyeXRange;
			}
		}
		else
		{
			eyeXOffset = 0f;
		}
		if (lookRotation.y > lookStartY)
		{
			eyeLookPosition.x -= eyeXOffset;
		}
		else
		{
			eyeLookPosition.x += eyeXOffset;
		}
		eyeTarget.transform.localPosition = eyeLookPosition;
		if (doEyeLookOnly)
		{
			lookRotation.y = lookStartY;
		}
		else
		{
			if (lookStartY - lookRotation.y > 180f)
			{
				lookStartY -= 360f;
			}
			else if (lookRotation.y - lookStartY > 180f)
			{
				lookStartY += 360f;
			}
			lookRotation.y = (1f - slowLookEasing) * lookStartY + slowLookEasing * lookRotation.y;
		}
		transform.rotation = Quaternion.Euler(lookRotation);
	}

	private void playerTeleported(Vector3 startpos, Vector3 endpos)
	{
		ReevaluateTarget();
	}

	public void ReevaluateTarget(bool doAnyway = false)
	{
		if (_lookTarget == null)
		{
			_lookTarget = EagerMaster.instance.mainCamera.transform;
		}
		lookPosition = _lookTarget.position;
		lookPosition.y = transform.position.y;
		lookStartY = transform.rotation.eulerAngles.y;
		transform.LookAt(lookPosition);
		transform.Rotate(extraTurn);
		lookRotation = transform.rotation.eulerAngles;
		if (Mathf.Abs(lookRotation.y - lookStartY) < sideEyeAngleMax)
		{
			doEyeLookOnly = true;
		}
		else
		{
			doEyeLookOnly = false;
			if (lookStartY - lookRotation.y > 180f)
			{
				lookStartY -= 360f;
			}
			else if (lookRotation.y - lookStartY > 180f)
			{
				lookStartY += 360f;
			}
		}
		lookRotation.y = lookStartY;
		transform.rotation = Quaternion.Euler(lookRotation);
	}
}
