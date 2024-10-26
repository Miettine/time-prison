using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : SentientCharacter {

	void Update() {
		if (SeesPresentPlayer())
		{
			throw new NotSupportedException("The guard character is currently not supported.");
			//timeTravel.TimeParadox(TimeParadoxCause.GuardCaughtPlayer);
		}
	}
}
