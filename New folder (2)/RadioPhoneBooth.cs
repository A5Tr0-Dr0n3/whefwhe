using System.Collections;
using UnityEngine;

public class RadioPhoneBooth : SceneBehaviorManager
{
	public ObjectBase radioHeadset;

	public ObjectBase phone;

	public GvrAudioSource phoneRingSound;

	public GvrAudioSource phoneSounds;

	public AudioClip phonePickUp;

	public AudioClip phoneDialTone;

	public AudioClip phonePutDown;

	public Material phoneMat;

	public Material headsetMat;

	public GameObject headset;

	public GameObject headsetEye;

	public GameObject phoneGO;

	public Headset headsetScript;

	private Vector3 phoneStartScale;

	private Vector3 headsetStartScale;

	private float flubberLerp;

	private float flubberLerpCount;

	private float flubberLerpMax = 5f;

	public Vector4 flubberMaxIntensity;

	public Vector3 flubberMaxIntensityV3;

	private void Start()
	{
		phoneRingSound.Play();
		headsetStartScale = headset.transform.localScale;
		phoneStartScale = phoneGO.transform.localScale;
		headset.SetActive(value: false);
		headsetScript.isActivated = false;
	}

	private IEnumerator PhoneBoothSequence()
	{
		while (!ToolManager.PlayerGrabbingObjectBase(phone))
		{
			yield return null;
		}
		phone.undroppable = true;
		SafeDebug.Log("Grabbed phone");
		phoneRingSound.Stop();
		phoneSounds.Play();
		headset.SetActive(value: true);
		while (flubberLerp < 1f)
		{
			flubberLerpCount += Time.deltaTime;
			flubberLerp = flubberLerpCount / flubberLerpMax;
			if (flubberLerp > 1f)
			{
				flubberLerp = 1f;
			}
			flubberMaxIntensity = Vector4.one * 0.01f;
			headsetMat.SetVector("_Distortion", Vector4.Lerp(flubberMaxIntensity, Vector4.zero, flubberLerp));
			phoneMat.SetVector("_Distortion", Vector4.Lerp(Vector4.zero, flubberMaxIntensity, flubberLerp));
			headset.transform.localScale = Vector3.Lerp(Vector3.zero, headsetStartScale, flubberLerp);
			phoneGO.transform.localScale = Vector3.Lerp(phoneStartScale, Vector3.zero, flubberLerp);
			yield return null;
		}
		headsetScript.isActivated = true;
		while (phoneSounds.isPlaying)
		{
			yield return null;
		}
		yield return new WaitForSeconds(5f);
		phoneSounds.clip = phonePutDown;
		phoneSounds.loop = false;
		phoneSounds.Play();
		phone.ForceDropByPlayer();
	}
}
