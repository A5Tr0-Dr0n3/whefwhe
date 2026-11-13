using UnityEngine;

public class RadioHallway : MonoBehaviour
{
	public float speed = 1f;

	private Transform t;

	private void Start()
	{
		t = transform;
	}

	private void Update()
	{
		t.position -= new Vector3(0f, 0f, 1f * Time.deltaTime * speed * 2f);
		t.localScale -= new Vector3(0f, 0f, 1f * Time.deltaTime * speed * 0.5f);
		if (t.localScale.z < 2f)
		{
			t.localScale = new Vector3(t.localScale.x, t.localScale.y, 6f);
			t.position = new Vector3(t.position.x, t.position.y, 3f);
		}
	}
}
