namespace UnityEngine.Rendering.PostProcessing;

public sealed class VignetteRenderer : PostProcessEffectRenderer<Vignette>
{
	public override void Render(PostProcessRenderContext context)
	{
		PropertySheet uberSheet = context.uberSheet;
		uberSheet.EnableKeyword("VIGNETTE");
		uberSheet.properties.SetColor(ShaderIDs.Vignette_Color, settings.color.value);
		if ((VignetteMode)settings.mode == VignetteMode.Classic)
		{
			uberSheet.properties.SetFloat(ShaderIDs.Vignette_Mode, 0f);
			uberSheet.properties.SetVector(ShaderIDs.Vignette_Center, settings.center.value);
			float z = (1f - settings.roundness.value) * 6f + settings.roundness.value;
			uberSheet.properties.SetVector(ShaderIDs.Vignette_Settings, new Vector4(settings.intensity.value * 3f, settings.smoothness.value * 5f, z, (!settings.rounded.value) ? 0f : 1f));
		}
		else
		{
			uberSheet.properties.SetFloat(ShaderIDs.Vignette_Mode, 1f);
			uberSheet.properties.SetTexture(ShaderIDs.Vignette_Mask, settings.mask.value);
			uberSheet.properties.SetFloat(ShaderIDs.Vignette_Opacity, Mathf.Clamp01(settings.opacity.value));
		}
	}
}
