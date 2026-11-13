using System;
using UnityEngine;

namespace StixGames;

public static class GrassManipulationUtility
{
	public static Texture2D GetGrassTexture(Transform target, bool getColorHeight)
	{
		Material material = target.GetComponent<Renderer>().material;
		Texture2D texture2D = material.GetTexture((!getColorHeight) ? "_Displacement" : "_ColorMap") as Texture2D;
		if (texture2D == null)
		{
			throw new InvalidOperationException("You have to add a texture (or texture-adder component) if you want to use dynamic grass.");
		}
		return texture2D;
	}

	public static void Invert2x2Matrix(Vector2 forward, Vector2 right, out Vector2 inverseForward, out Vector2 inverseRight)
	{
		float num = right.x * forward.y - right.y * forward.x;
		inverseRight = new Vector2(forward.y, 0f - right.y) / num;
		inverseForward = new Vector2(0f - forward.x, right.x) / num;
	}

	public static Transform GetWorldToTextureSpaceMatrix(Vector3 pos, Vector3 rayDir, float maxDistance, float offset, LayerMask layerMask, out Vector2 texCoord, out Vector2 forward, out Vector2 right)
	{
		Ray ray = new Ray(pos, rayDir.normalized);
		Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);
		if (Physics.Raycast(ray, out var hitInfo, maxDistance, layerMask))
		{
			texCoord = hitInfo.textureCoord;
			forward = GetTexCoordDifference(hitInfo.transform, pos, texCoord, Vector3.forward * offset, maxDistance, layerMask);
			right = GetTexCoordDifference(hitInfo.transform, pos, texCoord, Vector3.right * offset, maxDistance, layerMask);
			return hitInfo.transform;
		}
		texCoord = Vector2.zero;
		forward = Vector2.zero;
		right = Vector2.zero;
		return null;
	}

	public static bool GetWorldToTextureSpaceMatrix(Ray ray, float offset, Collider collider, out RaycastHit hit, out Vector2 forward, out Vector2 right)
	{
		if (collider.Raycast(ray, out hit, float.PositiveInfinity))
		{
			forward = GetTexCoordDifference(ray.origin, hit.textureCoord, Vector3.forward * offset, collider);
			right = GetTexCoordDifference(ray.origin, hit.textureCoord, Vector3.right * offset, collider);
			return true;
		}
		hit = default;
		forward = Vector2.zero;
		right = Vector2.zero;
		return false;
	}

	private static Vector2 GetTexCoordDifference(Transform targetObject, Vector3 pos, Vector2 texCoords, Vector3 offset, float maxDistance, LayerMask layerMask)
	{
		Ray ray = new Ray(pos + offset, Vector3.down);
		if (Physics.Raycast(ray, out var hitInfo, maxDistance, layerMask) && hitInfo.transform == targetObject)
		{
			return (hitInfo.textureCoord - texCoords) / offset.magnitude;
		}
		ray.direction = -ray.direction;
		if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMask) && hitInfo.transform == targetObject)
		{
			return (texCoords - hitInfo.textureCoord) / offset.magnitude;
		}
		return Vector2.zero;
	}

	private static Vector2 GetTexCoordDifference(Vector3 pos, Vector2 texCoords, Vector3 offset, Collider collider)
	{
		Ray ray = new Ray(pos + offset, Vector3.down);
		if (collider.Raycast(ray, out var hitInfo, float.PositiveInfinity))
		{
			return (hitInfo.textureCoord - texCoords) / offset.magnitude;
		}
		ray.direction = -ray.direction;
		if (collider.Raycast(ray, out hitInfo, float.PositiveInfinity))
		{
			return (texCoords - hitInfo.textureCoord) / offset.magnitude;
		}
		return Vector2.zero;
	}

	public static Vector2 VectorToColorSpace(Vector2 vec)
	{
		return (vec + Vector2.one) * 0.5f;
	}

	public static Vector2 ColorToVectorSpace(Vector2 vec)
	{
		return vec * 2f - Vector2.one;
	}
}
