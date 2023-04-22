

public class PastPlayer : SentientCharacter {

	// Update is called once per frame
	void Update() {
		if (SeesPresentPlayer()) {
			timeTravel.TimeParadox(TimeParadoxCause.PastPlayerSawPresentPlayer);
		} else if (SeesObjectInteractionFromPresentPlayer()) {
			timeTravel.TimeParadox(TimeParadoxCause.PastPlayerSawObjectInteractionFromPresentPlayer);
		}
	}

}
