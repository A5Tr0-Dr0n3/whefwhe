using UnityEngine;

public class DialController : MonoBehaviour
{
	public int numSettings = 3;

	public float angleIncrement = 45f;

	public float breakTorque = 30f;

	private HingeJoint hj;

	private JointSpring spring;

	private int currentSetting;

	private float springStrength = 60f;

	private float springLockStrength = 2000f;

	private float forceScale;

	private void Start()
	{
		hj = GetComponent<HingeJoint>();
		spring = hj.spring;
		JointLimits limits = hj.limits;
		limits.max = (float)(numSettings - 1) * angleIncrement;
		hj.limits = limits;
		forceScale = 0.11037586f * transform.parent.localScale.x;
		springStrength *= forceScale;
		springLockStrength *= forceScale;
		breakTorque *= forceScale;
		spring.damper = 0.1f * springStrength;
		hj.spring = spring;
	}

	private void Update()
	{
		if (hj.angle > (float)currentSetting * angleIncrement + angleIncrement / 2f && currentSetting + 1 < numSettings)
		{
			spring.targetPosition = (float)currentSetting * angleIncrement + angleIncrement;
			spring.spring = springStrength;
			hj.spring = spring;
			currentSetting++;
		}
		else if (hj.angle < (float)currentSetting * angleIncrement - angleIncrement / 2f && currentSetting > 0)
		{
			spring.targetPosition = (float)currentSetting * angleIncrement - angleIncrement;
			spring.spring = springStrength;
			hj.spring = spring;
			currentSetting--;
		}
		SnapToSetting();
	}

	private void OnJointBreak(float breakForce)
	{
		SafeDebug.Log("A joint has just been broken!, force: " + breakForce);
		spring.spring = springStrength;
		hj.spring = spring;
	}

	private void SnapToSetting()
	{
		if (!gameObject.GetComponent<FixedJoint>() && hj.angle < (float)currentSetting * angleIncrement + 1f && hj.angle > (float)currentSetting * angleIncrement - 1f)
		{
			if (spring.spring != springLockStrength)
			{
				spring.spring = springLockStrength;
				hj.spring = spring;
			}
			if (hj.angle < (float)currentSetting * angleIncrement + 0.1f && hj.angle > (float)currentSetting * angleIncrement - 0.1f)
			{
				gameObject.AddComponent<FixedJoint>();
				gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
				gameObject.GetComponent<FixedJoint>().breakTorque = breakTorque;
			}
		}
		else if (spring.spring == springLockStrength)
		{
			spring.spring = springStrength;
			hj.spring = spring;
		}
	}
}
