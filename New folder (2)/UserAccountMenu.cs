using System;
using System.Collections;
using System.Collections.Generic;
using Edelweiss.Coroutine;
using TMPro;
using UnityEngine;

public class UserAccountMenu : MonoBehaviour
{
	private delegate void MenuTransition(float t, bool reverse);

	[Serializable]
	private struct ToolGroupToCursor
	{
		public ToolGroup ToolGroup;

		public UserAccountMenuCursorManager Cursor;
	}

	[Serializable]
	public class TransitionElementCurves
	{
		[Range(0f, 1f)]
		public float accordionOverlap = 0.5f;

		public AnimationCurve accordionFadePush = new AnimationCurve(new Keyframe(0f, 0f, 1f, 1f), new Keyframe(1f, 1f, 1f, 1f));

		public AnimationCurve accountIconScaling = new AnimationCurve(new Keyframe(0f, 0f, 1f, 1f), new Keyframe(1f, 1f, 1f, 1f));

		public AnimationCurve accountIconTranslation = new AnimationCurve(new Keyframe(0f, 0f, 1f, 1f), new Keyframe(1f, 1f, 1f, 1f));

		public AnimationCurve detailButtonFading = new AnimationCurve(new Keyframe(0f, 0f, 1f, 1f), new Keyframe(1f, 1f, 1f, 1f));

		public AnimationCurve loadingBarFadePush = new AnimationCurve(new Keyframe(0f, 0f, 1f, 1f), new Keyframe(1f, 1f, 1f, 1f));

		public AnimationCurve deleteIconMorph = new AnimationCurve(new Keyframe(0f, 0f, 1f, 1f), new Keyframe(1f, 1f, 1f, 1f));
	}

	[Serializable]
	public class CreateAccountUITimeline
	{
		public float duration = 0.5f;

		public NormalizedRange loadingBarIn;

		public NormalizedRange accountIconsOut;

		public NormalizedRange subtitleBarOut;
	}

	[Serializable]
	public class SelectAccountUITimeline
	{
		public float duration = 0.5f;

		public NormalizedRange detailAccordionIn;

		public NormalizedRange detailAccordionReverse;

		public NormalizedRange detailButtonsIn;

		public NormalizedRange selectedIconTranslate;

		public NormalizedRange unselectedIconsOut;
	}

	[Serializable]
	public class LoginUITimeline
	{
		public float duration = 0.5f;

		public NormalizedRange loadingBarIn;

		public NormalizedRange detailAccordionOut;

		public NormalizedRange detailButtonsOut;

		public NormalizedRange selectedIconOut;

		public NormalizedRange subtitleBarOut;
	}

	[Serializable]
	public class ConfirmDeleteUITimeline
	{
		public float duration = 0.5f;

		public NormalizedRange confirmationAccordionIn;

		public NormalizedRange detailAccordionOut;

		public NormalizedRange detailButtonsOut;
	}

	[Serializable]
	public class ExecuteDeleteUITimeline
	{
		public float duration = 0.5f;

		public NormalizedRange selectedIconMorph;

		public NormalizedRange selectedIconTranslate;

		public NormalizedRange accountIconsIn;

		public NormalizedRange detailAccordionOut;
	}

	[Header("General references:")]
	[SerializeField]
	private ToolGroupToCursor[] _cursors;

	[SerializeField]
	private GameObject defaultGeo;

	[SerializeField]
	private GameObject hacktivitudeGeo;

	[SerializeField]
	private LoginBgmRadio bgmRadio;

	[SerializeField]
	private LoginDayNightColors dayNightColors;

	[SerializeField]
	private LoginDayNightAnimation dayNightAnim;

	[Header("General Constants:")]
	[SerializeField]
	private float grabDistance = 10f;

	[SerializeField]
	private Vector3 userButtonCenter = Vector3.zero;

	[SerializeField]
	private float pulledInZ = -2f;

	[SerializeField]
	private float menuPlaneZ;

	[SerializeField]
	private float pushedOutZ = 2f;

	[Header("Account Selection UI:")]
	[SerializeField]
	private List<UserAccountButton> accountButtons;

	[SerializeField]
	private Transform displayButtonAnchor;

	[SerializeField]
	private float spacing = 0.5f;

	[Header("Selected User UI:")]
	[SerializeField]
	private ObjFadePushTransitionElement[] selectedScreenButtonFadePushers;

	[SerializeField]
	private ObjectButton backButton;

	[SerializeField]
	private ObjectButton loginAccountButton;

	[SerializeField]
	private ObjectButton deleteAccountButton;

	[SerializeField]
	private AccordionTransitionGroup userDetailAccordion;

	[SerializeField]
	private TextMeshPro lastActiveTimeText;

	[SerializeField]
	private TextSwapper lastClientText;

	[SerializeField]
	private TextMeshPro humanUsernameText;

	[SerializeField]
	private TextSwapper adminAccessText;

	[Header("Delete Account UI:")]
	[SerializeField]
	private AccordionTransitionGroup deleteAccountAccordion;

	[SerializeField]
	private ObjectButton cancelDeleteButton;

	[SerializeField]
	private ObjectButton confirmDeleteButton;

	[SerializeField]
	private AudioClip deleteAccountSfx;

	[Header("Login UI")]
	[SerializeField]
	private ObjFadePushTransitionElement loadingBarFadePusher;

	[Header("Transition Parameters")]
	public TransitionElementCurves tweenCurves;

	public SelectAccountUITimeline selectAccountUITiming;

	public CreateAccountUITimeline createAccountUITiming;

	public LoginUITimeline loginUITiming;

	public ConfirmDeleteUITimeline confirmDeleteUITiming;

	public ExecuteDeleteUITimeline executeDeleteUITiming;

	private int lastSelectedButton;

	private SafeCoroutine mTransitionThread;

	private Action startGameFunction;

	private VVR_SAVEDATA[] userSaves;

	public void SetUpUserAccountMenu(Action startGameFunctionIn)
	{
		startGameFunction = startGameFunctionIn;
		refreshKnownSaveData();
		if (PersistentDataManager.GetUseHacktivitudeMenu())
		{
			hacktivitudeGeo.SetActive(value: true);
			defaultGeo.SetActive(value: false);
			bgmRadio.ActivateAdminMode(adminModeIn: true);
			dayNightAnim.Disable();
			dayNightColors.ActivateHacktivitudeOverride();
		}
		else
		{
			hacktivitudeGeo.SetActive(value: false);
			defaultGeo.SetActive(value: true);
			bgmRadio.ActivateAdminMode(adminModeIn: false);
		}
		initializeAccountButtons();
		backButton.onClickEvent.AddListener(unselectExistingClick);
		confirmDeleteButton.onClickEvent.AddListener(executeDeletionClick);
		loginAccountButton.onClickEvent.AddListener(() =>
		{
			startGameFunction();
			enterTransition(loginTransition, loginUITiming.duration, reverse: false);
		});
		deleteAccountButton.onClickEvent.AddListener(() =>
		{
			enterTransition(showDeleteConfirmationTransition, confirmDeleteUITiming.duration, reverse: false);
		});
		cancelDeleteButton.onClickEvent.AddListener(() =>
		{
			enterTransition(showDeleteConfirmationTransition, confirmDeleteUITiming.duration, reverse: true);
		});
		userDetailAccordion.Hide();
		deleteAccountAccordion.Hide();
		loadingBarFadePusher.Hide();
		for (int num = 0; num < selectedScreenButtonFadePushers.Length; num++)
		{
			selectedScreenButtonFadePushers[num].Hide();
		}
		for (int num2 = 0; num2 < _cursors.Length; num2++)
		{
			if (_cursors[num2].ToolGroup.IsActive)
			{
				_cursors[num2].Cursor.Initialize(grabDistance, _cursors[num2].ToolGroup);
			}
		}
	}

	public void DisableUserAccountMenu()
	{
		gameObject.SetActive(value: false);
	}

	private void refreshKnownSaveData()
	{
		userSaves = SaveLoadUtility.GetAndCleanAllAccountSaves(clean: true);
	}

	private void initializeAccountButtons(bool deferNewUser = false)
	{
		for (int i = 0; i < accountButtons.Count; i++)
		{
			if (i < userSaves.Length)
			{
				accountButtons[i].transform.position = getCenteredButtonPosition(i);
				accountButtons[i].InitializeWithSave(userSaves[i]);
				accountButtons[i].SetButtonAction(selectExistingClick);
			}
			else if (i < userSaves.Length + 1 && !deferNewUser)
			{
				accountButtons[i].transform.position = getCenteredButtonPosition(i);
				accountButtons[i].InitializeWithSave(null);
				accountButtons[i].SetButtonAction(createNewAccountClick);
			}
			else if (i >= userSaves.Length + 1)
			{
				accountButtons[i].gameObject.SetActive(value: false);
				accountButtons[i].ShowText(show: false);
			}
		}
	}

	private Vector3 getCenteredButtonPosition(int index)
	{
		int num = ((!Application.isPlaying || userSaves == null) ? 3 : Mathf.Min(userSaves.Length + 1, accountButtons.Count));
		return userButtonCenter - Vector3.right * spacing / 2f * (num - 1) + Vector3.right * index * spacing;
	}

	private void enterTransition(MenuTransition transition, float duration, bool reverse)
	{
		if (mTransitionThread == null || !mTransitionThread.IsRunning)
		{
			mTransitionThread = GlobalCoroutineContainer.AddThread(transitionSequence(transition, duration, reverse), this);
		}
		else
		{
			SafeDebug.LogError("Trying to start a menu transition while one is already playing! This is kind of a problem");
		}
	}

	private static IEnumerator transitionSequence(MenuTransition transition, float duration, bool reverse)
	{
		float time = 0f;
		transition((!reverse) ? 0f : 1f, reverse);
		for (; time < duration; time += Time.deltaTime)
		{
			float t = ((!reverse) ? (time / duration) : (1f - time / duration));
			transition(t, reverse);
			yield return null;
		}
		transition((!reverse) ? 1f : 0f, reverse);
	}

	private void selectExistingClick(UserAccountButton sender)
	{
		lastSelectedButton = accountButtons.IndexOf(sender);
		PersistentDataManager.SetActiveSaveSlot(lastSelectedButton);
		VVR_SAVEDATA vVR_SAVEDATA = userSaves[lastSelectedButton];
		sender.SetButtonAction(null);
		lastActiveTimeText.text = vVR_SAVEDATA.arbitraryStringDictionary["LASTPLAYED"];
		lastClientText.SetNewKey(getLastJobStringID(vVR_SAVEDATA));
		humanUsernameText.text = vVR_SAVEDATA.arbitraryStringDictionary["USERNAME"];
		if (vVR_SAVEDATA.hasAdminAccess)
		{
			adminAccessText.usingAltString = false;
			adminAccessText.SetNewKey("TEXT_242");
		}
		else
		{
			adminAccessText.UseAltString(string.Empty);
		}
		enterTransition(selectAccountTransition, selectAccountUITiming.duration, reverse: false);
	}

	private void createNewAccountClick(UserAccountButton sender)
	{
		PersistentDataManager.CreateNewAccount();
		startGameFunction();
		enterTransition(createAccountTransition, createAccountUITiming.duration, reverse: false);
	}

	private void unselectExistingClick()
	{
		PersistentDataManager.SetActiveSaveSlot(-1);
		enterTransition(selectAccountTransition, selectAccountUITiming.duration, reverse: true);
	}

	private void executeDeletionClick()
	{
		PersistentDataManager.DeleteActiveSaveData();
		refreshKnownSaveData();
		UserAccountButton item = accountButtons[lastSelectedButton];
		accountButtons.RemoveAt(lastSelectedButton);
		accountButtons.Insert(userSaves.Length, item);
		initializeAccountButtons(deferNewUser: true);
		accountButtons[userSaves.Length].userLabel.SetNewKey("TEXT_243");
		accountButtons[userSaves.Length].userLabel.UseAltString(alt: false);
		lastSelectedButton = userSaves.Length;
		accountButtons[lastSelectedButton].SetButtonAction(createNewAccountClick);
		EagerMaster.instance.flatSource.PlayOneShot(deleteAccountSfx);
		enterTransition(executeDeletionTransition, executeDeleteUITiming.duration, reverse: false);
	}

	private void selectAccountTransition(float t, bool reverse)
	{
		float t2 = rangedCurveInterpolate(t, reverse, reverse ? selectAccountUITiming.detailAccordionReverse : selectAccountUITiming.detailAccordionIn, tweenCurves.accordionFadePush);
		float t3 = rangedCurveInterpolate(t, reverse, selectAccountUITiming.detailButtonsIn, tweenCurves.detailButtonFading);
		float t4 = rangedCurveInterpolate(t, reverse, selectAccountUITiming.selectedIconTranslate, tweenCurves.accountIconTranslation);
		float num = rangedCurveInterpolate(t, reverse, selectAccountUITiming.unselectedIconsOut, tweenCurves.accountIconScaling);
		userDetailAccordion.ApplyTransitionT(t2, tweenCurves.accordionOverlap, pushedOutZ, menuPlaneZ);
		accountButtons[lastSelectedButton].transform.position = Vector3.Lerp(getCenteredButtonPosition(lastSelectedButton), displayButtonAnchor.transform.position, t4);
		accountButtons[lastSelectedButton].SelectTransition(t4, !reverse);
		accountButtons[lastSelectedButton].SetButtonAction((!t.AlmostEquals(0f)) ? null : new UserAccountButton.UserButtonClickAction(selectExistingClick));
		for (int i = 0; i < selectedScreenButtonFadePushers.Length; i++)
		{
			selectedScreenButtonFadePushers[i].ApplyTransitionT(t3);
		}
		for (int j = 0; j < accountButtons.Count; j++)
		{
			accountButtons[j].ShowText(t.AlmostEquals(0f));
			if (j != lastSelectedButton)
			{
				accountButtons[j].ScaleInTransition(1f - num);
			}
		}
	}

	private void createAccountTransition(float t, bool reverse)
	{
		float t2 = rangedCurveInterpolate(t, reverse, createAccountUITiming.loadingBarIn, tweenCurves.loadingBarFadePush);
		float num = rangedCurveInterpolate(t, reverse, createAccountUITiming.accountIconsOut, tweenCurves.accountIconScaling);
		loadingBarFadePusher.ApplyTransitionT(t2, pulledInZ, menuPlaneZ);
		bgmRadio.SetVolumeT(1f - t);
		for (int i = 0; i < accountButtons.Count; i++)
		{
			accountButtons[i].ShowText(show: false);
			accountButtons[i].ScaleInTransition(1f - num);
		}
	}

	private void loginTransition(float t, bool reverse)
	{
		float t2 = rangedCurveInterpolate(t, reverse, loginUITiming.loadingBarIn, tweenCurves.loadingBarFadePush);
		float num = rangedCurveInterpolate(t, reverse, loginUITiming.detailAccordionOut, tweenCurves.accordionFadePush);
		float num2 = rangedCurveInterpolate(t, reverse, loginUITiming.detailButtonsOut, tweenCurves.detailButtonFading);
		float num3 = rangedCurveInterpolate(t, reverse, loginUITiming.selectedIconOut, tweenCurves.accountIconScaling);
		loadingBarFadePusher.ApplyTransitionT(t2, pulledInZ, menuPlaneZ);
		bgmRadio.SetVolumeT(1f - t);
		userDetailAccordion.ApplyTransitionT(1f - num, tweenCurves.accordionOverlap, pushedOutZ, menuPlaneZ);
		accountButtons[lastSelectedButton].ScaleInTransition(1f - num3);
		for (int i = 0; i < selectedScreenButtonFadePushers.Length; i++)
		{
			selectedScreenButtonFadePushers[i].ApplyTransitionT(1f - num2);
		}
	}

	private void showDeleteConfirmationTransition(float t, bool reverse)
	{
		float t2 = rangedCurveInterpolate(t, reverse, confirmDeleteUITiming.confirmationAccordionIn, tweenCurves.accordionFadePush);
		float num = rangedCurveInterpolate(t, reverse, confirmDeleteUITiming.detailAccordionOut, tweenCurves.accordionFadePush);
		float num2 = rangedCurveInterpolate(t, reverse, confirmDeleteUITiming.detailButtonsOut, tweenCurves.detailButtonFading);
		deleteAccountAccordion.ApplyTransitionT(t2, tweenCurves.accordionOverlap, pulledInZ, menuPlaneZ);
		userDetailAccordion.ApplyTransitionT(1f - num, tweenCurves.accordionOverlap, pushedOutZ, menuPlaneZ);
		for (int i = 0; i < selectedScreenButtonFadePushers.Length; i++)
		{
			selectedScreenButtonFadePushers[i].ApplyTransitionT(1f - num2);
		}
		if (reverse && num2 == 0f)
		{
			deleteAccountAccordion.Hide();
		}
	}

	private void executeDeletionTransition(float t, bool reverse)
	{
		float t2 = rangedCurveInterpolate(t, reverse, executeDeleteUITiming.selectedIconMorph, tweenCurves.deleteIconMorph);
		float t3 = rangedCurveInterpolate(t, reverse, executeDeleteUITiming.selectedIconTranslate, tweenCurves.accountIconTranslation);
		float t4 = rangedCurveInterpolate(t, reverse, executeDeleteUITiming.accountIconsIn, tweenCurves.accountIconScaling);
		float num = rangedCurveInterpolate(t, reverse, executeDeleteUITiming.detailAccordionOut, tweenCurves.accordionFadePush);
		for (int i = 0; i < accountButtons.Count; i++)
		{
			if (i == lastSelectedButton)
			{
				accountButtons[i].TransformIntoNewUserButtonTransition(t2);
				accountButtons[i].transform.position = Vector3.Lerp(displayButtonAnchor.position, getCenteredButtonPosition(lastSelectedButton), t3);
			}
			else
			{
				accountButtons[i].ScaleInTransition(t4);
			}
			accountButtons[i].ShowText(t.AlmostEquals(1f));
		}
		deleteAccountAccordion.ApplyTransitionT(1f - num, tweenCurves.accordionOverlap, pushedOutZ, menuPlaneZ);
	}

	private static float rangedCurveInterpolate(float t, bool reverse, NormalizedRange range, AnimationCurve curve)
	{
		float num = Mathf.InverseLerp(range.start, range.end, t);
		return (!reverse) ? curve.Evaluate(num) : (1f - curve.Evaluate(1f - num));
	}

	private static string getLastJobStringID(VVR_SAVEDATA save)
	{
		if (save.tutorialCompletion < TutorialLevelSequence.TutorialStates.Finished)
		{
			return "TEXT_254";
		}
		if (save.hubState < VVR_HUB.HubStates.HubGiveJobs)
		{
			return "TEXT_254";
		}
		if (save.jobState == VVR_HUB.JobStates.Macy)
		{
			return "TEXT_255";
		}
		if (save.jobState == VVR_HUB.JobStates.Garden)
		{
			return "TEXT_337";
		}
		if (save.jobState == VVR_HUB.JobStates.Sunset)
		{
			return "TEXT_256";
		}
		if (save.jobState == VVR_HUB.JobStates.Tumbleweed1 || save.jobState == VVR_HUB.JobStates.Union2Vacuum)
		{
			return "TEXT_257";
		}
		if (save.jobState == VVR_HUB.JobStates.ButterSnack)
		{
			return "TEXT_258";
		}
		if (save.jobState == VVR_HUB.JobStates.Tina)
		{
			return "TEXT_259";
		}
		if (save.jobState == VVR_HUB.JobStates.GardenNight)
		{
			return "TEXT_336";
		}
		if (save.jobState == VVR_HUB.JobStates.Tumbleweed2)
		{
			return "TEXT_260";
		}
		if (save.jobState == VVR_HUB.JobStates.LOOP)
		{
			if (save.kitchenLoopCounter == 0)
			{
				return "TEXT_261";
			}
			return "TEXT_262";
		}
		return string.Empty;
	}
}
