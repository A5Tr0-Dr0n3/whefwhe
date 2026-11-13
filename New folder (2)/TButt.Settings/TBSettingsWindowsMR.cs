using UnityEngine;

namespace TButt.Settings;

public class TBSettingsWindowsMR : TBSettingsBase
{
	public override void Initialize()
	{
		base.Initialize();
		if (GetRefreshRate() < 90f)
		{
			QualitySettings.SetQualityLevel(4, applyExpensiveChanges: true);
		}
	}
}
