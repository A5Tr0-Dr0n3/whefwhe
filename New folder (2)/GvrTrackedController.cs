using UnityEngine;

public class GvrTrackedController : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Arm model used to control the pose (position and rotation) of the object, and to propagate to children that implement IGvrArmModelReceiver.")]
	private GvrBaseArmModel armModel;

	[SerializeField]
	[Tooltip("Is the object's active status determined by the controller connection status.")]
	private bool isDeactivatedWhenDisconnected = true;

	public GvrBaseArmModel ArmModel
	{
		get
		{
			return armModel;
		}
		set
		{
			if (!(armModel == value))
			{
				armModel = value;
				PropagateArmModel();
			}
		}
	}

	public bool IsDeactivatedWhenDisconnected
	{
		get
		{
			return isDeactivatedWhenDisconnected;
		}
		set
		{
			if (isDeactivatedWhenDisconnected != value)
			{
				isDeactivatedWhenDisconnected = value;
				if (isDeactivatedWhenDisconnected)
				{
					OnControllerStateChanged(GvrControllerInput.State, GvrControllerInput.State);
				}
			}
		}
	}

	public void PropagateArmModel()
	{
		IGvrArmModelReceiver[] componentsInChildren = GetComponentsInChildren<IGvrArmModelReceiver>(includeInactive: true);
		foreach (IGvrArmModelReceiver gvrArmModelReceiver in componentsInChildren)
		{
			gvrArmModelReceiver.ArmModel = armModel;
		}
	}

	private void Awake()
	{
		GvrControllerInput.OnStateChanged += OnControllerStateChanged;
	}

	private void OnEnable()
	{
		GvrControllerInput.OnPostControllerInputUpdated += OnPostControllerInputUpdated;
		UpdatePose();
		OnControllerStateChanged(GvrControllerInput.State, GvrControllerInput.State);
	}

	private void OnDisable()
	{
		GvrControllerInput.OnPostControllerInputUpdated -= OnPostControllerInputUpdated;
	}

	private void Start()
	{
		PropagateArmModel();
		OnControllerStateChanged(GvrControllerInput.State, GvrControllerInput.State);
	}

	private void OnDestroy()
	{
		GvrControllerInput.OnStateChanged -= OnControllerStateChanged;
	}

	private void OnPostControllerInputUpdated()
	{
		UpdatePose();
	}

	private void OnControllerStateChanged(GvrConnectionState state, GvrConnectionState oldState)
	{
		if (isDeactivatedWhenDisconnected && enabled)
		{
			gameObject.SetActive(state == GvrConnectionState.Connected);
		}
	}

	private void UpdatePose()
	{
		if (!(armModel == null))
		{
			transform.localPosition = ArmModel.ControllerPositionFromHead;
			transform.localRotation = ArmModel.ControllerRotationFromHead;
		}
	}
}
