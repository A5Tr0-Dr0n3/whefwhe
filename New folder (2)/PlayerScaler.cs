using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class PlayerScaler : MonoBehaviour
{
	public static PlayerScaler instance;

	public Transform player;

	private Transform cameraTransform;

	[HideInInspector]
	public bool scaled;

	[HideInInspector]
	public Vector3 currentScale = Vector3.one;

	private Vector3 startPlayerPos;

	private Vector3 intendedCameraPos;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			return;
		}
		SafeDebug.LogError("Player Scaler instance already exists");
		Object.Destroy(gameObject);
	}

	private void Start()
	{
		cameraTransform = EagerMaster.instance.mainCamera.transform;
	}

	public void ScaleHead(Vector3 value)
	{
		if (value == Vector3.one)
		{
			scaled = false;
		}
		else
		{
			scaled = true;
		}
		currentScale = value;
		player.localScale = value;
	}

	public IEnumerator ScaleHeadOverTime(Vector3 value, float time)
	{
		Vector3 start = player.localScale;
		startPlayerPos = player.localPosition;
		float childOffsetX = (EagerMaster.instance.player.transform.localRotation * cameraTransform.localPosition).x;
		float childOffsetZ = (EagerMaster.instance.player.transform.localRotation * cameraTransform.localPosition).z;
		float projectedChildX = childOffsetX * value.x;
		float projectedChildZ = childOffsetZ * value.z;
		if (value == Vector3.one)
		{
			scaled = false;
		}
		else
		{
			scaled = true;
		}
		float t = 0f;
		while (t <= 1f)
		{
			t += Time.deltaTime / time;
			ScaleHead(Vector3.Lerp(start, value, t));
			float xTarget = Mathf.Lerp(0f, projectedChildX, t);
			float zTarget = Mathf.Lerp(0f, projectedChildZ, t);
			player.localPosition = new Vector3(startPlayerPos.x - xTarget, player.localPosition.y, startPlayerPos.z - zTarget);
			yield return null;
		}
	}

	public SafeCoroutine ScaleHead(Vector3 value, float time)
	{
		return GlobalCoroutineContainer.AddThread(ScaleHeadOverTime(value, time), this);
	}
}
