using UnityEngine;

public class TrailerScript : MonoBehaviour
{
	private void Start()
	{
		SafeDebug.LogError("KILL THIS SCRIPT");
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_BACKSTAGE_ELEVATOR);
		EagerScenes.instance.LoadQueue(EagerScenes.BACKSTAGE_LEVEL);
		EagerScenes.instance.LoadQueue(EagerScenes.HUB_LEVEL);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_ARCHIVE);
		TCPlatformScene vVR_LVL_TURBINE = TCScenes.S.VVR_LVL_TURBINE;
		TCPlatformScene vVR_LVL_UNDERWATER = TCScenes.S.VVR_LVL_UNDERWATER;
		TCPlatformScene vVR_LVL_GARDEN_VIGNETTE = TCScenes.S.VVR_LVL_GARDEN_VIGNETTE;
		TCPlatformScene vVR_LVL_RADIO = TCScenes.S.VVR_LVL_RADIO;
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_LVL_JOBKITCHEN);
		EagerScenes.instance.LoadQueue(vVR_LVL_TURBINE);
		EagerScenes.instance.LoadQueue(vVR_LVL_UNDERWATER);
		EagerScenes.instance.LoadQueue(vVR_LVL_GARDEN_VIGNETTE);
		EagerScenes.instance.LoadQueue(vVR_LVL_RADIO);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_LVL_PHONEBOOTH);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_LVL_RADIOBAIT);
	}
}
