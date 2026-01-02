public class DoorObjectInTime : ObjectInTime{
	public bool IsOpen { get; internal set; }

	public DoorObjectInTime(string name, float time, bool isOpen) : base(name, time) {
		IsOpen = isOpen;
	}

	public override string ToString() {
		return $"{Name} ({base.Time}):{IsOpen}";
	}
}