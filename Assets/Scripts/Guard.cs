using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : SentientCharacter {

	void Update() {
		if (SeesPresentPlayer()) {
			timeTravel.TimeParadox(TimeParadoxCause.GuardCaughtPlayer);
		}
	}
}
