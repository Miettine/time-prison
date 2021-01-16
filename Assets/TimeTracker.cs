using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static StateInTime;

public class TimeTracker : MonoBehaviour
{
	List<StateInTime> statesInTime = new List<StateInTime>();
	PlayerController playerController;
	GameObject pastPlayerPrefab;

	GameObject pastPlayer = null;

	public bool TimeTravelling { get; private set; } = false;

	private float timeTravelAmount;

	[SerializeField]
	private float snapshotRate = 0.1f;

	private void Awake() {
		playerController = GameObject.FindObjectOfType<PlayerController>();
		pastPlayerPrefab = (GameObject)Resources.Load("PastPlayer");
	}

	// Start is called before the first frame update
	void Start() {
		InvokeRepeating("TimeUpdate", snapshotRate, snapshotRate);
	}

	private void AddStateInTime(StateInTime stateInTime) {
		statesInTime.Add(stateInTime);
	}

	// Update is called once per frame
	void Update()
	{
		if (TimeTravelling) {
			var stateInTime = GetState(Time.time - timeTravelAmount);
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
	}

	private void TimeUpdate() {
		var playerTransform = playerController.transform;
		var l = (ActionType)(int)playerController.LatestAction;
		var stateInTime = new StateInTime(TimeTracker.ObjectInTime.Player, Time.time, new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z), playerTransform.rotation, l);
		//playerController.ResetLatestAction();
		AddStateInTime(stateInTime);
	}

	private object GetState(object p) {
		throw new NotImplementedException();
	}

	internal void StartTimeTravel(float toPastInSeconds) 
	{
		timeTravelAmount = toPastInSeconds;

		var playerTransform = playerController.transform;
		AddStateInTime(new StateInTime(TimeTracker.ObjectInTime.Player, Time.time, new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z), playerTransform.rotation, ActionType.StartTimeTravel));

		TimeTravelling = true;

		pastPlayer = Instantiate(pastPlayerPrefab);
		
		//var recorder = playerController.GetComponent<TimeRecorder>();


	}
	private StateInTime GetState(float time) {

		for (int i = 0; i < statesInTime.Capacity; i++) {
			var s = statesInTime[i];
			if (time <= s.Time) {
				return s;
			}
		}

		return null;
		//var matches = statesInTime.Find(stateInTime => stateInTime.Time >= time);
		//return matches;
	}

	public enum ObjectInTime {
		Player
	}
}
