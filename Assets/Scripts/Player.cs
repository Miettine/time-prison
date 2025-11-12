using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static CharacterInTime;

public class Player : Singleton<Player>
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
	float sneakingRadius = 4f;

	[SerializeField]
	float sneakingSpeedMultiplier = 0.5f;

    [SerializeField]
    float interactionRadius = 2f;

    UI ui;

	Quaternion lastLookDirection;

	ISet<KeyCardType> keycards = new HashSet<KeyCardType>();

	GameObject soundIndicator;

	int pastPlayerLayer;

	internal ActionType LatestAction { get; set; }

	int interactableObjectsLayerMask;

    private ControlMode controlMode;

    public ControlMode _ControlMode
    {
        get => controlMode; private set
        {
            controlMode = value;
        }
    }

    /// <summary>
    /// An object that is within the player character's arms reach and can be interacted with. Null if there is no such object.
	/// Currently this can only be a ButtonPedestal. It would be better design to make this a more general interface like IInteractableObject.
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
		if (timeTravel.IsTimeParadoxOngoing()) {
			return;
		}

		if (Input.GetKeyUp(KeyCode.Space)) {
			LatestAction = CharacterInTime.ActionType.StartTimeTravel;
			OnTimeTravelActivated();
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

				var sneaking = Vector3.Distance(transform.position, groundPoint) < sneakingRadius;

                ProcessMovementInput(movementVector.normalized, sneaking);
				return;
			}
		} else if (Input.anyKey) {
			OnKeyboardInputUsed();

			float v = Input.GetAxisRaw("Vertical");
			float h = Input.GetAxisRaw("Horizontal");

			bool sneaking = Input.GetKey(KeyCode.C);

			ProcessMovementInput(new Vector3(h, 0f, v), sneaking);

            return;
		}
		//TODO: Add gamepad controls. Can we use gamepad in WebGL?
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
		FocusedInteractableObject.Interact();
    }

	void FindNearbyFocusedObject()
	{
        var interactableObjectsColliders = Physics.OverlapSphere(transform.position, interactionRadius, interactableObjectsLayerMask);

        if (interactableObjectsColliders.Length == 0)
        {
			FocusedInteractableObject = null;
            return;
        }

        foreach (var collider in interactableObjectsColliders)
        {
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
