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

		for (int i = 1; i <= GetTimeTravelCount(); i++) {
			var stateInTime = momentsInTime.GetObject($"Player{i}", time);

			var pastPlayer = GameObject.Find($"Player{i}");

			Debug.Log(stateInTime);
			if (stateInTime != null && pastPlayer != null) {
				pastPlayer.transform.position = stateInTime.Position;
				pastPlayer.transform.rotation = stateInTime.Rotation;

				if (stateInTime.Action.Equals(ActionType.StartTimeTravel)) {
					Destroy(pastPlayer);
				}
				/*
				switch (stateInTime.Action) {
					case StateInTime.ActionType.StartTimeTravel:
						Destroy(pastPlayer);
						TimeTravelling = false;
						break;
				}*/
			} else if (pastPlayer != null && stateInTime == null) {
				Destroy(pastPlayer);
			} else if (pastPlayer == null && stateInTime != null) {
				Debug.LogWarning($"Found a state in time with no player instantiated. Instantiating Player{i}");
				InstantiatePastPlayer(i, stateInTime.Position, stateInTime.Rotation);
			}
		}
	}

	internal void TimeParadox() {
		throw new NotImplementedException();
	}

	private void TimeUpdate() {
		var playerTransform = playerController.transform;
		var l = (ActionType)(int)playerController.LatestAction;
		var stateInTime = new ObjectInTime($"Player{GetTimeTravelCount() + 1}", ObjectType.Player, GetTime(), new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z), playerTransform.rotation, l);
		//playerController.ResetLatestAction();
		momentsInTime.AddObject(stateInTime);
	}

	private float GetTime() {
		return Time.time - timeTravelAmounts.Sum();
	}
	private int GetTimeTravelCount() {
		return timeTravelAmounts.Count;
	}

	internal void StartTimeTravel(float toPastInSeconds) 
	{

		var playerTransform = playerController.transform;

		//TODO: Could remove the latest state in time before adding the StartTimeTravel-action, to make the time travel recording more reliable.

		//I want the time that gets stored here to be the moment in time when the player started time travel.
		momentsInTime.AddObject(
			new ObjectInTime(
				$"Player{GetTimeTravelCount() + 1}", 
				ObjectType.Player, 
				GetTime(), 
				new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z), 
				new Quaternion(playerTransform.rotation.x, playerTransform.rotation.y, playerTransform.rotation.z, playerTransform.rotation.w),
				ActionType.StartTimeTravel
				)
			);

		timeTravelAmounts.Add(toPastInSeconds);

		TimeTravelling = true;

		//The current time is different now that the player has started to time travel.
		float currentTime = GetTime();

		for (int i = 1; i <= GetTimeTravelCount(); i++) {
			var state = momentsInTime.GetObject($"Player{i}", currentTime);

			InstantiatePastPlayer(i, state.Position, state.Rotation);
		}
		//var recorder = playerController.GetComponent<TimeRecorder>();
	}

	private void InstantiatePastPlayer(int playerId, Vector3 position, Quaternion rotation) {
		var pastPlayer = Instantiate(pastPlayerPrefab, position, rotation);
		pastPlayer.name = $"Player{playerId}";
	}

	internal void StartTimeTravelToBeginning() {
		StartTimeTravel(GetTime());
	}
}
