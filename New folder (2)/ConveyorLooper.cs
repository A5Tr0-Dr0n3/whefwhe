using System;
using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using UnityEngine;

public class ConveyorLooper : SceneBehaviorManager
{
	public Transform start;

	public Transform end;

	[Range(0f, 5f)]
	public float speed;

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve speedChangeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[CurveRange(0f, 1f, -1f, 1f)]
	public AnimationCurve endCapFix = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	public RotateObject log;

	private float height;

	private float length;

	private float curve;

	private float perimeter;

	private float rootPos;

	public List<ConveyorLink> linkList;

	private SafeCoroutine speedChangeThread;

	private AudioLoop audioLoop;

	private float adjustedPos;

	private float targetRot;

	public bool changingSpeed
	{
		get
		{
			if (speedChangeThread == null || !speedChangeThread.IsRunning)
			{
				return false;
			}
			return true;
		}
	}

	private void Start()
	{
		if (linkList != null && start != null && end != null)
		{
			height = linkList[0].transform.GetChild(0).localPosition.y;
			length = Vector3.Distance(start.position, end.position);
			curve = height * (float)Math.PI;
			perimeter = 2f * (length + curve);
			for (int i = 0; i < linkList.Count; i++)
			{
				linkList[i].percent = (float)i / (float)linkList.Count;
			}
		}
		audioLoop = GetComponent<AudioLoop>();
	}

	private void FixedUpdate()
	{
		rootPos += speed * Time.deltaTime;
		if (rootPos >= perimeter)
		{
			rootPos -= perimeter;
		}
		for (int i = 0; i < linkList.Count; i++)
		{
			if (linkList[i] != null && !linkList[i].broken)
			{
				Place(linkList[i]);
			}
		}
	}

	private void Place(ConveyorLink link)
	{
		adjustedPos = rootPos + link.percent * perimeter;
		if (adjustedPos >= perimeter)
		{
			adjustedPos -= perimeter;
		}
		if (adjustedPos < length)
		{
			link.rb.MovePosition(new Vector3(start.position.x, start.position.y, start.position.z - adjustedPos));
			link.rb.rotation = Quaternion.Euler(0f, 0f, 0f);
		}
		else if (adjustedPos < length + curve)
		{
			targetRot = -180f * (adjustedPos - length) / curve;
			link.rb.MovePosition(end.position + Vector3.up * endCapFix.Evaluate(targetRot / -180f) * 0.08f);
			link.rb.rotation = Quaternion.Euler(targetRot, 0f, 0f);
		}
		else if (adjustedPos < 2f * length + curve)
		{
			link.rb.MovePosition(new Vector3(end.position.x, end.position.y, end.position.z + (adjustedPos - (length + curve))));
			link.rb.rotation = Quaternion.Euler(180f, 0f, 0f);
		}
		else
		{
			targetRot = -180f - 180f * (adjustedPos - (2f * length + curve)) / curve;
			link.rb.MovePosition(start.position + Vector3.up * endCapFix.Evaluate((targetRot + 180f) / -180f) * 0.08f);
			link.rb.rotation = Quaternion.Euler(targetRot, 0f, 0f);
		}
	}

	public void ChangeSpeed(float newSpeed, float duration)
	{
		if (newSpeed == 0f || speedChangeThread == null || !speedChangeThread.IsRunning)
		{
			if (speed == 0f)
			{
				GetComponent<AudioLoop>().Play();
			}
			speedChangeThread = GlobalCoroutineContainer.AddThread(changeSpeedCoroutine(newSpeed, duration), this);
		}
	}

	private IEnumerator changeSpeedCoroutine(float newSpeed, float duration)
	{
		float elapsed = 0f;
		float oldSpeed = speed;
		while (elapsed < duration)
		{
			speed = oldSpeed + speedChangeCurve.Evaluate(elapsed / duration) * (newSpeed - oldSpeed);
			log.rotation.y = speed * 90f;
			if (newSpeed > oldSpeed && oldSpeed <= 0f)
			{
				audioLoop.SetSourcePitch((oldSpeed + (newSpeed - oldSpeed) * speedChangeCurve.Evaluate(elapsed / duration)) / 6f);
			}
			else if (newSpeed < oldSpeed)
			{
				audioLoop.SetSourcePitch((oldSpeed - (oldSpeed - newSpeed) * speedChangeCurve.Evaluate(elapsed / duration)) / 6f);
			}
			elapsed += Time.deltaTime;
			yield return null;
		}
		if (newSpeed <= 0f)
		{
			GetComponent<AudioLoop>().Stop();
		}
		speed = newSpeed;
		log.rotation.y = speed * 90f;
		yield return null;
	}
}
