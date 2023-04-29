

public class PastPlayer : SentientCharacter {

	// Update is called once per frame
	void Update() {
		if (SeesPresentPlayer()) {
			timeTravel.TimeParadox(TimeParadoxCause.PastPlayerSawPresentPlayer, transform);
		} else if (SeesObjectInteractionFromPresentPlayer()) {
			timeTravel.TimeParadox(TimeParadoxCause.PastPlayerSawObjectInteractionFromPresentPlayer, TimeParadoxObject);
		}
	}

}
