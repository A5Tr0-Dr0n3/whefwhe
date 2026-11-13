using System;
using System.Collections.Generic;

[Serializable]
public struct SubtitleContainer
{
	public string key;

	public List<SubtitleLine> dialogueLines;

	public void LoadDialogueFromFile(string filename)
	{
	}
}
