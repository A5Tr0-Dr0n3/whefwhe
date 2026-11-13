using System.Collections;
using System.Collections.Generic;
using TButt;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{
	public delegate void OnFinishTeleportEventHandler(Vector3 startPos, Vector3 endPos);

	public GameObject gun;

	public GameObject teleportIndicator;

	private Renderer teleportIndicatorRenderer;

	public ParticleSystem teleportParticles;

	public Color yesColor;

	public Color noColor;

	public float lerpSpeed = 0.06f;

	public bool dropOnTeleport;

	public bool orientOnSurface;

	public float minTeleportDot = 0.5f;

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[CurveRange(0f, 1f, -1f, 1f)]
	public AnimationCurve positionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[CurveRange(0f, 1f, -1f, 1f)]
	public AnimationCurve fishEyeCurve;

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve whooshVolumeCurve;

	public GameObject lineDash;

	public float dashScaleCoefficient = 3f;

	public float teleportStrength = 4f;

	public float gravityCoefficient = 1f;

	public int dashIterations = 25;

	public float dashIterationLength = 0.05f;

	public int rayIterations = 60;

	public float rayIterationLength = 0.05f;

	[Space]
	[Header("SFX")]
	public AudioEvent teleportSFX;

	[HideInInspector]
	public Vector3 hitPoint;

	[HideInInspector]
	public bool isLerpingToPosition;

	[HideInInspector]
	public float timeToLerp = 0.1f;

	private RaycastHit hitInfo;

	private Vector3 _startPos = Vector3.zero;

	private Vector3 _endPos = Vector3.zero;

	private Quaternion _startRot = Quaternion.identity;

	private Quaternion _endRot = Quaternion.identity;

	private float t;

	private bool _cancelDisabled;

	private bool canTeleportInternal;

	private float timeHeld;

	private Vector3 rayDirection;

	private Vector3 trajectoryDir = Vector3.zero;

	private Vector3 trajectoryPos = Vector3.zero;

	private Vector3 oldTrajectoryPos = Vector3.zero;

	private List<GameObject> lineDashes = new List<GameObject>();

	private float lerpDuration;

	private float lerpPercentage;

	private bool gunStuckInCollider;

	private const float gunStuckRadius = 0.01f;

	private GameObject lineDashParent;

	public float headroomSphereRadius = 0.35f;

	private const int nHeadRoomHits = 20;

	private Collider[] headroomCols = new Collider[20];

	private ToolGroup _toolGroup;

	private Material _mat;

	private float endLength;

	private float endInteration;

	[SerializeField]
	private LayerMask layers;

	private Vector3 positionParameter;

	private Vector3 startHeadsetPos;

	private Vector3 endHeadsetPos;

	private int marchingDashes
	{
		get
		{
			if ((double)timeHeld - (double)ToolManager.AllTeleporters.MinHoldTime * 0.5 < (double)ToolManager.AllTeleporters.MinHoldTime * 2.5)
			{
				return Mathf.FloorToInt(Mathf.Clamp01(timeHeld - ToolManager.AllTeleporters.MinHoldTime * 0.5f) * (float)dashIterations / (ToolManager.AllTeleporters.MinHoldTime * 2.5f));
			}
			return dashIterations;
		}
	}

	public static event OnFinishTeleportEventHandler OnFinishTeleportEvent;

	public void Initialize(ToolGroup toolGroup)
	{
		_toolGroup = toolGroup;
	}

	private void Awake()
	{
		if (teleportIndicator == null)
		{
			teleportIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			teleportIndicator.name = "Teleport Indicator";
			Object.Destroy(teleportIndicator.GetComponent<SphereCollider>());
			teleportIndicator.transform.localScale = Vector3.one * 0.5f;
		}
		_mat = teleportIndicator.GetComponent<Renderer>().material;
		_mat.color = yesColor;
		lineDash.GetComponentInChildren<Renderer>().material = _mat;
		teleportIndicatorRenderer = teleportIndicator.GetComponent<Renderer>();
		teleportIndicator.transform.SetParent(null);
		SceneManager.MoveGameObjectToScene(teleportIndicator, SceneManager.GetSceneByName("VVR_ROOT"));
		lineDashParent = new GameObject();
		SceneManager.MoveGameObjectToScene(lineDashParent, SceneManager.GetSceneByName("VVR_ROOT"));
		lineDashParent.name = "DashParent";
		if (lineDash == null)
		{
			lineDash = new GameObject("TempDash");
		}
	}

	private void Start()
	{
		teleportIndicator.SetActive(value: false);
	}

	private void FixedUpdate()
	{
		if (isLerpingToPosition)
		{
			LerpToPositionSpeed(_startPos, _endPos, _startRot, _endRot);
			if (!isLerpingToPosition && OnFinishTeleportEvent != null)
			{
				OnFinishTeleportEvent(_startPos, _endPos);
			}
			ToolManager.OnTeleport();
		}
	}

	private void Update()
	{
		if (ToolManager.AllTeleporters.TeleportEnabled && !PlayerScaler.instance.scaled)
		{
			if (TBInput.GetButtonDown(TBInput.Button.Action2, _toolGroup.Controller) && !_cancelDisabled)
			{
				timeHeld = 0f;
				ToggleLine(flag: false);
			}
			if (TBInput.GetButton(TBInput.Button.Action2, _toolGroup.Controller) && !_cancelDisabled)
			{
				timeHeld += Time.deltaTime;
				if (!teleportIndicator.activeSelf)
				{
					if (_toolGroup.CursorReticle.rendering)
					{
						_toolGroup.CursorReticle.ToggleOnOff(flag: false);
					}
					_toolGroup.CurrentTool = ToolGroup.Tool.Teleporter;
				}
				rayDirection = gun.transform.forward;
				CalculateHitPoint(ref hitPoint);
				DrawDashes();
				float num = Vector3.Dot(EagerMaster.instance.player.transform.up, rayDirection);
				if (num > 0.93f)
				{
					ToggleLine(flag: false);
					teleportIndicator.SetActive(value: false);
					canTeleportInternal = false;
				}
				else if (_toolGroup.CursorReticle.rendering)
				{
					_toolGroup.CursorReticle.ToggleOnOff(flag: false);
					_toolGroup.Manipulator.tipMesh.SetActive(value: false);
				}
				teleportIndicator.transform.position = hitPoint;
				teleportIndicator.transform.up = hitInfo.normal;
			}
			if (TBInput.GetButtonUp(TBInput.Button.Action2, _toolGroup.Controller) && !_cancelDisabled)
			{
				ToggleLine(flag: false);
				if (canTeleportInternal && timeHeld > ToolManager.AllTeleporters.MinHoldTime)
				{
					ToolManager.StopAllOtherTeleport(_toolGroup);
					isLerpingToPosition = true;
					if (orientOnSurface)
					{
						positionParameter = new Vector3(hitPoint.x, hitPoint.y, hitPoint.z);
						TeleportToPosition(positionParameter, hitInfo.normal);
					}
					else
					{
						positionParameter = new Vector3(hitPoint.x, hitPoint.y, hitPoint.z);
						TeleportToPosition(positionParameter);
					}
					if (_toolGroup.Manipulator.grabTargetDistance > 1.2f && _toolGroup.Manipulator.CurrentManipulatorMode != Manipulator.manipulatorMode.vacuum)
					{
						_toolGroup.Manipulator.AnimateGrabToLength(1.2f, 0.2f, dropOnTeleport, 1f);
					}
				}
				else
				{
					teleportIndicator.SetActive(value: false);
					teleportIndicator.transform.position = EagerMaster.instance.player.transform.position;
					_toolGroup.CurrentTool = ToolGroup.Tool.Leash;
					if (_toolGroup.Manipulator.CurrentManipulatorMode == Manipulator.manipulatorMode.grab && _toolGroup.Manipulator.HeldObjectBase != null)
					{
						_toolGroup.CursorReticle.ToggleOnOff(flag: true);
					}
				}
			}
			if (!canTeleportInternal)
			{
				_mat.color = noColor;
			}
			else
			{
				_mat.color = yesColor;
			}
		}
		if (TBInput.GetButtonUp(TBInput.Button.Action2, _toolGroup.Controller))
		{
			_cancelDisabled = false;
		}
	}

	private void CalculateHitPoint(ref Vector3 hitPoint)
	{
		Vector3 vector = rayDirection * teleportStrength;
		Vector3 vector2 = gravityCoefficient * Physics.gravity;
		oldTrajectoryPos = gun.transform.position;
		gunStuckInCollider = Physics.CheckSphere(gun.transform.position, 0.01f, layers.value, QueryTriggerInteraction.Ignore);
		if (gunStuckInCollider)
		{
			canTeleportInternal = false;
		}
		for (int i = 0; i < rayIterations - 1; i++)
		{
			trajectoryPos = vector * (i + 1) * rayIterationLength + vector2 * Mathf.Pow((float)(i + 1) * rayIterationLength, 2f) * 0.5f + gun.transform.position;
			trajectoryDir = trajectoryPos - oldTrajectoryPos;
			hitPoint = trajectoryPos;
			if (Physics.Raycast(oldTrajectoryPos, trajectoryDir.normalized, out hitInfo, trajectoryDir.magnitude, layers, QueryTriggerInteraction.Ignore))
			{
				hitPoint = hitInfo.point;
				endLength = hitInfo.distance;
				endInteration = (float)i * rayIterationLength;
				if (Vector3.Dot(hitInfo.normal, Vector3.up) > minTeleportDot && hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Teleport") && !Physics.CheckSphere(hitPoint, 0.1f, 1 << LayerMask.NameToLayer("DontTeleportThrough")) && !gunStuckInCollider)
				{
					Vector3 position = hitInfo.point + Vector3.up * EagerMaster.instance.headsetHeightManager.ProjectedHeight();
					int num = Physics.OverlapSphereNonAlloc(position, headroomSphereRadius, headroomCols, layers);
					bool flag = true;
					for (int j = 0; j < num; j++)
					{
						if (!headroomCols[j].isTrigger)
						{
							flag = false;
							break;
						}
					}
					canTeleportInternal = flag;
				}
				else
				{
					canTeleportInternal = false;
				}
				return;
			}
			oldTrajectoryPos = trajectoryPos;
		}
		teleportIndicator.SetActive(value: false);
		endInteration = (float)(rayIterations - 2) * rayIterationLength;
		canTeleportInternal = false;
	}

	private void DrawDashes()
	{
		Vector3 vector = Vector3.zero;
		Vector3 zero = Vector3.zero;
		Vector3 one = Vector3.one;
		Vector3 vector2 = rayDirection * teleportStrength;
		Vector3 vector3 = gravityCoefficient * Physics.gravity;
		for (int i = 0; i < marchingDashes - 1; i++)
		{
			ToggleLine(flag: true);
			if (i == dashIterations - 2)
			{
				teleportIndicator.SetActive(value: true);
			}
			one = vector;
			one = gun.transform.position + one;
			AssignDashAt(i, one);
			zero = vector2 * (i + 1) * dashIterationLength + vector3 * Mathf.Pow((float)(i + 1) * dashIterationLength, 2f) * 0.5f;
			vector = zero;
			zero = gun.transform.position + zero;
			if (endInteration < (float)(i + 1) * dashIterationLength && endLength * endLength < (oldTrajectoryPos - zero).sqrMagnitude)
			{
				for (int j = i; j < marchingDashes - 1; j++)
				{
					lineDashes[j].transform.position = one;
				}
				teleportIndicator.SetActive(value: true);
				break;
			}
		}
	}

	private void AssignDashAt(int index, Vector3 pos)
	{
		while (lineDashes.Count < marchingDashes - 1)
		{
			GameObject gameObject = Object.Instantiate(lineDash, pos, Quaternion.identity, lineDashParent.transform);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f) * dashScaleCoefficient;
			if (gameObject.GetComponentInChildren<Renderer>().material != _mat)
			{
				gameObject.GetComponentInChildren<Renderer>().material = _mat;
			}
			lineDashes.Add(gameObject);
		}
		while (lineDashes.Count > marchingDashes - 1)
		{
			Object.Destroy(lineDashes[0]);
			lineDashes.RemoveAt(0);
		}
		lineDashes[index].transform.position = pos;
	}

	private void LerpToPositionSpeed(Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot)
	{
		if (t == 0f)
		{
			startHeadsetPos = EagerMaster.instance.headsetHeightManager.headModel.localPosition;
			endHeadsetPos = EagerMaster.instance.headsetHeightManager.ProjectedLocalPosition();
		}
		t += Time.deltaTime;
		lerpPercentage = Mathf.Clamp01(t / lerpDuration);
		EagerMaster.instance.player.transform.position = Vector3.Lerp(startPos, endPos, positionCurve.Evaluate(lerpPercentage));
		EagerMaster.instance.player.transform.rotation = Quaternion.Lerp(startRot, endRot, rotationCurve.Evaluate(lerpPercentage));
		EagerMaster.instance.headsetHeightManager.headModel.transform.localPosition = Vector3.Lerp(startHeadsetPos, endHeadsetPos, lerpPercentage);
		if (lerpPercentage >= 1f)
		{
			StopTeleport();
		}
	}

	public void StopTeleport()
	{
		teleportIndicator.SetActive(value: false);
		if (_toolGroup.Manipulator.CurrentManipulatorMode == Manipulator.manipulatorMode.grab && _toolGroup.Manipulator.HeldObjectBase != null)
		{
			_toolGroup.CursorReticle.ToggleOnOff(flag: true);
		}
		hitPoint = EagerMaster.instance.player.transform.position;
		teleportIndicator.transform.position = EagerMaster.instance.player.transform.position;
		isLerpingToPosition = false;
		_toolGroup.CurrentTool = ToolGroup.Tool.Leash;
		teleportParticles.Stop();
		teleportParticles.Clear();
		lerpPercentage = 1f;
		timeHeld = 0f;
		t = 0f;
	}

	public void TeleportToPosition(Vector3 endPos)
	{
		TeleportToPosition(endPos, EagerMaster.instance.player.transform.up);
	}

	public void TeleportToPosition(Vector3 endPos, Vector3 upVector)
	{
		teleportIndicator.SetActive(value: true);
		_toolGroup.CursorReticle.ToggleOnOff(flag: false);
		_endPos = endPos;
		t = 0f;
		_startPos = EagerMaster.instance.player.transform.position;
		_startRot = EagerMaster.instance.player.transform.rotation;
		_endRot = Quaternion.FromToRotation(EagerMaster.instance.player.transform.up, upVector) * _startRot;
		if (!(_endPos == _startPos))
		{
			_endPos.x -= (EagerMaster.instance.player.transform.localRotation * EagerMaster.instance.mainCamera.transform.localPosition).x;
			_endPos.z -= (EagerMaster.instance.player.transform.localRotation * EagerMaster.instance.mainCamera.transform.localPosition).z;
			Vector3 worldPosition = endPos;
			worldPosition.y += 1.5f;
			lerpDuration = Vector3.Distance(_endPos, _startPos) * lerpSpeed;
			if (lerpDuration > 0.25f)
			{
				teleportParticles.transform.parent.position = EagerMaster.instance.mainCamera.transform.position;
				teleportParticles.transform.parent.LookAt(worldPosition);
				teleportParticles.Play();
			}
			GlobalCoroutineContainer.AddThread(playTeleportSound(lerpDuration), this);
			lerpPercentage = 0f;
			isLerpingToPosition = true;
		}
	}

	private IEnumerator playTeleportSound(float time)
	{
		yield return new WaitForSeconds(time - 0.4f);
		teleportSFX.randVolume = Mathf.Clamp01(lerpDuration) * 0.8f;
		teleportSFX.Play(_toolGroup.Controller, rumble: true);
	}

	public void ToggleLine(bool flag)
	{
		lineDashParent.SetActive(flag);
	}

	public static void SetTeleportEnabled(bool enabledIn)
	{
		ToolManager.AllTeleporters.TeleportEnabled = enabledIn;
		SafeDebug.Log("<color=##ffdf2bff>TeleportEnabled is now " + enabledIn + "</color>");
	}

	public void CancelDisableTeleport()
	{
		if (TBInput.GetButton(TBInput.Button.Action2, _toolGroup.Controller))
		{
			_cancelDisabled = true;
			ToggleLine(flag: false);
			_toolGroup.CurrentTool = ToolGroup.Tool.Leash;
			teleportIndicator.SetActive(value: false);
			teleportIndicator.transform.position = EagerMaster.instance.player.transform.position;
		}
	}

	public static bool GetTeleportEnabled()
	{
		return ToolManager.AllTeleporters.TeleportEnabled;
	}
}
