using UnityEngine;

public class LogoSphereAnimate : MonoBehaviour
{
	[CurveRange(0f, 1f, -1f, 1f)]
	public AnimationCurve bounce;

	public float wait;

	public float duration;

	public float offset;

	public float scalar;

	private float t;

	public Transform[] sphere;

	private Vector3[] initPos;

	private void Start()
	{
		initPos = new Vector3[sphere.Length];
		for (int i = 0; i < sphere.Length; i++)
		{
			ref Vector3 reference = ref initPos[i];
			reference = sphere[i].position;
		}
		t -= offset;
	}

	private void Update()
	{
		t += Time.deltaTime;
		if (t >= wait)
		{
			for (int i = 0; i < sphere.Length; i++)
			{
				if (t - wait >= (float)i * offset)
				{
					sphere[i].position = new Vector3(sphere[i].position.x, initPos[i].y + bounce.Evaluate((t - wait - (float)i * offset) / duration) * scalar, sphere[i].position.z);
				}
			}
		}
		if (t >= wait + duration + (float)sphere.Length * offset)
		{
			t = 0f;
		}
	}
}
