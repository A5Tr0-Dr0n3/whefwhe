using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

public class HeadsetWrangler : MonoBehaviour
{
	public float regrabDistance = 0.5f;

	protected float sqrRegrab;

	protected Transform hTransform;

	public GameObject headsetGO;

	protected Rigidbody headsetRB;

	public float regrabTime = 3f;

	public float regrabSpeed = 3f;

	public float reSpawnTime = 6f;

	protected Vector3 initialHPos;

	protected Quaternion initialHRot;

	protected bool cachedIsKinematic;

	private float sqrDist;

	private float timer;

	private float reSpawnTimer;

	private SafeCoroutine animateBackRoutine;

	protected virtual void Start()
	{
		headsetRB = headsetGO.GetComponent<Rigidbody>();
		sqrRegrab = regrabDistance * regrabDistance;
		CalibrateDesiredPoint();
	}

	public virtual void CalibrateDesiredPoint()
	{
		hTransform = headsetGO.transform;
		initialHPos = hTransform.position;
		initialHRot = hTransform.rotation;
	}

	protected virtual void Update()
	{
		sqrDist = Vector3.SqrMagnitude(initialHPos - hTransform.position);
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
		else
		{
			timer = 0f;
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
		headsetRB.detectCollisions = false;
		while (sqrDist > 0.001f && !ToolManager.PlayerGrabbingGameObject(headsetGO))
		{
			hTransform.position = Vector3.Lerp(hTransform.position, initialHPos, Time.deltaTime * regrabSpeed);
			hTransform.rotation = Quaternion.Lerp(hTransform.rotation, initialHRot, Time.deltaTime * regrabSpeed);
			yield return null;
		}
		animateBackRoutine = null;
		headsetRB.isKinematic = cachedIsKinematic;
		headsetRB.detectCollisions = true;
	}
}
