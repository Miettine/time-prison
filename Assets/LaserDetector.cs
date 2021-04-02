using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDetector : MonoBehaviour
{
	bool alarmByPresentAction = false;

	[SerializeField]
	LargeDoor[] doors;

	UI ui;

	private void Awake() {
		ui = FindObjectOfType<UI>();
	}

	private void Start() {
		foreach (var door in doors) {
			door.OpenByPresentAction();
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (!alarmByPresentAction && other.gameObject.layer == LayerMask.NameToLayer("Player")) {
			alarmByPresentAction = true;
			ui.ShowAlarm();
			foreach (var door in doors) {
				door.CloseByPresentAction();
			}
		}
	}
}
