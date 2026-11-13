using System;
using UnityEngine;

public class HernandezHelperBody : MonoBehaviour
{
	[Serializable]
	public class C4Trigger
	{
		public Transform transform;

		public bool armed = true;

		public float radius = 0.5f;

		public bool fired { get; private set; }

		public bool CheckTrigger(Vector3 testPos)
		{
			if (armed && !fired && Vector3.Distance(testPos, transform.position) < radius)
			{
				fired = true;
				return true;
			}
			return false;
		}
	}

	[SerializeField]
	private Transform[] path;

	[SerializeField]
	private Rigidbody meshPivot;

	[SerializeField]
	private Transform bottomPoint;

	[SerializeField]
	private SpringJoint bodySpring;

	[SerializeField]
	private ObjectBase body;

	public bool shouldMove = true;

	[SerializeField]
	private bool looping;

	[SerializeField]
	private float speedMag = 3f;

	[SerializeField]
	private float floatAboveGroundHeight = 0.8f;

	[SerializeField]
	private float directionSmoothing = 4f;

	[SerializeField]
	private float nodeHitDist = 0.08f;

	[SerializeField]
	private float tolerantNodeHitDist = 3f;

	private Rigidbody mRigidbody;

	private Transform lookAtOverride;

	private int targetPathIndex;

	private Vector3 dir;

	private bool useTolerantNodeHitDist;

	private Transform[] defaultPath;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(bottomPoint.position, 0.2f);
		Gizmos.DrawLine(transform.position, bottomPoint.position);
		DrawPath(path, looping, this);
	}

	public void DrawTrigger(C4Trigger trigger)
	{
		if (trigger.transform != null)
		{
			Gizmos.color = ((!trigger.fired) ? new Color(0.4f, 0.8f, 0.2f) : new Color(0.8f, 0.4f, 0.2f));
			Gizmos.DrawWireSphere(trigger.transform.position, trigger.radius);
		}
	}

	public static void DrawPath(Transform[] pointsIn, bool loopIn, HernandezHelperBody refBody = null)
	{
	}

	private void Start()
	{
		mRigidbody = GetComponent<Rigidbody>();
		bodySpring.connectedAnchor = new Vector3(0f, floatAboveGroundHeight, 0f);
		defaultPath = path;
	}

	private void FixedUpdate()
	{
		moveAlongPath();
		manageBottomPoint();
		manageMeshPivot();
	}

	private void moveAlongPath()
	{
		if (getCanMove())
		{
			if (targetPathIndex >= path.Length)
			{
				return;
			}
			if (Vector3.Distance(transform.position.Flatten(), path[targetPathIndex].position.Flatten()) < ((!useTolerantNodeHitDist) ? nodeHitDist : tolerantNodeHitDist))
			{
				targetPathIndex++;
				if (looping && targetPathIndex >= path.Length)
				{
					targetPathIndex = 0;
				}
			}
			if (targetPathIndex < path.Length)
			{
				Vector3 normalized = (path[targetPathIndex].position - transform.position).normalized;
				dir = Vector3.Slerp(dir, normalized, Time.fixedDeltaTime * directionSmoothing);
				mRigidbody.velocity = speedMag * dir;
			}
			else
			{
				mRigidbody.velocity = Vector3.zero;
			}
		}
		else
		{
			mRigidbody.velocity = Vector3.zero;
		}
	}

	private bool getCanMove()
	{
		return shouldMove && !ToolManager.PlayerGrabbingObjectBase(body);
	}

	private void manageMeshPivot()
	{
		if (lookAtOverride == null)
		{
			if (dir.Flatten().normalized.magnitude != 0f)
			{
				meshPivot.transform.rotation = Quaternion.LookRotation(dir.Flatten().normalized);
			}
		}
		else
		{
			meshPivot.transform.rotation = Quaternion.LookRotation((lookAtOverride.position - meshPivot.transform.position).normalized);
		}
		meshPivot.transform.position = new Vector3(transform.position.x, meshPivot.position.y, transform.position.z);
	}

	private void manageBottomPoint()
	{
		RaycastHit hitInfo = default;
		if (Physics.Raycast(meshPivot.position, Vector3.down, out hitInfo))
		{
			bottomPoint.transform.position = hitInfo.point;
		}
	}

	public void SetDefaultPath(Transform[] newDefault)
	{
		defaultPath = newDefault;
	}

	public void RevertToDefaultPath()
	{
		OverridePath(defaultPath, newLooping: true);
	}

	public void OverridePath(Transform[] newPath, bool newLooping)
	{
		path = newPath;
		looping = newLooping;
		targetPathIndex = 0;
	}

	public void SetLookAtTarget(Transform newTarget)
	{
		lookAtOverride = newTarget;
	}

	public Vector3 GetMeshForward()
	{
		return meshPivot.transform.forward;
	}

	public void AddBobForce(Vector3 force)
	{
		meshPivot.AddForce(force);
	}

	public void UseTolerantTargetMode(bool tolerant)
	{
		useTolerantNodeHitDist = tolerant;
	}
}
