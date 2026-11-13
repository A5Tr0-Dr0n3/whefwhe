using System.Collections.Generic;

namespace TButt.Services;

public class TBPSNService : TBServiceBase
{
	public static readonly string serviceFilename = "ServicePSN";

	public static readonly string psnTrophyIDsFilename = "ServicePSNTrophies";

	public static List<string> GetPSNTrophyIDs()
	{
		return new List<string>(TBDataManager.FromJsonWrapper(TBDataManager.DeserializeFromFile<TBDataManager.Wrapper<string>>(TBSettings.settingsFolder + psnTrophyIDsFilename, TBDataManager.PathType.ResourcesFolder)));
	}
}
