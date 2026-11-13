using UnityEngine;

public class RotateToPlayer : MonoBehaviour
{
	[Header("Rotate on Axis")]
	[SerializeField]
	private bool _x = true;

	[SerializeField]
	private bool _y = true;

	[SerializeField]
	private bool _z = true;

	private void Update()
	{
		transform.LookAt(EagerMaster.instance.player.transform);
		transform.rotation = Quaternion.Euler((!_x) ? 0f : transform.rotation.eulerAngles.x, (!_y) ? 0f : transform.rotation.eulerAngles.y, (!_z) ? 0f : transform.rotation.eulerAngles.z);
	}
}
