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

	/// <summary>
	/// The time portals in the level. We want to add these at the beginning of the scene to ensure the references are found.
	/// </summary>
	List<TimePortal> timePortals = new List<TimePortal>();

	List<ButtonPedestal> buttonPedestals = new List<ButtonPedestal>();

	List<LargeDoor> largeDoors = new List<LargeDoor>();

	[SerializeField]
	private float snapshotRate = 0.017f;

	/// <summary>
	/// Choose the behaviour of the game when a time paradox occurs. Only need to change for testing purposes.
	/// </summary>
	[SerializeField]
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

		largeDoors = FindLargeDoors();
		buttonPedestals = FindButtonPedestals();
		timePortals = FindTimePortals();

		// Ensure all portals are enabled initially
		foreach (var portal in timePortals)
		{
			portal.Enable();
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

		if (!TimeTravelling) {
			return;
		}

		int numberOfPastPlayers = 0;

		UpdatePastPlayers(time, ref numberOfPastPlayers);

		UpdateInanimateObjects(time);

		UpdateSecuritySystem(time);

		if (TimeTravelling && numberOfPastPlayers == 0)
		{
			TimeTravelling = false;
			ui.OnTimeTravelEnded();
			foreach (var portal in timePortals)
			{
				portal.Enable();
			}
		}
	}

	private void UpdatePastPlayers(float time, ref int numberOfPastPlayers)
	{
		for (int i = 1; i <= GetTimeTravelCount(); i++)
		{
			var stateInTime = momentsInTime.GetCharacter($"Player{i}", time);

			// Check for duplicate objects with the same name and warn if found.
			var pastPlayers = FindObjectsByType<PastPlayer>(FindObjectsSortMode.None).Where(g => g.name == $"Player{i}").ToArray();
			if (pastPlayers.Length >1) {
				Debug.LogWarning($"Found {pastPlayers.Length} GameObjects with name Player{i}. This may cause unexpected behavior.");
			}

			GameObject pastPlayer = null;
			if (pastPlayers.Length >0) {
				pastPlayer = pastPlayers[0].gameObject;
			}

			if (stateInTime != null && pastPlayer != null)
			{
				pastPlayer.transform.position = stateInTime.Position;
				pastPlayer.transform.rotation = stateInTime.Rotation;

				if (ActionType.StartTimeTravel.Equals(stateInTime.Action))
				{
					Debug.Log($"The latest action is StartTimeTravel for Player{i}");
				}

				numberOfPastPlayers++;
			}
			else if (pastPlayer != null && stateInTime == null)
			{
				Debug.LogWarning($"Given state in time has no player recorded. Destroying that player. Player{i}");
				Destroy(pastPlayer);
			}
			else if (pastPlayer == null && stateInTime != null)
			{
				Debug.LogWarning($"Found a state in time with no player instantiated. Instantiating Player{i}");
				InstantiatePastPlayer(i, stateInTime.Position, stateInTime.Rotation);
			}
		}
	}

	/// <summary>
	/// Update inanimate objects like doors and buttons to their past states. Note: it is recommended to call this only during time travel.
	/// Calling it outside of time travel should be safe but unnecessary.
	/// </summary>
	/// <param name="time">In what time do you wish to update the objects' states to</param>
	/// <exception cref="Exception"></exception>
	private void UpdateInanimateObjects(float time)
	{
		foreach (var largeDoor in largeDoors)
		{
			var stateInTime = momentsInTime.GetObject<DoorObjectInTime>(largeDoor.name, time);

			if (stateInTime != null && largeDoor != null)
			{
				// Wrapping my head around this logic has been difficult.
				// Some of these helper variables are redundant.

				bool wasOpenInTimeStateRecords = stateInTime.IsOpen;
				bool wasClosedInTimeStateRecords = !stateInTime.IsOpen;

				bool isOpenedNowByPastAction = largeDoor.IsOpenByPastAction();

				bool isClosedNowByPastAction = !largeDoor.IsOpenByPastAction();

				// Note: I do not make any checks of largeDoor.IsOpenByPresentAction() here.
				// This class does not deal with opening things by present action.
				// The Player interacts with objects like ButtonPedestal and PressurePlate.
				// They call LargeDoor::OpenByPresentAction

				// if (wasOpenInTimeStateRecords && isOpenedNowByPastAction) ...
				// No effect.
				if (wasOpenInTimeStateRecords && isClosedNowByPastAction)
				{
					largeDoor.OpenByPastAction();
				}
				else if (wasClosedInTimeStateRecords && isOpenedNowByPastAction)
				{
					largeDoor.CloseByPastAction();
				}
				// else if (wasClosedInTimeStateRecords && isClosedNowByPastAction) ...
				// No effect.
			}
		}

		foreach (var buttonPedestal in buttonPedestals)
		{
			if (buttonPedestal != null && buttonPedestal.IsOneShot())
			{
				var buttonPedestalTime = momentsInTime.GetObject<ButtonPedestalInTime>(buttonPedestal.name, time);
				if (buttonPedestalTime != null && !buttonPedestalTime.IsInteractable)
				{
					buttonPedestal.ActivatedByPastPlayer();
				}
			}
		}
	}

	private void UpdateSecuritySystem(float time)
	{
		var security = FindFirstObjectByType<SecuritySystem>(FindObjectsInactive.Include);

		if (security != null)
		{
			var past = momentsInTime.GetObject<SecuritySystemInTime>(time);

			if (past != null)
			{
				security.AlarmByPastAction = past.Alarm;
			}
		}
	}

	internal void ResetTimeline() {
		throw new NotImplementedException();
	}

	[Obsolete("Deprecated, use TimeParadox(TimeParadoxCause, Transform) instead")]
	internal void TimeParadox(TimeParadoxCause eventThatCausedTimeParadox) {
		TimeParadox(eventThatCausedTimeParadox, FindAnyObjectByType<PastPlayer>().transform);
	}

	internal void TimeParadox(TimeParadoxCause eventThatCausedTimeParadox, Transform entityThatCausedTimeParadox) {
		if (ongoingTimeParadox) {
			return;
		}

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

				ongoingTimeParadox = true;

				tutorial.OnTimeParadox();

				CauseOfTimeParadox = eventThatCausedTimeParadox;
				EntityThatCausedTimeParadox = entityThatCausedTimeParadox;

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
		SnapshotButtonPedestals();
	}

	private void SnapshotButtonPedestals() {
		foreach (var buttonPedestal in buttonPedestals) {
			var stateInTime = new ButtonPedestalInTime(buttonPedestal.gameObject.name, GetTime(), buttonPedestal.IsInteractable());
			momentsInTime.AddObject(stateInTime);
		}
	}

	private static List<ButtonPedestal> FindButtonPedestals() {
		return FindObjectsByType<ButtonPedestal>(FindObjectsSortMode.None).ToList().FindAll(e => e.IsOneShot());
	}

	private static List<TimePortal> FindTimePortals()
	{
		return FindObjectsByType<TimePortal>(FindObjectsSortMode.None).ToList();
	}

	private static List<LargeDoor> FindLargeDoors()
	{
		return FindObjectsByType<LargeDoor>(FindObjectsSortMode.None).ToList();
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

	private void StartTimeTravel(float toPastInSeconds) 
	{
		timeTravelAmounts.Add(toPastInSeconds);

		TimeTravelling = true;

		//The current time is different now that the player has started to time travel.
		float currentTime = GetTime();

		for (int i =1; i <= GetTimeTravelCount(); i++) {
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

	private void PrepareToStartTimeTravel()
	{
		var playerTransform = playerController.transform;

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

		var pastPlayers = FindObjectsByType<PastPlayer>(FindObjectsSortMode.None);
		foreach (var pp in pastPlayers)
		{
			if (pp != null)
			{
				Destroy(pp.gameObject);
			}
		}
	}

	internal void StartTimeTravelToBeginning() {
		tutorial.OnTimeMachineActivated();

		PrepareToStartTimeTravel();
		StartTimeTravel(GetTime());

		ui.OnTimeTravelStarted();
	}
}
