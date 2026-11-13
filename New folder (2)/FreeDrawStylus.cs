using System.Collections;
using System.Collections.Generic;
using TButt;
using UnityEngine;

[RequireComponent(typeof(FixedJoint))]
public class FreeDrawStylus : ObjectBase
{
	private enum FreeDrawStylusState
	{
		free,
		drawing
	}

	[SerializeField]
	private Transform mCursor;

	[SerializeField]
	private float maxCursorDistance = 7.5f;

	[SerializeField]
	private float dropPointDist = 0.1f;

	[SerializeField]
	private Material lineRendererMat;

	[SerializeField]
	private float lineRendererWidth = 0.1f;

	[SerializeField]
	private int lineRendererCornerVerts = 2;

	[SerializeField]
	private float trailUndrawSpeed = 30f;

	[SerializeField]
	private Color trailColor;

	[SerializeField]
	private Animator whiteboardAnimator;

	[SerializeField]
	private GvrAudioSource whiteboardAudioSource;

	[SerializeField]
	private AudioClip whiteboardPopIn;

	[SerializeField]
	private AudioClip whiteboardPopOut;

	[SerializeField]
	private float hitOffsetDist = 0.01f;

	[SerializeField]
	private LayerMask cursorMask;

	[SerializeField]
	private FreedrawSfx sfxModule;

	[SerializeField]
	private bool limitTrailCount;

	[SerializeField]
	private int maxTrailLimit = 10;

	[SerializeField]
	private List<int> undrawTrailIds;

	private FreeDrawStylusState mState;

	private FixedJoint mFixedJoint;

	private List<FreeDrawLineRenderer> mRenderers = new List<FreeDrawLineRenderer>();

	private FreeDrawLineRenderer activeRenderer;

	private float timeSinceUsed;

	private bool hasBeenUsedOnce;

	private bool grabClick;

	private float cachedBreakForce;

	private float currentStrokeDistance;

	private float currentStrokeMaxCursorDistance;

	protected override void Start()
	{
		base.Start();
		mCursor.gameObject.SetActive(value: false);
		mFixedJoint = GetComponent<FixedJoint>();
		cachedBreakForce = mFixedJoint.breakForce;
		mFixedJoint.breakForce = float.PositiveInfinity;
		currentStrokeMaxCursorDistance = maxCursorDistance;
		sfxModule.SetSoundsEnabled(enabled: false);
	}

	public override void GrabWith(Manipulator grabber)
	{
		ForceDropByPlayer(playSFX: false);
		base.GrabWith(grabber);
		mCursor.gameObject.SetActive(value: true);
		undroppable = true;
		float z = (0f - gameObject.GetComponentInChildren<Collider>().bounds.extents.z) * 0.8f;
		grabber.GrabbingToolGroup.Manipulator.heldHighVector = gameObject.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.center + new Vector3(0f, 0f, z) / transform.lossyScale.z;
		grabber.GrabbingToolGroup.Manipulator.orientDuringGrab = true;
		grabClick = true;
		if (mFixedJoint != null)
		{
			mFixedJoint.breakForce = cachedBreakForce;
		}
		if (whiteboardAnimator != null)
		{
			whiteboardAnimator.SetTrigger("PopIn");
			whiteboardAudioSource.PlayOneShot(whiteboardPopIn);
		}
	}

	public override void GrabUpdate(Manipulator grabber)
	{
		if (grabber != grabbedBy[0])
		{
			return;
		}
		base.GrabUpdate(grabber);
		Vector3 position = transform.position + transform.forward * currentStrokeMaxCursorDistance;
		if (Physics.Raycast(new Ray(transform.position, transform.forward), out var hitInfo, currentStrokeMaxCursorDistance, cursorMask.value, QueryTriggerInteraction.Ignore))
		{
			Vector3 vector = hitInfo.point + hitInfo.normal * hitOffsetDist;
			position = vector;
			if (mState == FreeDrawStylusState.free && TBInput.GetButtonDown(TBInput.Button.Action1, grabber.GrabbingToolGroup.Controller) && !grabClick)
			{
				hasBeenUsedOnce = true;
				mState = FreeDrawStylusState.drawing;
				activeRenderer = new GameObject("LineRenderer path " + mRenderers.Count).AddComponent<FreeDrawLineRenderer>();
				activeRenderer.transform.parent = transform;
				mRenderers.Add(activeRenderer);
				activeRenderer.Initialize(lineRendererWidth, trailColor, lineRendererCornerVerts, lineRendererMat, hitInfo.transform);
				activeRenderer.AddPoint(vector);
				currentStrokeDistance = Vector3.Distance(transform.position, vector);
				currentStrokeMaxCursorDistance = Mathf.Min(currentStrokeDistance + 1f, maxCursorDistance);
				sfxModule.StartEndStroke(grabbedBy[0].transform);
				sfxModule.SetSoundsEnabled(enabled: true);
			}
			if (mState == FreeDrawStylusState.drawing)
			{
				sfxModule.Tick(grabbedBy[0].transform);
				float num = Vector3.Distance(vector, activeRenderer.GetLastPoint());
				if (num > dropPointDist)
				{
					activeRenderer.AddPoint(vector);
					currentStrokeDistance = Vector3.Distance(transform.position, vector);
				}
			}
		}
		else if (mState == FreeDrawStylusState.drawing)
		{
			sfxModule.Tick(grabbedBy[0].transform);
			Vector3 vector2 = transform.position + transform.forward * currentStrokeDistance;
			float num2 = Vector3.Distance(vector2, activeRenderer.GetLastPoint());
			if (num2 > dropPointDist)
			{
				activeRenderer.AddPoint(vector2);
			}
		}
		if (mState == FreeDrawStylusState.drawing && TBInput.GetButtonUp(TBInput.Button.Action1, grabber.GrabbingToolGroup.Controller))
		{
			mState = FreeDrawStylusState.free;
			currentStrokeMaxCursorDistance = maxCursorDistance;
			sfxModule.StartEndStroke(grabbedBy[0].transform);
			if (limitTrailCount && mRenderers.Count > maxTrailLimit)
			{
				for (int i = 0; i < mRenderers.Count - maxTrailLimit; i++)
				{
					if (!undrawTrailIds.Contains(mRenderers[i].id))
					{
						undrawTrailIds.Add(mRenderers[i].id);
					}
				}
			}
		}
		if (TBInput.GetButtonUp(TBInput.Button.Action1, grabber.GrabbingToolGroup.Controller))
		{
			grabClick = false;
		}
		mCursor.transform.position = position;
		if (TBInput.GetButton(TBInput.Button.Action1, grabber.GrabbingToolGroup.Controller))
		{
			timeSinceUsed = 0f;
		}
		else
		{
			timeSinceUsed += Time.deltaTime;
		}
	}

	public override void GrabRelease(Manipulator grabber)
	{
		mCursor.gameObject.SetActive(value: false);
		base.GrabRelease(grabber);
	}

	public float GetTimeInactive()
	{
		return timeSinceUsed;
	}

	public bool HasBeenUsedOnce()
	{
		return hasBeenUsedOnce;
	}

	public void EraseMemo()
	{
		GlobalCoroutineContainer.AddThread(eraseMemoSequence(), this);
	}

	public void UndrawAllTrails()
	{
		GlobalCoroutineContainer.AddThread(undrawAllTrailsSequence(), this);
	}

	private void Update()
	{
		if (undrawTrailIds.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < mRenderers.Count; i++)
		{
			for (int j = 0; j < undrawTrailIds.Count; j++)
			{
				if (mRenderers[i].id == undrawTrailIds[j] && mRenderers[i].TickUndrawTrail(Time.deltaTime, trailUndrawSpeed))
				{
					Object.Destroy(mRenderers[i].gameObject);
					mRenderers.RemoveAt(i);
					i--;
				}
			}
		}
	}

	private IEnumerator undrawAllTrailsSequence()
	{
		yield return null;
		bool cachedUngrabbable = ungrabbable;
		ungrabbable = false;
		while (mRenderers.Count > 0)
		{
			for (int i = 0; i < mRenderers.Count; i++)
			{
				if (mRenderers[i].TickUndrawTrail(Time.deltaTime, trailUndrawSpeed))
				{
					Object.Destroy(mRenderers[i].gameObject);
					mRenderers.RemoveAt(i);
					i--;
				}
			}
			yield return null;
		}
		ungrabbable = cachedUngrabbable;
	}

	private IEnumerator eraseMemoSequence()
	{
		if (whiteboardAnimator != null)
		{
			whiteboardAudioSource.PlayOneShot(whiteboardPopOut);
			whiteboardAnimator.SetTrigger("PopOut");
		}
		Transform cachedGrabTransform = ((!isGrabbed) ? ToolManager.ActiveToolGroups[0].Manipulator.transform : grabbedBy[0].transform);
		ForceDropByPlayer();
		ungrabbable = true;
		rb.velocity *= 1.5f;
		sfxModule.ActivateUndrawSound();
		for (int i = 0; i < mRenderers.Count; i++)
		{
			mRenderers[i].CacheParentTransform(transform.parent);
		}
		yield return GlobalCoroutineContainer.AddThread(undrawAllTrailsSequence(), this);
		sfxModule.StartEndStroke(cachedGrabTransform);
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(2f, this);
		sfxModule.SetSoundsEnabled(enabled: false);
		gameObject.SetActive(value: false);
	}
}
