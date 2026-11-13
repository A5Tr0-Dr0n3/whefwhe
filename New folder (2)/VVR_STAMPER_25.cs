using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using UnityEngine;

public class VVR_STAMPER_25 : MonoBehaviour
{
	[Header("Campfire")]
	public ObjectWater waterTrigger;

	public ParticleSystem fire;

	public FireWatchTrigger fireTrigger;

	public FireFlicker point;

	public AudioEvent burnSound;

	[Header("Objects")]
	public List<ObjectMorphToHeadset> morphers;

	[Header("Headsets")]
	public Transform endTransform;

	public Headset laser;

	public float animateTime = 3f;

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[Header("Environments")]
	public ElevatorEnvironments environmentController;

	public ElizaEnvironmentProperties dayEnvironment;

	public ElizaEnvironmentProperties nightEnvironment;

	[HideInInspector]
	public SafeCoroutine extinguishThread;

	[HideInInspector]
	public SafeCoroutine burnThread00;

	[HideInInspector]
	public SafeCoroutine burnThread01;

	[HideInInspector]
	public SafeCoroutine burnThread02;

	[HideInInspector]
	public SafeCoroutine spawnThread;

	private GameObject grabbedObj;

	private float crispy;

	private void Start()
	{
		laser.gameObject.SetActive(value: false);
		for (int i = 0; i < morphers.Count; i++)
		{
			fireTrigger.objectChecks.Add(morphers[i].gameObject);
		}
	}

	private void Update()
	{
		if (waterTrigger.isSatisfied && extinguishThread == null)
		{
			extinguishThread = GlobalCoroutineContainer.AddThread(extinguishCoroutine(), this);
		}
		for (int i = 0; i < ToolManager.ActiveToolGroups.Count; i++)
		{
			if (ToolManager.ActiveToolGroups[i].Manipulator.isGrabbing())
			{
				grabbedObj = ToolManager.ActiveToolGroups[i].Manipulator.HeldObjectBase.gameObject;
				if (grabbedObj.GetComponent<ObjectMorphToHeadset>() != null && morphers.Contains(grabbedObj.GetComponent<ObjectMorphToHeadset>()) && grabbedObj.GetComponent<SpringJoint>() != null)
				{
					grabbedObj.GetComponent<SpringJoint>().breakForce = 5f;
				}
			}
		}
		if (!fireTrigger.hasObjectInside)
		{
			return;
		}
		for (int num = fireTrigger.ObjectsInside.Count - 1; num >= 0; num--)
		{
			if (fireTrigger.ObjectsTimeInside[num] > 1f)
			{
				if (burnThread00 == null)
				{
					StartBurn(fireTrigger.ObjectsInside[num]);
				}
				else if (burnThread01 == null)
				{
					StartBurn(fireTrigger.ObjectsInside[num], 1);
				}
				else if (burnThread02 == null)
				{
					StartBurn(fireTrigger.ObjectsInside[num], 2);
				}
			}
		}
	}

	private void StartBurn(GameObject burnTarget, int threadIndex = 0)
	{
		fireTrigger.AddObjectIgnore(burnTarget);
		switch (threadIndex)
		{
		case 2:
			burnThread02 = GlobalCoroutineContainer.AddThread(burnCoroutine(burnTarget.GetComponent<ObjectMorphToHeadset>(), threadIndex), this);
			break;
		case 1:
			burnThread01 = GlobalCoroutineContainer.AddThread(burnCoroutine(burnTarget.GetComponent<ObjectMorphToHeadset>(), threadIndex), this);
			break;
		default:
			burnThread00 = GlobalCoroutineContainer.AddThread(burnCoroutine(burnTarget.GetComponent<ObjectMorphToHeadset>(), threadIndex), this);
			break;
		}
	}

	private IEnumerator extinguishCoroutine()
	{
		fire.Stop();
		environmentController.MatchEnvironment(2f, nightEnvironment);
		while (point.baseIntensity > 0f)
		{
			point.baseIntensity -= 0.01f;
			yield return null;
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.25f, this);
		fire.gameObject.SetActive(value: false);
		spawnThread = GlobalCoroutineContainer.AddThread(headsetSpawnCoroutine(laser), this);
	}

	private IEnumerator burnCoroutine(ObjectMorphToHeadset morpher, int threadNum = 0)
	{
		burnSound.Play();
		morpher.MorphIntoHeadset(immediatelyForceOn: false);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(3f, this);
		switch (threadNum)
		{
		case 2:
			burnThread01 = null;
			break;
		case 1:
			burnThread01 = null;
			break;
		default:
			burnThread00 = null;
			break;
		}
	}

	private IEnumerator headsetSpawnCoroutine(Headset headset)
	{
		headset.GetComponent<Collider>().enabled = false;
		headset.gameObject.SetActive(value: true);
		float t = 0f;
		Vector3 initPos = headset.transform.position;
		Quaternion initRot = headset.transform.rotation;
		while (t <= animateTime)
		{
			headset.transform.position = Vector3.Lerp(initPos, endTransform.position, curve.Evaluate(t / animateTime));
			headset.transform.rotation = Quaternion.Slerp(initRot, endTransform.rotation, curve.Evaluate(t / animateTime));
			t += Time.deltaTime;
			yield return null;
		}
		headset.GetComponent<Collider>().enabled = true;
		while (!headset.GetComponent<ObjectBase>().isGrabbed)
		{
			yield return null;
		}
		headset.GetComponent<Rigidbody>().isKinematic = false;
		headset.isActivated = true;
		yield return null;
	}
}
