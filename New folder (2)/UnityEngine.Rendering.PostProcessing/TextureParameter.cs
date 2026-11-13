using System;

namespace UnityEngine.Rendering.PostProcessing;

[Serializable]
public sealed class TextureParameter : ParameterOverride<Texture>
{
	public TextureParameterDefault defaultState = TextureParameterDefault.Black;

	public override void Interp(Texture from, Texture to, float t)
	{
		if (from == null && to == null)
		{
			value = null;
			return;
		}
		if (from != null && to != null)
		{
			value = TextureLerper.instance.Lerp(from, to, t);
			return;
		}
		Texture texture;
		switch (defaultState)
		{
		case TextureParameterDefault.Black:
			texture = RuntimeUtilities.blackTexture;
			break;
		case TextureParameterDefault.White:
			texture = RuntimeUtilities.whiteTexture;
			break;
		case TextureParameterDefault.Transparent:
			texture = RuntimeUtilities.transparentTexture;
			break;
		case TextureParameterDefault.Lut2D:
		{
			int size = ((!(from != null)) ? to.height : from.height);
			texture = RuntimeUtilities.GetLutStrip(size);
			break;
		}
		default:
			texture = null;
			break;
		}
		if (from == null)
		{
			from = texture;
		}
		if (to == null)
		{
			to = texture;
		}
		if (from == null || to == null)
		{
			base.Interp(from, to, t);
		}
		else
		{
			value = TextureLerper.instance.Lerp(from, to, t);
		}
	}
}
