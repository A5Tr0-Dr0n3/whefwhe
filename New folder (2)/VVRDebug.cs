using TButt;
using UnityEngine;

public class VVRDebug : MonoBehaviour
{
	[SerializeField]
	private VVRLogDisplay logDisplay;

	[SerializeField]
	private VVRDebugInterface debugInterface;

	[SerializeField]
	public ObjectButton debugButton1;

	[SerializeField]
	public ObjectButton debugButton2;

	[SerializeField]
	public ObjectToggle dialogueSpeedUpToggle;

	[SerializeField]
	public ObjectToggle debugToggle1;

	[SerializeField]
	public ObjectToggle debugToggle2;

	private const int numClicksForActivation = 4;

	private const float clickCounterResetTime = 0.3f;

	private const float lastClickDebugInterfaceHoldTime = 0.8f;

	private int clickCount;

	private float clickResetTimer;

	private float clickHoldTimer;

	public static VVRDebug instance;

	private void Awake()
	{
		instance = this;
		if (!PersistentDataManager.GetCanActivateDebug())
		{
			logDisplay.Activate(activeIn: false);
		}
	}

	public void DeactivateInterface()
	{
		debugInterface.Activate(activeIn: false);
		clickCount = 0;
	}

	private void Update()
	{
		if (!PersistentDataManager.GetCanActivateDebug())
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			debugInterface.ToggleActive();
		}
		if (debugInterface.activated)
		{
			return;
		}
		if (TBInput.GetButtonDown(TBInput.Button.Action1, TBInput.Controller.Active))
		{
			clickCount++;
		}
		if (TBInput.GetButtonUp(TBInput.Button.Action1, TBInput.Controller.Active) && clickCount >= 4 && clickHoldTimer < 0.8f)
		{
			if (PersistentDataManager.GetDebugPanelType() != DebugPanelType.Disabled)
			{
				logDisplay.ToggleActive();
			}
			clickCount = 0;
		}
		if (clickCount >= 4 && clickHoldTimer > 0.8f)
		{
			debugInterface.ToggleActive();
			clickCount = 0;
		}
		if (TBInput.GetButtonDown(TBInput.Button.Action1, TBInput.Controller.Active) || TBInput.GetButtonUp(TBInput.Button.Action1, TBInput.Controller.Active))
		{
			clickResetTimer = 0f;
			clickHoldTimer = 0f;
		}
		if (TBInput.GetButton(TBInput.Button.Action1, TBInput.Controller.Active))
		{
			clickHoldTimer += Time.deltaTime;
		}
		else
		{
			clickResetTimer += Time.deltaTime;
		}
		if (clickResetTimer >= 0.3f)
		{
			clickCount = 0;
		}
	}
}
