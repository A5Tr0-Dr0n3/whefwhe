using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[Obsolete("GvrAudio is deprecated. Please upgrade to Resonance Audio (https://developers.google.com/resonance-audio/migrate).")]
public static class GvrAudio
{
	public enum Quality
	{
		Stereo,
		Low,
		High
	}

	public enum SpatializerData
	{
		Id,
		Type,
		NumChannels,
		ChannelSet,
		Gain,
		DistanceAttenuation,
		MinDistance,
		ZeroOutput
	}

	public enum SpatializerType
	{
		Source,
		Soundfield
	}

	private struct RoomProperties
	{
		public float positionX;

		public float positionY;

		public float positionZ;

		public float rotationX;

		public float rotationY;

		public float rotationZ;

		public float rotationW;

		public float dimensionsX;

		public float dimensionsY;

		public float dimensionsZ;

		public GvrAudioRoom.SurfaceMaterial materialLeft;

		public GvrAudioRoom.SurfaceMaterial materialRight;

		public GvrAudioRoom.SurfaceMaterial materialBottom;

		public GvrAudioRoom.SurfaceMaterial materialTop;

		public GvrAudioRoom.SurfaceMaterial materialFront;

		public GvrAudioRoom.SurfaceMaterial materialBack;

		public float reflectionScalar;

		public float reverbGain;

		public float reverbTime;

		public float reverbBrightness;
	}

	private static int sampleRate = -1;

	private static int numChannels = -1;

	private static int framesPerBuffer = -1;

	public static readonly Color listenerDirectivityColor = 0.65f * Color.magenta;

	public static readonly Color sourceDirectivityColor = 0.65f * Color.blue;

	public const float distanceEpsilon = 0.01f;

	public const float maxDistanceLimit = 1000000f;

	public const float minDistanceLimit = 990099f;

	public const float maxGainDb = 24f;

	public const float minGainDb = -24f;

	public const float maxReverbBrightness = 1f;

	public const float minReverbBrightness = -1f;

	public const float maxReverbTime = 3f;

	public const float maxReflectivity = 2f;

	public const int maxNumOcclusionHits = 12;

	public const float occlusionDetectionInterval = 0.2f;

	public const int numFoaChannels = 4;

	private static Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

	private static List<GvrAudioRoom> enabledRooms = new List<GvrAudioRoom>();

	private static bool initialized = false;

	private static Transform listenerTransform = null;

	private static RaycastHit[] occlusionHits = new RaycastHit[12];

	private static int occlusionMaskValue = -1;

	private static Matrix4x4 transformMatrix = Matrix4x4.identity;

	private const string pluginName = "audioplugingvrunity";

	public static int SampleRate => sampleRate;

	public static int NumChannels => numChannels;

	public static int FramesPerBuffer => framesPerBuffer;

	public static void Initialize(GvrAudioListener listener, Quality quality)
	{
		if (!initialized)
		{
			AudioConfiguration configuration = AudioSettings.GetConfiguration();
			sampleRate = configuration.sampleRate;
			numChannels = (int)configuration.speakerMode;
			framesPerBuffer = configuration.dspBufferSize;
			if (numChannels != 2)
			{
				Debug.LogError("Only 'Stereo' speaker mode is supported by GVR Audio.");
				return;
			}
			Initialize((int)quality, sampleRate, numChannels, framesPerBuffer);
			listenerTransform = listener.transform;
			initialized = true;
		}
		else if (listener.transform != listenerTransform)
		{
			Debug.LogError("Only one GvrAudioListener component is allowed in the scene.");
			UnityEngine.Object.Destroy(listener);
		}
	}

	public static void Shutdown(GvrAudioListener listener)
	{
		if (initialized && listener.transform == listenerTransform)
		{
			initialized = false;
			Shutdown();
			sampleRate = -1;
			numChannels = -1;
			framesPerBuffer = -1;
			listenerTransform = null;
		}
	}

	public static void UpdateAudioListener(float globalGainDb, LayerMask occlusionMask)
	{
		if (initialized)
		{
			occlusionMaskValue = occlusionMask.value;
			SetListenerGain(ConvertAmplitudeFromDb(globalGainDb));
		}
	}

	public static int CreateAudioSoundfield()
	{
		int result = -1;
		if (initialized)
		{
			result = CreateSoundfield(4);
		}
		return result;
	}

	public static void UpdateAudioSoundfield(int id, GvrAudioSoundfield soundfield)
	{
		if (initialized)
		{
			SetSourceBypassRoomEffects(id, soundfield.bypassRoomEffects);
		}
	}

	public static int CreateAudioSource(bool hrtfEnabled)
	{
		int result = -1;
		if (initialized)
		{
			result = CreateSoundObject(hrtfEnabled);
		}
		return result;
	}

	public static void DestroyAudioSource(int id)
	{
		if (initialized)
		{
			DestroySource(id);
		}
	}

	public static void UpdateAudioSource(int id, GvrAudioSource source, float currentOcclusion)
	{
		if (initialized)
		{
			SetSourceBypassRoomEffects(id, source.bypassRoomEffects);
			SetSourceDirectivity(id, source.directivityAlpha, source.directivitySharpness);
			SetSourceListenerDirectivity(id, source.listenerDirectivityAlpha, source.listenerDirectivitySharpness);
			SetSourceOcclusionIntensity(id, currentOcclusion);
		}
	}

	public static void UpdateAudioRoom(GvrAudioRoom room, bool roomEnabled)
	{
		if (roomEnabled)
		{
			if (!enabledRooms.Contains(room))
			{
				enabledRooms.Add(room);
			}
		}
		else
		{
			enabledRooms.Remove(room);
		}
		if (initialized)
		{
			if (enabledRooms.Count > 0)
			{
				GvrAudioRoom room2 = enabledRooms[enabledRooms.Count - 1];
				RoomProperties roomProperties = GetRoomProperties(room2);
				IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(roomProperties));
				Marshal.StructureToPtr(roomProperties, intPtr, fDeleteOld: false);
				SetRoomProperties(intPtr);
				Marshal.FreeHGlobal(intPtr);
			}
			else
			{
				SetRoomProperties(IntPtr.Zero);
			}
		}
	}

	public static float ComputeOcclusion(Transform sourceTransform)
	{
		float num = 0f;
		if (initialized)
		{
			Vector3 position = listenerTransform.position;
			Vector3 direction = sourceTransform.position - position;
			int num2 = Physics.RaycastNonAlloc(position, direction, occlusionHits, direction.magnitude, occlusionMaskValue);
			for (int i = 0; i < num2; i++)
			{
				if (occlusionHits[i].transform != listenerTransform && occlusionHits[i].transform != sourceTransform)
				{
					num++;
				}
			}
		}
		return num;
	}

	public static float ConvertAmplitudeFromDb(float db)
	{
		return Mathf.Pow(10f, 0.05f * db);
	}

	public static Vector2[] Generate2dPolarPattern(float alpha, float order, int resolution)
	{
		Vector2[] array = new Vector2[resolution];
		float num = (float)Math.PI * 2f / (float)resolution;
		for (int i = 0; i < resolution; i++)
		{
			float f = (float)i * num;
			float num2 = Mathf.Pow(Mathf.Abs(1f - alpha + alpha * Mathf.Cos(f)), order);
			ref Vector2 reference = ref array[i];
			reference = new Vector2(num2 * Mathf.Sin(f), num2 * Mathf.Cos(f));
		}
		return array;
	}

	public static bool IsListenerInsideRoom(GvrAudioRoom room)
	{
		bool result = false;
		if (initialized)
		{
			Vector3 vector = listenerTransform.position - room.transform.position;
			Quaternion quaternion = Quaternion.Inverse(room.transform.rotation);
			bounds.size = Vector3.Scale(room.transform.lossyScale, room.size);
			result = bounds.Contains(quaternion * vector);
		}
		return result;
	}

	private static void ConvertAudioTransformFromUnity(ref Vector3 position, ref Quaternion rotation)
	{
		transformMatrix = Pose3D.FlipHandedness(Matrix4x4.TRS(position, rotation, Vector3.one));
		position = transformMatrix.GetColumn(3);
		rotation = Quaternion.LookRotation(transformMatrix.GetColumn(2), transformMatrix.GetColumn(1));
	}

	private static RoomProperties GetRoomProperties(GvrAudioRoom room)
	{
		Vector3 position = room.transform.position;
		Quaternion rotation = room.transform.rotation;
		Vector3 vector = Vector3.Scale(room.transform.lossyScale, room.size);
		ConvertAudioTransformFromUnity(ref position, ref rotation);
		RoomProperties result = default;
		result.positionX = position.x;
		result.positionY = position.y;
		result.positionZ = position.z;
		result.rotationX = rotation.x;
		result.rotationY = rotation.y;
		result.rotationZ = rotation.z;
		result.rotationW = rotation.w;
		result.dimensionsX = vector.x;
		result.dimensionsY = vector.y;
		result.dimensionsZ = vector.z;
		result.materialLeft = room.leftWall;
		result.materialRight = room.rightWall;
		result.materialBottom = room.floor;
		result.materialTop = room.ceiling;
		result.materialFront = room.frontWall;
		result.materialBack = room.backWall;
		result.reverbGain = ConvertAmplitudeFromDb(room.reverbGainDb);
		result.reverbTime = room.reverbTime;
		result.reverbBrightness = room.reverbBrightness;
		result.reflectionScalar = room.reflectivity;
		return result;
	}

	[DllImport("audioplugingvrunity")]
	private static extern void SetListenerGain(float gain);

	[DllImport("audioplugingvrunity")]
	private static extern int CreateSoundfield(int numChannels);

	[DllImport("audioplugingvrunity")]
	private static extern int CreateSoundObject(bool enableHrtf);

	[DllImport("audioplugingvrunity")]
	private static extern void DestroySource(int sourceId);

	[DllImport("audioplugingvrunity")]
	private static extern void SetSourceBypassRoomEffects(int sourceId, bool bypassRoomEffects);

	[DllImport("audioplugingvrunity")]
	private static extern void SetSourceDirectivity(int sourceId, float alpha, float order);

	[DllImport("audioplugingvrunity")]
	private static extern void SetSourceListenerDirectivity(int sourceId, float alpha, float order);

	[DllImport("audioplugingvrunity")]
	private static extern void SetSourceOcclusionIntensity(int sourceId, float intensity);

	[DllImport("audioplugingvrunity")]
	private static extern void SetRoomProperties(IntPtr roomProperties);

	[DllImport("audioplugingvrunity")]
	private static extern void Initialize(int quality, int sampleRate, int numChannels, int framesPerBuffer);

	[DllImport("audioplugingvrunity")]
	private static extern void Shutdown();
}
