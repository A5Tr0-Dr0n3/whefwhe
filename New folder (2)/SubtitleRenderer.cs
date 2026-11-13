using TMPro;
using UnityEngine;

public class SubtitleRenderer : MonoBehaviour
{
	[Header("Tweakables")]
	[SerializeField]
	private float offToOnscreenTransTime = 1f;

	[SerializeField]
	private AnimationCurve transitionPosition;

	[SerializeField]
	private AnimationCurve transitionRotation;

	[SerializeField]
	private bool forceDisplay;

	[SerializeField]
	protected bool noDialogueSource;

	[SerializeField]
	protected float yDampValue = 0.1f;

	[SerializeField]
	protected float xzDampValue = 0.1f;

	[SerializeField]
	protected bool noBillboarding;

	[SerializeField]
	protected bool noScale;

	[SerializeField]
	private float cameraFollowOffsetZ = 1.5f;

	[SerializeField]
	private float cameraFollowOffsetY = 1.5f;

	[SerializeField]
	private float onScreenXThreshhold;

	[SerializeField]
	private float onScreenYThreshhold;

	[SerializeField]
	private float offScreenXThreshhold = 0.25f;

	[SerializeField]
	private float offScreenYThreshhold = 0.25f;

	[SerializeField]
	private float widthStep = 0.25f;

	[SerializeField]
	private float arrowScaleTime = 0.3f;

	[SerializeField]
	private bool forceToHead;

	[SerializeField]
	private bool forceToCharacter;

	[SerializeField]
	private float scaleAtMaxDist = 0.6f;

	[SerializeField]
	private float maxScalingDist = 10f;

	[SerializeField]
	private float textScaler;

	[SerializeField]
	private float arrowRadiusScaler = 0.25f;

	[Header("Character Refs")]
	[SerializeField]
	protected Transform characterPos;

	public float xOffsetChar;

	public float yOffsetChar;

	public float zOffsetChar;

	[SerializeField]
	protected float minDistanceThreshold = 2f;

	[Header("Subtitle Refs")]
	[SerializeField]
	protected VVRDialogueSource dialogueSource;

	protected SubtitleContainer subtitles;

	[SerializeField]
	protected TextMeshPro textMesh;

	[SerializeField]
	protected Transform meshParent;

	[SerializeField]
	protected GvrAudioSource audioSource;

	[SerializeField]
	protected Transform arrow;

	protected int lineIndex = -1;

	protected bool isPlaying;

	protected SubtitleLine currentLine;

	protected Vector3 targetPos;

	public NPCDialogueClip currentClip;

	protected Vector2 originalSize = new Vector2(0f, 0f);

	private int numLines;

	private bool onScreen;

	private bool onScreenLastFrame;

	private float onToOffscreenT;

	private float offToOnscreenT;

	private float arrowTransitionT;

	private float currentTime;

	private bool arrowOut;

	private bool ignoreArrow;

	private Vector3 screenPoint;

	private bool prePaddingLastFrame;

	private bool firstframe = true;

	private bool ignoreArrowLastFrame;

	private Vector3 lastOffscreenCameraUp;

	private Vector3 lastOffScreenPosition;

	private Camera mainCamera;

	private TMP_FontAsset startFont;

	private Material startMat;

	private Vector3 originalArrowScale = Vector3.zero;

	private void Awake()
	{
		Init();
		mainCamera = Camera.main;
	}

	private void OnDisable()
	{
		firstframe = true;
		arrow.gameObject.SetActive(value: false);
		PersistentDataManager.onLanguageChange -= SwapFont;
	}

	private void OnEnable()
	{
		Init();
		PersistentDataManager.onLanguageChange += SwapFont;
	}

	private void LateUpdate()
	{
		if (mainCamera == null)
		{
			mainCamera = EagerMaster.instance.mainCamera;
		}
		if (IsPlaying())
		{
			if (!textMesh.enabled)
			{
				textMesh.enabled = true;
			}
			if (noDialogueSource && !forceDisplay)
			{
				UpdateCurrentLine(audioSource);
			}
			else if (!forceDisplay)
			{
				UpdateCurrentLine();
			}
			if (!arrow.gameObject.activeInHierarchy)
			{
				arrow.gameObject.SetActive(value: true);
			}
			DisplayText();
			if (firstframe)
			{
				if (onScreen && !forceToHead)
				{
					Vector3 vector = meshParent.position - characterPos.position;
					vector.Normalize();
					Vector3 vector2 = Vector3.Cross(vector, Vector3.up);
					meshParent.position = characterPos.position + Vector3.up * yOffsetChar + vector2 * xOffsetChar + vector * zOffsetChar;
					meshParent.LookAt(mainCamera.transform.position);
					ScaleText();
					offToOnscreenT = offToOnscreenTransTime;
				}
				else
				{
					meshParent.position = mainCamera.transform.position + mainCamera.transform.forward * cameraFollowOffsetZ + mainCamera.transform.up * cameraFollowOffsetY;
					meshParent.LookAt(mainCamera.transform.position);
					ScaleText();
					onToOffscreenT = offToOnscreenTransTime;
				}
				DisplayText();
				firstframe = false;
			}
		}
		else
		{
			if (textMesh.enabled)
			{
				textMesh.enabled = false;
			}
			if (arrow.gameObject.activeInHierarchy)
			{
				arrow.gameObject.SetActive(value: false);
				arrowOut = false;
				arrow.transform.localScale = Vector3.zero;
			}
		}
	}

	protected virtual void DisplayText()
	{
		ignoreArrow = false;
		meshParent.transform.rotation = Quaternion.identity;
		Vector3 vector = mainCamera.transform.position - characterPos.position;
		vector.Normalize();
		Vector3 vector2 = Vector3.Cross(vector, Vector3.up);
		vector2.Normalize();
		Vector3 vector3 = Vector3.Cross(vector, -vector2);
		targetPos = characterPos.position + vector2 * xOffsetChar + vector * zOffsetChar;
		if (currentLine.text == string.Empty)
		{
			ignoreArrow = true;
		}
		screenPoint = mainCamera.WorldToViewportPoint(targetPos);
		if (onScreenLastFrame)
		{
			onScreen = screenPoint.z > 0f && screenPoint.x > onScreenXThreshhold && screenPoint.x < 1f - onScreenXThreshhold && screenPoint.y > onScreenYThreshhold && screenPoint.y < 1f - onScreenYThreshhold;
		}
		else
		{
			onScreen = screenPoint.z > 0f && screenPoint.x > offScreenXThreshhold && screenPoint.x < 1f - offScreenXThreshhold && screenPoint.y > offScreenYThreshhold && screenPoint.y < 1f - offScreenYThreshhold;
		}
		if (Mathf.Abs(Vector3.Distance(mainCamera.transform.position, targetPos)) < minDistanceThreshold)
		{
			ignoreArrow = true;
			onScreen = false;
		}
		if (forceToHead)
		{
			onScreen = false;
			ignoreArrow = true;
		}
		if (forceToCharacter)
		{
			onScreen = true;
		}
		if (onScreen)
		{
			if (arrowOut)
			{
				ScaleArrow(active: false);
			}
			else if (arrow.gameObject.activeInHierarchy)
			{
				arrow.localScale = Vector3.zero;
				arrow.gameObject.SetActive(value: false);
				arrowOut = false;
				arrowTransitionT = 0f;
			}
			onToOffscreenT = 0f;
			if (offToOnscreenT < offToOnscreenTransTime)
			{
				offToOnscreenT += Time.deltaTime;
				float x = Mathf.Lerp(lastOffScreenPosition.x, targetPos.x, transitionPosition.Evaluate(offToOnscreenT / offToOnscreenTransTime));
				float y = Mathf.Lerp(lastOffScreenPosition.y, targetPos.y + yOffsetChar * findScaleConstant(targetPos), transitionPosition.Evaluate(offToOnscreenT / offToOnscreenTransTime));
				float z = Mathf.Lerp(lastOffScreenPosition.z, targetPos.z, transitionPosition.Evaluate(offToOnscreenT / offToOnscreenTransTime));
				meshParent.position = new Vector3(x, y, z);
				if (!noScale)
				{
					ScaleText();
				}
				if (!noBillboarding)
				{
					meshParent.transform.LookAt(mainCamera.transform.position, Vector3.Lerp(lastOffscreenCameraUp, vector3, transitionRotation.Evaluate(offToOnscreenT / offToOnscreenTransTime)));
				}
			}
			else
			{
				float num = TimelockedDamp(meshParent.position.y, targetPos.y, yDampValue, Time.deltaTime);
				float x2 = TimelockedDamp(meshParent.position.x, targetPos.x, xzDampValue, Time.deltaTime);
				float z2 = TimelockedDamp(meshParent.position.z, targetPos.z, xzDampValue, Time.deltaTime);
				num = targetPos.y + yOffsetChar * findScaleConstant(targetPos);
				meshParent.position = new Vector3(x2, num, z2);
				if (!noScale)
				{
					ScaleText();
				}
				if (!noBillboarding)
				{
					meshParent.transform.LookAt(mainCamera.transform.position, vector3);
				}
			}
		}
		else
		{
			offToOnscreenT = 0f;
			lastOffscreenCameraUp = mainCamera.transform.up;
			targetPos = mainCamera.transform.position + mainCamera.transform.forward * cameraFollowOffsetZ + mainCamera.transform.up * cameraFollowOffsetY;
			if (onToOffscreenT < 1f)
			{
				onToOffscreenT += Time.deltaTime * 0.5f;
				meshParent.position = Vector3.Lerp(meshParent.transform.position, targetPos, onToOffscreenT);
			}
			else
			{
				meshParent.position = targetPos;
			}
			lastOffScreenPosition = meshParent.position;
			if (!noScale)
			{
				ScaleText();
			}
			if (!noBillboarding)
			{
				if (onToOffscreenT <= 1f)
				{
					meshParent.transform.LookAt(mainCamera.transform.position, Vector3.Lerp(Vector3.up, mainCamera.transform.up, onToOffscreenT * 5f));
				}
				else
				{
					meshParent.transform.LookAt(mainCamera.transform.position, mainCamera.transform.up);
				}
			}
			if (onToOffscreenT > arrowScaleTime && dialogueSource != null && dialogueSource.GetClipQueue().Count > 0 && dialogueSource.GetClipQueue()[0].playbackProgress != DialoguePlaybackProgress.prePadding)
			{
				PositionArrow(ignoreArrow);
			}
			else if (noDialogueSource)
			{
				PositionArrow(ignoreArrow);
			}
		}
		onScreenLastFrame = onScreen;
	}

	protected void UpdateCurrentLine()
	{
		if (dialogueSource != null && dialogueSource.GetClipQueue() != null && dialogueSource.GetClipQueue().Count > 0)
		{
			currentTime = dialogueSource.GetClipQueue()[0].progressTime;
			if (subtitles.dialogueLines != null && lineIndex != -1 && currentTime < subtitles.dialogueLines[lineIndex].timeKey)
			{
				lineIndex = -1;
			}
			if (dialogueSource.GetClipQueue().Count > 0 && dialogueSource.GetClipQueue()[0].playbackProgress == DialoguePlaybackProgress.postPadding && subtitles.dialogueLines != null)
			{
				currentTime = subtitles.dialogueLines[subtitles.dialogueLines.Count - 1].timeKey;
			}
			if (dialogueSource.GetClipQueue()[0].mClipData.GetSubtitles().dialogueLines != subtitles.dialogueLines)
			{
				SetSubtitleContainer(dialogueSource.GetClipQueue()[0].mClipData.GetSubtitles());
				lineIndex = -1;
			}
			if (dialogueSource.GetClipQueue()[0].mClipData != currentClip || (subtitles.dialogueLines != null && lineIndex != -1 && subtitles.dialogueLines[lineIndex].timeKey > currentTime && lineIndex < subtitles.dialogueLines.Count - 1) || prePaddingLastFrame)
			{
				currentClip = dialogueSource.GetClipQueue()[0].mClipData;
				if (subtitles.dialogueLines != null && lineIndex != -1)
				{
					currentLine = subtitles.dialogueLines[0];
					if (onScreen && offToOnscreenT >= offToOnscreenTransTime)
					{
						meshParent.position = targetPos;
					}
					textMesh.text = currentLine.text;
					SizeLines();
				}
				lineIndex = -1;
			}
		}
		if (dialogueSource == null || !dialogueSource.isActiveAndEnabled || dialogueSource.GetClipQueue().Count == 0)
		{
			if (!noDialogueSource)
			{
				textMesh.text = string.Empty;
				arrow.localScale = Vector3.zero;
				arrow.gameObject.SetActive(value: false);
				arrowOut = false;
				arrowTransitionT = 0f;
				lineIndex = -1;
				return;
			}
		}
		else if (subtitles.dialogueLines != null && lineIndex < subtitles.dialogueLines.Count - 1 && !prePaddingLastFrame)
		{
			bool flag = false;
			while (lineIndex < subtitles.dialogueLines.Count - 1 && currentTime >= subtitles.dialogueLines[lineIndex + 1].timeKey)
			{
				flag = true;
				lineIndex++;
			}
			if (flag)
			{
				currentLine = subtitles.dialogueLines[lineIndex];
				if (onScreen && offToOnscreenT >= offToOnscreenTransTime)
				{
					meshParent.position = targetPos;
				}
				textMesh.text = currentLine.text;
				SizeLines();
			}
		}
		if (!noDialogueSource && dialogueSource.GetClipQueue().Count > 0)
		{
			currentClip = dialogueSource.GetClipQueue()[0].mClipData;
		}
		if (!noDialogueSource && dialogueSource.GetClipQueue().Count > 0 && dialogueSource.GetClipQueue()[0].playbackProgress == DialoguePlaybackProgress.prePadding)
		{
			textMesh.text = string.Empty;
			arrow.localScale = Vector3.zero;
			arrow.gameObject.SetActive(value: false);
			arrowOut = false;
			arrowTransitionT = 0f;
			prePaddingLastFrame = true;
			lineIndex = -1;
		}
		else
		{
			prePaddingLastFrame = false;
		}
	}

	protected void UpdateCurrentLine(GvrAudioSource source)
	{
		if (audioSource.clip == null || !audioSource.isPlaying)
		{
			textMesh.enabled = false;
			return;
		}
		if (!textMesh.enabled)
		{
			textMesh.enabled = true;
		}
		SetSubtitleContainer(SubtitleLoader.instance.GetSubtitleContainer(audioSource.clip.name));
		float time = source.time;
		if (lineIndex == 0)
		{
			currentLine = subtitles.dialogueLines[lineIndex];
			if (onScreen && offToOnscreenT >= offToOnscreenTransTime)
			{
				meshParent.position = targetPos;
			}
			SizeLines();
		}
		if (lineIndex < subtitles.dialogueLines.Count - 1 && time >= subtitles.dialogueLines[lineIndex + 1].timeKey)
		{
			currentLine = subtitles.dialogueLines[lineIndex + 1];
			if (onScreen && offToOnscreenT >= offToOnscreenTransTime)
			{
				meshParent.position = targetPos;
			}
			SizeLines();
			textMesh.text = currentLine.text;
			lineIndex++;
		}
		if (subtitles.dialogueLines != null && subtitles.dialogueLines.Count > 1 && time < subtitles.dialogueLines[1].timeKey && time >= subtitles.dialogueLines[0].timeKey)
		{
			lineIndex = 0;
			currentLine = subtitles.dialogueLines[0];
			if (onScreen && offToOnscreenT >= offToOnscreenTransTime)
			{
				meshParent.position = targetPos;
			}
			SizeLines();
			textMesh.text = currentLine.text;
		}
	}

	protected virtual void Init()
	{
		if (startFont == null)
		{
			startFont = textMesh.font;
		}
		if (startMat == null)
		{
			startMat = textMesh.fontSharedMaterial;
		}
		if (!forceDisplay)
		{
			textMesh.text = string.Empty;
		}
		textMesh.enabled = false;
		if (subtitles.dialogueLines != null && subtitles.dialogueLines.Count > 0)
		{
			currentLine = subtitles.dialogueLines[0];
			lineIndex = 0;
			textMesh.text = currentLine.text;
		}
		if (characterPos != null)
		{
			targetPos = characterPos.position + Vector3.up * yOffsetChar + characterPos.transform.right * xOffsetChar + characterPos.transform.forward * zOffsetChar;
		}
		if (originalSize == Vector2.zero)
		{
			originalSize = textMesh.rectTransform.sizeDelta;
		}
		if (originalArrowScale == Vector3.zero)
		{
			originalArrowScale = arrow.localScale;
		}
		else
		{
			arrow.localScale = originalArrowScale;
		}
		arrowTransitionT = 0f;
		meshParent.position = targetPos;
	}

	protected void SizeLines()
	{
		textMesh.rectTransform.sizeDelta = originalSize;
		SwapFont();
		textMesh.ForceMeshUpdate();
		numLines = textMesh.textInfo.lineCount;
		bool flag = false;
		while (numLines > 1 && textMesh.rectTransform.sizeDelta.x > widthStep && !flag)
		{
			textMesh.rectTransform.sizeDelta = new Vector2(textMesh.rectTransform.sizeDelta.x - widthStep, textMesh.rectTransform.sizeDelta.y);
			textMesh.ForceMeshUpdate();
			if (textMesh.textInfo.lineCount > numLines)
			{
				textMesh.rectTransform.sizeDelta = new Vector2(textMesh.rectTransform.sizeDelta.x + widthStep, textMesh.rectTransform.sizeDelta.y);
				textMesh.ForceMeshUpdate();
				flag = true;
				break;
			}
		}
	}

	private void PositionArrow(bool ignoreArrow)
	{
		if (arrow != null && !ignoreArrow)
		{
			if (!arrowOut)
			{
				ScaleArrow(active: true);
			}
			Vector3 vector = characterPos.position - meshParent.position;
			Vector3 vector2 = Vector3.ProjectOnPlane(vector, textMesh.transform.forward);
			SafeDebug.DrawRay(meshParent.transform.position, vector2, Color.black);
			Vector3 position = meshParent.transform.position + vector2 * arrowRadiusScaler;
			arrow.position = position;
			position = arrow.localPosition;
			Vector4 vector3 = default;
			vector3.z = textMesh.transform.localPosition.y + originalSize.y;
			vector3.w = textMesh.transform.localPosition.y - originalSize.y;
			vector3.x = textMesh.transform.localPosition.x + originalSize.x / 2f;
			vector3.y = textMesh.transform.localPosition.x - originalSize.x / 2f;
			position.x = Mathf.Clamp(position.x, vector3.y, vector3.x);
			position.y = Mathf.Clamp(position.y, vector3.w, vector3.z);
			arrow.localPosition = position;
			arrow.rotation = Quaternion.LookRotation(vector2, mainCamera.transform.forward);
		}
		else if (arrow.gameObject.activeInHierarchy && arrowOut)
		{
			ScaleArrow(active: false);
		}
	}

	private void ScaleArrow(bool active)
	{
		if (active)
		{
			if (!ignoreArrowLastFrame)
			{
				arrowTransitionT = 0f;
			}
			arrowTransitionT += Time.deltaTime;
			if (arrowTransitionT < arrowScaleTime)
			{
				arrow.localScale = Vector3.one * (arrowTransitionT * (1f / arrowScaleTime));
			}
			else
			{
				arrow.localScale = Vector3.one;
				arrowOut = true;
				arrowTransitionT = 0f;
			}
		}
		else
		{
			if (ignoreArrowLastFrame)
			{
				arrowTransitionT = 0f;
			}
			arrowTransitionT += Time.deltaTime;
			if (arrowTransitionT < arrowScaleTime)
			{
				arrow.localScale = Vector3.one - Vector3.one * (arrowTransitionT * (1f / arrowScaleTime));
			}
			else
			{
				arrow.localScale = Vector3.zero;
				arrow.gameObject.SetActive(value: false);
				arrowOut = false;
				arrowTransitionT = 0f;
			}
		}
		ignoreArrowLastFrame = active;
	}

	private void ScaleText()
	{
		Vector3 vector = characterPos.position - mainCamera.transform.position;
		float distanceToPoint = ((!onScreen) ? new Plane(mainCamera.transform.forward, mainCamera.transform.position) : new Plane(vector.normalized, mainCamera.transform.position)).GetDistanceToPoint(meshParent.position);
		float num = distanceToPoint * textScaler;
		float t = Mathf.InverseLerp(0f, maxScalingDist, distanceToPoint);
		num *= Mathf.Lerp(1f, scaleAtMaxDist, t);
		meshParent.localScale = Vector3.one * num;
	}

	private float findScaleConstant()
	{
		Vector3 vector = characterPos.position - mainCamera.transform.position;
		float distanceToPoint = ((!onScreen) ? new Plane(mainCamera.transform.forward, mainCamera.transform.position) : new Plane(vector.normalized, mainCamera.transform.position)).GetDistanceToPoint(meshParent.position);
		float num = distanceToPoint * textScaler;
		float t = Mathf.InverseLerp(0f, maxScalingDist, distanceToPoint);
		return num * Mathf.Lerp(1f, scaleAtMaxDist, t);
	}

	private float findScaleConstant(Vector3 endPos)
	{
		Vector3 vector = characterPos.position - mainCamera.transform.position;
		float distanceToPoint = ((!onScreen) ? new Plane(mainCamera.transform.forward, mainCamera.transform.position) : new Plane(vector.normalized, mainCamera.transform.position)).GetDistanceToPoint(endPos);
		float num = distanceToPoint * textScaler;
		float t = Mathf.InverseLerp(0f, maxScalingDist, distanceToPoint);
		return num * Mathf.Lerp(1f, scaleAtMaxDist, t);
	}

	public void SetIsPlaying(bool s)
	{
		isPlaying = s;
		if (!IsPlaying())
		{
			textMesh.enabled = false;
			textMesh.text = string.Empty;
			currentClip = null;
			subtitles = default;
			currentLine = default;
			firstframe = true;
			lineIndex = -1;
		}
		else
		{
			textMesh.enabled = true;
			if (noDialogueSource)
			{
				UpdateCurrentLine(audioSource);
			}
			DisplayText();
		}
	}

	public virtual bool IsPlaying()
	{
		if (PersistentDataManager.GetSubtitleLanguage() != VVR_METASAVEDATA.SubtitlesState.NONE)
		{
			if (forceDisplay)
			{
				return true;
			}
			return isPlaying;
		}
		return false;
	}

	public void SetSubtitleContainer(SubtitleContainer s)
	{
		subtitles = s;
	}

	public void SetCharacter(Transform s)
	{
		characterPos = s;
	}

	public Transform GetMeshRoot()
	{
		return meshParent;
	}

	public float TimelockedDamp(float source, float target, float smoothing, float dt)
	{
		return Mathf.Lerp(source, target, 1f - Mathf.Pow(smoothing, dt));
	}

	public void SetAudioSource(GvrAudioSource source)
	{
		audioSource = source;
	}

	public void SetForceToHead(bool s)
	{
		arrowTransitionT = 0f;
		forceToHead = s;
	}

	public void SwapFont()
	{
		textMesh.font = FontMatContainer.instance.GetFontSet(startFont).GetCurrentFont();
		textMesh.fontSharedMaterial = FontMatContainer.instance.GetMatSet(startMat).GetCurrentMaterial();
	}
}
