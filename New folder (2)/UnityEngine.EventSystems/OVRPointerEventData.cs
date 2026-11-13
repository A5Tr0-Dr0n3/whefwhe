using System.Text;

namespace UnityEngine.EventSystems;

public class OVRPointerEventData : PointerEventData
{
	public Ray worldSpaceRay;

	public Vector2 swipeStart;

	public OVRPointerEventData(EventSystem eventSystem)
		: base(eventSystem)
	{
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("<b>Position</b>: " + position);
		stringBuilder.AppendLine("<b>delta</b>: " + delta);
		stringBuilder.AppendLine("<b>eligibleForClick</b>: " + eligibleForClick);
		stringBuilder.AppendLine("<b>pointerEnter</b>: " + pointerEnter);
		stringBuilder.AppendLine("<b>pointerPress</b>: " + pointerPress);
		stringBuilder.AppendLine("<b>lastPointerPress</b>: " + lastPress);
		stringBuilder.AppendLine("<b>pointerDrag</b>: " + pointerDrag);
		stringBuilder.AppendLine("<b>worldSpaceRay</b>: " + worldSpaceRay);
		stringBuilder.AppendLine("<b>swipeStart</b>: " + swipeStart);
		stringBuilder.AppendLine("<b>Use Drag Threshold</b>: " + useDragThreshold);
		return stringBuilder.ToString();
	}
}
