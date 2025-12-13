using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static CharacterInTime;

public class Player : Singleton<Player>
{

	new Rigidbody rigidbody;

	TimeTravel timeTravel;

	/// <summary>
	/// The player variables scriptable object that contains various configuration values for the player character.
	/// </summary>
	[SerializeField]
	PlayerVariables playerVariables;

	UI ui;

	Quaternion lastLookDirection;

	ISet<KeyCardType> keycards = new HashSet<KeyCardType>();

	GameObject soundIndicator;

	int pastPlayerLayer;

	/// <summary>
	/// The latest action performed by the player character. This is used to record what the player is doing so that we can play back the correct animation on time travel.
	/// </summary>
	internal ActionType LatestAction { get; private set; } = ActionType.Standing;

	int interactableObjectsLayerMask;

	private ControlMode controlMode;

	public bool HasObtainedTimeMachine { get; private set; } = false;

	public ControlMode _ControlMode
	{
		get => controlMode; private set
		{
			controlMode = value;
		}
	 }

	// Made these properties to make the code more readable.
	float Deadzone => playerVariables.Deadzone;
	float MoveSpeed => playerVariables.MoveSpeed;
	float LookTowardsRotationModifier => playerVariables.LookTowardsRotationModifier;
	float HideSoundIndicatorDelay => playerVariables.HideSoundIndicatorDelay;
	float RunningSoundWaveRadius => playerVariables.RunningSoundWaveRadius;
	float SneakingRadius => playerVariables.SneakingRadius;
	float SneakingSpeedMultiplier => playerVariables.SneakingSpeedMultiplier;
	float InteractionRadius => playerVariables.InteractionRadius;

	/// <summary>
	/// An object that is within the player character's arms reach and can be interacted with. Null if there is no such object.
	/// Currently this can only be a ButtonPedestal. It would be better design to make this a more general interface like IInteractableObject.
	/// Though this is the only kind of object that the player can presently interact with.
	/// </summary>
	public ButtonPedestal FocusedInteractableObject { get; private set; }

	 public enum ControlMode
	 {
	 Touch,
	 Keyboard
	 }


	private void Awake() {
		pastPlayerLayer = LayerMask.GetMask("PastPlayer");

		rigidbody = GetComponent<Rigidbody>();
		timeTravel = TimeTravel.GetInstance();
		interactableObjectsLayerMask = LayerMask.GetMask("Interactable");
		ui = UI.GetInstance();

		var soundIndicatorTransform = transform.Find("SoundIndicator");
		// use values from the scriptable object
		soundIndicatorTransform.localScale = new Vector3(RunningSoundWaveRadius *2,1, RunningSoundWaveRadius *2);
		soundIndicator = soundIndicatorTransform.gameObject;
	}

	internal bool HasKeyCard(KeyCardType type) {
		return keycards.Contains(type);
	}

	// Start is called before the first frame update
	void Start()
	{
		soundIndicator.SetActive(false);
		HasObtainedTimeMachine = Tutorial.LevelStartsWithTimeMachine();
	}

	// Update is called once per frame
	void Update() {
		if (timeTravel.IsTimeParadoxOngoing()) {
			return;
		}

		if (Input.GetKeyUp(KeyCode.Space)) {
			OnPlayerWantsToActivateTimeMachine();
			return;
		}

		FindNearbyFocusedObject();

		if (Input.GetKeyDown(KeyCode.E) && FocusedInteractableObject != null) {
			OnKeyboardInputUsed();
			InteractWithFocusedObject();
		}

		//There used to be a check if(!isHiding) right about here. The hiding feature has been discontinued.

		if (Input.GetMouseButton(0)) {
			OnMouseInputUsed();

			if (EventSystem.current.IsPointerOverGameObject()) {
				return;
			}

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Cast a ray from the camera towards the mouse position

			float rayDistance; // Declare a variable to store the distance along the ray to the intersection point
			Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Create a plane at y = 0

			if (groundPlane.Raycast(ray, out rayDistance)) // Check if the ray intersects with the ground plane
			{
				Vector3 groundPoint = ray.GetPoint(rayDistance); // Get the point of intersection between the ray and the ground plane

				/* The player either moves towards the mouse or the player moves to the opposite direction from where the mouse is clicked.
				* When this game is played on a phone, some players could complain that they can't see where the character is going because
				* their finger is blocking the view. I am making the following variable in preparation of configuring the movement:
				* allowing the player to make the character move towards the finger or away from the finger.
				*/
				bool moveTowardsMouse = true;

				float movementCoordinateX = moveTowardsMouse ? groundPoint.x - transform.position.x : transform.position.x - groundPoint.x;
				float movementCoordinateZ = moveTowardsMouse ? groundPoint.z - transform.position.z : transform.position.x - groundPoint.x;

				var movementVector = new Vector3(movementCoordinateX, 0f, movementCoordinateZ);

				var sneaking = Vector3.Distance(transform.position, groundPoint) < SneakingRadius;

				ProcessMovementInput(movementVector.normalized, sneaking);
				return;
			}
			return;
		}

		float v = Input.GetAxisRaw("Vertical");
		float h = Input.GetAxisRaw("Horizontal");
		var direction = new Vector3(h, 0f, v);

		if (direction.magnitude > Deadzone) {
			OnKeyboardInputUsed();

			bool sneaking = Input.GetKey(KeyCode.C);

			ProcessMovementInput(direction, sneaking);

			return;
		}
		//TODO: Add gamepad controls. Can we use gamepad in WebGL?
	}

	public void OnPlayerWantsToActivateTimeMachine()
	{
		if (HasObtainedTimeMachine)
		{
			OnTimeTravelActivated();
		}
	}

	public void OnTimeMachinePickedUp(){
		HasObtainedTimeMachine = true;
	}

	private void OnKeyboardInputUsed()
	{
		_ControlMode = ControlMode.Keyboard;
		ui.OnKeyboardInputUsed();
	}

	private void OnMouseInputUsed()
	{
		_ControlMode = ControlMode.Touch;
		ui.OnMouseInputUsed();
	}

	private static bool AnyMouseButtonDown() {
		return LeftMouseButtonDown() || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2);
	}

	private static bool LeftMouseButtonDown() {
		return Input.GetMouseButtonDown(0);
	}

	public void OnTimeTravelActivated() {
		if (!timeTravel.IsTimeParadoxOngoing()) {
			timeTravel.StartTimeTravelToBeginning();
			// The latest action is set to standing because when the time travel starts, the player character will be standing still.
			// Not setting the standing action has caused bugs in the animation playback system.
			// The action StartTimeTravel will be added in the TimeTravel class.
			LatestAction = ActionType.Standing;
		}
	}

	public void OnResetActivated() {
		timeTravel.ResetTimeline();
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

	private void InteractWithFocusedObject() {
		if (FocusedInteractableObject == null)
		{
			throw new Exception("FocusedInteractableObject is null when trying to interact with it.");
		}
		FocusedInteractableObject.Interact();
	}

	void FindNearbyFocusedObject()
	{
		var interactableObjectsColliders = Physics.OverlapSphere(transform.position, InteractionRadius, interactableObjectsLayerMask);

		if (interactableObjectsColliders.Length == 0)
		{
			FocusedInteractableObject = null;
			return;
		}

		foreach (var collider in interactableObjectsColliders)
		{
			if (IsOccludedByWall(collider.transform))
			{
				FocusedInteractableObject = null;
				continue;
			}

			var buttonPedestal = collider.gameObject.GetComponentInParent<ButtonPedestal>();

			if (buttonPedestal != null && buttonPedestal.IsInteractable())
			{
				FocusedInteractableObject = buttonPedestal;
				return;
			}
		}
	}

	void ProcessMovementInput(Vector3 direction, bool sneaking){

		// I want the player character to rotate slowly towards the direction that the player pushed the arrow keys in. The following code accomplishes this.

		Quaternion lookRotation;
		if (direction.magnitude > Deadzone) {
			lookRotation = Quaternion.LookRotation(direction, Vector3.up);
		} else if (lastLookDirection != null) {
			lookRotation = lastLookDirection;
		} else {
			lookRotation = Quaternion.identity;
		}

		transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, UnityEngine.Time.deltaTime * LookTowardsRotationModifier);

		if (direction.magnitude > Deadzone) 
		{
			lastLookDirection = lookRotation;

			float movementForceRunning = MoveSpeed * UnityEngine.Time.deltaTime;

			var movementVector = sneaking ? direction.normalized * movementForceRunning * SneakingSpeedMultiplier : direction.normalized * movementForceRunning;

			rigidbody.AddForce(movementVector);

			if (timeTravel.TimeTravelling && !sneaking) {
				SendSoundWaves();
			} else if (timeTravel.TimeTravelling && sneaking) {
				HideSoundIndicator();
			}
			LatestAction = ActionType.Walking;
		} else {
			LatestAction = ActionType.Standing;
		}
	}

	void SendSoundWaves() {
		CancelInvoke("HideSoundIndicator");

		soundIndicator.SetActive(true);

		var soundOverlapSphereColliders = Physics.OverlapSphere(transform.position, RunningSoundWaveRadius, pastPlayerLayer, QueryTriggerInteraction.Collide);
		
		foreach (var collider in soundOverlapSphereColliders) {
			if (collider.gameObject.layer == LayerMask.NameToLayer("PastPlayer")) {
				// If there is a wall or door between the present player and the past player, treat as occluded and do not trigger paradox.
				if (!IsOccludedByWall(collider.transform))
				{
					timeTravel.TimeParadox(TimeParadoxCause.PastPlayerHeardPresentPlayer, collider.transform);
				}
			}
		}
		
		Invoke("HideSoundIndicator", HideSoundIndicatorDelay);
	}

	void HideSoundIndicator() {
		CancelInvoke("HideSoundIndicator");
		soundIndicator.SetActive(false);
	}

	/// <summary>
	/// Returns true if there is a wall or door between the present player and the specified target transform.
	/// Uses a raycast from present player's approximate eye or ear height to the target's position and checks for hits on the "Walls" or "Doors" layers.
	/// </summary>
	private bool IsOccludedByWall(Transform target)
	{
		if (target == null) return true;

		Vector3 origin = transform.position + Vector3.up *1.55f; // approximate eye or ear height
		Vector3 toTarget = target.position - origin;
		float dist = toTarget.magnitude;
		if (dist <= 0f) return false;

		int occlusionMask = LayerMask.GetMask("Walls", "Doors");

		// draw the attempted LOS attempt
		Debug.DrawLine(origin, target.position, Color.cyan,0.05f);

		// If the ray hits any wall/door between origin and target, the sound or sight is occluded.
		if (Physics.Raycast(origin, toTarget, out RaycastHit hit, dist, occlusionMask, QueryTriggerInteraction.Collide))
		{
			// draw hit
			Debug.DrawLine(origin, hit.point, Color.red,0.1f);
			Debug.DrawLine(hit.point, target.position, Color.gray,0.1f);
			return true;
		}

		// no hit -> visible
		Debug.DrawLine(origin, target.position, Color.green,0.1f);
		return false;
	}
}