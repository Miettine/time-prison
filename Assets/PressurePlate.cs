using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
	bool activated = false;

	int playerLayer;
	UI ui;

	[SerializeField]
	LargeDoor door;

	private void Awake() {
		if (door == null) {
			throw new System.Exception("Reference to door is null. Please set the reference.");
		}

		playerLayer = LayerMask.NameToLayer("Player");

		ui = FindObjectOfType<UI>();
	}
	private void OnTriggerEnter(Collider other) {
		Debug.Log(other.gameObject.layer);
		Debug.Log(playerLayer);
		if (other.gameObject.layer == playerLayer) {
			activated = true;
			door.OpenByPresentAction();
			ui.ShowDoorOpenPermanentNotification();
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.gameObject.layer == playerLayer) {
			activated = false;
			door.CloseByPresentAction();
			ui.ShowDoorClosed();
		}
	}

	public bool isActivated() {
		return activated;
	}
}
