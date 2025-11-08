using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPedestal : MonoBehaviour, IEffectedByTimeTravel
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

	GameObject buttonMechanismObject;

	public void ActivatedByPastPlayer() {
		if (interactable) {
			SetButtonActive(false);
		}
	}

	private void Awake() {
		if (door == null) {

			var doors = FindObjectsByType<LargeDoor>(FindObjectsSortMode.None);

			if (doors.Length == 1) {
				door = doors[0];
			} else {
				throw new System.Exception("Reference to door is null. Many doors present in scene. Please set the reference.");
			}
		}
		buttonMechanismObject = transform.Find("ButtonMechanism").gameObject;

		ui = UI.GetInstance();
	}

	void Update() {
        if (interactable) {
            ui.ShowPressTextAtWorldObject(buttonMechanismObject.transform);
        } else {
            ui.HidePressText();
        }
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
			SetButtonActive(false);
		}
	}
	void CloseDoor() {
		door.CloseByPresentAction();
	}

	void SetButtonActive(bool active) {
		buttonMechanismObject.SetActive(active);
		interactable = active;

		if (active) {
			ui.ShowPressTextAtWorldObject(buttonMechanismObject.transform);
		} else {
			ui.HidePressText();
		}
	}

	public void OnTimeTravelStarted() {
		SetButtonActive(true);
	}
}
