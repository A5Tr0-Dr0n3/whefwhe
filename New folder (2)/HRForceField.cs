using UnityEngine;

public class HRForceField : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem ps;

	[SerializeField]
	private GvrAudioSource destructionSound;

	private bool hasBroken;

	public Rigidbody plugJoint;

	public VVRConsumable consumable;

	private void Update()
	{
		if ((bool)plugJoint && !plugJoint.GetComponent<FixedJoint>() && !hasBroken)
		{
			hasBroken = true;
			plugJoint.AddForce(Vector3.up * -7500f);
			ps.Play(withChildren: true);
			destructionSound.Play();
			consumable.unconsumable = false;
		}
	}
}
