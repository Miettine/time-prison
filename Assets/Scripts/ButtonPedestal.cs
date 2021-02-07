using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPedestal : MonoBehaviour
{
	[SerializeField]
	LargeDoor door;

	[SerializeField]
	float openForTime = 5;

	private void Awake() {
		if (door == null) {
			throw new System.Exception("Reference to door is null. Please set the reference.");
		}
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
