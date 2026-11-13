using UnityEngine;

public class HRPlug : ObjectBase
{
	[HideInInspector]
	public bool pluggedIn = true;

	[SerializeField]
	private HingeJoint claspLeft;

	[SerializeField]
	private HingeJoint claspRight;

	[SerializeField]
	private GameObject explosionParent;

	[SerializeField]
	private Collider plugCollider;

	[SerializeField]
	private GvrAudioSource claspReleaseSound;

	private bool claspsOff;

	private bool claspLeftOpen;

	private bool claspRightOpen;

	protected override void Awake()
	{
		base.Awake();
		claspLeft.GetComponent<Collider>().enabled = true;
		claspRight.GetComponent<Collider>().enabled = true;
	}

	private void Update()
	{
		if (!claspLeftOpen && claspLeft == null)
		{
			claspLeftOpen = true;
			claspReleaseSound.Play();
		}
		if (!claspRightOpen && claspRight == null)
		{
			claspRightOpen = true;
			claspReleaseSound.Play();
		}
		if (!claspsOff && claspLeft == null && claspRight == null)
		{
			plugCollider.enabled = true;
			claspsOff = true;
			GetComponent<Joint>().breakForce = 300f;
			GetComponent<Joint>().breakTorque = 300f;
			rb.isKinematic = false;
			return;
		}
		if (claspLeft != null && claspLeft.angle < -85f)
		{
			ObjectBase component = claspLeft.GetComponent<ObjectBase>();
			component.ForceDropByPlayer();
			claspLeft.GetComponent<Rigidbody>().isKinematic = true;
			Object.Destroy(claspLeft.GetComponent<ObjectBase>());
			VVR_HR.instance.ParentPlugClasp(claspLeft.gameObject.transform);
			Object.Destroy(claspLeft);
		}
		if (claspRight != null && claspRight.angle < -85f)
		{
			ObjectBase component2 = claspRight.GetComponent<ObjectBase>();
			component2.ForceDropByPlayer();
			claspRight.GetComponent<Rigidbody>().isKinematic = true;
			Object.Destroy(component2);
			VVR_HR.instance.ParentPlugClasp(claspRight.gameObject.transform);
			Object.Destroy(claspRight);
		}
	}

	private void OnJointBreak()
	{
		ForceDropByPlayer();
		explosionParent.SetActive(value: true);
		explosionParent.GetComponentInChildren<GvrAudioSource>().playOnAwake = false;
		rb.AddForce(myTransform.up * -5000f);
		pluggedIn = false;
		if ((bool)VVR_HR.instance)
		{
			VVR_HR.instance.PlugBreak();
		}
	}
}
