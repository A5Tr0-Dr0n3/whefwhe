using UnityEngine;

[CreateAssetMenu(fileName = "New Save State", menuName = "TenderClaws/Create Save State", order = 1)]
public class SaveDataScriptableObject : ScriptableObject
{
	public VVR_SAVEDATA saveData;

	public SaveDataScriptableObject()
	{
		saveData = new VVR_SAVEDATA(-1, fromScriptableObjectConstructor: true);
	}
}
