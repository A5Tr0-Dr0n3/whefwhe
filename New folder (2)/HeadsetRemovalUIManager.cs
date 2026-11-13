using System.Collections;
using System.Collections.Generic;
using GVR.Gfx;
using TButt;
using UnityEngine;

public class HeadsetRemovalUIManager : MonoBehaviour
{
	public enum HeadsetRemovalFailureType
	{
		needAdmin,
		forceDisabled
	}

	public static HeadsetRemovalUIManager instance;

	public bool canRemove = true;

	[SerializeField]
	private GameObject[] cantRemoveAdminUI;

	[SerializeField]
	private GameObject[] cantRemoveForceUI;

	[Range(0f, 1f)]
	public float value;

	private float ceiling = 1f;

	private Vector3 pupilStartPos;

	private Vector3 pupilEndPos;

	public AudioLoop removalLoop;

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve removalLoopCurve;

	public AudioEvent deniedSound;

	public AudioEvent headsetGrabSFX;

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve lowpassCurve;

	[HideInInspector]
	public float currentFisheyeScalar = 1f;

	private Transform headTransform;

	private float fadeLerp;

	private float tempFisheye;

	private Vector3 projectedVector;

	private Vector3 lockVector;

	private Vector3 initialVector;

	private Vector3 planeNormal;

	private Vector3 lockedforwardVector;

	private Quaternion initialRotation;

	private float valDiff;

	private ToolGroup _grabbingGroup;

	private Dictionary<ToolGroup, bool> _lastFrameAtHead;

	private float lastThreshold;

	[SerializeField]
	private float rumbleDelta = 0.05f;

	private bool pullingOff;

	private bool planeLocked;

	private Quaternion lockRotation;

	private float pullAngle;

	private float removalThreshold = 40f;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		for (int i = 0; i < cantRemoveAdminUI.Length; i++)
		{
			cantRemoveAdminUI[i].SetActive(value: false);
		}
		for (int j = 0; j < cantRemoveForceUI.Length; j++)
		{
			cantRemoveForceUI[j].SetActive(value: false);
		}
		headTransform = Camera.main.transform;
		_lastFrameAtHead = new Dictionary<ToolGroup, bool>();
	}

	private void Update()
	{
		if (canRemove && !EagerHeadsetManager.instance.isAddingHeadset)
		{
			HeadSetRemovalLogic();
			if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
			{
				HeadsetRumbleLogic();
			}
		}
		if (value > 0f)
		{
			if (!pullingOff)
			{
				value -= Time.deltaTime * 10f;
				if (value < 0f)
				{
					value = 0f;
				}
			}
			if (value > 0.001f)
			{
				EnvironmentProperties.fishEyeStrength = (0f - EagerHeadsetManager.instance.fisheyeStrength) * currentFisheyeScalar * value;
				EagerMaster.instance.gvrMixer.SetFloat("lowpassFilter", 4000f * lowpassCurve.Evaluate(value));
				EagerMaster.instance.flatMixer.SetFloat("lowpassFilter", 4000f * lowpassCurve.Evaluate(value));
			}
			else
			{
				value = 0f;
				EnvironmentProperties.fishEyeStrength = (0f - EagerHeadsetManager.instance.fisheyeStrength) * currentFisheyeScalar * value;
			}
		}
		else
		{
			EagerMaster.instance.gvrMixer.SetFloat("lowpassFilter", 22000f);
			EagerMaster.instance.flatMixer.SetFloat("lowpassFilter", 22000f);
		}
	}

	public void CannotTakeOffHeadset(HeadsetRemovalFailureType adminOrForce)
	{
		value = 0f;
		tempFisheye = EnvironmentProperties.fishEyeStrength;
		EagerHeadsetManager.instance.SetEnableFisheye();
		EnvironmentProperties.fishEyeStrength = tempFisheye;
		EagerHeadsetManager.instance.SetRemoveFisheye();
		StartCoroutine(RejectHeadsetCoroutine(adminOrForce == HeadsetRemovalFailureType.needAdmin));
	}

	private IEnumerator RejectHeadsetCoroutine(bool adminOrForce)
	{
		GameObject[] doNotRemoveIcons = ((!adminOrForce) ? cantRemoveForceUI : cantRemoveAdminUI);
		for (int i = 0; i < doNotRemoveIcons.Length; i++)
		{
			doNotRemoveIcons[i].SetActive(value: true);
		}
		deniedSound.Play();
		fadeLerp = 0.5f;
		while (fadeLerp > 0f)
		{
			fadeLerp -= Time.deltaTime;
			yield return null;
		}
		for (int j = 0; j < doNotRemoveIcons.Length; j++)
		{
			doNotRemoveIcons[j].SetActive(value: false);
		}
	}

	private void HeadSetRemovalLogic()
	{
		if (_grabbingGroup == null)
		{
			for (int i = 0; i < ToolManager.ActiveToolGroups.Count; i++)
			{
				if (PointingAtHead(ToolManager.ActiveToolGroups[i].Controller) && TBInput.GetButtonDown(TBInput.Button.Action1, ToolManager.ActiveToolGroups[i].Controller) && !ToolManager.ActiveToolGroups[i].Manipulator.isGrabbing())
				{
					removalLoop.Play();
					headsetGrabSFX.Play();
					initialRotation = EagerMaster.instance.player.transform.rotation * TBInput.GetRotation(ToolManager.ActiveToolGroups[i].Controller);
					initialVector = initialRotation * Vector3.forward;
					pullingOff = true;
					value = 0f;
					planeLocked = false;
					_grabbingGroup = ToolManager.ActiveToolGroups[i];
				}
			}
		}
		if (_grabbingGroup != null && TBInput.GetButtonUp(TBInput.Button.Action1, _grabbingGroup.Controller))
		{
			StopInProgressRemoval();
		}
		if (pullingOff)
		{
			valDiff = value;
			UpdateRemovalValue();
			valDiff = value - valDiff;
			removalLoop.SetSourceVolume(0.03f + Mathf.Lerp(removalLoop.flatSource.volume, 80f * Mathf.Abs(valDiff), 15f * Time.deltaTime));
		}
	}

	public void StopInProgressRemoval()
	{
		pullingOff = false;
		removalLoop.Stop();
		_grabbingGroup = null;
	}

	public bool PointingAtHead(TBInput.Controller controller)
	{
		if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
		{
			Vector3 vector = Vector3.Normalize(EagerMaster.instance.mainCamera.transform.position - ToolManager.GetToolGroupForController(controller).Manipulator.transform.position);
			float num = 0f;
			num = ((EagerMaster.instance.player.transform.rotation.eulerAngles.y == 0f) ? Vector3.Dot(vector, TBInput.GetPointerRotation(controller) * Vector3.forward) : Vector3.Dot(Quaternion.Euler(0f, 180f, 0f) * vector, TBInput.GetPointerRotation(controller) * Vector3.forward));
			return num > 0.8f;
		}
		float num2 = Vector3.Dot(headTransform.up, EagerMaster.instance.player.transform.rotation * TBInput.GetRotation(controller) * Vector3.forward);
		float num3 = Vector3.Dot(headTransform.forward, EagerMaster.instance.player.transform.rotation * TBInput.GetRotation(controller) * Vector3.forward);
		return num2 > 0f && num3 < 0.2f;
	}

	private void HeadsetRumbleLogic()
	{
		if (pullingOff)
		{
			if (Mathf.Abs(lastThreshold - value) > rumbleDelta)
			{
				TBInput.SetRumble(_grabbingGroup.Controller, 0.15f);
				lastThreshold = value;
			}
			return;
		}
		lastThreshold = 0f;
		for (int i = 0; i < ToolManager.ActiveToolGroups.Count; i++)
		{
			if (!_lastFrameAtHead.ContainsKey(ToolManager.ActiveToolGroups[i]))
			{
				_lastFrameAtHead.Add(ToolManager.ActiveToolGroups[i], value: false);
			}
			bool flag = _lastFrameAtHead[ToolManager.ActiveToolGroups[i]];
			bool flag2 = PointingAtHead(ToolManager.ActiveToolGroups[i].Controller);
			if (flag != flag2)
			{
				if (flag2)
				{
					TBInput.SetRumble(ToolManager.ActiveToolGroups[i].Controller, 0.3f);
				}
				_lastFrameAtHead[ToolManager.ActiveToolGroups[i]] = flag2;
			}
		}
	}

	private void UpdateRemovalValue()
	{
		if (!planeLocked)
		{
			pullAngle = Quaternion.Angle(initialRotation, EagerMaster.instance.player.transform.rotation * TBInput.GetRotation(_grabbingGroup.Controller));
			if (pullAngle > removalThreshold * 0.5f)
			{
				planeLocked = true;
				lockRotation = EagerMaster.instance.player.transform.rotation * TBInput.GetRotation(_grabbingGroup.Controller);
				lockVector = lockRotation * Vector3.forward;
				planeNormal = Vector3.Cross(initialVector, lockVector);
				lockedforwardVector = Vector3.Cross(planeNormal, initialVector);
			}
		}
		else
		{
			projectedVector = Vector3.ProjectOnPlane(EagerMaster.instance.player.transform.rotation * TBInput.GetRotation(_grabbingGroup.Controller) * Vector3.forward, planeNormal);
			if (Vector3.Dot(lockedforwardVector, EagerMaster.instance.player.transform.rotation * TBInput.GetRotation(_grabbingGroup.Controller) * Vector3.forward) > 0f)
			{
				pullAngle = Vector3.Angle(initialVector, projectedVector);
			}
		}
		value = Mathf.Clamp(pullAngle / removalThreshold, 0f, 9999f);
		if (value >= ceiling)
		{
			pullingOff = false;
			value = 0f;
			removalLoop.Stop();
			EagerHeadsetManager.instance.RemoveHeadset(_grabbingGroup.Manipulator);
			_grabbingGroup = null;
		}
	}
}
