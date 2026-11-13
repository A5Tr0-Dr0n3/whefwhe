using TButt;
using UnityEngine;

public class HubControls : MonoBehaviour
{
	public enum Submenu
	{
		grabDrop,
		headset,
		movement,
		vacuum,
		NUMELEMENTS
	}

	[Header("References")]
	public GameObject toggle;

	public GameObject controllerDiagram;

	public GameObject removalDiagram;

	public TextSwapper labelText;

	public TextSwapper infoText;

	public HubControlsButton[] buttons;

	public ControllerDiagram platformController;

	public ControllerDiagram gearPlatformController;

	public ControllerDiagram goPlatformController;

	public ControllerDiagram riftPlatformController;

	public GameObject gearControllerDiagram;

	public GameObject goControllerDiagram;

	public GameObject riftControllerDiagram;

	public GameObject gearRemovalDiagram;

	public GameObject goRemovalDiagram;

	public GameObject riftRemovalDiagram;

	public GameObject[] vacuumStuff;

	[Header("Submenus")]
	public Submenu currentSubmenu;

	public HubControlsSubmenuItem[] grabDropSubmenu;

	public HubControlsSubmenuItem[] headsetSubmenu;

	public HubControlsSubmenuItem[] movementSubmenu;

	public HubControlsSubmenuItem[] vacuumSubmenu;

	public HubControlsSubmenuItem[] goMovementSubmenu;

	public HubControlsSubmenuItem[] gearMovementSubmenu;

	public HubControlsSubmenuItem[] riftMovementSubmenu;

	public HubControlsSubmenuItem[] viveMovementSubmenu;

	public HubControlsSubmenuItem[] psVRMovementubmenuDualshock;

	public HubControlsSubmenuItem[] psVRMovementubmenuMove;

	public HubControlsSubmenuItem[] riftVacuumSubmenu;

	public HubControlsSubmenuItem[] psVRVacuumSubmenuDualshock;

	public HubControlsSubmenuItem[] psVRVacuumSubmenuMove;

	public HubControlsSubmenuItem[] riftGrabDropSubmenu;

	public HubControlsSubmenuItem[] psVRGrabDropSubmenuDualshock;

	public HubControlsSubmenuItem[] psVRGrabDropSubmenuMove;

	public HubControlsSubmenuItem[] riftHeadsetSubmenu;

	public HubControlsSubmenuItem[] psVRHeadsetSubmenuMove;

	public HubControlsSubmenuItem[] psVRHeadsetSubmenuDualshock;

	public Color activeColor;

	public Color inactiveColor;

	private int currentMenuID;

	private int currentSubmenuItem;

	private float timer;

	private bool justOpened;

	private void Start()
	{
		if (TBCore.GetActivePlatform() == VRPlatform.OculusMobile)
		{
			if (OVRPlugin.productName == "Oculus Go")
			{
				platformController.gameObject.SetActive(value: false);
				controllerDiagram.gameObject.SetActive(value: false);
				platformController = goPlatformController;
				controllerDiagram = goControllerDiagram;
				removalDiagram = goRemovalDiagram;
				platformController.gameObject.SetActive(value: true);
				controllerDiagram.gameObject.SetActive(value: true);
				movementSubmenu = goMovementSubmenu;
			}
			else
			{
				platformController.gameObject.SetActive(value: false);
				controllerDiagram.gameObject.SetActive(value: false);
				platformController = gearPlatformController;
				controllerDiagram = gearControllerDiagram;
				removalDiagram = gearRemovalDiagram;
				platformController.gameObject.SetActive(value: true);
				controllerDiagram.gameObject.SetActive(value: true);
				movementSubmenu = gearMovementSubmenu;
			}
		}
		else if (TBCore.GetActivePlatform() == VRPlatform.OculusPC)
		{
			platformController.gameObject.SetActive(value: false);
			controllerDiagram.gameObject.SetActive(value: false);
			platformController = riftPlatformController;
			controllerDiagram = riftControllerDiagram;
			removalDiagram = riftRemovalDiagram;
			platformController.gameObject.SetActive(value: true);
			controllerDiagram.gameObject.SetActive(value: true);
			movementSubmenu = riftMovementSubmenu;
			grabDropSubmenu = riftGrabDropSubmenu;
			vacuumSubmenu = riftVacuumSubmenu;
			headsetSubmenu = riftHeadsetSubmenu;
		}
		Toggle();
		controllerDiagram.SetActive(value: false);
		removalDiagram.SetActive(value: false);
	}

	private void Update()
	{
		for (int i = 0; i < ToolManager.ActiveToolGroups.Count; i++)
		{
			if (TBInput.GetButtonUp(TBInput.Button.Action1, ToolManager.ActiveToolGroups[i].Controller))
			{
				if (justOpened)
				{
					justOpened = false;
				}
				else
				{
					Toggle(forceClose: true);
				}
				break;
			}
		}
		if (!PersistentDataManager.GetVacuumState())
		{
			for (int j = 0; j < vacuumStuff.Length; j++)
			{
				vacuumStuff[j].SetActive(value: false);
			}
		}
		else
		{
			for (int k = 0; k < vacuumStuff.Length; k++)
			{
				vacuumStuff[k].SetActive(value: true);
			}
		}
		if (!toggle.activeSelf || currentSubmenu == Submenu.NUMELEMENTS)
		{
			return;
		}
		timer += Time.deltaTime;
		if (!(timer > SubmenuEnumToArray(currentSubmenu)[currentSubmenuItem].displayTime))
		{
			return;
		}
		timer = 0f;
		if (currentSubmenuItem < SubmenuEnumToArray(currentSubmenu).Length - 1)
		{
			currentSubmenuItem++;
		}
		else
		{
			currentSubmenuItem = 0;
			if (currentSubmenu < Submenu.vacuum)
			{
				currentSubmenu++;
			}
			else
			{
				currentSubmenu = Submenu.grabDrop;
			}
		}
		if (currentSubmenu == Submenu.vacuum && !PersistentDataManager.GetVacuumState())
		{
			currentSubmenu = Submenu.grabDrop;
		}
		DeselectAllButtons();
		buttons[(int)currentSubmenu].Select();
		labelText.SetNewKey(SubmenuEnumToArray(currentSubmenu)[currentSubmenuItem].label);
		infoText.SetNewKey(SubmenuEnumToArray(currentSubmenu)[currentSubmenuItem].text);
		Diagram diagram = SubmenuEnumToArray(currentSubmenu)[currentSubmenuItem].diagram;
		removalDiagram.SetActive(value: false);
		switch (diagram)
		{
		case Diagram.ControllerTPClick:
			platformController.StartGrabAnimation();
			break;
		case Diagram.ControllerTPSwipeVert:
			platformController.StartUpDownAnimation();
			break;
		case Diagram.ControllerTPSwipeHoriz:
			platformController.StartFlipAnimtaion(0.6f);
			break;
		case Diagram.ControllerTPTop:
			platformController.StartVacuumOutAnimation();
			break;
		case Diagram.ControllerTPBottom:
			platformController.StartVacuumInAnimation();
			break;
		case Diagram.ControllerAppHold:
			platformController.StartTeleportAnimation();
			break;
		case Diagram.ControllerAppTripleClick:
			platformController.StartTrippleTapAnimation();
			break;
		case Diagram.RemovalSign:
			removalDiagram.SetActive(value: true);
			platformController.DisableController();
			break;
		}
	}

	public void Toggle(bool forceClose)
	{
		if (forceClose)
		{
			toggle.SetActive(value: false);
			CloseMenu();
			return;
		}
		bool flag = !toggle.activeInHierarchy;
		toggle.SetActive(!toggle.activeSelf);
		CloseMenu();
		if (flag)
		{
			SubmenuSwitch(Submenu.grabDrop);
		}
	}

	public void Toggle()
	{
		bool flag = !toggle.activeInHierarchy;
		toggle.SetActive(!toggle.activeSelf);
		if (toggle.activeInHierarchy)
		{
			justOpened = true;
		}
		CloseMenu();
		if (flag)
		{
			SubmenuSwitch(Submenu.grabDrop);
		}
	}

	public void SubmenuSwitch(Submenu newSubmenu)
	{
		if (currentSubmenu == newSubmenu)
		{
			justOpened = true;
			return;
		}
		CloseMenu();
		if (newSubmenu != currentSubmenu || newSubmenu == Submenu.grabDrop)
		{
			OpenMenu(newSubmenu);
		}
		timer = 0f;
		currentSubmenuItem = 0;
	}

	private void CloseMenu()
	{
		removalDiagram.SetActive(value: false);
		controllerDiagram.SetActive(value: false);
		labelText.gameObject.SetActive(value: false);
		infoText.gameObject.SetActive(value: false);
		currentSubmenu = Submenu.NUMELEMENTS;
		currentMenuID = 0;
	}

	private void OpenMenu(Submenu newSubmenu)
	{
		justOpened = true;
		labelText.gameObject.SetActive(value: true);
		infoText.gameObject.SetActive(value: true);
		switch (newSubmenu)
		{
		case Submenu.grabDrop:
			buttons[0].Select();
			platformController.StartGrabAnimation();
			controllerDiagram.SetActive(value: true);
			break;
		case Submenu.headset:
			buttons[1].Select();
			controllerDiagram.SetActive(value: true);
			platformController.StartUpDownAnimation();
			break;
		case Submenu.movement:
			platformController.StartTeleportAnimation();
			buttons[2].Select();
			controllerDiagram.SetActive(value: true);
			break;
		case Submenu.vacuum:
			platformController.StartFlipAnimtaion(0.6f);
			buttons[3].Select();
			controllerDiagram.SetActive(value: true);
			break;
		}
		currentSubmenu = newSubmenu;
		if (currentSubmenu == Submenu.movement && TBCore.GetActivePlatform() == VRPlatform.OculusMobile)
		{
			if (OVRPlugin.productName == "Oculus Go")
			{
				labelText.SetNewKey("TEXT_000");
			}
			else
			{
				labelText.SetNewKey("TEXT_000");
			}
		}
		labelText.SetNewKey(SubmenuEnumToArray(currentSubmenu)[0].label);
		infoText.SetNewKey(SubmenuEnumToArray(currentSubmenu)[0].text);
	}

	private HubControlsSubmenuItem[] SubmenuEnumToArray(Submenu enumElement)
	{
		return enumElement switch
		{
			Submenu.grabDrop => grabDropSubmenu, 
			Submenu.headset => headsetSubmenu, 
			Submenu.movement => movementSubmenu, 
			Submenu.vacuum => vacuumSubmenu, 
			_ => null, 
		};
	}

	public void DeselectAllButtons()
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i].DeSelect();
		}
	}
}
