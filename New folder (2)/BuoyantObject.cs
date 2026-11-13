using UnityEngine;

[RequireComponent(typeof(ObjectBase))]
[RequireComponent(typeof(GvrAudioSource))]
public class BuoyantObject : MonoBehaviour
{
	private ObjectBase objBase;

	[SerializeField]
	protected GvrAudioSource splashSound;

	public float distanceFromWater;

	protected SunsetWater water;

	public bool floating;

	public float maxMagnitude = 0.5f;

	public float buoyencyForce = 0.15f;

	public Rigidbody mRigidBody { get; private set; }

	protected virtual void Awake()
	{
		mRigidBody = GetComponent<Rigidbody>();
		objBase = GetComponent<ObjectBase>();
		if (splashSound == null)
		{
			splashSound = GetComponent<GvrAudioSource>();
		}
	}

	private void FixedUpdate()
	{
		if (water != null)
		{
			distanceFromWater = transform.position.y - water.transform.position.y;
		}
		if (objBase.isGrabbed && floating)
		{
			LeaveWater();
		}
		if ((double)distanceFromWater > 0.2 && floating)
		{
			LeaveWater();
		}
		addBuoyencyForce();
	}

	private void addBuoyencyForce()
	{
		if (floating && water != null)
		{
			float num = transform.position.y - water.transform.position.y;
			Vector3 force = ((!(num < -0.1f)) ? (Physics.gravity * buoyencyForce) : (-(Physics.gravity * buoyencyForce)));
			mRigidBody.AddForce(force);
			mRigidBody.velocity = Vector3.ClampMagnitude(mRigidBody.velocity, maxMagnitude);
		}
	}

	public virtual void SetWater(SunsetWater set)
	{
		if (buoyencyForce != 0f)
		{
			mRigidBody.velocity = Vector3.zero;
			mRigidBody.AddForce(Vector3.up * 5f);
			floating = true;
			water = set;
			mRigidBody.useGravity = false;
		}
		splashSound.Play();
	}

	public void LeaveWater()
	{
		floating = false;
		mRigidBody.useGravity = true;
		water.floatingObjects.Remove(this);
		water = null;
	}
}
