using System;

[Serializable]
public class GraphicsMode
{
	public float renderScale = 1f;

	public int msaa = 4;

	public int shadowDivider = 1;

	public bool disablePostProcessing;
}
