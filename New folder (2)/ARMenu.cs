using UnityEngine;

public class ARMenu : MonoBehaviour
{
	[Header("Values")]
	public Color suffVacValColor;

	public Color insuffVacValColor;

	[SerializeField]
	private float _rotationSpeed;

	[Header("References")]
	[SerializeField]
	private ARButton[] _buttons;

	private float _iconYRotation;

	private float _timer;

	private Quaternion _iconRotation = Quaternion.identity;

	private void Start()
	{
		for (int i = 0; i < _buttons.Length; i++)
		{
			_buttons[i]._myMenu = this;
		}
	}

	private void OnEnable()
	{
		for (int i = 0; i < _buttons.Length; i++)
		{
			_buttons[i].state = ARButton.ButtonState.Menu;
		}
	}

	private void Update()
	{
		_timer += Time.deltaTime;
		_iconYRotation = Mathf.Repeat(_timer * _rotationSpeed, 360f);
		_iconRotation = new Quaternion(0f, _iconYRotation, 0f, 0f);
		ResetAllButtonColors();
	}

	public void ResetAllButtonColors()
	{
		for (int i = 0; i < _buttons.Length; i++)
		{
			_buttons[i].ResetTextColor();
		}
	}

	public Quaternion GetRotation()
	{
		return _iconRotation;
	}
}
