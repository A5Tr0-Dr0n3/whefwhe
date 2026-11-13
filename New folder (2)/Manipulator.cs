using System.Collections;
using TButt;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Bezier))]
public class Manipulator : MonoBehaviour
{
	public delegate void ToolSwitchEventHandler();

	public enum manipulatorMode
	{
		grab,
		vacuum,
		portal,
		zap
	}

	public enum grabbableStatus
	{
		grabbable,
		button,
		neutral,
		noHit
	}

	public enum ButtonPressSFXType
	{
		normal,
		loginMenu,
		none
	}

	private struct ChoppedCone
	{
		private Vector3 originOffset;

		private Vector3 direction;

		private float minDotProd;

		public ChoppedCone(Vector3 origin, Vector3 direction, float startRadius, float endRadius, float height)
		{
			float num = (0f - height / (endRadius - startRadius)) * startRadius;
			minDotProd = new Vector2(startRadius, 0f - num).normalized.y;
			originOffset = origin + direction * num;
			this.direction = direction;
		}

		public bool InCone(Vector3 testPoint)
		{
			float num = Vector3.Dot(direction, (testPoint - originOffset).normalized);
			return num >= minDotProd;
		}
	}

	private ToolGroup _toolGroup;

	private bool forceSwitching;

	public float touchPadSensitivityY = 1f;

	public float grabForce = 3000f;

	public float counterForce = 300f;

	public float weightRotationCutoff = 8f;

	public float grabCourseCorrection = 8f;

	[HideInInspector]
	public Vector3 grabLocation = Vector3.zero;

	private bool grabbing;

	public manipulatorMode CurrentManipulatorMode;

	public grabbableStatus currentGrabbableStatus = grabbableStatus.noHit;

	public Material vacuumLeashMaterial;

	public Material defaultLeashMaterial;

	public GameObject tipMesh;

	[HideInInspector]
	public Transform t;

	[Space]
	[Header("Swipe Animation")]
	public GameObject controllerGO;

	public AnimationClip transformRightAnim;

	public AnimationClip unTransformRightAnim;

	public AnimationCurve swipeAnimCurve;

	public float swipeAnimationDuration = 2f;

	[Space]
	[Header("VacuumTool")]
	public GameObject blackHoleGO;

	public GameObject vacFillUI;

	public TextMeshPro fillText;

	public GameObject tempHackedBits;

	public Material ControllerLabelMat;

	public MeshRenderer HackedTouchpadTop;

	public MeshRenderer HackedTouchpadBottom;

	public Renderer joyStick;

	private int colId;

	[Space]
	[Header("SFX")]
	public AudioEvent leashOnSFX;

	public AudioEvent leashOffSFX;

	public AudioEvent leashOnSFXair;

	public AudioEvent leashOffSFXair;

	public AudioEvent leashBreakSFX;

	public AudioEvent switchToolsSFX;

	public AudioLoop leashHumSFX;

	public AudioEvent grabFailSFX;

	public AudioEvent defaultButtonPressSFX;

	public AudioEvent loginButtonPressSFX;

	[Space]
	[Header("Union Messages")]
	public GvrAudioSource unionSpeaker;

	[Space]
	[HideInInspector]
	public Bezier grabBezier;

	[HideInInspector]
	public ObjectBlackHole blackHole;

	private Headset grabbedHeadset;

	private float firstClickTime;

	private bool firstClick = true;

	private float oldFill;

	private float roundedLerpingFill;

	private float lerpingFill;

	private float sqrCursorToFace;

	private int gameplayAnimState;

	private float currentAnimState;

	private AnimationClip activeClip;

	private float animTimer;

	private bool animTimerActive;

	private float timerStartAnimState;

	private float timerEndAnimState;

	private EagerTouch.VacInputStatus pTouchStatus;

	private float fillTextTimer;

	private bool showingFillMessage;

	private float fillMessageTimer;

	private float fillMessageDuration;

	private int fillMessageNFlashes;

	private bool grabAnimating;

	private float grabAnimationEndGTD;

	private float grabAnimationStartGTD;

	private float animationLength;

	private float animationProgress;

	private bool canGrab;

	private RaycastHit hitInfo;

	private GameObject selectedGameObject;

	[HideInInspector]
	public float hitPointDistance;

	[HideInInspector]
	public float grabTargetDistance;

	[HideInInspector]
	public ObjectBase hitObjectBase;

	private ObjectBase newHitObjectBase;

	private Vector3 rayDirection = Vector3.one;

	private Vector3 localHitPoint = Vector3.zero;

	private Rigidbody selectedRB;

	private Transform initialObjectTransform;

	[Space]
	public bool useSpherecast;

	private ObjectBase _newHitObjectBase;

	[SerializeField]
	private Vector2 _spherecastRadius;

	[SerializeField]
	private LayerMask _spherecastLayer;

	[SerializeField]
	private float _minDistToUseRay = 1f;

	[SerializeField]
	private float _autoAimRadiusAddition = 0.3f;

	private float _hoverHapticsTimer;

	[SerializeField]
	private float _minDelayBetweenHoverHaptics = 0.5f;

	private RaycastHit[] _hitInfos = new RaycastHit[100];

	[HideInInspector]
	public bool orientDuringGrab;

	[HideInInspector]
	public Quaternion grabbedTargetRotation = Quaternion.identity;

	[HideInInspector]
	public Vector3 heldHighVector = new Vector3(0f, 0f, 0f);

	private float angularVelocity;

	private float moveFilterStrength;

	private Vector3 positionDelta;

	private Quaternion rotationDelta;

	private Vector3 axis;

	private float angle;

	private float velocityLerp;

	private float distanceLerp;

	private float vacSwipeMult = 1f;

	[HideInInspector]
	public RaycastHit[] castResults;

	[HideInInspector]
	public int nResults;

	private const float spherecastRadius = 1f;

	private const float spherecastOffset = 0.5f;

	private const int maxSphereCasts = 36;

	private VVRConsumable glimmerConsumable;

	public ObjectBase HeldObjectBase { get; private set; }

	public ToolGroup toolGroup => _toolGroup;

	private float gtdMax
	{
		get
		{
			if (CurrentManipulatorMode == manipulatorMode.grab)
			{
				return ToolManager.AllManipulators.MaxGrabLength;
			}
			if (CurrentManipulatorMode == manipulatorMode.vacuum)
			{
				return ToolManager.AllVacuums.VacMaxLength;
			}
			return 0f;
		}
	}

	public ToolGroup GrabbingToolGroup => _toolGroup;

	public Transform GrabbingTransform => _toolGroup.CursorReticle.CursorTransform;

	private Vector3 manipulatorGrabLocationTarget => Quaternion.identity * t.forward * grabTargetDistance + t.position;

	private bool isMultiGrabbing => HeldObjectBase != null && HeldObjectBase.grabbedBy.Count > 1;

	public event ToolSwitchEventHandler OnSwitchTool;

	public void Initialize(ToolGroup toolGroup)
	{
		_toolGroup = toolGroup;
	}

	private void Awake()
	{
		if (TBCore.GetActivePlatform() != VRPlatform.OculusPC)
		{
			useSpherecast = false;
		}
		t = transform;
		grabBezier = GetComponent<Bezier>();
		grabBezier.useLineRenderer = true;
		defaultLeashMaterial = grabBezier.GetCurrentMaterial();
		blackHole = blackHoleGO.GetComponent<ObjectBlackHole>();
		if (PersistentDataManager.HasAccountLoaded())
		{
			ToolManager.AllManipulators.HasVacuum = PersistentDataManager.GetVacuumState();
			EagerMaster.instance.vacuumFill = blackHole.vacConsumer.fill;
		}
		PersistentDataManager.onSaveSlotChange += reactToSaveSlotChange;
		castResults = new RaycastHit[36];
		colId = Shader.PropertyToID("_Color");
	}

	public void reactToSaveSlotChange(bool hasValidSaveSlot)
	{
		if (hasValidSaveSlot)
		{
			ToolManager.AllManipulators.HasVacuum = PersistentDataManager.GetVacuumState();
			EagerMaster.instance.vacuumFill = blackHole.vacConsumer.fill;
		}
		else
		{
			ToolManager.AllManipulators.HasVacuum = false;
		}
	}

	private void FixedUpdate()
	{
		if (!grabbing || selectedRB == null)
		{
			return;
		}
		ApplyForceToObject(selectedRB);
		if (grabbedHeadset != null)
		{
			grabbedHeadset.UpdateHeadset(this);
			if (EagerMaster.instance.isSwitchingScenes && grabBezier.activeLine)
			{
				grabBezier.ToggleLine(flag: false);
			}
		}
	}

	private void AppButtonClicked()
	{
		if (firstClick)
		{
			firstClickTime = Time.time;
			firstClick = false;
		}
		else if (Time.time - firstClickTime < 0.3f)
		{
			SwitchTools(CurrentManipulatorMode);
			firstClick = true;
		}
		else
		{
			firstClickTime = Time.time;
		}
	}

	private void UpdateFillText(bool noLerp = false, bool force = false)
	{
		lerpingFill = ((!noLerp) ? Mathf.Lerp(lerpingFill, blackHole.vacConsumer.fill, 10f * Time.deltaTime) : blackHole.vacConsumer.fill);
		roundedLerpingFill = Mathf.Round(lerpingFill * 10f) * 0.1f;
		if (oldFill != roundedLerpingFill || force)
		{
			fillText.text = roundedLerpingFill.ToString("F1") + " TB";
			oldFill = roundedLerpingFill;
		}
	}

	public void SwitchTools(manipulatorMode switchMode)
	{
		_toolGroup.EagerTouch.ClearVelocity();
		if (switchMode != CurrentManipulatorMode)
		{
			return;
		}
		switch (switchMode)
		{
		case manipulatorMode.grab:
			if (grabbing)
			{
				Drop();
			}
			switchToolsSFX.Play(_toolGroup.Controller, rumble: true);
			CurrentManipulatorMode = manipulatorMode.vacuum;
			UpdateFillText(noLerp: true);
			blackHole.gameObject.SetActive(value: true);
			blackHole.transform.position = t.position + t.forward * 0.1f;
			blackHole.transform.rotation = t.rotation;
			Grab(blackHole, blackHole.transform.position, playSound: false);
			_toolGroup.CursorReticle.ToggleOnOff(flag: false);
			AnimateGrabToLength(0.4f, 0.4f, dropObject: false);
			grabBezier.SetMaterial(vacuumLeashMaterial);
			break;
		case manipulatorMode.vacuum:
			switchToolsSFX.Play(_toolGroup.Controller, rumble: true);
			grabbing = false;
			if ((bool)initialObjectTransform)
			{
				Object.Destroy(initialObjectTransform.gameObject);
			}
			if (HeldObjectBase != null)
			{
				HeldObjectBase.GrabRelease(this);
				HeldObjectBase = null;
			}
			selectedRB = null;
			selectedGameObject = null;
			grabBezier.ToggleLine(flag: false);
			CurrentManipulatorMode = manipulatorMode.grab;
			_toolGroup.CursorReticle.gameObject.transform.position = transform.position;
			_toolGroup.CursorReticle.ToggleOnOff(flag: true);
			blackHole.transform.position = new Vector3(1000f, 1000f, 1000f);
			blackHole.gameObject.SetActive(value: false);
			grabBezier.SetMaterial(defaultLeashMaterial);
			break;
		}
	}

	private void Update()
	{
		if (!ToolManager.AllManipulators.SceneCanVacuum && CurrentManipulatorMode == manipulatorMode.vacuum && !forceSwitching)
		{
			GlobalCoroutineContainer.AddThread(SwitchControllerBackOnDelay(direction: true, 0.15f), this);
		}
		if (ToolManager.AllManipulators.HasVacuum)
		{
			CheckForGlimmerObjects();
		}
		bool flag = ToolManager.AllManipulators.HasVacuum && !EagerHeadsetManager.instance.isAddingHeadset && ((CurrentManipulatorMode == manipulatorMode.grab && !grabbing && !TBInput.GetButton(TBInput.Button.Action1, _toolGroup.Controller)) || ((CurrentManipulatorMode == manipulatorMode.vacuum) & !TBInput.GetButton(TBInput.Button.Action1, _toolGroup.Controller)));
		if (flag)
		{
			bool flag2 = CurrentManipulatorMode == manipulatorMode.grab;
			if (_toolGroup.EagerTouch.GetToolSwitchDetected(left: true, CurrentManipulatorMode))
			{
				PlayClip(flag2, swipeRight: false);
				if (!ToolManager.AllManipulators.SceneCanVacuum && !forceSwitching)
				{
					GlobalCoroutineContainer.AddThread(SwitchControllerBackOnDelay(direction: false, 0.35f), this);
					if (OnSwitchTool != null)
					{
						OnSwitchTool();
					}
				}
				SwitchTools(CurrentManipulatorMode);
			}
			else if (_toolGroup.EagerTouch.GetToolSwitchDetected(left: false, CurrentManipulatorMode))
			{
				PlayClip(flag2, swipeRight: true);
				if (!ToolManager.AllManipulators.SceneCanVacuum && !forceSwitching)
				{
					GlobalCoroutineContainer.AddThread(SwitchControllerBackOnDelay(direction: true, 0.35f), this);
					if (OnSwitchTool != null)
					{
						OnSwitchTool();
					}
				}
				SwitchTools(CurrentManipulatorMode);
			}
		}
		UpdateToolSwitchAnimation(flag);
		switch (CurrentManipulatorMode)
		{
		case manipulatorMode.grab:
			GrabGunUpdate();
			break;
		case manipulatorMode.vacuum:
			VacuumUpdate();
			break;
		}
	}

	private void UpdateToolSwitchAnimation(bool canSwitchTools)
	{
		if (animTimerActive)
		{
			animTimer += Time.deltaTime;
			float num = animTimer / swipeAnimationDuration;
			if (num > 1f)
			{
				animTimerActive = false;
				currentAnimState = timerEndAnimState;
			}
			currentAnimState = Mathf.Lerp(timerStartAnimState, timerEndAnimState, swipeAnimCurve.Evaluate(Mathf.Clamp01(num)));
		}
		else if (canSwitchTools && TBInput.GetTouch(TBInput.Button.Action1, _toolGroup.Controller))
		{
			currentAnimState += _toolGroup.EagerTouch.delta.x * 0.175f;
		}
		else
		{
			if (Mathf.Abs(currentAnimState - (float)gameplayAnimState) > 1f)
			{
				currentAnimState -= 2f;
			}
			currentAnimState = Mathf.Lerp(currentAnimState, gameplayAnimState, Time.deltaTime * 9f);
		}
		while (currentAnimState < 0f)
		{
			currentAnimState += 2f;
		}
		currentAnimState %= 2f;
		if (vacFillUI != null && vacFillUI.activeInHierarchy && (currentAnimState < 0.5f || currentAnimState > 1.5f))
		{
			vacFillUI.SetActive(value: false);
		}
		else if (vacFillUI != null && !vacFillUI.activeInHierarchy && currentAnimState >= 0.5f && currentAnimState <= 1.5f)
		{
			vacFillUI.SetActive(value: true);
		}
		if (transformRightAnim != null && unTransformRightAnim != null && controllerGO != null)
		{
			activeClip = ((!(currentAnimState < 1f)) ? unTransformRightAnim : transformRightAnim);
			activeClip.SampleAnimation(controllerGO, currentAnimState % 1f * activeClip.length);
		}
	}

	private void PlayClip(bool transform, bool swipeRight)
	{
		gameplayAnimState = (transform ? 1 : 0);
		timerEndAnimState = gameplayAnimState;
		timerStartAnimState = currentAnimState;
		if (swipeRight && timerEndAnimState < timerStartAnimState)
		{
			timerEndAnimState += 2f;
		}
		else if (!swipeRight && timerEndAnimState > timerStartAnimState)
		{
			timerEndAnimState -= 2f;
		}
		animTimer = 0f;
		animTimerActive = true;
	}

	private void VacuumUpdate()
	{
		SwipeY();
		if (grabAnimating)
		{
			AnimateGrab();
		}
		Grabbing();
		if (!showingFillMessage)
		{
			fillTextTimer += Time.deltaTime;
			if (fillTextTimer > 0.05f)
			{
				fillTextTimer = 0f;
				UpdateFillText();
			}
			UpdateFillUIColors();
		}
		else
		{
			UpdateFillUIMessage();
		}
		UpdateControllerButtons();
		EagerMaster.instance.vacuumFill = blackHole.vacConsumer.fill;
	}

	private void UpdateFillUIColors()
	{
		if (pTouchStatus != _toolGroup.EagerTouch.CurrentVacInputStatus())
		{
			pTouchStatus = _toolGroup.EagerTouch.CurrentVacInputStatus();
			switch (_toolGroup.EagerTouch.CurrentVacInputStatus())
			{
			case EagerTouch.VacInputStatus.PreviewIn:
			case EagerTouch.VacInputStatus.In:
				SetFillUIColor(_toolGroup.VLManager.label.downColor);
				break;
			case EagerTouch.VacInputStatus.previewOut:
			case EagerTouch.VacInputStatus.Out:
				SetFillUIColor(_toolGroup.VLManager.label.upColor);
				break;
			default:
				SetFillUIColor(_toolGroup.VLManager.label.neutralUpColor);
				break;
			}
		}
	}

	private void UpdateControllerButtons()
	{
		float num = 0f;
		switch (_toolGroup.EagerTouch.CurrentVacInputStatus())
		{
		case EagerTouch.VacInputStatus.PreviewIn:
		case EagerTouch.VacInputStatus.In:
			num = 16f;
			break;
		case EagerTouch.VacInputStatus.previewOut:
		case EagerTouch.VacInputStatus.Out:
			num = -16f;
			break;
		}
		float x = -16f + num;
		float x2 = 16f + num;
		if (TBCore.GetActivePlatform() != VRPlatform.OculusPC)
		{
			Quaternion b = Quaternion.Euler(new Vector3(x, HackedTouchpadBottom.transform.localEulerAngles.y, HackedTouchpadBottom.transform.localEulerAngles.z));
			Quaternion b2 = Quaternion.Euler(new Vector3(x2, HackedTouchpadTop.transform.localEulerAngles.y, HackedTouchpadTop.transform.localEulerAngles.z));
			HackedTouchpadBottom.transform.localRotation = Quaternion.Slerp(HackedTouchpadBottom.transform.localRotation, b, Time.deltaTime * 20f);
			HackedTouchpadTop.transform.localRotation = Quaternion.Slerp(HackedTouchpadTop.transform.localRotation, b2, Time.deltaTime * 20f);
		}
	}

	private void SetFillUIColor(Color color)
	{
		ControllerLabelMat.SetColor(colId, color);
	}

	public void TriggerFillUIMessage(string message, float duration, int nFlashes)
	{
		showingFillMessage = true;
		fillMessageTimer = 0f;
		fillMessageDuration = duration;
		fillMessageNFlashes = nFlashes;
		fillText.text = message;
	}

	private void UpdateFillUIMessage()
	{
		fillMessageTimer += Time.deltaTime;
		showingFillMessage = fillMessageTimer < fillMessageDuration;
		if (!showingFillMessage)
		{
			UpdateFillText(noLerp: false, force: true);
			UpdateFillUIColors();
		}
		else
		{
			float num = Mathf.Clamp01(fillMessageTimer / fillMessageDuration);
			int num2 = (int)(num * (float)fillMessageNFlashes * 2f) % 2;
			SetFillUIColor((num2 != 0) ? _toolGroup.VLManager.label.neutralUpColor : _toolGroup.VLManager.label.upColor);
		}
	}

	private void GrabGunUpdate()
	{
		if (ToolManager.AllManipulators.UserCanScroll)
		{
			SwipeY();
		}
		if (grabAnimating)
		{
			AnimateGrab();
		}
		else if (TBInput.GetButtonDown(TBInput.Button.Action1, _toolGroup.Controller))
		{
			GrabClickDown();
		}
		if (grabbing)
		{
			Grabbing();
		}
		else if (!grabAnimating && _toolGroup.CurrentTool != ToolGroup.Tool.Teleporter && EagerMaster.instance.shouldRaycast)
		{
			RayCastCheck();
		}
	}

	private void GrabClickDown()
	{
		if (!ToolManager.AllManipulators.UserCanGrab)
		{
			return;
		}
		if ((bool)hitObjectBase && hitObjectBase.isButton && !hitObjectBase.ungrabbable)
		{
			Button(hitObjectBase);
		}
		else if (canGrab && !grabbing)
		{
			Grab(hitObjectBase, grabLocation);
		}
		else if (grabbing)
		{
			if (ToolManager.AllManipulators.UserCanDrop && !HeldObjectBase.undroppable)
			{
				Drop();
			}
			else if (HeldObjectBase.undroppable && HeldObjectBase.grabbedBy.Count > 1 && !HeldObjectBase.gameObject.GetComponent<ElevatorEmergencyPhone>())
			{
				Drop();
			}
		}
		else if (currentGrabbableStatus == grabbableStatus.neutral && !HeadsetRemovalUIManager.instance.PointingAtHead(_toolGroup.Controller))
		{
			grabFailSFX.Play(_toolGroup.Controller, rumble: true);
		}
	}

	public void AnimateGrabToLength(float _newGrabTargetDistance, float _speed, bool dropObject, float maxTime = 999f)
	{
		grabAnimating = true;
		if (dropObject && grabbing)
		{
			Drop();
		}
		grabAnimationStartGTD = grabTargetDistance;
		grabAnimationEndGTD = ((!(_newGrabTargetDistance < gtdMax)) ? gtdMax : _newGrabTargetDistance);
		animationLength = Mathf.Clamp(_speed * Mathf.Abs(_newGrabTargetDistance - grabTargetDistance), 0f, maxTime);
		animationProgress = 0f;
	}

	private void AnimateGrab()
	{
		animationProgress += Time.deltaTime;
		float num = animationProgress / animationLength;
		if (num > 0.99f)
		{
			num = 1f;
			grabAnimating = false;
		}
		grabTargetDistance = Mathf.Lerp(grabAnimationStartGTD, grabAnimationEndGTD, num);
	}

	private float CastRadiusFor(float localRadius)
	{
		return localRadius * transform.lossyScale.x;
	}

	private bool RoundConeCast(Vector3 origin, float startRadius, float endRadius, Vector3 direction, out RaycastHit hitInfo, float maxDist, int layerMask)
	{
		float num = CastRadiusFor(Mathf.Max(startRadius, endRadius) + _autoAimRadiusAddition);
		Vector3 origin2 = origin + direction * num;
		float maxDistance = maxDist - num;
		int num2 = Physics.SphereCastNonAlloc(origin2, num, direction, _hitInfos, maxDistance, layerMask, QueryTriggerInteraction.Ignore);
		ChoppedCone choppedCone = new ChoppedCone(origin, direction, startRadius, endRadius, maxDist);
		ChoppedCone choppedCone2 = new ChoppedCone(origin, direction, startRadius + _autoAimRadiusAddition, endRadius + _autoAimRadiusAddition, maxDist);
		hitInfo = default;
		float num3 = float.PositiveInfinity;
		float num4 = -1f;
		ObjectBase objectBase = null;
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < num2; i++)
		{
			ObjectBase componentInParent = _hitInfos[i].transform.GetComponentInParent<ObjectBase>();
			if (componentInParent != null)
			{
				float num5 = _hitInfos[i].distance;
				if ((bool)_hitInfos[i].transform.gameObject.GetComponent<Headset>())
				{
					num5--;
				}
				if (((!(componentInParent == _newHitObjectBase)) ? choppedCone.InCone(_hitInfos[i].point) : choppedCone2.InCone(_hitInfos[i].point)) && num5 < num3)
				{
					num3 = num5;
					hitInfo = _hitInfos[i];
					objectBase = componentInParent;
					vector = (_hitInfos[i].point - origin).normalized;
					num4 = Vector3.Dot(vector, direction);
				}
			}
		}
		for (int j = 0; j < num2; j++)
		{
			ObjectBase componentInParent2 = _hitInfos[j].transform.GetComponentInParent<ObjectBase>();
			if (componentInParent2 == objectBase)
			{
				Vector3 normalized = (_hitInfos[j].point - origin).normalized;
				float num6 = Vector3.Dot(normalized, direction);
				if (num6 > num4)
				{
					num4 = num6;
					vector = normalized;
					hitInfo = _hitInfos[j];
				}
			}
		}
		if (objectBase != null)
		{
			hitInfo = BinarySearchRaycast(hitInfo.collider, origin, maxDist, 8, vector, direction, 0f, 1f, hitInfo);
		}
		return num3 < float.PositiveInfinity;
	}

	private RaycastHit BinarySearchRaycast(Collider collider, Vector3 origin, float maxDist, int raycastBudget, Vector3 dirA, Vector3 dirB, float start, float end, RaycastHit bestHitSoFar)
	{
		if (raycastBudget <= 0)
		{
			return bestHitSoFar;
		}
		float num = (start + end) / 2f;
		Vector3 normalized = Vector3.Lerp(dirA, dirB, num).normalized;
		if (Mathf.Approximately(normalized.sqrMagnitude, 0f))
		{
			return bestHitSoFar;
		}
		if (collider.Raycast(new Ray(origin, normalized), out var bestHitSoFar2, maxDist))
		{
			return BinarySearchRaycast(collider, origin, maxDist, raycastBudget - 1, dirA, dirB, num, end, bestHitSoFar2);
		}
		return BinarySearchRaycast(collider, origin, maxDist, raycastBudget - 1, dirA, dirB, start, num, bestHitSoFar);
	}

	private bool FinalValidationCheck(Vector3 origin, Vector3 hitPoint, float maxDist, int layerMask, ObjectBase desiredObjectBase, out RaycastHit hitInfo)
	{
		if (Physics.Raycast(origin, hitPoint - origin, out hitInfo, maxDist, layerMask, QueryTriggerInteraction.Ignore))
		{
			ObjectBase componentInParent = hitInfo.transform.GetComponentInParent<ObjectBase>();
			if (desiredObjectBase == componentInParent)
			{
				return true;
			}
			return false;
		}
		return true;
	}

	private bool CursorCast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDist, int layerMask)
	{
		if (useSpherecast)
		{
			bool flag = Physics.Raycast(origin, direction, out hitInfo, maxDist, layerMask, QueryTriggerInteraction.Ignore);
			bool flag2 = RoundConeCast(origin, _spherecastRadius[0], _spherecastRadius[1], direction, out var raycastHit, maxDist, _spherecastLayer);
			Vector3 vector = origin + direction * CastRadiusFor(_spherecastRadius[0]);
			ObjectBase objectBase = ((!flag) ? null : hitInfo.transform.GetComponentInParent<ObjectBase>());
			ObjectBase objectBase2 = ((!flag2) ? null : raycastHit.transform.GetComponentInParent<ObjectBase>());
			if ((bool)objectBase && (bool)objectBase2)
			{
				float magnitude = (raycastHit.point - origin).magnitude;
				float distance = hitInfo.distance;
				if (!(distance - magnitude < _minDistToUseRay))
				{
					if (!FinalValidationCheck(origin, raycastHit.point, maxDist, layerMask, objectBase2, out var _))
					{
						return flag;
					}
					hitInfo = raycastHit;
				}
				return true;
			}
			if (!objectBase && (bool)objectBase2)
			{
				if (FinalValidationCheck(origin, raycastHit.point, maxDist, layerMask, objectBase2, out var _))
				{
					hitInfo = raycastHit;
					return true;
				}
				return flag;
			}
			return flag;
		}
		return Physics.Raycast(origin, direction, out hitInfo, maxDist, layerMask, QueryTriggerInteraction.Ignore);
	}

	private void RayCastCheck()
	{
		if (!_toolGroup.CursorReticle.rendering)
		{
			_toolGroup.CursorReticle.ToggleOnOff(flag: true);
		}
		rayDirection = EagerMaster.instance.player.transform.rotation * (TBInput.GetPointerRotation(_toolGroup.Controller) * Vector3.forward);
		if (CursorCast(_toolGroup.EagerTouch.GetRayCastPosition(), rayDirection, out hitInfo, ToolManager.AllManipulators.MaxGrabLength, EagerMaster.instance.leashRayMask))
		{
			grabLocation = hitInfo.point;
			hitPointDistance = Vector3.Distance(hitInfo.point, t.position);
			newHitObjectBase = hitInfo.transform.GetComponentInChildren<ObjectBase>();
			if ((bool)newHitObjectBase)
			{
				newHitObjectBase = newHitObjectBase.GetObjectBaseForLeash();
				canGrab = ((hitPointDistance <= GrabbableRange() && !newHitObjectBase.ungrabbable) ? true : false);
				localHitPoint = newHitObjectBase.transform.InverseTransformPoint(hitInfo.point);
				if (canGrab && ToolManager.AllManipulators.UserCanGrab)
				{
					currentGrabbableStatus = ((newHitObjectBase.isButton && !newHitObjectBase.ungrabbable) ? grabbableStatus.button : grabbableStatus.grabbable);
				}
				else
				{
					currentGrabbableStatus = ((newHitObjectBase.isButton && !newHitObjectBase.ungrabbable) ? grabbableStatus.button : grabbableStatus.neutral);
				}
			}
			else
			{
				newHitObjectBase = null;
				canGrab = false;
				currentGrabbableStatus = grabbableStatus.neutral;
			}
		}
		else
		{
			newHitObjectBase = null;
			canGrab = false;
			currentGrabbableStatus = grabbableStatus.noHit;
			grabLocation = rayDirection * ToolManager.AllManipulators.MaxGrabLength + EagerMaster.instance.mainCamera.transform.position;
		}
		hitPointDistance = Vector3.Distance(hitInfo.point, t.position);
		t.rotation = Quaternion.Lerp(t.rotation, Quaternion.LookRotation(grabLocation - t.position, EagerMaster.instance.player.transform.rotation * TBInput.GetPointerRotation(_toolGroup.Controller) * Vector3.up), 10f * Time.deltaTime);
		_hoverHapticsTimer = Mathf.MoveTowards(_hoverHapticsTimer, 0f, Time.deltaTime);
		if (hitObjectBase == newHitObjectBase)
		{
			if ((bool)hitObjectBase)
			{
				hitObjectBase.OnHoverStay(this);
			}
			return;
		}
		if ((bool)hitObjectBase)
		{
			hitObjectBase.OnHoverExit();
		}
		if ((bool)newHitObjectBase)
		{
			newHitObjectBase.OnHoverEnter();
			if (_hoverHapticsTimer <= 0f)
			{
				TBInput.SetRumble(_toolGroup.Controller, 0.2f);
				_hoverHapticsTimer = _minDelayBetweenHoverHaptics;
			}
		}
		hitObjectBase = newHitObjectBase;
	}

	public void Button(ObjectBase grabbedObjectBase)
	{
		grabbedObjectBase.OnButtonPress();
		if (!ToolManager.AllCursorReticles.HideCursor)
		{
			_toolGroup.CursorReticle.CursorTransform.localScale = _toolGroup.CursorReticle.grabScale;
		}
		if (grabbedObjectBase is ObjectButton)
		{
			switch (((ObjectButton)grabbedObjectBase).buttonSFX)
			{
			case ButtonPressSFXType.loginMenu:
				loginButtonPressSFX.Play(_toolGroup.Controller, rumble: true);
				break;
			case ButtonPressSFXType.normal:
				defaultButtonPressSFX.Play(_toolGroup.Controller, rumble: true);
				break;
			case ButtonPressSFXType.none:
				break;
			default:
				defaultButtonPressSFX.Play(_toolGroup.Controller, rumble: true);
				break;
			}
		}
		else
		{
			defaultButtonPressSFX.Play(_toolGroup.Controller, rumble: true);
		}
	}

	public void Grab(ObjectBase grabbedObjectBase, Vector3 grabPoint, bool playSound = true)
	{
		if ((bool)grabbedObjectBase.rb)
		{
			grabbing = true;
			canGrab = false;
			grabTargetDistance = Vector3.Distance(grabPoint, t.position);
			if (playSound)
			{
				TBInput.StartRumblePulse(_toolGroup.Controller, 0.9f, 0.05f);
				leashHumSFX.Play(_toolGroup.Controller);
				leashOnSFX.Play(_toolGroup.Controller, rumble: true);
				leashOnSFXair.Play(_toolGroup.Controller);
			}
			grabBezier.ToggleLine(flag: true);
			HeldObjectBase = grabbedObjectBase.GetObjectBaseForLeash();
			selectedGameObject = HeldObjectBase.gameObject;
			selectedRB = HeldObjectBase.rb;
			grabbedHeadset = selectedGameObject.GetComponent<Headset>();
			if (grabTargetDistance > gtdMax)
			{
				grabLocation = rayDirection * gtdMax + t.position;
				grabTargetDistance = gtdMax;
			}
			else
			{
				grabLocation = grabPoint;
			}
			localHitPoint = HeldObjectBase.transform.InverseTransformPoint(grabPoint);
			initialObjectTransform = new GameObject().transform;
			initialObjectTransform.name = "initialPickupRotation";
			initialObjectTransform.parent = t;
			initialObjectTransform.position = t.position;
			initialObjectTransform.rotation = selectedGameObject.transform.rotation;
			selectedRB.maxAngularVelocity = float.MaxValue;
			if ((bool)hitObjectBase)
			{
				hitObjectBase.OnHoverExit();
				hitObjectBase = null;
			}
			orientDuringGrab = false;
			grabbedTargetRotation = Quaternion.identity;
			heldHighVector = localHitPoint;
			HeldObjectBase.GrabWith(this);
		}
	}

	private void Grabbing()
	{
		if (HeldObjectBase == null || !HeldObjectBase.gameObject.activeInHierarchy)
		{
			Drop();
			return;
		}
		bool flag = HeldObjectBase.grabbedBy.Count == 1;
		if (orientDuringGrab && flag)
		{
			initialObjectTransform.localRotation = Quaternion.Lerp(initialObjectTransform.localRotation, grabbedTargetRotation, 0.8f * Time.deltaTime);
			localHitPoint = Vector3.Lerp(localHitPoint, heldHighVector, 0.8f * Time.deltaTime);
		}
		angularVelocity = TBInput.GetAngularVelocity(_toolGroup.Controller).magnitude;
		moveFilterStrength = Mathf.Clamp((angularVelocity - 0.2f) / 15f, 0f, 0.1f);
		t.rotation = Quaternion.Lerp(t.rotation, EagerMaster.instance.player.transform.rotation * TBInput.GetPointerRotation(_toolGroup.Controller), moveFilterStrength);
		grabLocation = selectedRB.transform.TransformPoint(localHitPoint);
		grabBezier.DrawBezier(tipMesh.transform.position, t.forward * grabTargetDistance + t.position, grabLocation, 30);
		if ((bool)HeldObjectBase)
		{
			HeldObjectBase.GrabUpdate(this);
		}
	}

	public void Break()
	{
		Drop(playSFX: false);
		leashBreakSFX.Play(_toolGroup.Controller, rumble: true);
	}

	public void Drop(bool playSFX = true)
	{
		if (CurrentManipulatorMode == manipulatorMode.vacuum)
		{
			return;
		}
		grabbing = false;
		if (HeldObjectBase != null)
		{
			HeldObjectBase.GrabRelease(this);
			if (playSFX)
			{
				TBInput.StartRumblePulse(_toolGroup.Controller, 0.7f, 0.02f);
				leashOffSFX.Play(_toolGroup.Controller, rumble: true);
				leashOffSFXair.Play(_toolGroup.Controller);
			}
		}
		if (grabbedHeadset != null)
		{
			grabbedHeadset.headsetHasBeenSafelyRemoved = true;
			grabbedHeadset = null;
		}
		if ((bool)initialObjectTransform)
		{
			Object.Destroy(initialObjectTransform.gameObject);
		}
		leashHumSFX.Stop(_toolGroup.Controller);
		HeldObjectBase = null;
		selectedRB = null;
		selectedGameObject = null;
		grabBezier.ToggleLine(flag: false);
	}

	public bool isGrabbing()
	{
		return grabbing;
	}

	public float GrabbableRange()
	{
		return gtdMax;
	}

	private void ApplyForceToObject(Rigidbody selectedRB)
	{
		ApplyForceToObject(selectedRB, updateFieldsOnly: false);
	}

	private void ApplyForceToObject(Rigidbody selectedRB, bool updateFieldsOnly)
	{
		grabLocation = selectedRB.transform.TransformPoint(localHitPoint);
		rotationDelta = initialObjectTransform.rotation * Quaternion.Inverse(selectedRB.transform.rotation);
		positionDelta = manipulatorGrabLocationTarget - grabLocation;
		hitPointDistance = Vector3.Distance(grabLocation, t.position);
		distanceLerp = 1f - Mathf.Clamp01(hitPointDistance * hitPointDistance / 25f);
		positionDelta -= selectedRB.velocity * Time.deltaTime * grabCourseCorrection * Mathf.Clamp01(hitPointDistance * 0.05f);
		if (updateFieldsOnly)
		{
			return;
		}
		if (selectedRB.mass > weightRotationCutoff)
		{
			selectedRB.AddForceAtPosition(positionDelta * grabForce * (1f + distanceLerp) * Time.deltaTime, grabLocation);
			selectedRB.AddForceAtPosition(-selectedRB.velocity * 2f * (counterForce / (positionDelta.magnitude + 0.1f)) * Time.deltaTime, grabLocation, ForceMode.Acceleration);
			if (selectedRB.constraints == RigidbodyConstraints.None)
			{
				float num = Quaternion.Angle(initialObjectTransform.rotation, selectedRB.rotation) / 180f;
				rotationDelta.ToAngleAxis(out angle, out axis);
				if (angle > 180f)
				{
					angle -= 360f;
				}
				if (angle != 0f)
				{
					Vector3 vector = angle * axis;
					selectedRB.AddTorque(vector * Time.deltaTime * 10f * num, ForceMode.Force);
				}
			}
			return;
		}
		bool flag = HeldObjectBase.grabbedBy.Count == 1;
		bool flag2 = false;
		if (HeldObjectBase.grabbedBy.Count > 0)
		{
			flag2 = !flag && HeldObjectBase.grabbedBy[0] == this;
		}
		if (flag2)
		{
			MainManipulatorLightObjectKabschUpdate(selectedRB, out var correctingPositionDelta, out var correctingRotationDelta);
			float num2 = 0f;
			foreach (Manipulator item in HeldObjectBase.grabbedBy)
			{
				num2 += item.distanceLerp;
			}
			num2 /= (float)HeldObjectBase.grabbedBy.Count;
			ApplyKabschLightForce(selectedRB, correctingPositionDelta, correctingRotationDelta, num2);
		}
		else
		{
			if (!flag)
			{
				return;
			}
			selectedRB.AddForce(positionDelta * grabForce * (1f + distanceLerp) * Time.deltaTime);
			selectedRB.AddForce(-selectedRB.velocity * (counterForce / (positionDelta.magnitude + 0.1f)) * Time.deltaTime, ForceMode.Acceleration);
			if (selectedRB.useGravity)
			{
				selectedRB.AddForce(-Physics.gravity * distanceLerp * (1f - Mathf.Clamp01((selectedRB.mass - 2f) / (weightRotationCutoff - 2f))), ForceMode.Acceleration);
			}
			velocityLerp = 1f - Mathf.Clamp01(hitPointDistance / 1.2f);
			if (selectedRB.mass < weightRotationCutoff / 2f && hitPointDistance < 1.2f)
			{
				selectedRB.velocity = Vector3.Lerp(selectedRB.velocity, positionDelta * 8f / (Vector3.Distance(t.position, grabLocation) + 0.5f), Mathf.Pow(velocityLerp, 4f));
			}
			if (selectedRB.constraints == RigidbodyConstraints.None)
			{
				rotationDelta.ToAngleAxis(out angle, out axis);
				if (angle > 180f)
				{
					angle -= 360f;
				}
				if (angle != 0f)
				{
					Vector3 target = angle * axis;
					selectedRB.angularVelocity = Vector3.MoveTowards(selectedRB.angularVelocity, target, 11f - selectedRB.mass * 1f);
				}
			}
		}
	}

	private void ApplyKabschLightForce(Rigidbody selectedRB, Vector3 coorectingPositionDelta, Quaternion correctionRotationDelta, float avDistanceLerp)
	{
		int num = 2;
		selectedRB.AddForce(coorectingPositionDelta * num * grabForce * (1f + avDistanceLerp) * Time.deltaTime);
		selectedRB.AddForce(-selectedRB.velocity * (counterForce / (coorectingPositionDelta.magnitude + 0.1f)) * Time.deltaTime, ForceMode.Acceleration);
		if (selectedRB.useGravity)
		{
			selectedRB.AddForce(-Physics.gravity * avDistanceLerp * (1f - Mathf.Clamp01((selectedRB.mass - 2f) / (weightRotationCutoff - 2f))), ForceMode.Acceleration);
		}
		if (selectedRB.constraints == RigidbodyConstraints.None)
		{
			correctionRotationDelta.ToAngleAxis(out var num2, out var vector);
			if (num2 > 180f)
			{
				num2 -= 360f;
			}
			if (num2 != 0f)
			{
				Vector3 target = num2 * vector;
				selectedRB.angularVelocity = Vector3.MoveTowards(selectedRB.angularVelocity, target, 11f - selectedRB.mass * 1f);
			}
		}
	}

	private void MainManipulatorLightObjectKabschUpdate(Rigidbody selectedRB, out Vector3 correctingPositionDelta, out Quaternion correctingRotationDelta)
	{
		foreach (Manipulator item in HeldObjectBase.grabbedBy)
		{
			item.ApplyForceToObject(selectedRB, updateFieldsOnly: true);
		}
		int count = HeldObjectBase.grabbedBy.Count;
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		Vector3 zero3 = Vector3.zero;
		foreach (Manipulator item2 in HeldObjectBase.grabbedBy)
		{
			Vector3 vector = item2.grabLocation;
			Transform transform = item2.transform;
			zero += item2.manipulatorGrabLocationTarget / count;
			zero2 += vector / count;
			zero3 += item2.manipulatorGrabLocationTarget - vector;
		}
		correctingPositionDelta = zero3 / count;
		int num = 6;
		float num2 = 0.05f;
		Vector3[] array = new Vector3[count * num];
		Vector3[] array2 = new Vector3[count * num];
		for (int i = 0; i < HeldObjectBase.grabbedBy.Count; i++)
		{
			Manipulator manipulator = HeldObjectBase.grabbedBy[i];
			Quaternion quaternion = HeldObjectBase.transform.rotation * Quaternion.Inverse(manipulator.initialObjectTransform.localRotation);
			Vector3 vector2 = manipulator.t.rotation * Vector3.right * num2;
			Vector3 vector3 = quaternion * Vector3.right * num2;
			Vector3 vector4 = manipulator.t.rotation * Vector3.up * num2;
			Vector3 vector5 = quaternion * Vector3.up * num2;
			Vector3 vector6 = manipulator.t.rotation * Vector3.forward * num2;
			Vector3 vector7 = quaternion * Vector3.forward * num2;
			ref Vector3 reference = ref array[num * i];
			reference = manipulator.manipulatorGrabLocationTarget + vector2;
			ref Vector3 reference2 = ref array[num * i + 1];
			reference2 = manipulator.manipulatorGrabLocationTarget - vector2;
			ref Vector3 reference3 = ref array[num * i + 2];
			reference3 = manipulator.manipulatorGrabLocationTarget + vector4;
			ref Vector3 reference4 = ref array[num * i + 3];
			reference4 = manipulator.manipulatorGrabLocationTarget - vector4;
			ref Vector3 reference5 = ref array[num * i + 4];
			reference5 = manipulator.manipulatorGrabLocationTarget + vector6;
			ref Vector3 reference6 = ref array[num * i + 5];
			reference6 = manipulator.manipulatorGrabLocationTarget - vector6;
			ref Vector3 reference7 = ref array2[num * i];
			reference7 = manipulator.grabLocation + vector3;
			ref Vector3 reference8 = ref array2[num * i + 1];
			reference8 = manipulator.grabLocation - vector3;
			ref Vector3 reference9 = ref array2[num * i + 2];
			reference9 = manipulator.grabLocation + vector5;
			ref Vector3 reference10 = ref array2[num * i + 3];
			reference10 = manipulator.grabLocation - vector5;
			ref Vector3 reference11 = ref array2[num * i + 4];
			reference11 = manipulator.grabLocation + vector7;
			ref Vector3 reference12 = ref array2[num * i + 5];
			reference12 = manipulator.grabLocation - vector7;
		}
		KabschSolver kabschSolver = new KabschSolver();
		Matrix4x4 matrix4x = kabschSolver.SolveKabsch(array, array2);
		Quaternion identity = Quaternion.identity;
		identity = Quaternion.Inverse(matrix4x.rotation);
		if (identity != Quaternion.identity)
		{
		}
		correctingRotationDelta = identity;
	}

	private void SwipeY()
	{
		vacSwipeMult = ((CurrentManipulatorMode != manipulatorMode.vacuum) ? 1f : 1.7f);
		if (TBInput.GetTouch(TBInput.Button.Action1, _toolGroup.Controller))
		{
			grabTargetDistance = Mathf.Clamp(grabTargetDistance + _toolGroup.EagerTouch.LeashLengthVelocity().y * Time.deltaTime * touchPadSensitivityY * 1.5f * vacSwipeMult, 0.15f, gtdMax);
		}
		else
		{
			grabTargetDistance = Mathf.Clamp(grabTargetDistance + _toolGroup.EagerTouch.LeashLengthVelocity().y * Time.deltaTime * touchPadSensitivityY * vacSwipeMult, 0.15f, gtdMax);
		}
		if (grabTargetDistance > hitPointDistance + 0.25f && !isMultiGrabbing)
		{
			grabTargetDistance = Mathf.Clamp(Mathf.Lerp(grabTargetDistance, hitPointDistance, 2.5f * Time.deltaTime), 0.15f, gtdMax);
		}
	}

	private IEnumerator SwitchControllerBackOnDelay(bool direction, float time)
	{
		forceSwitching = true;
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(time, this);
		if (CurrentManipulatorMode != manipulatorMode.grab)
		{
			bool flag = CurrentManipulatorMode == manipulatorMode.grab;
			PlayClip(flag, direction);
			SwitchTools(manipulatorMode.vacuum);
		}
		forceSwitching = false;
	}

	public void CheckForGlimmerObjects()
	{
		Ray ray = new Ray(transform.position + transform.forward * 0.5f, transform.forward);
		LayerMask layerMask = EagerMaster.instance.fillableOnlyMask;
		switch (_toolGroup.EagerTouch.CurrentVacInputStatus())
		{
		case EagerTouch.VacInputStatus.PreviewIn:
		case EagerTouch.VacInputStatus.In:
			layerMask = EagerMaster.instance.edibleOnlyMask;
			break;
		case EagerTouch.VacInputStatus.previewOut:
		case EagerTouch.VacInputStatus.Off:
		case EagerTouch.VacInputStatus.Out:
			layerMask = EagerMaster.instance.fillableOnlyMask;
			break;
		}
		nResults = Physics.SphereCastNonAlloc(ray, 1f, castResults, 120f, layerMask);
		for (int i = 0; i < nResults; i++)
		{
			glimmerConsumable = castResults[i].collider.GetComponentInParent<VVRConsumable>();
			if (!(glimmerConsumable == null) && (glimmerConsumable.currentObjectState == VVRConsumable.objectState.fullyConsumedGlimmer || glimmerConsumable.currentObjectState == VVRConsumable.objectState.fullyConsumedStatic))
			{
				glimmerConsumable.MarkInGlimmerRange();
			}
		}
	}
}
