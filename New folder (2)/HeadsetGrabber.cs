using System;
using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class HeadsetGrabber : MonoBehaviour
{
	[SerializeField]
	private float headsetGrabTime = 1f;

	[SerializeField]
	private AnimationCurve headsetInterpolationCurve;

	[SerializeField]
	private Rigidbody headsetAnchor;

	[SerializeField]
	private float fixedJointBreakForce = 20f;

	[HideInInspector]
	public Headset myHeadset;

	private FixedJoint myHeadsetJoint;

	private SafeCoroutine grabThread;

	public void ReactToSceneActivated()
	{
		if (myHeadset == null)
		{
			myHeadset = EagerScenes.instance.lastHeadsetRemoved;
			if (myHeadset != null)
			{
				grabThread = GlobalCoroutineContainer.AddThread(grabHeadsetSequence(EagerScenes.instance.lastHeadsetRemoved), this);
			}
			else
			{
				SafeDebug.Log("HeadsetGrabber could not find EagerHeadsetManager.instance.lastRemovedHeadset");
			}
		}
	}

	private void Update()
	{
		if (grabThread != null && !grabThread.IsRunning && myHeadset != null && myHeadsetJoint == null)
		{
			myHeadset = null;
		}
	}

	private IEnumerator grabHeadsetSequence(Headset targetHeadset)
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.2f, this);
		Vector3 originalHeadsetPos = targetHeadset.transform.position;
		Quaternion originalHeadsetRot = targetHeadset.transform.rotation;
		targetHeadset.rb.isKinematic = true;
		float timer = 0f;
		while (timer < headsetGrabTime)
		{
			ToolManager.PlayerGrabbingObjectBase(myHeadset.GetComponent<ObjectBase>(), dropIfGrabbing: true);
			timer += Time.deltaTime;
			float t = timer / headsetGrabTime;
			float curvedT = headsetInterpolationCurve.Evaluate(t);
			Quaternion toPlayer = Quaternion.LookRotation((headsetAnchor.transform.position - Camera.main.transform.position).Flatten().normalized);
			targetHeadset.transform.rotation = Quaternion.Slerp(originalHeadsetRot, toPlayer, t);
			targetHeadset.transform.position = new Vector3(Mathf.LerpUnclamped(originalHeadsetPos.x, headsetAnchor.transform.position.x, curvedT), Mathf.LerpUnclamped(originalHeadsetPos.y, headsetAnchor.transform.position.y, curvedT), Mathf.LerpUnclamped(originalHeadsetPos.z, headsetAnchor.transform.position.z, curvedT));
			yield return null;
		}
		targetHeadset.rb.isKinematic = false;
		myHeadsetJoint = targetHeadset.gameObject.AddComponent<FixedJoint>();
		myHeadsetJoint.connectedBody = headsetAnchor;
		myHeadsetJoint.breakForce = fixedJointBreakForce;
	}

	[Obsolete("Consider Replacing with Mathf.LerpUnclamped or Vector3.LerpUnclamped")]
	private static float unclampedLerp(float a, float b, float t)
	{
		return a * (1f - t) + b * t;
	}
}
