using UnityEngine;

public class MetaSaveDataScriptableObject : ScriptableObject
{
	public VVR_METASAVEDATA metaSaveData;

	public MetaSaveDataScriptableObject()
	{
		metaSaveData = new VVR_METASAVEDATA();
	}
}
