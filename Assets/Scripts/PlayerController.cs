using System;
using UnityEngine;
using static ObjectInTime;

public class PlayerController : MonoBehaviour
{

	new Rigidbody rigidbody;
	TimeTracker timeTracker;

	[SerializeField]
	private float deadzone = 0.1f;

	[SerializeField]
	private float moveSpeed = 1000f;

	[SerializeField]
	private float timeTravelAmount = 3f;

	[SerializeField]
	private float lookTowardsRotationModifier = 250f;

	Quaternion lastLookDirection;

	internal ActionType LatestAction { get; set; } 
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
		timeTracker = GameObject.Find("TimeTracker").GetComponent<TimeTracker>();
	}
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		float v = Input.GetAxisRaw("Vertical");
		float h = Input.GetAxisRaw("Horizontal");

		Vector3 direction = new Vector3(h, 0f, v).normalized;
		var lookRotation = Quaternion.LookRotation(direction);

		transform.rotation = Quaternion.RotateTowards(transform.rotation, lastLookDirection == null ? lookRotation : lastLookDirection, Time.deltaTime * lookTowardsRotationModifier);

		if (direction.normalized.magnitude > deadzone) 
		{
			lastLookDirection = lookRotation;

			rigidbody.AddForce(direction * moveSpeed * Time.deltaTime);

			LatestAction = ObjectInTime.ActionType.Walking;
			//transform.Translate(direction * moveSpeed * Time.deltaTime);
		} else {
			LatestAction = ObjectInTime.ActionType.Standing;
		}

		if (Input.GetKeyUp(KeyCode.Space))
		{
			LatestAction = ObjectInTime.ActionType.StartTimeTravel;
			timeTracker.StartTimeTravelToBeginning();
		}
	}
/*
	internal void ResetLatestAction() {
		LatestAction = ActionType.Undefined;
	}
*/
}
