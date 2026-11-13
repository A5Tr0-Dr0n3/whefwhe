using System.Collections;
using UnityEngine;

public class VVR_Underhub_06_TenderOffice_Credits : SceneBehaviorManager
{
	[SerializeField]
	[HideInInspector]
	private Rigidbody[] Rigidbodies;

	[SerializeField]
	[HideInInspector]
	private Vector3[] InitialObjectPositions;

	[SerializeField]
	[HideInInspector]
	private Quaternion[] InitialObjectRotations;

	[SerializeField]
	[HideInInspector]
	private Transform[] FixedJointTransforms;

	[SerializeField]
	[HideInInspector]
	private Vector3[] foundObjectPositions;

	[SerializeField]
	[HideInInspector]
	private Quaternion[] foundObjectRotations;

	public AnimationCurve ResetObjectCurve;

	public float ResetObjectDuration;

	[SerializeField]
	private FreeDrawStylus mStylus;

	protected override void OnSceneActivated()
	{
		base.OnSceneActivated();
		if (EagerScenes.instance.lastHeadsetRemoved == null && EagerScenes.instance.headsets.Count > 0)
		{
			EagerScenes.instance.lastHeadsetRemoved = EagerScenes.instance.headsets[EagerScenes.instance.headsets.Count - 1];
		}
		GlobalCoroutineContainer.AddThread(resetTransforms(), this);
		mStylus.UndrawAllTrails();
	}

	private void ResetObjectTransforms()
	{
		for (int i = 0; i < Rigidbodies.Length; i++)
		{
			Rigidbody rigidbody = Rigidbodies[i];
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
			rigidbody.transform.position = InitialObjectPositions[i];
			rigidbody.transform.rotation = InitialObjectRotations[i];
		}
		for (int j = 0; j < FixedJointTransforms.Length; j++)
		{
			FixedJoint component = FixedJointTransforms[j].GetComponent<FixedJoint>();
			if (component == null)
			{
				component = FixedJointTransforms[j].gameObject.AddComponent<FixedJoint>();
				component.breakForce = 20f;
			}
		}
	}

	public void CacheObjectTransforms()
	{
		Rigidbodies = Object.FindObjectsOfType<Rigidbody>();
		InitialObjectPositions = new Vector3[Rigidbodies.Length];
		InitialObjectRotations = new Quaternion[Rigidbodies.Length];
		for (int i = 0; i < Rigidbodies.Length; i++)
		{
			ref Vector3 reference = ref InitialObjectPositions[i];
			reference = Rigidbodies[i].transform.position;
			ref Quaternion reference2 = ref InitialObjectRotations[i];
			reference2 = Rigidbodies[i].transform.rotation;
		}
		FixedJoint[] array = Object.FindObjectsOfType<FixedJoint>();
		FixedJointTransforms = new Transform[array.Length];
		for (int j = 0; j < array.Length; j++)
		{
			FixedJointTransforms[j] = array[j].transform;
		}
		foundObjectPositions = new Vector3[Rigidbodies.Length];
		foundObjectRotations = new Quaternion[Rigidbodies.Length];
	}

	private void Update()
	{
		for (int i = 0; i < ToolManager.ActiveToolGroups.Count; i++)
		{
			if (ToolManager.ActiveToolGroups[i].EagerTouch.GetToolSwitchDetected() || ToolManager.ActiveToolGroups[i].EagerTouch.GetToolSwitchDetected(left: false))
			{
				ObjectBase heldObjectBase = ToolManager.ActiveToolGroups[i].Manipulator.HeldObjectBase;
				if (heldObjectBase is ObjectLeafBlower || heldObjectBase is FreeDrawStylus)
				{
					ToolManager.ActiveToolGroups[i].Manipulator.Drop();
				}
			}
		}
	}

	private IEnumerator resetTransforms()
	{
		for (int i = 0; i < Rigidbodies.Length; i++)
		{
			ref Vector3 reference = ref foundObjectPositions[i];
			reference = Rigidbodies[i].transform.position;
			ref Quaternion reference2 = ref foundObjectRotations[i];
			reference2 = Rigidbodies[i].transform.rotation;
			Rigidbodies[i].isKinematic = true;
			Rigidbodies[i].velocity = Vector3.zero;
			Rigidbodies[i].angularVelocity = Vector3.zero;
			Rigidbodies[i].detectCollisions = false;
		}
		float timer = 0f;
		while (timer < ResetObjectDuration)
		{
			timer += Time.deltaTime;
			float t = ResetObjectCurve.Evaluate(Mathf.Clamp01(timer / ResetObjectDuration));
			for (int j = 0; j < Rigidbodies.Length; j++)
			{
				Rigidbodies[j].transform.position = Vector3.LerpUnclamped(foundObjectPositions[j], InitialObjectPositions[j], t);
				Rigidbodies[j].transform.rotation = Quaternion.SlerpUnclamped(foundObjectRotations[j], InitialObjectRotations[j], t);
			}
			yield return null;
		}
		for (int k = 0; k < FixedJointTransforms.Length; k++)
		{
			FixedJoint component = FixedJointTransforms[k].GetComponent<FixedJoint>();
			if (component == null)
			{
				component = FixedJointTransforms[k].gameObject.AddComponent<FixedJoint>();
				component.breakForce = 20f;
			}
		}
		for (int l = 0; l < Rigidbodies.Length; l++)
		{
			Rigidbodies[l].isKinematic = false;
			Rigidbodies[l].velocity = Vector3.zero;
			Rigidbodies[l].angularVelocity = Vector3.zero;
			Rigidbodies[l].detectCollisions = true;
		}
	}
}
