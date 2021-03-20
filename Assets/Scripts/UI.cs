using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
	Text timeText;
	Text doorOpenText;

	GameObject blueKeyCardIndicator;
	GameObject greenKeyCardIndicator;
	GameObject yellowKeyCardIndicator;

	void Awake() {
		timeText = GameObject.Find("TimeText").GetComponent<Text>();
		doorOpenText = GameObject.Find("DoorOpenText").GetComponent<Text>();

		blueKeyCardIndicator = GameObject.Find("BlueKeyCardIndicator");
		greenKeyCardIndicator = GameObject.Find("GreenKeyCardIndicator");
		yellowKeyCardIndicator = GameObject.Find("YellowKeyCardIndicator");

		blueKeyCardIndicator.SetActive(false);
		greenKeyCardIndicator.SetActive(false);
		yellowKeyCardIndicator.SetActive(false);

		doorOpenText.text = "";
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
		StartCoroutine(ShowDoorClosedNotification());
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

	IEnumerator ShowDoorClosedNotification() {
		doorOpenText.text = "Door closed";
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
