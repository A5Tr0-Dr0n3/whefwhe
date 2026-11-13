using UnityEngine;

public class BeetleBall : MonoBehaviour
{
	public ChazController chazBeetle;

	public Transform beetleBallCenter;

	public Rigidbody beetleBallRB;

	public bool startOff = true;

	public float maxForce = 100f;

	private Transform chazHeadTransform;

	private Vector3 chazJoystickVector;

	private Vector3 chazJoystickFlat;

	private void Awake()
	{
		if (startOff)
		{
			SetObjectsKinematic(beetleBallRB.gameObject, toKinematic: true);
			SetObjectsKinematic(chazBeetle.gameObject, toKinematic: true);
			chazBeetle.gameObject.SetActive(value: false);
			enabled = false;
		}
	}

	private void Start()
	{
		chazHeadTransform = chazBeetle.head.transform;
	}

	private void Update()
	{
		MoveBall();
	}

	private void MoveBall()
	{
		chazJoystickVector = Vector3.Normalize(chazHeadTransform.position - beetleBallCenter.position);
		chazBeetle.groundOrigin = beetleBallCenter.position + chazJoystickVector * 2.125f;
		chazBeetle.groundUp = chazJoystickVector;
		if (maxForce > 0f)
		{
			chazJoystickFlat = new Vector3(chazJoystickVector.x, 0f, chazJoystickVector.z);
			beetleBallRB.AddForce(chazJoystickFlat * maxForce * Time.deltaTime * 60f);
		}
	}

	private void SetObjectsKinematic(GameObject parent, bool toKinematic)
	{
		Rigidbody[] componentsInChildren = parent.GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody rigidbody in componentsInChildren)
		{
			rigidbody.isKinematic = toKinematic;
			rigidbody.velocity = Vector3.zero;
		}
	}

	public void SetBallKinematic(bool toKinematic)
	{
		SetObjectsKinematic(beetleBallRB.gameObject, toKinematic);
	}

	public void SetChazKinematic(bool toKinematic)
	{
		SetObjectsKinematic(chazBeetle.gameObject, toKinematic);
	}
}
