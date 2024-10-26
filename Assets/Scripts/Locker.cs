using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : MonoBehaviour
{
	private Player player;
	TimeTravel timeTravel;

	private void Awake() {
		player = FindFirstObjectByType<Player>(FindObjectsInactive.Include);
		timeTravel = FindFirstObjectByType<TimeTravel>(FindObjectsInactive.Include);
	}

	public bool OccupiedByPresentPlayer { get; set; } = false;

	public bool OccupiedByPastPlayer { get; set; } = false;

	private void Update() {
		Debug.Log($"Past {OccupiedByPastPlayer}, Present {OccupiedByPresentPlayer}");
		if (OccupiedByPastPlayer && OccupiedByPresentPlayer) {
			timeTravel.TimeParadox(TimeParadoxCause.PastPlayerFoundPresentPlayerHiding, player.transform);
		}
	}
}
