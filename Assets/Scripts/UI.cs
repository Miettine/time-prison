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

	TextMeshProUGUI timeParadoxReasonText;

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

		timeParadoxReasonText = GameObject.Find("TimeParadoxReasonTextGameObject").GetComponent<TextMeshProUGUI>();
		timeParadoxReasonText.gameObject.SetActive(false);

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

	private enum ControlMode {
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

	public float GetTimeParadoxAnimationStepDelay() {
		return timeParadoxAnimationLength / 6;
	}

	IEnumerator TimeParadoxAnimation() {

		timeParadoxTextGameObject.SetActive(true);

		yield return WaitForTimeParadoxAnimationStepDelay();

		timeParadoxTextGameObject.SetActive(false);

		switch (timeTravel.CauseOfTimeParadox) {
			case TimeParadoxCause.PastPlayerSawPresentPlayer:
				timeParadoxReasonText.gameObject.SetActive(true);
				timeParadoxReasonText.text = "Cause:\npast self saw you";
				break;
			case TimeParadoxCause.PastPlayerHeardPresentPlayer:
				timeParadoxReasonText.gameObject.SetActive(true);
				timeParadoxReasonText.text = "Cause:\npast self heard you";
				break;
		}

		yield return cameraControl.MoveToTarget(timeTravel.EntityThatCausedTimeParadox, GetTimeParadoxAnimationStepDelay());

		yield return WaitForTimeParadoxAnimationStepDelay();

		timeParadoxReasonText.gameObject.SetActive(true);
		timeParadoxReasonText.text = "Resetting timeline";

		yield return cameraControl.MoveToTarget(player.transform, GetTimeParadoxAnimationStepDelay());
		
		timeParadoxReasonText.gameObject.SetActive(false);

		cameraControl.StartTimelineResetAnimation(); //lasts 2*TimeParadoxAnimationStepDelay
	}

	WaitForSeconds WaitForTimeParadoxAnimationStepDelay() {
		return new WaitForSeconds(GetTimeParadoxAnimationStepDelay());
	}
}
