using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class exists to show the player tutorial texts. Keeps track of what tutorials the player has seen.
/// </summary>
public class Tutorial : Singleton<Tutorial> {

	bool timeParadoxHasHappenedAtLeastOnceThroughSeeing = false;
	bool timeParadoxWarningThroughSeeingHasBeenShown = false;

	bool timeParadoxHasHappenedAtLeastOnceThroughHearing = false;
	bool timeParadoxWarningThroughHearingHasBeenShown = false;

	UI ui;
	TimeTravel timeTravel;

	void Awake() {

	}

	// Start is called before the first frame update
	void Start() {
		DontDestroyOnLoad(this);
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

	public void OnLevelLoaded() {
		ui = UI.GetInstance();
		timeTravel = TimeTravel.GetInstance();

		if (timeParadoxHasHappenedAtLeastOnceThroughSeeing && !timeParadoxWarningThroughSeeingHasBeenShown) {
			ui.ShowTemporaryCenterNotificationText(GetFirstTimeParadoxWarningText());
			timeParadoxWarningThroughSeeingHasBeenShown = true;
		} else if (timeParadoxHasHappenedAtLeastOnceThroughHearing && !timeParadoxWarningThroughHearingHasBeenShown) {
			ui.ShowTemporaryCenterNotificationText(GetFirstTimeParadoxWarningText());
			timeParadoxWarningThroughSeeingHasBeenShown = true;
		}
	}


	string GetGoalTutorialText() {
		return "Proceed to the next chamber";
	}

	string GetTimePortalTutorialText() {
		return "Approach that portal to time travel";
	}

	string GetTimeFirstTimeTravelTutorialText() {
		return "You have travelled XX seconds to the past";
	}
	string GetFirstTimeParadoxWarningText() {
		return "WARNING: Do not be seen or heard by your past self";
	}

	public string GetTimeTimeMachineTutorialText() {
		return "Press space bar key to time travel";
	}

	public string GetTimeTimeMachineTouchControlTutorialText() {
		return "Press the Time Travel button";
	}
	
}