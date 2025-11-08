using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : Singleton<UI>
{
	private Player player;
	private CameraControl cameraControl;
	private TimeTravel timeTravel;

	private Text timeText;
	private Text doorOpenText;

	private Button timeTravelButton;
	private Button resetButton;

	private GameObject timeTravelHelpText;
	private GameObject resetHelpText;
	 
	private GameObject blueKeyCardIndicator;
	private GameObject greenKeyCardIndicator;
	private GameObject yellowKeyCardIndicator;
	 
	private GameObject timeParadoxTextGameObject;
 
	private TextMeshProUGUI centerImportantNotificationText;
	private TextMeshProUGUI centerNeutralNotificationText;

	[SerializeField] private float timeParadoxAnimationLength = 10f;

	// New fields for the 'Press' UI element
	private Canvas canvas;
	private Text pressText;
	private RectTransform pressTextRect;

	void Awake()
    {
        cameraControl = CameraControl.GetInstance();
        timeTravel = TimeTravel.GetInstance();

        timeText = GameObject.Find("TimeText").GetComponent<Text>();
        doorOpenText = GameObject.Find("DoorOpenText").GetComponent<Text>();

        blueKeyCardIndicator = GameObject.Find("BlueKeyCardIndicator");
        greenKeyCardIndicator = GameObject.Find("GreenKeyCardIndicator");
        yellowKeyCardIndicator = GameObject.Find("YellowKeyCardIndicator");

        timeTravelButton = GameObject.Find("TimeTravelButton").GetComponent<Button>();
        resetButton = GameObject.Find("ResetButton").GetComponent<Button>();

        timeTravelHelpText = GameObject.Find("TimeTravelHelpText");
        resetHelpText = GameObject.Find("ResetHelpText");

        timeParadoxTextGameObject = GameObject.Find("TimeParadoxTextGameObject");

        centerImportantNotificationText = GameObject.Find("CenterImportantNotificationTextGameObject").GetComponent<TextMeshProUGUI>();
        centerImportantNotificationText.gameObject.SetActive(false);

        centerNeutralNotificationText = GameObject.Find("CenterNeutralNotificationTextGameObject").GetComponent<TextMeshProUGUI>();
        centerNeutralNotificationText.gameObject.SetActive(false);

        InitPressButtonPromptPlaceholder();

        player = Player.GetInstance();

        timeTravelButton.onClick.AddListener(() => player.OnTimeTravelActivated());
        resetButton.onClick.AddListener(() => player.OnResetActivated());

        blueKeyCardIndicator.SetActive(false);
        greenKeyCardIndicator.SetActive(false);
        yellowKeyCardIndicator.SetActive(false);

        doorOpenText.text = "";
    }

    /// <summary>
    /// Creates a placeholder 'Press' button prompt UI element. This functionality is placeholder, improve code in the future!
    /// </summary>
    private void InitPressButtonPromptPlaceholder()
    {
        canvas = FindAnyObjectByType<Canvas>();
		if (canvas == null)
		{
			throw new Exception("No Canvas found in the scene. Please add a Canvas to the scene for the UI to work.");
        }
        // Create a Text object for the 'Press' label
        GameObject pressGO = new GameObject("PressText");
        pressGO.transform.SetParent(canvas.transform, false);
        pressText = pressGO.AddComponent<Text>();
        pressText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        pressText.text = "Press E";
        pressText.alignment = TextAnchor.LowerCenter;
        pressText.fontSize = 24;
        pressText.color = Color.white;
        pressText.raycastTarget = false;
        pressTextRect = pressText.GetComponent<RectTransform>();
        pressTextRect.sizeDelta = new Vector2(120f, 30f);
		pressGO.SetActive(false);
    }

    void Start() {
		timeParadoxTextGameObject.SetActive(false);
		SetControlMode(ControlMode.Touch);
	}

	private void SetControlMode(ControlMode controlMode) {
		bool touchControlsEnabled = controlMode == ControlMode.Touch;

		timeTravelButton.gameObject.SetActive(touchControlsEnabled);
		resetButton.gameObject.SetActive(touchControlsEnabled);

		timeTravelHelpText.SetActive(!touchControlsEnabled);
		resetHelpText.SetActive(!touchControlsEnabled);
	}

	public enum ControlMode {
		Touch,
		Keyboard
	}

	public void SetTimeText(float time) {
		timeText.text = time.ToString("Time: 0");
	}

	internal void ShowDoorOpenPermanentNotification(string notificationText) {
		doorOpenText.text = notificationText;
	}

	internal void ShowDoorOpenPermanentNotification() {
		ShowDoorOpenPermanentNotification("Door open");
	}

	internal void ShowDoorClosed() {
		StartCoroutine(ShowNotification("Door closed"));
	}

	internal void ShowDoorOpenForSeconds(float openForTime) {
		StopAllCoroutines();
		doorOpenText.text = "";

		ShowDoorOpenText(openForTime);

		StartCoroutine(CountDownShowDoorOpen(openForTime));
	}

	internal void OnKeyboardInputUsed() {
		SetControlMode(ControlMode.Keyboard);
	}

	internal void OnMouseInputUsed() {
		SetControlMode(ControlMode.Touch);
	}

	void ShowDoorOpenText(float seconds) {
		doorOpenText.text = Math.Ceiling(seconds).ToString($"Door open for 0 seconds");
	}

	IEnumerator CountDownShowDoorOpen(float wait) {
		while (wait > 0) {
			wait -= Time.deltaTime;
			ShowDoorOpenText(wait);
			yield return null;
		}
		ShowDoorClosed();
	}

	internal void ShowAlarm() {
		StartCoroutine(ShowNotification("Alarm triggered. Lockdown in effect."));
	}

	IEnumerator ShowNotification(string text) {
		doorOpenText.text = text;
		yield return new WaitForSeconds(1.5f);
		doorOpenText.text = "";
	}

	private TextMeshProUGUI GetTextObject(NotificationType type)
	{
		return type == NotificationType.Important ? centerImportantNotificationText : centerNeutralNotificationText;
	}
	
	/// <summary>
	/// Get the other center notification text that is NOT the one that the given type parameter asks for.
	/// </summary>
	/// <param name="type">The other text that is NOT of this type</param>
	/// <returns>The text object</returns>
	private TextMeshProUGUI GetTextObjectNot(NotificationType type)
	{
		return type == NotificationType.Important ? centerNeutralNotificationText : centerImportantNotificationText;
	}

	/// <summary>
	/// This notification is shown until the level ends.
	/// </summary>
	/// <param name="text">The text to show</param>
	/// <param name="type">Whether to show an important kind of notification in yellow text or a more neutral
	/// notification in white text.</param>
	internal void ShowPermanentCenterNotificationText(string text, NotificationType type = NotificationType.Neutral) {
		ShowCenterNotificationTextHideOther(text, type);
	}

	private TextMeshProUGUI ShowCenterNotificationTextHideOther(string text, NotificationType type = NotificationType.Neutral) {
		var textObject = GetTextObject(type);

		textObject.gameObject.SetActive(true);
		textObject.text = text;
		
		var otherTextObject = GetTextObjectNot(type);

		otherTextObject.gameObject.SetActive(false);
		otherTextObject.text = "";
		
		return textObject;
	}

	internal void HideCenterNotificationTexts()
	{
		centerImportantNotificationText.gameObject.SetActive(false);
		centerNeutralNotificationText.gameObject.SetActive(false);
	}

	internal void ShowTemporaryCenterNotificationText(string text, NotificationType type = NotificationType.Neutral, float time = 3f) {
		StartCoroutine(ShowTemporaryCenterNotificationTextAsync(text, type , time));
	}

	private IEnumerator ShowTemporaryCenterNotificationTextAsync(string text, NotificationType type = NotificationType.Neutral, float time = 3f) {
		var textObject = ShowCenterNotificationTextHideOther(text, type);

		yield return WaitForSeconds(time);

		textObject.gameObject.SetActive(false);
		textObject.text = "";
	}
	
	internal void ShowKeyCardIndicator(KeyCardType type, bool visible) {
		switch (type) {
			case KeyCardType.Blue:
				blueKeyCardIndicator.SetActive(visible);
				break;
			case KeyCardType.Green:
				greenKeyCardIndicator.SetActive(visible);
				break;
			case KeyCardType.Yellow:
				yellowKeyCardIndicator.SetActive(visible);
				break;
		}
	}

	/// <summary>
	/// In order for the player to have time to read all of the messages, it it requires 
	/// a waiting time of timeParadoxAnimationStepDelay*4 for the last effect to finish playing.
	/// after which the level may be reset.
	/// </summary>
	/// <param name="timeParadoxCause"></param>
	internal void OnTimeParadoxAnimationStart() {
		StartCoroutine(TimeParadoxAnimation());
	}

	/// <summary>
	/// Currently the time paradox animation has about 5 pauses 
	/// where we display some text to the player, move the camera around
	/// or show some graphical effects to the player.
	/// If the amount of pauses ever changes, remember to update this function
	/// accordingly so that the player will have enough time to see everything
	/// we want them to see during the time paradox animation.
	/// </summary>
	/// <returns></returns>
	public float GetTimeParadoxAnimationStepDelay() {
		return GetTimeParadoxAnimationLength() / 5;
	}

	public float GetTimeParadoxAnimationLength() {
		return timeParadoxAnimationLength;
	}

	IEnumerator TimeParadoxAnimation() {

		timeParadoxTextGameObject.SetActive(true);

		yield return WaitForTimeParadoxAnimationStepDelay();

		timeParadoxTextGameObject.SetActive(false);

		switch (timeTravel.CauseOfTimeParadox) {
			case TimeParadoxCause.PastPlayerSawPresentPlayer:
				ShowCenterNotificationTextHideOther("Cause:\npast self saw you", NotificationType.Important);
				break;
			case TimeParadoxCause.PastPlayerHeardPresentPlayer:
				ShowCenterNotificationTextHideOther("Cause:\npast self heard you", NotificationType.Important);
				break;
			case TimeParadoxCause.PastPlayerSawObjectInteractionFromPresentPlayer:
				ShowCenterNotificationTextHideOther("Cause:\npast self saw an object that you had interacted with", NotificationType.Important);
				break;
		}

		yield return cameraControl.MoveToTarget(timeTravel.EntityThatCausedTimeParadox, GetTimeParadoxAnimationStepDelay());

		yield return WaitForTimeParadoxAnimationStepDelay();
		
		ShowCenterNotificationTextHideOther("Resetting timeline", NotificationType.Important);

		yield return cameraControl.MoveToTarget(player.transform, GetTimeParadoxAnimationStepDelay());
		
		cameraControl.StartTimelineResetAnimation(); //lasts one TimeParadoxAnimationStepDelay

		yield return WaitForSeconds(GetTimeParadoxAnimationStepDelay() / 6);
		//The camera warping effect and the text "Resetting timeline" are intentionally made to overlap for a moment.
		//This is to make the time paradox animation seem more visually interesting and organic.
		HideCenterNotificationTexts();
	}

	WaitForSeconds WaitForTimeParadoxAnimationStepDelay() {
		return WaitForSeconds(GetTimeParadoxAnimationStepDelay());
	}

	WaitForSeconds WaitForSeconds(float time) {
		return new WaitForSeconds(time);
	}

	/// <summary>
	/// Show a UI text that will be positioned on the Canvas where the specified world position is.
	/// Useful for showing small contextual hints above world objects.
	/// </summary>
	/// <param name="targetTransform">The world position to place the text above.</param>
	public void ShowPressTextAtWorldObject(Transform targetTransform)
	{
		// For ScreenSpaceOverlay canvases the camera parameter must be null
		Camera cam = Camera.main;

		pressTextRect.position = cam.WorldToScreenPoint(targetTransform.position);
		pressText.gameObject.SetActive(true);
	}

	public void HidePressText()
	{
		if (pressText != null) pressText.gameObject.SetActive(false);
	}
}
