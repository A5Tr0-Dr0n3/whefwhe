using UnityEngine.EventSystems;

public static class GvrExecuteEventsExtension
{
	private static readonly ExecuteEvents.EventFunction<IGvrPointerHoverHandler> s_HoverHandler = Execute;

	public static ExecuteEvents.EventFunction<IGvrPointerHoverHandler> pointerHoverHandler => s_HoverHandler;

	private static void Execute(IGvrPointerHoverHandler handler, BaseEventData eventData)
	{
		handler.OnGvrPointerHover(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
	}
}
