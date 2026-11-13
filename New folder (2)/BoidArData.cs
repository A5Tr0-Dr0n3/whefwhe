using UnityEngine;

public class BoidArData : MonoBehaviour
{
	[Header("References")]
	[SerializeField]
	private BoidController _boid;

	[SerializeField]
	private LineRenderer _line;

	[Header("Values")]
	[SerializeField]
	private float _targetYOffset;

	[SerializeField]
	private float _yOffsetSpeed;

	private float _currentYPos;

	private Vector3 _boidCenter;

	private float _boidCenterX;

	private float _boidCenterY;

	private float _boidCenterZ;

	private float _boidCenterCount;

	private LineRenderer[] _lines;

	private void Start()
	{
		_boidCenter = BoidCenter();
		_currentYPos = _boidCenter.y + _targetYOffset;
		transform.position = new Vector3(_boidCenter.x, _currentYPos, _boidCenter.z);
		_lines = new LineRenderer[_boid.boidList.Count];
		for (int i = 0; i < _lines.Length; i++)
		{
			_lines[i] = Object.Instantiate(_line);
			_lines[i].transform.parent = transform;
		}
	}

	private void Update()
	{
		_boidCenter = BoidCenter();
		_currentYPos = Mathf.Lerp(transform.position.y, _boidCenter.y + _targetYOffset, _yOffsetSpeed * Time.deltaTime);
		transform.position = new Vector3(_boidCenter.x, _currentYPos, _boidCenter.z);
		for (int i = 0; i < _lines.Length; i++)
		{
			if (i < _boid.boidList.Count - 1 && _boid.boidList[i] != null && _boid.boidList[i].currentBehavior != ObjectBoid.behaviorMode.roost)
			{
				_lines[i].gameObject.SetActive(value: true);
				_lines[i].SetPosition(0, Vector3.MoveTowards(transform.position, _boid.boidList[i].transform.position, 0.5f));
				_lines[i].SetPosition(2, Vector3.MoveTowards(transform.position, _boid.boidList[i].transform.position, 8f));
				_lines[i].SetPosition(1, new Vector3(_lines[i].GetPosition(2).x, _lines[i].GetPosition(0).y, _lines[i].GetPosition(2).z));
			}
			else
			{
				_lines[i].gameObject.SetActive(value: false);
			}
		}
	}

	private Vector3 BoidCenter()
	{
		_boidCenterX = (_boidCenterY = (_boidCenterZ = (_boidCenterCount = 0f)));
		for (int i = 0; i < _boid.boidList.Count; i++)
		{
			if (_boid.boidList[i].currentBehavior != ObjectBoid.behaviorMode.roost)
			{
				_boidCenterX += _boid.boidList[i].transform.position.x;
				_boidCenterY += _boid.boidList[i].transform.position.y;
				_boidCenterZ += _boid.boidList[i].transform.position.z;
				_boidCenterCount++;
			}
		}
		if (_boidCenterCount > 0f)
		{
			return new Vector3(_boidCenterX / _boidCenterCount, _boidCenterY / _boidCenterCount, _boidCenterZ / _boidCenterCount);
		}
		return Vector3.zero;
	}
}
