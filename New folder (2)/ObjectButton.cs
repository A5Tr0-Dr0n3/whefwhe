using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ObjectButton : ObjectBase
{
	[SerializeField]
	public UnityEvent onClickEvent = new UnityEvent();

	[SerializeField]
	public Manipulator.ButtonPressSFXType buttonSFX;

	[HideInInspector]
	public bool hasBeenPressed;

	[SerializeField]
	private Vector3 axis = new Vector3(0f, 0f, 1f);

	[SerializeField]
	private float pushDepth;

	[SerializeField]
	private bool pressOnClick;

	private Vector3 localStartPos;

	protected override void Start()
	{
		base.Start();
		localStartPos = transform.localPosition;
	}

	protected override void Awake()
	{
		base.Awake();
		isButton = true;
	}

	private void Update()
	{
		if (pressOnClick)
		{
			transform.localPosition = Vector3.Lerp(transform.localPosition, localStartPos, 5f * Time.deltaTime);
		}
	}

	public override void OnButtonPress()
	{
		if (!ungrabbable && !hasBeenPressed)
		{
			base.OnButtonPress();
			hasBeenPressed = true;
			StartCoroutine(ToggleButton());
			if (onClickEvent != null)
			{
				onClickEvent.Invoke();
			}
			transform.localPosition = localStartPos + axis * pushDepth;
		}
	}

	protected IEnumerator ToggleButton()
	{
		yield return null;
		hasBeenPressed = false;
	}
}
