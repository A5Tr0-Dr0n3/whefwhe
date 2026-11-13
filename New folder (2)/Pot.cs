using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : ObjectBase
{
	private bool planting;

	protected GameObject plant;

	public Vector3 timeLength = new Vector3(0.25f, 0.25f, 0.25f);

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve curve1 = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve curve2 = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve curve3 = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[CurveRange(0f, 1f, 0f, 1f)]
	public AnimationCurve curveSpin = AnimationCurve.EaseInOut(1f, 1f, 1f, 1f);

	[CurveRange(0f, 1f, 0f, 2f)]
	public AnimationCurve curveZscale = AnimationCurve.EaseInOut(1f, 1f, 1.2f, 0.9f);

	public List<GameObject> dontBreakOnColliders;

	[SerializeField]
	protected ObjectPottable plantObjectBase;

	private ObjectPottable testPottableObject;

	private Vector3 startPos = Vector3.zero;

	private Quaternion startRot = Quaternion.identity;

	private Quaternion potRot = Quaternion.identity;

	private Vector3 startScale = Vector3.one;

	private float time;

	private Rigidbody plantRB;

	private float curveEvaluated;

	private float scaleEvaluated;

	private float spinEvaluated;

	private Vector3 pos1;

	private Vector3 pos2;

	private Vector3 pos3;

	public ObjectShard[] smashPieces;

	public GameObject smashParticles;

	public AudioClip[] smashAudio;

	protected override void Start()
	{
		base.Start();
		if (rb != null)
		{
			rb.centerOfMass = new Vector3(0f, 0.045f, 0f);
		}
	}

	public override ObjectBase GetObjectBaseForLeash()
	{
		if ((bool)plant)
		{
			return plantObjectBase;
		}
		return base.GetObjectBaseForLeash();
	}

	protected virtual void OnTriggerEnter(Collider other)
	{
		testPottableObject = other.GetComponent<ObjectPottable>();
		if (testPottableObject != null && !testPottableObject.isPotted && testPottableObject.isPottable && plant == null && ToolManager.PlayerGrabbingObjectBase(testPottableObject))
		{
			plantObjectBase = testPottableObject;
			PotThePlant(other.gameObject);
		}
	}

	protected void PotThePlant(GameObject _plant)
	{
		plantRB = _plant.GetComponent<Rigidbody>();
		plantRB.isKinematic = true;
		plantObjectBase.rb.mass += rb.mass;
		if (isGrabbed)
		{
			ForceDropByPlayer();
		}
		Object.Destroy(rb);
		planting = true;
		plant = _plant;
		startPos = _plant.transform.position;
		startRot = _plant.transform.rotation;
		startScale = _plant.transform.localScale;
		potRot = transform.rotation;
		time = 0f;
		StartCoroutine(PotCoroutine());
	}

	private void Planting()
	{
		if (!planting)
		{
			return;
		}
		time += Time.deltaTime;
		pos1 = transform.TransformPoint(0f, 0.2f, 0f);
		pos2 = transform.TransformPoint(0f, 0.3f, 0f);
		pos3 = transform.TransformPoint(-plantObjectBase.potOffset);
		if (time <= timeLength.x)
		{
			curveEvaluated = curve1.Evaluate(time / timeLength.x);
			if (Mathf.Acos(Vector3.Dot(potRot * Vector3.up, Vector3.up)) * 57.29578f > 45f)
			{
				transform.rotation = Quaternion.Lerp(potRot, potRot * Quaternion.FromToRotation(potRot * Vector3.up, Vector3.up), curveEvaluated);
			}
			plant.transform.rotation = Quaternion.Lerp(startRot, Quaternion.FromToRotation(startRot * Vector3.up, transform.up) * startRot, curveEvaluated);
			plant.transform.position = Vector3.Lerp(startPos, pos1, curveEvaluated);
		}
		else if (time <= timeLength.x + timeLength.y)
		{
			curveEvaluated = curve2.Evaluate((time - timeLength.x) / timeLength.y);
			plant.transform.position = Vector3.Lerp(pos1, pos2, curveEvaluated);
		}
		else if (time <= timeLength.x + timeLength.y + timeLength.z)
		{
			curveEvaluated = curve3.Evaluate((time - timeLength.x - timeLength.y) / timeLength.z);
			scaleEvaluated = curveZscale.Evaluate((time - timeLength.x - timeLength.y) / timeLength.z);
			spinEvaluated = curveSpin.Evaluate((time - timeLength.x - timeLength.y) / timeLength.z);
			plant.transform.position = Vector3.Lerp(pos2, pos3, curveEvaluated);
			plant.transform.Rotate(Vector3.up, 480f * spinEvaluated * Time.deltaTime);
			plant.transform.localScale = new Vector3(startScale.x, startScale.y, startScale.z * scaleEvaluated);
		}
		Soiling();
		if (time > timeLength.x + timeLength.y + timeLength.z)
		{
			planting = false;
			transform.SetParent(plant.transform);
			plantRB.isKinematic = false;
			plantObjectBase.colliders = plantObjectBase.GetComponentsInChildren<Collider>();
			plantObjectBase.rb.centerOfMass = plantObjectBase.potOffset + new Vector3(0f, 0.045f, 0f);
		}
	}

	private void Soiling()
	{
	}

	private IEnumerator PotCoroutine()
	{
		plantObjectBase.myPot = this;
		plantObjectBase.OnPotted();
		plantObjectBase.ForceDropByPlayer();
		while (planting)
		{
			Planting();
			yield return null;
		}
		Satisfaction();
		yield return null;
	}

	public void BreakPot()
	{
		StopCoroutine(PotCoroutine());
		Vector3 velocity;
		Vector3 angularVelocity;
		if (plantObjectBase != null)
		{
			velocity = plantObjectBase.rb.velocity;
			angularVelocity = plantObjectBase.rb.angularVelocity;
		}
		else
		{
			velocity = rb.velocity;
			angularVelocity = rb.angularVelocity;
		}
		ObjectShard[] array = smashPieces;
		foreach (ObjectShard objectShard in array)
		{
			objectShard.OnBreak();
			objectShard.gameObject.SetActive(value: true);
			objectShard.shardRb.velocity = velocity;
			objectShard.shardRb.angularVelocity = angularVelocity;
			objectShard.shardRb.angularVelocity = angularVelocity;
			if (plantObjectBase != null)
			{
				objectShard.transform.SetParent(plantObjectBase.transform.parent);
			}
			else
			{
				objectShard.transform.SetParent(transform.parent);
			}
		}
		if (plantObjectBase != null)
		{
			smashParticles.transform.parent = plantObjectBase.transform.parent;
		}
		else
		{
			smashParticles.transform.parent = transform.parent;
		}
		smashParticles.SetActive(value: true);
		if ((bool)plant)
		{
			plantObjectBase.OnPottedBreak();
			plantObjectBase.myPot = null;
			plantRB.isKinematic = false;
			plant.layer = LayerMask.NameToLayer("Grabbable");
		}
		if ((bool)this == ToolManager.PlayerGrabbingObjectBase(this, dropIfGrabbing: true) || !plantObjectBase || ToolManager.PlayerGrabbingObjectBase(plantObjectBase, dropIfGrabbing: true))
		{
		}
		Object.Destroy(gameObject);
		Object.Destroy(this);
	}

	public bool hasPlant()
	{
		return plant != null;
	}

	protected override void OnCollisionEnter(Collision collision)
	{
		base.OnCollisionEnter(collision);
		if (ToolManager.IsTeleporting || !(collision.relativeVelocity.magnitude > smashVelocityMin))
		{
			return;
		}
		if (dontBreakOnColliders.Count > 0)
		{
			if (!dontBreakOnColliders.Contains(collision.gameObject))
			{
				BreakPot();
			}
		}
		else
		{
			BreakPot();
		}
	}
}
