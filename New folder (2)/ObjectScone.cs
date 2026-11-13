using System.Collections;
using UnityEngine;

public class ObjectScone : ObjectBase
{
	public MeshFilter meshFilter;

	public Mesh bitMesh;

	private Mesh startMesh;

	public AudioEvent sfx;

	public ParticleSystem particles;

	private bool bit;

	private bool bittenRecently;

	protected override void Awake()
	{
		base.Awake();
		startMesh = meshFilter.mesh;
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
		if (!bittenRecently)
		{
			bittenRecently = true;
			sfx.Play();
			particles.transform.position = transform.position;
			particles.Play();
			if (bit)
			{
				ForceDropByPlayer();
				gameObject.SetActive(value: false);
				meshFilter.mesh = startMesh;
			}
			else
			{
				bit = true;
				meshFilter.mesh = bitMesh;
			}
			GlobalCoroutineContainer.AddThread(ReEnableBiteOnDelay(0.1f), this);
		}
	}

	private IEnumerator ReEnableBiteOnDelay(float t)
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(t, this);
		bittenRecently = false;
	}
}
