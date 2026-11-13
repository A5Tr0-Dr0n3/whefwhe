using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VVRConsumer : MonoBehaviour
{
	public delegate void OneShotEventHandler(bool isSucking, VVRConsumable consumable);

	public float suckRate = -10f;

	public float spewRate = 5f;

	public float restPower = 0.5f;

	public float activePower = 10f;

	public float restRange = 0.5f;

	public float activeRange = 1f;

	public int shaderPriority = 1;

	public bool useGlobalFillValue;

	[SerializeField]
	private float _fill;

	public float capacity = 300f;

	[HideInInspector]
	public VVRConsumable specifiedConsumable;

	public float _mouthRadius = 0.5f;

	public Transform mouthTransform;

	public SphereCollider _nearFoodTrigger;

	public List<VVRConsumable> _nearbyEdibles = new List<VVRConsumable>(5);

	private List<VVRConsumable> _inMouth = new List<VVRConsumable>(5);

	[HideInInspector]
	public bool isSucking = true;

	[HideInInspector]
	public bool isTransfering;

	private const int MAX_BUFFER = 32;

	private Collider[] consumableColliders;

	private VVRConsumable consumable;

	private bool oneShotting;

	private Coroutine vacuumRoutine;

	private float oneShotTimer;

	private float oneShotThreshold;

	private bool firstShot = true;

	private float mouthTimer;

	[HideInInspector]
	public int cnt;

	private float squareToMouth;

	private float bestSquareToMouth;

	private float edibleSquareToMouth;

	private float fillableSquareToMouth;

	private float bestGlimmerSquare;

	private float glimmerSquare;

	private VVRConsumable tastiest;

	private VVRConsumable bestBest;

	private VVRConsumable bestEdible;

	private VVRConsumable bestFillable;

	private VVRConsumable toEat;

	private Collider bestCol;

	private Collider edibleCol;

	private Collider fillableCol;

	private VVRConsumable glimmerConsumable;

	public ToolGroup tg;

	private Vector3 closestPoint;

	private VVRConsumable currentEdible;

	public float vacuumRate
	{
		get
		{
			if (isSucking)
			{
				return suckRate;
			}
			return spewRate;
		}
	}

	public float vacuumPower
	{
		get
		{
			if (isTransfering)
			{
				return activePower;
			}
			return restPower;
		}
	}

	public float vacuumRange
	{
		get
		{
			if (isTransfering)
			{
				return activeRange;
			}
			return restRange;
		}
	}

	public float fill
	{
		get
		{
			if (useGlobalFillValue)
			{
				return PersistentDataManager.GetSavedVacuumValue();
			}
			return _fill;
		}
		set
		{
			if (useGlobalFillValue)
			{
				PersistentDataManager.OverwriteVacuumValue(value);
			}
			else
			{
				_fill = value;
			}
		}
	}

	public event OneShotEventHandler OneShotEvent;

	private void Awake()
	{
		if (mouthTransform == null)
		{
			mouthTransform = transform;
		}
	}

	public void StartTransfer(bool suckBool, bool oneShot = false)
	{
		isSucking = suckBool;
		oneShotting = oneShot;
		isTransfering = true;
		specifiedConsumable = null;
		if (vacuumRoutine == null)
		{
			vacuumRoutine = StartCoroutine(IsVacuuming());
		}
	}

	public void StartTransfer(bool suckBool, VVRConsumable specConsumable, bool oneShot = false)
	{
		specifiedConsumable = specConsumable;
		isSucking = suckBool;
		oneShotting = oneShot;
		isTransfering = true;
		if (vacuumRoutine == null)
		{
			vacuumRoutine = StartCoroutine(IsVacuuming());
		}
	}

	public void StopTransfer()
	{
		isTransfering = false;
		oneShotting = false;
		specifiedConsumable = null;
	}

	private IEnumerator IsVacuuming()
	{
		mouthTimer = 999f;
		oneShotTimer = 10f;
		oneShotThreshold = 0f;
		firstShot = true;
		while (isTransfering)
		{
			if (oneShotting)
			{
				oneShotTimer += Time.deltaTime;
				if (oneShotTimer > oneShotThreshold)
				{
					CheckMouthOneShot();
					oneShotTimer = 0f;
					if (firstShot)
					{
						firstShot = false;
						oneShotThreshold = 0.3f;
					}
					else
					{
						oneShotThreshold = Random.Range(0.02f, 0.1f);
					}
				}
			}
			else
			{
				CheckMouth();
			}
			yield return null;
		}
		vacuumRoutine = null;
	}

	private void CheckMouth()
	{
		mouthTimer += Time.deltaTime;
		if (mouthTimer > 0.2f)
		{
			if (consumableColliders == null)
			{
				consumableColliders = new Collider[32];
			}
			cnt = Physics.OverlapSphereNonAlloc(mouthTransform.position, _mouthRadius, consumableColliders, EagerMaster.instance.edibleFillableMask);
			mouthTimer = Random.Range(0f, 0.05f);
			_inMouth.Clear();
			for (int i = 0; i < cnt; i++)
			{
				consumable = consumableColliders[i].GetComponentInParent<VVRConsumable>();
				if (consumable != null && consumable.gameObject != gameObject)
				{
					_inMouth.Add(consumable);
				}
			}
		}
		if ((bool)specifiedConsumable)
		{
			for (int j = 0; j < _inMouth.Count; j++)
			{
				consumable = _inMouth[j];
				if (consumable == specifiedConsumable)
				{
					consumable.Transfer(this, vacuumRate);
				}
			}
			return;
		}
		for (int k = 0; k < _inMouth.Count; k++)
		{
			consumable = _inMouth[k];
			if (consumable != null)
			{
				consumable.Transfer(this, vacuumRate);
			}
		}
	}

	private void CheckMouthOneShot()
	{
		toEat = FindTastiest();
		if (toEat != null)
		{
			if (toEat.OneShotTransfer(this, vacuumRate) && OneShotEvent != null)
			{
				OneShotEvent(isSucking, toEat);
			}
			toEat = null;
		}
	}

	public VVRConsumable FindTastiest()
	{
		tastiest = null;
		bestSquareToMouth = 99999f;
		bestBest = null;
		edibleSquareToMouth = 99999f;
		bestEdible = null;
		fillableSquareToMouth = 99999f;
		bestFillable = null;
		if (consumableColliders == null)
		{
			consumableColliders = new Collider[32];
		}
		LayerMask layerMask = EagerMaster.instance.edibleFillableMask;
		if ((bool)tg)
		{
			switch (tg.EagerTouch.CurrentVacInputStatus())
			{
			case EagerTouch.VacInputStatus.PreviewIn:
			case EagerTouch.VacInputStatus.In:
				layerMask = EagerMaster.instance.edibleOnlyMask;
				break;
			case EagerTouch.VacInputStatus.previewOut:
			case EagerTouch.VacInputStatus.Out:
				layerMask = EagerMaster.instance.fillableOnlyMask;
				break;
			}
		}
		cnt = Physics.OverlapSphereNonAlloc(mouthTransform.position, _mouthRadius, consumableColliders, layerMask);
		for (int i = 0; i < cnt; i++)
		{
			consumable = consumableColliders[i].GetComponentInParent<VVRConsumable>();
			if (consumable != null && !consumable.dontStartNewTransfer && !consumable.unconsumable && consumable.gameObject.activeInHierarchy)
			{
				closestPoint = consumableColliders[i].ClosestPointOnBounds(mouthTransform.position);
				squareToMouth = Vector3.SqrMagnitude(closestPoint - mouthTransform.position);
				if (squareToMouth < bestSquareToMouth)
				{
					bestSquareToMouth = squareToMouth;
					bestBest = consumable;
					bestCol = consumableColliders[i];
				}
				if (squareToMouth < edibleSquareToMouth && consumable.currentVacuumValueCapped > 0.01f)
				{
					edibleSquareToMouth = squareToMouth;
					bestEdible = consumable;
					edibleCol = consumableColliders[i];
				}
				if (squareToMouth < fillableSquareToMouth && consumable.currentVacuumLerp < 1f)
				{
					fillableSquareToMouth = squareToMouth;
					bestFillable = consumable;
					fillableCol = consumableColliders[i];
				}
			}
		}
		if ((bool)tg)
		{
			switch (tg.EagerTouch.CurrentVacInputStatus())
			{
			case EagerTouch.VacInputStatus.Off:
				tastiest = bestBest;
				tg.VLManager.closestCol = bestCol;
				break;
			case EagerTouch.VacInputStatus.PreviewIn:
				tastiest = ((!(bestEdible != null)) ? bestBest : bestEdible);
				tg.VLManager.closestCol = ((!(bestEdible != null)) ? bestCol : edibleCol);
				break;
			case EagerTouch.VacInputStatus.previewOut:
				tastiest = ((!(bestFillable != null)) ? bestBest : bestFillable);
				tg.VLManager.closestCol = ((!(bestFillable != null)) ? bestCol : fillableCol);
				break;
			case EagerTouch.VacInputStatus.Out:
			case EagerTouch.VacInputStatus.In:
			{
				tastiest = ((!isSucking) ? bestFillable : bestEdible);
				tg.VLManager.closestCol = ((!isSucking) ? fillableCol : edibleCol);
				if (!(tastiest == null))
				{
					break;
				}
				bestGlimmerSquare = 99999f;
				if (isSucking)
				{
					for (int j = 0; j < tg.Manipulator.nResults; j++)
					{
						glimmerConsumable = tg.Manipulator.castResults[j].collider.GetComponentInParent<VVRConsumable>();
						if (!(glimmerConsumable == null) && glimmerConsumable.currentObjectState != VVRConsumable.objectState.fullyConsumedGlimmer && glimmerConsumable.currentObjectState != VVRConsumable.objectState.fullyConsumedStatic && !glimmerConsumable.dontStartNewTransfer && !glimmerConsumable.unconsumable)
						{
							glimmerSquare = Vector3.SqrMagnitude(glimmerConsumable.transform.position - transform.position);
							if (glimmerSquare < bestGlimmerSquare)
							{
								bestGlimmerSquare = glimmerSquare;
								tastiest = glimmerConsumable;
							}
						}
					}
					break;
				}
				for (int k = 0; k < tg.Manipulator.nResults; k++)
				{
					glimmerConsumable = tg.Manipulator.castResults[k].collider.GetComponentInParent<VVRConsumable>();
					if (!(glimmerConsumable == null) && (glimmerConsumable.currentObjectState == VVRConsumable.objectState.partiallyConsumed || glimmerConsumable.currentObjectState == VVRConsumable.objectState.fullyConsumedGlimmer || glimmerConsumable.currentObjectState == VVRConsumable.objectState.fullyConsumedStatic) && !glimmerConsumable.unconsumable)
					{
						glimmerSquare = Vector3.SqrMagnitude(glimmerConsumable.transform.position - transform.position);
						if (glimmerSquare < bestGlimmerSquare)
						{
							bestGlimmerSquare = glimmerSquare;
							tastiest = glimmerConsumable;
						}
					}
				}
				break;
			}
			}
		}
		return tastiest;
	}

	private void OnTriggerEnter(Collider col)
	{
		currentEdible = col.GetComponentInParent<VVRConsumable>();
		if (currentEdible != null && !_nearbyEdibles.Contains(currentEdible))
		{
			currentEdible.onEnterVacuumRange(this);
			_nearbyEdibles.Add(currentEdible);
		}
	}

	private void OnTriggerExit(Collider col)
	{
		currentEdible = col.GetComponentInParent<VVRConsumable>();
		if (currentEdible != null && _nearbyEdibles.Contains(currentEdible))
		{
			currentEdible.onExitVacuumRange(this);
			_nearbyEdibles.Remove(currentEdible);
		}
	}

	public void removeFromEdibleList(VVRConsumable edible)
	{
		if (_nearbyEdibles.Remove(edible))
		{
			cnt--;
		}
	}

	private void OnDisable()
	{
		if (vacuumRoutine != null)
		{
			StopCoroutine(vacuumRoutine);
			vacuumRoutine = null;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		if ((bool)mouthTransform)
		{
			Gizmos.DrawWireSphere(mouthTransform.position, _mouthRadius);
		}
	}
}
