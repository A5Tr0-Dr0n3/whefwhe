using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjectData : MonoBehaviour
{
	private VVRConsumable dataBase;

	public float addTime = 2f;

	public float dataValue = 10f;

	private bool safetyStop;

	private void Awake()
	{
	}

	private void Start()
	{
		GetComponent<Rigidbody>().AddForce(transform.forward * -2f, ForceMode.Impulse);
		EagerMaster.instance.vacuumFill -= dataValue;
	}

	private void OnTriggerEnter(Collider col)
	{
		dataBase = col.GetComponentInParent<VVRConsumable>();
		if (dataBase != null && !safetyStop)
		{
			Object.Destroy(gameObject);
			safetyStop = true;
		}
	}
}
