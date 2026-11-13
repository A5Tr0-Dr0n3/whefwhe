using UnityEngine;

public class HeasetRemovalIconStretch : MonoBehaviour
{
	private HeadsetRemovalUIManager manager;

	public int index;

	[Range(-10f, 10f)]
	public float stretch;

	[Range(-5f, 5f)]
	public float curve;

	private void Start()
	{
		manager = GetComponentInParent<HeadsetRemovalUIManager>();
	}

	private void Update()
	{
		float value = manager.value;
		transform.localPosition = new Vector3(0f, 0f, 0f);
		transform.localPosition += Vector3.forward * value * stretch * index + Vector3.up * value * curve * index;
	}
}
