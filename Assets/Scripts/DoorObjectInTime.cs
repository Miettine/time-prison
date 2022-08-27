public class DoorObjectInTime : ObjectInTime{
	public InanimateObjectType ObjectType { get; private set; }
	public bool IsOpen { get; private set; }

	public DoorObjectInTime(string name, float time, InanimateObjectType type, bool isOpen) : base(name, time) {
		ObjectType = type;
		IsOpen = isOpen;
	}

	public override string ToString() {
		return $"{Name} ({base.Time}):{IsOpen}";
	}
}