using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VVR_LOGO : MonoBehaviour
{
	[SerializeField]
	private float fadeTime;

	[SerializeField]
	private Renderer fadeImage;

	[SerializeField]
	private ShaderVariantCollection shaderCollection1;

	[SerializeField]
	private Transform logoTransform;

	[SerializeField]
	private float logoDistanceFromFace = 14f;

	[SerializeField]
	private GameObject tenderClawsLogo;

	[SerializeField]
	private GameObject vvrLogo;

	[SerializeField]
	private GameObject loadFailMessage;

	private AsyncOperation rootLoadOperation;

	private void Start()
	{
		StartCoroutine(fadeSequence());
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	private IEnumerator fadeSequence()
	{
		vvrLogo.SetActive(value: false);
		yield return null;
		vvrLogo.SetActive(value: true);
		DateTime startTime = DateTime.Now;
		logoTransform.position = Camera.main.transform.position + Camera.main.transform.forward * logoDistanceFromFace;
		logoTransform.rotation = Quaternion.LookRotation(-(Camera.main.transform.position - logoTransform.position), Vector3.up);
		yield return null;
		shaderCollection1.WarmUp();
		yield return null;
		bool rootStuck = false;
		rootLoadOperation = SceneManager.LoadSceneAsync("VVR_ROOT", LoadSceneMode.Single);
		rootLoadOperation.allowSceneActivation = false;
		yield return null;
		if (rootLoadOperation.progress > 0.89f)
		{
			try
			{
				AudioClip audioClip = UnityEngine.Object.Instantiate(Resources.Load("Satisfied", typeof(AudioClip))) as AudioClip;
				SafeDebug.Log("loadState: " + audioClip.loadState);
				if (audioClip.loadState != AudioDataLoadState.Loaded)
				{
					rootStuck = true;
				}
			}
			catch (Exception ex)
			{
				rootStuck = true;
				SafeDebug.Log("UnityException:" + ex);
			}
		}
		TimeSpan loadTime = DateTime.Now - startTime;
		if (loadTime.TotalSeconds < 1.5)
		{
			yield return new WaitForSeconds(1.5f - (float)loadTime.TotalSeconds);
		}
		else
		{
			yield return null;
		}
		fadeImage.gameObject.SetActive(value: true);
		float timer = 0f;
		while (timer < fadeTime)
		{
			timer += Time.deltaTime;
			fadeImage.sharedMaterial.SetColor("_Color", new Color(0f, 0f, 0f, timer / fadeTime));
			yield return null;
		}
		timer = 0f;
		fadeImage.sharedMaterial.SetColor("_Color", new Color(0f, 0f, 0f, 1f));
		vvrLogo.SetActive(value: false);
		while (timer < 0.5f)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		if (rootStuck)
		{
			logoTransform.position = Camera.main.transform.position + Vector3.forward * logoDistanceFromFace;
			logoTransform.rotation = Quaternion.identity;
			loadFailMessage.SetActive(value: true);
			timer = 0f;
			while (timer < fadeTime)
			{
				fadeImage.sharedMaterial.SetColor("_Color", new Color(0f, 0f, 0f, 1f - timer / fadeTime));
				timer += Time.deltaTime;
				yield return null;
			}
			while (true)
			{
				yield return null;
			}
		}
		tenderClawsLogo.SetActive(value: true);
		timer = 0f;
		while (timer < fadeTime)
		{
			fadeImage.sharedMaterial.SetColor("_Color", new Color(0f, 0f, 0f, 1f - timer / fadeTime));
			timer += Time.deltaTime;
			yield return null;
		}
		fadeImage.sharedMaterial.SetColor("_Color", new Color(0f, 0f, 0f, 0f));
		timer = 0f;
		while (timer < 1.5f || rootLoadOperation.progress < 0.9f)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		timer = 0f;
		while (timer < 0.35f)
		{
			fadeImage.sharedMaterial.SetColor("_Color", new Color(0f, 0f, 0f, timer / 0.35f));
			timer += Time.deltaTime;
			yield return null;
		}
		fadeImage.sharedMaterial.SetColor("_Color", new Color(0f, 0f, 0f, 1f));
		rootLoadOperation.allowSceneActivation = true;
		yield return null;
		while (!rootLoadOperation.isDone)
		{
			yield return null;
		}
	}
}
