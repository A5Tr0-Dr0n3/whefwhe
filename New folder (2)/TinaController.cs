using System;
using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class TinaController : MonoBehaviour, IJointBreakSubscriber
{
	[Serializable]
	public struct Arc
	{
		public float Radius;

		public Vector2 AngleRangeDeg;

		public float CurrAngleOnArc;
	}

	[Serializable]
	public struct Claw
	{
		public Transform Root;

		public Vector3 LocalHingeAxis;

		private FixedJoint FixedJoint;

		public ObjectBase Held;

		public Transform heldObjectRoot;

		private Transform heldObjectParent;

		private Vector3 heldObjectScale;

		public CCDBone[] LeftSkeleton;

		public float LeftClosedFraction;

		public float RightClosedFraction;

		public CCDBone[] RightSkeleton;

		internal Collider[] Colliders;

		[SerializeField]
		private Dollhouse DollHouse;

		[Tooltip("Keep this perpendicular to the grab transform")]
		public Vector3 ReferenceAxis;

		public static void Init(ref Claw claw, IJointBreakSubscriber jointBreakSubscriber)
		{
			claw.FixedJoint = claw.heldObjectRoot.gameObject.GetComponent<FixedJoint>();
			claw.Root.gameObject.AddComponent<JointBreakListener>().Subscriber = jointBreakSubscriber;
			claw.Colliders = claw.Root.GetComponentsInChildren<Collider>();
		}

		public void PickupImmediate(ObjectBase obj, bool forceToMouth = false)
		{
			if (forceToMouth)
			{
				heldObjectParent = obj.transform.parent;
				heldObjectScale = obj.transform.localScale;
				Held = obj;
				Held.ForceDropByPlayer();
				Held.transform.SetParent(heldObjectRoot, worldPositionStays: false);
				Held.transform.localPosition = Vector3.zero;
				Held.rb.velocity = Vector3.zero;
				FixedJoint.connectedBody = obj.rb;
			}
			else
			{
				heldObjectParent = obj.transform.parent;
				heldObjectScale = obj.transform.localScale;
				Held = obj;
				Held.ForceDropByPlayer();
				FixedJoint.connectedBody = obj.rb;
				Held.transform.SetParent(heldObjectRoot);
			}
		}

		public void TryDropImmediate()
		{
			if (Held != null)
			{
				FixedJoint.connectedBody = null;
				Held.rb.WakeUp();
				SetIgnoreCollisionsWith(Held, ignore: false);
				Held.rb.velocity = Vector3.zero;
				Held.transform.SetParent(heldObjectParent, worldPositionStays: true);
				Held.transform.localScale = heldObjectScale;
				if ((bool)Held.GetComponent<DollhouseObject>() && !Held.GetComponent<DollhouseDoll>())
				{
					Held.GetComponent<DollhouseObject>().SetStateOnDelay(DollhouseObjectState.gettingUp, 1.5f);
				}
				else if ((bool)Held.GetComponent<DollhouseDoll>() && !Held.GetComponent<DollhouseDoll>().isTinaDoll)
				{
					Held.GetComponent<DollhouseDoll>().SetStateOnDelay(DollhouseObjectState.gettingUp, 1.5f);
				}
				Held = null;
			}
		}

		public void ForceDropImmediate(Transform place)
		{
			if (Held != null)
			{
				FixedJoint.connectedBody = null;
				Held.rb.WakeUp();
				SetIgnoreCollisionsWith(Held, ignore: false);
				Held.rb.velocity = Vector3.zero;
				Held.transform.SetParent(heldObjectParent, worldPositionStays: true);
				Held.transform.localScale = heldObjectScale;
				Held.transform.position = place.transform.position;
				Held.transform.rotation = place.transform.rotation;
				Held = null;
			}
		}

		public void SetIgnoreCollisionsWith(ObjectBase obj, bool ignore)
		{
			for (int i = 0; i < obj.colliders.Length; i++)
			{
				for (int j = 0; j < Colliders.Length; j++)
				{
					Physics.IgnoreCollision(Colliders[j], obj.colliders[i], ignore);
				}
				SetColliderTrigger(obj.colliders[i], ignore);
				SetUseGravity(obj, ignore);
			}
		}

		public void SetColliderTrigger(Collider col, bool s)
		{
			col.isTrigger = s;
		}

		public void SetUseGravity(ObjectBase obj, bool s)
		{
			obj.rb.useGravity = !s;
		}

		public void SetKinematic(ObjectBase obj, bool s)
		{
			obj.rb.isKinematic = s;
		}
	}

	[Serializable]
	public struct CCDBone
	{
		public Vector3 LocalHingeAxis;

		public Vector3 LimitReferenceAxis;

		public Vector2 AngleLimitsDeg;

		public Transform Root;

		public Transform Tip;

		[Tooltip("We don't want all bones to try to point to target!")]
		public bool AimForPerpToTarget;

		public static void CopyValues(CCDBone from, ref CCDBone to)
		{
			to.LocalHingeAxis = from.LocalHingeAxis;
			to.LimitReferenceAxis = from.LimitReferenceAxis;
			to.AngleLimitsDeg = from.AngleLimitsDeg;
			to.Root.position = from.Root.position;
			to.Root.rotation = from.Root.rotation;
			to.Tip.position = from.Tip.position;
			to.Tip.rotation = from.Tip.rotation;
			to.AimForPerpToTarget = from.AimForPerpToTarget;
		}
	}

	[Serializable]
	public struct SpeedParams
	{
		public float RailMoveDegreesPerSec;

		public float RotDegreesPerSec;
	}

	[Serializable]
	private struct AudioLoopProps
	{
		public AudioLoop Loop;

		private float _volume;

		private float _pitch;

		public float VolumeScale;

		public float PitchScale;

		public Vector2 PitchScaleSpeedRange;

		public float Volume
		{
			get
			{
				return _volume;
			}
			set
			{
				_volume = value;
				Loop.SetSourceVolume(value);
			}
		}

		public float Pitch
		{
			get
			{
				return _pitch;
			}
			set
			{
				_pitch = value;
				Loop.SetSourcePitch(value);
			}
		}
	}

	[Serializable]
	private struct Thresholds
	{
		public float MinGrabDist;

		public float MinRotDist;

		public float CCDEpsilon;
	}

	[Serializable]
	private struct VisualizationParams(bool useDefaults)
	{
		public Color BoneColor = Color.red;

		public Color OffsetColor = Color.white;

		public Color HingeAxisColor = Color.yellow;

		public Color AngleLimitReferenceColor = Color.white;

		public Color AngleLimitColor = Color.white;

		public Color MinGrabDistColor = Color.green;

		public Color MinRotDistColor = Color.green;

		public bool SimulateWhileNotRunning = false;
	}

	[SerializeField]
	private Transform _base;

	[SerializeField]
	private Transform _lastGrabTarget;

	[SerializeField]
	private Arc _railArc;

	[SerializeField]
	private CCDBone[] _referenceSkeleton;

	[SerializeField]
	private Claw _claw;

	[SerializeField]
	private JobTina sceneRef;

	[SerializeField]
	private VVRDialogueSource tinaDialogueSource;

	[SerializeField]
	private ObjectToyBox toyBox;

	[SerializeField]
	private Transform tableSpinTransform;

	[SerializeField]
	private Transform dummyTransform;

	private CCDBone[] _restingSkeleton;

	private CCDBone[] _initialSkeleton;

	private CCDBone[] _dynamicSkeleton;

	private SafeCoroutine grabFromBoxThread;

	private VVRConsumable[] allConsumables;

	private SafeCoroutine dropRoutine;

	private int boxInterIndex;

	public bool isGrabbing;

	private int timesVacuumed;

	private bool isDropping;

	private SafeCoroutine moveHeadCoroutine;

	private bool usingClaw;

	private bool bobbingHead;

	[SerializeField]
	public SpeedParams _speed = new SpeedParams
	{
		RailMoveDegreesPerSec = 500f,
		RotDegreesPerSec = 500f
	};

	[SerializeField]
	private AudioLoopProps _movementSFX;

	[SerializeField]
	private AudioLoopProps _clawMovementSFX;

	[SerializeField]
	private AudioSourceLoudness tinaTalker;

	[SerializeField]
	private Thresholds _thresholds = new Thresholds
	{
		MinGrabDist = 0.06f,
		MinRotDist = 3f,
		CCDEpsilon = 0.001f
	};

	private bool hasBeenVacuumed;

	private float animationTimer;

	[SerializeField]
	private AnimationCurve movementCurve;

	private CCDBone[] aSkeleton;

	private CCDBone[] aTargetSkeleton;

	private Transform aClawRotTarget;

	[SerializeField]
	private VisualizationParams _visualizationParams = new VisualizationParams(useDefaults: true);

	public ObjectBase HeldObject => _claw.Held;

	public Transform RailCenter => transform;

	public Arc GetArc()
	{
		return _railArc;
	}

	public Claw GetClaw()
	{
		return _claw;
	}

	public void ReactToVacuumOut()
	{
	}

	public void VacuumTina()
	{
		if (!hasBeenVacuumed)
		{
			hasBeenVacuumed = true;
			for (int i = 0; i < allConsumables.Length; i++)
			{
				allConsumables[i].unconsumable = false;
				allConsumables[i].OneShotTransfer(ToolManager.BestBlackHoleConsumer(allConsumables[i].transform.position), -15f);
			}
		}
	}

	public void ReactToDollConsumed()
	{
	}

	public void InterruptGrabFromBox()
	{
	}

	public SafeCoroutine Grab(ObjectBase grabMe, float maxAttemptToGrabDuration = 9999f)
	{
		return GlobalCoroutineContainer.AddThread(GrabRoutine(grabMe, maxAttemptToGrabDuration), this);
	}

	public SafeCoroutine GrabToyFromBox(int index)
	{
		return grabFromBoxThread = GlobalCoroutineContainer.AddThread(GrabFromBoxRoutine(index), this);
	}

	public SafeCoroutine GrabToysFromBox(int[] indicies)
	{
		return grabFromBoxThread = GlobalCoroutineContainer.AddThread(GrabAndPlaceFromBoxRoutine(indicies), this);
	}

	public SafeCoroutine DropAt(Transform dest)
	{
		return dropRoutine = GlobalCoroutineContainer.AddThread(DropRoutine(dest), this);
	}

	public SafeCoroutine MoveHeadTo(Transform dest)
	{
		return GlobalCoroutineContainer.AddThread(MoveHeadRoutine(dest), this);
	}

	public SafeCoroutine GoToRestPose()
	{
		return GlobalCoroutineContainer.AddThread(ToRestPoseRoutine(), this);
	}

	public SafeCoroutine CloseClaw()
	{
		return GlobalCoroutineContainer.AddThread(ClawRoutine(close: true), this);
	}

	public SafeCoroutine OpenClaw()
	{
		return GlobalCoroutineContainer.AddThread(ClawRoutine(close: false), this);
	}

	public SafeCoroutine RotateToAngle(float target, float time)
	{
		return GlobalCoroutineContainer.AddThread(RotateToAngleOverTime(target, time), this);
	}

	public SafeCoroutine FacePlayer(float time)
	{
		return GlobalCoroutineContainer.AddThread(RotateToFacePlayer(time), this);
	}

	public SafeCoroutine PlaceDollDown()
	{
		Transform dest = null;
		return DropAt(dest);
	}

	public SafeCoroutine MoveHeadAlongCurve(BezierCurve c, float t)
	{
		return moveHeadCoroutine = GlobalCoroutineContainer.AddThread(MoveAlongCurveRoutine(c, t), this);
	}

	private IEnumerator GrabRoutine(ObjectBase grabMe, float maxAttemptToGrabDuration)
	{
		isGrabbing = true;
		yield return ClawRoutine(close: false);
		_claw.TryDropImmediate();
		_lastGrabTarget = grabMe.transform;
		SetupNewSkeletonAnimation(_referenceSkeleton, _dynamicSkeleton, grabMe.transform);
		_claw.SetIgnoreCollisionsWith(grabMe, ignore: true);
		do
		{
			CopySkeletonValues(_restingSkeleton, _dynamicSkeleton);
			CCDSolve(_dynamicSkeleton, grabMe.transform.position, 12);
			SkeletonAnimationStep();
			maxAttemptToGrabDuration -= Time.deltaTime;
			if ((_referenceSkeleton[_referenceSkeleton.Length - 1].Tip.position - grabMe.transform.position).sqrMagnitude < _thresholds.MinGrabDist * _thresholds.MinGrabDist)
			{
				break;
			}
			yield return null;
		}
		while (maxAttemptToGrabDuration > 0f);
		yield return ClawRoutine(close: true);
		if ((_referenceSkeleton[_referenceSkeleton.Length - 1].Tip.position - grabMe.transform.position).sqrMagnitude < _thresholds.MinGrabDist * _thresholds.MinGrabDist)
		{
			_claw.PickupImmediate(grabMe);
		}
		else
		{
			SafeDebug.LogFormat("Failed to Grab {0}", grabMe.name);
		}
		isGrabbing = false;
	}

	private IEnumerator GrabFromBoxRoutine(int index)
	{
		ObjectBase grabMe = toyBox.SpawnDollToy(index).obj;
		grabMe.rb.velocity = Vector3.zero;
		grabMe.rb.isKinematic = true;
		toyBox.openFlaps();
		isGrabbing = true;
		yield return ClawRoutine(close: false);
		_claw.TryDropImmediate();
		_lastGrabTarget = grabMe.transform;
		SetupNewSkeletonAnimation(_referenceSkeleton, _dynamicSkeleton, grabMe.transform);
		_claw.SetIgnoreCollisionsWith(grabMe, ignore: true);
		yield return MoveHeadAlongCurve(toyBox.grabCurve, 0.15f);
		bool animatedToTarget;
		do
		{
			CopySkeletonValues(_restingSkeleton, _dynamicSkeleton);
			CCDSolve(_dynamicSkeleton, grabMe.transform.position, 12);
			animatedToTarget = SkeletonAnimationStep();
			if ((_referenceSkeleton[_referenceSkeleton.Length - 1].Tip.position - grabMe.transform.position).sqrMagnitude < _thresholds.MinGrabDist * _thresholds.MinGrabDist)
			{
				break;
			}
			yield return null;
		}
		while (!animatedToTarget);
		yield return ClawRoutine(close: true);
		_claw.PickupImmediate(grabMe);
		toyBox.closeFlaps();
		grabMe.rb.isKinematic = false;
		isGrabbing = false;
		yield return null;
	}

	private IEnumerator GrabAndPlaceFromBoxRoutine(int[] indicies, bool forceToMouth = false, bool useRail = false)
	{
		yield return toyBox.openFlaps();
		for (int i = 0; i < indicies.Length; i++)
		{
			DollhouseItem hold = toyBox.itemsToSpawn[indicies[i]];
			hold.obj.transform.position = toyBox.itemSpawnPos.position;
			yield return GrabToyFromBox(indicies[i]);
			yield return DropAt(hold.dropPos);
		}
		yield return GoToRestPose();
		toyBox.closeFlaps();
		grabFromBoxThread = null;
	}

	private IEnumerator DropRoutine(Transform dest)
	{
		isDropping = true;
		SetupNewSkeletonAnimation(_referenceSkeleton, _dynamicSkeleton, dest);
		bool animationFinished;
		do
		{
			CopySkeletonValues(_restingSkeleton, _dynamicSkeleton);
			CCDSolve(_dynamicSkeleton, dest.position, 12);
			animationFinished = SkeletonAnimationStep();
			yield return null;
		}
		while (!animationFinished);
		yield return ClawRoutine(close: false);
		_claw.TryDropImmediate();
		_lastGrabTarget = null;
		isDropping = false;
	}

	private IEnumerator MoveHeadRoutine(Transform dest)
	{
		SetupNewSkeletonAnimation(_referenceSkeleton, _dynamicSkeleton, dest);
		bool animationFinished;
		do
		{
			CopySkeletonValues(_restingSkeleton, _dynamicSkeleton);
			CCDSolve(_dynamicSkeleton, dest.position, 12);
			animationFinished = SkeletonAnimationStep();
			yield return null;
		}
		while (!animationFinished);
	}

	private IEnumerator ClawRoutine(bool close, float duration = 0.015f)
	{
		usingClaw = true;
		if (close)
		{
			bool leftDone = false;
			bool rightDone = false;
			do
			{
				leftDone = ((!leftDone) ? RotateClawSideAndTest(_claw.LeftSkeleton, ref _claw.LeftClosedFraction, Mathf.MoveTowards(_claw.LeftClosedFraction, 1f, 1f / duration * Time.deltaTime)) : leftDone);
				rightDone = ((!rightDone) ? RotateClawSideAndTest(_claw.RightSkeleton, ref _claw.RightClosedFraction, Mathf.MoveTowards(_claw.RightClosedFraction, 1f, 1f / duration * Time.deltaTime)) : rightDone);
				yield return null;
			}
			while (!leftDone || !rightDone);
		}
		else
		{
			do
			{
				RotateClawSideAndTest(_claw.LeftSkeleton, ref _claw.LeftClosedFraction, Mathf.MoveTowards(_claw.LeftClosedFraction, 0f, 1f / duration * Time.deltaTime));
				RotateClawSideAndTest(_claw.RightSkeleton, ref _claw.RightClosedFraction, Mathf.MoveTowards(_claw.RightClosedFraction, 0f, 1f / duration * Time.deltaTime));
				yield return null;
			}
			while (_claw.LeftClosedFraction > 0f || _claw.RightClosedFraction > 0f);
		}
		usingClaw = false;
	}

	private IEnumerator MoveAlongCurveRoutine(BezierCurve curve, float time)
	{
		dummyTransform.position = curve.GetPoint(0f);
		dummyTransform.rotation = curve.transform.rotation;
		yield return MoveHeadTo(dummyTransform);
		SetupNewSkeletonAnimation(_referenceSkeleton, _dynamicSkeleton, transform);
		float timer = 0f;
		while (timer <= 1f)
		{
			Vector3 pos = curve.GetPoint(timer);
			do
			{
				CopySkeletonValues(_restingSkeleton, _dynamicSkeleton);
				CCDSolve(_dynamicSkeleton, pos, 12);
			}
			while (!SkeletonAnimationStep());
			timer += Time.deltaTime / time;
			yield return null;
		}
	}

	private IEnumerator RotateToAngleOverTime(float target, float time)
	{
		float t = 0f;
		float startAng = _railArc.CurrAngleOnArc;
		while (t < 1f)
		{
			t += Mathf.Clamp01(Time.deltaTime / time);
			_railArc.CurrAngleOnArc = Mathf.Lerp(startAng, target, movementCurve.Evaluate(t));
			PositionBaseAtAngle(_railArc.CurrAngleOnArc);
			yield return null;
		}
		yield return null;
	}

	private IEnumerator RotateToFacePlayer(float time)
	{
		float t = 0f;
		Transform root = _base;
		Quaternion startAng = root.rotation;
		Quaternion endAng = Quaternion.AngleAxis(90f, transform.up) * Quaternion.LookRotation((Camera.main.transform.position - transform.position).normalized.Flatten(), transform.up);
		while (t <= 1f)
		{
			SafeDebug.DrawRay(transform.position, transform.position - Camera.main.transform.position, Color.blue);
			t += Time.deltaTime / time;
			root.rotation = Quaternion.Slerp(startAng, endAng, t);
			yield return null;
		}
	}

	private IEnumerator ToRestPoseRoutine()
	{
		SetupNewSkeletonAnimation(_referenceSkeleton, _restingSkeleton, transform);
		bool atRest;
		do
		{
			atRest = SkeletonAnimationStep();
			yield return null;
		}
		while (!atRest);
	}

	private bool RotateClawSideAndTest(CCDBone[] clawSkel, ref float closedFrac, float desFrac)
	{
		float t = Mathf.Min(desFrac / 0.25f, 1f);
		float angle = Mathf.Lerp(clawSkel[0].AngleLimitsDeg[0], clawSkel[0].AngleLimitsDeg[1], t);
		Vector3 vector = clawSkel[0].Root.rotation * clawSkel[0].LocalHingeAxis;
		Vector3 normalized = (clawSkel[0].Tip.position - clawSkel[0].Root.position).normalized;
		float angle2 = MathHelpers.SignedAngleBetween(normalized, Quaternion.AngleAxis(angle, vector) * (_claw.Root.rotation * clawSkel[0].LimitReferenceAxis), vector);
		clawSkel[0].Root.rotation = Quaternion.AngleAxis(angle2, vector) * clawSkel[0].Root.rotation;
		float t2 = Mathf.Clamp01((desFrac - 0.25f) / 0.75f);
		float angle3 = Mathf.Lerp(clawSkel[1].AngleLimitsDeg[0], clawSkel[1].AngleLimitsDeg[1], t2);
		vector = clawSkel[1].Root.rotation * clawSkel[1].LocalHingeAxis;
		normalized = (clawSkel[1].Tip.position - clawSkel[1].Root.position).normalized;
		float angle4 = MathHelpers.SignedAngleBetween(normalized, Quaternion.AngleAxis(angle3, vector) * (clawSkel[0].Root.rotation * clawSkel[1].LimitReferenceAxis), vector);
		clawSkel[1].Root.rotation = Quaternion.AngleAxis(angle4, vector) * clawSkel[1].Root.rotation;
		closedFrac = desFrac;
		Vector3 vector2 = clawSkel[1].Tip.forward * 0.02f;
		Vector3 vector3 = clawSkel[1].Tip.position + vector2;
		SafeDebug.DrawRay(vector3, clawSkel[1].Tip.forward * 0.05f, Color.red, 1f);
		RaycastHit hitInfo;
		return desFrac >= 1f || Physics.Raycast(vector3, clawSkel[1].Tip.forward, out hitInfo, 0.05f);
	}

	private float ClosestAngleOnArc(Vector3 arcCenter, Vector3 planeNorm, Vector3 angleReference, Vector2 angleLimits, Vector3 point)
	{
		return Mathf.Clamp(MathHelpers.SignedAngleBetween(angleReference, (point - arcCenter).normalized, planeNorm), angleLimits[0], angleLimits[1]);
	}

	private Vector3 PointOnArcAtAngle(Vector3 arcCenter, Vector3 planeNorm, Vector3 angleReference, float radius, float angle)
	{
		return Quaternion.AngleAxis(angle, planeNorm) * angleReference * radius + arcCenter;
	}

	private static CCDBone[] CloneSkeleton(CCDBone[] skeleton, bool hidden)
	{
		CCDBone[] array = new CCDBone[skeleton.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Root = new GameObject($"{skeleton[i].Root.name}").transform;
			if (i > 0)
			{
				array[i].Root.parent = array[i - 1].Tip;
			}
			else
			{
				array[i].Root.parent = skeleton[i].Root.parent;
			}
			array[i].Tip = new GameObject($"{skeleton[i].Tip.name}").transform;
			array[i].Tip.parent = array[i].Root;
			if (hidden)
			{
				array[i].Root.gameObject.hideFlags = HideFlags.HideAndDontSave;
				array[i].Tip.gameObject.hideFlags = HideFlags.HideAndDontSave;
			}
		}
		CopySkeletonValues(skeleton, array);
		return array;
	}

	private static void CopySkeletonValues(CCDBone[] from, CCDBone[] to)
	{
		for (int i = 0; i < to.Length; i++)
		{
			CCDBone.CopyValues(from[i], ref to[i]);
		}
	}

	private void PositionBaseAtAngle(float angle)
	{
		PositionBaseAt(PointOnArcAtAngle(RailCenter.position, RailCenter.up, RailCenter.forward, _railArc.Radius, angle));
	}

	private void PositionBaseAt(Vector3 pos)
	{
		Transform transform = _base;
		transform.position = pos;
		Vector3 normalized = (pos - RailCenter.position).normalized;
		transform.rotation = Quaternion.AngleAxis(-90f, base.transform.up) * Quaternion.LookRotation(normalized, base.transform.up);
	}

	private void OnEnable()
	{
		PositionBaseAtAngle(_railArc.CurrAngleOnArc);
		Claw.Init(ref _claw, this);
		_dynamicSkeleton = CloneSkeleton(_referenceSkeleton, hidden: true);
		_restingSkeleton = CloneSkeleton(_referenceSkeleton, hidden: true);
		_initialSkeleton = CloneSkeleton(_referenceSkeleton, hidden: true);
	}

	private void OnDisable()
	{
	}

	private void Start()
	{
		allConsumables = GetComponentsInChildren<VVRConsumable>();
	}

	private void SetupNewSkeletonAnimation(CCDBone[] skeletonToAnimate, CCDBone[] targetSkeleton, Transform clawRotTarget)
	{
		animationTimer = 0f;
		CopySkeletonValues(skeletonToAnimate, _initialSkeleton);
		aSkeleton = skeletonToAnimate;
		aTargetSkeleton = targetSkeleton;
		aClawRotTarget = clawRotTarget;
	}

	private bool SkeletonAnimationStep()
	{
		animationTimer += Time.deltaTime;
		bool flag = true;
		for (int i = 0; i < _initialSkeleton.Length; i++)
		{
			Vector3 normalized = (_initialSkeleton[i].Tip.position - _initialSkeleton[i].Root.position).normalized;
			Vector3 normalized2 = (aTargetSkeleton[i].Tip.position - aTargetSkeleton[i].Root.position).normalized;
			Vector3 axisNorm = _initialSkeleton[i].Root.rotation * _initialSkeleton[i].LocalHingeAxis;
			float f = MathHelpers.SignedAngleBetween(normalized, normalized2, axisNorm);
			float num = Mathf.Abs(f) / _speed.RotDegreesPerSec;
			float num2 = Mathf.Clamp01(animationTimer / (num + 0.05f));
			flag = flag && num2 >= 1f;
			aSkeleton[i].Root.rotation = Quaternion.Slerp(_initialSkeleton[i].Root.rotation, _dynamicSkeleton[i].Root.rotation, num2);
		}
		Vector3 vector = _claw.Root.rotation * _claw.LocalHingeAxis;
		Vector3 a = _claw.Root.rotation * _claw.ReferenceAxis;
		float b = MathHelpers.SignedAngleBetween(a, aClawRotTarget.up, vector);
		_claw.Root.rotation = Quaternion.AngleAxis(Mathf.Lerp(0f, b, 12f * Time.deltaTime), vector) * _claw.Root.rotation;
		return flag;
	}

	private float CCDSolve(CCDBone[] chain, Vector3 target, int maxIterations, int debugSteps = -1, bool stepForwards = true)
	{
		bool flag = debugSteps > -1;
		int num = 0;
		float cCDEpsilon = _thresholds.CCDEpsilon;
		for (int i = 0; i < maxIterations; i++)
		{
			if (stepForwards)
			{
				for (int j = 0; j < chain.Length; j++)
				{
					if (flag && num >= debugSteps)
					{
						break;
					}
					float num2 = CCDStep(chain, target, j);
					if (num2 < cCDEpsilon)
					{
						return num2;
					}
				}
				continue;
			}
			int num3 = chain.Length - 1;
			while (num3 >= 0 && (!flag || num < debugSteps))
			{
				float num2 = CCDStep(chain, target, num3);
				if (num2 < cCDEpsilon)
				{
					return num2;
				}
				num3--;
			}
		}
		return (chain[chain.Length - 1].Tip.position - target).sqrMagnitude;
	}

	private float CCDStep(CCDBone[] chain, Vector3 target, int i)
	{
		if (!chain[i].AimForPerpToTarget)
		{
			Vector3 normalized = (chain[i].Tip.position - chain[i].Root.position).normalized;
			Vector3 normalized2 = (chain[chain.Length - 1].Tip.position - chain[i].Root.position).normalized;
			Vector3 normalized3 = (target - chain[i].Root.position).normalized;
			Vector3 vector = chain[i].Root.rotation * chain[i].LocalHingeAxis;
			normalized3 = Vector3.ProjectOnPlane(normalized3, vector).normalized;
			Quaternion quaternion = ((i <= 0) ? Quaternion.identity : chain[i - 1].Root.rotation);
			Vector3 vector2 = quaternion * chain[i].LimitReferenceAxis;
			float angle = MathHelpers.SignedAngleBetween(normalized2, normalized3, vector);
			if (chain[i].AngleLimitsDeg[0] > -180f || chain[i].AngleLimitsDeg[0] < 180f)
			{
				Vector3 b = Quaternion.AngleAxis(angle, vector) * normalized;
				angle = Mathf.Clamp(MathHelpers.SignedAngleBetween(vector2, b, vector), chain[i].AngleLimitsDeg[0], chain[i].AngleLimitsDeg[1]);
				b = Quaternion.AngleAxis(angle, vector) * vector2;
				angle = MathHelpers.SignedAngleBetween(normalized, b, vector);
			}
			chain[i].Root.rotation = Quaternion.AngleAxis(angle, vector) * chain[i].Root.rotation;
		}
		return (chain[chain.Length - 1].Tip.position - target).sqrMagnitude;
	}

	public void OnJointBreak(float breakForce)
	{
		_claw.TryDropImmediate();
	}

	private void OnDrawGizmos()
	{
		if (!Application.isPlaying)
		{
			Vector3 vector = PointOnArcAtAngle(RailCenter.position, RailCenter.up, RailCenter.forward, _railArc.Radius, _railArc.CurrAngleOnArc);
			Vector3 normalized = (vector - RailCenter.position).normalized;
			Gizmos.DrawWireMesh(_base.GetComponent<MeshFilter>().sharedMesh, vector, Quaternion.AngleAxis(-90f, transform.up) * Quaternion.LookRotation(normalized, transform.up));
		}
		if (_lastGrabTarget != null)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawCube(_lastGrabTarget.position, Vector3.one * 0.2f);
		}
		GizmosArc(RailCenter.position, RailCenter.rotation * Vector3.up, RailCenter.rotation * Vector3.forward, _railArc.Radius, _railArc.AngleRangeDeg);
		if (_lastGrabTarget != null)
		{
			float angle = ClosestAngleOnArc(RailCenter.position, RailCenter.up, RailCenter.forward, _railArc.AngleRangeDeg, _lastGrabTarget.position);
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(PointOnArcAtAngle(RailCenter.position, RailCenter.up, RailCenter.forward, _railArc.Radius, angle), Vector3.one * 0.2f);
		}
		if (!Application.isPlaying)
		{
			if (_visualizationParams.SimulateWhileNotRunning && _lastGrabTarget != null)
			{
				_dynamicSkeleton = CloneSkeleton(_referenceSkeleton, hidden: true);
				CCDSolve(_dynamicSkeleton, _lastGrabTarget.position, 12);
				GizmosCCDHierarchy(_dynamicSkeleton);
			}
			else
			{
				GizmosCCDHierarchy(_referenceSkeleton);
				GizmosCCDHierarchy(_claw.LeftSkeleton);
				GizmosCCDHierarchy(_claw.RightSkeleton);
				Gizmos.DrawWireSphere(_referenceSkeleton[_referenceSkeleton.Length - 1].Tip.position, _thresholds.MinGrabDist);
				Gizmos.DrawWireSphere(_base.position, _thresholds.MinRotDist);
			}
		}
		else
		{
			if (_dynamicSkeleton != null)
			{
				GizmosCCDHierarchy(_dynamicSkeleton);
			}
			else
			{
				GizmosCCDHierarchy(_referenceSkeleton);
			}
			GizmosCCDHierarchy(_claw.LeftSkeleton);
			GizmosCCDHierarchy(_claw.RightSkeleton);
		}
	}

	private void GizmosCCDHierarchy(CCDBone[] chain)
	{
		if (chain == null)
		{
			return;
		}
		for (int i = 0; i < chain.Length; i++)
		{
			CCDBone cCDBone = chain[i];
			Gizmos.color = _visualizationParams.BoneColor;
			Gizmos.DrawLine(cCDBone.Root.position, cCDBone.Tip.position);
			float magnitude = (cCDBone.Tip.position - cCDBone.Root.position).magnitude;
			Gizmos.DrawSphere(cCDBone.Root.position, 0.1f * magnitude);
			Gizmos.DrawSphere(cCDBone.Tip.position, 0.05f * magnitude);
			Gizmos.color = _visualizationParams.OffsetColor;
			if (i < chain.Length - 1)
			{
				GizmosDottedLine(cCDBone.Tip.position, chain[i + 1].Root.position);
			}
			Gizmos.color = _visualizationParams.HingeAxisColor;
			Gizmos.DrawRay(cCDBone.Root.position, cCDBone.Root.rotation * cCDBone.LocalHingeAxis * 0.5f);
			Quaternion quaternion = ((i <= 0) ? Quaternion.identity : chain[i - 1].Root.rotation);
			Gizmos.color = _visualizationParams.AngleLimitReferenceColor;
			Gizmos.DrawRay(cCDBone.Root.position, quaternion * cCDBone.LimitReferenceAxis * 0.5f);
			Gizmos.color = _visualizationParams.AngleLimitColor;
			if (cCDBone.AngleLimitsDeg.sqrMagnitude < 0.001f)
			{
				GizmosArc(cCDBone.Root.position, cCDBone.Root.rotation * cCDBone.LocalHingeAxis, quaternion * cCDBone.LimitReferenceAxis, 0.2f * magnitude, new Vector2(0f, 360f));
			}
			else
			{
				GizmosArc(cCDBone.Root.position, cCDBone.Root.rotation * cCDBone.LocalHingeAxis, quaternion * cCDBone.LimitReferenceAxis, 0.3f * magnitude, cCDBone.AngleLimitsDeg);
			}
		}
	}

	private void GizmosDottedLine(Vector3 start, Vector3 end)
	{
		Vector3 vector = (end - start) / 10f;
		for (int i = 0; i < 10; i++)
		{
			Gizmos.DrawLine(start + vector * i, start + vector * ((float)i + 0.5f));
		}
	}

	private void GizmosArc(Vector3 center, Vector3 axis, Vector3 reference, float radius, Vector2 rangeDeg)
	{
		int num = 30;
		for (int i = 0; i < num; i++)
		{
			Vector3 vector = Quaternion.AngleAxis(Mathf.Lerp(rangeDeg[0], rangeDeg[1], (float)i / (float)num), axis) * reference * radius + center;
			Vector3 to = Quaternion.AngleAxis(Mathf.Lerp(rangeDeg[0], rangeDeg[1], (float)(i + 1) / (float)num), axis) * reference * radius + center;
			Gizmos.DrawLine(vector, to);
		}
	}
}
