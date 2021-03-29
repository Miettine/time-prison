public class DoorObjectInTime : ObjectInTime{
	public InanimateObjectType ObjectType { get; private set; }
	public bool IsOpen { get; private set; }

	public DoorObjectInTime(string id, float time, InanimateObjectType type, bool isOpen) : base(id, time) {
		ObjectType = type;
		IsOpen = isOpen;
	}

	public override string ToString() {
		return $"{Id} ({base.Time}):{IsOpen}";
	}
}