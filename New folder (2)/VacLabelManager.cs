using UnityEngine;

public class VacLabelManager : MonoBehaviour
{
	public VacLabel label;

	public ToolGroup _toolGroup;

	private Transform playerTransform;

	private VacLabel workingLabel;

	private Vector3 targetPosition;

	private VVRConsumer blackHoleConsumer;

	private bool preHideTimerActive;

	private float preHideTimer;

	private float preHideTimerDur;

	private Vector3 scaleTarget = Vector3.one;

	private Vector3 labelTarget;

	private Vector3 labelOrigin;

	private Vector3 meshCenter;

	private Vector3 labelLinePos;

	private RaycastHit hit;

	private Ray ray;

	private VVRConsumable tastiest;

	private VVRConsumable consumable;

	private Collider[] consumableColliders;

	private float lowestSquareToMouth;

	private float squareToMouth;

	private VacLabel testLabel;

	private int cnt;

	private Vector3 closestPoint;

	[HideInInspector]
	public Collider closestCol;

	private Vector3 labelGone = new Vector3(0f, 1f, 1f);

	private float roundedValue;

	private void Awake()
	{
		playerTransform = EagerMaster.instance.mainCamera.transform;
	}

	private void Start()
	{
		blackHoleConsumer = _toolGroup.BlackHole.vacConsumer;
	}

	private void Update()
	{
		if (preHideTimerActive)
		{
			PositionLabel();
			HideLabel();
		}
		else if (_toolGroup.Manipulator.CurrentManipulatorMode == Manipulator.manipulatorMode.vacuum)
		{
			switch (_toolGroup.EagerTouch.CurrentVacInputStatus())
			{
			case EagerTouch.VacInputStatus.previewOut:
			case EagerTouch.VacInputStatus.Off:
			case EagerTouch.VacInputStatus.PreviewIn:
				UpdateLabel();
				break;
			case EagerTouch.VacInputStatus.In:
				HideLabel();
				break;
			case EagerTouch.VacInputStatus.Out:
				CheckForInsufficient();
				HideLabel();
				break;
			}
		}
		else if (label.gameObject.activeSelf)
		{
			preHideTimerActive = false;
			HideLabel();
		}
	}

	private void UpdateLabel()
	{
		CheckNearby();
		PositionLabel();
		label.UpdateLabelColors(_toolGroup.EagerTouch.CurrentVacInputStatus());
	}

	private void StartPreHideTimer(float duration)
	{
		preHideTimerActive = true;
		preHideTimerDur = duration;
		preHideTimer = 0f;
	}

	private void HideLabel()
	{
		if (preHideTimerActive)
		{
			preHideTimer += Time.deltaTime;
			if (!(preHideTimer > preHideTimerDur))
			{
				return;
			}
			preHideTimerActive = false;
		}
		if (label.transform.localScale.x < 0.01f)
		{
			label.consumable = null;
			label.gameObject.SetActive(value: false);
			label.transform.localScale = labelGone;
		}
		else
		{
			label.transform.localScale = Vector3.Lerp(label.transform.localScale, labelGone, 20f * Time.deltaTime);
			label.transform.position = Vector3.Lerp(label.gameObject.transform.position, labelOrigin, 20f * Time.deltaTime);
			label.SetStemEndPoint(labelOrigin);
		}
	}

	private void CheckForInsufficient()
	{
		if (label.currentLabelStatus != VacLabel.labelStatus.insufficient && label.consumable != null && label.consumable.totalVacuumValue - label.consumable.currentVacuumValueCapped > EagerMaster.instance.vacuumFill + 0.1f && !label.consumable.dontStartNewTransfer)
		{
			label.SetLabelStatus(VacLabel.labelStatus.insufficient);
			label.SetColor(label.upColor);
			StartPreHideTimer(0.5f);
			_toolGroup.Manipulator.TriggerFillUIMessage("INSUFFICIENT", 3f, 4);
		}
	}

	public void PositionLabel()
	{
		if (label.consumable == null)
		{
			_toolGroup.BlackHole.labelOrigin = null;
			return;
		}
		label.gameObject.SetActive(value: true);
		closestPoint = label.col.ClosestPointOnBounds(blackHoleConsumer.mouthTransform.position);
		labelTarget = Vector3.MoveTowards(closestPoint + Vector3.up * 0.25f + label.transform.right * -0.15f, Camera.main.transform.position, 0.2f);
		meshCenter = label.consumable.transform.TransformPoint(label.consumable.gameObject.GetComponentInChildren<MeshRenderer>().bounds.center);
		ray = new Ray(closestPoint, meshCenter - closestPoint);
		if (label.col.Raycast(ray, out hit, 10f))
		{
			labelOrigin = hit.point;
		}
		else
		{
			labelOrigin = closestPoint;
		}
		_toolGroup.BlackHole.labelOrigin = labelOrigin;
		scaleTarget = ((!label.unconsumableGO.activeSelf) ? Vector3.one : new Vector3(0.2f, 1f, 1f));
		label.transform.position = Vector3.Lerp(label.gameObject.transform.position, labelTarget, 16f * Time.deltaTime);
		label.transform.localScale = Vector3.Lerp(label.transform.localScale, Vector3.one, 20f * Time.deltaTime);
		label.labelBack.transform.localScale = Vector3.Lerp(label.labelBack.transform.localScale, scaleTarget, 20f * Time.deltaTime);
		label.SetStemEndPoint(labelOrigin);
		targetPosition = playerTransform.position;
		label.transform.LookAt(targetPosition);
	}

	private void CheckNearby()
	{
		tastiest = blackHoleConsumer.FindTastiest();
		if (tastiest != null && !tastiest.unconsumable)
		{
			if (label.consumable == null || tastiest != label.consumable || label.currentLabelStatus == VacLabel.labelStatus.insufficient)
			{
				RemakeLabel(tastiest, closestCol);
			}
		}
		else if (label.gameObject.activeSelf)
		{
			label.col = null;
			label.consumable = null;
			HideLabel();
		}
	}

	private void RemakeLabel(VVRConsumable newConsumable, Collider newCol)
	{
		label.consumable = newConsumable;
		label.col = newCol;
		if (newConsumable.unconsumable)
		{
			label.SetLabelStatus(VacLabel.labelStatus.unconsumable);
		}
		else if (newConsumable.currentVacuumValueCapped < 0.01f)
		{
			label.SetLabelStatus(VacLabel.labelStatus.fillValue);
		}
		else if (newConsumable.currentVacuumValueCapped == newConsumable.totalVacuumValue)
		{
			label.SetLabelStatus(VacLabel.labelStatus.consumeValue);
		}
		else
		{
			label.SetLabelStatus(VacLabel.labelStatus.fillOrConsume);
		}
		label.UpdateLabelColors(_toolGroup.EagerTouch.CurrentVacInputStatus(), force: true);
		if (!label.gameObject.activeSelf)
		{
			closestPoint = label.col.ClosestPointOnBounds(blackHoleConsumer.mouthTransform.position);
			label.gameObject.transform.position = closestPoint + Vector3.up * 0.2f;
		}
		label.gameObject.SetActive(value: true);
	}
}
