using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class ChazGrabber : MonoBehaviour
{
	private enum ChazGrabberState
	{
		inactive,
		grabbing
	}

	[Header("Scene References")]
	[SerializeField]
	private Transform plugTarget;

	[SerializeField]
	private Transform chazHandlw;

	[SerializeField]
	private ChazResetter chazReset;

	[SerializeField]
	private ChazController chaz;

	[SerializeField]
	private Transform holdLocation;

	[SerializeField]
	private Rigidbody clawRb;

	[SerializeField]
	private GvrAudioSource magnetStartSound;

	[SerializeField]
	private GvrAudioSource magnetLoopSound;

	[Header("Tweakables")]
	public float dropForce = 900f;

	private Vector3 clawHome = Vector3.zero;

	private Vector3 targetPos;

	private SafeCoroutine magnetThread;

	private SafeCoroutine movingThread;

	private bool hasChaz;

	private bool returningChaz;

	private void Awake()
	{
		if (clawHome == Vector3.zero)
		{
			SetClawHome(clawRb.transform.position);
		}
	}

	private void Update()
	{
	}

	public void PrepareChaz()
	{
		chaz.DropChaz();
		chazReset.ResetChaz(Vector3.zero, move: false);
		chaz.DisablePhysics();
	}

	public void MoveGrabberToChaz(float speed)
	{
		if (movingThread == null)
		{
			movingThread = GlobalCoroutineContainer.AddThread(MoveClawToChaz(speed), this);
		}
	}

	public void ActivateMagnet(float force)
	{
		if (magnetThread == null)
		{
			hasChaz = true;
			chaz.isMagnetized = true;
			magnetThread = GlobalCoroutineContainer.AddThread(LatchChazToMagnet(force), this);
		}
	}

	public void DeactivateMagnet()
	{
		hasChaz = false;
		chaz.isMagnetized = false;
		magnetThread.Stop();
		magnetThread = null;
		magnetLoopSound.Stop();
	}

	public void MoveGrabberToCeiling(float speed)
	{
		if (movingThread == null)
		{
			movingThread = GlobalCoroutineContainer.AddThread(MoveClawToPos(clawHome, speed), this);
		}
	}

	public void ReturnGrabberToGround(float speed)
	{
		if (movingThread == null)
		{
			movingThread = GlobalCoroutineContainer.AddThread(MoveClawToPos(targetPos, speed), this);
		}
	}

	public void SetClawHome(Vector3 s)
	{
		clawHome = s;
	}

	public void SetTargetPos(Vector3 s)
	{
		targetPos = s;
	}

	public IEnumerator MoveClawToChaz(float time)
	{
		float t = 0f;
		SetTargetPos(chazHandlw.position + Vector3.up);
		Vector3 startpos = clawRb.transform.position;
		chaz.GoLimpDrag();
		chaz.GoLimpSpring();
		while (t < 1f)
		{
			t += Time.deltaTime / time;
			plugTarget.position = Vector3.Lerp(startpos, chazHandlw.position + Vector3.up, t);
			yield return null;
		}
		yield return null;
		movingThread = null;
	}

	public IEnumerator MoveClawToPos(Vector3 pos, float time)
	{
		float t = 0f;
		chaz.GoLimpDrag();
		chaz.GoLimpSpring();
		Vector3 startPos = clawRb.transform.position;
		while (t < 1f)
		{
			t += Time.deltaTime / time;
			plugTarget.position = Vector3.Lerp(startPos, pos, t);
			yield return null;
		}
		yield return null;
		movingThread = null;
	}

	public IEnumerator LatchChazToMagnet(float force = 300f, float threshhold = 0.5f)
	{
		chaz.KillCoroutines();
		float grabbedTime = 0f;
		bool attracting = true;
		float t = 0f;
		while (true)
		{
			if (ToolManager.PlayerGrabbingObjectType<ObjectChazLeg>())
			{
				if (movingThread == null)
				{
					plugTarget.position = chazHandlw.position + Vector3.up * threshhold;
				}
				grabbedTime += Time.deltaTime;
				if (grabbedTime > 25f)
				{
					chaz.DropChaz();
					grabbedTime = 0f;
				}
			}
			if (Vector3.Distance(chaz.head.transform.position, holdLocation.position) > threshhold && attracting)
			{
				if (movingThread == null)
				{
					Vector3 position = chazHandlw.position;
					position.y = plugTarget.position.y;
					plugTarget.position = position;
				}
				t = 0f;
				Vector3 vector = holdLocation.position - chaz.head.transform.position;
				vector.Normalize();
				SafeDebug.DrawRay(chaz.head.transform.position, vector, Color.blue);
				vector *= force;
				chaz.GoLimpDrag();
				chaz.GoLimpSpring();
				chaz.head.GetComponent<Rigidbody>().AddForce(vector);
			}
			else
			{
				if (movingThread == null)
				{
					Vector3 vector2 = chazHandlw.position + Vector3.up * threshhold;
					vector2.y = targetPos.y;
					targetPos = vector2;
				}
				t += Time.deltaTime;
				t = Mathf.Clamp01(t);
				if (ToolManager.PlayerGrabbingObjectType<ObjectChazLeg>())
				{
					attracting = true;
				}
				else
				{
					chaz.GoLimpDrag();
					chaz.GoLimpSpring();
				}
			}
			yield return null;
		}
	}
}
