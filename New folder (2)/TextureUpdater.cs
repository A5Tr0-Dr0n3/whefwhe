using UnityEngine;

public class TextureUpdater : MonoBehaviour
{
	public Texture2D targetTexture;

	private bool apply;

	private void LateUpdate()
	{
		if (apply)
		{
			targetTexture.Apply(updateMipmaps: false);
			apply = false;
		}
	}

	public void RequestTextureUpdate()
	{
		apply = true;
	}
}
