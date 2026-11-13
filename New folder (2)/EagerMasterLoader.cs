using UnityEngine;
using UnityEngine.SceneManagement;

public class EagerMasterLoader : MonoBehaviour
{
	private void Awake()
	{
		if (EagerMaster.instance == null)
		{
			SceneManager.LoadScene("VVR_ROOT", LoadSceneMode.Additive);
		}
		transform.parent.gameObject.SetActive(value: false);
		Object.Destroy(transform.parent.gameObject);
	}
}
