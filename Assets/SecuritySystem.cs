using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecuritySystem : MonoBehaviour
{
	bool alarmByPresentAction = false;

	UI ui;

	ISet<LargeDoor> doors = new HashSet<LargeDoor>();

	public bool AlarmByPastAction { get; internal set; }

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

	internal void DetectedPresentPlayer() {
		if (!alarmByPresentAction && !AlarmByPastAction) {
			ui.ShowAlarm();
		}

		if (!alarmByPresentAction) {
			alarmByPresentAction = true;
			Lockdown();
		}
	}

	void ResetDoors() {
		foreach (var door in doors) {
			door.OpenByPastAction();
		}
	}

	void Lockdown() {
		foreach (var door in doors) {
			door.CloseByPresentAction();
		}
	}
}
