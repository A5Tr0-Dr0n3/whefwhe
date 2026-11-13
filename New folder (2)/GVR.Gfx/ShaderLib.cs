namespace GVR.Gfx;

public static class ShaderLib
{
	public static class Variables
	{
		public const string VECTOR_NEAR_APEX = "_NearApexColor";

		public const string VECTOR_NEAR_HORIZON = "_NearHorizonColor";

		public const string VECTOR_FOG_PARAMS = "_FogParams";

		public const string VECTOR_FAR_APEX = "_FarApexColor";

		public const string VECTOR_FAR_HORIZON = "_FarHorizonColor";

		public const string VECTOR_CUSTOM_PROJECTOR_WORLD_DIR = "_CustomProjectorWorldDir";

		public const string VECTOR_OUTLINE_COLOR = "_OutlineColor";

		public const string SAMPLER2D_MAINTEX = "_MainTex";

		public const string SAMPLER2D_BLENDTEX = "_BlendTex";

		public const string FLOAT_OUTLINE_THICKNESS = "_Thickness";

		public const string MATRIX_CUSTOM_PROJECTOR = "_CustomProjector";

		public const string FLOAT_FISHEYESTRENGTH = "_FishEyeStrength";

		public const string VECTOR_VVR_LIGHT = "_VVRLightColor";

		public const string VECTOR_VVR_SHADOW = "_VVRShadowColor";

		public const string VECTOR_VVR_LIGHT0 = "_VVR_Light0";

		public const string VECTOR_VVR_LIGHT1 = "_VVR_Light1";

		public const string VECTOR_VVR_LIGHT2 = "_VVR_Light2";

		public const string VECTOR_VVR_LIGHT3 = "_VVR_Light3";

		public const string VECTOR_VVR_LIGHTATTEN = "_VVR_LightAtten";

		public const string CUBE_VVR_REFLECTIONMAP = "_VVR_ReflectionMap";
	}

	public static class Keywords
	{
		public const string AO_TEX = "AO_TEX_ON";
	}
}
