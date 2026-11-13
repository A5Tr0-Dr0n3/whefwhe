using UnityEngine;

public class ArchiveBlockyEffect : MonoBehaviour
{
	public Material blockyMat;

	public float startBlock = 0.1f;

	private int blockScalePropID;

	private int fillPropID;

	private Renderer[] oRenderer;

	private MaterialPropertyBlock block;

	[Header("Distance Quality")]
	public float maxBlockyMultiplier;

	public float maxDistance;

	private float currentDistance;

	private Vector3 startPos;

	public float pastDistanceFillMultiplier = 0.1f;

	private ObjectBase myObject;

	private Rigidbody rb;

	private bool affectFillIn;

	private float newFill;

	private float blockScale;

	private void Start()
	{
		startPos = transform.position;
		block = new MaterialPropertyBlock();
		oRenderer = GetComponentsInChildren<Renderer>();
		if (blockyMat == null)
		{
			blockyMat = oRenderer[0].sharedMaterial;
		}
		myObject = GetComponent<ObjectBase>();
		rb = GetComponent<Rigidbody>();
		updateMaterialPropIDs();
		blockScale = startBlock;
		updateBlock(firstUpdate: true);
	}

	private void Update()
	{
		if (myObject.isGrabbed)
		{
			currentDistance = Vector3.SqrMagnitude(transform.position - startPos);
			if (currentDistance > maxDistance)
			{
				affectFillIn = true;
			}
			else
			{
				affectFillIn = false;
			}
			updateBlock();
		}
		else if (rb.velocity.sqrMagnitude > 0.1f)
		{
			currentDistance = Vector3.SqrMagnitude(transform.position - startPos);
			if (currentDistance > maxDistance)
			{
				affectFillIn = true;
			}
			else
			{
				affectFillIn = false;
			}
			updateBlock();
		}
	}

	private void updateBlock(bool firstUpdate = false)
	{
		if (!firstUpdate)
		{
			if (affectFillIn)
			{
				newFill = 1f - pastDistanceFillMultiplier * (currentDistance - maxDistance);
				if (newFill < 0f)
				{
					newFill = 0f;
				}
				block.SetFloat(blockScalePropID, blockScale * maxBlockyMultiplier);
			}
			else
			{
				newFill = 1f;
				if (currentDistance > 0f)
				{
					block.SetFloat(blockScalePropID, blockScale * Mathf.Lerp(1f, maxBlockyMultiplier, currentDistance / maxDistance));
				}
				else
				{
					block.SetFloat(blockScalePropID, blockScale);
				}
			}
			block.SetFloat(fillPropID, newFill);
		}
		else
		{
			newFill = 1f;
			block.SetFloat(fillPropID, newFill);
			block.SetFloat(blockScalePropID, blockScale);
		}
		for (int i = 0; i < oRenderer.Length; i++)
		{
			oRenderer[i].SetPropertyBlock(block);
		}
	}

	private void updateMaterialPropIDs()
	{
		blockScalePropID = Shader.PropertyToID("_BlockScale");
		fillPropID = Shader.PropertyToID("_SuckUp");
	}
}
