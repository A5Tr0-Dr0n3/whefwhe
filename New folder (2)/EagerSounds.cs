using UnityEngine;

public class EagerSounds : MonoBehaviour
{
	[Space]
	[Header("UI")]
	public AudioEvent UI_Success;

	public AudioEvent UI_Wrong;

	public AudioEvent UI_Spawn;

	public static EagerSounds instance;

	private void Start()
	{
		instance = this;
	}
}
