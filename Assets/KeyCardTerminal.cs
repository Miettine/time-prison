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

	public void Interact() {
		CancelInvoke();
		door.OpenByPresentAction();

		Invoke("CloseDoor", openForTime);
	}
	void CloseDoor() {
		door.CloseByPresentAction();
	}
}
