using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Player;

public class UI : Singleton<UI>
{
	Player player;
	CameraControl cameraControl;
	TimeTravel timeTravel;

	Text timeText;
	Text doorOpenText;

	GameObject timeTravelingTextGameObject;

	Button timeTravelTouchButton;
	Button resetTouchButton;

	GameObject timeTravelKeyboardPromptTextGameObject;
	GameObject resetPromptTextGameObject;
	
	GameObject blueKeyCardIndicator;
	GameObject greenKeyCardIndicator;
	GameObject yellowKeyCardIndicator;

	Image vignette;

	/// <summary>
	/// The vignette becomes this color when time travel starts.
	/// </summary>
	[SerializeField] Color timeTravelVignetteColor = new Color(0f, 0f, 52f / 255f, 121f / 255f);
	/// <summary>
	/// The vignette becomes this color when a time paradox occurs.
	/// </summary>
	[SerializeField] Color timeParadoxVignetteColor = new Color(145f / 255f, 40f / 255f, 0f, 175f / 255f);
	[SerializeField] float timeTravelVignetteFadeToTransparentDuration = 1.5f;
	[SerializeField] float timeTravelVignetteFadeToVisibleDuration = 0.1f;

	GameObject timeParadoxTextGameObject;

	PlatformSpecificText platformSpecificText;

	TextMeshProUGUI centerImportantNotificationText;
	TextMeshProUGUI centerNeutralNotificationText;

	ControlMode currentControlMode;

	[SerializeField] float timeParadoxAnimationLength = 10f;

	Canvas canvas;

	GameObject interactUIPromptPrefab;
	Coroutine vignetteFadeCoroutine;

	void Awake()
	{
		cameraControl = CameraControl.GetInstance();
		timeTravel = TimeTravel.GetInstance();

		canvas = FindAnyObjectByType<Canvas>();
		if (canvas == null)
		{
			throw new Exception("No Canvas found in the scene. Please add a Canvas to the scene for the UI to work.");
		}

		timeText = GameObject.Find("TimeText").GetComponent<Text>();
		doorOpenText = GameObject.Find("DoorOpenText").GetComponent<Text>();

		timeTravelingTextGameObject = GameObject.Find("TimeTravelingText");
		timeTravelingTextGameObject.SetActive(false);

		interactUIPromptPrefab = (GameObject) Resources.Load("InteractUIPrompt");

		if (interactUIPromptPrefab == null)
		{
			throw new Exception("Could not load InteractUIPrompt prefab from Resources folder. Please ensure it exists at Resources/InteractUIPrompt.prefab");
		}
		blueKeyCardIndicator = GameObject.Find("BlueKeyCardIndicator");
		greenKeyCardIndicator = GameObject.Find("GreenKeyCardIndicator");
		yellowKeyCardIndicator = GameObject.Find("YellowKeyCardIndicator");

		vignette = GameObject.Find("Vignette").GetComponent<Image>();

		timeTravelTouchButton = GameObject.Find("TimeTravelButton").GetComponent<Button>();
		resetTouchButton = GameObject.Find("ResetButton").GetComponent<Button>();

		timeTravelKeyboardPromptTextGameObject = GameObject.Find("TimeTravelKeyboardPromptText");
		resetPromptTextGameObject = GameObject.Find("ResetKeyboardPromptText");

		timeParadoxTextGameObject = GameObject.Find("TimeParadoxTextGameObject");

		centerImportantNotificationText = GameObject.Find("CenterImportantNotificationTextGameObject").GetComponent<TextMeshProUGUI>();
		centerImportantNotificationText.gameObject.SetActive(false);

		centerNeutralNotificationText = GameObject.Find("CenterNeutralNotificationTextGameObject").GetComponent<TextMeshProUGUI>();
		centerNeutralNotificationText.gameObject.SetActive(false);

		player = Player.GetInstance();
		platformSpecificText = PlatformSpecificText.GetInstance();

		timeTravelTouchButton.onClick.AddListener(() => player.OnPlayerWantsToActivateTimeMachine());
		resetTouchButton.onClick.AddListener(() => player.OnResetActivated());

		blueKeyCardIndicator.SetActive(false);
		greenKeyCardIndicator.SetActive(false);
		yellowKeyCardIndicator.SetActive(false);

		doorOpenText.text = "";
	}

	void Start() {
		// Ensure vignette is initially hidden and fully transparent
		vignette.enabled = false;
		var col = vignette.color;
		col.a = 0f;

		vignette.color = col;
 		timeParadoxTextGameObject.SetActive(false);
 		UpdateControlModeUI(ControlMode.Touch);
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

	internal void OnMouseInputUsed()
	{
		SetControlMode(ControlMode.Touch);
	}

	void SetControlMode(ControlMode controlMode)
	{
		this.currentControlMode = controlMode;
		UpdateControlModeUI(controlMode);
	}

	private void UpdateControlModeUI(ControlMode controlMode)
	{
		if (player.HasObtainedTimeMachine){ 
			var touchControlsEnabled = controlMode == ControlMode.Touch;

			ShowTouchControlButtons(touchControlsEnabled);
			ShowKeyboardControlPrompts(!touchControlsEnabled);
		} else {
			ShowTouchControlButtons(false);
			ShowKeyboardControlPrompts(false);
		}
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
	internal void OnTimeParadox() {

		timeText.gameObject.SetActive(false);
		doorOpenText.gameObject.SetActive(false);

		timeTravelingTextGameObject.SetActive(false);

		ShowTouchControlButtons(false);
		ShowKeyboardControlPrompts(false);

		blueKeyCardIndicator.SetActive(false);
		greenKeyCardIndicator.SetActive(false);
		yellowKeyCardIndicator.SetActive(false);

		timeParadoxTextGameObject.SetActive(false);

		centerImportantNotificationText.gameObject.SetActive(false);
		centerNeutralNotificationText.gameObject.SetActive(false);

		ShowVignette(timeParadoxVignetteColor);
		StartCoroutine(TimeParadoxAnimation());
	}

	void ShowTouchControlButtons(bool show) {
		timeTravelTouchButton.gameObject.SetActive(show);
		resetTouchButton.gameObject.SetActive(show);
	}

	void ShowKeyboardControlPrompts(bool show) {
		timeTravelKeyboardPromptTextGameObject.SetActive(show);
		resetPromptTextGameObject.SetActive(show);
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
			case TimeParadoxCause.PastPlayerSawDoorOpenedByPresentPlayer:
				ShowCenterNotificationTextHideOther("Cause:\npast self saw a door opened by you", NotificationType.Important);
				break;
			case TimeParadoxCause.PastPlayerSawDoorClosedByPresentPlayer:
				ShowCenterNotificationTextHideOther("Cause:\npast self saw a door closed by you", NotificationType.Important);
				break;
			case TimeParadoxCause.TouchedYourPastSelf:
				ShowCenterNotificationTextHideOther("Cause:\nyou touched your past self", NotificationType.Important);
				break;
			default:
				ShowCenterNotificationTextHideOther("Cause:\n404 cause not found", NotificationType.Important);
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

	internal InteractPrompt OnButtonPedestalCreated(ButtonPedestal buttonPedestal)
	{
		var promptGameObject = Instantiate(interactUIPromptPrefab, canvas.transform);
		var interactPrompt = promptGameObject.GetComponent<InteractPrompt>();

		interactPrompt.LinkedButtonPedestal = buttonPedestal;
		return interactPrompt;
	}

	internal void OnTimeMachineObtained()
	{
		SetControlMode(currentControlMode);
		platformSpecificText.ShowTimeMachineObtainedTutorialText(currentControlMode);
	}

	internal void OnTimeTravelStarted()
	{
		ShowVignette(timeTravelVignetteColor, timeTravelVignetteFadeToVisibleDuration);
		timeTravelingTextGameObject.SetActive(true);
		platformSpecificText.Hide(); // Might have to make this more elaborate later if there are more texts to show.
	}

	internal void OnTimeTravelEnded()
	{
		// Fade to a transparent version of the timeTravelVignetteColor
		var transparent = timeTravelVignetteColor;
		transparent.a = 0f;
		timeTravelingTextGameObject.SetActive(false);
		ShowVignette(transparent, timeTravelVignetteFadeToTransparentDuration);
	}

	private void ShowVignette(Color targetColor, float fadeDuration = 0.1f)
	{
		// Cancel any running fade and start a fade to target color
		if (vignetteFadeCoroutine != null)
		{
			StopCoroutine(vignetteFadeCoroutine);
			vignetteFadeCoroutine = null;
		}
		vignetteFadeCoroutine = StartCoroutine(FadeVignette(targetColor, fadeDuration));
	}

	// Reusable vignette fade coroutine: lerps color from current to target over duration
	private IEnumerator FadeVignette(Color targetColor, float duration)
	{
		// Ensure vignette is enabled so we can see the fade
		vignette.enabled = true;

		Color startColor = vignette.color;
		float elapsed = 0f;
		duration = Mathf.Max(0f, duration);

		// If duration is zero, set immediately
		if (duration <= 0f)
		{
			vignette.color = targetColor;
			if (Mathf.Approximately(targetColor.a, 0f)) vignette.enabled = false;
			vignetteFadeCoroutine = null;
			yield break;
		}

		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;
			float t = Mathf.Clamp01(elapsed / duration);
			var c = Color.Lerp(startColor, targetColor, t);
			vignette.color = c;
			yield return null;
		}

		var final = targetColor;
		vignette.color = final;
		if (Mathf.Approximately(targetColor.a, 0f))
		{
			vignette.enabled = false;
		}
		vignetteFadeCoroutine = null;
	}
}
