public abstract class ObjectInTime {

	public string Name { get; private set; }

	public float Time { get; private set; }

	public ObjectInTime(string name, float time) {
		Name = name;
		Time = time;
	}

	new virtual public string ToString() {
		return $"Id {Name}, Time {Time}";
	}
}