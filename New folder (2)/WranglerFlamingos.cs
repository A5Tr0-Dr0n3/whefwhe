using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class WranglerFlamingos : MonoBehaviour
{
	public float regrabDistance = 0.5f;

	private float sqrRegrab;

	private Transform hTransform;

	public GameObject headsetGO;

	private Rigidbody headsetRB;

	public float regrabTime = 3f;

	public float regrabSpeed = 3f;

	public float reSpawnTime = 6f;

	private bool hasBeenGrabbed;

	private Vector3 initialHPos;

	private Quaternion initialHRot;

	private bool cachedIsKinematic;

	private float sqrDist;

	private float timer;

	private float reSpawnTimer;

	private SafeCoroutine animateBackRoutine;

	private void Start()
	{
		headsetRB = headsetGO.GetComponent<Rigidbody>();
		sqrRegrab = regrabDistance * regrabDistance;
		CalibrateDesiredPoint();
	}

	public void CalibrateDesiredPoint()
	{
		hTransform = headsetGO.transform;
		initialHPos = hTransform.localPosition;
		initialHRot = hTransform.localRotation;
	}

	private void Update()
	{
		if (ToolManager.PlayerGrabbingGameObject(headsetGO))
		{
			hasBeenGrabbed = true;
		}
		if (hasBeenGrabbed)
		{
			sqrDist = Vector3.SqrMagnitude(initialHPos - hTransform.localPosition);
			if (sqrDist > sqrRegrab && !ToolManager.PlayerGrabbingGameObject(headsetGO))
			{
				timer += Time.deltaTime;
				if (timer > regrabTime)
				{
					timer = 0f;
					if (animateBackRoutine != null && animateBackRoutine.IsRunning)
					{
						animateBackRoutine.Stop();
					}
					animateBackRoutine = GlobalCoroutineContainer.AddThread(animateBack(), this);
				}
			}
		}
		else
		{
			hTransform.localPosition = initialHPos;
			hTransform.localRotation = initialHRot;
		}
		if (!headsetGO.activeSelf)
		{
			reSpawnTimer += Time.deltaTime;
			if (reSpawnTimer > reSpawnTime)
			{
				reSpawnTimer = 0f;
				headsetGO.SetActive(value: true);
			}
		}
	}

	private IEnumerator animateBack()
	{
		cachedIsKinematic = headsetRB.isKinematic;
		headsetRB.isKinematic = true;
		while (sqrDist > 0.001f && !ToolManager.PlayerGrabbingGameObject(headsetGO))
		{
			hTransform.localPosition = Vector3.Lerp(hTransform.localPosition, initialHPos, Time.deltaTime * regrabSpeed);
			hTransform.localRotation = Quaternion.Lerp(hTransform.localRotation, initialHRot, Time.deltaTime * regrabSpeed);
			yield return null;
		}
		animateBackRoutine = null;
		headsetRB.isKinematic = cachedIsKinematic;
		hasBeenGrabbed = false;
	}
}
