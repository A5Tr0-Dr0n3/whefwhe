using UnityEngine;

public class AestheticObject : MonoBehaviour
{
	private MeshFilter mMeshFilter;

	[SerializeField]
	private bool tutorialObj;

	[SerializeField]
	private TutorialLevelSequence.AestheticObjects overrideObjectType;

	[SerializeField]
	private bool rootedPlant;

	[SerializeField]
	private bool endGameItem;

	private void Start()
	{
		Mesh aestheticObjectMesh = AestheticObjectDatabase.instance.meshSet.GetAestheticObjectMesh(PersistentDataManager.GetAestheticObject(), PersistentDataManager.GetBeeslot());
		mMeshFilter = GetComponent<MeshFilter>();
		if (mMeshFilter == null)
		{
			SafeDebug.LogError("The aesthetic object " + gameObject.name + " does not have a mesh filter");
		}
		if (mMeshFilter.sharedMesh != aestheticObjectMesh)
		{
			mMeshFilter.sharedMesh = aestheticObjectMesh;
		}
		if (!tutorialObj)
		{
			if (PersistentDataManager.GetAestheticObject() == TutorialLevelSequence.AestheticObjects.Plant)
			{
				mMeshFilter.sharedMesh = AestheticObjectDatabase.instance.meshSet.GetAestheticObjectMesh(TutorialLevelSequence.AestheticObjects.Fish, PersistentDataManager.GetBeeslot());
			}
			else if (!rootedPlant)
			{
				mMeshFilter.sharedMesh = AestheticObjectDatabase.instance.meshSet.GetAestheticObjectMesh(TutorialLevelSequence.AestheticObjects.Plant, PersistentDataManager.GetBeeslot());
			}
			else
			{
				mMeshFilter.sharedMesh = AestheticObjectDatabase.instance.meshSet.GetAestheticObjectMesh(TutorialLevelSequence.AestheticObjects.Plant, PersistentDataManager.GetBeeslot(), useUnpottedPlants: true);
			}
			return;
		}
		int num = PersistentDataManager.GetBeeslot();
		if (endGameItem)
		{
			num++;
			if (num > 2)
			{
				num = 0;
			}
		}
		mMeshFilter.sharedMesh = AestheticObjectDatabase.instance.meshSet.GetAestheticObjectMesh(overrideObjectType, num);
		if (!rootedPlant)
		{
			mMeshFilter.sharedMesh = AestheticObjectDatabase.instance.meshSet.GetAestheticObjectMesh(overrideObjectType, num);
		}
		else
		{
			mMeshFilter.sharedMesh = AestheticObjectDatabase.instance.meshSet.GetAestheticObjectMesh(overrideObjectType, num, useUnpottedPlants: true);
		}
	}
}
