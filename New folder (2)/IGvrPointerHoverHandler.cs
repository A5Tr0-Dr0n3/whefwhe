using UnityEngine.EventSystems;

public interface IGvrPointerHoverHandler : IEventSystemHandler
{
	void OnGvrPointerHover(PointerEventData eventData);
}
