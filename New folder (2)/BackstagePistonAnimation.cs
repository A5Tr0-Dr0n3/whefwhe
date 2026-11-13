using UnityEngine;

public class BackstagePistonAnimation : MonoBehaviour
{
	private Transform myTransform;

	private Vector3 initialLocalPos;

	public float speed = 1f;

	public float height = 5f;

	private void Start()
	{
		myTransform = transform;
		initialLocalPos = myTransform.localPosition;
	}

	private void Update()
	{
		myTransform.localPosition = new Vector3(initialLocalPos.x, initialLocalPos.y + Mathf.Sin(Time.time * speed) * height, initialLocalPos.z);
	}
}
