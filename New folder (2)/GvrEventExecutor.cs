using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GvrEventExecutor : IGvrEventExecutor
{
	public delegate void EventDelegate(GameObject target, PointerEventData eventData);

	private Dictionary<Type, EventDelegate> eventTable;

	public event EventDelegate OnPointerClick
	{
		add
		{
			AddEventDelegate<IPointerClickHandler>(value);
		}
		remove
		{
			RemoveEventDelegate<IPointerClickHandler>(value);
		}
	}

	public event EventDelegate OnPointerDown
	{
		add
		{
			AddEventDelegate<IPointerDownHandler>(value);
		}
		remove
		{
			RemoveEventDelegate<IPointerDownHandler>(value);
		}
	}

	public event EventDelegate OnPointerUp
	{
		add
		{
			AddEventDelegate<IPointerUpHandler>(value);
		}
		remove
		{
			RemoveEventDelegate<IPointerUpHandler>(value);
		}
	}

	public event EventDelegate OnPointerEnter
	{
		add
		{
			AddEventDelegate<IPointerEnterHandler>(value);
		}
		remove
		{
			RemoveEventDelegate<IPointerEnterHandler>(value);
		}
	}

	public event EventDelegate OnPointerExit
	{
		add
		{
			AddEventDelegate<IPointerExitHandler>(value);
		}
		remove
		{
			RemoveEventDelegate<IPointerExitHandler>(value);
		}
	}

	public event EventDelegate OnScroll
	{
		add
		{
			AddEventDelegate<IScrollHandler>(value);
		}
		remove
		{
			RemoveEventDelegate<IScrollHandler>(value);
		}
	}

	public GvrEventExecutor()
	{
		eventTable = new Dictionary<Type, EventDelegate>();
	}

	public bool Execute<T>(GameObject target, BaseEventData eventData, ExecuteEvents.EventFunction<T> functor) where T : IEventSystemHandler
	{
		bool result = ExecuteEvents.Execute(target, eventData, functor);
		CallEventDelegate<T>(target, eventData);
		return result;
	}

	public GameObject ExecuteHierarchy<T>(GameObject root, BaseEventData eventData, ExecuteEvents.EventFunction<T> callbackFunction) where T : IEventSystemHandler
	{
		GameObject result = ExecuteEvents.ExecuteHierarchy(root, eventData, callbackFunction);
		CallEventDelegate<T>(root, eventData);
		return result;
	}

	public GameObject GetEventHandler<T>(GameObject root) where T : IEventSystemHandler
	{
		return ExecuteEvents.GetEventHandler<T>(root);
	}

	private void CallEventDelegate<T>(GameObject target, BaseEventData eventData) where T : IEventSystemHandler
	{
		Type typeFromHandle = typeof(T);
		if (eventTable.TryGetValue(typeFromHandle, out var value))
		{
			if (!(eventData is PointerEventData eventData2))
			{
				Debug.LogError("Event data must be PointerEventData.");
			}
			else
			{
				value(target, eventData2);
			}
		}
	}

	private void AddEventDelegate<T>(EventDelegate eventDelegate)
	{
		Type typeFromHandle = typeof(T);
		if (eventTable.TryGetValue(typeFromHandle, out var value))
		{
			eventTable[typeFromHandle] = (EventDelegate)Delegate.Combine(value, eventDelegate);
		}
		else
		{
			eventTable[typeFromHandle] = eventDelegate;
		}
	}

	private void RemoveEventDelegate<T>(EventDelegate eventDelegate)
	{
		Type typeFromHandle = typeof(T);
		if (eventTable.TryGetValue(typeFromHandle, out var value))
		{
			eventDelegate = (EventDelegate)Delegate.Remove(value, eventDelegate);
			if (eventDelegate != null)
			{
				eventTable[typeFromHandle] = eventDelegate;
			}
			else
			{
				eventTable.Remove(typeFromHandle);
			}
		}
	}
}
