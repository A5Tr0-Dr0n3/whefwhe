using UnityEngine;

public class LivingOnAPrayer : MonoBehaviour
{
	[SerializeField]
	private Transform _anchor;

	private Transform _target;

	private Vector3 _halfwayThere;

	private void Start()
	{
		_target = EagerMaster.instance.mainCamera.transform;
	}

	private void Update()
	{
		_halfwayThere = (_target.position - _anchor.position) / 2f;
		transform.position = new Vector3(_halfwayThere.x, _anchor.position.y, _halfwayThere.z);
	}
}
