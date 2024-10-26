using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : MonoBehaviour
{

	TimeTravel timeTravel;

	private void Awake() {
		timeTravel = FindObjectOfType<TimeTravel>();
	}

	public bool OccupiedByPresentPlayer { get; set; } = false;

	public bool OccupiedByPastPlayer { get; set; } = false;

	private void Update() {
		Debug.Log($"Past {OccupiedByPastPlayer}, Present {OccupiedByPresentPlayer}");
		if (OccupiedByPastPlayer && OccupiedByPresentPlayer) {
			timeTravel.TimeParadox(TimeParadoxCause.PastPlayerFoundPresentPlayerHiding);
		}
	}
}
