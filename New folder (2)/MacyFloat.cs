using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using UnityEngine;

public class MacyFloat : MonoBehaviour
{
	public enum BalloonType
	{
		Owl,
		Frog,
		Rabbit,
		Carrot,
		Chaz,
		Star,
		Fish,
		SoccerBall,
		Surfdog,
		Toast,
		Cactus,
		Possum,
		Butter
	}

	public enum FloatStates
	{
		Attached,
		Free
	}

	[Header("References")]
	public List<MacyLine> lines;

	public GvrAudioSource audioSource;

	public TransformTargetRocket rocket;

	public ObjectBase balloonBase;

	public TransformShaker targetShaker;

	public Rigidbody balloonRb;

	public ParticleSystem sparks;

	public AudioClip[] breakClips;

	[Header("Values")]
	public BalloonType type;

	public float breakEase = 1f;

	public float breakTorqueStrength = 0.06f;

	public float breakThreshhold = 0.8f;

	public float breakAllThreshhold = 2f;

	public float reductFactorMin = 0.01f;

	public float reductFactorMax = 0.12f;

	public float floatSpeedMod = 0.5f;

	public float floatSpinMod = 0.2f;

	public float despawnHeight = 20f;

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve reductionFactorCurve;

	private FloatStates currState;

	private float lineLength;

	private float lineLengthChange;

	private float midLineWidth;

	private float balloonDistanceFromAnchors;

	private float grabDistance;

	private float grabDistanceMod;

	private float sqrMagFromPlayer;

	private int mostStretchedLine;

	private int startingLineCount;

	private SafeCoroutine breakAllLinesThread;

	private void Start()
	{
		currState = FloatStates.Attached;
		for (int i = 0; i < lines.Count; i++)
		{
			lines[i].line.positionCount = 3;
			lines[i].SetStartLength(Vector3.SqrMagnitude(lines[i].floatAnchor.position - lines[i].groundAnchor.position));
		}
		startingLineCount = lines.Count;
	}

	private void Update()
	{
		if (currState == FloatStates.Attached)
		{
			sqrMagFromPlayer = Vector3.SqrMagnitude(balloonRb.gameObject.transform.position - EagerMaster.instance.mainCamera.transform.position);
			if (balloonBase.isGrabbed || sqrMagFromPlayer < 80f)
			{
				if (!targetShaker.enabled)
				{
					targetShaker.enabled = true;
				}
				mostStretchedLine = 0;
				for (int i = 0; i < lines.Count; i++)
				{
					if (!lines[i].line.gameObject.activeSelf)
					{
						lines[i].line.gameObject.SetActive(value: true);
					}
					lines[i].line.SetPosition(0, lines[i].floatAnchor.position);
					lines[i].line.SetPosition(1, Vector3.Lerp(lines[i].floatAnchor.position, lines[i].groundAnchor.position, lines[i].GetMidPoint()));
					lines[i].line.SetPosition(2, lines[i].groundAnchor.position);
					if (balloonBase.isGrabbed || sqrMagFromPlayer < 20f)
					{
						lineLength = Vector3.SqrMagnitude(lines[i].floatAnchor.position - lines[i].groundAnchor.position);
						if (lines[i].GetCurrLength() - lineLength > (float)mostStretchedLine)
						{
							mostStretchedLine = i;
						}
						lines[i].SetCurrLength(lineLength);
					}
				}
				if (sqrMagFromPlayer > 30f && !balloonBase.isGrabbed)
				{
					if (rocket.enabled)
					{
						rocket.enabled = false;
					}
					balloonRb.isKinematic = true;
					balloonBase.gameObject.transform.position = rocket.GetTarget().position;
					balloonBase.gameObject.transform.rotation = rocket.GetTarget().rotation;
				}
				else
				{
					if (!rocket.enabled)
					{
						rocket.enabled = true;
					}
					balloonRb.isKinematic = false;
				}
				if (balloonBase.isGrabbed && lines.Count > 0)
				{
					grabDistance = 0f;
					for (int j = 0; j < balloonBase.grabbedBy.Count; j++)
					{
						grabDistance += balloonBase.grabbedBy[j].grabTargetDistance;
					}
					grabDistance /= balloonBase.grabbedBy.Count;
					grabDistanceMod = Mathf.Clamp01(grabDistance * 0.2f + 0.6f);
					balloonDistanceFromAnchors = Vector3.Distance(balloonRb.transform.position, lines[0].groundAnchor.position);
					if (balloonDistanceFromAnchors > breakAllThreshhold * grabDistanceMod)
					{
						BreakAllLines();
					}
					else if (balloonDistanceFromAnchors > breakThreshhold * grabDistanceMod && Random.Range(0f, 1f) < balloonDistanceFromAnchors * Time.deltaTime * breakEase)
					{
						GlobalCoroutineContainer.AddThread(BreakLine(lines[mostStretchedLine]), this);
					}
				}
				return;
			}
			if (targetShaker.enabled)
			{
				targetShaker.enabled = false;
			}
			for (int k = 0; k < lines.Count; k++)
			{
				if (lines[k].line.gameObject.activeSelf)
				{
					lines[k].line.gameObject.SetActive(value: false);
				}
			}
			if (rocket.enabled)
			{
				rocket.enabled = false;
			}
			balloonRb.isKinematic = true;
			balloonBase.gameObject.transform.position = rocket.GetTarget().position;
			balloonBase.gameObject.transform.rotation = rocket.GetTarget().rotation;
		}
		else if (currState == FloatStates.Free)
		{
			balloonRb.AddForce(Vector3.up * floatSpeedMod, ForceMode.Force);
			balloonRb.AddTorque(Vector3.forward * floatSpinMod, ForceMode.Force);
			if (balloonRb.transform.position.y > despawnHeight)
			{
				gameObject.SetActive(value: false);
			}
		}
	}

	private IEnumerator BreakLine(MacyLine toBreak)
	{
		audioSource.PlayOneShot(breakClips[Random.Range(0, breakClips.Length - 1)]);
		sparks.transform.position = toBreak.floatAnchor.position;
		sparks.Play();
		balloonRb.AddTorque(toBreak.floatAnchor.position * breakTorqueStrength, ForceMode.Impulse);
		rocket.SetReductionFactor(Mathf.Lerp(reductFactorMin, reductFactorMax, reductionFactorCurve.Evaluate((float)lines.Count / (float)startingLineCount)));
		lines.Remove(toBreak);
		toBreak.line.widthCurve = new AnimationCurve(new Keyframe(0f, 0.3f), new Keyframe(1f, 1f));
		toBreak.line.positionCount = 2;
		float t = 0f;
		float durr = 0.25f;
		while (t < durr)
		{
			toBreak.line.SetPosition(0, Vector3.Lerp(toBreak.floatAnchor.position, toBreak.groundAnchor.position, t / durr));
			toBreak.line.SetPosition(1, toBreak.groundAnchor.position);
			t += Time.deltaTime;
			yield return null;
		}
		toBreak.line.gameObject.SetActive(value: false);
		if (lines.Count <= 0 && currState == FloatStates.Attached)
		{
			rocket.enabled = false;
			balloonBase.ForceDropByPlayer();
			currState = FloatStates.Free;
			JobMacy.instance.BalloonFree(this);
			yield return null;
		}
	}

	public void BreakAllLines()
	{
		if (breakAllLinesThread == null || !breakAllLinesThread.IsRunning)
		{
			breakAllLinesThread = GlobalCoroutineContainer.AddThread(BreakAllLinesCoroutine(), this);
		}
	}

	private IEnumerator BreakAllLinesCoroutine()
	{
		while (lines.Count > 0)
		{
			GlobalCoroutineContainer.AddThread(BreakLine(lines[0]), this);
			yield return GlobalCoroutineContainer.SafeWaitForSeconds(Random.Range(0.01f, 0.08f), this);
		}
	}

	public FloatStates State()
	{
		return currState;
	}
}
