using UnityEngine;

public class JointBreakListener : MonoBehaviour
{
	public IJointBreakSubscriber Subscriber;

	private void OnJointBreak(float breakForce)
	{
		if (Subscriber != null)
		{
			Subscriber.OnJointBreak(breakForce);
		}
	}
}
