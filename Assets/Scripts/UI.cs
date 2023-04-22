using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
	Player player;

	Text timeText;
	Text doorOpenText;

	Button timeTravelButton;
	Button resetButton;

	GameObject timeTravelHelpText;
	GameObject resetHelpText;

	GameObject blueKeyCardIndicator;
	GameObject greenKeyCardIndicator;
	GameObject yellowKeyCardIndicator;

	void Awake() {
		timeText = GameObject.Find("TimeText").GetComponent<Text>();
		doorOpenText = GameObject.Find("DoorOpenText").GetComponent<Text>();

		blueKeyCardIndicator = GameObject.Find("BlueKeyCardIndicator");
		greenKeyCardIndicator = GameObject.Find("GreenKeyCardIndicator");
		yellowKeyCardIndicator = GameObject.Find("YellowKeyCardIndicator");

		timeTravelButton = GameObject.Find("TimeTravelButton").GetComponent<Button>();
		resetButton = GameObject.Find("ResetButton").GetComponent<Button>();

		timeTravelHelpText = GameObject.Find("TimeTravelHelpText");
		resetHelpText = GameObject.Find("ResetHelpText");

		player = Player.GetInstance();

		timeTravelButton.onClick.AddListener(() => player.OnTimeTravelActivated());
		resetButton.onClick.AddListener(() => player.OnResetActivated());

		blueKeyCardIndicator.SetActive(false);
		greenKeyCardIndicator.SetActive(false);
		yellowKeyCardIndicator.SetActive(false);

		doorOpenText.text = "";

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
}
