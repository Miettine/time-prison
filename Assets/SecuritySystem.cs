using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecuritySystem : MonoBehaviour
{
	bool alarmByPresentAction = false;

	UI ui;

	ISet<LargeDoor> doors = new HashSet<LargeDoor>();

	private void Awake()
	{
		foreach( Transform child in transform ) {
			doors.Add(child.GetComponent<LargeDoor>());
		}
		if (doors.Count == 0) {
			throw new Exception("Security system has no doors set");
		}
		ui = FindObjectOfType<UI>();
	}

	void Start() {
		ResetDoors();
	}

	internal void OnTimeTravelStarted() {
		alarmByPresentAction = false;
		ResetDoors();
	}

	internal void DetectedPlayer() {
		Lockdown();
	}

	void ResetDoors() {
		foreach (var door in doors) {
			door.OpenByPastAction();
		}
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
