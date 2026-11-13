using UnityEngine;

public class DollhouseRegion : MonoBehaviour
{
	[SerializeField]
	private DollhouseRegionType mType;

	private void OnTriggerExit(Collider col)
	{
		DollhouseObject dollhouseObject = col.transform.parent.GetComponentInChildren<DollhouseDoll>();
		if (dollhouseObject == null)
		{
			dollhouseObject = col.transform.parent.GetComponent<DollhouseObject>();
		}
		if (!(dollhouseObject != null) || ((bool)dollhouseObject.GetComponent<DollhouseDoll>() && dollhouseObject.GetComponent<DollhouseDoll>().mainSequenceState == PlayerDollState.toDiningroom))
		{
			return;
		}
		if (mType == DollhouseRegionType.Dollhouse)
		{
			if (dollhouseObject.currentState == DollhouseObjectState.home || dollhouseObject.isGrabbed)
			{
				dollhouseObject.SetStateOnDelay(DollhouseObjectState.gettingUp, 1.5f);
			}
		}
		else if (mType == DollhouseRegionType.Table)
		{
			if (dollhouseObject.currentState == DollhouseObjectState.home || dollhouseObject.currentState == DollhouseObjectState.onTable || dollhouseObject.currentState == DollhouseObjectState.inactive || dollhouseObject.currentState == DollhouseObjectState.gettingUp)
			{
				dollhouseObject.SetState(DollhouseObjectState.onGround);
			}
		}
		else
		{
			dollhouseObject.SetParentToHoldRoot();
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		DollhouseObject dollhouseObject = col.transform.GetComponent<DollhouseObject>();
		if (dollhouseObject == null)
		{
			dollhouseObject = col.transform.parent.GetComponent<DollhouseObject>();
		}
		else if (dollhouseObject == null)
		{
			dollhouseObject = col.transform.GetComponentInChildren<DollhouseObject>();
		}
		if (!(dollhouseObject != null))
		{
			return;
		}
		if ((bool)dollhouseObject.GetComponent<DollhouseDoll>())
		{
			if (dollhouseObject.GetComponent<DollhouseDoll>().mainSequenceState != PlayerDollState.toDiningroom && mType == DollhouseRegionType.Table && (dollhouseObject.currentState == DollhouseObjectState.jumpToTable || dollhouseObject.isGrabbed))
			{
				dollhouseObject.SetStateOnDelay(DollhouseObjectState.gettingUp, 2f);
			}
			return;
		}
		dollhouseObject.ResetParent();
		if ((dollhouseObject.currentState == DollhouseObjectState.jumpToTable || dollhouseObject.isGrabbed) && mType == DollhouseRegionType.Table)
		{
			dollhouseObject.SetStateOnDelay(DollhouseObjectState.gettingUp, 2f);
		}
	}
}
