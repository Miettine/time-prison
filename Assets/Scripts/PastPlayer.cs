

using static LargeDoor;

public class PastPlayer : SentientCharacter {

	// Update is called once per frame
	void Update() {
		if (SeesPresentPlayer()) {
			timeTravel.TimeParadox(TimeParadoxCause.PastPlayerSawPresentPlayer, transform);
		} else if (SeesDoorPresentPlayerInteractedWith(out DoorTimeTravelState? doorState)) {
			// Coming up with design for time travel code is hard... :(
			if (doorState.HasValue) {
				if (doorState.Value.OpenInPast != doorState.Value.OpenInPresent) {
					if (doorState.Value.ClosedInPast && doorState.Value.OpenInPresent) {
						timeTravel.TimeParadox(TimeParadoxCause.PastPlayerSawDoorOpenedByPresentPlayer, TimeParadoxObject);
					} else if (doorState.Value.OpenInPast && doorState.Value.ClosedInPresent) {
						timeTravel.TimeParadox(TimeParadoxCause.PastPlayerSawDoorClosedByPresentPlayer, TimeParadoxObject);
					}
				}
			}
		}
	}
}
