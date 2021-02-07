using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
	Text timeText;
	Text doorOpenText;

	void Awake() {
		timeText = GameObject.Find("TimeText").GetComponent<Text>();
		doorOpenText = GameObject.Find("DoorOpenText").GetComponent<Text>();

		doorOpenText.text = "";
	}

	public void SetTimeText(float time) {
		timeText.text = time.ToString("Time: 0");
	}

	internal void ShowDoorOpenForSeconds(float openForTime) {
		StopAllCoroutines();
		doorOpenText.text = "";

		ShowDoorOpenText(openForTime);

		StartCoroutine(CountDownShowDoorOpen(openForTime));
	}

	void ShowDoorOpenText(float seconds) {
		doorOpenText.text = seconds.ToString("Door open for 0 seconds");
	}

	IEnumerator CountDownShowDoorOpen(float wait) {
		while (wait > 0) {
			wait -= Time.deltaTime;
			ShowDoorOpenText(wait);
			yield return null;
		}
		doorOpenText.text = "Door closed";
		yield return new WaitForSeconds(1.5f);
		doorOpenText.text = "";
	}
}
