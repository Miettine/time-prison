using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
	bool activated = false;
	bool activatedByPastAction = false;

	int playerLayer;
	private void Awake() {
		playerLayer = LayerMask.NameToLayer("Player");
	}
	private void OnTriggerEnter(Collider other) {
		UpdateActivated(other, true);
	}

	private void OnTriggerExit(Collider other) {
		UpdateActivated(other, false);
	}

	void UpdateActivated(Collider other, bool activate) {
		if (other.gameObject.layer == playerLayer) {
			activated = activate;
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
