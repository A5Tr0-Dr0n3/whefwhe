using System.Collections.Generic;
using UnityEngine;

public class SceneBehaviorManager : MonoBehaviour
{
	private bool hasBeenLoaded;

	private KeyValuePair<Manipulator, ObjectBase> undroppableObjectPair;

	public virtual void ActivateSceneBehavior()
	{
		if (!hasBeenLoaded)
		{
			OnFirstTimeSceneActivated();
			hasBeenLoaded = true;
		}
		OnSceneActivated();
	}

	protected virtual void OnFirstTimeSceneActivated()
	{
		undroppableObjectPair = new KeyValuePair<Manipulator, ObjectBase>(null, null);
	}

	protected virtual void OnSceneActivated()
	{
		if (undroppableObjectPair.Key != null)
		{
			bool flag = false;
			if (undroppableObjectPair.Value.ungrabbable)
			{
				flag = true;
				undroppableObjectPair.Value.ungrabbable = false;
			}
			undroppableObjectPair.Key.Grab(undroppableObjectPair.Value, undroppableObjectPair.Value.transform.position, playSound: false);
			if (flag)
			{
				undroppableObjectPair.Value.ungrabbable = true;
			}
			undroppableObjectPair = new KeyValuePair<Manipulator, ObjectBase>(null, null);
		}
	}

	public virtual void OnSceneDeactivated()
	{
		ToolGroup[] allToolgroups = ToolManager.GetAllToolgroups();
		for (int i = 0; i < allToolgroups.Length; i++)
		{
			ObjectBase heldObjectBase = allToolgroups[i].Manipulator.HeldObjectBase;
			if (heldObjectBase != null && heldObjectBase.undroppable)
			{
				undroppableObjectPair = new KeyValuePair<Manipulator, ObjectBase>(allToolgroups[i].Manipulator, heldObjectBase);
			}
		}
	}
}
