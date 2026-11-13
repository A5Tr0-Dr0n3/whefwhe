using UnityEngine;

public class BoidJointBreakTrigger : MonoBehaviour
{
	public ChazController myChaz;

	private void OnJointBreak(float breakForce)
	{
		myChaz.saveBoid.GetComponent<BoidOnJointBreak>().BreakJoint();
	}
}
