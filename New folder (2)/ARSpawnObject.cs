using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ARSpawnObject : MonoBehaviour
{
	[HideInInspector]
	public Collider mCollider;

	private void Start()
	{
		mCollider = GetComponent<Collider>();
	}
}
