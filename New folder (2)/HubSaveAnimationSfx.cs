using UnityEngine;

public class HubSaveAnimationSfx : MonoBehaviour
{
	[SerializeField]
	private AudioClip checkSound;

	private GvrAudioSource mAudioSource;

	private void Start()
	{
		mAudioSource = GetComponent<GvrAudioSource>();
	}

	public void CheckFillSound()
	{
		mAudioSource.PlayOneShot(checkSound);
	}
}
