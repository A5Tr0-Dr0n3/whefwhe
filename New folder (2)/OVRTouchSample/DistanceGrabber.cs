using UnityEngine;

namespace OVRTouchSample;

[RequireComponent(typeof(Rigidbody))]
public class DistanceGrabber : OVRGrabber
{
	[SerializeField]
	private float m_spherecastRadius;

	[SerializeField]
	private float m_noSnapThreshhold = 0.05f;

	[SerializeField]
	private bool m_useSpherecast;

	[SerializeField]
	public bool m_preventGrabThroughWalls;

	[SerializeField]
	private float m_objectPullVelocity = 10f;

	private float m_objectPullMaxRotationRate = 360f;

	private bool m_movingObjectToHand;

	[SerializeField]
	private float m_maxGrabDistance;

	[SerializeField]
	private int m_grabObjectsInLayer;

	[SerializeField]
	private int m_obstructionLayer;

	[SerializeField]
	private GameObject m_player;

	private DistanceGrabber m_otherHand;

	protected DistanceGrabbable m_target;

	protected Collider m_targetCollider;

	private OVRCameraRig m_camera;

	public bool UseSpherecast
	{
		get
		{
			return m_useSpherecast;
		}
		set
		{
			m_useSpherecast = value;
			GrabVolumeEnable(!m_useSpherecast);
		}
	}

	protected override void Start()
	{
		base.Start();
		SphereCollider componentInChildren = m_player.GetComponentInChildren<SphereCollider>();
		m_maxGrabDistance = componentInChildren.radius + 3f;
		if (m_parentHeldObject)
		{
			Debug.LogError("m_parentHeldObject incompatible with DistanceGrabber. Setting to false.");
			m_parentHeldObject = false;
		}
		DistanceGrabber[] array = Object.FindObjectsOfType<DistanceGrabber>();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != this)
			{
				m_otherHand = array[i];
			}
		}
		m_camera = GameObject.Find("OVRCameraRig").GetComponent<OVRCameraRig>();
	}

	private void Update()
	{
		FindTarget(out var dgOut, out var collOut);
		if (dgOut != m_target)
		{
			if (m_target != null)
			{
				m_target.Targeted = m_otherHand.m_target == m_target;
			}
			m_target = dgOut;
			m_targetCollider = collOut;
			if (m_target != null)
			{
				m_target.Targeted = true;
			}
		}
	}

	protected override void GrabBegin()
	{
		DistanceGrabbable target = m_target;
		Collider targetCollider = m_targetCollider;
		GrabVolumeEnable(enabled: false);
		if (!(target != null))
		{
			return;
		}
		if (target.isGrabbed)
		{
			((DistanceGrabber)target.grabbedBy).OffhandGrabbed(target);
		}
		m_grabbedObj = target;
		m_grabbedObj.GrabBegin(this, targetCollider);
		m_movingObjectToHand = true;
		m_lastPos = transform.position;
		m_lastRot = transform.rotation;
		Vector3 vector = targetCollider.ClosestPointOnBounds(m_gripTransform.position);
		if (!m_grabbedObj.snapPosition && !m_grabbedObj.snapOrientation && m_noSnapThreshhold > 0f && (vector - m_gripTransform.position).magnitude < m_noSnapThreshhold)
		{
			Vector3 vector2 = m_grabbedObj.transform.position - transform.position;
			m_movingObjectToHand = false;
			vector2 = Quaternion.Inverse(transform.rotation) * vector2;
			m_grabbedObjectPosOff = vector2;
			Quaternion grabbedObjectRotOff = Quaternion.Inverse(transform.rotation) * m_grabbedObj.transform.rotation;
			m_grabbedObjectRotOff = grabbedObjectRotOff;
			return;
		}
		m_grabbedObjectPosOff = m_gripTransform.localPosition;
		if ((bool)m_grabbedObj.snapOffset)
		{
			Vector3 position = m_grabbedObj.snapOffset.position;
			if (m_controller == OVRInput.Controller.LTouch)
			{
				position.x = 0f - position.x;
			}
			m_grabbedObjectPosOff += position;
		}
		m_grabbedObjectRotOff = m_gripTransform.localRotation;
		if ((bool)m_grabbedObj.snapOffset)
		{
			m_grabbedObjectRotOff = m_grabbedObj.snapOffset.rotation * m_grabbedObjectRotOff;
		}
	}

	protected override void MoveGrabbedObject(Vector3 pos, Quaternion rot, bool forceTeleport = false)
	{
		if (m_grabbedObj == null)
		{
			return;
		}
		Rigidbody grabbedRigidbody = m_grabbedObj.grabbedRigidbody;
		Vector3 vector = pos + rot * m_grabbedObjectPosOff;
		Quaternion quaternion = rot * m_grabbedObjectRotOff;
		if (m_movingObjectToHand)
		{
			float num = m_objectPullVelocity * Time.deltaTime;
			Vector3 vector2 = vector - m_grabbedObj.transform.position;
			if (num * num * 1.1f > vector2.sqrMagnitude)
			{
				m_movingObjectToHand = false;
			}
			else
			{
				vector2.Normalize();
				vector = m_grabbedObj.transform.position + vector2 * num;
				quaternion = Quaternion.RotateTowards(m_grabbedObj.transform.rotation, quaternion, m_objectPullMaxRotationRate * Time.deltaTime);
			}
		}
		grabbedRigidbody.MovePosition(vector);
		grabbedRigidbody.MoveRotation(quaternion);
	}

	private static DistanceGrabbable HitInfoToGrabbable(RaycastHit hitInfo)
	{
		if (hitInfo.collider != null)
		{
			GameObject gameObject = hitInfo.collider.gameObject;
			return gameObject.GetComponent<DistanceGrabbable>() ?? gameObject.GetComponentInParent<DistanceGrabbable>();
		}
		return null;
	}

	protected bool FindTarget(out DistanceGrabbable dgOut, out Collider collOut)
	{
		dgOut = null;
		collOut = null;
		float num = float.MaxValue;
		foreach (ColorGrabbable key in m_grabCandidates.Keys)
		{
			DistanceGrabbable distanceGrabbable = key as DistanceGrabbable;
			if (!(distanceGrabbable != null) || !distanceGrabbable.InRange || (distanceGrabbable.isGrabbed && !distanceGrabbable.allowOffhandGrab))
			{
				continue;
			}
			for (int i = 0; i < distanceGrabbable.grabPoints.Length; i++)
			{
				Collider collider = distanceGrabbable.grabPoints[i];
				Vector3 vector = collider.ClosestPointOnBounds(m_gripTransform.position);
				float sqrMagnitude = (m_gripTransform.position - vector).sqrMagnitude;
				if (!(sqrMagnitude < num))
				{
					continue;
				}
				bool flag = true;
				if (m_preventGrabThroughWalls)
				{
					Ray ray = new Ray
					{
						direction = distanceGrabbable.transform.position - m_gripTransform.position,
						origin = m_gripTransform.position
					};
					Debug.DrawRay(ray.origin, ray.direction, Color.red, 0.1f);
					if (Physics.Raycast(ray, out var hitInfo, m_maxGrabDistance, 1 << m_obstructionLayer))
					{
						float magnitude = (collider.ClosestPointOnBounds(m_gripTransform.position) - m_gripTransform.position).magnitude;
						if ((double)magnitude > (double)hitInfo.distance * 1.1)
						{
							flag = false;
						}
					}
				}
				if (flag)
				{
					num = sqrMagnitude;
					dgOut = distanceGrabbable;
					collOut = collider;
				}
			}
		}
		if (dgOut == null && m_useSpherecast)
		{
			return FindTargetWithSpherecast(out dgOut, out collOut);
		}
		return dgOut != null;
	}

	protected bool FindTargetWithSpherecast(out DistanceGrabbable dgOut, out Collider collOut)
	{
		dgOut = null;
		collOut = null;
		Ray ray = new Ray(m_gripTransform.position, m_gripTransform.forward);
		if (Physics.SphereCast(ray, m_spherecastRadius, out var hitInfo, m_maxGrabDistance, 1 << m_grabObjectsInLayer))
		{
			DistanceGrabbable distanceGrabbable = null;
			Collider collider = null;
			if (hitInfo.collider != null)
			{
				distanceGrabbable = hitInfo.collider.gameObject.GetComponentInParent<DistanceGrabbable>();
				collider = ((!(distanceGrabbable == null)) ? hitInfo.collider : null);
			}
			if (distanceGrabbable != null && m_preventGrabThroughWalls)
			{
				ray.direction = hitInfo.point - m_gripTransform.position;
				dgOut = distanceGrabbable;
				collOut = collider;
				if (Physics.Raycast(ray, out var hitInfo2, 1 << m_obstructionLayer))
				{
					DistanceGrabbable distanceGrabbable2 = null;
					if (hitInfo.collider != null)
					{
						distanceGrabbable2 = hitInfo2.collider.gameObject.GetComponentInParent<DistanceGrabbable>();
					}
					if (distanceGrabbable2 != distanceGrabbable && hitInfo2.distance < hitInfo.distance)
					{
						dgOut = null;
						collOut = null;
					}
				}
			}
		}
		return dgOut != null;
	}

	protected new virtual void GrabVolumeEnable(bool enabled)
	{
		if (m_useSpherecast)
		{
			enabled = false;
		}
		base.GrabVolumeEnable(enabled);
	}

	protected override void OffhandGrabbed(OVRGrabbable grabbable)
	{
		base.OffhandGrabbed(grabbable);
	}
}
