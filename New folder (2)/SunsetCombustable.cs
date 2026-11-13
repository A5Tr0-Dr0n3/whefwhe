using System.Collections;
using UnityEngine;

public class SunsetCombustable : MonoBehaviour
{
	public float timeUntilBurnt = 1f;

	public bool onFire;

	public ParticleSystem fireParticleSystem;

	private bool counting;

	private float counter;

	[SerializeField]
	private bool perminantFire;

	[SerializeField]
	private GvrAudioSource burningSound;

	[SerializeField]
	private GvrAudioSource combustionSound;

	[SerializeField]
	private GvrAudioSource burntSound;

	public event SunsetFireEventHandler OnStartFire;

	protected void Update()
	{
		if (!counting)
		{
			return;
		}
		counter += Time.deltaTime;
		if (!(counter > timeUntilBurnt))
		{
			return;
		}
		if (fireParticleSystem != null)
		{
			if (combustionSound != null)
			{
				combustionSound.Play();
			}
			if (burntSound != null)
			{
				burntSound.Play();
			}
			GlobalCoroutineContainer.AddThread(combustOnDelay(0.15f), this);
		}
		if (OnStartFire != null)
		{
			OnStartFire();
		}
		counting = false;
	}

	private IEnumerator combustOnDelay(float s)
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(s, this);
		onFire = true;
		fireParticleSystem.gameObject.SetActive(value: true);
		if (!fireParticleSystem.isPlaying)
		{
			fireParticleSystem.Play();
		}
		burningSound.Pause();
	}

	protected void OnTriggerEnter(Collider other)
	{
		ObjectSun component = other.GetComponent<ObjectSun>();
		if (component != null)
		{
			counting = true;
			if (burningSound != null)
			{
				burningSound.Play();
			}
		}
	}

	protected void OnTriggerExit(Collider other)
	{
		ObjectSun component = other.GetComponent<ObjectSun>();
		if (!(component != null))
		{
			return;
		}
		if (burningSound != null)
		{
			burningSound.Pause();
		}
		if (fireParticleSystem != null && !perminantFire)
		{
			fireParticleSystem.gameObject.SetActive(value: false);
			if (burntSound != null)
			{
				burntSound.Pause();
			}
		}
		counting = false;
		onFire = false;
		counter = 0f;
	}
}
