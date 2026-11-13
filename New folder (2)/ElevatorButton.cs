using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ElevatorButton : ObjectButton
{
	[SerializeField]
	private Material enabledMaterial;

	[SerializeField]
	private Material litUpMaterial;

	[SerializeField]
	private AnimationCurve retractAnimation;

	[SerializeField]
	private float retractTime = 0.5f;

	[SerializeField]
	private float retractDistance = 0.025f;

	private MeshRenderer mRenderer;

	private bool litUp;

	private bool tempDisabled;

	private bool permDisabled;

	private Vector3 startPosition;

	protected override void Awake()
	{
		base.Awake();
		mRenderer = GetComponent<MeshRenderer>();
		SetTempDisabled(isDisabled: false);
		SetPermDisabled(isDisabled: false);
		startPosition = transform.position;
	}

	public void LightUp()
	{
		litUp = true;
		setMaterialByStatus();
	}

	public void SetTempDisabled(bool isDisabled)
	{
		tempDisabled = isDisabled;
		if (tempDisabled)
		{
			isButton = false;
			ungrabbable = true;
		}
		else if (!permDisabled)
		{
			isButton = true;
			ungrabbable = false;
		}
		setMaterialByStatus();
	}

	public void SetPermDisabled(bool isDisabled, bool skipAnimation = false)
	{
		bool flag = permDisabled;
		permDisabled = isDisabled;
		if (permDisabled && permDisabled != flag)
		{
			isButton = false;
			ungrabbable = true;
			if (!skipAnimation)
			{
				StartCoroutine(retractInAnimation());
			}
			else
			{
				transform.position = startPosition + Vector3.forward * retractDistance;
			}
		}
		else if (!tempDisabled)
		{
			isButton = true;
			ungrabbable = false;
		}
		setMaterialByStatus();
	}

	private IEnumerator retractInAnimation()
	{
		float timer = 0f;
		Vector3 retractedPosition = startPosition + Vector3.forward * retractDistance;
		while (timer < retractTime)
		{
			timer += Time.deltaTime;
			float t = timer / retractTime;
			float animatedT = retractAnimation.Evaluate(t);
			transform.position = startPosition * (1f - animatedT) + retractedPosition * animatedT;
			yield return null;
		}
		transform.position = retractedPosition;
	}

	private void setMaterialByStatus()
	{
		if (litUp)
		{
			mRenderer.sharedMaterial = litUpMaterial;
		}
		else
		{
			mRenderer.sharedMaterial = enabledMaterial;
		}
	}
}
