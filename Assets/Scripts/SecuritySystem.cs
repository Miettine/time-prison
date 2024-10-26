using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecuritySystem : MonoBehaviour, IEffectedByTimeTravel
{
	public bool AlarmByPresentAction { get; private set; }

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
		ui = FindFirstObjectByType<UI>(FindObjectsInactive.Include);
	}

	void Start() {
		ResetDoors();
	}

	public void OnTimeTravelStarted() {
		AlarmByPresentAction = false;
		ResetDoors();
	}

	internal void DetectedPresentPlayer() {
		if (!AlarmByPresentAction && !AlarmByPastAction) {
			ui.ShowAlarm();
		}

		if (!AlarmByPresentAction) {
			AlarmByPresentAction = true;
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
