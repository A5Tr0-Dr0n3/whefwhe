using UnityEngine;

public class KillSwitch : MonoBehaviour
{
	public float lifeTime = 10f;

	private void Update()
	{
		lifeTime -= Time.deltaTime;
		if (lifeTime < 0f)
		{
			Object.Destroy(gameObject);
		}
	}
}
