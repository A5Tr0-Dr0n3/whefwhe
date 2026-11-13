using UnityEngine;

public class BoidSwarmARDisplay : MonoBehaviour
{
	private TransformFollow _myFollow;

	private LineRenderer _myLine;

	private Vector2 dottedScale = Vector2.one;

	private Vector2 dotOffset = Vector2.zero;

	private float offsetRate = 10f;

	private float scalingValue = 6f;

	private bool _arEnabled = true;

	private GameObject[] swarmPoints;

	public GameObject swarmPtReference;

	private ARSwarmLineRenderer _swarmLineRenderer;

	public bool arEnabled => _arEnabled;

	private void Start()
	{
		_myLine = GetComponent<LineRenderer>();
		_myFollow = GetComponent<TransformFollow>();
		_swarmLineRenderer = GetComponent<ARSwarmLineRenderer>();
		_myLine.positionCount = _myFollow.nodes.Length + 2;
		_myLine.SetPositions(_myFollow.nodes);
		_myLine.SetPosition(_myLine.positionCount - 2, _myFollow.nodes[0]);
		_myLine.SetPosition(_myLine.positionCount - 1, _myFollow.nodes[1]);
		swarmPoints = new GameObject[_myLine.positionCount];
		drawPaths();
		ToggleAR(newToggle: false);
	}

	private void Update()
	{
		if (_arEnabled)
		{
			offsetPath();
		}
	}

	private void offsetPath()
	{
		dotOffset.x += offsetRate * Time.deltaTime;
		if (Mathf.Abs(dotOffset.x) > 1000f)
		{
			dotOffset.x = 0f;
		}
		_myLine.materials[0].mainTextureOffset = dotOffset;
	}

	private void drawPaths()
	{
		dottedScale = Vector2.one;
		dottedScale.x = 0f;
		for (int i = 0; i < swarmPoints.Length; i++)
		{
			swarmPoints[i] = Object.Instantiate(swarmPtReference, transform);
			swarmPoints[i].transform.position = _myLine.GetPosition(i);
			swarmPoints[i].SetActive(value: true);
		}
		_swarmLineRenderer.CreateLine(_myLine);
		for (int j = 1; j < _myFollow.nodes.Length; j++)
		{
			dottedScale.x += Vector3.Distance(_myFollow.nodes[j], _myFollow.nodes[j - 1]);
			if (j == _myFollow.nodes.Length - 1)
			{
				dottedScale.x += Vector3.Distance(_myFollow.nodes[j], _myFollow.nodes[0]);
			}
		}
		dottedScale.x *= scalingValue;
		_myLine.materials[0].mainTextureScale = dottedScale;
	}

	public void ToggleAR(bool newToggle)
	{
		_arEnabled = newToggle;
		if (!_arEnabled)
		{
			_myLine.enabled = false;
		}
		else
		{
			_myLine.enabled = true;
		}
	}
}
