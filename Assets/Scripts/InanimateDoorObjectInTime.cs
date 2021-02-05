public class InanimateDoorObjectInTime : ObjectInTime{
	public InanimateObjectType ObjectType { get; private set; }
	public bool IsOpen { get; private set; }

	public InanimateDoorObjectInTime(string id, float time, InanimateObjectType type, bool isOpen) : base(id, time) {
		ObjectType = type;
		IsOpen = isOpen;
	}
}