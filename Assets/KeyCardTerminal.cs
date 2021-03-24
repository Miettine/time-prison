using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCardTerminal : MonoBehaviour {
	[SerializeField]
	KeyCardType type;

	LargeDoor door;

	[SerializeField]
	float openForTime = 5;

	//TODO: Lessen copypasting between this class and ButtonPedestal

	private void Awake() {
		door = GetComponentInParent<LargeDoor>();
	}

	internal KeyCardType GetKeyCardType() {
		return type;
	}

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
			Interact(other.gameObject.GetComponentInParent<Player>());
		}
	}

	/*private void OnTriggerExit(Collider other) {
		CloseDoor();
	}*/

	public void Interact(Player player) {

		if (player.HasKeyCard(type)) {
			CancelInvoke();
			door.OpenByPresentAction();
			Invoke("CloseDoor", openForTime);
		} else {
			Debug.Log("You do not have the correct keycard to open this door");
		}
	}
	void CloseDoor() {
		door.CloseByPresentAction();
	}
}
