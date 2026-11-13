using TButt;
using UnityEngine;
using UnityEngine.XR;

public class ToolGroup : MonoBehaviour
{
	public enum Tool
	{
		Leash,
		Teleporter
	}

	[SerializeField]
	private XRNode _nodeToAttachWith;

	[SerializeField]
	private TBInput.Controller _controller;

	[SerializeField]
	private ObjectBlackHole _blackHole;

	[SerializeField]
	private Manipulator _manipulator;

	[SerializeField]
	private CursorReticle _cursorReticle;

	[SerializeField]
	private Teleporter _teleporter;

	[SerializeField]
	private EagerTouch _eagerTouch;

	[SerializeField]
	private VacLabelManager _vLManager;

	public Tool CurrentTool;

	public TBInput.Controller Controller => _controller;

	public XRNode Node => _nodeToAttachWith;

	public ObjectBlackHole BlackHole => _blackHole;

	public Manipulator Manipulator => _manipulator;

	public CursorReticle CursorReticle => _cursorReticle;

	public Teleporter Teleporter => _teleporter;

	public EagerTouch EagerTouch => _eagerTouch;

	public VacLabelManager VLManager => _vLManager;

	public float AdminCheatTimer { get; private set; }

	public bool IsActive { get; private set; }

	public void Activate(Transform t)
	{
		if (!IsActive)
		{
			gameObject.SetActive(value: true);
			_cursorReticle.gameObject.SetActive(value: true);
			_cursorReticle.Initialize(this);
			_teleporter.gameObject.SetActive(value: true);
			_teleporter.Initialize(this);
			_manipulator.Initialize(this);
			_blackHole.gameObject.SetActive(value: false);
			_blackHole.Initialize(this);
			_manipulator.gameObject.SetActive(value: true);
			_manipulator.transform.MakeZeroedChildOf(t);
			IsActive = true;
		}
	}

	public void ToggleManipulator(bool toggle)
	{
		HeadsetRemovalUIManager.instance.gameObject.SetActive(toggle);
		_manipulator.gameObject.SetActive(toggle);
		gameObject.SetActive(toggle);
		if (!toggle)
		{
			_cursorReticle.transform.position = EagerMaster.instance.player.transform.position + Vector3.down * 2f;
		}
		_cursorReticle.centerRenderer.gameObject.SetActive(toggle);
		_cursorReticle.stateRenderer.gameObject.SetActive(toggle);
	}

	private void Update()
	{
		if (IsActive && PersistentDataManager.HasAccountLoaded() && !PersistentDataManager.GetHasAdmin())
		{
			if (TBInput.GetButton(TBInput.Button.Action2, _controller) && TBInput.GetButton(TBInput.Button.Action1, _controller))
			{
				AdminCheatTimer += Time.deltaTime;
			}
			else
			{
				AdminCheatTimer = 0f;
			}
		}
	}

	public void ResetAdminCheatTimer()
	{
		AdminCheatTimer = 0f;
	}
}
