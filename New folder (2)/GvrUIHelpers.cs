using UnityEngine;

public static class GvrUIHelpers
{
	public static float GetMetersToCanvasScale(Transform canvasObject)
	{
		Canvas canvas = canvasObject.GetComponentInParent<Canvas>();
		if (canvas == null)
		{
			return 0f;
		}
		if (!canvas.isRootCanvas)
		{
			canvas = canvas.rootCanvas;
		}
		return canvas.transform.localScale.x;
	}
}
