using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPedestal : MonoBehaviour
{
	[SerializeField]
	LargeDoor door;

	UI ui;

	/// <summary>
	/// Whether this is a one-shot button. A one-shot button can be pressed only once. Buttons that
	/// are not one-shot can be pressed many times during a level.
	/// </summary>
	[SerializeField]
	bool oneShot = false;

	bool interactable = true;

	[SerializeField]
	float openForTime = 5;

	private void Awake() {
		if (door == null) {
			throw new System.Exception("Reference to door is null. Please set the reference.");
		}

		ui = FindObjectOfType<UI>();
	}

	public bool IsInteractable() {
		return interactable;
	}

	public bool IsOneShot() {
		return oneShot;
	}

	public void Interact() {
		if (!interactable) {
			return;
		}
		CancelInvoke();
		door.OpenByPresentAction();

		Invoke("CloseDoor", openForTime);

		ui.ShowDoorOpenForSeconds(openForTime);

		if (oneShot) {
			transform.Find("PressableButton").gameObject.SetActive(false);
			interactable = false;
		}

	}
	void CloseDoor() {
		door.CloseByPresentAction();
	}
}
