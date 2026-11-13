using System;
using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using UnityEngine;

public class ChazController : MonoBehaviour
{
	public enum chazMode
	{
		leftLegGrab,
		rightLegGrab,
		headGrab,
		standing,
		walking,
		waving,
		knockedOut,
		standingUp,
		bothLegsGrabbed
	}

	[HideInInspector]
	public enum eyeColor
	{
		Default,
		Thinking,
		Angry,
		Happy,
		Deactivated
	}

	[Serializable]
	public class ChazLook
	{
		public Transform target;

		public float duration = 0.2f;

		public Color color;

		public float pupilScalar = 1f;
	}

	public GameObject head;

	public GameObject eyeGlowGO;

	private Rigidbody headRB;

	public RootPositioner rootPositioner;

	private Transform rootPositionerTransform;

	private Transform headTransform;

	public float stepThreshold = 1f;

	private Vector3 initialOffset;

	private float initialHeight;

	private SpringJoint chazSpring;

	public float groundLevel;

	private Vector3 initialLeftOffset;

	private Vector3 initialRightOffset;

	private ConfigurableJoint storageJoint;

	public VVRDialogueSource chazSpeak;

	public HingeJoint leftHip;

	public HingeJoint rightHip;

	private chazMode currentChazMode = chazMode.standing;

	public Transform chazEye;

	public Transform chazPupil;

	public Transform eyeCenter;

	public Transform mouth;

	public GameObject particleIn;

	public Color eyeDefault;

	public Color eyeThinking;

	public Color eyeAngry;

	public Color eyeHappy;

	public Color eyeDeactivated;

	public Material eyeColorMat;

	public Material glowColorMat;

	public RotateObject chazCameraRotate;

	[HideInInspector]
	public Rigidbody[] chazRBs;

	public float stepSpeed = 0.05f;

	public float stepPastHeadDist = 0.1f;

	public bool canStep = true;

	private bool steppingActive = true;

	private bool straighteningActive = true;

	[SerializeField]
	[CurveRange(0f, 1f, 0f, 1f)]
	private AnimationCurve stepCurve;

	public float stepHeight = 0.05f;

	private float initialAngularDrag;

	private float initialSpring;

	private float initialDamper;

	private float initialDrag;

	private float initialPupilScale = 1f;

	public float walkingMoveForce = 60f;

	public bool poweredUp = true;

	public BoidController spawnBoids;

	[Space]
	[Header("LAZERS")]
	public GameObject lazerAshPrefab;

	public LineRenderer lazerLine;

	public GameObject lazerEnd;

	public GameObject lazerGO;

	public Color lazerColor;

	[HideInInspector]
	public bool isLazering;

	[Space]
	[Header("SFX")]
	public AudioEvent leftStepSFX;

	public AudioEvent rightStepSFX;

	public AudioEvent bodyStepSFX;

	public AudioLoop servoLoop;

	public AudioEvent openSound;

	public AudioEvent closeSound;

	[Space]
	[Header("Servo Values")]
	public float pitchMax = 1.5f;

	[Range(0f, 1f)]
	public float pitchRandomAmount = 1f;

	public float pitchRandomSpeed = 3f;

	public float pitchShiftSpeed = 10f;

	private Collider headCollider;

	private Rigidbody tempRB;

	private Vector3 flatRootPositioner;

	private Vector3 flatHead;

	private Vector3 flatTarget;

	private SafeCoroutine stepRoutine;

	private SafeCoroutine straightRoutine;

	private Vector3 eulerAngles;

	private Vector3 toTarget;

	private Rigidbody chillRB;

	private bool rightFootsTurn;

	private int straightenUpSteps = 999;

	private float stepTimerThreshold = 0.1f;

	private SafeCoroutine presentRoutine;

	private SafeCoroutine closeStorageRoutine;

	private SafeCoroutine grabRoutine;

	private GameObject objectToPresent;

	private GameObject _saveBoid;

	public bool isPresenting;

	private GameObject _objectToGrab;

	private GameObject lazeredObject;

	private SafeCoroutine lazerRoutine;

	private float lazerT;

	private GameObject lazerAsh;

	private FixedJoint objectJoint;

	public Rigidbody rootPositionerRB;

	private float currentVolume;

	private float currentPitch;

	[HideInInspector]
	public bool isMagnetized;

	private Vector3 w;

	private Vector3 e;

	private Vector3 groundPoint;

	public Vector3 groundOrigin = Vector3.zero;

	public Vector3 groundUp = Vector3.up;

	private float stepLerp;

	private Vector3 flatLeft;

	private Vector3 flatRight;

	private Vector3 endPos;

	private Vector3 startPos;

	private Rigidbody furthestFootRB;

	private float stepLength;

	private Vector3 pupilToTarget;

	private Vector3 pupilPoint;

	private Vector2 pupilCoordinates = new Vector2(0f, 0f);

	private Vector3 v;

	private Vector3 d;

	private Vector3 projectedPoint;

	private Vector3 toTargetConstrained;

	private bool eyeAnimating = true;

	private float testAngle;

	private float frontBack = 1f;

	private JointSpring hipSpring;

	private float finalStrideLength;

	private float straightenUpPause;

	private float t;

	private float stepTimer;

	public float maxStepDistance = 1.2f;

	private float upT;

	private float downT;

	private ChazLook defaultEyeLook;

	private ChazLook currentEyeLook;

	private List<ChazLook> chazLookQueue = new List<ChazLook>();

	private float chazLookTimer;

	private float emotionPupilScalar = 1f;

	private float cld = 1f;

	private SafeCoroutine eyeChangeRoutine;

	private SafeCoroutine pDownRoutine;

	private SafeCoroutine pUpRoutine;

	private SafeCoroutine lookRoutine;

	private float workingStepHeight;

	private bool fixedActive;

	private bool walking;

	public Vector3 walkingTarget;

	private float targetVolume;

	private float targetPitch;

	public GameObject saveBoid => _saveBoid;

	private void Start()
	{
		servoLoop.Play();
		chazSpring = head.GetComponent<SpringJoint>();
		headRB = head.GetComponent<Rigidbody>();
		headTransform = head.transform;
		rootPositionerTransform = rootPositioner.transform;
		storageJoint = head.GetComponent<ConfigurableJoint>();
		headCollider = head.GetComponent<Collider>();
		initialOffset = transform.position - rootPositionerTransform.position;
		chazRBs = GetComponentsInChildren<Rigidbody>();
		initialHeight = headTransform.position.y - groundLevel;
		initialDrag = headRB.drag;
		initialAngularDrag = headRB.angularDrag;
		initialSpring = chazSpring.spring;
		initialDamper = chazSpring.damper;
		initialLeftOffset = rootPositioner.pos1.position - headTransform.position;
		initialRightOffset = rootPositioner.pos2.position - headTransform.position;
		initialPupilScale = chazPupil.transform.localScale.x;
		if (!poweredUp)
		{
			PowerDown();
		}
		else
		{
			eyeColorMat.SetColor("_Color", eyeDefault);
			glowColorMat.SetColor("_Color", new Vector4(eyeDefault.r - 0.3f, eyeDefault.g - 0.3f, eyeDefault.b - 0.3f, 1f));
		}
		defaultEyeLook = new ChazLook();
		defaultEyeLook.color = eyeDefault;
		defaultEyeLook.target = EagerMaster.instance.mainCamera.transform;
		defaultEyeLook.duration = 0f;
		defaultEyeLook.pupilScalar = 1f;
		currentEyeLook = defaultEyeLook;
		rootPositionerRB = rootPositioner.GetComponent<Rigidbody>();
	}

	public void SetChazMode(chazMode newMode)
	{
		if (newMode != currentChazMode)
		{
			switch (newMode)
			{
			case chazMode.headGrab:
				straighteningActive = false;
				steppingActive = true;
				stepTimerThreshold = 0.1f;
				break;
			case chazMode.bothLegsGrabbed:
				straighteningActive = false;
				steppingActive = true;
				stepTimerThreshold = 0.3f;
				break;
			case chazMode.leftLegGrab:
				straighteningActive = false;
				steppingActive = true;
				stepTimerThreshold = 0.2f;
				break;
			case chazMode.rightLegGrab:
				straighteningActive = false;
				steppingActive = true;
				stepTimerThreshold = 0.2f;
				break;
			case chazMode.standing:
				straighteningActive = true;
				steppingActive = true;
				stepTimerThreshold = 0.1f;
				break;
			}
			currentChazMode = newMode;
		}
	}

	public void GoLimpSpring(float percentageInitial = 0f)
	{
		chazSpring.spring = initialSpring * percentageInitial;
		chazSpring.damper = initialDamper * percentageInitial;
	}

	public void GoLimpDrag()
	{
		for (int i = 0; i < chazRBs.Length; i++)
		{
			tempRB = chazRBs[i];
			if (tempRB != null)
			{
				tempRB.drag = 0f;
				tempRB.angularDrag = 0.03f;
			}
		}
	}

	public void ResetDrag()
	{
		for (int i = 0; i < chazRBs.Length; i++)
		{
			tempRB = chazRBs[i];
			if (tempRB != null)
			{
				tempRB.drag = initialDrag;
				tempRB.angularDrag = initialAngularDrag;
			}
		}
	}

	public void ResetSpring()
	{
		chazSpring.spring = initialSpring;
		chazSpring.damper = initialDamper;
	}

	public void PresentObject(GameObject newObject, bool radio = false)
	{
		objectToPresent = newObject;
		isPresenting = true;
		if (presentRoutine != null && presentRoutine.IsRunning)
		{
			presentRoutine.Stop();
		}
		if (closeStorageRoutine != null && closeStorageRoutine.IsRunning)
		{
			closeStorageRoutine.Stop();
		}
		presentRoutine = GlobalCoroutineContainer.AddThread(PresentObjectUpdate(boid: false, radio), this);
	}

	public void PresentBoid()
	{
		if (!(spawnBoids == null) && spawnBoids.boidList.Count != 0)
		{
			if (presentRoutine != null && presentRoutine.IsRunning)
			{
				presentRoutine.Stop();
				isPresenting = false;
			}
			if (closeStorageRoutine != null && closeStorageRoutine.IsRunning)
			{
				closeStorageRoutine.Stop();
			}
			presentRoutine = GlobalCoroutineContainer.AddThread(PresentObjectUpdate(boid: true), this);
		}
	}

	public void GrabObject(GameObject newObject, bool destroy = false)
	{
		SafeDebug.Log("Chaz is grabbing an object");
		if (newObject == null || !newObject.activeInHierarchy || newObject.GetComponent<Rigidbody>() == null || newObject.GetComponent<Collider>() == null || (objectJoint != null && objectJoint.connectedBody.gameObject == newObject))
		{
			return;
		}
		_objectToGrab = newObject;
		if (presentRoutine != null && presentRoutine.IsRunning)
		{
			presentRoutine.Stop();
		}
		if (grabRoutine == null || !grabRoutine.IsRunning)
		{
			if (closeStorageRoutine != null && closeStorageRoutine.IsRunning)
			{
				closeStorageRoutine.Stop();
			}
			grabRoutine = GlobalCoroutineContainer.AddThread(GrabObjectUpdate(destroy), this);
		}
	}

	public void CancelLazer()
	{
		if (lazerRoutine != null && lazerRoutine.IsRunning)
		{
			lazerRoutine.Stop();
		}
		lazerGO.SetActive(value: false);
		lazerRoutine = null;
		isLazering = false;
	}

	public void LazerObject(GameObject targetObject, bool destroy = false)
	{
		if (poweredUp)
		{
			lazeredObject = targetObject;
			if (lazerRoutine != null && lazerRoutine.IsRunning)
			{
				lazerRoutine.Stop();
			}
			lazerRoutine = GlobalCoroutineContainer.AddThread(LazerUpdate(destroy), this);
		}
	}

	private IEnumerator LazerUpdate(bool destroy)
	{
		isLazering = true;
		EyeLookAt(lazeredObject.transform, 1f, eyeAngry, 0.3f, immediately: true);
		lazerT = 0f;
		while (lazerT < 0.3f)
		{
			lazerT += Time.deltaTime;
			yield return null;
		}
		lazerGO.SetActive(value: true);
		lazerT = 0f;
		while (lazerT < 0.25f)
		{
			lazerEnd.transform.position = lazeredObject.transform.position;
			lazerLine.SetPosition(0, eyeCenter.position);
			lazerLine.SetPosition(1, lazeredObject.transform.position);
			lazerT += Time.deltaTime;
			yield return null;
		}
		lazerAsh = UnityEngine.Object.Instantiate(lazerAshPrefab, lazeredObject.transform.position, lazeredObject.transform.rotation);
		lazerAsh.transform.parent = EagerScenes.instance.ActiveScene.transform;
		lazerAsh.name = "LazerAshPile";
		lazerAsh.SetActive(value: true);
		lazeredObject.SetActive(value: false);
		lazerT = 0f;
		while (lazerT < 0.25f)
		{
			lazerLine.SetPosition(0, eyeCenter.position);
			lazerLine.SetPosition(1, lazerAsh.transform.position);
			lazerT += Time.deltaTime;
			yield return null;
		}
		lazerGO.SetActive(value: false);
		lazerRoutine = null;
		isLazering = false;
	}

	private bool SetupObject(bool boid = false, bool radio = false)
	{
		if (boid)
		{
			objectToPresent = null;
			for (int i = 0; i < spawnBoids.boidList.Count; i++)
			{
				if (spawnBoids.boidList[i] != null && !spawnBoids.boidList[i].gameObject.activeSelf)
				{
					objectToPresent = spawnBoids.boidList[i].gameObject;
					break;
				}
			}
			if (objectToPresent == null)
			{
				return false;
			}
			objectToPresent.transform.rotation = Quaternion.LookRotation(mouth.forward, mouth.up);
		}
		else
		{
			objectToPresent.transform.rotation = Quaternion.LookRotation(-mouth.forward, mouth.up);
		}
		objectToPresent.transform.position = mouth.position + mouth.up * -0.3f;
		CreatePresentationJoint(objectToPresent, boid, radio);
		return true;
	}

	private void CreatePresentationJoint(GameObject jointObject, bool boid = false, bool radio = false)
	{
		Physics.IgnoreCollision(jointObject.GetComponent<Collider>(), headCollider, ignore: true);
		jointObject.SetActive(value: true);
		objectJoint = mouth.gameObject.AddComponent<FixedJoint>();
		if (!boid && !radio)
		{
			objectJoint.autoConfigureConnectedAnchor = false;
			objectJoint.connectedAnchor = Vector3.up * 0.9f;
		}
		objectJoint.connectedBody = jointObject.GetComponent<Rigidbody>();
	}

	private IEnumerator GrabObjectUpdate(bool destroy)
	{
		closeSound.Play();
		while ((double)storageJoint.connectedAnchor.y > 0.21)
		{
			storageJoint.connectedAnchor -= new Vector3(0f, Time.deltaTime * 1f, 0f);
			if (storageJoint.connectedAnchor.y <= 0.21f)
			{
				storageJoint.connectedAnchor = new Vector3(0f, 0.2f, 0f);
			}
			yield return null;
		}
		DestroyJoint();
		openSound.Play();
		while (storageJoint.connectedAnchor.y < 0.39f)
		{
			storageJoint.connectedAnchor += new Vector3(0f, Time.deltaTime * 0.8f, 0f);
			if (storageJoint.connectedAnchor.y >= 0.39f)
			{
				storageJoint.connectedAnchor = new Vector3(0f, 0.4f, 0f);
			}
			yield return null;
		}
		Vector3 goal = mouth.position + mouth.up * -0.3f;
		Vector3 toMouth = goal - _objectToGrab.transform.position;
		Vector3 initialPosition = _objectToGrab.transform.position;
		Quaternion initialRotation = _objectToGrab.transform.rotation;
		float animationDuration = Mathf.Clamp(toMouth.magnitude * 0.15f, 0.2f, 3f);
		float timer = 0f;
		float lerp = 0f;
		particleIn.SetActive(value: true);
		while (lerp < 1f)
		{
			yield return null;
			goal = mouth.position + mouth.up * -0.3f;
			ObjectBase grabbedObjectBase = _objectToGrab.GetComponentInChildren<ObjectBase>();
			if (grabbedObjectBase != null)
			{
				grabbedObjectBase.ForceDropByPlayer();
			}
			timer += Time.deltaTime;
			lerp = Mathf.Clamp01(timer / animationDuration);
			_objectToGrab.transform.position = Vector3.Lerp(initialPosition, goal, lerp);
			_objectToGrab.transform.rotation = Quaternion.Lerp(initialRotation, Quaternion.LookRotation(-mouth.forward, mouth.up), lerp);
		}
		CreatePresentationJoint(_objectToGrab);
		particleIn.SetActive(value: false);
		if (!destroy)
		{
			if (objectJoint != null)
			{
				objectJoint.breakForce = float.PositiveInfinity;
			}
			while (!ToolManager.PlayerGrabbingGameObject(_objectToGrab))
			{
				isPresenting = true;
				yield return null;
			}
			isPresenting = false;
			if (objectJoint != null)
			{
				objectJoint.breakForce = 20f;
			}
			yield break;
		}
		while ((double)storageJoint.connectedAnchor.y > 0.21)
		{
			storageJoint.connectedAnchor -= new Vector3(0f, Time.deltaTime * 1f, 0f);
			if (storageJoint.connectedAnchor.y <= 0.21f)
			{
				storageJoint.connectedAnchor = new Vector3(0f, 0.2f, 0f);
			}
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.3f, this);
		DestroyJoint();
		_objectToGrab.SetActive(value: false);
	}

	public void DestroyJoint()
	{
		if (objectJoint != null)
		{
			objectJoint.connectedBody.gameObject.SetActive(value: false);
			Physics.IgnoreCollision(objectJoint.connectedBody.GetComponent<Collider>(), headCollider, ignore: false);
			UnityEngine.Object.Destroy(objectJoint);
			objectJoint = null;
		}
	}

	public void DropChaz()
	{
		ToolManager.PlayerGrabbingObjectType<ObjectChazLeg>(dropIfGrabbing: true);
	}

	public void StopCoroutines()
	{
		if (stepRoutine != null && stepRoutine.IsRunning)
		{
			stepRoutine.Stop();
			stepRoutine = null;
		}
		if (straightRoutine != null && straightRoutine.IsRunning)
		{
			straightRoutine.Stop();
			straightRoutine = null;
		}
	}

	private IEnumerator PresentObjectUpdate(bool boid, bool radio = false)
	{
		if (grabRoutine != null)
		{
			while (grabRoutine.IsRunning)
			{
				yield return null;
			}
		}
		if ((double)storageJoint.connectedAnchor.y > 0.22)
		{
			closeSound.Play();
		}
		while ((double)storageJoint.connectedAnchor.y > 0.21)
		{
			storageJoint.connectedAnchor -= new Vector3(0f, Time.deltaTime * 1f, 0f);
			if (storageJoint.connectedAnchor.y <= 0.21f)
			{
				storageJoint.connectedAnchor = new Vector3(0f, 0.2f, 0f);
			}
			yield return null;
		}
		DestroyJoint();
		if (!SetupObject(boid, radio))
		{
			yield break;
		}
		openSound.Play();
		while (storageJoint.connectedAnchor.y < 0.39f)
		{
			storageJoint.connectedAnchor += new Vector3(0f, Time.deltaTime * 0.8f, 0f);
			if (storageJoint.connectedAnchor.y >= 0.39f)
			{
				storageJoint.connectedAnchor = new Vector3(0f, 0.4f, 0f);
			}
			yield return null;
		}
		if (boid)
		{
			objectToPresent.GetComponent<ObjectBoid>().Power = true;
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(1f, this);
			objectJoint.breakForce = 0f;
			_saveBoid = objectToPresent;
		}
		else
		{
			while (!ToolManager.PlayerGrabbingGameObject(objectToPresent))
			{
				objectJoint.breakForce = float.PositiveInfinity;
				yield return null;
			}
			objectJoint.breakForce = 20f;
		}
		objectToPresent = null;
		isPresenting = false;
	}

	private IEnumerator closeStorageUpdate(float delay)
	{
		while (delay > 0f)
		{
			delay -= Time.deltaTime;
			yield return null;
		}
		closeSound.Play();
		while ((double)storageJoint.connectedAnchor.y > 0.21)
		{
			storageJoint.connectedAnchor -= new Vector3(0f, Time.deltaTime * 1f, 0f);
			if (storageJoint.connectedAnchor.y <= 0.21f)
			{
				storageJoint.connectedAnchor = new Vector3(0f, 0.2f, 0f);
			}
			yield return null;
		}
		DestroyJoint();
	}

	public void ReStraighten()
	{
		straightenUpSteps = 0;
	}

	public void CloseStorage(float delay = 0f)
	{
		if (presentRoutine != null && presentRoutine.IsRunning)
		{
			presentRoutine.Stop();
		}
		if (closeStorageRoutine != null && closeStorageRoutine.IsRunning)
		{
			closeStorageRoutine.Stop();
		}
		closeStorageRoutine = GlobalCoroutineContainer.AddThread(closeStorageUpdate(delay), this);
	}

	private void Update()
	{
		currentVolume = Mathf.Lerp(currentVolume, targetVolume, 10f * Time.deltaTime);
		currentPitch = Mathf.Lerp(currentPitch, targetPitch, pitchShiftSpeed * Time.deltaTime);
		servoLoop.SetSourceVolume(currentVolume);
		servoLoop.SetSourcePitch(currentPitch);
		UpdateEye();
		flatRootPositioner = PointOnGround(rootPositionerTransform.position);
		flatHead = PointOnGround(headTransform.position);
		stepTimer += Time.deltaTime;
		if ((flatRootPositioner - flatHead).sqrMagnitude > stepThreshold && canStep && steppingActive && stepTimer > stepTimerThreshold)
		{
			if (straightRoutine != null && straightRoutine.IsRunning)
			{
				straightRoutine.Stop();
				straightRoutine = null;
			}
			if (!isMagnetized)
			{
				stepRoutine = GlobalCoroutineContainer.AddThread(Stepping(), this);
			}
		}
		else if (straightenUpSteps < 3 && straightRoutine == null && stepTimer > stepTimerThreshold && straighteningActive)
		{
			straightenUpPause += Time.deltaTime;
			if (straightenUpPause > 0.35f && !isMagnetized)
			{
				straightRoutine = GlobalCoroutineContainer.AddThread(Straightening(), this);
			}
		}
	}

	private Vector3 PointOnGround(Vector3 point)
	{
		w = point - groundOrigin;
		e = Vector3.Project(w, groundUp);
		groundPoint = point - e;
		return groundPoint;
	}

	public void UpdateEye()
	{
		if (poweredUp)
		{
			toTarget = (currentEyeLook.target.position - chazEye.position).normalized;
			toTargetConstrained = toTarget - Vector3.Project(toTarget, chazEye.up);
			testAngle = Quaternion.Angle(Quaternion.LookRotation(toTargetConstrained), Quaternion.LookRotation(chazEye.forward));
			if (testAngle > 35f)
			{
				eyeAnimating = true;
			}
			if (testAngle < 6f)
			{
				eyeAnimating = false;
			}
			if (eyeAnimating)
			{
				chazEye.rotation = Quaternion.Lerp(chazEye.rotation, Quaternion.LookRotation(toTargetConstrained), 9f * Time.deltaTime);
				eulerAngles = chazEye.localEulerAngles;
				eulerAngles.x = 0f;
				eulerAngles.z = 0f;
				chazEye.localEulerAngles = eulerAngles;
				targetPitch = Mathf.Lerp(pitchMax, pitchMax * Mathf.PerlinNoise(Time.time * pitchRandomSpeed, 0f), pitchRandomAmount);
				targetVolume = 1f;
			}
			pupilToTarget = (currentEyeLook.target.position - eyeCenter.position).normalized;
			pupilPoint = eyeCenter.position + pupilToTarget;
			projectedPoint = chazEye.InverseTransformPoint(pupilPoint);
			pupilCoordinates = Vector2.Lerp(pupilCoordinates, new Vector2(projectedPoint.x, projectedPoint.y), 10f * Time.deltaTime);
			if (Vector3.Dot(-leftHip.transform.forward, pupilToTarget) > 0f)
			{
				frontBack = 1f;
			}
			else
			{
				frontBack = -1f;
			}
			UpdatePupil(pupilCoordinates);
			hipSpring = leftHip.spring;
			hipSpring.targetPosition = pupilCoordinates.y * -75f * frontBack - 5f;
			leftHip.spring = hipSpring;
			rightHip.spring = hipSpring;
		}
	}

	private void UpdatePupil(Vector2 pupilPos)
	{
		chazPupil.localEulerAngles = new Vector3(0f, 26f * pupilPos.x, 0f);
		chazPupil.localPosition = new Vector3(0f, 0.08f * pupilPos.y, 0f);
	}

	private IEnumerator Stepping()
	{
		stepTimer = 0f;
		straightenUpSteps = 0;
		straightenUpPause = 0f;
		canStep = false;
		flatLeft = PointOnGround(rootPositioner.pos1.position);
		flatRight = PointOnGround(rootPositioner.pos2.position);
		switch (currentChazMode)
		{
		case chazMode.leftLegGrab:
			furthestFootRB = rootPositioner.pos2.GetComponent<Rigidbody>();
			endPos = flatHead + (Vector3.MoveTowards(flatRight, flatHead, stepPastHeadDist) - flatRight) + headTransform.right * 0.2f;
			endPos = Vector3.MoveTowards(rootPositioner.pos2.position, endPos, maxStepDistance);
			rightFootsTurn = false;
			break;
		case chazMode.rightLegGrab:
			furthestFootRB = rootPositioner.pos1.GetComponent<Rigidbody>();
			endPos = flatHead + (Vector3.MoveTowards(flatLeft, flatHead, stepPastHeadDist) - flatLeft) + headTransform.right * -0.2f;
			endPos = Vector3.MoveTowards(rootPositioner.pos1.position, endPos, maxStepDistance);
			rightFootsTurn = true;
			break;
		default:
			if ((flatLeft - flatHead).sqrMagnitude > (flatRight - flatHead).sqrMagnitude)
			{
				furthestFootRB = rootPositioner.pos1.GetComponent<Rigidbody>();
				endPos = flatHead + (Vector3.MoveTowards(flatLeft, flatHead, stepPastHeadDist) - flatLeft) + headTransform.right * -0.2f;
				endPos = Vector3.MoveTowards(rootPositioner.pos1.position, endPos, maxStepDistance);
				rightFootsTurn = true;
			}
			else
			{
				furthestFootRB = rootPositioner.pos2.GetComponent<Rigidbody>();
				endPos = flatHead + (Vector3.MoveTowards(flatRight, flatHead, stepPastHeadDist) - flatRight) + headTransform.right * 0.2f;
				endPos = Vector3.MoveTowards(rootPositioner.pos2.position, endPos, maxStepDistance);
				rightFootsTurn = false;
			}
			break;
		}
		stepLerp = 0f;
		startPos = furthestFootRB.position;
		t = 0f;
		stepLength = Vector3.Distance(startPos, endPos);
		while (stepLerp < 0.99f)
		{
			t += Time.deltaTime;
			stepLerp = t / (stepSpeed * stepLength + 0.07f);
			fixedActive = true;
			workingStepHeight = stepHeight;
			yield return null;
		}
		fixedActive = false;
		canStep = true;
		stepRoutine = null;
	}

	private IEnumerator Straightening()
	{
		stepTimer = 0f;
		straightenUpSteps++;
		if (!rightFootsTurn)
		{
			furthestFootRB = rootPositioner.pos1.GetComponent<Rigidbody>();
			endPos = headTransform.position + initialLeftOffset;
			rightFootsTurn = true;
		}
		else
		{
			furthestFootRB = rootPositioner.pos2.GetComponent<Rigidbody>();
			endPos = headTransform.position + initialRightOffset;
			rightFootsTurn = false;
		}
		stepLerp = 0f;
		startPos = furthestFootRB.position;
		float stepT = 0f;
		stepLength = Vector3.Distance(startPos, endPos);
		if (stepLength < 0.25f)
		{
			straightenUpSteps++;
		}
		while (stepLerp < 0.99f)
		{
			stepT += Time.deltaTime;
			stepLerp = stepT / (stepSpeed * stepLength + 0.2f);
			fixedActive = true;
			workingStepHeight = stepHeight * 0.3f;
			yield return null;
		}
		fixedActive = false;
		straightRoutine = null;
	}

	public void PowerUp(bool pitch = false, float delay = 0f)
	{
		if (pUpRoutine != null && pUpRoutine.IsRunning)
		{
			pUpRoutine.Stop();
		}
		pUpRoutine = GlobalCoroutineContainer.AddThread(PowerUpRoutine(delay), this);
		if (pitch && chazSpeak != null)
		{
			chazSpeak.PitchUpUnpause(1f);
		}
	}

	public void PowerDown(bool pitch = false, float offlineDuration = 0f)
	{
		if (pDownRoutine != null && pDownRoutine.IsRunning)
		{
			pDownRoutine.Stop();
		}
		pDownRoutine = GlobalCoroutineContainer.AddThread(PowerDownRoutine(offlineDuration), this);
		if (pitch && chazSpeak != null)
		{
			chazSpeak.PitchDownPause(1f);
		}
	}

	private IEnumerator PowerUpRoutine(float delay)
	{
		upT = 0f;
		while (upT < delay)
		{
			upT += Time.deltaTime;
			yield return null;
		}
		chazCameraRotate.rotation = new Vector3(0f, 2000f, 0f);
		eyeGlowGO.SetActive(value: true);
		ResetDrag();
		ResetSpring();
		poweredUp = true;
		pUpRoutine = null;
	}

	private IEnumerator PowerDownRoutine(float offlineDuration)
	{
		EyeLookAt(rootPositioner.transform, 1f, eyeColor.Deactivated, immediately: true);
		GoLimpSpring(0.65f);
		downT = 0f;
		while (downT < 0.5f)
		{
			downT += Time.deltaTime;
			chazCameraRotate.rotation = new Vector3(0f, (1f - downT / 0.5f) * 2000f, 0f);
			yield return null;
		}
		chazCameraRotate.rotation = new Vector3(0f, 0f, 0f);
		eyeGlowGO.SetActive(value: false);
		poweredUp = false;
		if (offlineDuration > 0f)
		{
			if (pUpRoutine != null && pUpRoutine.IsRunning)
			{
				pUpRoutine.Stop();
			}
			pUpRoutine = GlobalCoroutineContainer.AddThread(PowerUpRoutine(offlineDuration), this);
		}
		pDownRoutine = null;
	}

	public void JointBroke()
	{
		CloseStorage(0.3f);
		Physics.IgnoreCollision(headCollider, objectJoint.connectedBody.GetComponent<Collider>(), ignore: false);
	}

	public void WalkTowards(Vector3 target, float distance)
	{
		WalkTo(headRB.position + distance * (target - headRB.position).normalized);
	}

	public void WalkTo(Vector3 target)
	{
		walkingTarget = new Vector3(target.x, initialHeight, target.z);
		walking = true;
	}

	public bool GetWalking()
	{
		return walking;
	}

	public void StopWalking()
	{
		walking = false;
	}

	public void EyeLookAt(Transform target, bool immediately)
	{
		EyeLookAt(target, 0f, eyeDefault, 1f, immediately);
	}

	public void EyeLookAt(Transform target)
	{
		EyeLookAt(target, 0f, eyeDefault, 1f, immediately: true);
	}

	public void EyeLookAt(Transform target, float duration, eyeColor color, bool immediately)
	{
		Color color2;
		switch (color)
		{
		case eyeColor.Default:
			color2 = eyeDefault;
			emotionPupilScalar = 1f;
			break;
		case eyeColor.Thinking:
			color2 = eyeThinking;
			emotionPupilScalar = 1.2f;
			break;
		case eyeColor.Angry:
			color2 = eyeAngry;
			emotionPupilScalar = 0.6f;
			break;
		case eyeColor.Happy:
			color2 = eyeHappy;
			emotionPupilScalar = 1.3f;
			break;
		case eyeColor.Deactivated:
			color2 = eyeDeactivated;
			emotionPupilScalar = 0.3f;
			break;
		default:
			color2 = eyeDefault;
			emotionPupilScalar = 1f;
			break;
		}
		EyeLookAt(target, duration, color2, emotionPupilScalar, immediately);
	}

	public void EyeLookAt(Transform target, float duration, Color color, float pupilScalar, bool immediately)
	{
		ChazLook chazLook = new ChazLook();
		chazLook.color = color;
		chazLook.target = target;
		chazLook.duration = duration;
		chazLook.pupilScalar = pupilScalar;
		if (duration > 0f)
		{
			if (immediately)
			{
				if (lookRoutine != null && lookRoutine.IsRunning)
				{
					lookRoutine.Stop();
				}
				chazLookTimer = 0f;
				chazLookQueue.Clear();
				chazLookQueue.Add(chazLook);
				lookRoutine = GlobalCoroutineContainer.AddThread(LookQueueManager(), this);
			}
			else
			{
				chazLookQueue.Add(chazLook);
				if (lookRoutine == null)
				{
					lookRoutine = GlobalCoroutineContainer.AddThread(LookQueueManager(), this);
				}
			}
			return;
		}
		defaultEyeLook = chazLook;
		if (immediately)
		{
			if (lookRoutine != null && lookRoutine.IsRunning)
			{
				lookRoutine.Stop();
				lookRoutine = null;
			}
			chazLookTimer = 0f;
			chazLookQueue.Clear();
			currentEyeLook = defaultEyeLook;
			eyeAnimating = true;
			if (eyeChangeRoutine != null && eyeChangeRoutine.IsRunning)
			{
				eyeChangeRoutine.Stop();
			}
			eyeChangeRoutine = GlobalCoroutineContainer.AddThread(EyeChangeAnimation(chazLook.color, 0.2f, chazLook.pupilScalar), this);
		}
	}

	private IEnumerator LookQueueManager()
	{
		while (chazLookQueue.Count > 0)
		{
			currentEyeLook = chazLookQueue[0];
			eyeAnimating = true;
			if (eyeChangeRoutine != null && eyeChangeRoutine.IsRunning)
			{
				eyeChangeRoutine.Stop();
			}
			eyeChangeRoutine = GlobalCoroutineContainer.AddThread(EyeChangeAnimation(currentEyeLook.color, 0.2f, currentEyeLook.pupilScalar), this);
			cld = currentEyeLook.duration;
			chazLookQueue.RemoveAt(0);
			chazLookTimer = 0f;
			while (chazLookTimer < cld)
			{
				if (poweredUp)
				{
					chazLookTimer += Time.deltaTime;
				}
				yield return null;
			}
		}
		currentEyeLook = defaultEyeLook;
		eyeAnimating = true;
		if (eyeChangeRoutine != null && eyeChangeRoutine.IsRunning)
		{
			eyeChangeRoutine.Stop();
		}
		eyeChangeRoutine = GlobalCoroutineContainer.AddThread(EyeChangeAnimation(defaultEyeLook.color, 0.2f, currentEyeLook.pupilScalar), this);
		lookRoutine = null;
	}

	private IEnumerator EyeChangeAnimation(Color newColor, float duration, float pupilScalar)
	{
		Color oldColor = eyeColorMat.color;
		float eyeLerp = 0f;
		float eyeTimer = 0f;
		Vector3 oldPupilScale = chazPupil.transform.localScale;
		Vector3 newPupilScale = new Vector3(initialPupilScale * pupilScalar, initialPupilScale * pupilScalar, 1f);
		while (eyeLerp < 1f)
		{
			eyeTimer += Time.deltaTime;
			eyeLerp = eyeTimer / duration;
			eyeColorMat.SetColor("_Color", Color.Lerp(oldColor, newColor, eyeLerp));
			glowColorMat.SetColor("_Color", Color.Lerp(oldColor, new Vector4(newColor.r - 0.3f, newColor.g - 0.3f, newColor.b - 0.3f, 1f), eyeLerp));
			chazPupil.transform.localScale = Vector3.Lerp(oldPupilScale, newPupilScale, eyeLerp);
			yield return null;
		}
	}

	private void FixedUpdate()
	{
		if (fixedActive && poweredUp)
		{
			targetPitch = Mathf.Lerp(pitchMax, pitchMax * Mathf.PerlinNoise(Time.time * pitchRandomSpeed, 0f), pitchRandomAmount);
			targetVolume = 1f;
			furthestFootRB.MovePosition(Vector3.Lerp(startPos, endPos, stepLerp) + groundUp * workingStepHeight * stepCurve.Evaluate(stepLerp));
		}
		else if (!eyeAnimating)
		{
			targetPitch = 0f;
			targetVolume = 0.5f;
		}
		if (walking && poweredUp)
		{
			headRB.AddForce((walkingTarget - headRB.position).normalized * walkingMoveForce * Time.deltaTime * 60f);
		}
	}

	private void OnDisable()
	{
	}

	public void MoveChaz(Vector3 targetpos, bool force = false, bool reEnablePhysics = true)
	{
		fixedActive = false;
		stepLerp = 0f;
		steppingActive = false;
		straighteningActive = false;
		walkingTarget = targetpos;
		if (force || ((presentRoutine == null || !presentRoutine.IsRunning) && (grabRoutine == null || !grabRoutine.IsRunning) && (closeStorageRoutine == null || !closeStorageRoutine.IsRunning)))
		{
			KillCoroutines();
			DisablePhysics();
			DestroyJoint();
			transform.position = targetpos;
			for (int i = 0; i < chazRBs.Length; i++)
			{
				chazRBs[i].velocity = Vector3.zero;
				chazRBs[i].angularVelocity = Vector3.zero;
			}
			if (reEnablePhysics)
			{
				EnablePhysics();
			}
		}
	}

	public void KillCoroutines()
	{
		if (straightRoutine != null && straightRoutine.IsRunning)
		{
			straightRoutine.Stop();
			straightRoutine = null;
		}
		if (stepRoutine != null && stepRoutine.IsRunning)
		{
			stepRoutine.Stop();
			stepRoutine = null;
		}
		currentChazMode = chazMode.standing;
	}

	public void DisablePhysics()
	{
		for (int i = 0; i < chazRBs.Length; i++)
		{
			chazRBs[i].isKinematic = true;
			chazRBs[i].velocity = Vector3.zero;
			chazRBs[i].angularVelocity = Vector3.zero;
		}
	}

	public void EnablePhysics()
	{
		for (int i = 0; i < chazRBs.Length; i++)
		{
			chazRBs[i].isKinematic = false;
		}
	}

	public void AddChazForce(Vector3 force)
	{
		for (int i = 0; i < chazRBs.Length; i++)
		{
			chazRBs[i].AddForce(force);
		}
	}
}
