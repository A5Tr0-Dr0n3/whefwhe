using UnityEngine;

public class ObjectToast : ObjectBase
{
	private Coroutine poolCoroutine;

	[HideInInspector]
	public ObjectToaster.Toast toastState;

	private VVRConsumable mConsumable;

	protected override void Start()
	{
		base.Start();
		mConsumable = GetComponent<VVRConsumable>();
		VVRConsumable.OnVacuumedUpEvent += onConsumed;
	}

	private void onConsumed(VVRConsumable consumedObject)
	{
		if (consumedObject == mConsumable)
		{
			KitchenToastManager.RemoveToast(this);
		}
	}
}
