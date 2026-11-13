using UnityEngine;

[CreateAssetMenu(fileName = "New End Survey Sequence Content", menuName = "TenderClaws/Create End Survey Sequence Content", order = 1)]
public class SurveySequenceScriptableObject : ScriptableObject
{
	[Header("Survey Text")]
	public string surveyIntroMessageJoin;

	public string surveyIntroMessageReset;

	public SurveyQuestion[] surveyQuestionsJoin;

	public SurveyQuestion[] surveyQuestionsReset;

	public string surveyEndMessage;

	[Header("Survey Timings")]
	public float inbetweenShowTime;

	public float checkShowTime;

	public float endScreenShowTime;
}
