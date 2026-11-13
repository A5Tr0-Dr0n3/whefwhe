using UnityEngine;

public class BlockyCreation : MonoBehaviour
{
	public Material blockyMat;

	public Material normalMat;

	public bool randomizeDuration;

	public float animationSeconds = 1f;

	public float startBlock = 0.1f;

	public float endBlock = 10f;

	public float lengthRandomMin = 0.5f;

	public float lengthRandomMax = 1.5f;

	private int blockScalePropID;

	private Renderer oRenderer;

	private MaterialPropertyBlock block;

	private float blockScale;

	private float animationProgress;

	private bool animating;

	private float updateThreshold = 0.05f;

	private float updateTimer = 999f;

	private float animationProgressLerp;

	private float randomBlock;

	private void Start()
	{
		block = new MaterialPropertyBlock();
		oRenderer = GetComponentInChildren<Renderer>();
		if (normalMat == null)
		{
			normalMat = oRenderer.sharedMaterial;
		}
		if (blockyMat == null)
		{
			blockyMat = oRenderer.sharedMaterial;
		}
		oRenderer.sharedMaterial = blockyMat;
		updateMaterialPropIDs();
	}

	private void Awake()
	{
		if (randomizeDuration)
		{
			animationSeconds = Random.Range(lengthRandomMin, lengthRandomMax);
		}
	}

	private void OnEnable()
	{
		AnimateIn();
	}

	public void AnimateIn(float seconds)
	{
		if (seconds > 0.001f)
		{
			animationSeconds = seconds;
		}
		AnimateIn();
	}

	public void AnimateIn()
	{
		animating = true;
	}

	private void Update()
	{
		if (animating)
		{
			updateTimer += Time.deltaTime;
			animationProgress += Time.deltaTime;
			if (animationProgress > animationSeconds)
			{
				blockScale = endBlock;
				animating = false;
				oRenderer.sharedMaterial = normalMat;
				block.Clear();
				oRenderer.SetPropertyBlock(block);
				Object.Destroy(this);
			}
			if (updateTimer > updateThreshold)
			{
				animationProgressLerp = animationProgress / animationSeconds;
				randomBlock = Random.Range(-0.3f * animationProgress, 0.3f * animationProgress);
				blockScale = MathHelpers.LinMap(0f, 1f, startBlock, endBlock, animationProgressLerp * animationProgressLerp + randomBlock);
				updateTimer = 0f;
				updateThreshold = 0.05f;
				updateBlock();
			}
		}
	}

	private void updateBlock()
	{
		block.SetFloat(blockScalePropID, blockScale);
		oRenderer.SetPropertyBlock(block);
	}

	private void updateMaterialPropIDs()
	{
		blockScalePropID = Shader.PropertyToID("_BlockScale");
	}
}
