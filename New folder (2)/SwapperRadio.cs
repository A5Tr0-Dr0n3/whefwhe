using UnityEngine;

public class SwapperRadio : MonoBehaviour
{
	public GvrAudioSource source;

	private void OnEnable()
	{
		source.Play();
	}

	private void OnDisable()
	{
		source.Stop();
	}
}
