internal class ButtonPedestalInTime : ObjectInTime{

	public bool IsInteractable { get; private set; }

	public ButtonPedestalInTime(string id, float time, bool isInteractable) : base(id, time) {
		IsInteractable = isInteractable;
	}
}