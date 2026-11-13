using UnityEngine;

public class Reflector : MonoBehaviour
{
	public Reflector reflectObject;

	public GameObject reflectionPlane;

	private bool reflector = true;

	[HideInInspector]
	public Rigidbody rb;

	[HideInInspector]
	public Collider[] childColliders;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		childColliders = GetComponentsInChildren<Collider>();
	}

	private void Update()
	{
		if (reflector)
		{
			reflectObject.transform.localPosition = transform.localPosition;
			reflectObject.transform.localRotation = transform.localRotation;
		}
	}

	private void SetReflector(bool refl)
	{
		if (refl)
		{
			reflector = true;
			rb.isKinematic = false;
			reflectObject.rb.isKinematic = true;
		}
	}
}
