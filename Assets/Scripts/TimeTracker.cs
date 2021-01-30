using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static ObjectInTime;

public class TimeTracker : MonoBehaviour
{
	private MomentsInTime momentsInTime;

	PlayerController playerController;
	GameObject pastPlayerPrefab;

	UIController uiController;

	public bool TimeTravelling { get; private set; } = false;

	private List<float> timeTravelAmounts = new List<float>();

	[SerializeField]
	private float snapshotRate = 0.1f;

	private void Awake() {
		momentsInTime = new MomentsInTime();

		uiController = GameObject.FindObjectOfType<UIController>();
		playerController = GameObject.FindObjectOfType<PlayerController>();
		pastPlayerPrefab = (GameObject)Resources.Load("PastPlayer");
	}

	// Start is called before the first frame update
	void Start() {
		InvokeRepeating("TimeUpdate", snapshotRate, snapshotRate);
	}

	// Update is called once per frame
	void Update()
	{
		float time = GetTime();

		uiController.SetTimeText(time);

		if (!TimeTravelling) {
			return;
		}

		var stateInTime = momentsInTime.GetObject($"Player{timeTravelCount}", time);

		var pastPlayer = GameObject.Find($"Player{timeTravelCount}");

		Debug.Log(stateInTime);
		if (stateInTime != null) {
			pastPlayer.transform.position = stateInTime.Position;
			pastPlayer.transform.rotation = stateInTime.Rotation;

			if (stateInTime.Action.Equals(ActionType.StartTimeTravel)) {
				Destroy(pastPlayer);

				TimeTravelling = false;
			}
			/*
			switch (stateInTime.Action) {
				case StateInTime.ActionType.StartTimeTravel:
					Destroy(pastPlayer);
					TimeTravelling = false;
					break;
			}*/
		}
	}

	private void TimeUpdate() {
		var playerTransform = playerController.transform;
		var l = (ActionType)(int)playerController.LatestAction;
		var stateInTime = new ObjectInTime($"Player{timeTravelCount + 1}", ObjectType.Player, GetTime(), new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z), playerTransform.rotation, l);
		//playerController.ResetLatestAction();
		momentsInTime.AddObject(stateInTime);
	}

	private float GetTime() {
		return Time.time - timeTravelAmounts.Sum();
	}
	int timeTravelCount = 0;

	internal void StartTimeTravel(float toPastInSeconds) 
	{

		var playerTransform = playerController.transform;

		//TODO: Could remove the latest state in time before adding the StartTimeTravel-action, to make the time travel recording more reliable.

		//I want the time that gets stored here to be the moment in time when the player started time travel.
		momentsInTime.AddObject(
			new ObjectInTime(
				$"Player{timeTravelCount + 1}", 
				ObjectType.Player, 
				GetTime(), 
				new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z), 
				new Quaternion(playerTransform.rotation.x, playerTransform.rotation.y, playerTransform.rotation.z, playerTransform.rotation.w),
				ActionType.StartTimeTravel
				)
			);

		timeTravelAmounts.Add(toPastInSeconds);

		TimeTravelling = true;
		timeTravelCount++;
		//The current time is different now that the player has started to time travel.
		float currentTime = GetTime();

		var state = momentsInTime.GetObject($"Player{timeTravelCount}", currentTime);

		var pastPlayer = Instantiate(pastPlayerPrefab, state.Position, state.Rotation);
		pastPlayer.name = $"Player{timeTravelCount}";
		//var recorder = playerController.GetComponent<TimeRecorder>();
	}

	internal void StartTimeTravelToBeginning() {
		StartTimeTravel(GetTime());
	}
}
