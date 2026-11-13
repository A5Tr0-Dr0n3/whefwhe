using UnityEngine;

public class AestheticObjectDatabase : MonoBehaviour
{
	private static AestheticObjectDatabase _instance;

	public AestheticObjMeshSet meshSet = new AestheticObjMeshSet();

	public static AestheticObjectDatabase instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType<AestheticObjectDatabase>();
			}
			return _instance;
		}
	}
}
