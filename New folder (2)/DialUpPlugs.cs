using System.Collections;
using UnityEngine;

public class DialUpPlugs : MonoBehaviour
{
	public AudioClip plugSound;

	public ParticleSystem plugParticles;

	public VVR_STAMPER_23_PLUGS levelScript;

	private GameObject cable;

	private bool pluggedIn;

	private void OnTriggerEnter(Collider other)
	{
		if (!pluggedIn && (bool)other.GetComponentInParent<SpringJoint>())
		{
			cable = other.transform.parent.gameObject;
			GlobalCoroutineContainer.AddThread(PlugInCord(), this);
		}
	}

	private IEnumerator PlugInCord()
	{
		cable.GetComponentInParent<ObjectBase>().ForceDropByPlayer();
		pluggedIn = true;
		plugParticles.Play(withChildren: true);
		AudioSource.PlayClipAtPoint(plugSound, transform.position);
		cable.GetComponentInParent<Rigidbody>().isKinematic = true;
		levelScript.PluggedIn();
		while (cable.transform.position.y > 0.6f)
		{
			cable.transform.position += cable.transform.forward * 5f * Time.deltaTime;
			yield return null;
		}
		yield return null;
	}
}
