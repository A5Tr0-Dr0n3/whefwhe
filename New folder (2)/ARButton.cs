using TMPro;
using UnityEngine;

public class ARButton : MonoBehaviour
{
	public enum ButtonState
	{
		Menu,
		Spawn
	}

	[Header("References")]
	[SerializeField]
	private ARSpawnObject _objIcon;

	[SerializeField]
	private VVRConsumable _toSpawnConsumable;

	[SerializeField]
	private TextMeshPro _costTextMesh;

	[Header("Values")]
	public float spawnScale = 1f;

	public float spawnCost;

	[HideInInspector]
	public ARMenu _myMenu;

	private ButtonState _state;

	public ButtonState state
	{
		get
		{
			return _state;
		}
		set
		{
		}
	}

	public ARSpawnObject objIcon => _objIcon;

	public VVRConsumable toSpawnConsumable => _toSpawnConsumable;

	private void Start()
	{
		_costTextMesh.text = spawnCost.ToString("0.0") + " TB";
	}

	private void Update()
	{
		if (_costTextMesh.gameObject.activeInHierarchy && !(spawnCost <= EagerMaster.instance.vacuumFill))
		{
		}
	}

	private void ArButtonPress()
	{
		if (!(spawnCost > EagerMaster.instance.vacuumFill))
		{
		}
	}

	private void OnEnable()
	{
		if (EagerMaster.instance != null && (!(spawnCost > EagerMaster.instance.vacuumFill) || !(_toSpawnConsumable != null)) && _toSpawnConsumable != null)
		{
			_costTextMesh.color = _myMenu.suffVacValColor;
		}
	}

	public void PositionSpawnIcon(Vector3 position)
	{
		_objIcon.transform.position = position;
	}

	public void ResetTextColor()
	{
		if (spawnCost <= EagerMaster.instance.vacuumFill)
		{
			_costTextMesh.color = _myMenu.suffVacValColor;
		}
		else
		{
			_costTextMesh.color = _myMenu.insuffVacValColor;
		}
	}
}
