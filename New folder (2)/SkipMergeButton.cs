using UnityEngine;

public class SkipMergeButton : ObjectButton
{
	public GameObject onButton;

	public GameObject offButton;

	public override void OnHoverExit()
	{
		base.OnHoverExit();
		onButton.SetActive(value: false);
		offButton.SetActive(value: true);
	}

	public override void OnHoverEnter()
	{
		base.OnHoverEnter();
		onButton.SetActive(value: true);
		offButton.SetActive(value: false);
	}

	public void LoadSceneSelect()
	{
		ToolManager.ToggleManipulator(toggle: true);
		EagerHeadsetManager.instance.RemoveHeadset();
		CharacterFlip.SetCanFlip(newValue: true);
	}
}
