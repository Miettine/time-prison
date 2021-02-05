using System;
using UnityEngine;
using static ObjectInTime;

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

	Quaternion lastLookDirection;

	internal ActionType LatestAction { get; set; }

	int interactableObjectsLayerMask;
	/*
	private ActionType lastRecordedAction = ActionType.Undefined;

	internal ActionType LatestAction { get {
			//if (lastRecordedAction == ActionType.StartTimeTravel)
			if (lastRecordedAction == ActionType.StartTimeTravel) {
				lastRecordedAction = ActionType.Undefined;
				return ActionType.StartTimeTravel;
			}
			return lastRecordedAction;
		} 
		private set {
			
			if (lastRecordedAction == ActionType.StartTimeTravel && value != ActionType.StartTimeTravel) {
				lastRecordedAction = ActionType.StartTimeTravel;
			} else {
				lastRecordedAction = value;
			}
		} 
	}*/
	//internal float TimeTravelAmount { get => timeTravelAmount; }

	private void Awake() {
		rigidbody = GetComponent<Rigidbody>();
		timeTracker = GameObject.Find("TimeTracker").GetComponent<TimeTravel>();
		interactableObjectsLayerMask = LayerMask.GetMask("Interactable");
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
		
		if (Input.GetKeyUp(KeyCode.Space))
		{
			LatestAction = ObjectInTime.ActionType.StartTimeTravel;
			timeTracker.StartTimeTravelToBeginning();
		}
		LookForInteractableObjects();
	}
	
	private void LookForInteractableObjects() {
		var interactableObjectsColliders = Physics.OverlapSphere(transform.position, 1f, interactableObjectsLayerMask);

		
		if (interactableObjectsColliders.Length == 0) {
			return;
		}

		foreach (var collider in interactableObjectsColliders) {
			Debug.Log(collider);

			var buttonPedestal = collider.gameObject.GetComponentInParent<ButtonPedestal>();

			if (buttonPedestal != null) {
				buttonPedestal.Interact();
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

			LatestAction = ObjectInTime.ActionType.Walking;
			//transform.Translate(direction * moveSpeed * Time.deltaTime);
		} else {
			LatestAction = ObjectInTime.ActionType.Standing;
		}
	}
/*
	internal void ResetLatestAction() {
		LatestAction = ActionType.Undefined;
	}
*/
}
