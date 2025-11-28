using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class exists to show the player tutorial texts. Keeps track of what tutorials the player has seen.
/// </summary>
public class Tutorial : Singleton<Tutorial> {

	bool timeParadoxHasHappenedAtLeastOnceThroughSeeing = false;
	bool timeParadoxWarningThroughSeeingHasBeenShown = false;

	bool timeParadoxHasHappenedAtLeastOnceThroughHearing = false;
	bool timeParadoxWarningThroughHearingHasBeenShown = false;

	bool fullyInitialized = false;

	UI ui;
	TimeTravel timeTravel;

	void Awake() {
		// Ensure only a single Tutorial instance exists in the scene.
		// If there are multiple, destroy the uninitialized ones. This way only the oldest and original one remains.
		var tutorials = FindObjectsByType<Tutorial>(FindObjectsInactive.Include, FindObjectsSortMode.None);
		if (tutorials.Length > 1) {
			foreach (var t in tutorials) {
				if (t.fullyInitialized != true) {
					var gameObject = t.gameObject;
					DestroyImmediate(t);
					Destroy(gameObject);
				}
			}
		}
	}

	// Start is called before the first frame update
	void Start() {
		DontDestroyOnLoad(this);
		fullyInitialized = true;
	}

	// Update is called once per frame
	void Update() {

	}

	public void OnTimeParadox() {
		TimeParadoxCause cause = timeTravel.CauseOfTimeParadox;

		switch (cause) {
			case TimeParadoxCause.PastPlayerSawPresentPlayer:
				timeParadoxHasHappenedAtLeastOnceThroughSeeing = true;
				break;
			case TimeParadoxCause.PastPlayerHeardPresentPlayer:
				timeParadoxHasHappenedAtLeastOnceThroughHearing = true;
				break;
		}
	}

	private bool IsMobilePlatform()
	{
		return Application.platform == RuntimePlatform.WebGLPlayer && Application.isMobilePlatform;
	}

	public void OnLevelLoaded() {
		ui = UI.GetInstance();
		timeTravel = TimeTravel.GetInstance();

		if (IsCurrentLevelFirstLevel())
		{
			var controlsTutorialText = IsMobilePlatform() ? GetPhoneTutorialText() : GetComputerTutorialText();

			ui.ShowPermanentCenterNotificationText(controlsTutorialText);
		}

		if (timeParadoxHasHappenedAtLeastOnceThroughSeeing && !timeParadoxWarningThroughSeeingHasBeenShown) {
			ui.ShowTemporaryCenterNotificationText(GetFirstTimeParadoxWarningText(), NotificationType.Important);
			timeParadoxWarningThroughSeeingHasBeenShown = true;
		} else if (timeParadoxHasHappenedAtLeastOnceThroughHearing && !timeParadoxWarningThroughHearingHasBeenShown) {
			ui.ShowTemporaryCenterNotificationText(GetFirstTimeParadoxWarningText(), NotificationType.Important);
			timeParadoxWarningThroughSeeingHasBeenShown = true;
		}
	}

	private static bool IsCurrentLevelFirstLevel() => CurrentLevelIsNumber(1);

	private static bool CurrentLevelIsNumber(int levelNumber)
	{
		return SceneManager.GetActiveScene().name.Contains(levelNumber.ToString());
	}

	public static bool LevelStartsWithTimeMachine() {
		return GetCurrentLevelNumber() >= 6;
	 }

	public static bool PlayerObtainsTimeMachineOnCurrentLevel()
	{
		return GetCurrentLevelNumber() == 5;
	}

	private static int GetCurrentLevelNumber(){
		var sceneName = SceneManager.GetActiveScene().name;
		var digits = System.Text.RegularExpressions.Regex.Match(sceneName, @"\d+").Value;
		if (int.TryParse(digits, out int levelNumber))
		{
			return levelNumber;
		}
		return -1; // Return -1 if no valid level number is found
	}

	private static string GetComputerTutorialText() => "Move: use the arrow keys\nor hold the left mouse button";
	
	private static string GetPhoneTutorialText() => "Move: touch and hold the screen";
	

	string GetGoalTutorialText() {
		return "Proceed to the next chamber";
	}

	string GetTimePortalTutorialText() {
		return "Enter that portal to time travel";
	}

	string GetFirstTimeTravelTutorialText(float secondsToPast) {
		int secs = Mathf.FloorToInt(secondsToPast);
		return $"You have travelled {secs} seconds to the past";
	}
	string GetFirstTimeParadoxWarningText() {
		return "WARNING: Do not be seen or heard by your past self";
	}

	public static string GetTimeMachineObtainedTutorialText() {
		return "Obtained time machine\nPress space bar key to time travel";
	}

	public static string GetTimeMachineTouchControlTutorialText() {
		return "Obtained time machine\nPress the Time Travel button";
	}

	internal void OnPlayerEnteredLevel1GoalTutorialTrigger()
	{
		ui.ShowPermanentCenterNotificationText(GetGoalTutorialText());
	}

	public void OnTimePortalEntered()
	{
		ShowTimeTravelCenterTutorialText();
	}

	public void OnTimeMachineActivated()
	{
		if(PlayerObtainsTimeMachineOnCurrentLevel()){
			OnTimeMachineActivatedForTheFirstTime();
		}
	}
	public void OnTimeMachineActivatedForTheFirstTime(){
		ShowTimeTravelCenterTutorialText();
	}

	void ShowTimeTravelCenterTutorialText(){
		ui.ShowTemporaryCenterNotificationText(GetFirstTimeTravelTutorialText(timeTravel.GetTime()), NotificationType.Neutral);	
	}
}
