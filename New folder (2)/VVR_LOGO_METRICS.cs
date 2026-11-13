using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VVR_LOGO_METRICS : MonoBehaviour
{
	[SerializeField]
	private ShaderVariantCollection shaderCollection1;

	[SerializeField]
	private TextMeshPro firstFrameIndicator;

	[SerializeField]
	private TextMeshPro shaderVariantTimerOutput;

	[SerializeField]
	private TextMeshPro loadProgressOutput;

	[SerializeField]
	private TextMeshPro loadTimerOutput;

	private AsyncOperation rootLoadOperation;

	private void Start()
	{
		StartCoroutine(fadeSequence());
	}

	private IEnumerator fadeSequence()
	{
		yield return null;
		firstFrameIndicator.text = "Past First Frame";
		firstFrameIndicator.color = Color.white;
		yield return null;
		shaderVariantTimerOutput.color = Color.green;
		yield return null;
		DateTime startTime = DateTime.Now;
		shaderCollection1.WarmUp();
		TimeSpan loadTime = DateTime.Now - startTime;
		shaderVariantTimerOutput.text = loadTime.TotalSeconds.ToString("F5");
		yield return new WaitForSeconds(3f);
		loadProgressOutput.color = Color.green;
		loadTimerOutput.color = Color.green;
		yield return null;
		rootLoadOperation = SceneManager.LoadSceneAsync("VVR_ROOT", LoadSceneMode.Single);
		float timer = 0f;
		while (!rootLoadOperation.isDone)
		{
			loadProgressOutput.text = rootLoadOperation.progress.ToString("F5");
			TextMeshPro textMeshPro = loadTimerOutput;
			float num;
			timer = (num = timer + Time.deltaTime);
			num = num;
			textMeshPro.text = num.ToString("F5");
			yield return null;
		}
		loadTimerOutput.text = timer.ToString("F5");
		loadProgressOutput.text = rootLoadOperation.progress.ToString("F5");
		yield return new WaitForSeconds(3f);
	}
}
