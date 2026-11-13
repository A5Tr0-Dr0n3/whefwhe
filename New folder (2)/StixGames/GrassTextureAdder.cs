using UnityEngine;

namespace StixGames;

[AddComponentMenu("StixGames/Grass Texture Adder")]
public class GrassTextureAdder : MonoBehaviour
{
	public int size = 1024;

	public bool addColorHeight;

	public bool addDisplacement;

	private void Start()
	{
		if (addColorHeight)
		{
			Texture2D texture2D = new Texture2D(size, size, TextureFormat.ARGB32, mipmap: false, linear: true);
			Color[] array = new Color[size * size];
			for (int i = 0; i < array.Length; i++)
			{
				ref Color reference = ref array[i];
				reference = new Color(1f, 1f, 1f, 1f);
			}
			texture2D.SetPixels(array);
			texture2D.Apply();
			GetComponent<Renderer>().material.SetTexture("_Displacement", texture2D);
		}
		if (addDisplacement)
		{
			Texture2D texture2D2 = new Texture2D(size, size, TextureFormat.ARGB32, mipmap: false, linear: true);
			Color[] array2 = new Color[size * size];
			for (int j = 0; j < array2.Length; j++)
			{
				ref Color reference2 = ref array2[j];
				reference2 = new Color(0.5f, 0.5f, 1f, 1f);
			}
			texture2D2.SetPixels(array2);
			texture2D2.Apply();
			GetComponent<Renderer>().material.SetTexture("_Displacement", texture2D2);
		}
	}
}
