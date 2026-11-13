using System;
using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class ObjectBoid : ObjectBase
{
	public enum behaviorMode
	{
		ambient,
		search,
		destroy,
		searchAndDestroy,
		chase,
		construct,
		deconstruct,
		roost
	}

	public enum actionMode
	{
		transfering,
		flying,
		sitting,
		held,
		avoidingObstacle,
		hugging,
		docking
	}

	public BoidController controller;

	public Mesh highLODHeadset;

	public Mesh lowLODHeadset;

	public MeshFilter hFilter;

	public GameObject eyeGlow;

	public GameObject eyeWhite;

	public GameObject eyeBlack;

	private float noiseOffset;

	private VVRConsumer consumer;

	public behaviorMode startBehavior;

	public behaviorMode currentBehavior;

	public actionMode currentAction = actionMode.flying;

	[Range(0.1f, 20f)]
	public float ambientRoostAlternateTime = 10f;

	private float alternateTimer;

	private Transform player;

	private SafeCoroutine restartThread;

	private SafeCoroutine powerDownThread;

	private SafeCoroutine powerUpThread;

	public BoidStraps Straps;

	[HideInInspector]
	public Headset h;

	private ParticleSystem particles;

	public Vector3 _targetPosition;

	public bool startPoweredOff;

	public bool startNoGrav;

	public bool killOnBreak;

	public bool noCollisions;

	public bool noLeashBreak;

	private bool power = true;

	private bool _highLOD;

	private Vector3 currentPosition;

	private Vector3 separation;

	private Vector3 alignment;

	private Vector3 cohesion;

	private Vector3 diff;

	private Quaternion currentRotation;

	private float noise;

	private float velocity;

	private float avg;

	private float diffLen;

	private float scaler;

	private const int MAX_BUFFER = 6;

	private Collider[] nearbyBoids = new Collider[6];

	private Collider boidCollider;

	private Transform bt;

	private int avgInt;

	private float foodSearchTimer;

	private float playerSearchTimer;

	private float hugTestTimer;

	private int priorityQueue = 3;

	private float constructTimer;

	public float constructionDetectaionSqr = 4f;

	private float deconstructTimer;

	private float lookDot;

	private float velocityDot;

	private Vector3 headsetDelta;

	private int cnt;

	private Vector3 cachedTP;

	private float dockingRotationSpeed = 0.03f;

	private float dockingMoveSpeed = 0.02f;

	private float angle;

	private float ip;

	private Quaternion rotation;

	private Vector3 positionDelta;

	private Vector3 direction;

	private Vector3 axis;

	private Vector3 angularDisplacement;

	private float sqrDistFromBite;

	private Vector3 biteDelta;

	public GameObject targetRoost;

	[HideInInspector]
	public Transform targetRoostTransform;

	[HideInInspector]
	public Vector3 targetRoostDock;

	public VVRConsumable targetConsumable;

	private VVRConsumable highConsumable;

	private float consumeScore;

	private float highConsumeScore;

	private int numberToCheck;

	public float playerVisionDistSQR = 25f;

	private Vector3 bitePosition;

	private VVRConsumable testConsumable;

	private int chaseAvoidInt;

	public float roostDetectionSqrDist = 0.5f;

	public float dockingDistance = 1f;

	private actionMode preGrabAction;

	[SerializeField]
	[CurveRange]
	private AnimationCurve struggleCurve;

	private float scalar = 80f;

	private float struggleTimer;

	private float struggleFreq;

	private float stretch;

	private float length;

	private Vector3 randomDirection = Vector3.zero;

	private int breakCount;

	private bool alreadyIncrementedBreakCount;

	private float breakValue;

	public bool Power
	{
		get
		{
			return power;
		}
		set
		{
			if (value && !power && (powerUpThread == null || !powerUpThread.IsRunning))
			{
				PowerUp();
			}
			else if (!value && power && (powerDownThread == null || !powerDownThread.IsRunning))
			{
				PowerDown();
			}
		}
	}

	private Vector3 targetPosition
	{
		get
		{
			if (currentAction == actionMode.avoidingObstacle)
			{
				return (!(targetConsumable != null)) ? controller.t.position : targetConsumable.transform.position;
			}
			if (currentBehavior == behaviorMode.chase)
			{
				return player.position + player.forward;
			}
			if (currentBehavior == behaviorMode.roost)
			{
				return (!(targetRoost != null)) ? controller.t.position : targetRoostDock;
			}
			return (!(targetConsumable != null)) ? controller.t.position : targetConsumable.transform.position;
		}
		set
		{
			_targetPosition = value;
		}
	}

	public bool highLOD
	{
		get
		{
			return _highLOD;
		}
		set
		{
			if (highLODHeadset != null && lowLODHeadset != null && hFilter != null)
			{
				if (value)
				{
					hFilter.sharedMesh = highLODHeadset;
					if (eyeGlow != null)
					{
						eyeGlow.SetActive(value: true);
					}
				}
				else
				{
					hFilter.sharedMesh = lowLODHeadset;
					if (eyeGlow != null)
					{
						eyeGlow.SetActive(value: false);
					}
				}
			}
			_highLOD = value;
		}
	}

	private Vector3 GetSeparationVector(Transform target)
	{
		diff = currentPosition - target.position;
		diffLen = diff.magnitude;
		scaler = Mathf.Clamp01(1f - diffLen / controller.neighborDist);
		if (float.IsNaN((diff * (scaler / diffLen)).x))
		{
			return Vector3.zero;
		}
		return diff * (scaler / diffLen);
	}

	protected override void Awake()
	{
		base.Awake();
		if (startPoweredOff)
		{
			power = false;
			if (!startNoGrav && rb != null)
			{
				rb.useGravity = true;
			}
			SwapColliders(boxOn: true);
			eyeWhite.SetActive(value: false);
			eyeBlack.SetActive(value: true);
		}
		preGrabAction = currentAction;
	}

	private void SwapColliders(bool boxOn)
	{
		if (GetComponent<BoxCollider>() != null && GetComponent<SphereCollider>() != null)
		{
			GetComponent<BoxCollider>().enabled = boxOn;
			GetComponent<SphereCollider>().enabled = !boxOn;
		}
	}

	protected override void Start()
	{
		base.Start();
		changeHeldLayer = false;
		SetBehaviorMode(startBehavior);
		noiseOffset = UnityEngine.Random.value * 10f;
		rb.maxAngularVelocity = float.MaxValue;
		consumer = gameObject.GetComponent<VVRConsumer>();
		player = Camera.main.transform;
		h = GetComponent<Headset>();
		particles = GetComponentInChildren<ParticleSystem>();
		if (particles != null)
		{
			particles.Stop();
		}
		highLOD = false;
	}

	private void Update()
	{
		if (power)
		{
			switch (currentBehavior)
			{
			case behaviorMode.ambient:
				AmbientUpdate();
				break;
			case behaviorMode.destroy:
				SearchAndDestroyUpdate();
				break;
			case behaviorMode.search:
				SearchAndDestroyUpdate();
				break;
			case behaviorMode.searchAndDestroy:
				SearchAndDestroyUpdate();
				break;
			case behaviorMode.chase:
				ChaseUpdate();
				break;
			case behaviorMode.construct:
				ConstructUpdate();
				break;
			case behaviorMode.deconstruct:
				DeconstructUpdate();
				break;
			case behaviorMode.roost:
				RoostUpdate();
				break;
			}
		}
		if (currentAction == actionMode.flying || currentAction == actionMode.avoidingObstacle)
		{
			UpdateNearbyBoids();
		}
	}

	private void FixedUpdate()
	{
		if (!power)
		{
			return;
		}
		switch (currentAction)
		{
		case actionMode.flying:
			FlyingFixedUpdate();
			break;
		case actionMode.avoidingObstacle:
			FlyingFixedUpdate();
			break;
		case actionMode.transfering:
			TransferingFixedUpdate();
			break;
		case actionMode.sitting:
			SittingFixedUpdate();
			break;
		case actionMode.held:
			if (grabbedBy.Count > 0)
			{
				HeldFixedUpdate();
			}
			break;
		case actionMode.hugging:
			HuggingFixedUpdate();
			break;
		case actionMode.docking:
			DockingFixedUpdate();
			break;
		}
	}

	private void OnDisable()
	{
		if (BoidSoundPool.instance != null)
		{
			if (controller != null && controller.flyingSoundBoids.Contains(this))
			{
				controller.flyingSoundBoids.Remove(this);
			}
			BoidSoundPool.instance.StopBoidSound(this, 0f);
			if (BoidSoundPool.instance.musicEmitter[0].GetAttachedObject() == this)
			{
				BoidSoundPool.instance.StopBoidMusic(0f);
			}
		}
	}

	private void OnEnable()
	{
		if (controller == null)
		{
			GlobalCoroutineContainer.AddThread(DelayedOnEnable(), this);
			return;
		}
		setUpFlyingSound(currentAction);
		setUpMusic(currentBehavior, currentAction);
	}

	private IEnumerator DelayedOnEnable()
	{
		yield return null;
		setUpFlyingSound(currentAction);
		setUpMusic(currentBehavior, currentAction);
	}

	private void setUpFlyingSound(actionMode action)
	{
		if (action == actionMode.flying && !(BoidSoundPool.instance == null))
		{
			if (controller == null)
			{
				SafeDebug.LogWarning("Controller still null on: " + name);
			}
			else if (BoidSoundPool.instance == null)
			{
				SafeDebug.LogWarning("Boid sound pool not created yet music will be not set up (likely caused by launching from not root)");
			}
			else if (controller.playsSound && controller.flyingSoundBoids.Count < controller.GetFlyingSoundCount() && BoidSoundPool.instance.getAttachedEmitter(this) == null && !controller.flyingSoundBoids.Contains(this))
			{
				controller.flyingSoundBoids.Add(this);
				BoidSoundPool.instance.PlayBoidSound(BoidSoundType.flying, this);
			}
		}
	}

	private void setUpMusic(behaviorMode b, actionMode a)
	{
		if (controller == null)
		{
			SafeDebug.LogWarning("Controller still null on: " + name);
		}
		else if (BoidSoundPool.instance == null)
		{
			SafeDebug.LogWarning("Boid sound pool not created yet music will be not set up (likely caused by launching from not root)");
		}
		else if (controller.playsMusic)
		{
			if (b == behaviorMode.search)
			{
			}
			if (b == behaviorMode.chase)
			{
				BoidSoundPool.instance.SetMusicState(1);
				BoidSoundPool.instance.PlayBoidSound(BoidSoundType.music, this);
			}
			if (a == actionMode.hugging)
			{
				BoidSoundPool.instance.SetMusicState(2);
				BoidSoundPool.instance.PlayBoidSound(BoidSoundType.music, this);
			}
		}
	}

	private void transferFlyingSound(actionMode action)
	{
		bool flag = false;
		if (action == actionMode.held)
		{
			for (int i = 0; i < controller.boidList.Count; i++)
			{
				if (controller.boidList[i].currentAction == actionMode.flying)
				{
					controller.flyingSoundBoids.Remove(this);
					BoidSoundPool.instance.PlayBoidSound(BoidSoundType.flying, controller.boidList[i]);
					controller.flyingSoundBoids.Add(controller.boidList[i]);
					break;
				}
			}
			return;
		}
		for (int j = 0; j < controller.boidList.Count; j++)
		{
			if (controller.boidList[j].currentAction == actionMode.flying)
			{
				BoidSoundPool.instance.ChangeAttachedObject(this, controller.boidList[j]);
				flag = true;
				controller.flyingSoundBoids.Remove(this);
				controller.flyingSoundBoids.Add(controller.boidList[j]);
			}
		}
		if (!flag)
		{
			BoidSoundPool.instance.StopBoidSound(this, 0f);
			controller.flyingSoundBoids.Remove(this);
		}
	}

	public void SetActionMode(actionMode newAction)
	{
		if (newAction == currentAction)
		{
			return;
		}
		if (currentAction == actionMode.flying && controller.flyingSoundBoids.Contains(this))
		{
			transferFlyingSound(newAction);
		}
		else if (currentAction == actionMode.held)
		{
			BoidSoundPool.instance.StopBoidSound(this, 2f);
		}
		setUpFlyingSound(newAction);
		switch (newAction)
		{
		case actionMode.flying:
			if (currentAction == actionMode.transfering && BoidSoundPool.instance != null)
			{
				BoidSoundPool.instance.StopBoidSound(this, 1f);
			}
			break;
		case actionMode.transfering:
			if (consumer == null)
			{
				return;
			}
			if (BoidSoundPool.instance != null)
			{
				BoidSoundPool.instance.PlayBoidSound(BoidSoundType.consuming, this);
			}
			break;
		case actionMode.hugging:
			if (currentAction == actionMode.transfering && BoidSoundPool.instance != null)
			{
				BoidSoundPool.instance.StopBoidSound(this, 1f);
			}
			if (controller.playsMusic && BoidSoundPool.instance != null)
			{
				BoidSoundPool.instance.SetMusicState(2);
				BoidSoundPool.instance.PlayBoidSound(BoidSoundType.music, this);
			}
			break;
		case actionMode.avoidingObstacle:
			if (currentAction == actionMode.transfering && BoidSoundPool.instance != null)
			{
				BoidSoundPool.instance.StopBoidSound(this, 1f);
			}
			targetPosition = myTransform.position + UnityEngine.Random.onUnitSphere * 2f;
			Invoke("RejoinSwarm", 1f);
			break;
		case actionMode.held:
			if (currentAction == actionMode.transfering && BoidSoundPool.instance != null)
			{
				BoidSoundPool.instance.StopBoidSound(this, 1f);
			}
			break;
		}
		currentAction = newAction;
	}

	public void SetBehaviorMode(behaviorMode newBehavior)
	{
		if ((currentBehavior == behaviorMode.search || currentBehavior == behaviorMode.chase) && newBehavior != behaviorMode.search && newBehavior != behaviorMode.chase && BoidSoundPool.instance != null && BoidSoundPool.instance.IsPlayingMusic() && BoidSoundPool.instance.musicEmitter[0].GetAttachedObject() == this)
		{
			BoidSoundPool.instance.StopBoidMusic(1f);
		}
		if (newBehavior == currentBehavior)
		{
			return;
		}
		if (currentBehavior == behaviorMode.roost && controller.roostController != null && controller.roostController.occupiedRoostPositions.Contains(targetRoost))
		{
			controller.roostController.vacateRoost(targetRoost);
			targetRoost = null;
		}
		switch (newBehavior)
		{
		case behaviorMode.ambient:
			targetConsumable = null;
			SetActionMode(actionMode.flying);
			break;
		case behaviorMode.destroy:
			if (consumer == null)
			{
				SetBehaviorMode(behaviorMode.ambient);
			}
			SetActionMode(actionMode.flying);
			break;
		case behaviorMode.search:
			if (controller.playsMusic && BoidSoundPool.instance != null)
			{
				BoidSoundPool.instance.SetMusicState(0);
				BoidSoundPool.instance.PlayBoidSound(BoidSoundType.music, this);
			}
			SetActionMode(actionMode.flying);
			break;
		case behaviorMode.searchAndDestroy:
			if (consumer == null)
			{
				SetBehaviorMode(behaviorMode.search);
			}
			SetActionMode(actionMode.flying);
			break;
		case behaviorMode.chase:
			if (controller.playsMusic && BoidSoundPool.instance != null)
			{
				BoidSoundPool.instance.SetMusicState(1);
				BoidSoundPool.instance.PlayBoidSound(BoidSoundType.music, this);
			}
			SetActionMode(actionMode.flying);
			break;
		case behaviorMode.construct:
			if (consumer == null)
			{
				SetBehaviorMode(behaviorMode.ambient);
			}
			SetActionMode(actionMode.flying);
			break;
		case behaviorMode.deconstruct:
			if (consumer == null)
			{
				SetBehaviorMode(behaviorMode.ambient);
			}
			SetActionMode(actionMode.flying);
			break;
		case behaviorMode.roost:
			SetActionMode(actionMode.flying);
			break;
		}
		currentBehavior = newBehavior;
	}

	private void AmbientUpdate()
	{
		if (!controller.ambientRoostAlternate || currentAction == actionMode.held || !power)
		{
			return;
		}
		alternateTimer += Time.deltaTime;
		if (alternateTimer >= ambientRoostAlternateTime)
		{
			alternateTimer = 0f;
			if (UnityEngine.Random.Range(0, 10) < 2)
			{
				SetBehaviorMode(behaviorMode.roost);
			}
		}
	}

	private void SearchAndDestroyUpdate()
	{
		if (targetConsumable != null && (targetConsumable.currentObjectState == VVRConsumable.objectState.fullyConsumedGlimmer || targetConsumable.currentObjectState == VVRConsumable.objectState.fullyConsumedStatic))
		{
			targetConsumable = null;
		}
		if (currentBehavior == behaviorMode.destroy || currentBehavior == behaviorMode.searchAndDestroy)
		{
			foodSearchTimer += Time.deltaTime;
			if (foodSearchTimer > 0.25f)
			{
				FoodSearch();
				foodSearchTimer = 0f;
			}
		}
		if (currentBehavior == behaviorMode.search || currentBehavior == behaviorMode.searchAndDestroy)
		{
			playerSearchTimer += Time.deltaTime;
			if (playerSearchTimer > 0.17f)
			{
				playerSearchTimer = 0f;
				PlayerSearch();
			}
		}
	}

	private void ChaseUpdate()
	{
		hugTestTimer += Time.deltaTime;
		if (hugTestTimer > 0.1f)
		{
			hugTestTimer = 0f;
			HugTest();
		}
	}

	private void ConstructUpdate()
	{
		constructTimer += Time.deltaTime;
		if (!(constructTimer > 0.5f))
		{
			return;
		}
		constructTimer = 0f;
		if (controller.toConstruct.Count <= 0 && !controller.constructUpdate() && controller.toDeconstruct.Count <= 0)
		{
			targetConsumable = null;
			SetBehaviorMode(behaviorMode.ambient);
			return;
		}
		if (targetConsumable != null)
		{
			if (controller.toConstruct.Contains(targetConsumable) || !targetConsumable.gameObject.activeInHierarchy)
			{
				if (targetConsumable.currentObjectState == VVRConsumable.objectState.wholeStatic || targetConsumable.currentObjectState == VVRConsumable.objectState.wholeShaking)
				{
					controller.toConstruct.Remove(targetConsumable);
					targetConsumable = null;
				}
			}
			else
			{
				targetConsumable = null;
			}
		}
		if (consumer.fill <= 0f || controller.toConstruct.Count <= 0)
		{
			if (controller.toDeconstruct.Count > 0)
			{
				SetBehaviorMode(behaviorMode.deconstruct);
			}
			else
			{
				SetBehaviorMode(behaviorMode.ambient);
			}
			return;
		}
		if (targetConsumable == null)
		{
			int index = UnityEngine.Random.Range(0, Mathf.Min(priorityQueue, controller.toConstruct.Count));
			targetConsumable = controller.toConstruct[index];
		}
		if (targetConsumable != null && targetConsumable.gameObject.activeInHierarchy && Vector3.SqrMagnitude(myTransform.position - targetConsumable.transform.position) < constructionDetectaionSqr)
		{
			SetupTransfer(targetConsumable);
		}
	}

	private void DeconstructUpdate()
	{
		deconstructTimer += Time.deltaTime;
		if (!(deconstructTimer >= 0.5f))
		{
			return;
		}
		deconstructTimer = 0f;
		if (controller.toDeconstruct.Count <= 0 && !controller.deconstructUpdate() && controller.toConstruct.Count <= 0)
		{
			targetConsumable = null;
			SetBehaviorMode(behaviorMode.ambient);
			return;
		}
		if (targetConsumable != null)
		{
			if (controller.toDeconstruct.Contains(targetConsumable))
			{
				if (targetConsumable.currentObjectState == VVRConsumable.objectState.fullyConsumedGlimmer || targetConsumable.currentObjectState == VVRConsumable.objectState.fullyConsumedStatic || !targetConsumable.gameObject.activeInHierarchy)
				{
					controller.toDeconstruct.Remove(targetConsumable);
					targetConsumable = null;
				}
			}
			else
			{
				targetConsumable = null;
			}
		}
		if (consumer.fill >= consumer.capacity - 0.001f || controller.toDeconstruct.Count <= 0)
		{
			if (controller.toConstruct.Count > 0)
			{
				SetBehaviorMode(behaviorMode.construct);
			}
			else
			{
				SetBehaviorMode(behaviorMode.ambient);
			}
		}
		else if (targetConsumable == null || !targetConsumable.gameObject.activeInHierarchy)
		{
			int index = UnityEngine.Random.Range(0, Mathf.Min(priorityQueue, controller.toDeconstruct.Count));
			targetConsumable = controller.toDeconstruct[index];
			if (targetConsumable == null)
			{
				controller.toDeconstruct.RemoveAt(index);
			}
		}
	}

	private void HugTest()
	{
		headsetDelta = myTransform.position - player.position;
		lookDot = Vector3.Dot(player.forward, headsetDelta.normalized);
		if (lookDot > 0.8f && headsetDelta.sqrMagnitude < 9f && headsetDelta.sqrMagnitude > 1f)
		{
			if (!EagerMaster.instance.playerUnderAttack && !EagerMaster.instance.isSwitchingScenes)
			{
				EagerMaster.instance.playerUnderAttack = true;
				SetActionMode(actionMode.hugging);
				if (Straps != null)
				{
					Straps.Activate();
				}
				highLOD = true;
			}
		}
		else if ((lookDot < 0.1f || headsetDelta.sqrMagnitude > 16f) && currentAction == actionMode.hugging)
		{
			SetActionMode(actionMode.flying);
			EagerMaster.instance.playerUnderAttack = false;
			if (Straps != null)
			{
				Straps.Deactivate();
			}
			highLOD = false;
		}
	}

	private void HuggingFixedUpdate()
	{
		if (Vector3.Distance(myTransform.position, player.position) < 1f)
		{
			h.StartAddingHeadset();
			if (Straps != null)
			{
				Straps.StartGrab();
			}
			EagerMaster.instance.playerUnderAttack = false;
			enabled = false;
		}
		headsetDelta = Camera.main.transform.position - rb.position;
		rb.AddForce(headsetDelta * 0.3f);
		rb.MoveRotation(Quaternion.Lerp(rb.rotation, Quaternion.LookRotation(headsetDelta.normalized), 0.15f));
	}

	private void UpdateNearbyBoids()
	{
		cnt = Physics.OverlapSphereNonAlloc(myTransform.position, controller.neighborDist, nearbyBoids, controller.searchLayer);
	}

	private void FlyingFixedUpdate()
	{
		currentPosition = rb.position;
		currentRotation = rb.rotation;
		noise = Mathf.PerlinNoise(Time.time, noiseOffset) * 2f - 1f;
		velocity = controller.velocity * (1f + noise * controller.velocityVariation);
		separation = Vector3.zero;
		alignment = Vector3.zero;
		cohesion = targetPosition;
		if (cnt > 1)
		{
			for (int i = 0; i < cnt; i++)
			{
				boidCollider = nearbyBoids[i];
				if (!(boidCollider.gameObject == gameObject))
				{
					bt = boidCollider.transform;
					separation += GetSeparationVector(bt);
					alignment += bt.forward;
					cohesion += bt.position;
				}
			}
			avg = 1f / (float)cnt;
			alignment *= avg;
			cohesion *= avg;
		}
		cohesion = (cohesion - currentPosition).normalized;
		direction = separation + alignment + cohesion;
		rotation = Quaternion.FromToRotation(Vector3.forward, direction.normalized);
		updateBoidPosition();
	}

	private void updateBoidPosition()
	{
		if (rotation != currentRotation)
		{
			ip = Mathf.Exp((0f - controller.rotationCoeff) * Time.deltaTime);
			rb.rotation = Quaternion.Slerp(rotation, currentRotation, ip);
		}
		rb.MovePosition(currentPosition + myTransform.forward * (velocity * Time.deltaTime));
	}

	private void updateBoidVelocity()
	{
		rotation = Quaternion.FromToRotation(rb.transform.forward, direction.normalized);
		rotation.ToAngleAxis(out angle, out axis);
		if (angle != 0f)
		{
			angularDisplacement = axis * angle * ((float)Math.PI / 180f);
			rb.angularVelocity = angularDisplacement / 0.2f;
		}
		positionDelta = myTransform.forward * velocity;
		rb.velocity = positionDelta;
	}

	private void DockingFixedUpdate()
	{
		myTransform.rotation = Quaternion.Lerp(myTransform.rotation, targetRoostTransform.rotation, dockingRotationSpeed);
		if (Vector3.SqrMagnitude(myTransform.position - targetRoostTransform.position) > 3E-05f)
		{
			myTransform.position = Vector3.Lerp(myTransform.position, targetRoostTransform.position, dockingMoveSpeed);
			return;
		}
		myTransform.position = targetRoostTransform.position;
		myTransform.rotation = targetRoostTransform.rotation;
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		SetActionMode(actionMode.sitting);
	}

	private void SittingFixedUpdate()
	{
		if (!controller.ambientRoostAlternate)
		{
			return;
		}
		alternateTimer += Time.deltaTime;
		if (!(alternateTimer >= ambientRoostAlternateTime))
		{
			return;
		}
		alternateTimer = 0f;
		if (UnityEngine.Random.Range(0, 10) < 3)
		{
			SetBehaviorMode(behaviorMode.ambient);
			if (controller.roostController != null)
			{
				controller.roostController.vacateRoost(targetRoost);
			}
		}
	}

	private void TransferingFixedUpdate()
	{
		if (targetConsumable == null || targetConsumable.currentObjectState == VVRConsumable.objectState.fullyConsumedGlimmer || targetConsumable.currentObjectState == VVRConsumable.objectState.fullyConsumedStatic)
		{
			consumer.StopTransfer();
			targetConsumable = null;
			SetActionMode(actionMode.flying);
			return;
		}
		biteDelta = bitePosition - rb.position;
		rb.AddForce(-biteDelta * 5f);
		sqrDistFromBite = Vector3.SqrMagnitude(biteDelta);
		if (sqrDistFromBite > Mathf.Pow(consumer._mouthRadius + 0.25f, 2f))
		{
			consumer.StopTransfer();
			SetActionMode(actionMode.flying);
		}
	}

	private void PlayerSearch()
	{
		if (Vector3.SqrMagnitude(player.position - rb.position) < playerVisionDistSQR)
		{
			SetBehaviorMode(behaviorMode.chase);
		}
	}

	private void FoodSearch()
	{
		highConsumable = null;
		highConsumeScore = -999f;
		numberToCheck = Mathf.Min(consumer._nearbyEdibles.Count, 8);
		if (numberToCheck == 0)
		{
			return;
		}
		for (int i = 0; i < numberToCheck; i++)
		{
			testConsumable = consumer._nearbyEdibles[i];
			if (testConsumable == null)
			{
				continue;
			}
			consumeScore = 0f;
			if (testConsumable.consumerList.Count > 4)
			{
				consumeScore--;
			}
			if (testConsumable.currentObjectState == VVRConsumable.objectState.partiallyConsumed)
			{
				consumeScore++;
				if (testConsumable.currentVacuumLerp > 0.25f && testConsumable.currentVacuumLerp < 0.6f && testConsumable.consumerList.Count < 2)
				{
					consumeScore += 2f;
				}
			}
			consumeScore += 2f - Mathf.Clamp((testConsumable.gameObject.transform.position - myTransform.position).sqrMagnitude, 0f, 8f) * 0.25f;
			if (consumeScore > highConsumeScore)
			{
				highConsumeScore = consumeScore;
				highConsumable = testConsumable;
			}
		}
		if (highConsumable != null)
		{
			targetConsumable = highConsumable;
		}
	}

	protected override void OnCollisionEnter(Collision collision)
	{
		if (noCollisions)
		{
			return;
		}
		base.OnCollisionEnter(collision);
		if (collision.relativeVelocity.magnitude > 9f)
		{
			if (isGrabbed && !noLeashBreak)
			{
				for (int i = 0; i < grabbedBy.Count; i++)
				{
					grabbedBy[i].Break();
				}
			}
			if (killOnBreak)
			{
				rb.useGravity = true;
				rb.isKinematic = false;
				SwapColliders(boxOn: true);
				enabled = false;
				return;
			}
			Spark();
			if (Power)
			{
				Restart(Mathf.Clamp(collision.relativeVelocity.magnitude * 0.25f, 3f, 15f));
			}
		}
		if (collision.contacts.Length <= 0)
		{
			return;
		}
		bitePosition = collision.contacts[0].point;
		testConsumable = collision.collider.gameObject.GetComponentInParent<VVRConsumable>();
		if (!(testConsumable != null) || testConsumable.boidsIgnore)
		{
			return;
		}
		switch (currentBehavior)
		{
		case behaviorMode.destroy:
			SetupTransfer(testConsumable);
			break;
		case behaviorMode.searchAndDestroy:
			SetupTransfer(testConsumable);
			break;
		case behaviorMode.chase:
			if (chaseAvoidInt < 3)
			{
				chaseAvoidInt++;
			}
			break;
		case behaviorMode.deconstruct:
			if (testConsumable == targetConsumable)
			{
				SetupTransfer(testConsumable);
			}
			else if (controller.toDeconstruct.Contains(testConsumable))
			{
				targetConsumable = testConsumable;
				SetupTransfer(testConsumable);
			}
			break;
		case behaviorMode.construct:
			break;
		}
	}

	private void RejoinSwarm()
	{
		SetActionMode(actionMode.flying);
	}

	private void RoostUpdate()
	{
		if (!(controller.roostController != null))
		{
			return;
		}
		if (targetRoost == null)
		{
			if (controller.roostController.unoccupiedRoostPositions.Count > 0)
			{
				targetRoost = controller.roostController.unoccupiedRoostPositions[UnityEngine.Random.Range(0, controller.roostController.unoccupiedRoostPositions.Count)];
				targetRoostTransform = targetRoost.transform;
				targetRoostDock = targetRoostTransform.position + targetRoostTransform.forward * dockingDistance;
				controller.roostController.occupyRoost(targetRoost);
			}
		}
		else if (Vector3.SqrMagnitude(myTransform.position - targetRoostDock) <= roostDetectionSqrDist && currentAction != actionMode.sitting)
		{
			SetActionMode(actionMode.docking);
		}
	}

	private void SetupTransfer(VVRConsumable newConsumable)
	{
		chaseAvoidInt = 0;
		targetConsumable = newConsumable;
		if (consumer != null && !consumer.isTransfering)
		{
			if (currentBehavior == behaviorMode.construct)
			{
				consumer.StartTransfer(suckBool: false, targetConsumable);
			}
			else if (currentBehavior == behaviorMode.deconstruct)
			{
				consumer.StartTransfer(suckBool: true, targetConsumable);
			}
			else
			{
				consumer.StartTransfer(suckBool: true);
			}
		}
		if (consumer != null)
		{
			SetActionMode(actionMode.transfering);
		}
		rb.angularVelocity = Vector3.zero;
		rb.velocity = Vector3.zero;
	}

	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		preGrabAction = currentAction;
		SafeDebug.Log("setting pregrabAction to: " + preGrabAction);
		GlobalCoroutineContainer.AddThread(controller.ScatterSwarm(4f, 3f), controller);
		if (controller.canAttack)
		{
			controller.SetSwarmBehavior(behaviorMode.chase);
		}
		if ((bool)GetComponent<FixedJoint>())
		{
			GetComponent<FixedJoint>().breakForce = 150f;
		}
		if (currentAction == actionMode.hugging)
		{
			EagerMaster.instance.playerUnderAttack = false;
		}
		SetActionMode(actionMode.held);
		highLOD = true;
	}

	private void HeldFixedUpdate()
	{
		if (power)
		{
			struggleTimer += Time.deltaTime;
			if (struggleTimer >= struggleFreq)
			{
				randomDirection = UnityEngine.Random.onUnitSphere * scalar;
				if (grabbedBy[0] != null)
				{
					if (Vector3.Dot(randomDirection, grabbedBy[0].t.forward) <= -0.3f)
					{
						randomDirection = -randomDirection;
					}
				}
				else
				{
					SafeDebug.LogError("Doing held fixed update when your grabbed by list is null or empty. Something is very wrong");
				}
				struggleTimer = 0f;
				struggleFreq = UnityEngine.Random.Range(0.8f, 2f);
			}
			rb.rotation = Quaternion.Lerp(rb.rotation, Quaternion.LookRotation(randomDirection), 0.2f);
			rb.AddForce(randomDirection * (struggleCurve.Evaluate(struggleTimer / struggleFreq) * 2f - 1f));
		}
		if (noLeashBreak || grabbedBy.Count <= 0)
		{
			return;
		}
		stretch = Vector3.SqrMagnitude(grabbedBy[0].grabLocation - (Quaternion.identity * grabbedBy[0].transform.forward * grabbedBy[0].grabTargetDistance + grabbedBy[0].transform.position));
		length = Vector3.SqrMagnitude(grabbedBy[0].transform.position - grabbedBy[0].grabLocation);
		breakValue = stretch / (length + 5f);
		if (!alreadyIncrementedBreakCount && breakValue > 0.5f)
		{
			breakCount++;
			alreadyIncrementedBreakCount = true;
			if (breakCount >= 3)
			{
				for (int i = 0; i < grabbedBy.Count; i++)
				{
					grabbedBy[i].Break();
				}
				breakCount = 0;
				alreadyIncrementedBreakCount = false;
			}
		}
		else if (breakValue < 0.45f)
		{
			alreadyIncrementedBreakCount = false;
		}
	}

	public override void GrabRelease(Manipulator grabber)
	{
		base.GrabRelease(grabber);
		if (EagerMaster.instance.playerUnderAttack)
		{
			EagerMaster.instance.playerUnderAttack = false;
		}
		myTransform.localScale = new Vector3(1f, 1f, 1f);
		BADefault();
		highLOD = false;
	}

	public void OnRemovedHeadset()
	{
		if (Straps != null)
		{
			Straps.Deactivate();
		}
		highLOD = false;
	}

	private void Restart(float duration)
	{
		if (restartThread == null || !restartThread.IsRunning)
		{
			restartThread = GlobalCoroutineContainer.AddThread(RestartCoroutine(duration), this);
		}
	}

	private IEnumerator RestartCoroutine(float duration)
	{
		Power = false;
		while (Power)
		{
			yield return null;
		}
		float t = 0f;
		while (t < duration)
		{
			if (UnityEngine.Random.Range(0, 600) == 0)
			{
				Spark();
				if (rb != null)
				{
					rb.AddForce(Vector3.up * UnityEngine.Random.Range(8f, 24f));
					rb.AddTorque(transform.position * UnityEngine.Random.Range(-0.07f, 0.07f));
				}
			}
			t += Time.deltaTime;
			yield return null;
		}
		Power = true;
		yield return null;
	}

	private void PowerUp()
	{
		if (!power && (powerUpThread == null || !powerUpThread.IsRunning))
		{
			powerUpThread = GlobalCoroutineContainer.AddThread(PowerUpCoroutine(), this);
		}
	}

	private IEnumerator PowerUpCoroutine()
	{
		if (BoidSoundPool.instance != null)
		{
			BoidSoundPool.instance.PlayBoidSound(BoidSoundType.on, this);
			setUpMusic(currentBehavior, currentAction);
		}
		float t = 1.1f;
		while (t > 0f)
		{
			if (UnityEngine.Random.Range(0, 10) == 0)
			{
				if (eyeWhite.activeSelf)
				{
					eyeWhite.SetActive(value: false);
					eyeBlack.SetActive(value: true);
				}
				else
				{
					eyeWhite.SetActive(value: true);
					eyeBlack.SetActive(value: false);
				}
			}
			t -= Time.deltaTime;
			yield return null;
		}
		eyeWhite.SetActive(value: true);
		eyeBlack.SetActive(value: false);
		if (rb != null)
		{
			rb.useGravity = false;
		}
		SwapColliders(boxOn: false);
		BADefault();
		power = true;
		yield return null;
		if (BoidSoundPool.instance != null)
		{
			setUpFlyingSound(actionMode.flying);
		}
	}

	private void PowerDown()
	{
		if (power && (powerDownThread == null || !powerDownThread.IsRunning))
		{
			powerDownThread = GlobalCoroutineContainer.AddThread(PowerDownCoroutine(), this);
		}
	}

	private IEnumerator PowerDownCoroutine()
	{
		power = false;
		if (BoidSoundPool.instance != null)
		{
			BoidSoundPool.instance.StopBoidSound(this, 0f);
			BoidSoundPool.instance.PlayBoidSound(BoidSoundType.off, this);
			if (controller.flyingSoundBoids.Contains(this))
			{
				controller.flyingSoundBoids.Remove(this);
			}
			if (BoidSoundPool.instance.musicEmitter[0].GetAttachedObject() == this)
			{
				BoidSoundPool.instance.StopBoidMusic(0f);
			}
		}
		float t = 0.7f;
		while (t > 0f)
		{
			if (UnityEngine.Random.Range(0, 10) == 0)
			{
				if (eyeWhite.activeSelf)
				{
					eyeWhite.SetActive(value: false);
					eyeBlack.SetActive(value: true);
				}
				else
				{
					eyeWhite.SetActive(value: true);
					eyeBlack.SetActive(value: false);
				}
			}
			t -= Time.deltaTime;
			yield return null;
		}
		eyeWhite.SetActive(value: false);
		eyeBlack.SetActive(value: true);
		if (rb != null)
		{
			rb.useGravity = true;
		}
		SwapColliders(boxOn: true);
		yield return null;
	}

	private void Spark()
	{
		GlobalCoroutineContainer.AddThread(SparkCoroutine(), this);
	}

	private IEnumerator SparkCoroutine()
	{
		if (particles != null)
		{
			particles.Play();
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.1f, this);
			particles.Stop();
		}
		yield return null;
	}

	private void BADefault()
	{
		if (isGrabbed)
		{
			SetActionMode(actionMode.held);
		}
		else if (!controller.canAttack)
		{
			if (preGrabAction != actionMode.docking && preGrabAction != actionMode.sitting && preGrabAction != actionMode.held)
			{
				SetActionMode(preGrabAction);
			}
			else
			{
				SetBehaviorMode(behaviorMode.ambient);
			}
		}
	}
}
