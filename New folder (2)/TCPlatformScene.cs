using UnityEngine;

[CreateAssetMenu(fileName = "New Scene Container", menuName = "TenderClaws/Create Scene Container", order = 1)]
public class TCPlatformScene : ScriptableObject
{
	[SerializeField]
	public string Identifier;

	public string Name;

	public new string name => Name;

	public override bool Equals(object obj)
	{
		TCPlatformScene tCPlatformScene = obj as TCPlatformScene;
		return tCPlatformScene.Identifier == Identifier;
	}

	public override int GetHashCode()
	{
		if (Identifier != null)
		{
			return Identifier.GetHashCode();
		}
		return base.GetHashCode();
	}

	public static bool operator ==(TCPlatformScene a, TCPlatformScene b)
	{
		if ((Object)a == (Object)null)
		{
			return (Object)b == (Object)null;
		}
		return b != null && a.Identifier == b.Identifier;
	}

	public static bool operator !=(TCPlatformScene a, TCPlatformScene b)
	{
		return !(a == b);
	}
}
