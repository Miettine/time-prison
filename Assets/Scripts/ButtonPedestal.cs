using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPedestal : MonoBehaviour
{
	[SerializeField]
	LargeDoor door;

	UI ui;

	[SerializeField]
	float openForTime = 5;

	private void Awake() {
		if (door == null) {
			throw new System.Exception("Reference to door is null. Please set the reference.");
		}

		ui = FindObjectOfType<UI>();
	}
	public void Interact() {
		CancelInvoke();
		door.OpenByPresentAction();

		Invoke("CloseDoor", openForTime);

		ui.ShowDoorOpenForSeconds(openForTime);
	}
	void CloseDoor() {
		door.CloseByPresentAction();
	}
}
