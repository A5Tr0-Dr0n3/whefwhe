using StixGames;
using UnityEngine;

public class GrassRegenerator : MonoBehaviour
{
	public float regenerationSpeed = 1f;

	private Texture2D texture;

	private readonly Color originalColor = new Color(0.5f, 0.5f, 1f, 1f);

	private void Update()
	{
		if (texture == null)
		{
			texture = GrassManipulationUtility.GetGrassTexture(transform, getColorHeight: false);
		}
		Color[] pixels = texture.GetPixels();
		for (int i = 0; i < pixels.Length; i++)
		{
			ref Color reference = ref pixels[i];
			reference = Color.Lerp(pixels[i], originalColor, regenerationSpeed * Time.deltaTime);
		}
		texture.SetPixels(pixels);
		TextureUpdater textureUpdater = GetComponent<TextureUpdater>();
		if (textureUpdater == null)
		{
			textureUpdater = gameObject.AddComponent<TextureUpdater>();
			textureUpdater.targetTexture = texture;
		}
		textureUpdater.RequestTextureUpdate();
	}
}
