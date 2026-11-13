using System;

[Serializable]
public class HubControlsSubmenu
{
	public HubControlsSubmenuItem[] items;

	public HubControlsSubmenu(HubControlsSubmenuItem[] newItems)
	{
		items = newItems;
	}
}
