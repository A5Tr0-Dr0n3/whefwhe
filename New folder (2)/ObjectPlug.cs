using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class ObjectPlug : ObjectBase
{
	private ParticleSystem connect;

	public AudioClip connectClip;

	public Transform connectRig;

	[HideInInspector]
	public bool pluggedIn;

	private bool allowExtPlugIn;

	public AudioEvent connectAudioEvent;

	public AudioEvent disconnectAudioEvent;

	private WhaleOutlet outletRef;

	private ObjectPlugCord[] allCordObjects;

	private SafeCoroutine plugInRoutine;

	private Transform targetTransform;

	private float plugLerp;

	private Vector3 startPosition;

	private Vector3 targetPosition;

	private Quaternion startRotation;

	private Quaternion targeRotation;

	private FixedJoint plugJoint;

	private float plugCoolDown;

	public Transform targetTransformReference => targetTransform;

	public FixedJoint PlugJointReference => plugJoint;

	protected override void Awake()
	{
		base.Awake();
		allCordObjects = connectRig.GetComponentsInChildren<ObjectPlugCord>();
	}

	private void OnTriggerStay(Collider other)
	{
		if (!pluggedIn && plugCoolDown < 0f)
		{
			if (!(other.tag == "Outlet") || (!isGrabbed && !allowExtPlugIn))
			{
				return;
			}
			if (other.gameObject.GetComponent<WhaleOutlet>() != null)
			{
				outletRef = other.gameObject.GetComponent<WhaleOutlet>();
				if (!outletRef.isPluggedIn && plugInRoutine == null)
				{
					plugInRoutine = GlobalCoroutineContainer.AddThread(plugIn(other.transform), this);
					pluggedIn = true;
					outletRef.isPluggedIn = true;
				}
			}
			else if (plugInRoutine == null)
			{
				plugInRoutine = GlobalCoroutineContainer.AddThread(plugIn(other.transform), this);
				pluggedIn = true;
				SafeDebug.Log("pluggingIn");
			}
		}
		else
		{
			plugCoolDown -= Time.deltaTime;
		}
	}

	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		if (plugJoint != null)
		{
			plugJoint.breakForce = 200f;
			plugJoint.breakTorque = 200f;
			rb.isKinematic = false;
		}
	}

	private IEnumerator plugIn(Transform target)
	{
		targetTransform = target;
		targetPosition = targetTransform.position;
		plugLerp = 0f;
		startPosition = rb.position;
		startRotation = rb.rotation;
		rb.isKinematic = true;
		while (plugLerp < 1f)
		{
			yield return null;
			plugLerp = Mathf.Clamp01(plugLerp + Time.deltaTime * 1.2f);
			targetPosition = Vector3.Lerp(targetTransform.position + targetTransform.up * -6f, targetTransform.position + targetTransform.up * -0.4f, plugLerp);
			rb.MovePosition(Vector3.Lerp(startPosition, targetPosition, plugLerp));
			rb.MoveRotation(Quaternion.Lerp(startRotation, targetTransform.rotation, plugLerp));
		}
		if (connectAudioEvent != null)
		{
			connectAudioEvent.Play();
		}
		plugInRoutine = null;
		createJoint();
		ForceDropByPlayer();
		for (int i = 0; i < allCordObjects.Length; i++)
		{
			allCordObjects[i].ForceDropByPlayer();
		}
	}

	public void StartPlugIn(Transform target)
	{
		if (plugInRoutine == null)
		{
			plugInRoutine = GlobalCoroutineContainer.AddThread(plugIn(target), this);
			pluggedIn = true;
			SafeDebug.Log("pluggingIn");
		}
	}

	public void PlugInImmediate(Transform target)
	{
		if (target.gameObject.GetComponent<WhaleOutlet>() != null)
		{
			outletRef = target.gameObject.GetComponent<WhaleOutlet>();
			outletRef.isPluggedIn = true;
		}
		if (plugInRoutine == null)
		{
			plugInRoutine = GlobalCoroutineContainer.AddThread(plugInImmediateRoutine(target), this);
			pluggedIn = true;
		}
	}

	private IEnumerator plugInImmediateRoutine(Transform target)
	{
		targetTransform = target;
		targetPosition = targetTransform.position + targetTransform.up * -0.4f;
		plugLerp = 0f;
		startPosition = rb.position;
		startRotation = rb.rotation;
		rb.isKinematic = true;
		while (plugLerp < 1f)
		{
			yield return null;
			plugLerp = Mathf.Clamp01(plugLerp + Time.deltaTime * 2f);
			rb.MovePosition(Vector3.Lerp(startPosition, targetPosition, plugLerp));
			rb.MoveRotation(Quaternion.Lerp(startRotation, targetTransform.rotation, plugLerp));
		}
		transform.position = targetTransform.position + targetTransform.up * -0.4f;
		rb.rotation = targetTransform.rotation;
		plugInRoutine = null;
		createJoint();
	}

	private void createJoint()
	{
		plugJoint = myTransform.gameObject.AddComponent<FixedJoint>();
		plugJoint.connectedBody = targetTransform.gameObject.GetComponent<Rigidbody>();
		rb.isKinematic = false;
	}

	private void OnJointBreak()
	{
		pluggedIn = false;
		plugCoolDown = 2f;
		plugJoint = null;
		if (outletRef != null)
		{
			outletRef.isPluggedIn = false;
		}
		if ((bool)disconnectAudioEvent)
		{
			disconnectAudioEvent.Play();
		}
	}

	public void SetExtPlugIn(bool p)
	{
		allowExtPlugIn = p;
	}
}
