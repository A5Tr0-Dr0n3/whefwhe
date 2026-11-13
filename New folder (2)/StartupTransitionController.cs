using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class StartupTransitionController : MonoBehaviour
{
	[SerializeField]
	private GameLoadingBar loadingBar;

	[SerializeField]
	private AudioSource startLoadSource;

	[SerializeField]
	private AudioSource finishLoadSource;

	[SerializeField]
	private LoginDayNightColors dayNightColors;

	[SerializeField]
	private float fadeTime;

	[SerializeField]
	private AudioSource finishLoadingChime;

	private SafeCoroutine fadeTransitionThread;

	public void Initialize()
	{
		FadeBubbleTransitionController.MakeFullyOpaque();
		fadeTransitionThread = GlobalCoroutineContainer.AddThread(fadeIntoPreloadSequence(), this);
	}

	private IEnumerator fadeIntoPreloadSequence()
	{
		HeadsetRemovalUIManager.instance.canRemove = false;
		FadeBubbleTransitionController.SetFadeT(1f);
		PersistentDataManager.PreWarmMetaSaveFile();
		float timer = 0f;
		while (timer < fadeTime)
		{
			FadeBubbleTransitionController.SetFadeT(1f - timer / fadeTime);
			timer += Time.deltaTime;
			yield return null;
		}
		FadeBubbleTransitionController.SetFadeT(0f);
	}

	public IEnumerator LoadingSequence()
	{
		startLoadSource.Play();
		float smoothedLoadProgress;
		while (!loadingBar.TickLoadingSequence(EagerScenes.instance.GetLoadProgress(), out smoothedLoadProgress))
		{
			if (dayNightColors.gameObject.activeInHierarchy)
			{
				dayNightColors.SetTimeOffset(smoothedLoadProgress);
			}
			yield return null;
		}
		finishLoadSource.Play();
		finishLoadingChime.Play();
	}

	public IEnumerator ExitLoadingScreenSeqeunce()
	{
		if (fadeTransitionThread != null && fadeTransitionThread.IsRunning)
		{
			yield return fadeTransitionThread;
		}
		HeadsetRemovalUIManager.instance.canRemove = true;
		Teleporter.SetTeleportEnabled(enabledIn: true);
		yield return new WaitForSeconds(0.25f);
		float timer = 0f;
		while (timer < fadeTime)
		{
			FadeBubbleTransitionController.SetFadeT(timer / fadeTime);
			float scaleT = Mathf.Pow(Mathf.Clamp01(1f - timer / (fadeTime * 0.25f)), 2f);
			loadingBar.ScaleOutY(scaleT);
			timer += Time.deltaTime;
			yield return null;
		}
		FadeBubbleTransitionController.SetFadeT(0f);
		yield return EagerScenes.instance.pauseMenu.AnimateEyes(openEyes: false, immediate: true);
	}
}
