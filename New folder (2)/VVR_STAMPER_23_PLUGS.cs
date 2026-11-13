using UnityEngine;

public class VVR_STAMPER_23_PLUGS : SceneBehaviorManager
{
	[SerializeField]
	private DialUpPlugs[] plugs;

	[SerializeField]
	private GvrAudioSource music;

	[SerializeField]
	private GameObject headset;

	[SerializeField]
	private GvrAudioSource successSound;

	private float numberPlugged;

	protected override void OnFirstTimeSceneActivated()
	{
		base.OnFirstTimeSceneActivated();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void PluggedIn()
	{
		numberPlugged++;
		if (numberPlugged == 1f)
		{
			music.Play();
		}
		else if (numberPlugged == (float)plugs.Length)
		{
			headset.SetActive(value: true);
			successSound.Play();
		}
		music.volume = numberPlugged / (float)plugs.Length;
	}
}
