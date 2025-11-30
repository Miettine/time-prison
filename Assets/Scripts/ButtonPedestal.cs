using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPedestal : MonoBehaviour, IEffectedByTimeTravel
{
	[SerializeField]
	LargeDoor door;

	/// <summary>
	/// We want there to be a specific point on the button mechanism where the interact prompt appears.
	/// The button mechanism could do but that might be offset or rotated in some way.
	/// This empty game object is used as the target point for the prompt.
	/// </summary>
	public Transform InteractPromptTargetTransform { get; private set; }

	private Transform buttonMechanismGameObject;

	private Vector3 buttonMechanismStartPosition;
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

	Coroutine pushCoroutine;

	public void ActivatedByPastPlayer() {
		if (interactable) {
			SetButtonActive(false);
		}
	}

	private void Awake() {

		InteractPromptTargetTransform = transform.Find("InteractPromptTarget");
		if (InteractPromptTargetTransform == null) {
			throw new Exception("InteractPromptTarget child not found on ButtonPedestal");
		}

		buttonMechanismGameObject = transform.Find("ButtonMechanism");
		if (buttonMechanismGameObject == null)
		{
			throw new Exception("ButtonMechanism child not found on ButtonPedestal");
		}

		buttonMechanismStartPosition = buttonMechanismGameObject.localPosition;

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
		// It's hard to say where this code belongs. It could go to Player::Update or InteractPrompt::Update.
		// A reference to the button mechanism's world position is needed to position the prompt.
		// This is the best place.
		if (player.FocusedInteractableObject == this && interactable)
		{
			LinkedInteractPrompt.ShowAtWorldObject(InteractPromptTargetTransform);
		}
		else
		{
			LinkedInteractPrompt.Hide();
		}
	}

	void OnDestroy() {
		if (LinkedInteractPrompt != null&& LinkedInteractPrompt.gameObject != null)
		{
			Destroy(LinkedInteractPrompt.gameObject);
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

		// play push animation
		if (pushCoroutine != null) {
			StopCoroutine(pushCoroutine);
		}
		pushCoroutine = StartCoroutine(PushButtonAnimation());

		CancelInvoke();
		door.OpenByPresentAction();

		Invoke("CloseDoor", openForTime);

		ui.ShowDoorOpenForSeconds(openForTime);

		if (oneShot) {
			LinkedInteractPrompt.Hide();
			SetButtonActive(false);
		}
	}
	void CloseDoor() {
		door.CloseByPresentAction();
	}

	void SetButtonActive(bool active) {
		// Ensure we toggle the mechanism gameobject via the transform reference
		buttonMechanismGameObject.gameObject.SetActive(active);
		interactable = active;
	}

	public void OnTimeTravelStarted() {
		SetButtonActive(true);
	}

	IEnumerator PushButtonAnimation()
	{
		// Animate localPosition Y to target 1.3 and back

		Transform rt = buttonMechanismGameObject.transform;
		Vector3 start = buttonMechanismStartPosition;
		Vector3 down = new Vector3(start.x, 1.3f, start.z);

		float downTime =0.08f;
		float upTime =0.12f;

		float t =0f;
		while (t < downTime)
		{
			t += Time.deltaTime;
			rt.localPosition = Vector3.Lerp(start, down, Mathf.Clamp01(t / downTime));
			yield return null;
		}
		rt.localPosition = down;

		// small hold at bottom
		yield return new WaitForSeconds(0.05f);

		t =0f;
		while (t < upTime)
		{
			t += Time.deltaTime;
			rt.localPosition = Vector3.Lerp(down, start, Mathf.Clamp01(t / upTime));
			yield return null;
		}
		rt.localPosition = start;

		pushCoroutine = null;
	}
}
