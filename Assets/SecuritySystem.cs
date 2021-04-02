using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecuritySystem : MonoBehaviour
{
	bool alarmByPresentAction = false;

	UI ui;

	[SerializeField]
	LargeDoor[] doors;

	private void Awake()
	{
		if (doors.Length == 0) {
			throw new Exception("Security system has no doors set");
		}
		ui = FindObjectOfType<UI>();
	}

	void Start() {
		foreach (var door in doors) {
			door.OpenByPastAction();
		}
	}

	internal void DetectedPlayer() {
		Lockdown();
	}

	void Lockdown() {
		if (!alarmByPresentAction) {
			alarmByPresentAction = true;
			ui.ShowAlarm();
			foreach (var door in doors) {
				door.CloseByPresentAction();
			}
		}
	}
}
