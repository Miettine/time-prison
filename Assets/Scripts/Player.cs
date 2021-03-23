using System;
using System.Collections.Generic;
using UnityEngine;
using static CharacterInTime;

public class Player : MonoBehaviour
{

	new Rigidbody rigidbody;
	TimeTravel timeTracker;

	[SerializeField]
	private float deadzone = 0.1f;

	[SerializeField]
	private float moveSpeed = 1000f;

	[SerializeField]
	private float lookTowardsRotationModifier = 250f;

	UI ui;

	Quaternion lastLookDirection;

	ISet<KeyCardType> keycards = new HashSet<KeyCardType>();

	internal ActionType LatestAction { get; set; }

	int interactableObjectsLayerMask;

	private void Awake() {
		rigidbody = GetComponent<Rigidbody>();
		timeTracker = FindObjectOfType<TimeTravel>();
		interactableObjectsLayerMask = LayerMask.GetMask("Interactable");
		ui = FindObjectOfType<UI>();
	}

	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		{
			float v = Input.GetAxisRaw("Vertical");
			float h = Input.GetAxisRaw("Horizontal");

			ProcessMovementInput(new Vector3(h, 0f, v));
		}
		
		if (Input.GetKeyUp(KeyCode.Space)) {
			LatestAction = CharacterInTime.ActionType.StartTimeTravel;
			timeTracker.StartTimeTravelToBeginning();
			return;
		}
		if (Input.GetKeyDown(KeyCode.E)) {
			InteractWithNearbyObjects();
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.layer == LayerMask.NameToLayer("KeyCards")) {
			var card = other.gameObject.GetComponentInChildren<KeyCard>();
			var type = card.GetKeyCardType();
			keycards.Add(type);
			ui.ShowKeyCardIndicator(type, true);
			Destroy(other.gameObject);
		}
	}

	private void InteractWithNearbyObjects() {
		var interactableObjectsColliders = Physics.OverlapSphere(transform.position, 2f, interactableObjectsLayerMask);

		if (interactableObjectsColliders.Length == 0) {
			return;
		}

		foreach (var collider in interactableObjectsColliders) {

			var buttonPedestal = collider.gameObject.GetComponentInParent<ButtonPedestal>();

			if (buttonPedestal != null) {
				buttonPedestal.Interact();
				return;
			}

			var keyCardTerminal = collider.gameObject.GetComponentInParent<KeyCardTerminal>();

			if (keyCardTerminal != null) {
				if (keycards.Contains(keyCardTerminal.GetKeyCardType())) {
					/**
					 * In reality the player would present a keycard to the door and the door would check for permissions.
					 * Here the permissions check is done within the player-object.
					 * The way this door's security functions is not realistic nor very "object-oriented". 
					 * Spending time redesigning this code to be more "realistic" is not worth the time and effort.
					 * The change would be invisible from the player's perspective and I will rather spend the time 
					 * doing something else that players will like.
					*/
					keyCardTerminal.Interact();
				} else {
					Debug.Log("You do not have the correct keycard to open this door");
				}
				
				return;
			}
		}
	}

	private void ProcessMovementInput(Vector3 direction){
		// I want the player character to rotate slowly towards the direction that the player pushed the arrow keys in. The following code accomplishes this.
		
		Quaternion lookRotation;
		if (direction.magnitude > deadzone) {
			 lookRotation = Quaternion.LookRotation(direction, Vector3.up);
		} else if (lastLookDirection != null) {
			lookRotation = lastLookDirection;
		} else {
			lookRotation = Quaternion.identity;
		}

		transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, UnityEngine.Time.deltaTime * lookTowardsRotationModifier);

		if (direction.magnitude > deadzone) 
		{
			lastLookDirection = lookRotation;

			rigidbody.AddForce(direction.normalized * moveSpeed * UnityEngine.Time.deltaTime);

			LatestAction = CharacterInTime.ActionType.Walking;
		} else {
			LatestAction = CharacterInTime.ActionType.Standing;
		}
	}
}
