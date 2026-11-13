using System.Collections;
using UnityEngine;

public class SpawnScale : MonoBehaviour
{
	private Vector3 startScale;

	private Vector3 vanishScale;

	private float lerpVal;

	private float lerpSpeed = 8f;

	private Coroutine scaleCor;

	private bool scalingDown;

	public bool rotate;

	private Quaternion startRotation;

	public WorldControllerSwapper _worldControllerSwapper;

	private void Awake()
	{
		startScale = transform.localScale;
		startRotation = transform.rotation;
	}

	private void OnEnable()
	{
		if (!scalingDown)
		{
			scaleCor = StartCoroutine(ScaleUp());
		}
	}

	private IEnumerator ScaleUp()
	{
		lerpVal = 0f;
		transform.rotation = startRotation;
		while (lerpVal < 1f)
		{
			lerpVal += Time.deltaTime * lerpSpeed;
			transform.localScale = Vector3.Lerp(Vector3.zero, startScale, lerpVal);
			yield return null;
		}
		lerpVal = 0f;
		while (lerpVal < 1f)
		{
			lerpVal += Time.deltaTime * 0.08f;
			if (_worldControllerSwapper != null && _worldControllerSwapper.worldControllerModel != null)
			{
				if (rotate && Mathf.Abs(transform.rotation.y - startRotation.y) < 10f)
				{
					_worldControllerSwapper.worldControllerModel.transform.Rotate(Vector3.up * 2.5f * Time.deltaTime, Space.World);
				}
			}
			else if (rotate && Mathf.Abs(transform.rotation.y - startRotation.y) < 10f)
			{
				transform.Rotate(Vector3.up * 2.5f * Time.deltaTime, Space.World);
			}
			yield return null;
		}
		transform.localScale = startScale;
		scaleCor = null;
	}

	private IEnumerator ScaleDown()
	{
		lerpVal = 0f;
		while (lerpVal < 1f)
		{
			lerpVal += Time.deltaTime * lerpSpeed;
			transform.localScale = Vector3.Lerp(startScale, Vector3.zero, lerpVal);
			yield return null;
		}
		transform.localScale = Vector3.zero;
		scaleCor = null;
		gameObject.SetActive(value: false);
		scalingDown = false;
	}

	public void Disable()
	{
		if (gameObject.activeInHierarchy && transform.localScale != Vector3.zero)
		{
			if (scaleCor != null)
			{
				StopCoroutine(scaleCor);
			}
			scalingDown = true;
			scaleCor = StartCoroutine(ScaleDown());
		}
	}
}
