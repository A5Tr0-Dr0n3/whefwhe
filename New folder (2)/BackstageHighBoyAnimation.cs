using UnityEngine;

public class BackstageHighBoyAnimation : MonoBehaviour
{
	private Transform myTransform;

	private Vector3 initialLocalPos;

	public float speed = 1f;

	public float bounceSpeed = 1f;

	public float bounceOffset = 1f;

	public float bounceMagnitude = 1f;

	public bool zAxis = true;

	public float startPos = -200f;

	public float endPos = 200f;

	public AnimationCurve traversalCurve;

	public float t;

	private void Start()
	{
		myTransform = transform;
		initialLocalPos = myTransform.localPosition;
	}

	private void Update()
	{
		t += Time.deltaTime * speed;
		while (t > 1f)
		{
			t--;
		}
		if (zAxis)
		{
			myTransform.localPosition = new Vector3(initialLocalPos.x, initialLocalPos.y + Mathf.Sin(Time.time * bounceSpeed + t * bounceOffset) * bounceMagnitude, startPos + traversalCurve.Evaluate(t) * (endPos - startPos));
		}
		else
		{
			myTransform.localPosition = new Vector3(startPos + traversalCurve.Evaluate(t) * (endPos - startPos), initialLocalPos.y + Mathf.Sin(Time.time * bounceSpeed + t * bounceOffset) * bounceMagnitude, initialLocalPos.z);
		}
	}
}
