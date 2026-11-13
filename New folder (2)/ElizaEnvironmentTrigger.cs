using UnityEngine;

public class ElizaEnvironmentTrigger : MonoBehaviour
{
	public ElizaEnvironments elizaEnviroReference;

	[Space]
	public bool isCenterTrigger;

	[Space]
	public bool isBeachTrigger;

	public bool isCanyonTrigger;

	public bool isVolcanoTrigger;

	public bool isCaveTrigger;

	public bool isSpaceTrigger;

	private bool _playerInRange;

	public bool playerInRange => _playerInRange;

	private void Start()
	{
		if (isCenterTrigger)
		{
			elizaEnviroReference.SetCenterTrigger(this);
		}
		if (isBeachTrigger)
		{
			elizaEnviroReference.SetEndingTrigger(this);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!(other.gameObject.tag == "MainCamera"))
		{
			return;
		}
		_playerInRange = true;
		if (isCenterTrigger)
		{
			elizaEnviroReference.SetUseCamera(newC: true);
		}
		if (isBeachTrigger)
		{
			if (elizaEnviroReference.beachComplete)
			{
				elizaEnviroReference.SetNewArea(0);
				elizaEnviroReference.elizaReference.FadeInMusic();
			}
			else
			{
				elizaEnviroReference.SetNewArea(11);
				elizaEnviroReference.elizaReference.FadeOutMusic();
			}
			if (VVR_Eliza.instance != null && (VVR_Eliza.instance.currentElizaState == VVR_Eliza.ElizaStates.ElizaLookAtSun || VVR_Eliza.instance.currentElizaState == VVR_Eliza.ElizaStates.ElizaWalkToSun))
			{
				VVR_Eliza.instance.StartSunsetMonologue();
			}
		}
		if (isVolcanoTrigger)
		{
			if (elizaEnviroReference.volcanoComplete)
			{
				elizaEnviroReference.SetNewArea(1);
				elizaEnviroReference.elizaReference.FadeInMusic();
			}
			else
			{
				elizaEnviroReference.SetNewArea(11);
				elizaEnviroReference.elizaReference.FadeOutMusic();
			}
		}
		if (isCanyonTrigger)
		{
			if (elizaEnviroReference.canyonComplete)
			{
				elizaEnviroReference.SetNewArea(2);
				elizaEnviroReference.elizaReference.FadeInMusic();
			}
			else
			{
				elizaEnviroReference.SetNewArea(11);
				elizaEnviroReference.elizaReference.FadeOutMusic();
			}
		}
		if (isSpaceTrigger)
		{
			if (elizaEnviroReference.spaceComplete)
			{
				elizaEnviroReference.SetNewArea(3);
				elizaEnviroReference.elizaReference.FadeInMusic();
			}
			else
			{
				elizaEnviroReference.SetNewArea(11);
				elizaEnviroReference.elizaReference.FadeOutMusic();
			}
		}
		if (isCaveTrigger)
		{
			if (elizaEnviroReference.caveComplete)
			{
				elizaEnviroReference.SetNewArea(4);
				elizaEnviroReference.elizaReference.FadeInMusic();
			}
			else
			{
				elizaEnviroReference.SetNewArea(11);
				elizaEnviroReference.elizaReference.FadeOutMusic();
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "MainCamera")
		{
			_playerInRange = false;
		}
	}
}
