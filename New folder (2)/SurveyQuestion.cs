using System;

[Serializable]
public class SurveyQuestion
{
	public enum SurveyQuestionType
	{
		FiveResponse,
		FourResponse,
		ThreeResponse,
		TwoResponse,
		OneResponse
	}

	public SurveyQuestionType questionType;

	public string questionText;

	public string[] questionResponses;
}
