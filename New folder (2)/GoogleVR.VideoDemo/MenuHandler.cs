using System.Collections;
using UnityEngine;

namespace GoogleVR.VideoDemo;

public class MenuHandler : MonoBehaviour
{
	public GameObject[] menuObjects;

	public void HideMenu()
	{
		GameObject[] array = menuObjects;
		foreach (GameObject gameObject in array)
		{
			Renderer component = gameObject.GetComponent<Renderer>();
			if (component != null)
			{
				component.enabled = false;
			}
			else
			{
				gameObject.SetActive(value: false);
			}
			StartCoroutine(DoFade());
		}
	}

	public void ShowMenu()
	{
		GameObject[] array = menuObjects;
		foreach (GameObject gameObject in array)
		{
			Renderer component = gameObject.GetComponent<Renderer>();
			if (component != null)
			{
				component.enabled = true;
			}
			else
			{
				gameObject.SetActive(value: true);
			}
		}
		StartCoroutine(DoAppear());
	}

	private IEnumerator DoAppear()
	{
		CanvasGroup cg = GetComponent<CanvasGroup>();
		while ((double)cg.alpha < 1.0)
		{
			cg.alpha += Time.deltaTime * 2f;
			yield return null;
		}
		cg.interactable = true;
	}

	private IEnumerator DoFade()
	{
		CanvasGroup cg = GetComponent<CanvasGroup>();
		while (cg.alpha > 0f)
		{
			cg.alpha -= Time.deltaTime;
			yield return null;
		}
		cg.interactable = false;
	}
}
