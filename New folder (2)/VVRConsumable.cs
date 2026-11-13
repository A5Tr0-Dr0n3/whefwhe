using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using UnityEngine;

public class VVRConsumable : MonoBehaviour
{
	public delegate void OnVacuumedUpEventHandler(VVRConsumable consumedObj);

	public enum objectState
	{
		wholeStatic,
		wholeShaking,
		partiallyConsumed,
		fullyConsumedStatic,
		fullyConsumedGlimmer
	}

	private Material sharedStaticMat;

	public bool singleConsumer;

	public bool boidsIgnore;

	public Mesh lowPolyMesh;

	public Mesh highPolyMesh;

	private MeshFilter mFilter;

	private Material currentMaterial;

	private float seed = 1f;

	private bool originalKinematic;

	private int originalLayer;

	private static int seedPropID;

	private static int suckPropID;

	private static int vacPowerPropID;

	private static int vacRangePropID;

	private static int vacPosPropID;

	private static int glimmerTransPropID;

	private float fullMass;

	public List<VVRConsumable> linkedConsumables = new List<VVRConsumable>();

	[HideInInspector]
	public bool dontStartNewTransfer;

	[HideInInspector]
	public Rigidbody rb;

	[HideInInspector]
	public VVRConsumer mainAffector;

	[Space]
	[Header("Vac Visual FX")]
	public float vacuumEffectScalar = 1f;

	[HideInInspector]
	public List<VVRConsumer> consumerList = new List<VVRConsumer>(5);

	[Space]
	[Header("Vac Values")]
	public float totalVacuumValue;

	public float partialStartPercent;

	[Space]
	[Header("Vac Bools")]
	public bool destroyOnConsume;

	public bool unconsumable;

	public bool isBeingAffectedByVacuum;

	public bool usePhysicsVacuum;

	private float _currentVacuumValue;

	private float internalVacPower;

	private float internalVacRange = 1f;

	private bool initialized;

	private bool stateInitialized;

	private MaterialPropertyBlock block;

	private WaitForSeconds wfs;

	private float mainConsumerYield;

	private SafeCoroutine vacPropCoroutine;

	private SafeCoroutine oneShotCoroutine;

	private VVRConsumer tempConsumer;

	private VVRConsumer highConsumer;

	private float consumerScore;

	private float highConsumerScore;

	private int numberToCheck;

	private float currentFrameTransfer;

	private float startSpewPower;

	private bool drainingTheDregs;

	private float roundedValue;

	private float roundedFill;

	private float distanceFromVac;

	private float oneShotDuration;

	private float lastEmptyTime;

	private float emptyWaitThreshold = -1f;

	private bool toppingItOff;

	private SafeCoroutine animationUpdateCoroutine;

	private float totalAnimationDelta;

	private float animationProgress;

	private float animationFrameData;

	private bool keepAnimating;

	public objectState currentObjectState;

	private Material _vacMat;

	private Material _ghostMat;

	private bool ghostStatus;

	private const float glimmerTransDur = 0.75f;

	private bool glimmerTransActive;

	private bool glimmerFullActive;

	private bool inGlimmerRange;

	private float glimmerTransAmount;

	private bool shouldGlimmer;

	private SafeCoroutine glimmerTransRoutine;

	private float direction;

	public Renderer oRenderer { get; private set; }

	public float currentVacuumValueCapped
	{
		get
		{
			return _currentVacuumValue;
		}
		set
		{
			_currentVacuumValue = Mathf.Clamp(value, 0f, totalVacuumValue);
		}
	}

	public float currentVacuumLerp => Mathf.Clamp01(currentVacuumValueCapped * 2f / totalVacuumValue);

	private Material VacMat
	{
		get
		{
			if ((bool)_vacMat)
			{
				return _vacMat;
			}
			_vacMat = MaterialManager.GetMaterialVariation(sharedStaticMat, MaterialVariations.Vacuum);
			_vacMat = new Material(_vacMat);
			return _vacMat;
		}
		set
		{
			_vacMat = value;
		}
	}

	private Material GhostMat
	{
		get
		{
			if ((bool)_ghostMat)
			{
				return _ghostMat;
			}
			_ghostMat = MaterialManager.GetMaterialVariation(sharedStaticMat, MaterialVariations.Ghost);
			return _ghostMat;
		}
		set
		{
			_ghostMat = value;
		}
	}

	public static event OnVacuumedUpEventHandler OnVacuumedUpEvent;

	private void Start()
	{
		initialize();
		initializeState(forceInitialize: false);
	}

	private void initialize()
	{
		if (!initialized)
		{
			initialized = true;
			rb = gameObject.GetComponentInChildren<Rigidbody>();
			if (totalVacuumValue == 0f)
			{
				totalVacuumValue = ((!(rb != null)) ? (15f + 25f * Random.value) : (15f + 5f * Random.value * rb.mass));
			}
			mFilter = GetComponent<MeshFilter>();
			if (lowPolyMesh == null && highPolyMesh != null)
			{
				lowPolyMesh = mFilter.sharedMesh;
			}
			block = new MaterialPropertyBlock();
			oRenderer = GetComponentInChildren<Renderer>();
			currentMaterial = oRenderer.sharedMaterial;
			sharedStaticMat = currentMaterial;
			originalLayer = gameObject.layer;
			if (seedPropID == 0)
			{
				updateMaterialPropIDs();
			}
			if (rb != null)
			{
				originalKinematic = rb.isKinematic;
			}
		}
	}

	private void initializeState(bool forceInitialize)
	{
		if (stateInitialized && !forceInitialize)
		{
			return;
		}
		stateInitialized = true;
		if (partialStartPercent > 0f && partialStartPercent < 1f)
		{
			currentVacuumValueCapped = partialStartPercent * totalVacuumValue;
			UpdatePartialMaterial(force: true);
			return;
		}
		if (currentObjectState == objectState.wholeStatic || currentObjectState == objectState.wholeShaking || currentObjectState == objectState.partiallyConsumed)
		{
			currentVacuumValueCapped = totalVacuumValue;
			currentObjectState = (isBeingAffectedByVacuum ? objectState.wholeShaking : objectState.wholeStatic);
		}
		else
		{
			currentVacuumValueCapped = 0f;
			currentObjectState = objectState.fullyConsumedStatic;
		}
		SetObjectState(currentObjectState, force: true);
	}

	public void onEnterVacuumRange(VVRConsumer consumer)
	{
		if (!consumerList.Contains(consumer))
		{
			consumerList.Add(consumer);
			isBeingAffectedByVacuum = true;
			if (currentObjectState == objectState.wholeStatic)
			{
				SetObjectState(objectState.wholeShaking);
			}
			else if (currentObjectState == objectState.fullyConsumedStatic)
			{
				SetObjectState(objectState.fullyConsumedGlimmer);
			}
			if ((currentObjectState == objectState.wholeShaking || currentObjectState == objectState.partiallyConsumed) && vacPropCoroutine == null)
			{
				vacPropCoroutine = GlobalCoroutineContainer.AddThread(UpdateVacuumProperties(), this);
			}
		}
	}

	public void onExitVacuumRange(VVRConsumer consumer)
	{
		if (!consumerList.Remove(consumer) || consumerList.Count >= 1)
		{
			return;
		}
		isBeingAffectedByVacuum = false;
		if (!dontStartNewTransfer)
		{
			mainAffector = null;
			block.SetVector(vacPosPropID, new Vector3(1000f, 1000f, 1000f));
			oRenderer.SetPropertyBlock(block);
			VacMat.SetVector(vacPosPropID, new Vector3(1000f, 1000f, 1000f));
			if (currentObjectState == objectState.wholeShaking)
			{
				SetObjectState(objectState.wholeStatic);
			}
		}
	}

	public void OnEnable()
	{
		if (consumerList.Count > 0 && (currentObjectState == objectState.wholeShaking || currentObjectState == objectState.partiallyConsumed) && vacPropCoroutine == null)
		{
			vacPropCoroutine = GlobalCoroutineContainer.AddThread(UpdateVacuumProperties(), this);
		}
	}

	private IEnumerator UpdateVacuumProperties()
	{
		while (isBeingAffectedByVacuum)
		{
			mainConsumerYield += Time.deltaTime;
			if (mainAffector == null || mainConsumerYield > 0.15f)
			{
				UpdateMainConsumer();
				mainConsumerYield = 0f;
			}
			if (unconsumable)
			{
				internalVacPower = Mathf.Lerp(internalVacPower, mainAffector.vacuumPower, 9f * Time.deltaTime);
				internalVacRange = Mathf.Lerp(internalVacRange, mainAffector.vacuumRange, 9f * Time.deltaTime);
				if (Mathf.Abs(mainAffector.vacuumPower - internalVacPower) < 0.01f)
				{
					internalVacPower = mainAffector.vacuumPower;
				}
				if (Mathf.Abs(mainAffector.vacuumRange - internalVacRange) < 0.01f)
				{
					internalVacRange = mainAffector.vacuumRange;
				}
			}
			else
			{
				internalVacPower = Mathf.Lerp(internalVacPower, mainAffector.vacuumPower, 9f * Time.deltaTime);
				internalVacRange = Mathf.Lerp(internalVacRange, mainAffector.vacuumRange, 9f * Time.deltaTime);
				if (Mathf.Abs(mainAffector.vacuumPower - internalVacPower) < 0.01f)
				{
					internalVacPower = mainAffector.vacuumPower;
				}
				if (Mathf.Abs(mainAffector.vacuumRange - internalVacRange) < 0.01f)
				{
					internalVacRange = mainAffector.vacuumRange;
				}
			}
			block.SetVector(vacPosPropID, mainAffector.mouthTransform.position);
			block.SetFloat(vacPowerPropID, internalVacPower * vacuumEffectScalar);
			block.SetFloat(vacRangePropID, internalVacRange);
			oRenderer.SetPropertyBlock(block);
			VacMat.SetVector(vacPosPropID, mainAffector.mouthTransform.position);
			VacMat.SetFloat(vacPowerPropID, internalVacPower * vacuumEffectScalar);
			VacMat.SetFloat(vacRangePropID, internalVacRange);
			yield return null;
		}
		vacPropCoroutine = null;
	}

	private void UpdateMainConsumer()
	{
		if (dontStartNewTransfer && mainAffector != null)
		{
			return;
		}
		highConsumer = null;
		highConsumerScore = -1f;
		numberToCheck = Mathf.Min(consumerList.Count, 12);
		if (singleConsumer)
		{
			mainAffector = ToolManager.BestBlackHoleConsumer(transform.position);
			return;
		}
		for (int i = 0; i < numberToCheck; i++)
		{
			tempConsumer = consumerList[i];
			consumerScore = 0f;
			if (tempConsumer.isTransfering)
			{
				if (tempConsumer.isSucking)
				{
					consumerScore += 3f;
				}
				else
				{
					consumerScore += 2f;
				}
			}
			consumerScore += 1f - Mathf.Clamp01((tempConsumer.mouthTransform.position - transform.position).sqrMagnitude * 0.01f);
			consumerScore += tempConsumer.shaderPriority;
			if (consumerScore > highConsumerScore)
			{
				highConsumerScore = consumerScore;
				highConsumer = tempConsumer;
			}
		}
		mainAffector = highConsumer;
	}

	public bool OneShotTransfer(VVRConsumer consumer, float rate, bool linked = true)
	{
		roundedValue = Mathf.Round((totalVacuumValue - currentVacuumValueCapped) * 10f) * 0.1f;
		roundedFill = Mathf.Round(consumer.fill * 10f) * 0.1f;
		if (unconsumable || rate == 0f || dontStartNewTransfer || (currentVacuumValueCapped < 0.1f && consumer.isSucking) || (currentVacuumValueCapped == totalVacuumValue && !consumer.isSucking))
		{
			return false;
		}
		if (roundedValue > roundedFill && !consumer.isSucking)
		{
			if (Time.time - lastEmptyTime > emptyWaitThreshold)
			{
				EagerSounds.instance.UI_Wrong.Play();
				lastEmptyTime = Time.time;
				emptyWaitThreshold = Random.Range(1f, 3f);
			}
			return false;
		}
		if (linkedConsumables.Count > 0 && linked)
		{
			for (int i = 0; i < linkedConsumables.Count; i++)
			{
				if (linkedConsumables[i] != null)
				{
					linkedConsumables[i].OneShotTransfer(consumer, rate, linked: false);
				}
			}
		}
		if (!consumer.isSucking)
		{
			consumer.fill = Mathf.Max(0f, roundedFill - roundedValue);
		}
		dontStartNewTransfer = true;
		mainAffector = consumer;
		distanceFromVac = Vector3.Distance(transform.position, consumer.transform.position);
		if (rate < 0f)
		{
			internalVacPower = mainAffector.activePower;
			internalVacRange = mainAffector.activeRange;
			oneShotDuration = 0.35f + 0.5f * (currentVacuumValueCapped / (0f - rate));
			drainingTheDregs = true;
			toppingItOff = false;
		}
		else
		{
			internalVacPower = distanceFromVac * 4f + mainAffector.activePower;
			internalVacRange = distanceFromVac * 12f + mainAffector.activeRange;
			oneShotDuration = 0.35f + 0.5f * (totalVacuumValue - currentVacuumValueCapped) / rate;
			toppingItOff = true;
			drainingTheDregs = false;
		}
		if (vacPropCoroutine != null)
		{
			vacPropCoroutine.Stop();
			vacPropCoroutine = null;
		}
		SetObjectState(objectState.partiallyConsumed);
		if (oneShotCoroutine == null)
		{
			oneShotCoroutine = GlobalCoroutineContainer.AddThread(UpdateOneShot(drainingTheDregs, oneShotDuration), this);
		}
		return true;
	}

	private IEnumerator UpdateOneShot(bool sucking, float duration)
	{
		float t = 0f;
		float lerp = 0f;
		bool animationTriggered = false;
		distanceFromVac = Mathf.Max(2f, distanceFromVac);
		bool stillAnimating = true;
		while (lerp < 1f || stillAnimating)
		{
			t += Time.deltaTime;
			lerp = Mathf.Clamp01(t / duration);
			if (sucking)
			{
				internalVacPower += 4f * distanceFromVac * Time.deltaTime;
				internalVacRange += 12f * distanceFromVac * Time.deltaTime;
				if (lerp >= 0.7f && !animationTriggered)
				{
					AnimateSuckUpToValue(0f, 0.3f * duration);
					animationTriggered = true;
					stillAnimating = false;
				}
			}
			else
			{
				if (!animationTriggered)
				{
					AnimateSuckUpToValue(totalVacuumValue, 0.3f * duration);
					animationTriggered = true;
				}
				internalVacPower = Mathf.Clamp(internalVacPower - 4f * distanceFromVac * Time.deltaTime, 0f, 2000f);
				internalVacRange = Mathf.Clamp(internalVacRange - 12f * distanceFromVac * Time.deltaTime, 0f, 2000f);
				if (internalVacPower <= 0.01f && internalVacRange <= 0.01f)
				{
					stillAnimating = false;
				}
			}
			block.SetVector(vacPosPropID, mainAffector.mouthTransform.position);
			block.SetFloat(vacPowerPropID, internalVacPower * vacuumEffectScalar);
			block.SetFloat(vacRangePropID, internalVacRange);
			oRenderer.SetPropertyBlock(block);
			VacMat.SetVector(vacPosPropID, mainAffector.mouthTransform.position);
			VacMat.SetFloat(vacPowerPropID, internalVacPower * vacuumEffectScalar);
			VacMat.SetFloat(vacRangePropID, internalVacRange);
			yield return null;
		}
		dontStartNewTransfer = false;
		oneShotCoroutine = null;
		if (isBeingAffectedByVacuum && vacPropCoroutine == null)
		{
			vacPropCoroutine = GlobalCoroutineContainer.AddThread(UpdateVacuumProperties(), this);
		}
		if (!isBeingAffectedByVacuum)
		{
			if (sucking)
			{
				SetObjectState(objectState.fullyConsumedStatic);
			}
			else
			{
				SetObjectState(objectState.wholeStatic);
			}
		}
	}

	public float Transfer(VVRConsumer consumer, float rate)
	{
		if (unconsumable || rate == 0f)
		{
			return 0f;
		}
		currentFrameTransfer = Time.deltaTime * rate;
		if (rate < 0f)
		{
			toppingItOff = false;
			if (currentObjectState == objectState.fullyConsumedStatic || currentObjectState == objectState.fullyConsumedGlimmer || consumer.capacity - consumer.fill < 1E-06f || drainingTheDregs)
			{
				return 0f;
			}
			currentFrameTransfer = Mathf.Min(currentFrameTransfer, currentVacuumValueCapped);
			currentFrameTransfer = Mathf.Min(currentFrameTransfer, consumer.capacity - consumer.fill);
		}
		else if (rate > 0f)
		{
			drainingTheDregs = false;
			if (currentObjectState == objectState.wholeStatic || currentObjectState == objectState.wholeShaking || consumer.fill < 1E-06f || toppingItOff)
			{
				return 0f;
			}
			currentFrameTransfer = Mathf.Min(currentFrameTransfer, totalVacuumValue - currentVacuumValueCapped);
			currentFrameTransfer = Mathf.Min(currentFrameTransfer, consumer.fill);
		}
		currentVacuumValueCapped += currentFrameTransfer;
		consumer.fill -= currentFrameTransfer;
		UpdatePartialMaterial();
		seed += Time.deltaTime * 0.5f;
		if (currentVacuumLerp < 0.25f && rate < 0f)
		{
			drainingTheDregs = true;
			AnimateSuckUpToValue(0f, totalVacuumValue * 0.15f / (Mathf.Abs(rate) * (float)(consumerList.Count + 1)));
		}
		else if (currentVacuumLerp > 0.75f && rate > 0f)
		{
			toppingItOff = true;
			AnimateSuckUpToValue(totalVacuumValue, totalVacuumValue * 0.15f / (Mathf.Abs(rate) * (float)(consumerList.Count + 1)));
		}
		return currentFrameTransfer;
	}

	private void UpdatePartialMaterial(bool force = false)
	{
		SetObjectState(objectState.partiallyConsumed, force);
		block.SetFloat(suckPropID, currentVacuumLerp);
		block.SetFloat(seedPropID, seed);
		oRenderer.SetPropertyBlock(block);
		VacMat.SetFloat(suckPropID, currentVacuumLerp);
		VacMat.SetFloat(seedPropID, seed);
	}

	public void AnimateSuckUpToValue(float toValue, float totalSeconds)
	{
		if (gameObject.activeInHierarchy)
		{
			toValue = Mathf.Clamp(toValue, 0f, totalVacuumValue);
			if (animationUpdateCoroutine != null)
			{
				animationUpdateCoroutine.Stop();
			}
			animationUpdateCoroutine = GlobalCoroutineContainer.AddThread(AnimationUpdate(toValue, totalSeconds), this);
		}
	}

	private IEnumerator AnimationUpdate(float toValue, float totalSeconds)
	{
		keepAnimating = true;
		animationProgress = 0f;
		totalAnimationDelta = toValue - currentVacuumValueCapped;
		while (keepAnimating)
		{
			animationProgress += Time.deltaTime;
			animationFrameData = ((!(animationProgress < totalSeconds)) ? (toValue - currentVacuumValueCapped) : (Time.deltaTime / totalSeconds * totalAnimationDelta));
			currentVacuumValueCapped += animationFrameData;
			if (animationFrameData < 0f && mainAffector != null)
			{
				mainAffector.fill -= animationFrameData;
			}
			seed += Time.deltaTime * 0.5f;
			UpdatePartialMaterial();
			if (totalVacuumValue - currentVacuumValueCapped < 0.01f)
			{
				keepAnimating = false;
				currentVacuumValueCapped = totalVacuumValue;
				if (isBeingAffectedByVacuum || dontStartNewTransfer)
				{
					SetObjectState(objectState.wholeShaking);
				}
				else
				{
					SetObjectState(objectState.wholeStatic);
				}
				toppingItOff = false;
			}
			else if (currentVacuumValueCapped < 0.01f)
			{
				keepAnimating = false;
				currentVacuumValueCapped = 0f;
				SetObjectState(objectState.fullyConsumedStatic);
				drainingTheDregs = false;
			}
			else if (Mathf.Abs(currentVacuumValueCapped - toValue) < 0.01f)
			{
				keepAnimating = false;
				currentVacuumValueCapped = toValue;
			}
			yield return null;
		}
		animationUpdateCoroutine = null;
	}

	public void LoadFromSaveData(ConsumableSaveData saveData)
	{
		if (saveData.consumableState == objectState.fullyConsumedGlimmer)
		{
			saveData.consumableState = objectState.fullyConsumedStatic;
		}
		if (saveData.consumableState == objectState.wholeShaking)
		{
			saveData.consumableState = objectState.wholeStatic;
		}
		currentObjectState = saveData.consumableState;
		partialStartPercent = saveData.fillPercent;
		initialize();
		initializeState(forceInitialize: true);
	}

	private void SetObjectState(objectState os, bool force = false)
	{
		initialize();
		if (os == currentObjectState && !force)
		{
			return;
		}
		switch (os)
		{
		case objectState.wholeStatic:
			oRenderer.enabled = true;
			glimmerTransActive = false;
			glimmerFullActive = false;
			if (highPolyMesh != null)
			{
				mFilter.sharedMesh = lowPolyMesh;
			}
			setObjectGhost(ghost: false);
			gameObject.transform.SetLayer(originalLayer);
			currentMaterial = sharedStaticMat;
			internalVacPower = 0f;
			internalVacRange = 0.5f;
			block.Clear();
			oRenderer.SetPropertyBlock(block);
			break;
		case objectState.wholeShaking:
			oRenderer.enabled = true;
			glimmerTransActive = false;
			glimmerFullActive = false;
			if (highPolyMesh != null)
			{
				mFilter.sharedMesh = highPolyMesh;
			}
			setObjectGhost(ghost: false);
			gameObject.transform.SetLayer(originalLayer);
			currentMaterial = VacMat;
			break;
		case objectState.partiallyConsumed:
			oRenderer.enabled = true;
			glimmerTransActive = false;
			glimmerFullActive = false;
			if ((bool)highPolyMesh)
			{
				mFilter.sharedMesh = highPolyMesh;
			}
			setObjectGhost(ghost: false);
			currentMaterial = VacMat;
			gameObject.transform.SetLayer(LayerMask.NameToLayer("PartiallyConsumed"));
			break;
		case objectState.fullyConsumedStatic:
			oRenderer.enabled = false;
			glimmerTransActive = false;
			glimmerFullActive = false;
			setObjectGhost(ghost: true);
			gameObject.transform.SetLayer(LayerMask.NameToLayer("Vacuumed"));
			glimmerTransAmount = 0f;
			glimmerTransActive = false;
			break;
		case objectState.fullyConsumedGlimmer:
			oRenderer.enabled = true;
			glimmerTransActive = true;
			glimmerFullActive = false;
			block.Clear();
			oRenderer.SetPropertyBlock(block);
			if (highPolyMesh != null)
			{
				mFilter.sharedMesh = highPolyMesh;
			}
			currentMaterial = GhostMat;
			setObjectGhost(ghost: true);
			gameObject.transform.SetLayer(LayerMask.NameToLayer("Vacuumed"));
			if (glimmerTransRoutine == null || !glimmerTransRoutine.IsRunning)
			{
				glimmerTransRoutine = GlobalCoroutineContainer.AddThread(UpdateGlimmerProperties(), this);
			}
			break;
		}
		oRenderer.sharedMaterial = currentMaterial;
		currentObjectState = os;
	}

	private void setObjectGhost(bool ghost)
	{
		if (ghost == ghostStatus)
		{
			return;
		}
		ghostStatus = ghost;
		if (ghost)
		{
			if (OnVacuumedUpEvent != null)
			{
				OnVacuumedUpEvent(this);
			}
			if (destroyOnConsume)
			{
				destroyThis();
			}
			if (rb != null)
			{
				rb.isKinematic = true;
			}
			currentVacuumValueCapped = 0f;
		}
		else if (rb != null)
		{
			rb.isKinematic = originalKinematic;
		}
	}

	private void destroyThis()
	{
		foreach (VVRConsumer consumer in consumerList)
		{
			consumer.removeFromEdibleList(this);
		}
		Object.Destroy(gameObject);
	}

	private void OnDisable()
	{
		if (vacPropCoroutine != null)
		{
		}
		if (animationUpdateCoroutine == null)
		{
		}
	}

	private void updateMaterialPropIDs()
	{
		seedPropID = Shader.PropertyToID("_Seed");
		suckPropID = Shader.PropertyToID("_SuckUp");
		vacPowerPropID = Shader.PropertyToID("_VacuumPower");
		vacPosPropID = Shader.PropertyToID("_VacuumPosition");
		vacRangePropID = Shader.PropertyToID("_VacuumRange");
		glimmerTransPropID = Shader.PropertyToID("_GlimmerTransAmount");
	}

	public void MarkInGlimmerRange()
	{
		inGlimmerRange = true;
		if (currentObjectState == objectState.fullyConsumedStatic)
		{
			SetObjectState(objectState.fullyConsumedGlimmer);
		}
	}

	private IEnumerator UpdateGlimmerProperties()
	{
		while (glimmerTransActive || glimmerFullActive)
		{
			shouldGlimmer = inGlimmerRange || isBeingAffectedByVacuum;
			inGlimmerRange = false;
			if (glimmerFullActive && !shouldGlimmer)
			{
				glimmerTransActive = true;
				glimmerFullActive = false;
			}
			if (glimmerTransActive)
			{
				direction = (shouldGlimmer ? 1 : (-1));
				glimmerTransAmount += Time.deltaTime / 0.75f * direction;
				if (shouldGlimmer && glimmerTransAmount >= 1f)
				{
					glimmerTransAmount = 1f;
					glimmerTransActive = false;
					glimmerFullActive = true;
				}
				else
				{
					if (!shouldGlimmer && glimmerTransAmount <= 0f)
					{
						glimmerTransAmount = 0f;
						glimmerTransActive = false;
						glimmerFullActive = false;
						SetObjectState(objectState.fullyConsumedStatic);
						break;
					}
					block.SetFloat(glimmerTransPropID, glimmerTransAmount);
					oRenderer.SetPropertyBlock(block);
				}
			}
			yield return null;
		}
		glimmerTransRoutine = null;
	}

	public void LatestUpdate()
	{
		if (currentObjectState != objectState.fullyConsumedGlimmer && currentObjectState != objectState.fullyConsumedStatic)
		{
			return;
		}
		shouldGlimmer = inGlimmerRange || isBeingAffectedByVacuum;
		inGlimmerRange = false;
		if (currentObjectState == objectState.fullyConsumedStatic && shouldGlimmer)
		{
			SetObjectState(objectState.fullyConsumedGlimmer);
			glimmerTransActive = true;
			block.Clear();
		}
		else if (currentObjectState == objectState.fullyConsumedGlimmer && !shouldGlimmer)
		{
			glimmerTransActive = true;
			block.Clear();
		}
		if (!glimmerTransActive)
		{
			return;
		}
		float num = (shouldGlimmer ? 1 : (-1));
		glimmerTransAmount += Time.deltaTime / 0.75f * num;
		if (shouldGlimmer && glimmerTransAmount >= 1f)
		{
			glimmerTransAmount = 1f;
			glimmerTransActive = false;
			block.Clear();
			oRenderer.SetPropertyBlock(block);
			return;
		}
		if (!shouldGlimmer && glimmerTransAmount <= 0f)
		{
			glimmerTransAmount = 0f;
			glimmerTransActive = false;
			SetObjectState(objectState.fullyConsumedStatic);
			return;
		}
		float num2 = glimmerTransAmount;
		if (num2 < 0.5f)
		{
			num2 *= 2f;
			num2 = Mathf.Pow(num2, 3f);
			num2 *= 0.5f;
		}
		else
		{
			num2 -= 0.5f;
			num2 *= 2f;
			num2 = 1f - Mathf.Pow(1f - num2, 3f);
			num2 *= 0.5f;
			num2 += 0.5f;
		}
		block.SetFloat(glimmerTransPropID, glimmerTransAmount);
		oRenderer.SetPropertyBlock(block);
	}
}
