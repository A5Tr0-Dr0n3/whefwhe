using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Butterfly))]
public class ObjectButterfly : ObjectBase
{
	public bool dieOnGrab;

	public float deathSpin = 5f;

	private Butterfly butterfly;

	private Butterfly defaultButterfly;

	private GameObject leftWing;

	private Rigidbody lRB;

	private GameObject rightWing;

	private Rigidbody rRB;

	protected override void Awake()
	{
		base.Awake();
		defaultButterfly = new Butterfly();
		butterfly = GetComponent<Butterfly>();
		defaultButterfly = defaultButterfly.GetCopyOf(butterfly);
	}

	public override void GrabWith(Manipulator grabber)
	{
		base.GrabWith(grabber);
		if (dieOnGrab)
		{
			StartCoroutine(Die());
			return;
		}
		butterfly.flapSpeed = 40f;
		butterfly.rotSpeed = 0f;
		butterfly.moveSpeed = 0f;
	}

	public override void GrabRelease(Manipulator grabber)
	{
		base.GrabRelease(grabber);
		butterfly.flapSpeed = defaultButterfly.flapSpeed;
		butterfly.rotSpeed = defaultButterfly.rotSpeed;
		butterfly.moveSpeed = defaultButterfly.moveSpeed;
	}

	private IEnumerator Die()
	{
		yield return new WaitForSeconds(0.3f);
		leftWing = GetComponent<Butterfly>().leftWing;
		leftWing.transform.parent = null;
		rightWing = GetComponent<Butterfly>().rightWing;
		rightWing.transform.parent = null;
		leftWing.AddComponent<BoxCollider>();
		lRB = leftWing.AddComponent<Rigidbody>();
		lRB.drag = 6f;
		lRB.centerOfMass = Vector3.zero;
		lRB.AddForce(Random.insideUnitSphere, ForceMode.Impulse);
		lRB.AddTorque(Random.insideUnitSphere * deathSpin, ForceMode.Impulse);
		rightWing.AddComponent<BoxCollider>();
		rRB = rightWing.AddComponent<Rigidbody>();
		rRB.drag = 6f;
		rRB.centerOfMass = Vector3.zero;
		lRB.AddForce(Random.insideUnitSphere, ForceMode.Impulse);
		rRB.AddTorque(Random.insideUnitSphere * deathSpin, ForceMode.Impulse);
		ForceDropByPlayer();
		Object.Destroy(gameObject);
		yield return null;
	}
}
