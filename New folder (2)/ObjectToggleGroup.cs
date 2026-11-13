using UnityEngine;

public class ObjectToggleGroup : MonoBehaviour
{
	[SerializeField]
	private ObjectToggle[] groupToggles;

	public ObjectToggle currentToggled { get; private set; }

	public void Start()
	{
		for (int i = 0; i < groupToggles.Length; i++)
		{
			groupToggles[i].onToggled += reactToGroupObjectToggled;
		}
	}

	public void ToggleAtIndex(int index)
	{
		if (index < groupToggles.Length && groupToggles[index] != null)
		{
			groupToggles[index].Toggle(newState: true);
			currentToggled = groupToggles[index];
		}
		else
		{
			SafeDebug.LogError("Error! Trying to toggle nonexistent index!");
		}
	}

	public void ClearState()
	{
		for (int i = 0; i < groupToggles.Length; i++)
		{
			groupToggles[i].Toggle(newState: false);
		}
	}

	private void reactToGroupObjectToggled(ObjectToggle toggledObj, bool toggleValue, bool playerTriggered)
	{
		if (toggleValue)
		{
			currentToggled = toggledObj;
			for (int i = 0; i < groupToggles.Length; i++)
			{
				if (groupToggles[i] != toggledObj && groupToggles[i].toggleState)
				{
					groupToggles[i].Toggle(newState: false);
				}
			}
		}
		if (toggleValue || !playerTriggered)
		{
			return;
		}
		bool flag = false;
		for (int j = 0; j < groupToggles.Length; j++)
		{
			if (groupToggles[j].toggleState)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			toggledObj.Toggle(newState: true, playerTriggered);
		}
	}
}
