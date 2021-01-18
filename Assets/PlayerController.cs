using System;
using UnityEngine;
using static StateInTime;

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

		if (direction.normalized.magnitude > deadzone) 
		{

			transform.rotation = Quaternion.LookRotation(direction);

			rigidbody.AddForce(direction * moveSpeed * Time.deltaTime);

			LatestAction = StateInTime.ActionType.Walking;
			//transform.Translate(direction * moveSpeed * Time.deltaTime);
		} else {
			LatestAction = StateInTime.ActionType.Standing;
		}

		if (Input.GetKeyUp(KeyCode.Space) && !timeTracker.TimeTravelling)
		{
			LatestAction = StateInTime.ActionType.StartTimeTravel;
			timeTracker.StartTimeTravel(timeTravelAmount);
		}
	}
/*
	internal void ResetLatestAction() {
		LatestAction = ActionType.Undefined;
	}
*/
}