using TMPro;
using UnityEngine;

[RequireComponent(typeof(ObjectButton))]
public class HubControlsButton : MonoBehaviour
{
	public HubControls.Submenu mySubmenu;

	[HideInInspector]
	public HubControls myMenu;

	public TextMeshPro text;

	public Color selected;

	public Color unselected;

	private ObjectButton button;

	private void Start()
	{
		button = GetComponent<ObjectButton>();
		button.onClickEvent.AddListener(() =>
		{
			myMenu.SubmenuSwitch(mySubmenu);
			Select();
		});
	}

	public void DeSelect()
	{
		text.color = unselected;
	}

	public void Select()
	{
		text.color = selected;
	}
}
