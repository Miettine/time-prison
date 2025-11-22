using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using static CharacterInTime;
using static LargeDoor;

public class TimeTravel : Singleton<TimeTravel> {
	private MomentsInTime momentsInTime;

	Player playerController;
	GameObject pastPlayerPrefab;

	UI ui;
	Game game;
	Tutorial tutorial;

	public bool TimeTravelling { get; private set; } = false;

	private List<float> timeTravelAmounts = new List<float>();

	private List<string> trackedInanimateObjectNames = new List<string>();

	[SerializeField]
	private float snapshotRate = 0.1f;

	/// <summary>
	/// Choose the behaviour of the game when a time paradox occurs. Only need to change for testing purposes.
	/// </summary>
	private TimeParadoxBehaviour behaviourOnTimeParadox = TimeParadoxBehaviour.TimeParadoxAnimation;

	CameraControl cameraControl;
	private bool ongoingTimeParadox;

	public TimeParadoxCause CauseOfTimeParadox { get; private set; } = TimeParadoxCause.None;
	public Transform EntityThatCausedTimeParadox { get; private set; }

	private enum TimeParadoxBehaviour { NoEffect, DebugLog, ReloadScene, TimeParadoxAnimation }

	private void Awake() {
		momentsInTime = new MomentsInTime();

		ui = UI.GetInstance();
		playerController = Player.GetInstance();
		pastPlayerPrefab = (GameObject)Resources.Load("PastPlayer");
		cameraControl = CameraControl.GetInstance();
		game = Game.GetInstance();
	}

	// Start is called before the first frame update
	void Start() {
		tutorial = Tutorial.GetOrCreateInstance();

		ongoingTimeParadox = false;

		InvokeRepeating("TakeSnapshot", snapshotRate, snapshotRate);

		var largeDoors = GameObject.FindObjectsByType<LargeDoor>(FindObjectsSortMode.None);

		foreach (var largeDoor in largeDoors) {
			trackedInanimateObjectNames.Add(largeDoor.name);
		}

		List<ButtonPedestal> oneTimeButtonPedestals = FindOneShotButtonPedestals();

		foreach (var oneTimeButtonPedestal in oneTimeButtonPedestals) {
			trackedInanimateObjectNames.Add(oneTimeButtonPedestal.name);
		}
	}

	internal bool IsTimeParadoxOngoing() {
		return ongoingTimeParadox;
	}

	// Update is called once per frame
	void Update()
	{
		if (ongoingTimeParadox) {
			return;
		}

		float time = GetTime();

		ui.SetTimeText(time);

		int numberOfPastPlayers = 0;

		for (int i = 1; i <= GetTimeTravelCount(); i++) {
			var stateInTime = momentsInTime.GetObject<CharacterInTime>($"Player{i}", time);

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
		foreach (var name in trackedInanimateObjectNames) {

			var inanimateGameObject = GameObject.Find(name);

			if (inanimateGameObject == null) {
				throw new Exception($"Did not find an inanimate object with name {name}");
			}

			var stateInTime = momentsInTime.GetObject<DoorObjectInTime>(name, time);
			var largeDoor = inanimateGameObject.GetComponent<LargeDoor>();

			if (stateInTime != null && largeDoor != null) {
				//Wrapping my head around this logic has been difficult.
				//Some of these if-clauses and helper variables are redundant.

				bool wasOpenInTimeStateRecords = stateInTime.IsOpen;
				bool wasClosedInTimeStateRecords = !stateInTime.IsOpen;

				bool isOpenedNowByPastAction = largeDoor.IsOpenByPastAction();
				
				bool isClosedNowByPastAction = !largeDoor.IsOpenByPastAction();

				//Note: I do not make any checks of largeDoor.IsOpenByPresentAction() here.
				//This class does not deal with opening things by present action.
				//The Player interacts with objects like ButtonPedestal and PressurePlate.
				//They call LargeDoor::OpenByPresentAction

				/* 
				if (wasOpenInTimeStateRecords && isOpenedNowByPastAction) {
					//No effect.
				} else */ if (wasOpenInTimeStateRecords && isClosedNowByPastAction) {
					largeDoor.OpenByPastAction();
				} else if (wasClosedInTimeStateRecords && isOpenedNowByPastAction) {
					largeDoor.CloseByPastAction();
				} /* else if (wasClosedInTimeStateRecords && isClosedNowByPastAction) {
					//No effect.
				}
				*/
				continue;
			}

			var buttonPedestal = inanimateGameObject.GetComponent<ButtonPedestal>();

			if (buttonPedestal != null && buttonPedestal.IsOneShot()) {
				var buttonPedestalTime = momentsInTime.GetObject<ButtonPedestalInTime>(name, time);
				if (buttonPedestalTime != null && !buttonPedestalTime.IsInteractable) {
					buttonPedestal.ActivatedByPastPlayer();
				}
			}
		}
		var security = FindFirstObjectByType<SecuritySystem>(FindObjectsInactive.Include);

		if (security != null) {
			var past = momentsInTime.GetObject<SecuritySystemInTime>(time);
			Debug.Log(past);
			if (past != null) {
				security.AlarmByPastAction = past.Alarm;
			}
		}
	}

	internal void ResetTimeline() {
		throw new NotImplementedException();
	}

	[Obsolete("Deprecated, use TimeParadox(TimeParadoxCause, Transform) instead")]
	internal void TimeParadox(TimeParadoxCause eventThatCausedTimeParadox) {
		TimeParadox(eventThatCausedTimeParadox, FindObjectOfType<PastPlayer>().transform);
	}

	internal void TimeParadox(TimeParadoxCause eventThatCausedTimeParadox, Transform entityThatCausedTimeParadox) {
		if (ongoingTimeParadox) {
			return;
		}
		ongoingTimeParadox = true;

		tutorial.OnTimeParadox();

		CauseOfTimeParadox = eventThatCausedTimeParadox;
		EntityThatCausedTimeParadox = entityThatCausedTimeParadox;

		switch (behaviourOnTimeParadox) {
			case TimeParadoxBehaviour.NoEffect:
				break;
			case TimeParadoxBehaviour.DebugLog:
				DebugLogTimeParadox(CauseOfTimeParadox);
				break;
			case TimeParadoxBehaviour.ReloadScene:
				DebugLogTimeParadox(CauseOfTimeParadox);
				game.ReloadCurrentLevel();
				break;
			case TimeParadoxBehaviour.TimeParadoxAnimation:
				DebugLogTimeParadox(CauseOfTimeParadox);
				StartTimeParadoxAnimation();
				break;
		};
	}

	void StartTimeParadoxAnimation() {
		StopCoroutine("TakeSnapshot");

		cameraControl.OnTimeParadox();
		ui.OnTimeParadox();

		StartCoroutine(ReloadLevelAfterTimeParadoxAnimation());
	}

	private IEnumerator ReloadLevelAfterTimeParadoxAnimation() {
		yield return new WaitForSeconds(ui.GetTimeParadoxAnimationLength());
		game.ReloadCurrentLevel();
	}

	void DebugLogTimeParadox(TimeParadoxCause cause) {
		Debug.Log($"Time paradox! Cause: {cause}");
	}

	private void TakeSnapshot() {
		SnapshotPlayer();
		SnapshotDoors();
		
		SnapshotSecuritySystem();
		SnapshotOneTimeButtons();
	}

	private void SnapshotOneTimeButtons() {
		List<ButtonPedestal> oneTimeButtonPedestals = FindOneShotButtonPedestals();
		foreach (var buttonPedestal in oneTimeButtonPedestals) {
			var stateInTime = new ButtonPedestalInTime(buttonPedestal.gameObject.name, GetTime(), buttonPedestal.IsInteractable());
			momentsInTime.AddObject(stateInTime);
		}
	}

	private static List<ButtonPedestal> FindOneShotButtonPedestals() {
		return GameObject.FindObjectsByType<ButtonPedestal>(FindObjectsSortMode.None).ToList().FindAll(e => e.IsOneShot());
	}

	private void SnapshotSecuritySystem()
	{
		var security = FindFirstObjectByType<SecuritySystem>(FindObjectsInactive.Include);

		if (security != null) {
			var stateInTime = new SecuritySystemInTime(security.gameObject.name, GetTime(), security.AlarmByPresentAction);

			momentsInTime.AddObject(stateInTime);
		}
	}

	private void SnapshotDoors() {
		var largeDoors = FindObjectsByType<LargeDoor>(FindObjectsSortMode.None);
		
		foreach (var largeDoor in largeDoors) {
			var stateInTime = new DoorObjectInTime(largeDoor.gameObject.name, GetTime(), InanimateObjectType.LargeDoor, largeDoor.IsOpenByPresentAction());
			momentsInTime.AddObject(stateInTime);
		}
	}

	private void SnapshotPlayer() {
		var playerTransform = playerController.transform;
		var l = (ActionType)(int)playerController.LatestAction;

		var stateInTime = new CharacterInTime($"Player{GetTimeTravelCount() + 1}", GetTime(), CharacterType.Player, new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z), playerTransform.rotation, l);
		//playerController.ResetLatestAction();
		momentsInTime.AddObject(stateInTime);
	}

	internal bool HasStateContradiction(string doorName, LargeDoor door, out DoorTimeTravelState doorTimeTravelState) {
		var objectPastState = momentsInTime.GetObject<DoorObjectInTime>(doorName, GetTime());

		bool openInPast = objectPastState.IsOpen;
		bool openInPresent = door.IsOpenByPresentAction();

		doorTimeTravelState = new DoorTimeTravelState {
			OpenInPast = openInPast,
			OpenInPresent = openInPresent
		};

		return openInPast != openInPresent;
	}

	public float GetTime() {
		return UnityEngine.Time.timeSinceLevelLoad - timeTravelAmounts.Sum();
	}
	public int GetTimeTravelCount() {
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
			var state = momentsInTime.GetObject<CharacterInTime>($"Player{i}", currentTime);

			InstantiatePastPlayer(i, state.Position, state.Rotation);
		}

		var securitySystem = FindFirstObjectByType<SecuritySystem>(FindObjectsInactive.Include);

		if (securitySystem != null) {
			securitySystem.OnTimeTravelStarted();
		}

		foreach (var buttonPedestal in FindObjectsByType<ButtonPedestal>(FindObjectsSortMode.None)) {
			buttonPedestal.OnTimeTravelStarted();
		}
	}

	private void InstantiatePastPlayer(int playerId, Vector3 position, Quaternion rotation) {
		var pastPlayer = Instantiate(pastPlayerPrefab, position, rotation);
		pastPlayer.name = $"Player{playerId}";
	}

	internal void StartTimeTravelToBeginning() {
		StartTimeTravel(GetTime());
	}
}
