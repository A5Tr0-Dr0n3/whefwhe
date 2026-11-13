using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RootDelayLoader : MonoBehaviour
{
	public static bool loadedFromLogo;

	public List<GameObject> rootSceneObjects;

	public static RootDelayLoader instance;

	private void Awake()
	{
		SafeDebug.Log("Loaded root");
		SceneManager.activeSceneChanged += CheckReEnableRoot;
		instance = this;
		if (loadedFromLogo)
		{
			rootSceneObjects.ForEach((GameObject g) =>
			{
				g.SetActive(value: false);
			});
			loadedFromLogo = false;
		}
	}

	public void CheckReEnableRoot(Scene oldScene, Scene newScene)
	{
		if (newScene.name == "VVR_ROOT")
		{
			ReEnableRoot();
		}
	}

	public void ReEnableRoot()
	{
		rootSceneObjects.ForEach((GameObject g) =>
		{
			g.SetActive(value: true);
		});
	}
}
