using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
	bool activated = false;
	bool activatedByPastAction = false;

	int playerLayer;
	UI ui;

	private void Awake() {
		playerLayer = LayerMask.NameToLayer("Player");

		ui = FindObjectOfType<UI>();
	}
	private void OnTriggerEnter(Collider other) {
		Debug.Log(other.gameObject.layer);
		Debug.Log(playerLayer);
		if (other.gameObject.layer == playerLayer) {
			activated = true;
			
			ui.ShowDoorOpenPermanentNotification();
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.gameObject.layer == playerLayer) {
			activated = false;
			ui.ShowDoorClosed();
		}
	}

	public void ActivateByPastAction() {
		activatedByPastAction = true;
	}

	public void DeactivateByPastAction() {
		activatedByPastAction = false;
	}
	public bool isActivated() {
		return activated;
	}
}
