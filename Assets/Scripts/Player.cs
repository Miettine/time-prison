using System;
using System.Collections.Generic;
using UnityEngine;
using static CharacterInTime;

public class Player : Singleton<Player>
{

	new Rigidbody rigidbody;
	Collider physicsCollisionCollider;

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

	Locker lockerHiddenIn;

	internal ActionType LatestAction { get; set; }

	int interactableObjectsLayerMask;

	private void Awake() {
		pastPlayerLayer = LayerMask.GetMask("PastPlayer");

		physicsCollisionCollider = GetComponentInChildren<Collider>();
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
	void Update() {
		bool isHiding = IsHiding();

		if (Input.GetKeyUp(KeyCode.Space)) {
			LatestAction = CharacterInTime.ActionType.StartTimeTravel;
			OnTimeTravelActivated();
			return;
		}
		if (Input.GetKeyDown(KeyCode.E)) {
			if (isHiding) {
				LatestAction = CharacterInTime.ActionType.ExitLocker;
				LeaveLocker(lockerHiddenIn);
			} else {
				InteractWithNearbyObjects();
			}
		}
		//There used to be a check if(!isHiding) right about here. The hiding feature has been discontinued.

		if (Input.GetMouseButton(0)) {
			ui.OnMouseInputUsed();

			// Some of the code on this block was created with the assistance of ChatGPT!
			// The following code is not fully AI-generated, but some information was gathered through an AI chat bot instead of Googling.
			// How exciting! :D
			// Added "ChatGPT:" on comments and lines that were written by Artificial Intelligence

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // ChatGPT: Cast a ray from the camera towards the mouse position

			float rayDistance; // ChatGPT: Declare a variable to store the distance along the ray to the intersection point
			Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // ChatGPT: Create a plane at y = 0

			if (groundPlane.Raycast(ray, out rayDistance)) // ChatGPT: Check if the ray intersects with the ground plane
			{
				Vector3 groundPoint = ray.GetPoint(rayDistance); // ChatGPT: Get the point of intersection between the ray and the ground plane

				/* The player either moves towards the mouse or the player moves to the opposite direction from where the mouse is clicked.
				 * When this game is played on a phone, some players could complain that they can't see where the character is going because
				 * their finger is blocking the view. I am making the following variable in preparation of configuring the movement:
				 * allowing the player to make the character move towards the finger or away from the finger.
				 */
				bool moveTowardsMouse = true;

				float movementCoordinateX = moveTowardsMouse ? groundPoint.x - transform.position.x : transform.position.x - groundPoint.x;
				float movementCoordinateZ = moveTowardsMouse ? groundPoint.z - transform.position.z : transform.position.x - groundPoint.x;

				ProcessMovementInput(new Vector3(movementCoordinateX, 0f, movementCoordinateZ).normalized, false);
				return;
			}
		} else if (Input.anyKey) {
			ui.OnKeyboardInputUsed();

			float v = Input.GetAxisRaw("Vertical");
			float h = Input.GetAxisRaw("Horizontal");

			bool sneaking = Input.GetKey(KeyCode.C);

			ProcessMovementInput(new Vector3(h, 0f, v), sneaking);
			return;
		}
	}

	private static bool AnyMouseButtonDown() {
		return LeftMouseButtonDown() || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2);
	}

	private static bool LeftMouseButtonDown() {
		return Input.GetMouseButtonDown(0);
	}

	public void OnTimeTravelActivated() {
		timeTravel.StartTimeTravelToBeginning();
	}

	public void OnResetActivated() {
		timeTravel.ResetTimeline();
	}


	public bool IsHiding() {
		return lockerHiddenIn != null;
	}
	
	void HideInLocker(Locker locker) {
		timeTravel.PlayerHidesInLocker();
		locker.OccupiedByPresentPlayer = true;

		LatestAction = CharacterInTime.ActionType.EnterLocker;
		//Later there will be animations, so there will be a short delay when the player enters the locker and when they are hidden inside.

		this.transform.position = locker.transform.position;
		physicsCollisionCollider.enabled = false;
		rigidbody.isKinematic = true;

		lockerHiddenIn = locker;
		LatestAction = CharacterInTime.ActionType.HidingInLocker;
	}

	void LeaveLocker(Locker locker) {
		physicsCollisionCollider.enabled = true;
		locker.OccupiedByPresentPlayer = false;
		rigidbody.isKinematic = false;
		this.transform.position = locker.transform.position + new Vector3(0f, 0f, -1.5f);

		lockerHiddenIn = null;
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

			if (buttonPedestal != null && buttonPedestal.IsInteractable() ) {
				buttonPedestal.Interact();
				return;
			}

			var keyCardTerminal = collider.gameObject.GetComponentInParent<KeyCardTerminal>();

			if (keyCardTerminal != null) {
				keyCardTerminal.Interact(this);
				
				return;
			}

			if (collider.gameObject.tag == "Locker") {
				HideInLocker(collider.gameObject.GetComponentInParent<Locker>());
			}
		}
	}

	void ProcessMovementInput(Vector3 direction, bool sneaking){
		if (IsHiding()) {
			return;
		}

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
				timeTravel.TimeParadox(TimeParadoxCause.PastPlayerHeardPresentPlayer, collider.transform);
			}
		}
		
		Invoke("HideSoundIndicator", hideSoundIndicatorDelay);
	}

	void HideSoundIndicator() {
		CancelInvoke("HideSoundIndicator");
		soundIndicator.SetActive(false);
	}
}
