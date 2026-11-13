using System.Collections;
using UnityEngine;

public class TinaTest : MonoBehaviour
{
	[SerializeField]
	private ObjectBase[] _objectsToGrab;

	private int[] _objectReachAttempts;

	[SerializeField]
	private Transform[] _dropLocations;

	[SerializeField]
	private TinaController _tinaController;

	[SerializeField]
	private Transform _tableTop;

	[SerializeField]
	private float _tableRadius;

	[SerializeField]
	private float _maxGrabRadius;

	private void Start()
	{
		_objectReachAttempts = new int[_objectsToGrab.Length];
		GlobalCoroutineContainer.AddThread(KeepawayRoutine(), this);
	}

	private IEnumerator DebugRoutine()
	{
		while (true)
		{
			yield return _tinaController.CloseClaw();
			yield return new WaitForSeconds(1f);
			yield return _tinaController.OpenClaw();
		}
	}

	private IEnumerator DoCoroutineSequence()
	{
		yield return new WaitForSeconds(3f);
		for (int i = 0; i < _objectsToGrab.Length; i++)
		{
			yield return _tinaController.Grab(_objectsToGrab[i]);
			yield return new WaitForSeconds(1f);
			yield return _tinaController.DropAt(_dropLocations[Random.Range(0, _dropLocations.Length)]);
			if (i == _objectsToGrab.Length - 1)
			{
				i = -1;
			}
		}
	}

	private IEnumerator KeepawayRoutine()
	{
		while (true)
		{
			int indexToGrab = -1;
			for (int i = 0; i < _objectsToGrab.Length; i++)
			{
				if (_objectsToGrab[i].isGrabbed)
				{
					indexToGrab = i;
					_objectReachAttempts[i] = 0;
					break;
				}
				Vector3 vector = _objectsToGrab[i].transform.position - _tableTop.position;
				if ((vector.y > 0.3f || new Vector2(vector.x, vector.z).sqrMagnitude > _tableRadius * _tableRadius) && vector.sqrMagnitude < _maxGrabRadius * _maxGrabRadius && _objectReachAttempts[i] < 3)
				{
					indexToGrab = i;
					break;
				}
			}
			if (indexToGrab > -1)
			{
				yield return _tinaController.Grab(_objectsToGrab[indexToGrab]);
				if (_tinaController.HeldObject != null)
				{
					_objectReachAttempts[indexToGrab] = 0;
					yield return _tinaController.DropAt(_dropLocations[Random.Range(0, _dropLocations.Length)]);
				}
				else
				{
					_objectReachAttempts[indexToGrab]++;
					yield return _tinaController.GoToRestPose();
				}
			}
			yield return null;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(_tableTop.position, _tableRadius);
		Gizmos.DrawWireSphere(_tableTop.position, _maxGrabRadius);
	}
}
