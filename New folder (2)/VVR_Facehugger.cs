using System;
using UnityEngine;

[Obsolete("Very likely unused")]
public class VVR_Facehugger : MonoBehaviour
{
	public float chaseSpeed;

	public float turnSpeed;

	public float patrolRadius;

	public float patrolSpeedMax;

	public float patrolSpeedMin;

	public float patrolSpeed;

	private bool foundPlayer;

	private bool isHugging;

	private bool isPatrolling;

	private Rigidbody r;

	private Vector3 playerVector;

	private Vector3 patrolCentre = new Vector3(0f, 0f, 0f);

	private Vector3 newDestination = new Vector3(0f, 0f, 0f);

	private Vector3 patrolVector;

	public float killVelocity;

	private void Start()
	{
		r = GetComponent<Rigidbody>();
		patrolSpeed = UnityEngine.Random.Range(patrolSpeedMin, patrolSpeedMax);
		if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
		{
			patrolSpeed *= -1f;
		}
	}

	private void FixedUpdate()
	{
		if (GetComponent<ObjectBase>().isGrabbed)
		{
			isPatrolling = false;
		}
		if (isHugging)
		{
			HugPlayer();
		}
		else if (foundPlayer)
		{
			MoveTowardsPlayer();
			isPatrolling = false;
		}
		else
		{
			Patrol();
		}
	}

	private void MoveTowardsPlayer()
	{
		if (Vector3.Distance(transform.position, EagerMaster.instance.mainCamera.transform.position) < 1f)
		{
			enabled = false;
		}
		playerVector = (Camera.main.transform.position - transform.position).normalized;
		r.AddForce(playerVector * chaseSpeed);
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation((transform.position - Camera.main.transform.position).normalized), 0.15f);
	}

	private void HugPlayer()
	{
	}

	private void Patrol()
	{
		if (!isPatrolling)
		{
			patrolCentre = transform.position;
		}
		isPatrolling = true;
		newDestination.x = Mathf.Sin(Time.time * patrolSpeed) * patrolRadius + patrolCentre.x;
		newDestination.z = Mathf.Cos(Time.time * patrolSpeed) * patrolRadius + patrolCentre.z;
		newDestination.y = Mathf.Sin(Time.time * patrolSpeed * 0.5f) * patrolRadius * 0.25f + patrolCentre.y;
		patrolVector = newDestination - transform.position;
		r.AddForce(patrolVector);
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation((transform.position - newDestination).normalized), 0.15f);
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(patrolCentre, 0.05f);
		Gizmos.DrawWireSphere(newDestination, 0.05f);
		Gizmos.DrawLine(patrolCentre, newDestination);
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "MainCamera")
		{
			foundPlayer = true;
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (col.gameObject.tag == "MainCamera")
		{
			foundPlayer = false;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (r.velocity.magnitude > killVelocity)
		{
			r.useGravity = true;
			r.drag = 0f;
			ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				particleSystem.Stop();
			}
			ToolManager.DropEverything();
			enabled = false;
		}
	}
}
