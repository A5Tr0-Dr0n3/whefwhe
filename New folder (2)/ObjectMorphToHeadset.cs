using System.Collections;
using Edelweiss.Coroutine;
using UnityEngine;

[RequireComponent(typeof(ObjectBase))]
[RequireComponent(typeof(TransformShaker))]
[RequireComponent(typeof(Headset))]
public class ObjectMorphToHeadset : MonoBehaviour
{
	[SerializeField]
	private float morphCrossfadeDuration = 1f;

	[SerializeField]
	private float morphSettleDuration = 0.5f;

	[SerializeField]
	private Vector3 maxFlubberIntensity = new Vector3(0.08f, -0.3f, 0.04f);

	[SerializeField]
	private float eyeFlickerPhaseTime = 0.03f;

	[SerializeField]
	private int eyeFlickerCount = 20;

	[SerializeField]
	private GameObject fromObjectMesh;

	[SerializeField]
	private Renderer headsetMeshObj;

	[SerializeField]
	private Renderer headsetEyeRenderer;

	[SerializeField]
	private Material headsetEyeLitUpMat;

	[SerializeField]
	private Material headsetEyeDarkMat;

	private Renderer[] fromObjRenderers;

	private Material[] fromObjMaterials;

	private TransformShaker mTransformShaker;

	private Material headsetMat;

	private int distortionPropertyID;

	[HideInInspector]
	public Headset mHeadsetBehavior;

	[HideInInspector]
	public ObjectBase mObjectBase;

	private Vector3 fromObjMeshStartScale;

	private Vector3 headsetMeshStartScale;

	private float baseRotationShakerAmount;

	private float basePositionShakerAmount;

	private float morphTimer;

	private float morphT;

	private void Awake()
	{
		mTransformShaker = GetComponent<TransformShaker>();
		mObjectBase = GetComponent<ObjectBase>();
		mHeadsetBehavior = GetComponent<Headset>();
		fromObjRenderers = fromObjectMesh.GetComponentsInChildren<Renderer>();
		fromObjMaterials = new Material[fromObjRenderers.Length];
		for (int i = 0; i < fromObjRenderers.Length; i++)
		{
			fromObjMaterials[i] = fromObjRenderers[i].material;
		}
		headsetMat = headsetMeshObj.material;
	}

	private void Start()
	{
		headsetMeshObj.gameObject.SetActive(value: false);
		mHeadsetBehavior.isActivated = false;
		mTransformShaker.enabled = false;
		headsetMeshStartScale = headsetMeshObj.transform.localScale;
		fromObjMeshStartScale = fromObjectMesh.transform.localScale;
		distortionPropertyID = Shader.PropertyToID("_Distortion");
		baseRotationShakerAmount = mTransformShaker.rotation.amount;
		basePositionShakerAmount = mTransformShaker.position.amount;
		Material[] array = fromObjMaterials;
		foreach (Material material in array)
		{
			material.SetVector(distortionPropertyID, Vector3.zero);
		}
		headsetMat.SetVector(distortionPropertyID, Vector3.zero);
	}

	public SafeCoroutine MorphIntoHeadset(bool immediatelyForceOn = true)
	{
		return GlobalCoroutineContainer.AddThread(morphSequence(immediatelyForceOn), this);
	}

	public void MorphIntoHeadset2(bool immediatelyForceOn = true)
	{
		GlobalCoroutineContainer.AddThread(morphSequence(immediatelyForceOn), this);
	}

	private IEnumerator morphSequence(bool immediatelyForceOn)
	{
		headsetMeshObj.gameObject.SetActive(value: true);
		mTransformShaker.SetInitialValues();
		while (morphTimer < morphCrossfadeDuration)
		{
			morphTimer += Time.deltaTime;
			morphT = Mathf.Clamp01(morphTimer / morphCrossfadeDuration);
			Vector3 flubberValue = Vector3.Lerp(Vector3.zero, maxFlubberIntensity, morphT);
			headsetMat.SetVector(distortionPropertyID, flubberValue);
			for (int i = 0; i < fromObjMaterials.Length; i++)
			{
				fromObjMaterials[i].SetVector(distortionPropertyID, flubberValue);
			}
			headsetMeshObj.transform.localScale = Vector3.Lerp(Vector3.zero, headsetMeshStartScale, morphT * 2f);
			fromObjectMesh.transform.localScale = Vector3.Lerp(fromObjMeshStartScale, Vector3.zero, morphT * 2f - 1f);
			mTransformShaker.position.amount = basePositionShakerAmount * morphT;
			mTransformShaker.rotation.amount = baseRotationShakerAmount * morphT;
			yield return null;
		}
		morphTimer = 0f;
		fromObjectMesh.SetActive(value: false);
		while (morphTimer <= morphSettleDuration)
		{
			morphTimer += Time.deltaTime;
			float settleT = Mathf.Clamp01(morphTimer / morphSettleDuration);
			mTransformShaker.position.amount = basePositionShakerAmount * (1f - settleT);
			mTransformShaker.rotation.amount = baseRotationShakerAmount * (1f - settleT);
			Vector3 flubberValue2 = Vector3.Lerp(maxFlubberIntensity, Vector3.zero, settleT);
			headsetMat.SetVector(distortionPropertyID, flubberValue2);
			yield return null;
		}
		headsetEyeRenderer.gameObject.SetActive(value: true);
		for (int j = 0; j < eyeFlickerCount; j++)
		{
			if (j % 2 == 0)
			{
				headsetEyeRenderer.material = headsetEyeLitUpMat;
			}
			else
			{
				headsetEyeRenderer.material = headsetEyeDarkMat;
			}
			yield return eyeFlickerPhaseTime;
		}
		headsetEyeRenderer.material = headsetEyeLitUpMat;
		mHeadsetBehavior.isActivated = true;
		if (immediatelyForceOn)
		{
			ForceHeadsetOntoFace();
		}
	}

	public void Reset()
	{
		fromObjectMesh.SetActive(value: true);
		headsetMeshObj.gameObject.SetActive(value: false);
		mHeadsetBehavior.isActivated = false;
	}

	public void ForceHeadsetOntoFace()
	{
		for (int i = 0; i < mObjectBase.grabbedBy.Count; i++)
		{
			mObjectBase.grabbedBy[i].AnimateGrabToLength(0.25f, 0.9f, dropObject: false);
		}
	}
}
