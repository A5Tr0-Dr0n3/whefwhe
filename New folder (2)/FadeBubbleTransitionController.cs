using UnityEngine;

public class FadeBubbleTransitionController : MonoBehaviour
{
	[SerializeField]
	private Renderer bubbleRenderer;

	private static FadeBubbleTransitionController _instance;

	private static FadeBubbleTransitionController instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType<FadeBubbleTransitionController>();
			}
			return _instance;
		}
		set
		{
			_instance = value;
		}
	}

	private void initialize()
	{
		if (_instance == null)
		{
			_instance = this;
		}
	}

	private void Awake()
	{
		initialize();
	}

	public static void MakeFullyOpaque()
	{
		SetFadeT(1f);
	}

	public static void MakeFullyTransparent()
	{
		SetFadeT(0f);
	}

	public static void SetFadeT(float t)
	{
		instance.bubbleRenderer.sharedMaterial.SetColor("_Color", new Color(0f, 0f, 0f, t));
		instance.bubbleRenderer.gameObject.SetActive(t != 0f);
		instance.transform.position = EagerMaster.instance.mainCamera.transform.position;
	}

	private void LateUpdate()
	{
		bubbleRenderer.transform.position = EagerMaster.instance.mainCamera.transform.position;
	}
}
