using UnityEngine;

public class PooledSmashObject : MonoBehaviour
{
	public Rigidbody rb;

	public Transform bottle;

	public bool active;

	public PooledObjectScone scone;

	public void Disable()
	{
		active = false;
		bottle.localPosition = Vector3.zero;
		bottle.gameObject.SetActive(value: false);
	}

	public void Enable()
	{
		rb.velocity = Vector3.zero;
		active = true;
		bottle.gameObject.SetActive(value: true);
	}

	public void SetBottle(GameObject obj)
	{
		bottle = obj.transform;
	}
}
