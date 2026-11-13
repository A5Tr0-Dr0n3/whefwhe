using System.Collections;
using UnityEngine;

public class PupilTransitionController : MonoBehaviour
{
	public enum PauseType
	{
		controllerDisconnected,
		userPause
	}

	[SerializeField]
	private AudioEvent loadingWhoosh;

	public static PupilTransitionController instance;

	public GameObject pupilMask;

	private float pupilLerp;

	private float pupilLerpMax = 1f;

	private bool eyeOpen;

	private Vector3 maxPupilDistance = Vector3.forward * 15f;

	private Vector3 minPupilDistance = Vector3.forward * 0.12f;

	private Coroutine eyeCoroutine;

	private Vector3 startPos;

	private Vector3 endPos;

	private float pupilVal;

	private void Awake()
	{
		if (instance != null)
		{
			throw new UnityException("Only one instance of PauseMenu in a scene is allowed.");
		}
		instance = this;
	}

	public void FinishedLoading()
	{
		transform.position = EagerMaster.instance.mainCamera.transform.position;
	}

	public Coroutine AnimateEyes(bool openEyes, bool immediate = false)
	{
		if (eyeCoroutine == null && (immediate || openEyes != eyeOpen))
		{
			eyeCoroutine = StartCoroutine(AnimatePupils(openEyes, immediate));
			return eyeCoroutine;
		}
		return null;
	}

	private IEnumerator AnimatePupils(bool openEyes, bool immediate = false)
	{
		pupilLerp = ((!immediate) ? 0f : pupilLerpMax);
		FadeBubbleTransitionController.MakeFullyTransparent();
		if (openEyes)
		{
			if (!immediate)
			{
				WhooshSFX();
			}
			startPos = maxPupilDistance;
			endPos = minPupilDistance;
		}
		else
		{
			startPos = minPupilDistance;
			endPos = maxPupilDistance;
		}
		pupilMask.SetActive(value: true);
		pupilMask.transform.parent = EagerMaster.instance.mainCamera.transform;
		pupilMask.transform.localPosition = startPos;
		pupilMask.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
		if (openEyes && !immediate)
		{
			EagerHeadsetManager.instance.SetEnableFisheye();
		}
		while (pupilLerp / pupilLerpMax < 1f)
		{
			pupilLerp += Time.deltaTime;
			pupilVal = pupilLerp / pupilLerpMax;
			if (!openEyes)
			{
				pupilVal = Mathf.Pow(pupilVal, 8f);
			}
			pupilMask.transform.localPosition = Vector3.Lerp(startPos, endPos, pupilVal);
			yield return null;
		}
		if (openEyes)
		{
			FadeBubbleTransitionController.MakeFullyTransparent();
			eyeOpen = true;
		}
		else
		{
			FadeBubbleTransitionController.MakeFullyOpaque();
			eyeOpen = false;
		}
		pupilMask.transform.parent = transform;
		pupilMask.SetActive(value: false);
		eyeCoroutine = null;
		if (openEyes && !immediate)
		{
			EagerHeadsetManager.instance.SetRemoveFisheye();
		}
	}

	public void SnapEyesClosed()
	{
	}

	public void WhooshSFX()
	{
		loadingWhoosh.Play();
	}
}
