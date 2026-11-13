using UnityEngine;

public class ObjectStampButton : ObjectBase
{
	[SerializeField]
	private PuzzleBox puzzle;

	[SerializeField]
	private GvrAudioSource buttonSound;

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

	private void Update()
	{
		if (pressOnClick)
		{
			transform.localPosition = Vector3.Lerp(transform.localPosition, localStartPos, 5f * Time.deltaTime);
		}
	}

	public override void OnButtonPress()
	{
		if (puzzle != null && puzzle.IsFull())
		{
			puzzle.PrintHeadset();
			buttonSound.Play();
		}
		transform.localPosition = localStartPos + transform.localRotation * axis * pushDepth;
	}
}
