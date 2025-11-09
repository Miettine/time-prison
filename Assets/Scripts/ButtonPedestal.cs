using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPedestal : MonoBehaviour, IEffectedByTimeTravel
{
	[SerializeField]
	LargeDoor door;

    public Transform ButtonMechanismTransform { get; private set; }

    UI ui;
	Player player;

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

    // Backing field for the linked interact prompt. Can only be set once.
    private InteractPrompt _linkedInteractPrompt;

    // Public getter, internal setter that only allows a single assignment.
    public InteractPrompt LinkedInteractPrompt
    {
        get => _linkedInteractPrompt;
        private set
        {
            if (_linkedInteractPrompt != null)
            {
                throw new InvalidOperationException("LinkedInteractPrompt can only be set once.");
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "LinkedInteractPrompt cannot be set to null.");
            }

            _linkedInteractPrompt = value;
        }
    }

	 public void ActivatedByPastPlayer() {
			if (interactable) {
				SetButtonActive(false);
			}
		}

	 private void Awake() {

        ButtonMechanismTransform = transform.Find("ButtonMechanism");
		
		ui = UI.GetInstance();
		player = Player.GetInstance();

        if (door == null) {
			var doors = FindObjectsByType<LargeDoor>(FindObjectsSortMode.None);

			if (doors.Length == 1) {
				door = doors[0];
			} else {
				throw new Exception("Reference to door is null. Many doors present in scene. Please set the reference.");
			}
		}
	}

	void Start(){
        // Create and link an InteractPrompt for this pedestal. Must be done on Start to ensure UI instance is ready.
        LinkedInteractPrompt = ui.OnButtonPedestalCreated(this);
    }

	void Update() {
		if (player.FocusedInteractableObject == this)
		{
            LinkedInteractPrompt.gameObject.SetActive(true);
            LinkedInteractPrompt.ShowInteractPromptAtWorldObject(ButtonMechanismTransform);
        }
		else
		{
			LinkedInteractPrompt.gameObject.SetActive(false);
            LinkedInteractPrompt.HideInteractPrompt();
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
	}

	public void OnTimeTravelStarted() {
		SetButtonActive(true);
	}
}
