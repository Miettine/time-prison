using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static CharacterInTime;

public class TimeTravel : MonoBehaviour
{
	private MomentsInTime momentsInTime;

	Player playerController;
	GameObject pastPlayerPrefab;

	UI ui;
	Game game;

	public bool TimeTravelling { get; private set; } = false;

	private List<float> timeTravelAmounts = new List<float>();

	[SerializeField]
	private float snapshotRate = 0.1f;

	[SerializeField]
	private TimeParadoxBehaviour behaviourOnTimeParadox = TimeParadoxBehaviour.ReloadScene;

	public enum TimeParadoxBehaviour { NoEffect, DebugLog, ReloadScene }

	private void Awake() {
		momentsInTime = new MomentsInTime();

		ui = GameObject.FindObjectOfType<UI>();
		playerController = GameObject.FindObjectOfType<Player>();
		pastPlayerPrefab = (GameObject)Resources.Load("PastPlayer");
		game = FindObjectOfType<Game>();
	}

	// Start is called before the first frame update
	void Start() {
		InvokeRepeating("TakeSnapshot", snapshotRate, snapshotRate);
	}

	// Update is called once per frame
	void Update()
	{
		float time = GetTime();

		ui.SetTimeText(time);

		int numberOfPastPlayers = 0;

		for (int i = 1; i <= GetTimeTravelCount(); i++) {
			var stateInTime = momentsInTime.GetCharacter($"Player{i}", time);

			var pastPlayer = GameObject.Find($"Player{i}");

			if (stateInTime != null && pastPlayer != null) {
				pastPlayer.transform.position = stateInTime.Position;
				pastPlayer.transform.rotation = stateInTime.Rotation;

				if (stateInTime.Action.Equals(ActionType.StartTimeTravel)) {
					Destroy(pastPlayer);
				}

				numberOfPastPlayers++;
			} else if (pastPlayer != null && stateInTime == null) {
				Debug.LogWarning($"Given state in time has no player recorded. Destroying Player{i}");
				Destroy(pastPlayer);
			} else if (pastPlayer == null && stateInTime != null) {
				Debug.LogWarning($"Found a state in time with no player instantiated. Instantiating Player{i}");
				InstantiatePastPlayer(i, stateInTime.Position, stateInTime.Rotation);
			}
		}
		if (numberOfPastPlayers == 0) {
			TimeTravelling = false;
		}
		{ 
			var stateInTime = momentsInTime.GetInanimateObject("LargeDoor", time);

			var largeDoor = GameObject.FindObjectOfType<LargeDoor>();

			if (stateInTime != null && largeDoor != null) {
				if (stateInTime.IsOpen && !largeDoor.IsOpenByPastAction()) {
					largeDoor.OpenByPastAction();
				} else if ( !stateInTime.IsOpen && largeDoor.IsOpenByPastAction() ) {
					largeDoor.CloseByPastAction();
				}
			}
		}
	}

	internal void TimeParadox() {
		switch (behaviourOnTimeParadox) {
			case TimeParadoxBehaviour.NoEffect:
				break;
			case TimeParadoxBehaviour.DebugLog:
				DebugLogTimeParadox();
				break;
			case TimeParadoxBehaviour.ReloadScene:
				DebugLogTimeParadox();
				game.ReloadCurrentLevel();
				break;
		};
	}

	void DebugLogTimeParadox() {
		Debug.Log("Time paradox!");
	}
	
	//internal void TimeParadox(Transform causeLocation, TimeParadoxReason reason) { }
	
	private void TakeSnapshot() {
		{
			var playerTransform = playerController.transform;
			var l = (ActionType)(int)playerController.LatestAction;

			var stateInTime = new CharacterInTime($"Player{GetTimeTravelCount() + 1}", GetTime(), CharacterType.Player,  new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z), playerTransform.rotation, l);
			//playerController.ResetLatestAction();
			momentsInTime.AddObject(stateInTime);
		}
		{
			var largeDoors = GameObject.FindObjectsOfType<LargeDoor>();
			foreach (var largeDoor in largeDoors) {
				var stateInTime = new InanimateDoorObjectInTime(largeDoor.gameObject.name, GetTime(), InanimateObjectType.LargeDoor, largeDoor.IsOpenByPresentAction());
				momentsInTime.AddObject(stateInTime);
			}
		}
	}

	internal bool HasStateContradiction(string doorName, LargeDoor door) {
		var objectPastState = momentsInTime.GetInanimateObject(doorName, GetTime());

		return objectPastState.IsOpen != door.IsOpenByPresentAction();
	}

	private float GetTime() {
		return UnityEngine.Time.time - timeTravelAmounts.Sum();
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
			new CharacterInTime(
				$"Player{GetTimeTravelCount() + 1}",
				GetTime(),
				CharacterType.Player, 
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
			var state = momentsInTime.GetCharacter($"Player{i}", currentTime);

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
