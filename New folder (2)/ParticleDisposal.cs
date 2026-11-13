using UnityEngine;

public class ParticleDisposal : MonoBehaviour
{
	private float time;

	private void Start()
	{
	}

	private void Update()
	{
		if (!GetComponent<ParticleSystem>().IsAlive(withChildren: true))
		{
			Object.Destroy(gameObject);
		}
		time += Time.deltaTime;
		if (time > 5f)
		{
			Object.Destroy(gameObject);
		}
	}
}
