using System;
using System.IO;
using System.Text;
using UnityEngine;

public class UniqueCharacters : MonoBehaviour
{
	public string inputDir;

	public string[] inputs;

	public string outputDir;

	public string output;

	public void Load()
	{
		string text = " ";
		for (int i = 0; i < inputs.Length; i++)
		{
			string path = Application.dataPath + inputDir + '/' + inputs[i] + ".txt";
			StreamReader streamReader = new StreamReader(path, Encoding.Default);
			using (streamReader)
			{
				for (string text2 = streamReader.ReadLine(); text2 != null; text2 = streamReader.ReadLine())
				{
					for (int j = 0; j < text2.Length; j++)
					{
						char c = text2.Substring(j, 1).ToCharArray()[0];
						if (text.IndexOf(c) == -1)
						{
							text += c;
						}
					}
				}
			}
		}
		char[] array = text.ToCharArray();
		Array.Sort(array);
		text = new string(array);
		string text3 = string.Concat(Application.dataPath + outputDir, '/', output, ".txt");
		SafeDebug.Log("Saved  " + text.Length + " unique characters to " + text3);
		File.WriteAllText(text3, text);
	}
}
