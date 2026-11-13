using UnityEngine;
using UnityEngine.AI;

public class FurnitureNavigation : MonoBehaviour
{
	[Header("Scene References")]
	[SerializeField]
	private Transform homeTarget;

	[SerializeField]
	public Transform diningroomTarget;

	[SerializeField]
	public NavMeshAgent agent;

	[SerializeField]
	public Transform startParent;

	[SerializeField]
	private NavMeshObstacle carver;

	[SerializeField]
	public Transform laundryRoomPos;

	[SerializeField]
	private JobTina sceneRef;

	[SerializeField]
	private bool playerDoll;

	[SerializeField]
	internal bool neverUseCarver;

	internal NavFurnitureState currentState;

	private Rigidbody rb;

	private Transform mainSequenceTarget;

	private NavMeshPath navMeshPath;

	private int framesPathNotComplete;

	private static int FRAMESUNTILFORCEPATH = 25;

	private static float HITRADIUS = 0.25f;

	protected virtual void Awake()
	{
		if (agent == null)
		{
			agent = GetComponent<NavMeshAgent>();
		}
		if (carver == null)
		{
			carver = GetComponent<NavMeshObstacle>();
		}
		if (startParent == null)
		{
			startParent = transform.parent;
		}
		mainSequenceTarget = homeTarget;
		SetState(NavFurnitureState.Inactive, transform.localPosition, transform.localRotation);
	}

	protected virtual void Update()
	{
		Vector3 zero = Vector3.zero;
		if (currentState == NavFurnitureState.Active && agent.isOnNavMesh)
		{
			zero = homeTarget.position;
			agent.SetDestination(zero);
			if (carver != null && carver.enabled)
			{
				carver.enabled = false;
			}
		}
		else if (currentState == NavFurnitureState.DiningSequence && agent.isOnNavMesh)
		{
			if (diningroomTarget != null)
			{
				zero = diningroomTarget.position;
				agent.SetDestination(zero);
			}
			else
			{
				zero = homeTarget.position;
				agent.SetDestination(zero);
			}
			if (carver != null && carver.enabled)
			{
				carver.enabled = false;
			}
		}
		else if (currentState == NavFurnitureState.MainSequence && agent.isOnNavMesh)
		{
			zero = mainSequenceTarget.position;
			agent.SetDestination(zero);
			if (carver != null && carver.enabled)
			{
				carver.enabled = false;
			}
		}
		else if (currentState == NavFurnitureState.MainSequence)
		{
			if (agent.enabled)
			{
				agent.enabled = false;
			}
			if (!agent.enabled)
			{
				agent.enabled = true;
			}
		}
		if (agent.path.status != NavMeshPathStatus.PathComplete)
		{
			if (agent.enabled)
			{
				framesPathNotComplete++;
			}
		}
		else
		{
			framesPathNotComplete = 0;
		}
		if (framesPathNotComplete > 0 && agent.enabled)
		{
			sceneRef.navSyncer.AddAgentToNoPathList(this);
		}
		if ((double)DistanceFromAgentTarget() < 0.6 || !agent.enabled)
		{
			sceneRef.navSyncer.RemoveAgentFromNoPathList(this);
		}
	}

	public void SetState(NavFurnitureState s, Vector3 pos, Quaternion rot)
	{
		if (currentState == s)
		{
			return;
		}
		currentState = s;
		NavMeshHit hit;
		switch (currentState)
		{
		case NavFurnitureState.Active:
			transform.localPosition = pos;
			transform.localRotation = rot;
			if (!NavMesh.SamplePosition(transform.position, out hit, HITRADIUS, -1) && !playerDoll)
			{
				sceneRef.navSyncer.AddAgentToNoPathList(this);
				if (NavMesh.SamplePosition(transform.position, out hit, HITRADIUS, -1))
				{
					transform.position = hit.position;
				}
				else if (NavMesh.SamplePosition(transform.position, out hit, HITRADIUS * 2f, -1))
				{
					transform.position = hit.position;
				}
				else if (NavMesh.SamplePosition(transform.position, out hit, 1f, -1))
				{
					transform.position = hit.position;
				}
			}
			if (!agent.enabled)
			{
				agent.enabled = true;
			}
			if (carver != null && carver.enabled)
			{
				carver.enabled = false;
			}
			agent.updateRotation = true;
			agent.updateUpAxis = true;
			break;
		case NavFurnitureState.Inactive:
			if (agent.enabled)
			{
				agent.enabled = false;
			}
			if (carver != null && !carver.enabled && sceneRef.navSyncer.cuttersEnabled && !neverUseCarver)
			{
				carver.enabled = true;
			}
			transform.localPosition = pos;
			transform.localRotation = rot;
			agent.updateRotation = false;
			agent.updateUpAxis = false;
			break;
		case NavFurnitureState.FollowingPuppet:
			if (agent.enabled)
			{
				agent.enabled = false;
			}
			if (carver != null && carver.enabled)
			{
				carver.enabled = false;
			}
			transform.localPosition = pos;
			transform.localRotation = rot;
			agent.updateRotation = false;
			agent.updateUpAxis = false;
			break;
		case NavFurnitureState.MainSequence:
			transform.localPosition = pos;
			transform.localRotation = rot;
			if (!NavMesh.SamplePosition(transform.position, out hit, HITRADIUS, -1) && !playerDoll)
			{
				sceneRef.navSyncer.AddAgentToNoPathList(this);
				if (NavMesh.SamplePosition(transform.position, out hit, HITRADIUS, -1))
				{
					transform.position = hit.position;
				}
				else if (NavMesh.SamplePosition(transform.position, out hit, HITRADIUS * 2f, -1))
				{
					transform.position = hit.position;
				}
				else if (NavMesh.SamplePosition(transform.position, out hit, 1f, -1))
				{
					transform.position = hit.position;
				}
			}
			if (!agent.enabled)
			{
				agent.enabled = true;
			}
			if (carver != null && carver.enabled)
			{
				carver.enabled = false;
			}
			if (!agent.isOnNavMesh)
			{
				sceneRef.navSyncer.AddAgentToNoPathList(this);
				transform.localPosition = pos;
				transform.localRotation = rot;
			}
			agent.updateRotation = true;
			agent.updateUpAxis = true;
			break;
		case NavFurnitureState.DiningSequence:
			transform.localPosition = pos;
			transform.localRotation = rot;
			if (!NavMesh.SamplePosition(transform.position, out hit, HITRADIUS, -1) && !playerDoll)
			{
				sceneRef.navSyncer.AddAgentToNoPathList(this);
				if (NavMesh.SamplePosition(transform.position, out hit, HITRADIUS, -1))
				{
					transform.position = hit.position;
				}
				else if (NavMesh.SamplePosition(transform.position, out hit, HITRADIUS * 2f, -1))
				{
					transform.position = hit.position;
				}
				else if (NavMesh.SamplePosition(transform.position, out hit, 1f, -1))
				{
					transform.position = hit.position;
				}
			}
			if (!agent.enabled)
			{
				agent.enabled = true;
			}
			if (carver != null && carver.enabled)
			{
				carver.enabled = false;
			}
			if (!agent.isOnNavMesh)
			{
				sceneRef.navSyncer.AddAgentToNoPathList(this);
				transform.localPosition = pos;
				transform.localRotation = rot;
			}
			agent.updateRotation = true;
			agent.updateUpAxis = true;
			break;
		}
	}

	public void SetParent(Transform p, bool resetParent = false)
	{
		if (resetParent && transform.parent != startParent)
		{
			transform.SetParent(startParent, worldPositionStays: true);
		}
		else if (p == null && transform.parent != startParent)
		{
			transform.SetParent(startParent, worldPositionStays: true);
		}
		else if (transform.parent != p && !resetParent)
		{
			transform.SetParent(p, worldPositionStays: true);
		}
	}

	public NavMeshAgent GetAgent()
	{
		return agent;
	}

	public NavMeshObstacle GetCutter()
	{
		return carver;
	}

	public void SetMainSequenceTarget(Transform s)
	{
		mainSequenceTarget = s;
	}

	public Transform GetMainSequenceTarget()
	{
		return mainSequenceTarget;
	}

	public void SetHomeTarget(Transform s)
	{
		homeTarget = s;
	}

	public Transform GetHomeTarget()
	{
		return homeTarget;
	}

	public float DistanceFromTarget()
	{
		return Vector3.Distance(transform.position, mainSequenceTarget.position);
	}

	public float DistanceFromDiningRoom()
	{
		if (diningroomTarget == null)
		{
			return 0f;
		}
		return Vector3.Distance(transform.position, diningroomTarget.position);
	}

	public float DistanceFromAgentTarget()
	{
		return Vector3.Distance(transform.position, agent.destination);
	}
}
