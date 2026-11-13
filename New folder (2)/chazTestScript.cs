using UnityEngine;

public class chazTestScript : MonoBehaviour
{
	public ChazController chaz;

	public GameObject presentHeadset;

	public Transform chazFollow;

	public GameObject grabbableHeadset;

	public GameObject beeCage;

	public GameObject beetleBall;

	public ChazController beetleChaz;

	private float timer;

	private bool called1;

	private bool addingBirdcage;

	private void Start()
	{
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_LVL_RADIOBAIT);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_LVL_JOBKITCHEN);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_LVL_JOBGARDEN);
		EagerScenes.instance.LoadQueue(TCScenes.S.VVR_LVL_TURBINE);
		ToolManager.AllManipulators.HasVacuum = true;
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer > 5f && !called1)
		{
			called1 = true;
			if (beetleChaz != null)
			{
				beetleChaz.WalkTo(Vector3.MoveTowards(beetleChaz.head.transform.position, Camera.main.transform.position, 30f));
			}
		}
		if (Vector3.Distance(beetleBall.transform.position, Camera.main.transform.position) < 1.6f && !beeCage.activeSelf)
		{
			ToolManager.StopTeleport();
			Teleporter.SetTeleportEnabled(enabledIn: false);
			addingBirdcage = true;
		}
		if (Vector3.Distance(beetleBall.transform.position, Camera.main.transform.position) > 2f && addingBirdcage && !beeCage.activeSelf)
		{
			beeCage.transform.position = EagerMaster.instance.player.transform.position;
			beeCage.SetActive(value: true);
		}
		if (timer > 8f)
		{
			timer = 0f;
			called1 = false;
		}
	}
}
