using System;
using UnityEngine;

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

    private void Update()
    {
        var touchControlsEnabled = player._ControlMode == Player.ControlMode.Touch;

        touchPrompt.SetActive(touchControlsEnabled);
        keyboardPrompt.SetActive(!touchControlsEnabled);

        Camera cam = Camera.main;

        transform.position = cam.WorldToScreenPoint(LinkedButtonPedestal.ButtonMechanismTransform.position);
    }

    /// <summary>
    /// Show a UI text that will be positioned on the Canvas where the specified world position is.
    /// Useful for showing small contextual hints above world objects.
    /// </summary>
    /// <param name="targetTransform">The world position to place the text above.</param>
    public void ShowPressTextAtWorldObject(Transform targetTransform)
    {
        // For ScreenSpaceOverlay canvases the camera parameter must be null
        Camera cam = Camera.main;

        transform.position = cam.WorldToScreenPoint(targetTransform.position);
        gameObject.SetActive(true);
    }

    public void ShowTouchPrompt(bool show)
    {
        if (touchPrompt != null) touchPrompt.SetActive(show);
    }

    public void ShowKeyboardPrompt(bool show)
    {
        if (keyboardPrompt != null) keyboardPrompt.SetActive(show);
    }
}
