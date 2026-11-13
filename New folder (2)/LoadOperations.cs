using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadOperations : MonoBehaviour
{
	public Image myImage;

	public bool UseAsync;

	private AsyncOperation async;

	public int LevelToLoad;

	public float FadeoutTime;

	public float fadeSpeed = 1.5f;

	private bool fadeout;

	private bool fadein;

	public void FadeOut()
	{
		fadein = false;
		fadeout = true;
		SafeDebug.Log("Fading Out");
	}

	public void FadeIn()
	{
		fadeout = false;
		fadein = true;
		SafeDebug.Log("Fading In");
	}

	private void Update()
	{
		if (async != null)
		{
			SafeDebug.Log(async.progress.ToString());
			if ((double)async.progress >= 1.0)
			{
				async = null;
				FadeIn();
			}
		}
		if (fadeout)
		{
			myImage.color = Color.Lerp(myImage.color, Color.black, fadeSpeed * Time.deltaTime);
			if ((double)myImage.color.a >= 0.999)
			{
				StartCoroutine("LoadALevel");
				fadeout = false;
			}
		}
		if (fadein)
		{
			myImage.color = Color.Lerp(myImage.color, new Color(0f, 0f, 0f, 0f), fadeSpeed * Time.deltaTime);
			if ((double)myImage.color.a <= 0.01)
			{
				fadein = false;
			}
		}
	}

	public IEnumerator LoadALevel()
	{
		async = SceneManager.LoadSceneAsync(LevelToLoad);
		yield return async;
	}
}
