using UnityEngine;

public class HideOnPC : MonoBehaviour
{
	[ExecuteInEditMode]
	private void Start()
	{
		transform.gameObject.SetActive(value: false);
	}
}
