using UnityEngine;

public class TouchPadScroll : MonoBehaviour
{
	public Vector3 startPos;

	public float scaleFactor = 1f;

	private void Awake()
	{
		startPos = transform.localPosition;
	}

	private void Update()
	{
		transform.localPosition = startPos + Vector3.forward * Mathf.Sin(Time.time * 1.5f) * 0.02f * scaleFactor;
	}
}
