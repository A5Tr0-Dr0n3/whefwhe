using UnityEngine;

public class ObjectSun : ObjectBase
{
	private SunsetBehavior mSunsetBehavior;

	public Transform rightEdge;

	public Transform middle;

	private float storedTouchSensitivity;

	private int timesGrabbed;

	protected override void Awake()
	{
		base.Awake();
		mSunsetBehavior = GetComponent<SunsetBehavior>();
	}

	public override void GrabWith(Manipulator grabber)
	{
		grabbedBy.Add(grabber);
		isGrabbed = true;
		if (!ungrabbable)
		{
			timesGrabbed++;
		}
		if (onBecomeGrabbed != null)
		{
			onBecomeGrabbed.Invoke();
		}
		storedTouchSensitivity = grabber.touchPadSensitivityY;
		grabber.touchPadSensitivityY = 10f;
		mSunsetBehavior.SetIsInOrbit(value: false);
	}

	public override void GrabUpdate(Manipulator grabber)
	{
		if (!(grabber != grabbedBy[0]))
		{
			base.GrabUpdate(grabber);
			for (int i = 0; i < grabbedBy.Count; i++)
			{
				LineRenderer component = grabbedBy[i].GetComponent<LineRenderer>();
				component.startWidth = 0.0125f;
				component.endWidth = 0.03f * Mathf.Log(grabbedBy[i].hitPointDistance, 2f);
			}
		}
	}

	public override void GrabRelease(Manipulator grabber)
	{
		grabbedBy.Remove(grabber);
		isGrabbed = false;
		grabber.touchPadSensitivityY = storedTouchSensitivity;
		LineRenderer component = grabber.GetComponent<LineRenderer>();
		component.startWidth = 0.0125f;
		component.endWidth = 0.0125f;
	}

	public SunsetBehavior GetSunsetBehavior()
	{
		return mSunsetBehavior;
	}

	public int GetTimesGrabbed()
	{
		return timesGrabbed;
	}
}
