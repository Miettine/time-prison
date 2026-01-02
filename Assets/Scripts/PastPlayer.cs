

using System;
using UnityEngine;
using static LargeDoor;

public class PastPlayer : SentientCharacter {

	// Update is called once per frame
	void Update() {
		if (TouchesPresentPlayer()){
			timeTravel.TimeParadox(TimeParadoxCause.TouchedYourPastSelf, transform);
		} else if (SeesPresentPlayer()) {
			timeTravel.TimeParadox(TimeParadoxCause.PastPlayerSawPresentPlayer, transform);
		} else if (SeesDoorPresentPlayerInteractedWith(out DoorTimeTravelState? doorState)) {
			// Coming up with design for time travel code is hard... :(
			if (doorState.HasValue) {
				if (doorState.Value.OpenInPast != doorState.Value.OpenInPresent) {
					if (doorState.Value.ClosedInPast && doorState.Value.OpenInPresent) {
						timeTravel.TimeParadox(TimeParadoxCause.PastPlayerSawDoorOpenedByPresentPlayer, TimeParadoxObject);
						return;
					} else if (doorState.Value.OpenInPast && doorState.Value.ClosedInPresent) {
						timeTravel.TimeParadox(TimeParadoxCause.PastPlayerSawDoorClosedByPresentPlayer, TimeParadoxObject);
						return;
					}
				}
			}
			Debug.LogWarning("Past player saw door interaction but could not determine state correctly.");
		}
	}
}
