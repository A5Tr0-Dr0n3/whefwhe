using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GvrDropdown : Dropdown
{
	private GameObject currentBlocker;

	public override void OnPointerClick(PointerEventData eventData)
	{
		base.OnPointerClick(eventData);
		FixTemplateAndBlockerRaycasters();
	}

	public override void OnSubmit(BaseEventData eventData)
	{
		base.OnSubmit(eventData);
		FixTemplateAndBlockerRaycasters();
	}

	private void FixTemplateAndBlockerRaycasters()
	{
		if (template != null)
		{
			FixRaycaster(template.gameObject, shouldCopyProperties: false);
		}
		FixRaycaster(currentBlocker, shouldCopyProperties: true);
	}

	protected override GameObject CreateBlocker(Canvas rootCanvas)
	{
		currentBlocker = base.CreateBlocker(rootCanvas);
		return currentBlocker;
	}

	protected override GameObject CreateDropdownList(GameObject template)
	{
		GameObject gameObject = base.CreateDropdownList(template);
		FixRaycaster(gameObject, shouldCopyProperties: false);
		return gameObject;
	}

	private void FixRaycaster(GameObject go, bool shouldCopyProperties)
	{
		if (go == null)
		{
			return;
		}
		GraphicRaycaster component = go.GetComponent<GraphicRaycaster>();
		Object.Destroy(component);
		GvrPointerGraphicRaycaster orAddComponent = GetOrAddComponent<GvrPointerGraphicRaycaster>(go, out var addedComponent);
		if (shouldCopyProperties)
		{
			GvrPointerGraphicRaycaster templateRaycaster = GetTemplateRaycaster();
			if (addedComponent && templateRaycaster != null)
			{
				CopyRaycasterProperties(templateRaycaster, orAddComponent);
			}
		}
	}

	private GvrPointerGraphicRaycaster GetTemplateRaycaster()
	{
		if (template == null)
		{
			return null;
		}
		return template.GetComponent<GvrPointerGraphicRaycaster>();
	}

	private void CopyRaycasterProperties(GvrPointerGraphicRaycaster source, GvrPointerGraphicRaycaster dest)
	{
		if (!(source == null) && !(dest == null))
		{
			dest.blockingMask = source.blockingMask;
			dest.blockingObjects = source.blockingObjects;
			dest.ignoreReversedGraphics = source.ignoreReversedGraphics;
		}
	}

	private static T GetOrAddComponent<T>(GameObject go, out bool addedComponent) where T : Component
	{
		T val = go.GetComponent<T>();
		addedComponent = false;
		if (!val)
		{
			val = go.AddComponent<T>();
			addedComponent = true;
		}
		return val;
	}
}
