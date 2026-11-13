using UnityEngine;

public class JointKiller : MonoBehaviour
{
	public Joint mainJoint;

	private void Start()
	{
		if (mainJoint == null)
		{
			mainJoint = GetComponent<Joint>();
		}
	}

	private void Update()
	{
		if (mainJoint == null)
		{
			Object.Destroy(this);
		}
		else if (mainJoint.connectedBody == null)
		{
			Object.Destroy(mainJoint);
		}
	}
}
