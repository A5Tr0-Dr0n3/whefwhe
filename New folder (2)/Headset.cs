using TButt;
using UnityEngine;

public class Headset : MonoBehaviour
{
	public TCPlatformScene Scene;

	[HideInInspector]
	public float timeSinceRemoved = 99f;

	public bool isActivated = true;

	[HideInInspector]
	public bool headsetHasBeenSafelyRemoved = true;

	public const float rotationTriggerDistance = 2f;

	public const float addHeadsetTriggerDistance = 1f;

	[HideInInspector]
	public bool insideRotationTrigger;

	private bool isAddingHeadset;

	private bool wasGrabbed;

	private Quaternion headsetRotation;

	[HideInInspector]
	public Rigidbody rb;

	[HideInInspector]
	public bool hasBeenUsed;

	public Vector3 spawnPos = Vector3.zero;

	public float spawnScale = 1f;

	private Vector3 originalScale;

	[HideInInspector]
	public float headsetDropMinTime = 0.5f;

	private float headsetDistance;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		if (spawnPos != Vector3.zero)
		{
			hasBeenUsed = true;
		}
		originalScale = transform.localScale;
	}

	public void UpdateHeadset(Manipulator grabber)
	{
		if (!(grabber == null) && !(grabber.HeldObjectBase == null) && !(grabber.HeldObjectBase.grabbedBy[0] == null) && !(grabber != grabber.HeldObjectBase.grabbedBy[0]))
		{
			headsetDistance = Vector3.Distance(transform.position, EagerMaster.instance.mainCamera.transform.position);
			if (headsetDistance < 1f && insideRotationTrigger && !isAddingHeadset && EagerScenes.SceneIsLoaded(Scene) && headsetHasBeenSafelyRemoved)
			{
				StartAddingHeadset();
			}
			timeSinceRemoved += Time.deltaTime;
			if ((headsetDistance > 1.2f && timeSinceRemoved > headsetDropMinTime) || (timeSinceRemoved > 1.5f && headsetDistance < 0.5f))
			{
				headsetHasBeenSafelyRemoved = true;
			}
			if (!TBInput.GetButton(TBInput.Button.Action1, grabber.GrabbingToolGroup.Controller) && timeSinceRemoved < headsetDropMinTime && !headsetHasBeenSafelyRemoved)
			{
				ToolManager.PlayerGrabbingGameObject(gameObject, dropIfGrabbing: true);
			}
			if (!isAddingHeadset)
			{
				UpdateHeadRotation();
			}
		}
	}

	private void UpdateHeadRotation()
	{
		if (headsetDistance < 2f && ToolManager.PlayerGrabbingGameObject(gameObject))
		{
			ObjectBoid component = GetComponent<ObjectBoid>();
			if (!insideRotationTrigger)
			{
				headsetRotation = transform.rotation;
			}
			wasGrabbed = true;
			rb.angularVelocity = Vector3.zero;
			if (component != null)
			{
				if (component.hFilter != null && component.hFilter.transform.localRotation.eulerAngles.y == 180f)
				{
					headsetRotation = Quaternion.Lerp(headsetRotation, Quaternion.LookRotation(-(transform.position - EagerMaster.instance.mainCamera.transform.position)), 0.05f);
					if (component.eyeBlack != null)
					{
						component.eyeBlack.transform.localRotation = Quaternion.identity;
					}
					SafeDebug.DrawRay(transform.position, -(transform.position - EagerMaster.instance.mainCamera.transform.position), Color.cyan);
				}
				else
				{
					headsetRotation = Quaternion.Lerp(headsetRotation, Quaternion.LookRotation(transform.position - EagerMaster.instance.mainCamera.transform.position), 0.05f);
					if (component.eyeBlack != null)
					{
						component.eyeBlack.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
					}
					SafeDebug.DrawRay(transform.position, transform.position - EagerMaster.instance.mainCamera.transform.position, Color.cyan);
				}
			}
			else
			{
				headsetRotation = Quaternion.Lerp(headsetRotation, Quaternion.LookRotation(transform.position - EagerMaster.instance.mainCamera.transform.position), 0.05f);
				SafeDebug.DrawRay(transform.position, transform.position - EagerMaster.instance.mainCamera.transform.position, Color.cyan);
			}
			transform.rotation = headsetRotation;
			insideRotationTrigger = true;
		}
		else
		{
			wasGrabbed = false;
			insideRotationTrigger = false;
		}
	}

	public void StartAddingHeadset()
	{
		if (isActivated && !EagerHeadsetManager.instance.isAddingHeadset)
		{
			GetComponent<Collider>().enabled = false;
			rb.isKinematic = true;
			SendToEagerHeadset();
			ToolManager.PlayerGrabbingGameObject(gameObject, dropIfGrabbing: true);
		}
	}

	public void SendToEagerHeadset()
	{
		if (!EagerHeadsetManager.instance.isAddingHeadset)
		{
			isAddingHeadset = true;
			EagerHeadsetManager.instance.headset = gameObject;
			EagerHeadsetManager.instance.Scene = Scene;
			EagerHeadsetManager.instance.activeHeadset = this;
			EagerScenes.instance.activeHeadset = this;
			EagerMaster.instance.isSwitchingScenes = true;
			HeadsetRemovalUIManager.instance.StopInProgressRemoval();
			EagerHeadsetManager.instance.AddHeadset();
			enabled = false;
		}
	}

	public void ResetHeadset()
	{
		transform.parent = EagerScenes.instance.ActiveScene.transform;
		gameObject.SetActive(value: true);
		enabled = true;
		isAddingHeadset = false;
		wasGrabbed = false;
		insideRotationTrigger = false;
		if ((bool)GetComponent<BoxCollider>())
		{
			GetComponent<BoxCollider>().enabled = true;
		}
		if (rb != null)
		{
			rb.useGravity = true;
			rb.isKinematic = false;
		}
	}

	public void ResetPosition()
	{
		hasBeenUsed = false;
		spawnPos = Vector3.zero;
	}

	public void ResetScale()
	{
		transform.localScale = originalScale;
	}
}
