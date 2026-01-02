public class PressurePlateInTime : ObjectInTime {
	public bool IsActivated { get; private set; }

	public PressurePlateInTime(string id, float time, bool isActivated) : base(id, time) {
		IsActivated = isActivated;
	}
}