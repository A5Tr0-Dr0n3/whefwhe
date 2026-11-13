using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GvrAllEventsTrigger : MonoBehaviour
{
	[Serializable]
	public class TriggerEvent : UnityEvent<GameObject, PointerEventData>
	{
	}

	public TriggerEvent OnPointerClick;

	public TriggerEvent OnPointerDown;

	public TriggerEvent OnPointerUp;

	public TriggerEvent OnPointerEnter;

	public TriggerEvent OnPointerExit;

	public TriggerEvent OnScroll;

	private bool listenersAdded;

	private void OnEnable()
	{
		AddListeners();
	}

	private void OnDisable()
	{
		RemoveListeners();
	}

	private void Start()
	{
		AddListeners();
	}

	private void AddListeners()
	{
		GvrEventExecutor gvrEventExecutor = GvrPointerInputModule.FindEventExecutor();
		if (gvrEventExecutor != null && !listenersAdded)
		{
			gvrEventExecutor.OnPointerClick += OnPointerClickHandler;
			gvrEventExecutor.OnPointerDown += OnPointerDownHandler;
			gvrEventExecutor.OnPointerUp += OnPointerUpHandler;
			gvrEventExecutor.OnPointerEnter += OnPointerEnterHandler;
			gvrEventExecutor.OnPointerExit += OnPointerExitHandler;
			gvrEventExecutor.OnScroll += OnScrollHandler;
			listenersAdded = true;
		}
	}

	private void RemoveListeners()
	{
		GvrEventExecutor gvrEventExecutor = GvrPointerInputModule.FindEventExecutor();
		if (gvrEventExecutor != null && listenersAdded)
		{
			gvrEventExecutor.OnPointerClick -= OnPointerClickHandler;
			gvrEventExecutor.OnPointerDown -= OnPointerDownHandler;
			gvrEventExecutor.OnPointerUp -= OnPointerUpHandler;
			gvrEventExecutor.OnPointerEnter -= OnPointerEnterHandler;
			gvrEventExecutor.OnPointerExit -= OnPointerExitHandler;
			gvrEventExecutor.OnScroll -= OnScrollHandler;
			listenersAdded = false;
		}
	}

	private void OnPointerClickHandler(GameObject target, PointerEventData eventData)
	{
		OnPointerClick.Invoke(target, eventData);
	}

	private void OnPointerDownHandler(GameObject target, PointerEventData eventData)
	{
		OnPointerDown.Invoke(target, eventData);
	}

	private void OnPointerUpHandler(GameObject target, PointerEventData eventData)
	{
		OnPointerUp.Invoke(target, eventData);
	}

	private void OnPointerEnterHandler(GameObject target, PointerEventData eventData)
	{
		OnPointerEnter.Invoke(target, eventData);
	}

	private void OnPointerExitHandler(GameObject target, PointerEventData eventData)
	{
		OnPointerExit.Invoke(target, eventData);
	}

	private void OnScrollHandler(GameObject target, PointerEventData eventData)
	{
		OnScroll.Invoke(target, eventData);
	}
}
