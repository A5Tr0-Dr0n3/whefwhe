using UnityEngine;

[RequireComponent(typeof(FixedJoint))]
[RequireComponent(typeof(ObjectBase))]
[RequireComponent(typeof(AudioEvent))]
public class KitchenFridgeDoorTrigger : MonoBehaviour
{
	[SerializeField]
	private float fixedJointBreakForce;

	public bool hubFridge;

	[HideInInspector]
	public bool playerOpened;

	private ObjectBase _objBase;

	private FixedJoint _fixedJoint;

	private AudioEvent _openNoise;

	private bool hasBeenHeldByPlayer;

	private bool fixedJointBroken;

	private void Start()
	{
		_objBase = GetComponent<ObjectBase>();
		_fixedJoint = GetComponent<FixedJoint>();
		_openNoise = GetComponent<AudioEvent>();
		_fixedJoint.breakForce = float.PositiveInfinity;
	}

	private void Update()
	{
		if (ToolManager.PlayerGrabbingObjectBase(_objBase))
		{
			hasBeenHeldByPlayer = true;
			if (_fixedJoint != null)
			{
				_fixedJoint.breakForce = fixedJointBreakForce;
			}
		}
		else if (_fixedJoint != null)
		{
			_fixedJoint.breakForce = float.PositiveInfinity;
		}
		if (hasBeenHeldByPlayer && fixedJointBroken)
		{
			if (hubFridge)
			{
				VVR_HUB.instance.GetComponent<HubExt>().CommentSnackFridge();
			}
			playerOpened = true;
		}
	}

	private void OnJointBreak(float breakForce)
	{
		fixedJointBroken = true;
		_openNoise.Play();
	}
}
