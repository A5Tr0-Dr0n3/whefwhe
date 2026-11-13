using UnityEngine;

public class ObjectPooledToast : ObjectBase
{
	public MeshFilter meshFilter;

	public Mesh uneatenUndercooked;

	public Mesh uneatenToast;

	public Mesh uneatenBurnt;

	public Mesh eatenUndercooked;

	public Mesh eatenToast;

	public Mesh eatenBurnt;

	public AudioEvent sfx;

	public ParticleSystem particles;

	private Mesh bitMesh;

	private Mesh startMesh;

	private bool bit;

	internal ObjectToaster.Toast toastState;

	internal Quaternion startRot;

	protected override void Awake()
	{
		base.Awake();
		startMesh = meshFilter.mesh;
		startRot = transform.rotation;
	}

	public void SetMesh(ObjectToaster.Toast type)
	{
		toastState = type;
		switch (type)
		{
		case ObjectToaster.Toast.Uncooked:
			meshFilter.mesh = uneatenUndercooked;
			bitMesh = eatenUndercooked;
			break;
		case ObjectToaster.Toast.Toasted:
			meshFilter.mesh = uneatenToast;
			bitMesh = eatenToast;
			break;
		case ObjectToaster.Toast.Burning:
			meshFilter.mesh = uneatenBurnt;
			bitMesh = eatenBurnt;
			break;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == EagerMaster.instance.mainCamera.gameObject && isGrabbed)
		{
			Bite();
		}
	}

	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		grabber.orientDuringGrab = true;
	}

	private void Bite()
	{
		sfx.Play();
		particles.transform.position = transform.position;
		particles.Play();
		if (bit)
		{
			meshFilter.mesh = startMesh;
			ForceDropByPlayer();
			gameObject.SetActive(value: false);
		}
		else
		{
			bit = true;
			meshFilter.mesh = bitMesh;
		}
	}
}
