using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : Singleton<UI>
{
	Player player;
	CameraControl cameraControl;
	TimeTravel timeTravel;

	Text timeText;
	Text doorOpenText;

	Button timeTravelButton;
	Button resetButton;

	GameObject timeTravelHelpText;
	GameObject resetHelpText;

	GameObject blueKeyCardIndicator;
	GameObject greenKeyCardIndicator;
	GameObject yellowKeyCardIndicator;

	GameObject timeParadoxTextGameObject;

	TextMeshProUGUI centerNotificationText;

	[SerializeField] private float timeParadoxAnimationLength = 10f;

	void Awake() {
		cameraControl = CameraControl.GetInstance();
		timeTravel = TimeTravel.GetInstance();

		timeText = GameObject.Find("TimeText").GetComponent<Text>();
		doorOpenText = GameObject.Find("DoorOpenText").GetComponent<Text>();

		blueKeyCardIndicator = GameObject.Find("BlueKeyCardIndicator");
		greenKeyCardIndicator = GameObject.Find("GreenKeyCardIndicator");
		yellowKeyCardIndicator = GameObject.Find("YellowKeyCardIndicator");

		timeTravelButton = GameObject.Find("TimeTravelButton").GetComponent<Button>();
		resetButton = GameObject.Find("ResetButton").GetComponent<Button>();

		timeTravelHelpText = GameObject.Find("TimeTravelHelpText");
		resetHelpText = GameObject.Find("ResetHelpText");

		timeParadoxTextGameObject = GameObject.Find("TimeParadoxTextGameObject");

		centerNotificationText = GameObject.Find("CenterNotificationTextGameObject").GetComponent<TextMeshProUGUI>();
		centerNotificationText.gameObject.SetActive(false);

		player = Player.GetInstance();

		timeTravelButton.onClick.AddListener(() => player.OnTimeTravelActivated());
		resetButton.onClick.AddListener(() => player.OnResetActivated());

		blueKeyCardIndicator.SetActive(false);
		greenKeyCardIndicator.SetActive(false);
		yellowKeyCardIndicator.SetActive(false);

		doorOpenText.text = "";
	}

	void Start() {
		timeParadoxTextGameObject.SetActive(false);
		SetControlMode(ControlMode.Touch);
	}

	private void SetControlMode(ControlMode controlMode) {
		bool touchControlsEnabled = controlMode == ControlMode.Touch;

		timeTravelButton.gameObject.SetActive(touchControlsEnabled);
		resetButton.gameObject.SetActive(touchControlsEnabled);

		timeTravelHelpText.SetActive(!touchControlsEnabled);
		resetHelpText.SetActive(!touchControlsEnabled);
	}

	public enum ControlMode {
		Touch,
		Keyboard
	}

	public void SetTimeText(float time) {
		timeText.text = time.ToString("Time: 0");
	}

	internal void ShowDoorOpenPermanentNotification(string notificationText) {
		doorOpenText.text = notificationText;
	}

	internal void ShowDoorOpenPermanentNotification() {
		ShowDoorOpenPermanentNotification("Door open");
	}

	internal void ShowDoorClosed() {
		StartCoroutine(ShowNotification("Door closed"));
	}

	internal void ShowDoorOpenForSeconds(float openForTime) {
		StopAllCoroutines();
		doorOpenText.text = "";

		ShowDoorOpenText(openForTime);

		StartCoroutine(CountDownShowDoorOpen(openForTime));
	}

	internal void OnKeyboardInputUsed() {
		SetControlMode(ControlMode.Keyboard);
	}

	internal void OnMouseInputUsed() {
		SetControlMode(ControlMode.Touch);
	}

	void ShowDoorOpenText(float seconds) {
		doorOpenText.text = Math.Ceiling(seconds).ToString($"Door open for 0 seconds");
	}

	IEnumerator CountDownShowDoorOpen(float wait) {
		while (wait > 0) {
			wait -= Time.deltaTime;
			ShowDoorOpenText(wait);
			yield return null;
		}
		ShowDoorClosed();
	}

	internal void ShowAlarm() {
		StartCoroutine(ShowNotification("Alarm triggered. Lockdown in effect."));
	}

	IEnumerator ShowNotification(string text) {
		doorOpenText.text = text;
		yield return new WaitForSeconds(1.5f);
		doorOpenText.text = "";
	}

	internal void ShowTemporaryCenterNotificationText(string text) {
		ShowCenterNotificationText(text);
	}

	internal IEnumerator ShowCenterNotificationText(string text) {
		centerNotificationText.gameObject.SetActive(true);
		centerNotificationText.text = text;

		yield return WaitForSeconds(3f);

		centerNotificationText.gameObject.SetActive(false);
		centerNotificationText.text = "";
	}

	internal void ShowKeyCardIndicator(KeyCardType type, bool visible) {
		switch (type) {
			case KeyCardType.Blue:
				blueKeyCardIndicator.SetActive(visible);
				break;
			case KeyCardType.Green:
				greenKeyCardIndicator.SetActive(visible);
				break;
			case KeyCardType.Yellow:
				yellowKeyCardIndicator.SetActive(visible);
				break;
		}
	}

	/// <summary>
	/// In order for the player to have time to read all of the messages, it it requires 
	/// a waiting time of timeParadoxAnimationStepDelay*4 for the last effect to finish playing.
	/// after which the level may be reset.
	/// </summary>
	/// <param name="timeParadoxCause"></param>
	internal void OnTimeParadoxAnimationStart() {
		StartCoroutine(TimeParadoxAnimation());
	}

	/// <summary>
	/// Currently the time paradox animation has about 5 pauses 
	/// where we display some text to the player, move the camera around
	/// or show some graphical effects to the player.
	/// If the amount of pauses ever changes, remember to update this function
	/// accordingly so that the player will have enough time to see everything
	/// we want them to see during the time paradox animation.
	/// </summary>
	/// <returns></returns>
	public float GetTimeParadoxAnimationStepDelay() {
		return GetTimeParadoxAnimationLength() / 5;
	}

	public float GetTimeParadoxAnimationLength() {
		return timeParadoxAnimationLength;
	}

	IEnumerator TimeParadoxAnimation() {

		timeParadoxTextGameObject.SetActive(true);

		yield return WaitForTimeParadoxAnimationStepDelay();

		timeParadoxTextGameObject.SetActive(false);

		switch (timeTravel.CauseOfTimeParadox) {
			case TimeParadoxCause.PastPlayerSawPresentPlayer:
				centerNotificationText.gameObject.SetActive(true);
				centerNotificationText.text = "Cause:\npast self saw you";
				break;
			case TimeParadoxCause.PastPlayerHeardPresentPlayer:
				centerNotificationText.gameObject.SetActive(true);
				centerNotificationText.text = "Cause:\npast self heard you";
				break;
			case TimeParadoxCause.PastPlayerSawObjectInteractionFromPresentPlayer:
				centerNotificationText.gameObject.SetActive(true);
				centerNotificationText.text = "Cause:\npast self saw an object that you had interacted with";
				break;
		}

		yield return cameraControl.MoveToTarget(timeTravel.EntityThatCausedTimeParadox, GetTimeParadoxAnimationStepDelay());

		yield return WaitForTimeParadoxAnimationStepDelay();

		centerNotificationText.gameObject.SetActive(true);
		centerNotificationText.text = "Resetting timeline";

		yield return cameraControl.MoveToTarget(player.transform, GetTimeParadoxAnimationStepDelay());
		
		cameraControl.StartTimelineResetAnimation(); //lasts one TimeParadoxAnimationStepDelay

		yield return WaitForSeconds(GetTimeParadoxAnimationStepDelay() / 6);
		//The camera warping effect and the text "Resetting timeline" are intentionally made to overlap for a moment.
		//This is to make the time paradox animation seem more visually interesting and organic.
		centerNotificationText.gameObject.SetActive(false);
	}

	WaitForSeconds WaitForTimeParadoxAnimationStepDelay() {
		return WaitForSeconds(GetTimeParadoxAnimationStepDelay());
	}

	WaitForSeconds WaitForSeconds(float time) {
		return new WaitForSeconds(time);
	}
}
