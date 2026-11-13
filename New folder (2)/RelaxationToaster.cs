using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class RelaxationToaster : ObjectBase
{
	[Header("References")]
	[SerializeField]
	private Transform leftToastSpawnPos;

	[SerializeField]
	private Transform rightToastSpawnPos;

	[SerializeField]
	private ObjectPooledToast[] toastPool;

	[SerializeField]
	private float launchForce;

	[SerializeField]
	private Transform leftToastLoadPos;

	[SerializeField]
	private Transform rightToastLoadPos;

	[SerializeField]
	private Transform leftToastDownPos;

	[SerializeField]
	private Transform rightToastDownPos;

	public AudioClip toastStart;

	public AudioClip toastEject;

	public AudioClip toastExplosion;

	public ParticleSystem burntParticles;

	public ParticleSystem smokeParticles;

	public ParticleSystem toastParticles;

	[SerializeField]
	private Transform handle;

	private Transform leftToastTransform;

	private Transform rightToastTransform;

	[Header("Tweakables")]
	private Vector3 handleStart = new Vector3(0f, 0.05f, 0f);

	private bool isToasting;

	private Transform startParent;

	private ObjectToaster.Toast toastState;

	private SafeCoroutine cookingThread;

	private SafeCoroutine ejectThread;

	private ObjectPooledToast leftToast;

	private ObjectPooledToast rightToast;

	private bool toastCommentHasPlayed;

	private bool hasEjected;

	private void OnEnable()
	{
		ItemDisplaySwapper.OnFlip += ResetToastPool;
	}

	private void OnDisable()
	{
		ItemDisplaySwapper.OnFlip -= ResetToastPool;
	}

	protected override void Awake()
	{
		base.Awake();
		if (startParent == null)
		{
			startParent = toastPool[0].transform.parent;
		}
		ResetToastPool();
	}

	protected override void Start()
	{
		base.Start();
		ResetToastPool();
	}

	public override void OnButtonPress()
	{
		base.OnButtonPress();
		if (cookingThread != null && cookingThread.IsRunning && leftToast != null && rightToast != null)
		{
			EjectToast(leftToast, rightToast);
			return;
		}
		StartCookingToast();
		if (!toastCommentHasPlayed)
		{
			toastCommentHasPlayed = true;
			VVR_HUB.instance.GetComponent<HubExt>().CommentSnackToast();
		}
	}

	public SafeCoroutine StartCookingToast()
	{
		if (cookingThread == null || !cookingThread.IsRunning)
		{
			cookingThread = GlobalCoroutineContainer.AddThread(CookToast(), this);
			return cookingThread;
		}
		return null;
	}

	private IEnumerator CookToast()
	{
		hasEjected = false;
		float waitTime = 2.5f;
		float loadToastTimer = 0f;
		handle.localPosition = handleStart;
		AudioSource.PlayClipAtPoint(toastStart, transform.position, 0.1f);
		leftToast = GetNextUnusedToast(leftToastSpawnPos.position);
		rightToast = GetNextUnusedToast(rightToastSpawnPos.position);
		leftToast.SetCollidersEnabled(enabled: false);
		rightToast.SetCollidersEnabled(enabled: false);
		leftToastTransform = leftToast.transform;
		rightToastTransform = rightToast.transform;
		leftToastTransform.SetParent(transform, worldPositionStays: true);
		rightToastTransform.SetParent(transform, worldPositionStays: true);
		leftToast.rb.isKinematic = true;
		rightToast.rb.isKinematic = true;
		leftToast.rb.useGravity = false;
		rightToast.rb.useGravity = false;
		leftToast.rb.interpolation = RigidbodyInterpolation.None;
		rightToast.rb.interpolation = RigidbodyInterpolation.None;
		isButton = false;
		ungrabbable = true;
		toastState = ObjectToaster.Toast.Uncooked;
		while (loadToastTimer <= 1f)
		{
			loadToastTimer += Time.deltaTime;
			handle.transform.localPosition = Vector3.Lerp(handle.transform.localPosition, Vector3.zero, loadToastTimer);
			leftToastTransform.transform.position = Vector3.Lerp(leftToastLoadPos.position, leftToastDownPos.position, loadToastTimer);
			rightToastTransform.transform.position = Vector3.Lerp(rightToastLoadPos.position, rightToastDownPos.position, loadToastTimer);
			yield return null;
		}
		isButton = true;
		ungrabbable = false;
		isToasting = true;
		handle.transform.localPosition = Vector3.zero;
		smokeParticles.Play();
		float toastWaitTime = 1.5f;
		float toastTimer = 0f;
		toastState = ObjectToaster.Toast.Uncooked;
		while (toastTimer <= waitTime)
		{
			toastTimer += Time.deltaTime / toastWaitTime;
			if (toastTimer >= 0.5f)
			{
				toastState = ObjectToaster.Toast.Toasted;
			}
			yield return null;
		}
		toastState = ObjectToaster.Toast.Burning;
		smokeParticles.Stop();
		if (!hasEjected)
		{
			burntParticles.Play();
		}
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(0.5f, this);
		yield return EjectToast(leftToast, rightToast);
		yield return null;
	}

	public SafeCoroutine EjectToast(ObjectPooledToast leftToast, ObjectPooledToast rightToast)
	{
		if ((ejectThread == null || !ejectThread.IsRunning) && !hasEjected)
		{
			return ejectThread = GlobalCoroutineContainer.AddThread(EjectToastCoroutine(leftToast, rightToast), this);
		}
		return null;
	}

	private IEnumerator EjectToastCoroutine(ObjectPooledToast leftToast, ObjectPooledToast rightToast)
	{
		hasEjected = true;
		leftToast.SetMesh(toastState);
		rightToast.SetMesh(toastState);
		smokeParticles.Stop();
		burntParticles.Stop();
		toastParticles.Stop();
		AudioSource.PlayClipAtPoint(toastEject, transform.position, 0.4f);
		leftToast.rb.isKinematic = false;
		leftToast.rb.useGravity = true;
		leftToast.transform.SetParent(transform.parent, worldPositionStays: true);
		rightToast.rb.isKinematic = false;
		rightToast.rb.useGravity = true;
		rightToast.transform.SetParent(transform.parent, worldPositionStays: true);
		leftToast.rb.interpolation = RigidbodyInterpolation.Interpolate;
		rightToast.rb.interpolation = RigidbodyInterpolation.Interpolate;
		Vector3 launchDir = (Vector3.up * 2f + Vector3.right).normalized;
		leftToast.rb.AddForce(Vector3.up * 10f * launchForce + launchDir * 45f);
		rightToast.rb.AddForce(Vector3.up * 10f * launchForce + launchDir * 45f);
		GlobalCoroutineContainer.AddThread(EnableToastCollisionOnDelay(0.25f, leftToast), this);
		GlobalCoroutineContainer.AddThread(EnableToastCollisionOnDelay(0.25f, rightToast), this);
		leftToast.toastState = toastState;
		rightToast.toastState = toastState;
		yield return null;
	}

	private ObjectPooledToast GetNextUnusedToast(Vector3 pos)
	{
		ObjectPooledToast objectPooledToast = null;
		for (int i = 0; i < toastPool.Length; i++)
		{
			if (!toastPool[i].gameObject.activeInHierarchy)
			{
				objectPooledToast = toastPool[i];
				objectPooledToast.transform.position = pos;
				objectPooledToast.gameObject.SetActive(value: true);
			}
			if (objectPooledToast != null)
			{
				break;
			}
		}
		if (objectPooledToast == null)
		{
			toastPool[0].ForceDropByPlayer();
			toastPool[0].gameObject.SetActive(value: false);
			toastPool[0].transform.rotation = toastPool[0].startRot;
			return GetNextUnusedToast(pos);
		}
		return objectPooledToast;
	}

	private void ResetToastPool()
	{
		for (int i = 0; i < toastPool.Length; i++)
		{
			toastPool[i].transform.SetParent(startParent, worldPositionStays: true);
			toastPool[i].gameObject.SetActive(value: false);
			toastPool[i].SetCollidersEnabled(enabled: false);
		}
	}

	private IEnumerator EnableToastCollisionOnDelay(float s, ObjectPooledToast toast)
	{
		yield return GlobalCoroutineContainer.SafeWaitForSeconds(s, this);
		toast.SetCollidersEnabled(enabled: true);
	}
}
