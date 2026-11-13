using UnityEngine;
using UnityEngine.EventSystems;

public interface IGvrEventExecutor
{
	bool Execute<T>(GameObject target, BaseEventData eventData, ExecuteEvents.EventFunction<T> functor) where T : IEventSystemHandler;

	GameObject ExecuteHierarchy<T>(GameObject root, BaseEventData eventData, ExecuteEvents.EventFunction<T> callbackFunction) where T : IEventSystemHandler;

	GameObject GetEventHandler<T>(GameObject root) where T : IEventSystemHandler;
}
