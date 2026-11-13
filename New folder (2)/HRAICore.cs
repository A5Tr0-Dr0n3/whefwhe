using UnityEngine;

public class HRAICore : ObjectBase
{
	[HideInInspector]
	public bool fixedJointBroken;

	[HideInInspector]
	public bool isDisconnected;

	[SerializeField]
	private GvrAudioSource dyingSound;

	[SerializeField]
	private GvrAudioSource breakSound;

	[SerializeField]
	private GameObject explosionParent;

	[SerializeField]
	private GameObject chazHat;

	[SerializeField]
	private ConfigurableJoint cJoint;

	[SerializeField]
	public FixedJoint fJoint;

	[SerializeField]
	public bool isChaz;

	private Vector3 startPos;

	protected override void Awake()
	{
		base.Awake();
		startPos = transform.localPosition;
		changeHeldLayer = false;
	}

	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		if ((bool)fJoint && !fixedJointBroken)
		{
			fJoint.breakForce = 200f;
			fJoint.breakTorque = 200f;
		}
		rb.isKinematic = false;
	}

	public override void GrabRelease(Manipulator grabber)
	{
		base.GrabRelease(grabber);
		if ((bool)fJoint && !fixedJointBroken)
		{
			fJoint.breakForce = float.PositiveInfinity;
			fJoint.breakTorque = float.PositiveInfinity;
		}
	}

	private void Update()
	{
		if (!isChaz)
		{
			ungrabbable = !VVR_HR.instance.canGrabCores;
		}
		else
		{
			Vector3 normalized = (EagerMaster.instance.player.transform.position - transform.position).normalized;
			Vector3 forward = normalized - Vector3.Project(normalized, transform.up);
			transform.rotation = Quaternion.LookRotation(forward);
			Vector3 localEulerAngles = transform.localEulerAngles;
			localEulerAngles.x = 0f;
			localEulerAngles.z = 0f;
			transform.localEulerAngles = localEulerAngles;
		}
		if (!cJoint)
		{
			return;
		}
		if (isChaz && transform.localPosition.y > 2.525f)
		{
			rb.isKinematic = true;
			transform.localPosition = new Vector3(startPos.x, 2.525f, startPos.z);
			isDisconnected = true;
			Manipulator manipulator = ((!isGrabbed) ? ToolManager.ActiveToolGroups[0].Manipulator : grabbedBy[0]);
			Vector3 grabLocation = manipulator.grabLocation;
			ForceDropByPlayer(playSFX: false);
			chazHat.transform.parent = EagerScenes.instance.ActiveScene.transform;
			chazHat.GetComponent<Collider>().enabled = true;
			Object.Destroy(chazHat.GetComponent<FixedJoint>());
			manipulator.Grab(chazHat.GetComponent<ObjectBase>(), grabLocation, playSound: false);
			SafeDebug.Log("grabbing hat");
			explosionParent.SetActive(value: true);
			breakSound.Play();
			ungrabbable = true;
			if ((bool)VVR_HR.instance)
			{
				VVR_HR.instance.AICoreBreak();
			}
			enabled = false;
		}
		else if (transform.localPosition.y > 2.75f)
		{
			isDisconnected = true;
			Object.Destroy(cJoint);
			explosionParent.SetActive(value: true);
			breakSound.Play();
			dyingSound.Play();
			ForceDropByPlayer();
			transform.parent = EagerScenes.instance.ActiveScene.transform;
			if ((bool)VVR_HR.instance)
			{
				VVR_HR.instance.AICoreBreak();
			}
			enabled = false;
		}
	}

	private void OnJointBreak()
	{
		fixedJointBroken = true;
	}
}
