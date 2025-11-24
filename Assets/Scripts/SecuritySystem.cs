using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecuritySystem : Singleton<SecuritySystem>, IEffectedByTimeTravel
{
	public bool AlarmByPresentAction { get; private set; }

	UI ui;

	ISet<LargeDoor> doors = new HashSet<LargeDoor>();

	public bool AlarmByPastAction { get; internal set; }

	private void Awake()
	{
		// GetComponentsInChildren returns an array; add each element to the set
		var foundDoors = transform.GetComponentsInChildren<LargeDoor>();
		if (foundDoors == null || foundDoors.Length ==0) {
			throw new Exception("Security system has no doors set");
		}
		foreach (var d in foundDoors) {
			doors.Add(d);
		}
		ui = UI.GetInstance();
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
