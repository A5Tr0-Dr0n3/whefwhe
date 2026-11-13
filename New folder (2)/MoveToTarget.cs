using UnityEngine;

public class MoveToTarget : MonoBehaviour
{
	public Vector3 targetPosition;

	public float lerpSpeed;

	private bool animating = true;

	private Transform t;

	private Vector3 toTarget;

	private void Start()
	{
		t = transform;
	}

	private void Update()
	{
		if (animating)
		{
			toTarget = targetPosition - t.position;
			if (toTarget.sqrMagnitude < 0.4f)
			{
				animating = false;
			}
			else
			{
				t.position = Vector3.Lerp(t.position, targetPosition, lerpSpeed * Time.deltaTime);
			}
		}
	}
}
