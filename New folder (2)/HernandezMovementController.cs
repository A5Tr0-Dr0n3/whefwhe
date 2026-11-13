using UnityEngine;

public class HernandezMovementController : MonoBehaviour
{
	public delegate void HernandezMovementCallback();

	private enum WalkMode
	{
		stopOnArrive,
		pace,
		stopped,
		jumpingJacks,
		restorationFollowPlayer
	}

	[SerializeField]
	private Transform officeStandingPoint;

	[SerializeField]
	private Transform officeMemoPacingPointA;

	[SerializeField]
	private Transform officeMemoPacingPointB;

	[SerializeField]
	private Transform gymStandingPoint;

	[SerializeField]
	private Transform gymLiftingPoint;

	[SerializeField]
	private Transform[] restorationPlayerFollowPoints;

	[SerializeField]
	private Renderer footRenderer;

	[SerializeField]
	private float walkSpeed = 3f;

	[SerializeField]
	private float slowWalkSpeed = 3f;

	[SerializeField]
	private float lookSpeed = 10f;

	[SerializeField]
	private LayerMask floorMask;

	[SerializeField]
	private AnimationCurve walkingBounce;

	[SerializeField]
	private float walkingBounceHeight = 0.5f;

	[SerializeField]
	private float slowWalkingBounceHeight = 0.35f;

	[SerializeField]
	private float bouncesPerUnitWalked = 1f;

	[SerializeField]
	private float jumpingJackSpeed = 3f;

	private HernandezMovementCallback arrivedAtTargetCallback;

	private HernandezMovementCallback jumpingJackPeakCallback;

	private const float lookForPlayerForwardOffset = 1f;

	private const float lookForPlayerStopRadius = 1.5f;

	private Transform lookAtTarget;

	private Vector3 destination;

	private Vector3[] pacingPoints;

	private Vector3 target;

	private WalkMode mWalkMode = WalkMode.stopped;

	private int paceTargetIndex;

	private int followPlayerTargetIndex;

	private int followPlayerNextIndex;

	private int lastVisitedFollowIndex;

	private Vector3 xzVelocity;

	private float bounceTravelCounter;

	private bool slowWalking;

	private Transform dummyLookAtTarget;

	private float oldBounceCurveScalar;

	private Rigidbody mRigidbody;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(officeStandingPoint.position, 0.2f);
		Gizmos.DrawWireSphere(officeMemoPacingPointA.position, 0.2f);
		Gizmos.DrawWireSphere(officeMemoPacingPointB.position, 0.2f);
		Gizmos.DrawWireSphere(gymStandingPoint.position, 0.2f);
		Gizmos.DrawWireSphere(gymLiftingPoint.position, 0.2f);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(restorationPlayerFollowPoints[lastVisitedFollowIndex].position, 0.25f);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(restorationPlayerFollowPoints[followPlayerNextIndex].position, 0.25f);
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(restorationPlayerFollowPoints[followPlayerTargetIndex].position, 0.25f);
		HernandezHelperBody.DrawPath(restorationPlayerFollowPoints, loopIn: true);
	}

	private void Awake()
	{
		mRigidbody = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = mRigidbody.velocity.Flatten();
		if (vector2.magnitude >= 0.01f)
		{
			vector = vector2;
		}
		else if (lookAtTarget != null)
		{
			vector = (lookAtTarget.position - transform.position).Flatten().normalized;
		}
		if (vector != Vector3.zero)
		{
			float t = lookSpeed * Time.deltaTime;
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(vector, Vector3.up), t);
		}
	}

	private void FixedUpdate()
	{
		Vector3 vector = transform.forward;
		if (mWalkMode != WalkMode.stopped && mWalkMode != WalkMode.jumpingJacks)
		{
			target = Vector3.zero;
			if (mWalkMode == WalkMode.stopOnArrive)
			{
				target = destination;
			}
			else if (mWalkMode == WalkMode.pace)
			{
				paceTargetIndex %= pacingPoints.Length;
				target = pacingPoints[paceTargetIndex];
			}
			else if (mWalkMode == WalkMode.restorationFollowPlayer)
			{
				target = getClosestFollowPointToPlayer();
			}
			vector = (target - transform.position).Flatten().normalized;
			xzVelocity = vector * ((!slowWalking) ? walkSpeed : slowWalkSpeed);
		}
		else
		{
			xzVelocity = Vector3.zero;
		}
		if (mWalkMode == WalkMode.stopOnArrive && Vector3.Distance(transform.position.Flatten(), destination.Flatten()) <= 0.1f)
		{
			if (arrivedAtTargetCallback != null)
			{
				arrivedAtTargetCallback();
				arrivedAtTargetCallback = null;
			}
			mWalkMode = WalkMode.stopped;
		}
		if (mWalkMode == WalkMode.pace && Vector3.Distance(transform.position.Flatten(), pacingPoints[paceTargetIndex].Flatten()) <= 0.1f)
		{
			paceTargetIndex = (paceTargetIndex + 1) % pacingPoints.Length;
		}
		if (mWalkMode == WalkMode.restorationFollowPlayer)
		{
			float num = Vector3.Distance(transform.position.Flatten(), restorationPlayerFollowPoints[followPlayerNextIndex].position.Flatten());
			if (num <= 0.1f)
			{
				lastVisitedFollowIndex = followPlayerNextIndex;
				if (followPlayerNextIndex == followPlayerTargetIndex)
				{
					xzVelocity = Vector3.zero;
				}
			}
		}
		if (mWalkMode == WalkMode.jumpingJacks)
		{
			tickBounceTravel(loop: true);
		}
		else if (xzVelocity.magnitude != 0f)
		{
			adjustBounceTravelByXZMovement();
		}
		else
		{
			tickBounceTravel(loop: false);
		}
		Vector3 vector2 = ((!isBlockedByPlayer(vector)) ? xzVelocity : Vector3.zero);
		mRigidbody.velocity = new Vector3(vector2.x, getBouncedHeightVelocity(), vector2.z);
	}

	private bool isBlockedByPlayer(Vector3 dirToTarget)
	{
		Vector3 vec = transform.position + dirToTarget * 1f;
		return 1.5f > Vector3.Distance(EagerMaster.instance.mainCamera.transform.position.Flatten(), vec.Flatten());
	}

	private void adjustBounceTravelByXZMovement()
	{
		bounceTravelCounter += xzVelocity.magnitude * Time.fixedDeltaTime;
		while (bounceTravelCounter > bouncesPerUnitWalked)
		{
			bounceTravelCounter -= bouncesPerUnitWalked;
		}
	}

	private void tickBounceTravel(bool loop)
	{
		if (loop)
		{
			bounceTravelCounter += Time.deltaTime * jumpingJackSpeed;
			while (bounceTravelCounter > bouncesPerUnitWalked)
			{
				bounceTravelCounter -= bouncesPerUnitWalked;
			}
			return;
		}
		if (bounceTravelCounter > bouncesPerUnitWalked / 2f)
		{
			bounceTravelCounter += Time.deltaTime * jumpingJackSpeed;
		}
		else
		{
			bounceTravelCounter -= Time.deltaTime * jumpingJackSpeed;
		}
		bounceTravelCounter = Mathf.Clamp(bounceTravelCounter, 0f, bouncesPerUnitWalked);
	}

	private float getBouncedHeightVelocity()
	{
		RaycastHit hitInfo = default;
		if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, 9f, floorMask.value))
		{
			float num = hitInfo.point.y + (transform.position.y - footRenderer.bounds.min.y);
			float time = bounceTravelCounter / bouncesPerUnitWalked;
			float num2 = ((!slowWalking) ? walkingBounceHeight : slowWalkingBounceHeight);
			float num3 = walkingBounce.Evaluate(time);
			if (mWalkMode == WalkMode.jumpingJacks && jumpingJackPeakCallback != null && oldBounceCurveScalar < 0.35f && num3 >= 0.35f)
			{
				jumpingJackPeakCallback();
			}
			oldBounceCurveScalar = num3;
			float num4 = num + num2 * num3;
			return (num4 - transform.position.y) / Time.fixedDeltaTime;
		}
		return 0f;
	}

	private int getClosestFollowPointToMe()
	{
		float num = 999f;
		int result = 0;
		for (int i = 0; i < restorationPlayerFollowPoints.Length; i++)
		{
			float num2 = Vector3.Distance(restorationPlayerFollowPoints[i].position.Flatten(), transform.position.Flatten());
			if (num > num2)
			{
				num = num2;
				result = i;
			}
		}
		return result;
	}

	private Vector3 getClosestFollowPointToPlayer()
	{
		float num = 999f;
		for (int i = 0; i < restorationPlayerFollowPoints.Length; i++)
		{
			float num2 = Vector3.Distance(restorationPlayerFollowPoints[i].position.Flatten(), EagerMaster.instance.mainCamera.transform.position.Flatten());
			if (num > num2)
			{
				num = num2;
				followPlayerTargetIndex = i;
			}
		}
		if (followPlayerTargetIndex == followPlayerNextIndex)
		{
			return restorationPlayerFollowPoints[followPlayerNextIndex].position;
		}
		int num3 = 0;
		int num4 = 0;
		int num5 = lastVisitedFollowIndex;
		while (num5 != followPlayerTargetIndex)
		{
			num5++;
			num5 %= restorationPlayerFollowPoints.Length;
			num3++;
		}
		num5 = lastVisitedFollowIndex;
		while (num5 != followPlayerTargetIndex)
		{
			num5--;
			if (num5 < 0)
			{
				num5 = restorationPlayerFollowPoints.Length - 1;
			}
			num4++;
		}
		bool flag = num3 <= num4;
		followPlayerNextIndex = lastVisitedFollowIndex + (flag ? 1 : (-1));
		if (followPlayerNextIndex < 0)
		{
			followPlayerNextIndex = restorationPlayerFollowPoints.Length - 1;
		}
		if (followPlayerNextIndex >= restorationPlayerFollowPoints.Length)
		{
			followPlayerNextIndex = 0;
		}
		return restorationPlayerFollowPoints[followPlayerNextIndex].position;
	}

	public void PaceInOffice()
	{
		slowWalking = true;
		mWalkMode = WalkMode.pace;
		pacingPoints = new Vector3[2] { officeMemoPacingPointA.position, officeMemoPacingPointB.position };
	}

	public void DoJumpingJacks(HernandezMovementCallback callback)
	{
		jumpingJackPeakCallback = callback;
		mWalkMode = WalkMode.jumpingJacks;
	}

	public void WalkToGym(HernandezMovementCallback callback)
	{
		slowWalking = false;
		mWalkMode = WalkMode.stopOnArrive;
		arrivedAtTargetCallback = callback;
		destination = gymStandingPoint.position;
	}

	public void WalkToLiftPoint()
	{
		slowWalking = false;
		mWalkMode = WalkMode.stopOnArrive;
		destination = gymLiftingPoint.position;
	}

	public void StopWalking()
	{
		mWalkMode = WalkMode.stopped;
	}

	public void SetLookAtTarget(Transform targetIn)
	{
		lookAtTarget = targetIn;
	}

	public void TurnForAside(float angle)
	{
		if (dummyLookAtTarget != null)
		{
			Object.Destroy(dummyLookAtTarget.gameObject);
		}
		dummyLookAtTarget = new GameObject().transform;
		dummyLookAtTarget.name = "Hernandez Dummy LookAtTarget";
		dummyLookAtTarget.position = transform.position + Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
		dummyLookAtTarget.parent = transform.parent.transform;
		SetLookAtTarget(dummyLookAtTarget);
	}

	public void FollowPlayer()
	{
		slowWalking = false;
		mWalkMode = WalkMode.restorationFollowPlayer;
		getClosestFollowPointToPlayer();
		lastVisitedFollowIndex = Mathf.Max(getClosestFollowPointToMe() - 1, 0);
		followPlayerNextIndex = getClosestFollowPointToMe();
		followPlayerTargetIndex = getClosestFollowPointToMe();
	}
}
