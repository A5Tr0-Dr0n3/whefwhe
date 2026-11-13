using System.Collections;
using TButt;
using UnityEngine;

[RequireComponent(typeof(VVRConsumer))]
public class ObjectStylus : ObjectBase
{
	[SerializeField]
	private float memoVomitInterval = 0.002f;

	[SerializeField]
	private float cursorTickInterval = 0.75f;

	[SerializeField]
	private VVRConsumable[] playerWrittenMemoItems;

	[SerializeField]
	private VVRConsumable[] stylusWrittenMemoItems;

	[SerializeField]
	private TextMesh memoCursor;

	[SerializeField]
	private AudioEvent stylusWriteEvent;

	private float cachedBreakForce;

	private bool memoRecordingEnabled;

	private bool memoRecordingEnabledThisFrame;

	private Renderer memoCursorRenderer;

	private FixedJoint mFixedJoint;

	private VVRConsumer mConsumer;

	public int memoIndex { get; private set; }

	public int maxMemoIndex { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		mConsumer = GetComponent<VVRConsumer>();
		memoCursorRenderer = memoCursor.GetComponent<Renderer>();
		mFixedJoint = GetComponent<FixedJoint>();
		cachedBreakForce = mFixedJoint.breakForce;
		mFixedJoint.breakForce = float.PositiveInfinity;
		memoCursorRenderer.enabled = false;
		for (int i = 0; i < playerWrittenMemoItems.Length; i++)
		{
			playerWrittenMemoItems[i].gameObject.SetActive(value: false);
		}
		for (int j = 0; j < stylusWrittenMemoItems.Length; j++)
		{
			stylusWrittenMemoItems[j].gameObject.SetActive(value: false);
		}
	}

	public override void GrabWith(Manipulator grabber)
	{
		mConsumer.isSucking = false;
		base.GrabWith(grabber);
		enableMemoRecording();
		mFixedJoint.breakForce = cachedBreakForce;
		grabber.orientDuringGrab = true;
		float z = 0f - gameObject.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.extents.z;
		grabber.heldHighVector = gameObject.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.center + new Vector3(0f, 0f, z) / transform.lossyScale.z;
		GlobalCoroutineContainer.AddThread(tickMemoCursor(), this);
	}

	private void enableMemoRecording()
	{
		memoRecordingEnabled = true;
		memoRecordingEnabledThisFrame = true;
		memoCursorRenderer.enabled = true;
		for (int i = 0; i < playerWrittenMemoItems.Length; i++)
		{
			playerWrittenMemoItems[i].gameObject.SetActive(value: true);
		}
	}

	private void Update()
	{
		if (!memoRecordingEnabled)
		{
			return;
		}
		maxMemoIndex = playerWrittenMemoItems.Length;
		if (!memoRecordingEnabledThisFrame && memoIndex < maxMemoIndex && memoIndex < playerWrittenMemoItems.Length)
		{
			memoCursorRenderer.enabled = true;
			memoCursor.transform.position = playerWrittenMemoItems[memoIndex].transform.position - playerWrittenMemoItems[memoIndex].transform.right * (0.015f + playerWrittenMemoItems[memoIndex].oRenderer.bounds.extents.x);
			if (TBInput.GetButtonDown(TBInput.Button.Action1, TBInput.Controller.Active))
			{
				playerWrittenMemoItems[memoIndex].OneShotTransfer(mConsumer, mConsumer.spewRate);
				memoIndex++;
				stylusWriteEvent.Play();
			}
		}
		else
		{
			memoCursorRenderer.enabled = false;
		}
		memoRecordingEnabledThisFrame = false;
	}

	public void ReplaceMemo()
	{
		GlobalCoroutineContainer.AddThread(replaceMemoSequence(), this);
	}

	private IEnumerator replaceMemoSequence()
	{
		memoRecordingEnabled = false;
		float memoVomitTimer = 0f;
		mConsumer.isSucking = true;
		int memoIndex = stylusWrittenMemoItems.Length - 1;
		while (memoIndex > 0)
		{
			memoVomitTimer += Time.deltaTime;
			while (memoVomitTimer > memoVomitInterval)
			{
				memoVomitTimer -= memoVomitInterval;
				stylusWrittenMemoItems[memoIndex].OneShotTransfer(mConsumer, mConsumer.suckRate);
				memoIndex--;
			}
			yield return null;
		}
		yield return new WaitForSeconds(0.2f);
		ForceDropByPlayer();
		gameObject.transform.position = Vector3.zero + Vector3.down * 10f;
		mConsumer.isSucking = false;
		for (int i = 0; i < stylusWrittenMemoItems.Length; i++)
		{
			stylusWrittenMemoItems[i].gameObject.SetActive(value: false);
		}
		yield return new WaitForSeconds(0.1f);
		gameObject.SetActive(value: false);
	}

	public bool MemoFullyWritten()
	{
		return memoIndex >= playerWrittenMemoItems.Length;
	}

	public IEnumerator tickMemoCursor()
	{
		while (true)
		{
			memoCursor.text = string.Empty;
			yield return new WaitForSeconds(cursorTickInterval);
			memoCursor.text = "|";
			yield return new WaitForSeconds(cursorTickInterval);
		}
	}
}
