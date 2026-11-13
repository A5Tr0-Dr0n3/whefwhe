using UnityEngine;

public class ConveyorLink : MonoBehaviour
{
	public Rigidbody rb;

	public bool broken;

	[Range(0f, 1f)]
	public float percent;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}
}
