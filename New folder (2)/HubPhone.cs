using UnityEngine;

public class HubPhone : MonoBehaviour
{
	public AudioLoop ring;

	public AudioLoop message;

	public AudioEvent morph;

	public AudioEvent glitch;

	public AudioEvent pickUp;

	public AudioEvent putDown;

	public void Ring()
	{
		if (ring != null)
		{
			ring.Play();
		}
	}

	public void Message()
	{
		if (message != null)
		{
			message.Play();
		}
	}

	public void Morph()
	{
		if (morph != null)
		{
			morph.Play();
		}
	}

	public void Glitch()
	{
		if (glitch != null)
		{
			glitch.Play();
		}
	}

	public void PickUp()
	{
		if (pickUp != null)
		{
			pickUp.Play();
		}
	}

	public void PutDown()
	{
		if (putDown != null)
		{
			putDown.Play();
		}
	}

	public void Stop()
	{
		if (ring != null)
		{
			ring.Stop();
		}
		if (message != null)
		{
			message.Stop();
		}
	}
}
