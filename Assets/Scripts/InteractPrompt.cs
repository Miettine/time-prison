using System;
using UnityEngine;
using UnityEngine.UI;

public class InteractPrompt : MonoBehaviour
{
	private GameObject touchPrompt;
	private GameObject keyboardPrompt;

	private Player player;

	// Backing field for the linked pedestal. Can only be set once.
	private ButtonPedestal _linkedButtonPedestal;

	// setter that only allows a single assignment.
	public ButtonPedestal LinkedButtonPedestal
	{
		get => _linkedButtonPedestal;
		internal set
		{
			if (_linkedButtonPedestal != null)
			{
				// Fail fast to make incorrect reassignments obvious during development.
				throw new InvalidOperationException("LinkedButtonPedestal can only be set once.");
			}

			if (value == null)
			{
				throw new ArgumentNullException(nameof(value), "LinkedButtonPedestal cannot be set to null.");
			}

			_linkedButtonPedestal = value;
		}
	}

	private void Awake()
	{
		player = Player.GetInstance();

		touchPrompt = transform.Find("InteractPromptTouchScreen").gameObject;

		keyboardPrompt = transform.Find("InteractPromptKeyboard").gameObject;
	}

	private void Start()
	{
		// Initially hide both prompts until we know which to show.
		Hide();

		if (LinkedButtonPedestal == null)
		{
			throw new Exception("LinkedButtonPedestal is not set on InteractPrompt. Please ensure it is set before Start is called.");
		}

		// Add an onClick listener to the Button component on this GameObject (if present).
		var btn = GetComponentInChildren<Button>(includeInactive: true);

		if (btn == null)
		{
			throw new Exception("No Button component found in children of InteractPrompt. Please ensure there is a Button component present.");
		}
		btn.onClick.AddListener(() =>
		{
			LinkedButtonPedestal.Interact();
		});
	}

	private void Update()
	{
		// Don't type code for comparing what is the player's focused interactable object. That is in ButtonPedestal.

		UpdateControlMode();

		Camera cam = Camera.main;

		transform.position = cam.WorldToScreenPoint(LinkedButtonPedestal.InteractPromptTargetTransform.position);
	}

	/// <summary>
	/// Show a UI text that will be positioned on the Canvas where the specified world position is.
	/// Useful for showing small contextual hints above world objects.
	/// </summary>
	/// <param name="targetTransform">The world position to place the text above.</param>
	public void ShowAtWorldObject(Transform targetTransform)
	{
		// For ScreenSpaceOverlay canvases the camera parameter must be null
		Camera cam = Camera.main;

		transform.position = cam.WorldToScreenPoint(targetTransform.position);
		gameObject.SetActive(true);

		// Making a lot of calls to UpdateControlMode to ensure correct prompt is shown.
		// It tends to flash the incorrect one sometimes.
		UpdateControlMode();
	}
	private void UpdateControlMode()
	{
		var touchControlsEnabled = player._ControlMode == Player.ControlMode.Touch;
		touchPrompt.SetActive(touchControlsEnabled);
		keyboardPrompt.SetActive(!touchControlsEnabled);
	}


	internal void Hide()
	{
		gameObject.SetActive(false);

		// In some situations we could see both prompts briefly. Ensure both are hidden.
		touchPrompt.SetActive(false);
		keyboardPrompt.SetActive(false);
	}
}
