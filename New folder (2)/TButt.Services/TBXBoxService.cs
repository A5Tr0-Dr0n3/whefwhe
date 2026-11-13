using System.Collections.Generic;

namespace TButt.Services;

public class TBXBoxService : TBServiceBase
{
	public static readonly string serviceFilename = "ServiceXbox";

	public static readonly string xboxAchievementIDsFilename = "ServiceXboxAchievements";

	private static XboxIDs _ids;

	public static List<string> GetXboxAchievementIDs()
	{
		string[] array = TBDataManager.FromJsonWrapper(TBDataManager.DeserializeFromFile<TBDataManager.Wrapper<string>>(TBSettings.settingsFolder + xboxAchievementIDsFilename, TBDataManager.PathType.ResourcesFolder));
		if (array != null)
		{
			return new List<string>(array);
		}
		return new List<string>();
	}

	public static XboxIDs GetXboxIDs()
	{
		return TBDataManager.DeserializeFromFile<XboxIDs>(TBSettings.settingsFolder + serviceFilename, TBDataManager.PathType.ResourcesFolder);
	}
}
