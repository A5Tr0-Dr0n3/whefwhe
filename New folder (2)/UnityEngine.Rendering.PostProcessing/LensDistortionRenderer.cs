using System;

namespace UnityEngine.Rendering.PostProcessing;

public sealed class LensDistortionRenderer : PostProcessEffectRenderer<LensDistortion>
{
	public override void Render(PostProcessRenderContext context)
	{
		PropertySheet uberSheet = context.uberSheet;
		float val = 1.6f * Math.Max(Mathf.Abs(settings.intensity.value), 1f);
		float num = (float)Math.PI / 180f * Math.Min(160f, val);
		float y = 2f * Mathf.Tan(num * 0.5f);
		Vector4 value = new Vector4(settings.centerX.value, settings.centerY.value, Mathf.Max(settings.intensityX.value, 0.0001f), Mathf.Max(settings.intensityY.value, 0.0001f));
		Vector4 value2 = new Vector4((!(settings.intensity.value >= 0f)) ? (1f / num) : num, y, 1f / settings.scale.value, settings.intensity.value);
		uberSheet.EnableKeyword("DISTORT");
		uberSheet.properties.SetVector(ShaderIDs.Distortion_CenterScale, value);
		uberSheet.properties.SetVector(ShaderIDs.Distortion_Amount, value2);
	}
}
