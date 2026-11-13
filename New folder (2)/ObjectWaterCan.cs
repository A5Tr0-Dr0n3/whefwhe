using UnityEngine;

public class ObjectWaterCan : ObjectBase
{
	public LayerMask objectMask;

	public Transform spout;

	public ParticleSystem sparkles;

	public float rayDistance = 2f;

	private bool foundWaterTarget;

	private RaycastHit[] hitInfo;

	private ObjectBase gossipObject;

	private ObjectBase ob;

	private ObjectPlant obw;

	private ObjectPlant lastObw;

	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		grabber.orientDuringGrab = true;
		float z = 0f - gameObject.GetComponent<MeshFilter>().sharedMesh.bounds.extents.z;
		grabber.heldHighVector = gameObject.GetComponent<MeshFilter>().sharedMesh.bounds.center + new Vector3(0f, 0f, z) / transform.localScale.y;
	}

	public override void GrabUpdate(Manipulator grabber)
	{
		if (grabber != grabbedBy[0])
		{
			return;
		}
		base.GrabUpdate(grabber);
		hitInfo = Physics.CapsuleCastAll(spout.position + spout.forward * 0.1f, spout.position + spout.forward * rayDistance, 0.225f, spout.forward, rayDistance, objectMask, QueryTriggerInteraction.Ignore);
		if (isPouring() && hitInfo.Length > 0)
		{
			RaycastHit[] array = hitInfo;
			for (int i = 0; i < array.Length; i++)
			{
				RaycastHit raycastHit = array[i];
				if ((bool)raycastHit.collider.GetComponent<ObjectBase>())
				{
					ob = raycastHit.collider.GetComponent<ObjectBase>();
					if (ob is ObjectWater && !ob.isSatisfied)
					{
						sparkles.transform.position = raycastHit.collider.bounds.center;
						if (!sparkles.isPlaying)
						{
							sparkles.Play();
						}
						ObjectWater objectWater = (ObjectWater)ob;
						objectWater.WaterObject();
						break;
					}
				}
				else
				{
					sparkles.Stop();
				}
			}
		}
		else
		{
			sparkles.Stop();
		}
	}

	public override void GrabRelease(Manipulator grabber)
	{
		base.GrabRelease(grabber);
		sparkles.Stop();
	}

	private void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawRay(spout.position, spout.forward);
			Gizmos.DrawWireSphere(spout.position + spout.forward * 0.1f, 0.1f);
			Gizmos.DrawWireSphere(spout.position + spout.forward * rayDistance, 0.1f);
		}
	}

	private bool isPouring()
	{
		if (Vector3.Dot(spout.forward, -Vector3.up) > 0.4f)
		{
			return true;
		}
		return false;
	}
}
