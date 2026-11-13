using UnityEngine;

[RequireComponent(typeof(VVRConsumable))]
public class PlayOnConsume : MonoBehaviour
{
	public AudioEvent toPlay;

	private VVRConsumable consumable;

	private bool played;

	private void Start()
	{
		consumable = GetComponent<VVRConsumable>();
		if (toPlay == null)
		{
			toPlay = GetComponent<AudioEvent>();
		}
	}

	private void Update()
	{
		if (!played && consumable.currentObjectState == VVRConsumable.objectState.partiallyConsumed)
		{
			played = true;
			toPlay.Play();
		}
	}
}
