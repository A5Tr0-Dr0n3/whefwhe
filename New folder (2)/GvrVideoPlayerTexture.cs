using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using UnityEngine.UI;

public class GvrVideoPlayerTexture : MonoBehaviour
{
	public enum VideoType
	{
		Dash = 0,
		HLS = 2,
		Other = 3
	}

	public enum VideoResolution
	{
		Lowest = 1,
		_720 = 720,
		_1080 = 1080,
		_2048 = 2048,
		Highest = 4096
	}

	public enum VideoPlayerState
	{
		Idle = 1,
		Preparing,
		Buffering,
		Ready,
		Ended
	}

	public enum VideoEvents
	{
		VideoReady = 1,
		VideoStartPlayback,
		VideoFormatChanged,
		VideoSurfaceSet,
		VideoSizeChanged
	}

	public enum StereoMode
	{
		NoValue = -1,
		Mono,
		TopBottom,
		LeftRight
	}

	private enum RenderCommand
	{
		None = -1,
		InitializePlayer,
		UpdateVideo,
		RenderMono,
		RenderLeftEye,
		RenderRightEye,
		Shutdown
	}

	internal delegate void OnVideoEventCallback(IntPtr cbdata, int eventId);

	internal delegate void OnExceptionCallback(string type, string msg, IntPtr cbdata);

	private IntPtr videoPlayerPtr;

	private int videoPlayerEventBase;

	private Texture initialTexture;

	private Texture surfaceTexture;

	private float[] videoMatrixRaw;

	private Matrix4x4 videoMatrix;

	private int videoMatrixPropertyId;

	private long lastVideoTimestamp;

	private bool initialized;

	private int texWidth = 1024;

	private int texHeight = 1024;

	private long lastBufferedPosition;

	private float framecount;

	private Renderer screen;

	private IntPtr renderEventFunction;

	private bool playOnResume;

	private List<Action<int>> onEventCallbacks;

	private List<Action<string, string>> onExceptionCallbacks;

	private static readonly Queue<Action> ExecuteOnMainThread = new Queue<Action>();

	public Text statusText;

	public VideoType videoType;

	public string videoURL;

	public string videoContentID;

	public string videoProviderId;

	public VideoResolution initialResolution = VideoResolution.Highest;

	public bool adjustAspectRatio;

	public bool useSecurePath;

	private const string NOT_IMPLEMENTED_MSG = "Not implemented on this platform";

	public bool VideoReady => videoPlayerPtr != IntPtr.Zero && IsVideoReady(videoPlayerPtr);

	public long CurrentPosition
	{
		get
		{
			return (!(videoPlayerPtr != IntPtr.Zero)) ? 0 : GetCurrentPosition(videoPlayerPtr);
		}
		set
		{
			if (value == 0)
			{
				framecount = 0f;
			}
			SetCurrentPosition(videoPlayerPtr, value);
		}
	}

	public long VideoDuration => (!(videoPlayerPtr != IntPtr.Zero)) ? 0 : GetDuration(videoPlayerPtr);

	public long BufferedPosition => (!(videoPlayerPtr != IntPtr.Zero)) ? 0 : GetBufferedPosition(videoPlayerPtr);

	public int BufferedPercentage => (videoPlayerPtr != IntPtr.Zero) ? GetBufferedPercentage(videoPlayerPtr) : 0;

	public bool IsPaused => !initialized || videoPlayerPtr == IntPtr.Zero || IsVideoPaused(videoPlayerPtr);

	public VideoPlayerState PlayerState => (!(videoPlayerPtr != IntPtr.Zero)) ? VideoPlayerState.Idle : ((VideoPlayerState)GetPlayerState(videoPlayerPtr));

	public int MaxVolume => (videoPlayerPtr != IntPtr.Zero) ? GetMaxVolume(videoPlayerPtr) : 0;

	public int CurrentVolume
	{
		get
		{
			return (videoPlayerPtr != IntPtr.Zero) ? GetCurrentVolume(videoPlayerPtr) : 0;
		}
		set
		{
			SetCurrentVolume(value);
		}
	}

	public StereoMode CurrentStereoMode => (!(videoPlayerPtr != IntPtr.Zero)) ? StereoMode.NoValue : ((StereoMode)GetStereoMode(videoPlayerPtr));

	public bool HasProjection => videoPlayerPtr != IntPtr.Zero && HasProjectionData(videoPlayerPtr);

	public Renderer Screen
	{
		get
		{
			return screen;
		}
		set
		{
			if (!(screen == value))
			{
				if (screen != null && initialTexture != null)
				{
					screen.sharedMaterial.mainTexture = initialTexture;
				}
				screen = value;
				if (screen != null)
				{
					initialTexture = screen.sharedMaterial.mainTexture;
				}
			}
		}
	}

	public Texture CurrentFrameTexture => surfaceTexture;

	public int Width => texWidth;

	public int Height => texHeight;

	public float AspectRatio
	{
		get
		{
			if (texHeight == 0)
			{
				return 0f;
			}
			return (float)texWidth / (float)texHeight;
		}
	}

	private void Awake()
	{
		videoMatrixRaw = new float[16];
		videoMatrixPropertyId = Shader.PropertyToID("video_matrix");
		Screen = GetComponent<Renderer>();
		CreatePlayer();
	}

	private void CreatePlayer()
	{
		videoPlayerPtr = CreateVideoPlayer();
		videoPlayerEventBase = GetVideoPlayerEventBase(videoPlayerPtr);
		Debug.Log(" -- " + gameObject.name + " created with base " + videoPlayerEventBase);
		SetOnVideoEventCallback((int eventId) =>
		{
			Debug.Log("------------- E V E N T " + eventId + " -----------------");
			UpdateStatusText();
		});
		SetOnExceptionCallback((string type, string msg) =>
		{
			Debug.LogError("Exception: " + type + ": " + msg);
		});
		initialized = false;
	}

	private void OnDisable()
	{
		if (videoPlayerPtr != IntPtr.Zero && GetPlayerState(videoPlayerPtr) == 4)
		{
			PauseVideo(videoPlayerPtr);
		}
	}

	public void SetDisplayTexture(Texture texture)
	{
		if (texture == null)
		{
			texture = initialTexture;
		}
		if (!(texture == null) && screen != null)
		{
			screen.sharedMaterial.mainTexture = texture;
		}
	}

	public void CleanupVideo()
	{
		Debug.Log("Cleaning Up video!");
		if (videoPlayerPtr != IntPtr.Zero)
		{
			DestroyVideoPlayer(videoPlayerPtr);
			videoPlayerPtr = IntPtr.Zero;
		}
		if (surfaceTexture != null)
		{
			UnityEngine.Object.Destroy(surfaceTexture);
			surfaceTexture = null;
		}
		if (screen != null)
		{
			screen.sharedMaterial.mainTexture = initialTexture;
		}
	}

	public void ReInitializeVideo()
	{
		if (screen != null)
		{
			screen.sharedMaterial.mainTexture = initialTexture;
		}
		if (videoPlayerPtr == IntPtr.Zero)
		{
			CreatePlayer();
		}
		Init();
	}

	private void OnDestroy()
	{
		CleanupVideo();
	}

	private void OnApplicationPause(bool bPause)
	{
		if (videoPlayerPtr != IntPtr.Zero)
		{
			if (bPause)
			{
				playOnResume = !IsPaused;
				PauseVideo(videoPlayerPtr);
			}
			else if (playOnResume)
			{
				PlayVideo(videoPlayerPtr);
			}
		}
	}

	private void UpdateMaterial()
	{
		if (videoPlayerPtr == IntPtr.Zero)
		{
			return;
		}
		texWidth = GetWidth(videoPlayerPtr);
		texHeight = GetHeight(videoPlayerPtr);
		int externalSurfaceTextureId = GetExternalSurfaceTextureId(videoPlayerPtr);
		if (surfaceTexture != null && surfaceTexture.GetNativeTexturePtr().ToInt32() != externalSurfaceTextureId)
		{
			UnityEngine.Object.Destroy(surfaceTexture);
			surfaceTexture = null;
		}
		if (surfaceTexture == null && externalSurfaceTextureId != 0)
		{
			Debug.Log("Creating external texture with surface texture id " + externalSurfaceTextureId);
			surfaceTexture = Texture2D.CreateExternalTexture(4, 4, TextureFormat.RGBA32, mipmap: false, linear: false, new IntPtr(externalSurfaceTextureId));
		}
		if (surfaceTexture == null || PlayerState == VideoPlayerState.Ended)
		{
			return;
		}
		if (screen == null)
		{
			Debug.LogError("GvrVideoPlayerTexture: No screen to display the video is set.");
		}
		else if (screen != null)
		{
			if (screen.sharedMaterial.mainTexture == null || screen.sharedMaterial.mainTexture.GetNativeTexturePtr() != surfaceTexture.GetNativeTexturePtr())
			{
				screen.sharedMaterial.mainTexture = surfaceTexture;
			}
			screen.sharedMaterial.SetMatrix(videoMatrixPropertyId, videoMatrix);
		}
	}

	private void OnRestartVideoEvent(int eventId)
	{
		if (eventId == 1)
		{
			Debug.Log("Restarting video complete.");
			RemoveOnVideoEventCallback(OnRestartVideoEvent);
		}
	}

	public void RestartVideo()
	{
		SetOnVideoEventCallback(OnRestartVideoEvent);
		string text = ProcessURL();
		InitVideoPlayer(videoPlayerPtr, (int)videoType, text, videoContentID, videoProviderId, useSecurePath, useExisting: true);
		framecount = 0f;
		lastVideoTimestamp = -1L;
	}

	public void SetCurrentVolume(int val)
	{
		SetCurrentVolume(videoPlayerPtr, val);
	}

	public bool Init()
	{
		if (initialized)
		{
			Debug.Log("Skipping initialization: video player already loaded");
			return true;
		}
		if (videoURL == null || videoURL.Length == 0)
		{
			Debug.LogError("Cannot initialize with null videoURL");
			return false;
		}
		videoURL = ((videoURL != null) ? videoURL.Trim() : string.Empty);
		videoContentID = ((videoContentID != null) ? videoContentID.Trim() : string.Empty);
		videoProviderId = ((videoProviderId != null) ? videoProviderId.Trim() : string.Empty);
		SetInitialResolution(videoPlayerPtr, (int)initialResolution);
		string text = ProcessURL();
		Debug.Log(string.Concat("Playing ", videoType, " ", text));
		Debug.Log("videoContentID = " + videoContentID);
		Debug.Log("videoProviderId = " + videoProviderId);
		videoPlayerPtr = InitVideoPlayer(videoPlayerPtr, (int)videoType, text, videoContentID, videoProviderId, useSecurePath, useExisting: false);
		IssuePlayerEvent(RenderCommand.InitializePlayer);
		initialized = true;
		framecount = 0f;
		lastVideoTimestamp = -1L;
		return videoPlayerPtr != IntPtr.Zero;
	}

	public bool Play()
	{
		if (!initialized)
		{
			Init();
		}
		if (videoPlayerPtr != IntPtr.Zero && IsVideoReady(videoPlayerPtr))
		{
			return PlayVideo(videoPlayerPtr) == 0;
		}
		Debug.LogError("Video player not ready to Play!");
		return false;
	}

	public bool Pause()
	{
		if (!initialized)
		{
			Init();
		}
		if (VideoReady)
		{
			return PauseVideo(videoPlayerPtr) == 0;
		}
		Debug.LogError("Video player not ready to Pause!");
		return false;
	}

	private void AdjustAspectRatio()
	{
		float aspectRatio = AspectRatio;
		if (aspectRatio != 0f)
		{
			Vector3 localScale = transform.localScale;
			localScale.y = Mathf.Min(localScale.y, localScale.x / aspectRatio);
			transform.localScale = localScale;
		}
	}

	private void UpdateStatusText()
	{
		float num = ((CurrentPosition <= 0) ? ((float)CurrentPosition) : (framecount / ((float)CurrentPosition / 1000f)));
		string text = string.Concat(texWidth, " x ", texHeight, " buffer: ", BufferedPosition / 1000, " ", PlayerState, " fps: ", num);
		if (statusText != null && statusText.text != text)
		{
			statusText.text = text;
		}
	}

	private void IssuePlayerEvent(RenderCommand evt)
	{
		if (renderEventFunction == IntPtr.Zero)
		{
			renderEventFunction = GetRenderEventFunc();
		}
		if (renderEventFunction == IntPtr.Zero || evt == RenderCommand.None)
		{
			Debug.LogError("Attempt to IssuePlayerEvent before renderEventFunction ready.");
		}
		else
		{
			GL.IssuePluginEvent(renderEventFunction, (int)(videoPlayerEventBase + evt));
		}
	}

	private void Update()
	{
		while (ExecuteOnMainThread.Count > 0)
		{
			ExecuteOnMainThread.Dequeue()();
		}
		if (VideoReady)
		{
			IssuePlayerEvent(RenderCommand.UpdateVideo);
			GetVideoMatrix(videoPlayerPtr, videoMatrixRaw);
			videoMatrix = GvrMathHelpers.ConvertFloatArrayToMatrix(videoMatrixRaw);
			long videoTimestampNs = GetVideoTimestampNs(videoPlayerPtr);
			if (videoTimestampNs != lastVideoTimestamp)
			{
				framecount++;
			}
			lastVideoTimestamp = videoTimestampNs;
			UpdateMaterial();
			if (adjustAspectRatio)
			{
				AdjustAspectRatio();
			}
			if ((int)framecount % 30 == 0)
			{
				UpdateStatusText();
			}
			long bufferedPosition = BufferedPosition;
			if (bufferedPosition != lastBufferedPosition)
			{
				lastBufferedPosition = bufferedPosition;
				UpdateStatusText();
			}
		}
	}

	public void RemoveOnVideoEventCallback(Action<int> callback)
	{
		if (onEventCallbacks != null)
		{
			onEventCallbacks.Remove(callback);
		}
	}

	public void SetOnVideoEventCallback(Action<int> callback)
	{
		if (onEventCallbacks == null)
		{
			onEventCallbacks = new List<Action<int>>();
		}
		onEventCallbacks.Add(callback);
		SetOnVideoEventCallback(videoPlayerPtr, InternalOnVideoEventCallback, ToIntPtr(this));
	}

	internal void FireVideoEvent(int eventId)
	{
		if (onEventCallbacks == null)
		{
			return;
		}
		Action<int>[] array = onEventCallbacks.ToArray();
		Action<int>[] array2 = array;
		foreach (Action<int> action in array2)
		{
			try
			{
				action(eventId);
			}
			catch (Exception ex)
			{
				Debug.LogError("exception calling callback: " + ex);
			}
		}
	}

	[MonoPInvokeCallback(typeof(OnVideoEventCallback))]
	private static void InternalOnVideoEventCallback(IntPtr cbdata, int eventId)
	{
		if (cbdata == IntPtr.Zero)
		{
			return;
		}
		GCHandle gCHandle = GCHandle.FromIntPtr(cbdata);
		GvrVideoPlayerTexture player;
		try
		{
			player = (GvrVideoPlayerTexture)gCHandle.Target;
		}
		catch (InvalidCastException ex)
		{
			Debug.LogError(string.Concat("GC Handle pointed to unexpected type: ", gCHandle.Target, ". Expected ", typeof(GvrVideoPlayerTexture)));
			throw ex;
		}
		if (player != null)
		{
			ExecuteOnMainThread.Enqueue(() =>
			{
				player.FireVideoEvent(eventId);
			});
		}
	}

	public void SetOnExceptionCallback(Action<string, string> callback)
	{
		if (onExceptionCallbacks == null)
		{
			onExceptionCallbacks = new List<Action<string, string>>();
			SetOnExceptionCallback(videoPlayerPtr, InternalOnExceptionCallback, ToIntPtr(this));
		}
		onExceptionCallbacks.Add(callback);
	}

	[MonoPInvokeCallback(typeof(OnExceptionCallback))]
	private static void InternalOnExceptionCallback(string type, string msg, IntPtr cbdata)
	{
		if (cbdata == IntPtr.Zero)
		{
			return;
		}
		GCHandle gCHandle = GCHandle.FromIntPtr(cbdata);
		GvrVideoPlayerTexture player;
		try
		{
			player = (GvrVideoPlayerTexture)gCHandle.Target;
		}
		catch (InvalidCastException ex)
		{
			Debug.LogError(string.Concat("GC Handle pointed to unexpected type: ", gCHandle.Target, ". Expected ", typeof(GvrVideoPlayerTexture)));
			throw ex;
		}
		if (player != null)
		{
			ExecuteOnMainThread.Enqueue(() =>
			{
				player.FireOnException(type, msg);
			});
		}
	}

	internal void FireOnException(string type, string msg)
	{
		if (onExceptionCallbacks == null)
		{
			return;
		}
		foreach (Action<string, string> onExceptionCallback in onExceptionCallbacks)
		{
			try
			{
				onExceptionCallback(type, msg);
			}
			catch (Exception ex)
			{
				Debug.LogError("exception calling callback: " + ex);
			}
		}
	}

	internal static IntPtr ToIntPtr(object obj)
	{
		GCHandle value = GCHandle.Alloc(obj);
		return GCHandle.ToIntPtr(value);
	}

	internal string ProcessURL()
	{
		return videoURL.Replace("${Application.dataPath}", Application.dataPath);
	}

	private static IntPtr GetRenderEventFunc()
	{
		Debug.Log("Not implemented on this platform");
		return IntPtr.Zero;
	}

	private static void SetExternalTextures(IntPtr videoPlayerPtr, int[] texIds, int size, int w, int h)
	{
		Debug.Log("Not implemented on this platform");
	}

	private static IntPtr GetRenderableTextureId(IntPtr videoPlayerPtr)
	{
		Debug.Log("Not implemented on this platform");
		return IntPtr.Zero;
	}

	private static int GetExternalSurfaceTextureId(IntPtr videoPlayerPtr)
	{
		Debug.Log("Not implemented on this platform");
		return 0;
	}

	private static void GetVideoMatrix(IntPtr videoPlayerPtr, float[] videoMatrix)
	{
		Debug.Log("Not implemented on this platform");
	}

	private static long GetVideoTimestampNs(IntPtr videoPlayerPtr)
	{
		Debug.Log("Not implemented on this platform");
		return -1L;
	}

	public static IntPtr CreateVideoPlayer()
	{
		Debug.Log("Not implemented on this platform");
		return IntPtr.Zero;
	}

	public static void DestroyVideoPlayer(IntPtr videoPlayerPtr)
	{
		Debug.Log("Not implemented on this platform");
	}

	private static int GetVideoPlayerEventBase(IntPtr videoPlayerPtr)
	{
		Debug.Log("Not implemented on this platform");
		return 0;
	}

	private static IntPtr InitVideoPlayer(IntPtr videoPlayerPtr, int videoType, string videoURL, string contentID, string providerId, bool useSecurePath, bool useExisting)
	{
		Debug.Log("Not implemented on this platform");
		return IntPtr.Zero;
	}

	private static void SetInitialResolution(IntPtr videoPlayerPtr, int initialResolution)
	{
		Debug.Log("Not implemented on this platform");
	}

	private static int GetPlayerState(IntPtr videoPlayerPtr)
	{
		Debug.Log("Not implemented on this platform");
		return -1;
	}

	private static int GetWidth(IntPtr videoPlayerPtr)
	{
		Debug.Log("Not implemented on this platform");
		return -1;
	}

	private static int GetHeight(IntPtr videoPlayerPtr)
	{
		Debug.Log("Not implemented on this platform");
		return -1;
	}

	private static int PlayVideo(IntPtr videoPlayerPtr)
	{
		Debug.Log("Not implemented on this platform");
		return 0;
	}

	private static int PauseVideo(IntPtr videoPlayerPtr)
	{
		Debug.Log("Not implemented on this platform");
		return 0;
	}

	private static bool IsVideoReady(IntPtr videoPlayerPtr)
	{
		Debug.Log("Not implemented on this platform");
		return false;
	}

	private static bool IsVideoPaused(IntPtr videoPlayerPtr)
	{
		Debug.Log("Not implemented on this platform");
		return true;
	}

	private static long GetDuration(IntPtr videoPlayerPtr)
	{
		Debug.Log("Not implemented on this platform");
		return -1L;
	}

	private static long GetBufferedPosition(IntPtr videoPlayerPtr)
	{
		Debug.Log("Not implemented on this platform");
		return -1L;
	}

	private static long GetCurrentPosition(IntPtr videoPlayerPtr)
	{
		Debug.Log("Not implemented on this platform");
		return -1L;
	}

	private static void SetCurrentPosition(IntPtr videoPlayerPtr, long pos)
	{
		Debug.Log("Not implemented on this platform");
	}

	private static int GetBufferedPercentage(IntPtr videoPlayerPtr)
	{
		Debug.Log("Not implemented on this platform");
		return 0;
	}

	private static int GetMaxVolume(IntPtr videoPlayerPtr)
	{
		Debug.Log("Not implemented on this platform");
		return 0;
	}

	private static int GetCurrentVolume(IntPtr videoPlayerPtr)
	{
		Debug.Log("Not implemented on this platform");
		return 0;
	}

	private static void SetCurrentVolume(IntPtr videoPlayerPtr, int value)
	{
		Debug.Log("Not implemented on this platform");
	}

	private static int GetStereoMode(IntPtr videoPlayerPtr)
	{
		Debug.Log("Not implemented on this platform");
		return -1;
	}

	private static bool HasProjectionData(IntPtr videoPlayerPtr)
	{
		Debug.Log("Not implemented on this platform");
		return false;
	}

	private static bool SetVideoPlayerSupportClassname(IntPtr videoPlayerPtr, string classname)
	{
		Debug.Log("Not implemented on this platform");
		return false;
	}

	private static IntPtr GetRawPlayer(IntPtr videoPlayerPtr)
	{
		Debug.Log("Not implemented on this platform");
		return IntPtr.Zero;
	}

	private static void SetOnVideoEventCallback(IntPtr videoPlayerPtr, OnVideoEventCallback callback, IntPtr callback_arg)
	{
		Debug.Log("Not implemented on this platform");
	}

	private static void SetOnExceptionCallback(IntPtr videoPlayerPtr, OnExceptionCallback callback, IntPtr callback_arg)
	{
		Debug.Log("Not implemented on this platform");
	}
}
