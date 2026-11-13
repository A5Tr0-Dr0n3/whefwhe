using UnityEngine;

public class HernandezLeafAnchor : MonoBehaviour
{
	public delegate void OnLeafReattachedHandler();

	private HingeJoint mLeafConnection;

	private HernandezLeafController owner;

	private SphereCollider mSphereCollider;

	private const float reattachRadius = 0.3f;

	public Rigidbody mRigidbody { get; private set; }

	public Quaternion desiredLeafLocalRot { get; private set; }

	public Vector3 desiredLeafLocalPos { get; private set; }

	public static event OnLeafReattachedHandler OnLeafReattached;

	public void Initialize(HernandezLeaf startingLeaf, HernandezLeafController ownerIn)
	{
		name = "Leaf Anchor";
		owner = ownerIn;
		mRigidbody = gameObject.AddComponent<Rigidbody>();
		mRigidbody.isKinematic = true;
		mSphereCollider = gameObject.AddComponent<SphereCollider>();
		mSphereCollider.radius = 0.3f;
		mSphereCollider.isTrigger = true;
		desiredLeafLocalPos = startingLeaf.transform.localPosition;
		desiredLeafLocalRot = startingLeaf.transform.localRotation;
	}

	private void Update()
	{
		if (mSphereCollider.enabled && (!owner.acceptReattaching || mLeafConnection != null))
		{
			mSphereCollider.enabled = false;
		}
		else if (!mSphereCollider.enabled && owner.acceptReattaching && mLeafConnection == null)
		{
			mSphereCollider.enabled = true;
		}
	}

	private void OnTriggerStay(Collider c)
	{
		if (!owner.acceptReattaching || !(mLeafConnection == null))
		{
			return;
		}
		HernandezLeaf component = c.gameObject.GetComponent<HernandezLeaf>();
		if (component != null && component.mAnchorConnection == null && ToolManager.PlayerGrabbingObjectBase(component))
		{
			component.ConnectToAnchor(this, smoothTransform: true);
			if (OnLeafReattached != null)
			{
				OnLeafReattached();
			}
		}
	}

	public void AttachLeaf(HingeJoint leafConnectionIn)
	{
		mLeafConnection = leafConnectionIn;
	}
}
