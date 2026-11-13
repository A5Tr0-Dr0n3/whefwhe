using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VVRLogDisplay : MonoBehaviour
{
	[Serializable]
	public class Log
	{
		public string message;

		public string stackTrace;

		public LogType logType;

		public int repeatCount = 1;

		public Log(string messageIn, string stackTraceIn, LogType logTypeIn)
		{
			message = messageIn;
			stackTrace = stackTraceIn;
			logType = logTypeIn;
			repeatCount = 1;
		}
	}

	[SerializeField]
	private CanvasGroup globalVisibilityCanvas;

	[SerializeField]
	private Text logTextField;

	[SerializeField]
	private Text fpsTextField;

	[SerializeField]
	private ScrollRect logTextScroller;

	[SerializeField]
	private float scrollSensitivity = 80f;

	private bool activated;

	private List<Log> logs;

	private const string DISPLAY_TEXT_FORMAT = "{0} msf\n({1} FPS)";

	private const string MSF_FORMAT = "#.#";

	private const float MS_PER_SEC = 1000f;

	private float fps = 60f;

	private void Awake()
	{
		logs = new List<Log>();
		if (PersistentDataManager.GetDebugPanelType() == DebugPanelType.LogsAndFps)
		{
			Application.logMessageReceived += handleLog;
		}
		else if (PersistentDataManager.GetDebugPanelType() != DebugPanelType.Fps)
		{
		}
		Activate(activeIn: false);
	}

	public void ToggleActive()
	{
		Activate(!activated);
	}

	public void Activate(bool activeIn)
	{
		activated = activeIn;
		globalVisibilityCanvas.gameObject.SetActive(activated);
		SetVisibility(activated);
	}

	public void ClearLogs()
	{
		logs = new List<Log>();
		displayLogs();
	}

	private void Update()
	{
	}

	public void SetVisibility(bool newVis)
	{
		globalVisibilityCanvas.alpha = ((!newVis) ? 0f : 1f);
	}

	private void addMessage(Log newMessage)
	{
		for (int i = 0; i < logs.Count; i++)
		{
			if (newMessage.message == logs[i].message && newMessage.stackTrace == logs[i].stackTrace)
			{
				logs[i].repeatCount++;
				return;
			}
		}
		logs.Add(newMessage);
	}

	private void handleLog(string message, string stackTrace, LogType type)
	{
		addMessage(new Log(message, stackTrace, type));
		displayLogs();
	}

	private void displayLogs()
	{
		logTextField.text = string.Empty;
		for (int i = 0; i < logs.Count; i++)
		{
			string text = "<color=828282FF>";
			string text2 = "</color>";
			string text3 = string.Empty;
			switch (logs[i].logType)
			{
			case LogType.Log:
				text = "<color=#727272FF>L: ";
				break;
			case LogType.Warning:
				text = "<color=#F0D43AFF>W:";
				break;
			case LogType.Error:
				text = "<color=#E25130FF>E:";
				text3 = "<color=#440000FF>\n" + logs[i].stackTrace + "</color>";
				break;
			case LogType.Exception:
				text = "<color=#E25130FF>E:";
				text3 = "<color=#440000FF>\n" + logs[i].stackTrace + "</color>";
				break;
			}
			Text text4 = logTextField;
			string text5 = text4.text;
			text4.text = text5 + text + logs[i].message + " (" + logs[i].repeatCount + ")" + text2 + text3 + '\n';
		}
	}

	private void updateFPS()
	{
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		float t = unscaledDeltaTime / (0.5f + unscaledDeltaTime);
		float b = 1f / unscaledDeltaTime;
		fps = Mathf.Lerp(fps, b, t);
		float num = 1000f / fps;
		fpsTextField.text = string.Format("{0} msf\n({1} FPS)", num.ToString("#.#"), Mathf.RoundToInt(fps));
	}
}
