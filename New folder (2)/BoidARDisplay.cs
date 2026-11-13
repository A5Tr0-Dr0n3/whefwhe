using UnityEngine;

public class BoidARDisplay : MonoBehaviour
{
	private LineRenderer _myLine;

	private ObjectBoid _myBoid;

	private Vector2 dottedScale = Vector2.one;

	private Vector2 dotOffset = Vector2.zero;

	private float offsetRate;

	private float scalingValue = 5f;

	private bool _arEnabled = true;

	public GameObject targetIndicator;

	private GameObject indicatorInstance;

	public bool arEnabled => _arEnabled;

	private void Start()
	{
		_myLine = GetComponent<LineRenderer>();
		_myBoid = GetComponent<ObjectBoid>();
		ToggleAR(newToggle: false);
	}

	private void Update()
	{
		if (_arEnabled)
		{
			drawTargetLine();
		}
	}

	private void drawTargetLine()
	{
		if (_myBoid.currentBehavior != ObjectBoid.behaviorMode.ambient && _myBoid.currentAction != ObjectBoid.actionMode.avoidingObstacle && _myBoid.currentAction != ObjectBoid.actionMode.docking && _myBoid.currentAction != ObjectBoid.actionMode.held && _myBoid.currentAction != ObjectBoid.actionMode.sitting && _myBoid.currentAction != ObjectBoid.actionMode.hugging)
		{
			if (!_myLine.enabled)
			{
				if (indicatorInstance == null)
				{
					spawnIndicator();
				}
				indicatorInstance.SetActive(value: true);
				_myLine.enabled = true;
			}
			if (_myBoid.currentBehavior == ObjectBoid.behaviorMode.roost && (bool)_myBoid.targetRoostTransform)
			{
				_myLine.SetPosition(0, _myBoid.transform.position);
				_myLine.SetPosition(1, _myBoid.targetRoostTransform.position);
				setDottedLineScale();
			}
			else if ((_myBoid.currentBehavior == ObjectBoid.behaviorMode.search || _myBoid.currentBehavior == ObjectBoid.behaviorMode.searchAndDestroy || _myBoid.currentBehavior == ObjectBoid.behaviorMode.destroy || _myBoid.currentBehavior == ObjectBoid.behaviorMode.construct || _myBoid.currentBehavior == ObjectBoid.behaviorMode.deconstruct) && (bool)_myBoid.targetConsumable)
			{
				_myLine.SetPosition(0, _myBoid.transform.position);
				_myLine.SetPosition(1, _myBoid.targetConsumable.transform.position);
				setDottedLineScale();
			}
			else if (_myBoid.currentBehavior == ObjectBoid.behaviorMode.chase)
			{
				_myLine.SetPosition(0, _myBoid.transform.position);
				_myLine.SetPosition(1, EagerMaster.instance.player.transform.position);
				setDottedLineScale();
			}
			offsetPath();
		}
		else if (_myLine.enabled)
		{
			if ((bool)indicatorInstance)
			{
				indicatorInstance.SetActive(value: false);
			}
			_myLine.enabled = false;
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

	private void setDottedLineScale()
	{
		dottedScale.y = 1f;
		dottedScale.x = Vector3.Distance(_myLine.GetPosition(0), _myLine.GetPosition(1)) * scalingValue;
		_myLine.materials[0].mainTextureScale = dottedScale;
		if (!indicatorInstance)
		{
			spawnIndicator();
		}
		indicatorInstance.transform.position = _myLine.GetPosition(1);
	}

	private void spawnIndicator()
	{
		indicatorInstance = Object.Instantiate(targetIndicator);
		indicatorInstance.transform.parent = transform.parent;
		indicatorInstance.SetActive(value: true);
	}

	public void ToggleAR(bool newToggle)
	{
		_arEnabled = newToggle;
		if (!_arEnabled)
		{
			_myLine.enabled = false;
			if ((bool)indicatorInstance)
			{
				indicatorInstance.SetActive(value: false);
			}
		}
	}
}
