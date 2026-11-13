using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class DollDoor : MonoBehaviour
{
	[Header("Audio")]
	[SerializeField]
	private GvrAudioSource mAudio;

	[Header("Doll")]
	[SerializeField]
	private DollhouseDoll mDoll;

	[SerializeField]
	private Transform dollDown;

	[SerializeField]
	private Transform dollUp;

	[Header("Platform")]
	[SerializeField]
	private Transform mPlatform;

	[SerializeField]
	private Transform platformUp;

	[SerializeField]
	private Transform platformDown;

	[Header("Top Door")]
	[SerializeField]
	private Transform mTopDoor;

	[SerializeField]
	private Transform topDoorOpen;

	[SerializeField]
	private Transform topDoorClosed;

	[Header("Bottom Door")]
	[SerializeField]
	private Transform mBottomDoor;

	[SerializeField]
	private Transform bottomDoorOpen;

	[SerializeField]
	private Transform bottomDoorClosed;

	[Header("Tina's Dog")]
	[SerializeField]
	private DollhouseDoll dawkens;

	private bool isDown = true;

	private void Start()
	{
		mDoll.enabled = false;
		mDoll.rb.isKinematic = true;
		mDoll.ungrabbable = true;
		platformDown.position = mPlatform.transform.position;
	}

	private void Update()
	{
	}

	public SafeCoroutine RaiseDoll(float duration)
	{
		return GlobalCoroutineContainer.AddThread(RaiseDollCoroutine(duration), this);
	}

	private IEnumerator RaiseDollCoroutine(float duration)
	{
		if (isDown)
		{
			mAudio.Play();
			Vector3 topDoorStartPos = topDoorClosed.position;
			Vector3 topDoorEndPos = topDoorOpen.position;
			Vector3 bottomDoorStartPos = bottomDoorClosed.position;
			Vector3 bottomDoorEndPos = bottomDoorOpen.position;
			float doorTimer = 0f;
			while (doorTimer <= 1f)
			{
				doorTimer += Time.deltaTime * 3f;
				mTopDoor.position = Vector3.Lerp(topDoorStartPos, topDoorEndPos, doorTimer);
				mBottomDoor.position = Vector3.Lerp(bottomDoorStartPos, bottomDoorEndPos, doorTimer);
				yield return null;
			}
			Vector3 platformStartPos = platformDown.position;
			Vector3 platformEndPos = platformUp.position;
			Vector3 dollStartPos = dollDown.position;
			Vector3 dollEndPos = dollUp.position;
			float platformDollTimer = 0f;
			while (platformDollTimer <= 1f)
			{
				platformDollTimer += Time.deltaTime / duration;
				mPlatform.position = Vector3.Lerp(platformStartPos, platformEndPos, platformDollTimer);
				mDoll.transform.position = Vector3.Lerp(dollStartPos, dollEndPos, platformDollTimer);
				yield return null;
			}
			mAudio.Stop();
			mDoll.enabled = true;
			mDoll.SetState(DollhouseObjectState.inactive);
			mDoll.ungrabbable = false;
			isDown = false;
			yield return null;
		}
	}

	public SafeCoroutine LowerDoll(float duration, bool lowerDoll = true)
	{
		return GlobalCoroutineContainer.AddThread(LowerDollCoroutine(duration, lowerDoll), this);
	}

	private IEnumerator LowerDollCoroutine(float duration, bool lowerDoll)
	{
		if (isDown)
		{
			yield break;
		}
		if (lowerDoll)
		{
			mDoll.ungrabbable = true;
			mDoll.rb.isKinematic = true;
			mDoll.SetCollidersEnabled(enabled: false);
		}
		mAudio.Play();
		Vector3 platformStartPos = platformUp.position;
		Vector3 platformEndPos = platformDown.position;
		Vector3 dollStartPos = mDoll.transform.position;
		Vector3 dollEndPos = dollDown.position;
		float platformDollTimer = 0f;
		while (platformDollTimer <= 1f)
		{
			platformDollTimer += Time.deltaTime / duration;
			mPlatform.position = Vector3.Lerp(platformStartPos, platformEndPos, platformDollTimer);
			if (lowerDoll)
			{
				mDoll.transform.position = Vector3.Lerp(dollStartPos, dollEndPos, platformDollTimer);
			}
			yield return null;
		}
		Vector3 topDoorStartPos = topDoorOpen.position;
		Vector3 topDoorEndPos = topDoorClosed.position;
		Vector3 bottomDoorStartPos = bottomDoorOpen.position;
		Vector3 bottomDoorEndPos = bottomDoorClosed.position;
		float doorTimer = 0f;
		while (doorTimer <= 1f)
		{
			doorTimer += Time.deltaTime * 3f;
			mTopDoor.position = Vector3.Lerp(topDoorStartPos, topDoorEndPos, doorTimer);
			mBottomDoor.position = Vector3.Lerp(bottomDoorStartPos, bottomDoorEndPos, doorTimer);
			yield return null;
		}
		if (lowerDoll)
		{
			mDoll.gameObject.SetActive(value: false);
		}
		mAudio.Stop();
		isDown = true;
		yield return null;
	}

	public DollhouseDoll GetDoll()
	{
		return mDoll;
	}

	public SafeCoroutine RaiseDawkens(float duration, out DollhouseDoll d)
	{
		dawkens.gameObject.SetActive(value: true);
		dawkens.transform.position = dollDown.position;
		d = dawkens;
		mDoll = dawkens;
		return GlobalCoroutineContainer.AddThread(RaiseDollCoroutine(duration), this);
	}
}
