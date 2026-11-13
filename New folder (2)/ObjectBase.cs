using System.Collections.Generic;
using TButt;
using UnityEngine;
using UnityEngine.Events;

public class ObjectBase : MonoBehaviour
{
	[Header("Sound Settings")]
	public ObjectSoundType soundType;

	public float minAudibleForce = 0.75f;

	public float volumeMultiplier = 0.45f;

	[HideInInspector]
	public bool isGrabbed;

	public List<Manipulator> grabbedBy = new List<Manipulator>(2);

	[Header("Other settings")]
	public bool ungrabbable;

	public bool undroppable;

	public bool isButton;

	protected bool changeHeldLayer = true;

	protected int storedLayer;

	public bool forceNoInterpolate;

	[HideInInspector]
	public Rigidbody rb;

	[HideInInspector]
	public Collider[] colliders;

	protected Transform myTransform;

	public bool isSatisfied;

	public UnityEvent onBecomeGrabbed;

	[HideInInspector]
	public UnityEvent onHoverEnter = new UnityEvent();

	[HideInInspector]
	public UnityEvent onHoverExit = new UnityEvent();

	private float timeLastSoundTriggered;

	private float bufferBeforeNextSound = 0.15f;

	private float massVolumeScalar = 1f;

	private RigidbodyInterpolation startInterpolation;

	private GameObject colliderGO;

	protected int layerMaskInt;

	protected Mesh myMesh;

	public float smashVelocityMin = 5f;

	private Collider[] overlapTeleport = new Collider[10];

	protected virtual void Awake()
	{
		colliders = GetComponentsInChildren<Collider>();
		rb = GetComponentInChildren<Rigidbody>();
		if (rb != null)
		{
			massVolumeScalar = Mathf.Max(0.5f, Mathf.Sqrt(rb.mass));
			startInterpolation = rb.interpolation;
			if (TBCore.GetActivePlatform() != VRPlatform.OculusMobile && !forceNoInterpolate)
			{
				rb.interpolation = RigidbodyInterpolation.Interpolate;
			}
		}
		myTransform = transform;
	}

	protected virtual void Start()
	{
	}

	public virtual ObjectBase GetObjectBaseForLeash()
	{
		return this;
	}

	public virtual void GrabWith(Manipulator grabber)
	{
		if (ImpactSoundPool.instance != null)
		{
			ImpactSoundPool.instance.SetGrabbedObject(this);
		}
		grabbedBy.Add(grabber);
		if (!isGrabbed)
		{
			isGrabbed = true;
			if (changeHeldLayer)
			{
				storedLayer = gameObject.layer;
				layerMaskInt = LayerMask.NameToLayer("Held");
				gameObject.layer = layerMaskInt;
				for (int i = 0; i < colliders.Length; i++)
				{
					if (colliders[i] != null)
					{
						colliders[i].gameObject.layer = layerMaskInt;
					}
				}
			}
			if (onBecomeGrabbed != null)
			{
				onBecomeGrabbed.Invoke();
			}
		}
		if (rb != null && !forceNoInterpolate)
		{
			rb.interpolation = RigidbodyInterpolation.Interpolate;
		}
	}

	public virtual void Satisfaction()
	{
		isSatisfied = true;
	}

	public virtual void GrabRelease(Manipulator grabber)
	{
		grabbedBy.Remove(grabber);
		if (grabbedBy.Count <= 0)
		{
			OnDrop();
		}
	}

	public virtual void ForceDropByPlayer(bool playSFX = true)
	{
		for (int num = grabbedBy.Count - 1; num >= 0; num--)
		{
			grabbedBy[num].Drop(playSFX);
		}
		grabbedBy.Clear();
	}

	public void SetCollidersEnabled(bool enabled)
	{
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders[i].enabled = enabled;
		}
	}

	protected virtual void OnCollisionEnter(Collision collision)
	{
		if (collision.relativeVelocity.magnitude > minAudibleForce && Time.time - timeLastSoundTriggered > bufferBeforeNextSound)
		{
			ImpactSoundPool.instance.PlayImpactSound(Mathf.Clamp01(collision.relativeVelocity.magnitude * volumeMultiplier * massVolumeScalar), this);
			timeLastSoundTriggered = Time.time;
		}
	}

	public virtual void onTeleport()
	{
		if (Physics.OverlapSphereNonAlloc(transform.position, 0.2f, overlapTeleport, ~((1 << LayerMask.NameToLayer("Teleport")) | (1 << LayerMask.NameToLayer("Grabbable"))), QueryTriggerInteraction.Ignore) <= 0)
		{
			return;
		}
		for (int i = 0; i < ToolManager.ActiveToolGroups.Count; i++)
		{
			if (ToolManager.ActiveToolGroups[i].Manipulator.grabTargetDistance > 0.2f)
			{
				ToolManager.ActiveToolGroups[i].Manipulator.AnimateGrabToLength(0.2f, 0.2f, dropObject: false);
			}
		}
	}

	public virtual void OnHoverStay(Manipulator hoverer)
	{
		if (onHoverEnter != null)
		{
			onHoverEnter.Invoke();
		}
	}

	public virtual void OnHoverExit()
	{
		if (onHoverExit != null)
		{
			onHoverExit.Invoke();
		}
	}

	public virtual void OnDrop()
	{
		if (changeHeldLayer)
		{
			gameObject.layer = storedLayer;
			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i] != null)
				{
					colliders[i].gameObject.layer = storedLayer;
				}
			}
		}
		if (ImpactSoundPool.instance != null)
		{
			ImpactSoundPool.instance.ReleaseGrabbedObject();
		}
		isGrabbed = false;
	}

	public virtual void GrabUpdate(Manipulator grabber)
	{
	}

	public virtual void OnButtonPress()
	{
	}

	public virtual void OnHoverEnter()
	{
	}
}
