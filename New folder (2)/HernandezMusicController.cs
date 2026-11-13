using System.Collections;
using UnityEngine;

public class HernandezMusicController : MonoBehaviour
{
	[SerializeField]
	private GvrAudioSource musicSource;

	[SerializeField]
	private float[] musicBreakTimestamps;

	[SerializeField]
	private float postHernandezDeathTimestamp;

	[SerializeField]
	private GvrAudioSource liftMusicSource;

	private bool liftMusicPlaying;

	private float musicRestartTime;

	private bool jumpTimestamp;

	public void StopMusic()
	{
		jumpTimestamp = true;
		if (musicBreakTimestamps.Length > 0)
		{
			int num = 0;
			for (int i = 0; i < musicBreakTimestamps.Length; i++)
			{
				if (i < musicBreakTimestamps.Length - 1)
				{
					if (musicBreakTimestamps[i] > musicSource.time)
					{
						num = i;
						break;
					}
					continue;
				}
				num = 0;
				break;
			}
			musicRestartTime = musicBreakTimestamps[num];
		}
		else
		{
			musicRestartTime = 0f;
		}
		musicSource.Stop();
	}

	public void PauseMusic()
	{
		if (!jumpTimestamp)
		{
			musicSource.Pause();
		}
	}

	public void RestartMusic()
	{
		if (jumpTimestamp)
		{
			musicSource.Play();
			musicSource.time = musicRestartTime;
		}
		else if (!musicSource.isPlaying)
		{
			musicSource.UnPause();
		}
		jumpTimestamp = false;
	}

	public void JumpToPostHernandezDeathMusic(float delay)
	{
		GlobalCoroutineContainer.AddThread(hernandezPostDeathMusicOnDelay(delay), this);
	}

	private IEnumerator hernandezPostDeathMusicOnDelay(float delay)
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(delay, this);
		musicSource.Play();
		musicSource.time = postHernandezDeathTimestamp;
	}

	public void PlayLiftMusic()
	{
		liftMusicSource.Play();
		liftMusicPlaying = true;
	}

	public void StopLiftMusic()
	{
		liftMusicSource.Stop();
		liftMusicPlaying = false;
	}

	private void OnEnable()
	{
		if (liftMusicPlaying)
		{
			PlayLiftMusic();
		}
	}
}
