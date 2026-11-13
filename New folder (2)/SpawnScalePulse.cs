using System.Collections;
using UnityEngine;

public class SpawnScalePulse : MonoBehaviour
{
	private Vector3 startScale;

	private Vector3 vanishScale;

	private float lerpVal;

	public float lerpSpeed = 8f;

	public float pulseSpeed = 1f;

	private Coroutine scaleCor;

	private bool scalingDown;

	private Quaternion startRotation;

	private Quaternion twisted;

	private Vector3 pulseScale;

	public float pulseScalar = 1.5f;

	private void Awake()
	{
		startScale = transform.localScale;
		pulseScale = startScale * pulseScalar;
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
		while (lerpVal < 1f)
		{
			lerpVal += Time.deltaTime * lerpSpeed;
			transform.localScale = Vector3.Lerp(Vector3.zero, startScale, lerpVal);
			yield return null;
		}
		while (true)
		{
			transform.localScale = Vector3.Lerp(startScale, pulseScale, Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f);
			yield return null;
		}
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
