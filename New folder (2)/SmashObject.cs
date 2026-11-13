using UnityEngine;

public class SmashObject : MonoBehaviour
{
	public GameObject[] smashPieces;

	public GameObject smashParticles;

	public AudioEvent smashAudioEvent;

	public float smashVelocityMin;

	private bool _destroyed;

	private void OnCollisionEnter(Collision collision)
	{
		if (!_destroyed && GetComponent<Rigidbody>().velocity.sqrMagnitude > smashVelocityMin)
		{
			Debug.Log("SMASH TIME");
			for (int i = 0; i < smashPieces.Length; i++)
			{
				GameObject gameObject = Object.Instantiate(smashPieces[i], transform.position, Random.rotation);
				Debug.Log("Created smash peice: " + gameObject.name);
				gameObject.transform.parent = transform.parent;
				gameObject.transform.localScale = Vector3.one;
				gameObject.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
				gameObject.GetComponent<Rigidbody>().angularVelocity = GetComponent<Rigidbody>().angularVelocity;
			}
			Object.Instantiate(smashParticles, transform.position, Quaternion.identity);
			if (smashAudioEvent != null)
			{
				smashAudioEvent.transform.position = transform.position;
				smashAudioEvent.Play();
			}
			_destroyed = true;
			Object.Destroy(base.gameObject);
		}
	}
}
