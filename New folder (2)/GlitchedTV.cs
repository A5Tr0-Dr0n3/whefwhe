using System.Collections;
using UnityEngine;

public class GlitchedTV : MonoBehaviour
{
	[SerializeField]
	private Renderer mRenderer;

	[SerializeField]
	private AnimationCurve glitchCurve;

	[SerializeField]
	private float glitchScaler;

	[SerializeField]
	private GvrAudioSource staticSound;

	private MaterialPropertyBlock block;

	private int propId;

	private void Start()
	{
		propId = Shader.PropertyToID("_BlockScale");
		block = new MaterialPropertyBlock();
		block.SetFloat(propId, 0f);
		mRenderer.SetPropertyBlock(block);
	}

	public void StartGlitchAnimation(float duration)
	{
		GlobalCoroutineContainer.AddThread(GlitchAnimationCoroutine(duration), this);
	}

	private IEnumerator GlitchAnimationCoroutine(float duration)
	{
		staticSound.Play();
		MaterialPropertyBlock block = new MaterialPropertyBlock();
		float timer = 0f;
		while (timer <= 1f)
		{
			timer += Time.deltaTime / duration;
			block.SetFloat(value: glitchCurve.Evaluate(timer) * glitchScaler, nameID: propId);
			mRenderer.SetPropertyBlock(block);
			yield return null;
		}
		staticSound.Stop();
		gameObject.SetActive(value: false);
	}
}
