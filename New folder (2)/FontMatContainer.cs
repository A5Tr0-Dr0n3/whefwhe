using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FontMatContainer : MonoBehaviour
{
	public FontSet[] fontSets;

	public MatSet[] matSets;

	[HideInInspector]
	public FontDictionary fontSetDictionary;

	[HideInInspector]
	public MatDictionary matSetDictionary;

	public Dictionary<string, FontSet> constructedFontSetDictionary;

	public Dictionary<string, MatSet> constructedMatSetDictionary;

	public static FontMatContainer instance;

	public void CreateFontDictionary()
	{
		fontSetDictionary = new FontDictionary();
		for (int i = 0; i < fontSets.Length; i++)
		{
			fontSetDictionary.Add(fontSets[i].latinFont, fontSets[i]);
		}
	}

	public void CreateMatDictionary()
	{
		matSetDictionary = new MatDictionary();
		for (int i = 0; i < matSets.Length; i++)
		{
			matSetDictionary.Add(matSets[i].latinMat, matSets[i]);
		}
	}

	public FontSet GetFontSet(TMP_FontAsset key)
	{
		if (constructedFontSetDictionary.ContainsKey(key.name))
		{
			return constructedFontSetDictionary[key.name];
		}
		SafeDebug.LogError("Font Dictionary does not contain: " + key.name);
		return new FontSet();
	}

	public MatSet GetMatSet(Material key)
	{
		if (constructedMatSetDictionary.ContainsKey(key.name))
		{
			return constructedMatSetDictionary[key.name];
		}
		SafeDebug.LogError("Mat Dictionary does not contain: " + key.name);
		return new MatSet();
	}

	public void ClearDictionaries()
	{
		fontSetDictionary.Clear();
		matSetDictionary.Clear();
		fontSetDictionary = null;
		matSetDictionary = null;
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			SafeDebug.LogError("You have more than one subtitle loader. I'm deleting myself");
			Object.Destroy(gameObject);
		}
		ConstructDictionaries();
		ClearDictionaries();
	}

	private void ConstructDictionaries()
	{
		constructedFontSetDictionary = new Dictionary<string, FontSet>();
		foreach (KeyValuePair<TMP_FontAsset, FontSet> item in fontSetDictionary)
		{
			if (!constructedFontSetDictionary.ContainsKey(item.Key.name))
			{
				constructedFontSetDictionary.Add(item.Key.name, item.Value);
			}
		}
		constructedMatSetDictionary = new Dictionary<string, MatSet>();
		foreach (KeyValuePair<Material, MatSet> item2 in matSetDictionary)
		{
			if (!constructedMatSetDictionary.ContainsKey(item2.Key.name))
			{
				constructedMatSetDictionary.Add(item2.Key.name, item2.Value);
			}
		}
	}
}
