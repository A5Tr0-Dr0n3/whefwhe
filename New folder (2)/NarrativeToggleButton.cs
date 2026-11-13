using UnityEngine;

public class NarrativeToggleButton : ObjectToggle
{
	public string label;

	public string subTitle;

	public string description;

	public SaveDataScriptableObject mySaveState;

	[SerializeField]
	private NarrativeSceneLoader loader;

	private static int lineLength = 55;

	protected override void Awake()
	{
		base.Awake();
		mySaveState = Object.Instantiate(mySaveState);
		onToggled += pushSave;
		if (label == string.Empty)
		{
			label = gameObject.name;
		}
		if (subTitle == string.Empty)
		{
			subTitle = label + " subtitle";
		}
		if (description == string.Empty)
		{
			description = label + " description content";
		}
	}

	private string SetDescriptionText(string newResponse)
	{
		string[] array = newResponse.Split(" "[0]);
		string text = string.Empty;
		string text2 = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			string text3 = text2 + " " + array[i];
			if (text3.Length > lineLength)
			{
				text = text + text2 + "\n";
				text2 = array[i];
			}
			else
			{
				text2 = text3;
			}
		}
		text += text2;
		return text.Substring(1, text.Length - 1);
	}

	private void pushSave(ObjectToggle toggledObject, bool newValue, bool playerTriggered)
	{
		if (newValue)
		{
			int num = 0;
			num = (PersistentDataManager.GetCanSkipMerge() ? 1 : 0);
			if (mySaveState.saveData.arbitraryIntDictionary.ContainsKey("IVE BEEN HERE BEFORE"))
			{
				mySaveState.saveData.arbitraryIntDictionary["IVE BEEN HERE BEFORE"] = num;
			}
			else
			{
				mySaveState.saveData.arbitraryIntDictionary.Add("IVE BEEN HERE BEFORE", num);
			}
			loader.OverwriteSaveWithButton(mySaveState.saveData, this, playerTriggered);
		}
	}
}
