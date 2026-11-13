using UnityEngine;

public static class TransformExtensions
{
	public static void SetLayer(this Transform trans, int layer)
	{
		trans.gameObject.layer = layer;
		foreach (Transform tran in trans)
		{
			tran.SetLayer(layer);
		}
	}
}
