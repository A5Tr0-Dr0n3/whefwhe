using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class HernandezLeaf : ObjectBase
{
	public delegate void OnLeafEventHandler(HernandezLeaf leaf);

	public delegate void OnDelayDropCallback();

	[SerializeField]
	private AudioSourceLoudness mLoudnessModule;

	protected HernandezLeafController mOwner;

	private const float manualDropOffForceMag = 100f;

	private const float manualDropOffAngle = 30f;

	private const float reattachTime = 0.3f;

	private const float pluckBreakForce = 30f;

	private Transform detachedParent;

	private Transform attachedParent;

	private SafeCoroutine delayDropThread;

	[SerializeField]
	private float leafJointFreedom = 10f;

	[SerializeField]
	private float leafJointNegativeFreedom = -10f;

	[SerializeField]
	private float leafJointSpringiness = 100f;

	[SerializeField]
	private float leafJointDamper = 1f;

	[SerializeField]
	private float fullyClosedAngle = -5f;

	[SerializeField]
	private float fullyOpenAngle = 5f;

	private bool isPluckable;

	public Collider mCollider { get; private set; }

	public Rigidbody mRigidbody { get; private set; }

	public HingeJoint mAnchorConnection { get; private set; }

	public PersistentCollisionIgnorer mCollisionIgnorer { get; private set; }

	public static event OnLeafEventHandler OnLeafPlucked;

	public HernandezLeafAnchor Initialize(HernandezLeafController ownerIn)
	{
		mOwner = ownerIn;
		detachedParent = mOwner.transform.parent;
		attachedParent = transform.parent;
		mCollider = GetComponent<Collider>();
		mRigidbody = GetComponent<Rigidbody>();
		mCollisionIgnorer = gameObject.AddComponent<PersistentCollisionIgnorer>();
		HernandezLeafAnchor hernandezLeafAnchor = new GameObject().AddComponent<HernandezLeafAnchor>();
		hernandezLeafAnchor.transform.SetParent(transform.parent);
		hernandezLeafAnchor.transform.localPosition = transform.localPosition;
		hernandezLeafAnchor.Initialize(this, mOwner);
		ConnectToAnchor(hernandezLeafAnchor);
		return hernandezLeafAnchor;
	}

	public void ConnectToAnchor(HernandezLeafAnchor attachee, bool smoothTransform = false)
	{
		if (mAnchorConnection != null)
		{
			Object.Destroy(mAnchorConnection);
		}
		mOwner.RegisterLeaf(attachedToBody: true, this);
		if (smoothTransform)
		{
			GlobalCoroutineContainer.AddThread(reattachOverTime(attachee), this);
		}
		else
		{
			setUpJoint(attachee);
		}
	}

	protected virtual void setUpJoint(HernandezLeafAnchor attachee, bool useSpring = true)
	{
		if (mAnchorConnection != null)
		{
			Object.Destroy(mAnchorConnection);
		}
		mAnchorConnection = gameObject.AddComponent<HingeJoint>();
		gameObject.AddComponent<HingeJointFix>();
		mAnchorConnection.connectedBody = attachee.mRigidbody;
		mAnchorConnection.autoConfigureConnectedAnchor = false;
		mAnchorConnection.anchor = Vector3.zero;
		mAnchorConnection.connectedAnchor = Vector3.zero;
		mAnchorConnection.breakForce = float.PositiveInfinity;
		JointLimits limits = new JointLimits
		{
			min = leafJointNegativeFreedom,
			max = leafJointFreedom
		};
		mAnchorConnection.limits = limits;
		mAnchorConnection.useLimits = true;
		JointSpring spring = new JointSpring
		{
			spring = leafJointSpringiness,
			damper = leafJointDamper,
			targetPosition = 0f
		};
		mAnchorConnection.spring = spring;
		if (useSpring)
		{
			mAnchorConnection.useSpring = true;
		}
		mAnchorConnection.enablePreprocessing = false;
		attachee.AttachLeaf(mAnchorConnection);
	}

	private IEnumerator reattachOverTime(HernandezLeafAnchor newAnchor)
	{
		transform.SetParent(attachedParent);
		mRigidbody.isKinematic = true;
		float timer = 0f;
		Vector3 startingLocalPos = transform.localPosition;
		Quaternion startingLocalRot = transform.localRotation;
		ForceDropByPlayer();
		while (timer < 0.3f)
		{
			timer += Time.deltaTime;
			float t = timer / 0.3f;
			transform.localPosition = Vector3.Lerp(startingLocalPos, newAnchor.desiredLeafLocalPos, t);
			transform.localRotation = Quaternion.Slerp(startingLocalRot, newAnchor.desiredLeafLocalRot, t);
			yield return null;
		}
		transform.localPosition = newAnchor.desiredLeafLocalPos;
		transform.localRotation = newAnchor.desiredLeafLocalRot;
		mOwner.PlayLeafSfx(attach: true);
		setUpJoint(newAnchor);
		mRigidbody.isKinematic = false;
		mRigidbody.velocity = Vector3.zero;
		mRigidbody.angularVelocity = Vector3.zero;
	}

	public void DropOnDelay(float delay, OnDelayDropCallback callback = null)
	{
		if (delayDropThread != null && delayDropThread.IsRunning)
		{
			delayDropThread.Stop();
		}
		delayDropThread = GlobalCoroutineContainer.AddThread(dropOnDelaySequence(delay, callback), this);
	}

	private IEnumerator dropOnDelaySequence(float delay, OnDelayDropCallback callback)
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(delay, this);
		DropFromBody();
		callback?.Invoke();
	}

	public void DropFromBody()
	{
		if (mAnchorConnection != null)
		{
			mOwner.PlayLeafSfx(attach: false);
			Object.Destroy(mAnchorConnection);
			mOwner.RegisterLeaf(attachedToBody: false, this, 1.5f);
			Vector3 vector = (transform.position - mOwner.transform.position).Flatten();
			Vector3 axis = Vector3.Cross(vector, Vector3.up);
			Vector3 vector2 = Quaternion.AngleAxis(30f, axis) * vector;
			transform.SetParent(detachedParent);
			mRigidbody.AddForce(vector2.normalized * 100f);
			Debug.DrawLine(transform.position, transform.position + vector2.normalized * 100f, Color.red, 99f);
		}
		else
		{
			SafeDebug.LogError("Trying to drop off a leaf that's not actually attached!");
		}
	}

	public void MakePluckable()
	{
		if (mAnchorConnection != null)
		{
			isPluckable = true;
		}
		if (ToolManager.PlayerGrabbingObjectBase(this))
		{
			setPluckMode(pluckMode: true);
		}
	}

	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		if (isPluckable && mAnchorConnection != null)
		{
			setPluckMode(pluckMode: true);
		}
	}

	private void setPluckMode(bool pluckMode)
	{
		if (mAnchorConnection != null)
		{
			setUpJoint(mAnchorConnection.connectedBody.GetComponent<HernandezLeafAnchor>(), !pluckMode);
			mAnchorConnection.breakForce = ((!pluckMode) ? float.PositiveInfinity : 30f);
		}
	}

	public override void GrabRelease(Manipulator grabber)
	{
		base.GrabRelease(grabber);
		if (isPluckable && mAnchorConnection != null)
		{
			setPluckMode(pluckMode: false);
		}
	}

	private void OnJointBreak(float breakForce)
	{
		mOwner.RegisterLeaf(attachedToBody: false, this);
		mOwner.PlayLeafSfx(attach: false);
		if (ToolManager.PlayerGrabbingObjectBase(this) && OnLeafPlucked != null)
		{
			OnLeafPlucked(this);
		}
	}

	private void Update()
	{
		if (mAnchorConnection != null && mLoudnessModule != null && mAnchorConnection.useSpring)
		{
			JointSpring spring = mAnchorConnection.spring;
			float filteredLoudness = mLoudnessModule.GetFilteredLoudness();
			spring.targetPosition = Mathf.LerpAngle(fullyClosedAngle, fullyOpenAngle, filteredLoudness);
			mAnchorConnection.spring = spring;
		}
	}
}
