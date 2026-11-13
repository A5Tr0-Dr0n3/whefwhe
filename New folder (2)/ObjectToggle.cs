using UnityEngine;

[RequireComponent(typeof(ObjectButton))]
public class ObjectToggle : MonoBehaviour
{
	public delegate void ToggleEventHandler(ObjectToggle toggledObject, bool newValue, bool playerTriggered);

	[SerializeField]
	private MeshFilter onMesh;

	[SerializeField]
	private MeshFilter offMesh;

	private ObjectButton mButton;

	private MeshFilter mFilter;

	public bool toggleState { get; private set; }

	public event ToggleEventHandler onToggled;

	protected virtual void Awake()
	{
		toggleState = false;
		mFilter = GetComponent<MeshFilter>();
		updateMat();
		mButton = GetComponent<ObjectButton>();
		mButton.onClickEvent.AddListener(PlayerToggle);
	}

	public void Toggle()
	{
		Toggle(!toggleState);
	}

	public void PlayerToggle()
	{
		Toggle(!toggleState, playerToggled: true);
	}

	public void Toggle(bool newState, bool playerToggled = false)
	{
		toggleState = newState;
		if (onToggled != null)
		{
			onToggled(this, toggleState, playerToggled);
		}
		updateMat();
	}

	private void updateMat()
	{
		if (mFilter != null)
		{
			mFilter.sharedMesh = ((!toggleState) ? offMesh.sharedMesh : onMesh.sharedMesh);
		}
	}
}
