using System.Collections.Generic;
using UnityEngine;

public class ObjectButter : ObjectBase
{
	private class ObjDistPair
	{
		public ObjectBase obj;

		public float originalDistanceFromButterCenter;

		public ObjDistPair(ObjectBase objIn, float originalDistanceIn)
		{
			obj = objIn;
			originalDistanceFromButterCenter = originalDistanceIn;
		}
	}

	public GameObject objParent;

	public GameObject objMouthParent;

	private FixedJoint fj;

	public bool isOutOfFridge;

	public GameObject meltingButter;

	public ParticleSystem impactParticles;

	public GameObject plateTrigger;

	public bool absorbingToast = true;

	public bool objReactions;

	public List<ObjectBase> stuckObj;

	public KitchenSequenceScriptableObject tweakables;

	[SerializeField]
	private Kitchen kitchen;

	[SerializeField]
	private VVR_JobSnack snack;

	private List<ObjDistPair> objStuckToMe = new List<ObjDistPair>();

	[SerializeField]
	private float toastSchlupSpeed = 0.1f;

	[SerializeField]
	private float maxToastDisplacement = 0.1f;

	[HideInInspector]
	public bool sticky = true;

	public AudioEvent splatSound;

	public VVRConsumable mouth;

	private bool carrotLine;

	private bool frogLine;

	private bool houseplantLine;

	private bool mugLine;

	private bool toasterLine;

	private int otherLine;

	protected override void Start()
	{
		objStuckToMe = new List<ObjDistPair>();
	}

	private void OnTriggerEnter(Collider trigger)
	{
		if (!sticky)
		{
			return;
		}
		ObjectToast component = trigger.gameObject.GetComponent<ObjectToast>();
		if (component != null)
		{
			AttachObj(component.GetComponent<ObjectBase>());
			KitchenToastManager.AddButterToast(component);
			return;
		}
		ObjectBase component2 = trigger.gameObject.GetComponent<ObjectBase>();
		if (component2 != null && stuckObj.Contains(component2))
		{
			AttachObj(component2);
		}
	}

	private void Update()
	{
		if (!absorbingToast)
		{
			return;
		}
		for (int i = 0; i < objStuckToMe.Count; i++)
		{
			if (objStuckToMe[i].obj != null)
			{
				float num = Vector3.Distance(objStuckToMe[i].obj.transform.position, transform.position);
				if (objStuckToMe[i].originalDistanceFromButterCenter - num < maxToastDisplacement)
				{
					Vector3 normalized = (transform.position - objStuckToMe[i].obj.transform.position).normalized;
					Vector3 vector = normalized * toastSchlupSpeed * Time.deltaTime;
					objStuckToMe[i].obj.transform.position += vector;
				}
				else
				{
					Object.Destroy(objStuckToMe[i].obj.gameObject);
					objStuckToMe.RemoveAt(i);
					i--;
				}
			}
			else
			{
				objStuckToMe.RemoveAt(i);
				i--;
			}
		}
	}

	private void AttachObj(ObjectBase objIn)
	{
		objStuckToMe.Add(new ObjDistPair(objIn, Vector3.Distance(objIn.transform.position, transform.position)));
		splatSound.Play();
		impactParticles.transform.position = objIn.transform.position;
		impactParticles.Play();
		ToolManager.PlayerGrabbingObjectBase(objIn, dropIfGrabbing: true, playSFX: false);
		if (objIn.transform.position.x > 0.26f)
		{
			objIn.transform.parent = objMouthParent.transform;
		}
		else
		{
			objIn.transform.parent = objParent.transform;
		}
		objIn.ungrabbable = true;
		Rigidbody component = objIn.GetComponent<Rigidbody>();
		if (component != null)
		{
			Object.Destroy(component);
		}
		Collider[] components = objIn.GetComponents<Collider>();
		if (components != null)
		{
			for (int i = 0; i < components.Length; i++)
			{
				if (components[i].isTrigger)
				{
					components[i].enabled = false;
				}
			}
		}
		ObjectToast component2 = objIn.GetComponent<ObjectToast>();
		if (component2 != null)
		{
			AttachToast(component2);
		}
		else if (objReactions)
		{
			if (!carrotLine && objIn.name.Contains("Carrot"))
			{
				carrotLine = true;
				GetComponent<VVRDialogueSource>().PlayClip(tweakables.dialogue.oCarrot);
			}
			else if (!frogLine && objIn.name.Contains("Toad"))
			{
				frogLine = true;
				GetComponent<VVRDialogueSource>().PlayClip(tweakables.dialogue.oFrog);
			}
			else if (!houseplantLine && objIn.name.Contains("Succulent"))
			{
				houseplantLine = true;
				GetComponent<VVRDialogueSource>().PlayClip(tweakables.dialogue.oHouesplant);
			}
			else if (!mugLine && objIn.name.Contains("Mug"))
			{
				mugLine = true;
				GetComponent<VVRDialogueSource>().PlayClip(tweakables.dialogue.oMug);
			}
			else if (!toasterLine && objIn.name.Contains("Toaster"))
			{
				toasterLine = true;
				GetComponent<VVRDialogueSource>().PlayClip(tweakables.dialogue.oToaster);
			}
			else if (otherLine < tweakables.dialogue.oOther.Length)
			{
				GetComponent<VVRDialogueSource>().PlayClip(tweakables.dialogue.oOther[otherLine]);
				otherLine++;
			}
		}
	}

	private void AttachToast(ObjectToast toastIn)
	{
		if (kitchen != null)
		{
			kitchen.NewToastAttachedToButter(toastIn);
		}
		else if (snack != null)
		{
			snack.NewToastAttachedToButter(toastIn);
		}
	}
}
