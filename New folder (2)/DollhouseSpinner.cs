using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class DollhouseSpinner : MonoBehaviour
{
	public float spinSpeed = 0.5f;

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			FlipTable();
		}
	}

	public SafeCoroutine FlipTable()
	{
		return GlobalCoroutineContainer.AddThread(FlipTableCoroutine(), this);
	}

	private IEnumerator FlipTableCoroutine()
	{
		float timer = 0f;
		Quaternion start = transform.rotation;
		Quaternion end = start * Quaternion.AngleAxis(180f, Vector3.up);
		while (timer <= 1f)
		{
			timer += Time.deltaTime / spinSpeed;
			transform.rotation = Quaternion.Slerp(start, end, timer);
			yield return null;
		}
		yield return null;
	}
}
