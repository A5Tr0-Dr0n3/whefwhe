using TMPro;
using UnityEngine;

public class GraphicsSelectButton : MonoBehaviour
{
	public StandaloneGraphicsMode mMode;

	public TextMeshPro label;

	public ObjectButton button;

	public Color opaque = Vector4.one;

	public Color semiOpaque = new Vector4(1f, 1f, 1f, 0.6f);

	public Color selected = Vector4.one;

	public GraphicsSelectionMenu menu;

	internal bool isCurrentSetting;

	private FontStyles startFontStyle;

	internal bool highlight;

	public bool Highlight
	{
		get
		{
			return highlight;
		}
		set
		{
			if (isCurrentSetting)
			{
				label.color = selected;
				label.fontStyle = FontStyles.Bold;
			}
			else if (value)
			{
				label.color = opaque;
				label.fontStyle = startFontStyle;
			}
			else
			{
				label.color = semiOpaque;
				label.fontStyle = startFontStyle;
			}
			highlight = value;
		}
	}

	private void Start()
	{
		startFontStyle = label.fontStyle;
		button.onClickEvent.AddListener(Select);
		button.onHoverEnter.AddListener(() =>
		{
			Highlight = true;
		});
		button.onHoverExit.AddListener(() =>
		{
			Highlight = false;
		});
	}

	public void Select()
	{
		if (menu != null)
		{
			menu.SetGraphicsMode(mMode, this);
		}
	}
}
