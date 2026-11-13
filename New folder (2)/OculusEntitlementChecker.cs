using System.Collections;
using Oculus.Platform;
using UnityEngine;

public class OculusEntitlementChecker : MonoBehaviour
{
	public static OculusEntitlementChecker instance;

	private bool hasntEntitlementChecked;

	private void Start()
	{
		if (instance == null)
		{
			instance = this;
		}
		Object.DontDestroyOnLoad(this);
		StartCoroutine(EntitlementCheck());
	}

	private IEnumerator EntitlementCheck()
	{
		Core.AsyncInitialize("1554270381357502");
		while (!Core.IsInitialized())
		{
			yield return null;
		}
		Entitlements.IsUserEntitledToApplication().OnComplete((Message msg) =>
		{
			if (msg.IsError)
			{
				SafeDebug.LogError("Failed entitlment check");
				UnityEngine.Application.Quit();
			}
			else
			{
				SafeDebug.Log("Passed Entitlement Check");
			}
		});
		yield return null;
	}
}
