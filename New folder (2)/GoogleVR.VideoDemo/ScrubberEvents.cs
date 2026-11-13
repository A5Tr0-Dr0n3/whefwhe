using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GoogleVR.VideoDemo;

public class ScrubberEvents : MonoBehaviour
{
	private GameObject newPositionHandle;

	private Vector3[] corners;

	private Slider slider;

	private VideoControlsManager mgr;

	public VideoControlsManager ControlManager
	{
		set
		{
			mgr = value;
		}
	}

	private void Start()
	{
		Image[] componentsInChildren = GetComponentsInChildren<Image>(includeInactive: true);
		foreach (Image image in componentsInChildren)
		{
			if (image.gameObject.name == "newPositionHandle")
			{
				newPositionHandle = image.gameObject;
				break;
			}
		}
		corners = new Vector3[4];
		GetComponent<Image>().rectTransform.GetWorldCorners(corners);
		slider = GetComponentInParent<Slider>();
	}

	private void Update()
	{
		bool flag = false;
		if (GvrPointerInputModule.Pointer != null)
		{
			RaycastResult currentRaycastResult = GvrPointerInputModule.Pointer.CurrentRaycastResult;
			if (currentRaycastResult.gameObject != null)
			{
				newPositionHandle.transform.position = new Vector3(currentRaycastResult.worldPosition.x, newPositionHandle.transform.position.y, newPositionHandle.transform.position.z);
				flag = true;
			}
		}
		if (!flag)
		{
			newPositionHandle.transform.position = slider.handleRect.transform.position;
		}
	}

	public void OnPointerEnter(BaseEventData data)
	{
		if (GvrPointerInputModule.Pointer != null)
		{
			RaycastResult currentRaycastResult = GvrPointerInputModule.Pointer.CurrentRaycastResult;
			if (currentRaycastResult.gameObject != null)
			{
				newPositionHandle.transform.position = new Vector3(currentRaycastResult.worldPosition.x, newPositionHandle.transform.position.y, newPositionHandle.transform.position.z);
			}
		}
		newPositionHandle.SetActive(value: true);
	}

	public void OnPointerExit(BaseEventData data)
	{
		newPositionHandle.SetActive(value: false);
	}

	public void OnPointerClick(BaseEventData data)
	{
		float x = corners[0].x;
		float x2 = corners[3].x;
		float num = (newPositionHandle.transform.position.x - x) / (x2 - x);
		if (mgr != null)
		{
			long currentPosition = (long)(slider.maxValue * num);
			mgr.Player.CurrentPosition = currentPosition;
		}
	}
}
