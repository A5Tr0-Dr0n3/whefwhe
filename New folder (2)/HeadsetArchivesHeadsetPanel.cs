using System.Collections;
using UnityEngine;

public class HeadsetArchivesHeadsetPanel : MonoBehaviour
{
	[SerializeField]
	private Transform panelDoor;

	[SerializeField]
	private GameObject headset;

	[SerializeField]
	private AnimationCurve retractAnimation;

	[SerializeField]
	private float retractTime = 0.5f;

	[SerializeField]
	private float retractDistance = 0.025f;

	private Vector3 panelStartPosition;

	private void Awake()
	{
		panelStartPosition = panelDoor.position;
		headset.SetActive(value: false);
	}

	public void RevealHeadset()
	{
		headset.SetActive(value: true);
		GlobalCoroutineContainer.AddThread(retractInAnimation(), this);
	}

	private IEnumerator retractInAnimation()
	{
		Collider[] componentsInChildren = panelDoor.GetComponentsInChildren<Collider>();
		foreach (Collider collider in componentsInChildren)
		{
			collider.enabled = false;
		}
		float timer = 0f;
		Vector3 retractedPosition = panelStartPosition + Vector3.forward * retractDistance;
		while (timer < retractTime)
		{
			timer += Time.deltaTime;
			float t = timer / retractTime;
			float animatedT = retractAnimation.Evaluate(t);
			panelDoor.position = panelStartPosition * (1f - animatedT) + retractedPosition * animatedT;
			yield return null;
		}
		panelDoor.position = retractedPosition;
		panelDoor.gameObject.SetActive(value: false);
	}
}
