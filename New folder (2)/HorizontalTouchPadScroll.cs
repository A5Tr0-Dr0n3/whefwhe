using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class HorizontalTouchPadScroll : MonoBehaviour
{
	private Vector3 startLocalPos;

	[SerializeField]
	private Vector3 endLocalPos;

	private void Awake()
	{
		startLocalPos = transform.localPosition;
	}

	private void OnEnable()
	{
		transform.localPosition = startLocalPos;
	}

	public SafeCoroutine StartMove(float time)
	{
		return GlobalCoroutineContainer.AddThread(startMoveCoroutine(time), this);
	}

	private IEnumerator startMoveCoroutine(float time)
	{
		float timer = 0f;
		while (timer < 1f)
		{
			timer += Time.deltaTime / time;
			transform.localPosition = Vector3.Lerp(startLocalPos, endLocalPos, timer);
			yield return null;
		}
	}
}
