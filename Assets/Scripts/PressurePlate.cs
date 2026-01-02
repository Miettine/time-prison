using UnityEngine;

public class PressurePlate : MonoBehaviour {
	public bool ActivatedByPresentAction { get; private set; } = false;
	public bool ActivatedByPastAction { get; internal set; } = false;

	int playerLayer;
	UI ui;

	[SerializeField]
	LargeDoor door;

	private void Awake() {
		if (door == null) {
			var doors = FindObjectsByType<LargeDoor>(FindObjectsSortMode.None);
			if (doors.Length == 1) {
				door = doors[0];
			} else {
				throw new System.Exception("Please set a reference to a door.");
			}
		}

		playerLayer = LayerMask.NameToLayer("Player");

		ui = UI.GetInstance();
	}
	private void OnTriggerEnter(Collider other) {
		Debug.Log("Trigger entered on pressure plate Activated by past action: " + ActivatedByPastAction);
		if (other.gameObject.layer == playerLayer && !ActivatedByPastAction) {
			Debug.Log("Stepped on pressure plate");
			ActivatedByPresentAction = true;
			door.OpenByPresentAction();
			ui.ShowDoorOpenPermanentNotification();
		}
	}

	private void OnTriggerExit(Collider other) {
		Debug.Log("Trigger exited on pressure plate Activated by past action: " + ActivatedByPastAction);
		if (other.gameObject.layer == playerLayer && !ActivatedByPastAction) {
			Debug.Log("Stepped out of pressure plate");
			ActivatedByPresentAction = false;
			door.CloseByPresentAction();
			ui.ShowDoorClosed();
		}
	}
}
