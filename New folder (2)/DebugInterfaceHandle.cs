using UnityEngine;

[RequireComponent(typeof(ObjectBase))]
[RequireComponent(typeof(Rigidbody))]
public class DebugInterfaceHandle : MonoBehaviour
{
	private ObjectBase mObjectBase;

	private Rigidbody mRigidbody;

	private Vector3 startingOffset;

	private void Awake()
	{
		mObjectBase = GetComponent<ObjectBase>();
		mRigidbody = GetComponent<Rigidbody>();
	}

	public void ResetPosition()
	{
		transform.position = Camera.main.transform.position + 4f * new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);
	}

	private void Update()
	{
		if (!ToolManager.PlayerGrabbingObjectBase(mObjectBase))
		{
			mRigidbody.velocity = Vector3.zero;
		}
		transform.LookAt(Camera.main.transform.position);
	}
}
