using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimeParadoxCause {
	None,
	PastPlayerSawPresentPlayer, 
	PastPlayerHeardPresentPlayer, 
	PastPlayerFoundPresentPlayerHiding,
	PastPlayerSawObjectInteractionFromPresentPlayer,
	GuardCaughtPlayer
}
