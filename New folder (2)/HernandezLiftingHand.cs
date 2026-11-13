using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class HernandezLiftingHand : MonoBehaviour
{
	[SerializeField]
	private AnimationCurve lift;

	[SerializeField]
	private Transform hernandezCenter;

	[SerializeField]
	private float liftDuration = 1f;

	[SerializeField]
	private float minLiftAngle = -45f;

	[SerializeField]
	private float maxLiftAngle = 45f;

	[SerializeField]
	private float lateralAngle = -30f;

	[SerializeField]
	private float liftDist = 0.5f;

	[SerializeField]
	private float smoothWeightsToHandTime = 0.5f;

	private Rigidbody heldWeight;

	private SafeCoroutine liftThread;

	private FixedJoint mJoint;

	public void GrabWeight(Rigidbody weightIn)
	{
		heldWeight = weightIn;
	}

	public void BeginLifting()
	{
		liftThread = GlobalCoroutineContainer.AddThread(liftIndefinitely(), this);
	}

	public void QuitLifting()
	{
		heldWeight.isKinematic = false;
		if (mJoint != null)
		{
			Object.Destroy(mJoint);
		}
		if (liftThread != null && liftThread.IsRunning)
		{
			liftThread.Stop();
		}
	}

	private IEnumerator liftIndefinitely()
	{
		yield return grabWeights();
		while (true)
		{
			yield return singleLiftSequence();
		}
	}

	private IEnumerator liftNumberOfTimes(int liftCount)
	{
		yield return grabWeights();
		for (int i = 0; i < liftCount; i++)
		{
			yield return singleLiftSequence();
		}
	}

	private IEnumerator grabWeights()
	{
		float timer = 0f;
		Vector3 originalPosition = heldWeight.position;
		Quaternion originalRotation = heldWeight.rotation;
		heldWeight.isKinematic = true;
		while (timer < smoothWeightsToHandTime)
		{
			timer += Time.deltaTime;
			Vector3 liftForward = Quaternion.AngleAxis(lateralAngle, hernandezCenter.up) * hernandezCenter.forward;
			Vector3 liftPerpendicular = Vector3.Cross(liftForward, hernandezCenter.up);
			Vector3 loLiftDir = Quaternion.AngleAxis(minLiftAngle, liftPerpendicular) * liftForward;
			Vector3 targetPosition = hernandezCenter.transform.position + loLiftDir * liftDist;
			Vector3 lerpedPosition = Vector3.Lerp(originalPosition, targetPosition, timer / smoothWeightsToHandTime);
			Quaternion targetRotation = Quaternion.LookRotation(loLiftDir, Vector3.Cross(loLiftDir, liftPerpendicular));
			Quaternion slerpedRotation = Quaternion.Slerp(originalRotation, targetRotation, timer / smoothWeightsToHandTime);
			heldWeight.transform.position = lerpedPosition;
			heldWeight.transform.rotation = slerpedRotation;
			transform.rotation = slerpedRotation;
			yield return null;
		}
		heldWeight.isKinematic = false;
		mJoint = gameObject.AddComponent<FixedJoint>();
		mJoint.autoConfigureConnectedAnchor = false;
		mJoint.connectedBody = heldWeight;
		mJoint.anchor = Vector3.zero;
		mJoint.connectedAnchor = Vector3.zero;
	}

	private IEnumerator singleLiftSequence()
	{
		float timer = 0f;
		Vector3 originalPosition = heldWeight.position;
		while (timer < liftDuration)
		{
			timer += Time.deltaTime;
			Vector3 liftForward = Quaternion.AngleAxis(lateralAngle, hernandezCenter.up) * hernandezCenter.forward;
			Vector3 liftPerpendicular = Vector3.Cross(liftForward, hernandezCenter.up);
			Vector3 loLiftDir = Quaternion.AngleAxis(minLiftAngle, liftPerpendicular) * liftForward;
			Vector3 hiLiftDir = Quaternion.AngleAxis(maxLiftAngle, liftPerpendicular) * liftForward;
			float liftT = lift.Evaluate(timer / liftDuration);
			Vector3 liftDir = Vector3.Slerp(loLiftDir, hiLiftDir, liftT);
			transform.position = hernandezCenter.transform.position + liftDir * liftDist;
			transform.rotation = Quaternion.LookRotation(liftDir, Vector3.Cross(liftDir, liftPerpendicular));
			yield return new WaitForFixedUpdate();
		}
	}
}
