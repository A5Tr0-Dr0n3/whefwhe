public class SkipMergeObject : ObjectBase
{
	public override void OnHoverExit()
	{
		base.OnHoverExit();
	}

	public override void OnHoverEnter()
	{
		base.OnHoverEnter();
	}

	public void LoadSceneSelect()
	{
		ToolManager.ToggleManipulator(toggle: true);
		EagerHeadsetManager.instance.RemoveHeadset();
	}
}
