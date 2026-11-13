using UnityEngine;

public class HeadsetFaceGrappleModule : MonoBehaviour
{
	[SerializeField]
	private float minFaceGrappleTime = 1f;

	[SerializeField]
	private float maxFaceGrappleTime = 3f;

	[SerializeField]
	private Vector3 faceGrappleTargetOffset;

	[SerializeField]
	private FaceGrappleTentacle[] tentacles;

	private FaceGrapplerState brainState = FaceGrapplerState.searching;

	private float grappleProgress;

	private float targetGrappleProgress;

	private void Start()
	{
		StartGrapple();
	}

	private void Update()
	{
		checkDebugKeys();
		switch (brainState)
		{
		case FaceGrapplerState.grappling:
			updateGrapple();
			break;
		case FaceGrapplerState.searching:
			break;
		case FaceGrapplerState.disabled:
			break;
		}
	}

	private void checkDebugKeys()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			StartGrapple();
		}
	}

	private void StartGrapple()
	{
		brainState = FaceGrapplerState.grappling;
		targetGrappleProgress = Random.Range(minFaceGrappleTime, maxFaceGrappleTime);
		grappleProgress = 0f;
	}

	private void updateGrapple()
	{
		grappleProgress += Time.deltaTime;
		grappleProgress = Mathf.Clamp(grappleProgress, 0f, targetGrappleProgress);
		float progressT = grappleProgress / targetGrappleProgress;
		Vector3 position = EagerMaster.instance.mainCamera.transform.position;
		for (int i = 0; i < tentacles.Length; i++)
		{
			tentacles[i].UpdateTentaclePositions(position, progressT);
		}
	}
}
