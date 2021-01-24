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

	GameObject pastPlayer = null;

	UIController UIController;

	public bool TimeTravelling { get; private set; } = false;

	private List<float> timeTravelAmounts = new List<float>();

	[SerializeField]
	private float snapshotRate = 0.1f;

	private void Awake() {
		momentsInTime = new MomentsInTime();

		UIController = GameObject.FindObjectOfType<UIController>();
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

		UIController.SetTimeText(time);

		if (!TimeTravelling) {
			return;
		}

		var stateInTime = momentsInTime.GetObjectInTime(time);
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
		var stateInTime = new ObjectInTime(TimeTracker.ObjectType.Player, GetTime(), new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z), playerTransform.rotation, l);
		//playerController.ResetLatestAction();
		momentsInTime.AddObjectInTime(stateInTime);
	}

	private float GetTime() {
		return Time.time - timeTravelAmounts.Sum();
	}

	internal void StartTimeTravel(float toPastInSeconds) 
	{

		var playerTransform = playerController.transform;

		//TODO: Could remove the latest state in time before adding the StartTimeTravel-action, to make the time travel recording more reliable.

		momentsInTime.AddObjectInTime(new ObjectInTime(TimeTracker.ObjectType.Player, GetTime(), new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z), playerTransform.rotation, ActionType.StartTimeTravel));

		timeTravelAmounts.Add(toPastInSeconds);

		TimeTravelling = true;

		pastPlayer = Instantiate(pastPlayerPrefab);
		
		//var recorder = playerController.GetComponent<TimeRecorder>();
	}

	public enum ObjectType {
		Player
	}
}
