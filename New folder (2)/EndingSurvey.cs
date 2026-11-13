using System.Collections;
using UnityEngine;

public class EndingSurvey : MonoBehaviour
{
	public enum SurveyStates
	{
		SurveyIdle,
		SurveyQuestions,
		SurveyComplete,
		SurveyOff
	}

	[SerializeField]
	private SurveyStates _currentSurveyState;

	public SurveySequenceScriptableObject tweakables;

	private bool advanceQuestion;

	public bool isMurderEnding;

	public bool isJoinEnding;

	[Header("Screen Assignments")]
	public GameObject startScreen;

	public GameObject questionScreen;

	public GameObject inbetweenScreen;

	public GameObject endScreen;

	[Header("Text Fields")]
	public TextSwapper questionScreenTextMesh;

	public int questionLineLength = 14;

	public ObjectButton startButton;

	public SurveyButton responseButton;

	public SurveyButton[] responseButtonTwoResponse;

	public SurveyButton[] responseButtonThreeResponse;

	public SurveyButton[] responseButtonFourResponse;

	public SurveyButton[] responseButtonFiveResponse;

	public GameObject responseTwoButtons;

	public GameObject responseThreeButtons;

	public GameObject responseFourButtons;

	public GameObject responseFiveButtons;

	public ObjectTrigger playerCheck;

	private bool initialized;

	public SurveyStates currentSurveyState => _currentSurveyState;

	public void SetupSurveyRoom()
	{
		if (!isMurderEnding && !isJoinEnding)
		{
			TurnOffSurveyObjects();
			return;
		}
		SetUpSceneObjects();
		GlobalCoroutineContainer.AddThread(RunSurvey(), this);
	}

	private void SetUpSceneObjects()
	{
		advanceQuestion = false;
		startScreen.gameObject.SetActive(value: true);
		questionScreen.gameObject.SetActive(value: false);
		inbetweenScreen.gameObject.SetActive(value: false);
		endScreen.gameObject.SetActive(value: false);
		if (!initialized)
		{
			startButton.onClickEvent.AddListener(StartButtonPress);
			AddSurveyListeners();
			_currentSurveyState = SurveyStates.SurveyIdle;
		}
		initialized = true;
	}

	private void TurnOffSurveyObjects()
	{
		_currentSurveyState = SurveyStates.SurveyOff;
		advanceQuestion = false;
		startScreen.gameObject.SetActive(value: false);
		questionScreen.gameObject.SetActive(value: false);
		inbetweenScreen.gameObject.SetActive(value: false);
		endScreen.gameObject.SetActive(value: false);
		TurnOffAllButtons();
		if (initialized)
		{
			startButton.onClickEvent.RemoveListener(StartButtonPress);
			RemoveSurveyListeners();
		}
	}

	private IEnumerator RunSurvey()
	{
		while (_currentSurveyState == SurveyStates.SurveyIdle)
		{
			yield return null;
		}
		if (_currentSurveyState == SurveyStates.SurveyQuestions)
		{
			startScreen.gameObject.SetActive(value: false);
			if (isJoinEnding)
			{
				for (int i = 0; i < tweakables.surveyQuestionsJoin.Length; i++)
				{
					yield return new WaitForSeconds(tweakables.inbetweenShowTime);
					questionScreenTextMesh.SetNewKey(tweakables.surveyQuestionsJoin[i].questionText);
					SetUpCurrentQuestion(tweakables.surveyQuestionsJoin[i]);
					questionScreen.gameObject.SetActive(value: true);
					while (!advanceQuestion)
					{
						yield return null;
					}
					questionScreen.transform.parent.GetComponent<GvrAudioSource>().Play();
					advanceQuestion = false;
					questionScreen.gameObject.SetActive(value: false);
					yield return new WaitForSeconds(tweakables.inbetweenShowTime);
					inbetweenScreen.gameObject.SetActive(value: true);
					yield return new WaitForSeconds(tweakables.checkShowTime);
					inbetweenScreen.gameObject.SetActive(value: false);
				}
			}
			else
			{
				for (int j = 0; j < tweakables.surveyQuestionsReset.Length; j++)
				{
					yield return new WaitForSeconds(tweakables.inbetweenShowTime);
					questionScreenTextMesh.SetNewKey(tweakables.surveyQuestionsReset[j].questionText);
					SetUpCurrentQuestion(tweakables.surveyQuestionsReset[j]);
					questionScreen.gameObject.SetActive(value: true);
					while (!advanceQuestion)
					{
						yield return null;
					}
					questionScreen.transform.parent.GetComponent<GvrAudioSource>().Play();
					advanceQuestion = false;
					questionScreen.gameObject.SetActive(value: false);
					yield return new WaitForSeconds(tweakables.inbetweenShowTime);
					inbetweenScreen.gameObject.SetActive(value: true);
					yield return new WaitForSeconds(tweakables.checkShowTime);
					inbetweenScreen.gameObject.SetActive(value: false);
				}
			}
			yield return new WaitForSeconds(tweakables.inbetweenShowTime);
			_currentSurveyState = SurveyStates.SurveyComplete;
		}
		if (_currentSurveyState == SurveyStates.SurveyComplete)
		{
			endScreen.gameObject.SetActive(value: true);
			endScreen.GetComponent<GvrAudioSource>().Play();
			yield return new WaitForSeconds(tweakables.endScreenShowTime);
			endScreen.gameObject.SetActive(value: false);
			_currentSurveyState = SurveyStates.SurveyOff;
		}
	}

	private void SetUpCurrentQuestion(SurveyQuestion targetQuestion)
	{
		TurnOffAllButtons();
		if (targetQuestion.questionType == SurveyQuestion.SurveyQuestionType.OneResponse)
		{
			responseButton.SetResponseText(targetQuestion.questionResponses[0]);
			responseButton.responseButton.hasBeenPressed = false;
			responseButton.gameObject.SetActive(value: true);
		}
		if (targetQuestion.questionType == SurveyQuestion.SurveyQuestionType.TwoResponse)
		{
			for (int i = 0; i < responseButtonTwoResponse.Length; i++)
			{
				responseButtonTwoResponse[i].SetResponseText(targetQuestion.questionResponses[i]);
				responseButtonTwoResponse[i].gameObject.SetActive(value: true);
				responseButtonTwoResponse[i].responseButton.hasBeenPressed = false;
				responseTwoButtons.SetActive(value: true);
			}
		}
		if (targetQuestion.questionType == SurveyQuestion.SurveyQuestionType.ThreeResponse)
		{
			for (int j = 0; j < responseButtonThreeResponse.Length; j++)
			{
				responseButtonThreeResponse[j].SetResponseText(targetQuestion.questionResponses[j]);
				responseButtonThreeResponse[j].gameObject.SetActive(value: true);
				responseButtonThreeResponse[j].responseButton.hasBeenPressed = false;
				responseThreeButtons.SetActive(value: true);
			}
		}
		if (targetQuestion.questionType == SurveyQuestion.SurveyQuestionType.FourResponse)
		{
			for (int k = 0; k < responseButtonFourResponse.Length; k++)
			{
				responseButtonFourResponse[k].SetResponseText(targetQuestion.questionResponses[k]);
				responseButtonFourResponse[k].gameObject.SetActive(value: true);
				responseButtonFourResponse[k].responseButton.hasBeenPressed = false;
				responseFourButtons.SetActive(value: true);
			}
		}
		if (targetQuestion.questionType == SurveyQuestion.SurveyQuestionType.FiveResponse)
		{
			for (int l = 0; l < responseButtonFiveResponse.Length; l++)
			{
				responseButtonFiveResponse[l].SetResponseText(targetQuestion.questionResponses[l]);
				responseButtonFiveResponse[l].gameObject.SetActive(value: true);
				responseButtonFiveResponse[l].responseButton.hasBeenPressed = false;
				responseFiveButtons.SetActive(value: true);
			}
		}
	}

	private void StartButtonPress()
	{
		_currentSurveyState = SurveyStates.SurveyQuestions;
		startButton.onClickEvent.RemoveListener(StartButtonPress);
	}

	private void SurveyButtonPress()
	{
		advanceQuestion = true;
	}

	private void AddSurveyListeners()
	{
		responseButton.responseButton.onClickEvent.AddListener(SurveyButtonPress);
		for (int i = 0; i < responseButtonTwoResponse.Length; i++)
		{
			responseButtonTwoResponse[i].responseButton.onClickEvent.AddListener(SurveyButtonPress);
		}
		for (int j = 0; j < responseButtonThreeResponse.Length; j++)
		{
			responseButtonThreeResponse[j].responseButton.onClickEvent.AddListener(SurveyButtonPress);
		}
		for (int k = 0; k < responseButtonFourResponse.Length; k++)
		{
			responseButtonFourResponse[k].responseButton.onClickEvent.AddListener(SurveyButtonPress);
		}
		for (int l = 0; l < responseButtonFiveResponse.Length; l++)
		{
			responseButtonFiveResponse[l].responseButton.onClickEvent.AddListener(SurveyButtonPress);
		}
	}

	private void RemoveSurveyListeners()
	{
		responseButton.responseButton.onClickEvent.RemoveAllListeners();
		for (int i = 0; i < responseButtonTwoResponse.Length; i++)
		{
			responseButtonTwoResponse[i].responseButton.onClickEvent.RemoveAllListeners();
		}
		for (int j = 0; j < responseButtonThreeResponse.Length; j++)
		{
			responseButtonThreeResponse[j].responseButton.onClickEvent.RemoveAllListeners();
		}
		for (int k = 0; k < responseButtonFourResponse.Length; k++)
		{
			responseButtonFourResponse[k].responseButton.onClickEvent.RemoveAllListeners();
		}
		for (int l = 0; l < responseButtonFiveResponse.Length; l++)
		{
			responseButtonFiveResponse[l].responseButton.onClickEvent.RemoveAllListeners();
		}
	}

	private void TurnOffAllButtons()
	{
		responseButton.gameObject.SetActive(value: false);
		for (int i = 0; i < responseButtonTwoResponse.Length; i++)
		{
			responseButtonTwoResponse[i].gameObject.SetActive(value: false);
			responseTwoButtons.SetActive(value: false);
		}
		for (int j = 0; j < responseButtonThreeResponse.Length; j++)
		{
			responseButtonThreeResponse[j].gameObject.SetActive(value: false);
			responseThreeButtons.SetActive(value: false);
		}
		for (int k = 0; k < responseButtonFourResponse.Length; k++)
		{
			responseButtonFourResponse[k].gameObject.SetActive(value: false);
			responseFourButtons.SetActive(value: false);
		}
		for (int l = 0; l < responseButtonFiveResponse.Length; l++)
		{
			responseButtonFiveResponse[l].gameObject.SetActive(value: false);
			responseFiveButtons.SetActive(value: false);
		}
	}

	public string SetQuestionText(string newResponse)
	{
		string[] array = newResponse.Split(" "[0]);
		string text = string.Empty;
		string text2 = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			string text3 = text2 + " " + array[i];
			if (text3.Length > questionLineLength)
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
}
