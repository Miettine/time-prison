using System;
using System.Collections.Generic;
using UnityEngine;
using static CharacterInTime;

public class Player : MonoBehaviour
{

	new Rigidbody rigidbody;
	TimeTravel timeTravel;

	[SerializeField]
	private float deadzone = 0.1f;

	[SerializeField]
	private float moveSpeed = 1000f;

	[SerializeField]
	private float lookTowardsRotationModifier = 250f;

	[SerializeField]
	float hideSoundIndicatorDelay = 0.5f;

	[SerializeField]
	float runningSoundWaveRadius = 3f;

	[SerializeField]
	float sneakingSpeedMultiplier = 0.5f;

	UI ui;

	Quaternion lastLookDirection;

	ISet<KeyCardType> keycards = new HashSet<KeyCardType>();

	GameObject soundIndicator;

	int pastPlayerLayer;

	internal ActionType LatestAction { get; set; }

	int interactableObjectsLayerMask;

	private void Awake() {
		pastPlayerLayer = LayerMask.GetMask("PastPlayer");

		rigidbody = GetComponent<Rigidbody>();
		timeTravel = FindObjectOfType<TimeTravel>();
		interactableObjectsLayerMask = LayerMask.GetMask("Interactable");
		ui = FindObjectOfType<UI>();

		var soundIndicatorTransform = transform.Find("SoundIndicator");
		soundIndicatorTransform.localScale = new Vector3(runningSoundWaveRadius*2, 1, runningSoundWaveRadius*2);
		soundIndicator = soundIndicatorTransform.gameObject;
	}

	internal bool HasKeyCard(KeyCardType type) {
		return keycards.Contains(type);
	}

	// Start is called before the first frame update
	void Start()
	{
		soundIndicator.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{
		{
			float v = Input.GetAxisRaw("Vertical");
			float h = Input.GetAxisRaw("Horizontal");

			bool sneaking = Input.GetKey(KeyCode.C);

			ProcessMovementInput(new Vector3(h, 0f, v), sneaking);
		}
		
		if (Input.GetKeyUp(KeyCode.Space)) {
			LatestAction = CharacterInTime.ActionType.StartTimeTravel;
			timeTravel.StartTimeTravelToBeginning();
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
				keyCardTerminal.Interact(this);
				
				return;
			}
		}
	}

	void ProcessMovementInput(Vector3 direction, bool sneaking){
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

			float movementForceRunning =  moveSpeed * UnityEngine.Time.deltaTime;

			var movementVector = sneaking ? direction.normalized * movementForceRunning * sneakingSpeedMultiplier : direction.normalized * movementForceRunning;

			rigidbody.AddForce(movementVector);

			if (timeTravel.TimeTravelling && !sneaking) {
				SendSoundWaves();
			} else if (timeTravel.TimeTravelling && sneaking) {
				CancelInvoke("HideSoundIndicator");
				HideSoundIndicator();
			}

			LatestAction = CharacterInTime.ActionType.Walking;
		} else {
			LatestAction = CharacterInTime.ActionType.Standing;
		}
	}

	void SendSoundWaves() {
		CancelInvoke("HideSoundIndicator");

		soundIndicator.SetActive(true);

		var soundOverlapSphereColliders = Physics.OverlapSphere(transform.position, runningSoundWaveRadius, pastPlayerLayer, QueryTriggerInteraction.Collide);
		
		foreach (var collider in soundOverlapSphereColliders) {
			if (collider.gameObject.layer == LayerMask.NameToLayer("PastPlayer")) {
				timeTravel.TimeParadox(TimeParadoxCause.PastPlayerHeardPresentPlayer);
			}
		}
		
		Invoke("HideSoundIndicator", hideSoundIndicatorDelay);
	}

	void HideSoundIndicator() {
		soundIndicator.SetActive(false);
	}
}
