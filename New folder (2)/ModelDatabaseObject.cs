using UnityEngine;

[CreateAssetMenu(fileName = "New Multiplatform Sequence Content", menuName = "TenderClaws/Actions/Create Multiplatform Sequence Content", order = 1)]
public class ModelDatabaseObject : ScriptableObject
{
	[Header("Controller prefab")]
	public MultiplatformControllerManager ControllerModel;

	[Header("World prefabs")]
	public GameObject WorldModel;

	public GameObject WorldModelHacked;

	[Header("World Model Tweakables")]
	public Vector3 LocalPosOffset;

	public Vector3 HackedLocalPosOffset;

	public float LocalScaleScaler = 1f;

	public float HackedLocalScaleScaler = 1f;

	[Header("Gear stuff")]
	public GameObject GearWorldModel;

	public GameObject GearWorldModelHacked;

	public Vector3 GearLocalPosOffset;

	public Vector3 HackedGearLocalPosOffset;

	public float GearLocalScaleScaler = 1f;

	public float HackedGearLocalScaleScaler = 1f;
}
