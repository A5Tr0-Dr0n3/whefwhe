using System;
using StixGames;
using UnityEngine;

public class GrassPainter
{
	private const float EPSILON = 0.01f;

	public int Channel;

	public int BrushMode;

	public float Strength = 1f;

	public float Size = 1f;

	public float Softness = 0.5f;

	public float Spacing = 0.5f;

	public float Rotation;

	public Collider GrassCollider;

	public Texture2D Texture;

	public bool Draw(Ray ray)
	{
		if (Physics.Raycast(ray, out var hitInfo))
		{
			GrassCollider = hitInfo.collider;
			Texture = (Texture2D)hitInfo.collider.GetComponent<Renderer>().material.mainTexture;
			ApplyBrush(hitInfo.point);
			return true;
		}
		return false;
	}

	public void ApplyBrush(Vector3 hitPoint)
	{
		if (!GrassManipulationUtility.GetWorldToTextureSpaceMatrix(new Ray(hitPoint + Vector3.up * 1000f, Vector3.down), 0.01f, GrassCollider, out var hit, out var forward, out var _))
		{
			return;
		}
		Vector2 textureCoord = hit.textureCoord;
		int num = (int)(Size * forward.magnitude * (float)Texture.width);
		Vector2 vector = new Vector2(textureCoord.x * (float)Texture.width, textureCoord.y * (float)Texture.height);
		int num2 = (int)(vector.x - (float)num);
		int num3 = (int)(vector.y - (float)num);
		int num4 = Mathf.Clamp(num2, 0, Texture.width);
		int num5 = Mathf.Clamp(num3, 0, Texture.height);
		int num6 = Mathf.Min(num2 + num * 2, Texture.width) - num2;
		int num7 = Mathf.Min(num3 + num * 2, Texture.height) - num3;
		vector -= new Vector2(num4, num5);
		Color[] pixels = Texture.GetPixels(num4, num5, num6, num7);
		for (int i = 0; i < num7; i++)
		{
			for (int j = 0; j < num6; j++)
			{
				int num8 = i * num6 + j;
				Vector2 v = (new Vector2(j, i) - vector) / num * 0.5f + new Vector2(0.5f, 0.5f);
				ref Color reference = ref pixels[num8];
				reference = ApplyBrushToPixel(pixels[num8], v);
			}
		}
		Texture.SetPixels(num4, num5, num6, num7, pixels);
		Texture.Apply();
	}

	private Color ApplyBrushToPixel(Color c, Vector2 v)
	{
		v -= new Vector2(0.5f, 0.5f);
		v *= 2f;
		float magnitude = v.magnitude;
		float value = SmoothStep(1f, Mathf.Min(1f - Softness, 0.999f), magnitude);
		switch (BrushMode)
		{
		case 0:
			c = ChangeChannel(c, (float x) => x + value * Strength);
			break;
		case 1:
			c = ChangeChannel(c, (float x) => x - value * Strength);
			break;
		case 2:
			c = ChangeChannel(c, (float x) => x * (1f - value) + Strength * value);
			break;
		}
		return c;
	}

	private Color ChangeChannel(Color c, Func<float, float> densityChange)
	{
		switch (Channel)
		{
		case 0:
			c.r = Mathf.Clamp01(densityChange(c.r));
			break;
		case 1:
			c.g = Mathf.Clamp01(densityChange(c.g));
			break;
		case 2:
			c.b = Mathf.Clamp01(densityChange(c.b));
			break;
		case 3:
			c.a = Mathf.Clamp01(densityChange(c.a));
			break;
		default:
			throw new InvalidOperationException("Channel number invalid");
		}
		return c;
	}

	private float SmoothStep(float edge0, float edge1, float x)
	{
		x = Mathf.Clamp((x - edge0) / (edge1 - edge0), 0f, 1f);
		return x * x * (3f - 2f * x);
	}
}
