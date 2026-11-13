using UnityEngine;

public class SceneLoader : MonoBehaviour
{
	public string levelToLoad = string.Empty;

	public static SceneLoader instance;

	public Animator anim;

	public string introName = "FadeIn";

	public string outroName = "FadeOut";

	public GameObject newScene;

	private void Awake()
	{
		if (instance != null)
		{
			Object.Destroy(gameObject);
		}
		instance = this;
	}

	public void SwitchScene()
	{
		EagerMaster.instance.player.transform.position = newScene.transform.position;
		newScene.SetActive(value: true);
	}
}
