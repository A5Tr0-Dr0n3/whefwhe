using System;
using UnityEngine;

[Serializable]
public class AestheticObjMeshSet
{
	[SerializeField]
	private MeshFilter[] plantMeshes = new MeshFilter[3];

	[SerializeField]
	private MeshFilter[] ballMeshes = new MeshFilter[3];

	[SerializeField]
	private MeshFilter[] fishMeshes = new MeshFilter[3];

	[SerializeField]
	private MeshFilter[] plantObjectBackgroundSpheres = new MeshFilter[3];

	[SerializeField]
	private MeshFilter[] ballObjectBackgroundSpheres = new MeshFilter[3];

	[SerializeField]
	private MeshFilter[] fishObjectBackgroundSpheres = new MeshFilter[3];

	public Mesh GetAestheticObjectMesh(TutorialLevelSequence.AestheticObjects obj, int beeSlot, bool useUnpottedPlants = false)
	{
		if (beeSlot > 2)
		{
			SafeDebug.LogError("Attempting to use a bee slot greater than 2. GetAestheticObjectMesh does not support this. Set the useUnpottedPlants parameter to true if you want an unpotted plant.");
		}
		if (useUnpottedPlants && obj == TutorialLevelSequence.AestheticObjects.Plant)
		{
			beeSlot += 3;
		}
		switch (obj)
		{
		case TutorialLevelSequence.AestheticObjects.Plant:
			return plantMeshes[beeSlot].sharedMesh;
		case TutorialLevelSequence.AestheticObjects.Ball:
			return ballMeshes[beeSlot].sharedMesh;
		case TutorialLevelSequence.AestheticObjects.Fish:
			return fishMeshes[beeSlot].sharedMesh;
		default:
			SafeDebug.LogError("There isn't a saved aesthetic object defaulting to Plant");
			return plantMeshes[beeSlot].sharedMesh;
		}
	}

	public Mesh GetAestheticBackgroundSphere(TutorialLevelSequence.AestheticObjects obj, int beeSlot)
	{
		if (beeSlot > 2)
		{
			SafeDebug.LogError("Attempting to use a bee slot greater than 2. GetAestheticObjectMesh does not support this.");
			return null;
		}
		switch (obj)
		{
		case TutorialLevelSequence.AestheticObjects.Plant:
			return plantObjectBackgroundSpheres[beeSlot].sharedMesh;
		case TutorialLevelSequence.AestheticObjects.Ball:
			return ballObjectBackgroundSpheres[beeSlot].sharedMesh;
		case TutorialLevelSequence.AestheticObjects.Fish:
			return fishObjectBackgroundSpheres[beeSlot].sharedMesh;
		default:
			SafeDebug.LogError("There isn't a saved aesthetic object defaulting to Plant");
			return plantObjectBackgroundSpheres[beeSlot].sharedMesh;
		}
	}
}
