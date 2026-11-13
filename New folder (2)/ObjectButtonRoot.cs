using System.Collections;
using TButt;

public class ObjectButtonRoot : ObjectButton
{
	public override void OnHoverStay(Manipulator hoverer)
	{
		base.OnHoverStay(hoverer);
		if (TBCore.GetActivePlatform() == VRPlatform.OculusMobile && TBInput.GetButtonDown(TBInput.Button.Action2, hoverer.toolGroup.Controller))
		{
			hoverer.Button(this);
		}
	}

	public override void OnButtonPress()
	{
		if (!ungrabbable && !hasBeenPressed)
		{
			hasBeenPressed = true;
			GlobalCoroutineContainer.AddGlobalThread(ToggleRootButton());
			if (onClickEvent != null)
			{
				onClickEvent.Invoke();
			}
		}
	}

	protected IEnumerator ToggleRootButton()
	{
		yield return null;
		yield return null;
		hasBeenPressed = false;
	}
}
