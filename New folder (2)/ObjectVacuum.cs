using UnityEngine;

[RequireComponent(typeof(VVRConsumer))]
public class ObjectVacuum : ObjectBase
{
	public GameObject sphereHolder;

	public VVRConsumer vacConsumer;

	private AudioSource vacuumSound;

	private float blowForce;

	private float targetVolume;

	public bool suckOnly;
}
