public abstract class ObjectInTime {

	public string Id { get; private set; }

	public float Time { get; private set; }

	public ObjectInTime(string id, float time) {
		Id = id;
		Time = time;
	}

	new virtual public string ToString() {
		return $"Id {Id}, Time {Time}";
	}
}