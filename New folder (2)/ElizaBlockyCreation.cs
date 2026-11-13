using UnityEngine;

public class ElizaBlockyCreation : MonoBehaviour
{
	public Material blockyMat;

	public Material normalMat;

	public bool randomizeDuration;

	public float animationSeconds = 1f;

	public float startBlock = 0.1f;

	public float endBlock = 10f;

	private float endBlockAmbient = 8f;

	private float endBlockCompleteStart = 2f;

	public float lengthRandomMin = 0.5f;

	public float lengthRandomMax = 1.5f;

	private int blockScalePropID;

	public Renderer oRenderer;

	private MaterialPropertyBlock block;

	public bool animateOnEnable = true;

	public bool destroyOnComplete = true;

	private float blockScale;

	private float animationProgress;

	private bool animating;

	private bool animatingOut;

	private bool animatingPartial;

	private bool animatingComplete;

	private float updateThreshold = 0.05f;

	private float updateTimer = 999f;

	private float animationProgressLerp;

	private float randomBlock;

	private void Start()
	{
		block = new MaterialPropertyBlock();
		if (normalMat == null)
		{
			normalMat = oRenderer.sharedMaterial;
		}
		if (blockyMat == null)
		{
			blockyMat = oRenderer.sharedMaterial;
		}
		if (animateOnEnable)
		{
			oRenderer.sharedMaterial = blockyMat;
			updateMaterialPropIDs();
		}
	}

	private void Awake()
	{
		if (randomizeDuration)
		{
			animationSeconds = Random.Range(lengthRandomMin, lengthRandomMax);
		}
		oRenderer = GetComponentInChildren<Renderer>();
	}

	private void OnEnable()
	{
		if (animateOnEnable)
		{
			AnimateIn();
		}
	}

	public void Reanimate(float seconds)
	{
		oRenderer.sharedMaterial = blockyMat;
		updateMaterialPropIDs();
		animationProgress = 0f;
		AnimateIn(seconds);
	}

	public void ReanimatePartial(float seconds)
	{
		oRenderer.sharedMaterial = blockyMat;
		updateMaterialPropIDs();
		animationProgress = 0f;
		AnimateInPartial(seconds);
	}

	public void ReanimateComplete(float seconds)
	{
		oRenderer.sharedMaterial = blockyMat;
		updateMaterialPropIDs();
		animationProgress = 0f;
		AnimateInComplete(seconds);
	}

	public void ReanimateOut(float seconds)
	{
		oRenderer.sharedMaterial = blockyMat;
		updateMaterialPropIDs();
		animationProgress = 0f;
		AnimateOut(seconds);
	}

	public void AnimateIn(float seconds)
	{
		if (seconds > 0.001f)
		{
			animationSeconds = seconds;
		}
		AnimateIn();
	}

	public void AnimateInPartial(float seconds)
	{
		if (seconds > 0.001f)
		{
			animationSeconds = seconds;
		}
		AnimateInPartial();
	}

	public void AnimateInComplete(float seconds)
	{
		if (seconds > 0.001f)
		{
			animationSeconds = seconds;
		}
		AnimateInComplete();
	}

	public void AnimateOut(float seconds)
	{
		if (seconds > 0.001f)
		{
			animationSeconds = seconds;
		}
		AnimateOut();
	}

	public void AnimateIn()
	{
		animating = true;
	}

	public void AnimateInPartial()
	{
		animatingPartial = true;
	}

	public void AnimateInComplete()
	{
		animatingComplete = true;
	}

	public void AnimateOut()
	{
		animatingOut = true;
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
				if (destroyOnComplete)
				{
					Object.Destroy(this);
				}
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
		if (animatingOut)
		{
			updateTimer += Time.deltaTime;
			animationProgress += Time.deltaTime;
			if (animationProgress > animationSeconds)
			{
				blockScale = startBlock;
				animatingOut = false;
				oRenderer.sharedMaterial = normalMat;
				block.Clear();
				oRenderer.SetPropertyBlock(block);
				if (destroyOnComplete)
				{
					Object.Destroy(this);
				}
			}
			if (updateTimer > updateThreshold)
			{
				animationProgressLerp = animationProgress / animationSeconds;
				randomBlock = Random.Range(-0.3f * animationProgress, 0.3f * animationProgress);
				blockScale = MathHelpers.LinMap(0f, 1f, endBlock, startBlock, animationProgressLerp * animationProgressLerp + randomBlock);
				updateTimer = 0f;
				updateThreshold = 0.05f;
				updateBlock();
			}
		}
		if (animatingPartial)
		{
			updateTimer += Time.deltaTime;
			animationProgress += Time.deltaTime;
			if (animationProgress > animationSeconds)
			{
				blockScale = endBlockAmbient;
				animatingPartial = false;
			}
			if (updateTimer > updateThreshold)
			{
				animationProgressLerp = animationProgress / animationSeconds;
				randomBlock = Random.Range(-0.3f * animationProgress, 0.3f * animationProgress);
				blockScale = MathHelpers.LinMap(0f, 1f, startBlock, endBlockAmbient, animationProgressLerp * animationProgressLerp + randomBlock);
				updateTimer = 0f;
				updateThreshold = 0.05f;
				updateBlock();
			}
		}
		if (!animatingComplete)
		{
			return;
		}
		updateTimer += Time.deltaTime;
		animationProgress += Time.deltaTime;
		if (animationProgress > animationSeconds)
		{
			blockScale = endBlock;
			animatingComplete = false;
			oRenderer.sharedMaterial = normalMat;
			block.Clear();
			oRenderer.SetPropertyBlock(block);
			if (destroyOnComplete)
			{
				Object.Destroy(this);
			}
		}
		if (updateTimer > updateThreshold)
		{
			animationProgressLerp = animationProgress / animationSeconds;
			randomBlock = Random.Range(-0.3f * animationProgress, 0.3f * animationProgress);
			blockScale = MathHelpers.LinMap(0f, 1f, endBlockCompleteStart, endBlock, animationProgressLerp * animationProgressLerp + randomBlock);
			updateTimer = 0f;
			updateThreshold = 0.05f;
			updateBlock();
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
