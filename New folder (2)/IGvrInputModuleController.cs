using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IGvrInputModuleController
{
	EventSystem eventSystem { get; }

	List<RaycastResult> RaycastResultCache { get; }

	bool ShouldActivate();

	void Deactivate();

	GameObject FindCommonRoot(GameObject g1, GameObject g2);

	BaseEventData GetBaseEventData();

	RaycastResult FindFirstRaycast(List<RaycastResult> candidates);
}
