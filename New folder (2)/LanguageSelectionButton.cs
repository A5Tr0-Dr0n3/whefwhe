using TMPro;
using UnityEngine;

[RequireComponent(typeof(ObjectButton))]
public class LanguageSelectionButton : MonoBehaviour
{
	public TextMeshPro label;

	public VVR_METASAVEDATA.SubtitlesState language;

	[HideInInspector]
	public LanguageSelectionMenu menu;

	private ObjectButton button;

	public Color opaque = Vector4.one;

	public Color semiOpaque = new Vector4(1f, 1f, 1f, 0.6f);

	private bool highlight;

	public bool Highlight
	{
		get
		{
			return highlight;
		}
		set
		{
			if (value)
			{
				label.color = opaque;
			}
			else
			{
				label.color = semiOpaque;
			}
			highlight = value;
		}
	}

	private void Start()
	{
		button = GetComponent<ObjectButton>();
		button.onClickEvent.AddListener(Select);
		button.onHoverEnter.AddListener(() =>
		{
			Highlight = true;
		});
		button.onHoverExit.AddListener(() =>
		{
			if (this != menu.currentLanguage)
			{
				Highlight = false;
			}
		});
	}

	public void Select()
	{
		SafeDebug.Log("newlanguageselected");
		if (menu != null)
		{
			menu.SetCurrentLanguage(language);
		}
	}
}
