public class SecuritySystemInTime : ObjectInTime {

	public bool Alarm { get; private set; } = false;

	public SecuritySystemInTime(string id, float time, bool alarm) : base(id, time) {
		Alarm = alarm;
	}

	override public string ToString() {
		return $"{base.ToString()}, Alarm {Alarm}";
	}
}
