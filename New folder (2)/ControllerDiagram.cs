using System.Collections;
using TButt;
using UnityEngine;

public class ControllerDiagram : MonoBehaviour
{
	public Transform controllerRoot;

	public Transform hackedRoot;

	public Transform mergedRoot;

	public GameObject upDownTouchpad;

	public HorizontalTouchPadScroll leftRightTouchpad;

	public AnimationClip swapVacuumAnimation;

	public AnimationClip unVacuumAnimation;

	public AnimationCurve vacuumAnimCurve;

	public Renderer appButton;

	public Renderer thumbStick;

	public Renderer touchpad;

	public Renderer topHackedTouchpad;

	public Renderer bottomHackedTouchpad;

	private Quaternion uprightRotation;

	public bool useSidewaysEulers;

	public Vector3 sidewaysEulers;

	public bool useGrabEulers;

	public Vector3 grabEulers;

	public bool useSpitEulers;

	public Vector3 spitEulers;

	public bool useSuckEulers;

	public Vector3 suckEulers;

	public Material highlightMat;

	public Material normalMat;

	private Quaternion hackedStartRotation;

	private Quaternion mergedStartRotation;

	private bool isHacked;

	private void Awake()
	{
		uprightRotation = controllerRoot.rotation;
		hackedStartRotation = hackedRoot.rotation;
		mergedStartRotation = mergedRoot.rotation;
	}

	public void StartGrabAnimation()
	{
		if (useGrabEulers && isHacked)
		{
			StopAllAnimations();
			GlobalCoroutineContainer.AddThread(rotateToAngleCoroutine(Quaternion.Euler(grabEulers), 0.5f, 1f), this);
		}
		else if (useGrabEulers)
		{
			StopAllAnimations();
			GlobalCoroutineContainer.AddThread(rotateToAngleCoroutine(Quaternion.Euler(grabEulers), 0.4f), this);
		}
		else
		{
			StopAllAnimations();
		}
		touchpad.sharedMaterial = highlightMat;
	}

	public void StartUpDownAnimation()
	{
		StopAllAnimations();
		upDownTouchpad.SetActive(value: true);
	}

	public void StartTeleportAnimation()
	{
		StopAllAnimations();
		if (useSidewaysEulers)
		{
			GlobalCoroutineContainer.AddThread(rotateToAngleCoroutine(Quaternion.Euler(sidewaysEulers), 0.4f), this);
		}
		appButton.sharedMaterial = highlightMat;
	}

	public void StartTrippleTapAnimation()
	{
		StopAllAnimations();
		if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
		{
			thumbStick.sharedMaterial = highlightMat;
		}
		else
		{
			appButton.sharedMaterial = highlightMat;
		}
	}

	public void StartVacuumInAnimation()
	{
		StopAllAnimations(ignoreHacked: true);
		if (useSuckEulers)
		{
			GlobalCoroutineContainer.AddThread(rotateToAngleCoroutine(Quaternion.Euler(suckEulers), 0.4f), this);
		}
		bottomHackedTouchpad.gameObject.SetActive(value: true);
	}

	public void StartVacuumOutAnimation()
	{
		StopAllAnimations(ignoreHacked: true);
		if (useSpitEulers)
		{
			GlobalCoroutineContainer.AddThread(rotateToAngleCoroutine(Quaternion.Euler(spitEulers), 0.4f), this);
		}
		topHackedTouchpad.gameObject.SetActive(value: true);
	}

	public void StopAllAnimations(bool ignoreHacked = false)
	{
		leftRightTouchpad.gameObject.SetActive(value: false);
		upDownTouchpad.SetActive(value: false);
		GlobalCoroutineContainer.AddThread(rotateToAngleCoroutine(uprightRotation, 0.4f), this);
		appButton.sharedMaterial = normalMat;
		thumbStick.sharedMaterial = normalMat;
		touchpad.sharedMaterial = normalMat;
		topHackedTouchpad.gameObject.SetActive(value: false);
		bottomHackedTouchpad.gameObject.SetActive(value: false);
		EnableController();
		if (isHacked && !ignoreHacked)
		{
			UnFlipAnimation(0.6f);
		}
	}

	public void DisableController()
	{
		StopAllAnimations();
		controllerRoot.gameObject.SetActive(value: false);
	}

	public void EnableController()
	{
		controllerRoot.gameObject.SetActive(value: true);
	}

	public void StartFlipAnimtaion(float time)
	{
		StopAllAnimations();
		isHacked = true;
		GlobalCoroutineContainer.AddThread(flipAnimationCoroutine(time), this);
	}

	private IEnumerator flipAnimationCoroutine(float time)
	{
		float timer = 0f;
		leftRightTouchpad.gameObject.SetActive(value: true);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		if (leftRightTouchpad.enabled)
		{
			yield return leftRightTouchpad.StartMove(0.5f);
		}
		else
		{
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		}
		leftRightTouchpad.gameObject.SetActive(value: false);
		while (timer < 1f)
		{
			swapVacuumAnimation.SampleAnimation(controllerRoot.gameObject, vacuumAnimCurve.Evaluate(timer));
			timer += Time.deltaTime / time;
			yield return null;
		}
		yield return null;
	}

	public void UnFlipAnimation(float time)
	{
		isHacked = false;
		GlobalCoroutineContainer.AddThread(unFlipAnimationCoroutine(time), this);
	}

	private IEnumerator unFlipAnimationCoroutine(float time)
	{
		float timer = 0f;
		while (timer < 1f)
		{
			unVacuumAnimation.SampleAnimation(controllerRoot.gameObject, vacuumAnimCurve.Evaluate(timer));
			timer += Time.deltaTime / time;
			yield return null;
		}
		float rotationTimer = 0f;
		Quaternion baseHackRotation = hackedRoot.rotation;
		Quaternion baseMergeRotation = mergedRoot.rotation;
		if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
		{
			baseHackRotation = hackedRoot.localRotation;
			baseMergeRotation = mergedRoot.localRotation;
		}
		while (rotationTimer < 1f)
		{
			rotationTimer += Time.deltaTime / (time * 2f);
			if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
			{
				hackedRoot.localRotation = Quaternion.Lerp(baseHackRotation, Quaternion.identity, rotationTimer);
				mergedRoot.localRotation = Quaternion.Lerp(baseMergeRotation, Quaternion.identity, rotationTimer);
			}
			else
			{
				hackedRoot.rotation = Quaternion.Slerp(baseHackRotation, hackedStartRotation, rotationTimer);
				mergedRoot.rotation = Quaternion.Slerp(baseMergeRotation, mergedStartRotation, rotationTimer);
			}
			yield return null;
		}
		if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
		{
			hackedRoot.localRotation = Quaternion.identity;
			mergedRoot.localRotation = Quaternion.identity;
		}
		yield return null;
	}

	private IEnumerator rotateToAngleCoroutine(Quaternion angle, float duration)
	{
		Quaternion startAngle = controllerRoot.transform.rotation;
		float timer = 0f;
		while (timer < 1f)
		{
			timer += Time.deltaTime / duration;
			controllerRoot.transform.rotation = Quaternion.Slerp(startAngle, angle, timer);
			yield return null;
		}
	}

	private IEnumerator rotateToAngleCoroutine(Quaternion angle, float duration, float delay)
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(delay, this);
		yield return GlobalCoroutineContainer.AddThread(rotateToAngleCoroutine(angle, duration), this);
	}

	private void OnDisable()
	{
		controllerRoot.transform.rotation = uprightRotation;
	}
}
