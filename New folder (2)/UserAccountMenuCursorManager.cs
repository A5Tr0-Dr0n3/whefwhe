using UnityEngine;

public class UserAccountMenuCursorManager : MonoBehaviour
{
	[SerializeField]
	private GameObject rootMouseCursor;

	[SerializeField]
	private Transform scalingLayer;

	[SerializeField]
	private GameObject rootMousePointer;

	[SerializeField]
	private GameObject rootMouseHand;

	private bool activated;

	private ToolGroup _toolGroup;

	public void Initialize(float grabDistance, ToolGroup toolGroup)
	{
		_toolGroup = toolGroup;
		rootMouseCursor.SetActive(value: true);
		ToolManager.AllCursorReticles.HideCursor = true;
		ToolManager.AllManipulators.MaxGrabLength = grabDistance;
		activated = true;
		gameObject.SetActive(value: true);
	}

	private void Update()
	{
		if (!activated)
		{
			return;
		}
		rootMouseCursor.transform.position = _toolGroup.CursorReticle.CursorTransform.position;
		rootMouseCursor.transform.rotation = EagerMaster.instance.mainCamera.transform.rotation;
		rootMouseCursor.transform.localEulerAngles = new Vector3(rootMouseCursor.transform.localEulerAngles.x, rootMouseCursor.transform.localEulerAngles.y, 0f);
		scalingLayer.localScale = Vector3.one * _toolGroup.CursorReticle.scaleConstant;
		if ((bool)_toolGroup.Manipulator.HeldObjectBase)
		{
			ToolManager.AllCursorReticles.HideCursor = false;
			rootMouseHand.SetActive(value: false);
			rootMousePointer.SetActive(value: false);
			return;
		}
		ToolManager.AllCursorReticles.HideCursor = true;
		if (_toolGroup.Manipulator.currentGrabbableStatus == Manipulator.grabbableStatus.button || _toolGroup.Manipulator.currentGrabbableStatus == Manipulator.grabbableStatus.grabbable)
		{
			rootMouseHand.SetActive(value: true);
			rootMousePointer.SetActive(value: false);
		}
		else
		{
			rootMouseHand.SetActive(value: false);
			rootMousePointer.SetActive(value: true);
		}
	}
}
