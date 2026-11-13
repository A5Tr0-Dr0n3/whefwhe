using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using UnityEngine;

public class ObjectToaster : ObjectBase
{
	public enum Toast
	{
		Uncooked,
		Toasted,
		Burning,
		SOS,
		TBD
	}

	[Range(0.01f, 2f)]
	public float launchPower = 1f;

	public bool toastLaunchDebug;

	public bool tutorialDone;

	public bool tutorialToaster;

	public bool fireToaster;

	public ObjectToast[] extraToast;

	public static List<ObjectToast> loaf = new List<ObjectToast>();

	public ParticleSystem burntParticles;

	public ParticleSystem smokeParticles;

	public ParticleSystem toastParticles;

	private bool uncookedparticlesShouldPlay;

	private bool toastedParticlesShouldPlay;

	private bool burntParticlesShouldPlay;

	public GameObject target;

	public GameObject handle;

	public GameObject toastLeft;

	public GameObject toastRight;

	public GameObject toastParent;

	public AudioClip toastStart;

	public AudioClip toastEject;

	public AudioClip toastExplosion;

	public Mesh uncookedMesh;

	public Mesh cookedMesh;

	public Mesh burntMesh;

	[SerializeField]
	private bool startCookingToastOnStart = true;

	private float waitTime;

	private float maxStartTime = 5f;

	private float minToastTime = 6f;

	private float maxToastTime = 15f;

	private float minBurnTime = 5f;

	private float maxBurnTime = 15f;

	private float minExplodeTime = 15f;

	private float maxExplodeTime = 45f;

	private float loadToasterTimer;

	[HideInInspector]
	public bool startedToasting;

	[HideInInspector]
	public bool hasntEjected = true;

	public Toast toast;

	private Vector3 handleStart = new Vector3(0f, 0.05f, 0f);

	private SafeCoroutine cookCoroutine;

	private SafeCoroutine loadCoroutine;

	private SafeCoroutine popCoroutine;

	private Vector3 toastStartLeft;

	private Vector3 toastStartRight;

	private Vector3 toastStartParent;

	public GameObject toastCopyTemplate;

	private float lateralSpeed = 45f;

	private float verticalSpeedMin = 65f;

	private float verticalSpeedMax = 95f;

	[HideInInspector]
	public bool finalToast;

	public float reloadTime = 2.5f;

	public float loadingSpeed = 1f;

	protected override void Awake()
	{
		base.Awake();
		toastStartLeft = toastLeft.transform.localPosition;
		toastStartRight = toastRight.transform.localPosition;
		toastStartParent = toastParent.transform.localPosition;
		if (!fireToaster)
		{
			for (int i = 0; i < extraToast.Length; i++)
			{
				loaf.Add(extraToast[i]);
			}
		}
	}

	protected override void Start()
	{
		if (!fireToaster)
		{
			if (startCookingToastOnStart)
			{
				cookCoroutine = GlobalCoroutineContainer.AddThread(CookToast(), this);
			}
		}
		else
		{
			toastLeft.SetActive(value: false);
			toastRight.SetActive(value: false);
		}
		toastRight.GetComponent<VVRConsumable>().unconsumable = true;
		toastLeft.GetComponent<VVRConsumable>().unconsumable = true;
	}

	private void OnEnable()
	{
		if (uncookedparticlesShouldPlay)
		{
			smokeParticles.Play();
		}
		if (toastedParticlesShouldPlay)
		{
			toastParticles.Play();
		}
		if (burntParticlesShouldPlay)
		{
			burntParticles.Play();
		}
	}

	private void OnDisable()
	{
		uncookedparticlesShouldPlay = smokeParticles.isPlaying;
		toastedParticlesShouldPlay = toastParticles.isPlaying;
		burntParticlesShouldPlay = burntParticles.isPlaying;
	}

	public void StartCookingToast()
	{
		if (cookCoroutine == null || (cookCoroutine != null && !cookCoroutine.IsRunning))
		{
			if (cookCoroutine != null && cookCoroutine.IsRunning)
			{
				cookCoroutine.Stop();
			}
			cookCoroutine = GlobalCoroutineContainer.AddThread(CookToast(), this);
		}
	}

	private IEnumerator CookToast()
	{
		if (loadCoroutine != null && loadCoroutine.IsRunning)
		{
			loadCoroutine.Stop();
			loadCoroutine = null;
		}
		toast = Toast.Uncooked;
		if (!tutorialToaster)
		{
			waitTime = Random.Range(5f, maxStartTime);
		}
		else
		{
			waitTime = 2f;
		}
		handle.transform.localPosition = handleStart;
		float t = 0f;
		while (t < waitTime)
		{
			t += Time.deltaTime;
			yield return null;
		}
		AudioSource.PlayClipAtPoint(toastStart, transform.position, 0.1f);
		while (loadToasterTimer < 1f)
		{
			loadToasterTimer += Time.deltaTime;
			handle.transform.localPosition = Vector3.Lerp(handle.transform.localPosition, Vector3.zero, loadToasterTimer);
			toastParent.transform.localPosition = Vector3.Lerp(toastParent.transform.localPosition, Vector3.zero, loadToasterTimer);
			yield return null;
		}
		loadToasterTimer = 0f;
		isButton = true;
		startedToasting = true;
		handle.transform.localPosition = Vector3.zero;
		smokeParticles.Play();
		uncookedparticlesShouldPlay = true;
		if (!tutorialToaster)
		{
			waitTime = Random.Range(minToastTime, maxToastTime);
		}
		else
		{
			waitTime = 3f;
		}
		t = 0f;
		while (t < waitTime)
		{
			t += Time.deltaTime;
			yield return null;
		}
		toast = Toast.Toasted;
		toastLeft.GetComponent<MeshFilter>().sharedMesh = cookedMesh;
		toastRight.GetComponent<MeshFilter>().sharedMesh = cookedMesh;
		smokeParticles.Stop();
		uncookedparticlesShouldPlay = false;
		toastParticles.Play();
		toastedParticlesShouldPlay = true;
		if (!tutorialToaster)
		{
			waitTime = Random.Range(minBurnTime, maxBurnTime);
		}
		else
		{
			waitTime = 4f;
		}
		t = 0f;
		while (t < waitTime)
		{
			t += Time.deltaTime;
			yield return null;
		}
		toastParticles.Stop();
		toastedParticlesShouldPlay = false;
		burntParticles.Play();
		burntParticlesShouldPlay = true;
		toast = Toast.Burning;
		toastLeft.GetComponent<MeshFilter>().sharedMesh = burntMesh;
		toastRight.GetComponent<MeshFilter>().sharedMesh = burntMesh;
		waitTime = Random.Range(minExplodeTime, maxExplodeTime);
		t = 0f;
		while (t < waitTime)
		{
			t += Time.deltaTime;
			yield return null;
		}
		toast = Toast.SOS;
		EjectToast();
		cookCoroutine = null;
	}

	public void EjectToast(bool popLastToast = false)
	{
		if (startedToasting)
		{
			if (tutorialToaster)
			{
				tutorialDone = true;
			}
			isButton = false;
			finalToast = popLastToast;
			if (cookCoroutine != null && cookCoroutine.IsRunning)
			{
				cookCoroutine.Stop();
				cookCoroutine = null;
			}
			popCoroutine = GlobalCoroutineContainer.AddThread(PopOutToast(), this);
		}
	}

	private IEnumerator PopOutToast()
	{
		startedToasting = false;
		if (loadCoroutine != null && loadCoroutine.IsRunning)
		{
			loadCoroutine.Stop();
			loadCoroutine = null;
		}
		yield return null;
		GetComponent<AudioSource>().Stop();
		smokeParticles.Stop();
		burntParticles.Stop();
		toastParticles.Stop();
		uncookedparticlesShouldPlay = false;
		toastedParticlesShouldPlay = false;
		burntParticlesShouldPlay = false;
		hasntEjected = false;
		AudioSource.PlayClipAtPoint(toastEject, transform.position, 0.4f);
		toastRight.transform.parent = EagerScenes.instance.ActiveScene.transform;
		toastLeft.transform.parent = EagerScenes.instance.ActiveScene.transform;
		toastRight.GetComponent<Rigidbody>().isKinematic = false;
		toastLeft.GetComponent<Rigidbody>().isKinematic = false;
		toastRight.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
		toastLeft.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
		Vector3 dirToButter = ((!(target != null)) ? Vector3.up : (target.transform.position - Vector3.forward - transform.position).normalized);
		dirToButter.y = 0f;
		toastRight.GetComponent<Rigidbody>().AddForce((Vector3.up * Random.Range(verticalSpeedMin, verticalSpeedMax) * launchPower + dirToButter * lateralSpeed * launchPower) / 60f, ForceMode.Impulse);
		toastLeft.GetComponent<Rigidbody>().AddForce((Vector3.up * Random.Range(verticalSpeedMin, verticalSpeedMax) * launchPower + dirToButter * lateralSpeed * launchPower) / 60f, ForceMode.Impulse);
		toastRight.GetComponent<Rigidbody>().AddTorque(Random.onUnitSphere * 1000f * (1f / 60f / Time.fixedDeltaTime));
		toastLeft.GetComponent<Rigidbody>().AddTorque(Random.onUnitSphere * 1000f * (1f / 60f / Time.fixedDeltaTime));
		toastRight.GetComponent<ObjectToast>().toastState = toast;
		toastLeft.GetComponent<ObjectToast>().toastState = toast;
		KitchenToastManager.AddKitchenToast(toastLeft.GetComponent<ObjectToast>());
		KitchenToastManager.AddKitchenToast(toastRight.GetComponent<ObjectToast>());
		loadToasterTimer = 0f;
		while (loadToasterTimer < 1f)
		{
			if (loadToasterTimer > 0.25f)
			{
				toastRight.GetComponent<Collider>().enabled = true;
				toastLeft.GetComponent<Collider>().enabled = true;
				toastRight.GetComponent<ObjectToast>().enabled = true;
				toastLeft.GetComponent<ObjectToast>().enabled = true;
			}
			loadToasterTimer += Time.deltaTime;
			handle.transform.localPosition = Vector3.Lerp(handle.transform.localPosition, handleStart, loadToasterTimer);
			yield return null;
		}
		loadToasterTimer = 0f;
		toastRight.GetComponent<VVRConsumable>().unconsumable = false;
		toastLeft.GetComponent<VVRConsumable>().unconsumable = false;
		if (finalToast)
		{
			Deactivate();
		}
		else if (!tutorialToaster)
		{
			Reload(generateNewToast: true);
		}
	}

	public void Reload(bool generateNewToast = false)
	{
		if ((cookCoroutine == null || (cookCoroutine != null && !cookCoroutine.IsRunning)) && (loadCoroutine == null || (loadCoroutine != null && !loadCoroutine.IsRunning)))
		{
			startedToasting = false;
			loadCoroutine = GlobalCoroutineContainer.AddThread(ReloadToasterRoutine(generateNewToast), this);
		}
	}

	private IEnumerator ReloadToasterRoutine(bool generateNewToast = false)
	{
		yield return new WaitForSeconds(reloadTime);
		if (generateNewToast)
		{
			toastCopyTemplate.SetActive(value: true);
			toastLeft = Object.Instantiate(toastCopyTemplate);
			toastRight = Object.Instantiate(toastCopyTemplate);
			toastCopyTemplate.SetActive(value: false);
		}
		else
		{
			if (loaf.Count <= 1)
			{
				enabled = false;
				yield break;
			}
			toastLeft = loaf[0].gameObject;
			toastRight = loaf[1].gameObject;
			loaf.RemoveAt(0);
			loaf.RemoveAt(0);
		}
		toast = Toast.Uncooked;
		toastLeft.SetActive(value: true);
		toastRight.SetActive(value: true);
		toastLeft.GetComponent<MeshFilter>().sharedMesh = uncookedMesh;
		toastRight.GetComponent<MeshFilter>().sharedMesh = uncookedMesh;
		toastLeft.transform.parent = toastParent.transform;
		toastRight.transform.parent = toastParent.transform;
		toastLeft.GetComponent<Rigidbody>().isKinematic = true;
		toastRight.GetComponent<Rigidbody>().isKinematic = true;
		toastParent.transform.localPosition = toastStartParent;
		toastLeft.transform.localPosition = toastStartLeft;
		toastRight.transform.localPosition = toastStartRight;
		toastLeft.transform.localRotation = Quaternion.identity;
		toastRight.transform.localRotation = Quaternion.identity;
		toastRight.GetComponent<VVRConsumable>().unconsumable = true;
		toastLeft.GetComponent<VVRConsumable>().unconsumable = true;
		toastLeft.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
		toastRight.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
		toastLeft.transform.localScale = toastCopyTemplate.transform.localScale;
		toastRight.transform.localScale = toastCopyTemplate.transform.localScale;
		float loadingTimer = 0f;
		while (loadingTimer < 1f)
		{
			loadingTimer += Time.deltaTime * loadingSpeed;
			toastParent.transform.localPosition = Vector3.Lerp(Vector3.zero, toastStartParent, loadingTimer);
			yield return null;
		}
		loadingTimer = 0f;
		if (cookCoroutine != null && cookCoroutine.IsRunning)
		{
			cookCoroutine.Stop();
		}
		cookCoroutine = GlobalCoroutineContainer.AddThread(CookToast(), this);
	}

	public override void OnButtonPress()
	{
		base.OnButtonPress();
		EjectToast();
	}

	public void Deactivate()
	{
		if (cookCoroutine != null && cookCoroutine.IsRunning)
		{
			cookCoroutine.Stop();
			cookCoroutine = null;
		}
		if (loadCoroutine != null && loadCoroutine.IsRunning)
		{
			loadCoroutine.Stop();
			loadCoroutine = null;
		}
		if (popCoroutine != null && popCoroutine.IsRunning)
		{
			popCoroutine.Stop();
			popCoroutine = null;
		}
	}

	public void ActivateFireToaster()
	{
		toastRight.SetActive(value: true);
		toastLeft.SetActive(value: true);
		toast = Toast.Burning;
		handle.transform.localPosition = Vector3.zero;
		toastParent.transform.localPosition = Vector3.zero;
		finalToast = true;
		popCoroutine = GlobalCoroutineContainer.AddThread(PopOutToast(), this);
	}
}
