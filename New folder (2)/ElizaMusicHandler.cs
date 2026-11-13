using UnityEngine;

public class ElizaMusicHandler : MonoBehaviour
{
	public AudioLoop vesuviusLoop;

	public AudioLoop vesuviusRumble;

	public AudioLoop cosmicLoop;

	public AudioLoop mermanLoop;

	public AudioLoop burrowsLoop;

	public AudioLoop centerLoop;

	public AudioLoop balloonLoop;

	public AudioLoop finalLoop;

	[HideInInspector]
	public bool playVesuviusLoop;

	[HideInInspector]
	public bool playCosmicLoop;

	[HideInInspector]
	public bool playMermanLoop;

	[HideInInspector]
	public bool playBalloonLoop;

	[HideInInspector]
	public bool playBurrowsLoop;

	[HideInInspector]
	public bool playCenterLoop;

	[HideInInspector]
	public bool playFinalLoop;

	private float currentVesuviusSourceVolume;

	private float vesuviusMaxVolume = 0.7f;

	private float currentCosmicSourceVolume;

	private float cosmicMaxVolume = 0.7f;

	private float currentMermanSourceVolume;

	private float mermanMaxVolume = 0.7f;

	private float currentCenterSourceVolume;

	private float centerMaxVolume = 0.7f;

	private float currentBalloonSourceVolume;

	private float balloonMaxVolume = 0.7f;

	private float currentBurrowsSourceVolume;

	private float burrowsMaxVolume = 0.7f;

	private float currentFinalSourceVolume;

	private float finalMaxVolume = 0.7f;

	private float volumeChangeRate = 1f;

	private bool stoppedAds;

	private bool finalStartedPlaying;

	private bool enableFrame;

	private void Start()
	{
		initialize();
		startPlaying();
	}

	private void OnEnable()
	{
		startPlaying();
	}

	private void OnDisable()
	{
		enableFrame = true;
	}

	private void Update()
	{
		volumeHandler();
	}

	private void initialize()
	{
		currentVesuviusSourceVolume = 0f;
		currentCosmicSourceVolume = 0f;
		currentBalloonSourceVolume = 0f;
		currentBurrowsSourceVolume = 0f;
		currentMermanSourceVolume = 0f;
		currentCenterSourceVolume = 0f;
		playCenterLoop = (playCosmicLoop = (playMermanLoop = (playVesuviusLoop = (playBurrowsLoop = (playBalloonLoop = false)))));
		currentFinalSourceVolume = 0f;
	}

	private void startPlaying()
	{
		if ((bool)vesuviusLoop)
		{
			vesuviusLoop.Play();
			vesuviusRumble.Play();
			vesuviusLoop.SetSourceVolume(0f);
			vesuviusRumble.SetSourceVolume(0f);
			if (playVesuviusLoop)
			{
				vesuviusLoop.SetSourceVolume(1f);
				vesuviusRumble.SetSourceVolume(1f);
			}
		}
		if ((bool)cosmicLoop)
		{
			cosmicLoop.Play();
			cosmicLoop.SetSourceVolume(0f);
			if (playCosmicLoop)
			{
				cosmicLoop.SetSourceVolume(1f);
			}
		}
		if ((bool)centerLoop)
		{
			centerLoop.Play();
			centerLoop.SetSourceVolume(0f);
			if (playCenterLoop)
			{
				centerLoop.SetSourceVolume(1f);
			}
		}
		if ((bool)balloonLoop)
		{
			balloonLoop.Play();
			balloonLoop.SetSourceVolume(0f);
			if (playBalloonLoop)
			{
				balloonLoop.SetSourceVolume(1f);
			}
		}
		if ((bool)burrowsLoop)
		{
			burrowsLoop.Play();
			burrowsLoop.SetSourceVolume(0f);
			if (playBurrowsLoop)
			{
				burrowsLoop.SetSourceVolume(1f);
			}
		}
		if ((bool)mermanLoop)
		{
			mermanLoop.Play();
			mermanLoop.SetSourceVolume(0f);
			if (playMermanLoop)
			{
				mermanLoop.SetSourceVolume(1f);
			}
		}
	}

	public void StartPlayingFinal()
	{
		if ((bool)finalLoop)
		{
			finalStartedPlaying = true;
			playFinalLoop = true;
			finalLoop.Play();
			finalLoop.SetSourceVolume(0f);
		}
	}

	public void StopPlayingFinal()
	{
		if ((bool)finalLoop)
		{
			playFinalLoop = false;
		}
	}

	public void ResetAdBools()
	{
		if (!enableFrame)
		{
			playCenterLoop = (playCosmicLoop = (playMermanLoop = (playVesuviusLoop = (playBurrowsLoop = (playBalloonLoop = false)))));
		}
		else
		{
			enableFrame = false;
		}
	}

	public void StopAllAds()
	{
		if ((bool)vesuviusLoop)
		{
			vesuviusLoop.SetSourceVolume(0f);
			vesuviusRumble.SetSourceVolume(0f);
			vesuviusLoop.Stop();
			vesuviusRumble.Stop();
		}
		if ((bool)cosmicLoop)
		{
			cosmicLoop.SetSourceVolume(0f);
			cosmicLoop.Stop();
		}
		if ((bool)balloonLoop)
		{
			balloonLoop.SetSourceVolume(0f);
			balloonLoop.Stop();
		}
		if ((bool)burrowsLoop)
		{
			burrowsLoop.SetSourceVolume(0f);
			burrowsLoop.Stop();
		}
		if ((bool)mermanLoop)
		{
			mermanLoop.SetSourceVolume(0f);
			mermanLoop.Stop();
		}
		if ((bool)centerLoop)
		{
			centerLoop.SetSourceVolume(0f);
			centerLoop.Stop();
		}
		ResetAdBools();
		stoppedAds = true;
	}

	private void volumeHandler()
	{
		if (!stoppedAds)
		{
			if (playCosmicLoop)
			{
				if (currentCosmicSourceVolume < cosmicMaxVolume)
				{
					currentCosmicSourceVolume += Time.deltaTime * volumeChangeRate;
					cosmicLoop.SetSourceVolume(currentCosmicSourceVolume);
				}
			}
			else if (currentCosmicSourceVolume > 0f)
			{
				currentCosmicSourceVolume -= Time.deltaTime * volumeChangeRate;
				cosmicLoop.SetSourceVolume(currentCosmicSourceVolume);
			}
			if (playVesuviusLoop)
			{
				if (currentVesuviusSourceVolume < vesuviusMaxVolume)
				{
					currentVesuviusSourceVolume += Time.deltaTime * volumeChangeRate;
					vesuviusLoop.SetSourceVolume(currentVesuviusSourceVolume);
					vesuviusRumble.SetSourceVolume(currentVesuviusSourceVolume);
				}
			}
			else if (currentVesuviusSourceVolume > 0f)
			{
				currentVesuviusSourceVolume -= Time.deltaTime * volumeChangeRate;
				vesuviusLoop.SetSourceVolume(currentVesuviusSourceVolume);
				vesuviusRumble.SetSourceVolume(currentVesuviusSourceVolume);
			}
			if (playBalloonLoop)
			{
				if (currentBalloonSourceVolume < balloonMaxVolume)
				{
					currentBalloonSourceVolume += Time.deltaTime * volumeChangeRate;
					balloonLoop.SetSourceVolume(currentBalloonSourceVolume);
				}
			}
			else if (currentBalloonSourceVolume > 0f)
			{
				currentBalloonSourceVolume -= Time.deltaTime * volumeChangeRate;
				balloonLoop.SetSourceVolume(currentBalloonSourceVolume);
			}
			if (playBurrowsLoop)
			{
				if (currentBurrowsSourceVolume < burrowsMaxVolume)
				{
					currentBurrowsSourceVolume += Time.deltaTime * volumeChangeRate;
					burrowsLoop.SetSourceVolume(currentBurrowsSourceVolume);
				}
			}
			else if (currentBurrowsSourceVolume > 0f)
			{
				currentBurrowsSourceVolume -= Time.deltaTime * volumeChangeRate;
				burrowsLoop.SetSourceVolume(currentBurrowsSourceVolume);
			}
			if (playMermanLoop)
			{
				if (currentMermanSourceVolume < mermanMaxVolume)
				{
					currentMermanSourceVolume += Time.deltaTime * volumeChangeRate;
					mermanLoop.SetSourceVolume(currentMermanSourceVolume);
				}
			}
			else if (currentMermanSourceVolume > 0f)
			{
				currentMermanSourceVolume -= Time.deltaTime * volumeChangeRate;
				mermanLoop.SetSourceVolume(currentMermanSourceVolume);
			}
			if (playCenterLoop)
			{
				if (currentCenterSourceVolume < centerMaxVolume)
				{
					currentCenterSourceVolume += Time.deltaTime * volumeChangeRate;
					centerLoop.SetSourceVolume(currentCenterSourceVolume);
				}
			}
			else if (currentCenterSourceVolume > 0f)
			{
				currentCenterSourceVolume -= Time.deltaTime * volumeChangeRate;
				centerLoop.SetSourceVolume(currentCenterSourceVolume);
			}
		}
		if (!finalStartedPlaying)
		{
			return;
		}
		if (playFinalLoop)
		{
			if (currentFinalSourceVolume < finalMaxVolume)
			{
				currentFinalSourceVolume += Time.deltaTime * volumeChangeRate;
				finalLoop.SetSourceVolume(currentFinalSourceVolume);
			}
		}
		else if (currentFinalSourceVolume > 0f)
		{
			currentFinalSourceVolume -= Time.deltaTime * volumeChangeRate;
			finalLoop.SetSourceVolume(currentFinalSourceVolume);
		}
	}
}
