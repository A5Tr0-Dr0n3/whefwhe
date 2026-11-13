using TMPro;
using UnityEngine;

public class ReviewTile : MonoBehaviour
{
	public delegate void ButtonClickedHandler(int index);

	[SerializeField]
	public Transform aiMesh;

	private ObjectButton mButton;

	private int tileIndex;

	private Transform questionMark;

	private bool initialized;

	private event ButtonClickedHandler onButtonClickedEvent;

	private void Awake()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (!initialized)
		{
			mButton = GetComponent<ObjectButton>();
			mButton.onClickEvent.AddListener(HandleOnClicked);
			questionMark = GetComponentInChildren<TextMeshPro>().transform;
			initialized = true;
		}
	}

	public void SetUnlocked(bool unlocked)
	{
		if (unlocked)
		{
			questionMark.gameObject.SetActive(value: true);
			aiMesh.gameObject.SetActive(value: false);
		}
		else
		{
			questionMark.gameObject.SetActive(value: false);
			aiMesh.gameObject.SetActive(value: true);
		}
	}

	public void HookUpToReviewSystem(int index, ButtonClickedHandler buttonClickEventIn)
	{
		Initialize();
		tileIndex = index;
		onButtonClickedEvent = buttonClickEventIn;
	}

	private void Update()
	{
		aiMesh.Rotate(Vector3.up, 50f * Time.deltaTime);
	}

	public void HandleOnClicked()
	{
		if (onButtonClickedEvent != null)
		{
			onButtonClickedEvent(tileIndex);
		}
	}
}
