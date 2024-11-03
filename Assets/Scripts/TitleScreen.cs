using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : Singleton<TitleScreen> {
	
	private const string FullScreenButtonName = "FullScreenButton";

	private const string StartButtonName = "StartGameButton";

	private const string FirstLevelSceneName = "Level1";
	
	private const string WizardGameObjectName = "Wizard";

	private GameObject page1GameObject;
	
	private TextMeshProUGUI fullScreenButtonText;

	private void Awake() {
		var fullScreenButton = GameObject.Find(FullScreenButtonName).GetComponent<Button>();
		fullScreenButton.onClick.AddListener(ToggleFullScreen);
		fullScreenButtonText = fullScreenButton.transform.GetComponentInChildren<TextMeshProUGUI>();
		
		var wizardGameObject = GameObject.Find(WizardGameObjectName);

		page1GameObject = wizardGameObject.transform.GetChild(0).gameObject;		
		
		GameObject.Find(StartButtonName).GetComponent<Button>().onClick.AddListener(LoadFirstLevelScene);
	}

	private void Start()
	{
		page1GameObject.SetActive(true);
	}
	
	private static void LoadFirstLevelScene()
	{
		SceneManager.LoadScene(FirstLevelSceneName);
	}
	
	private void Update() {
		UpdateFullScreenButtonText();
		//The player can press the WebGL player's full screen button at any time.
		//This is why I must update the full screen button's text every frame.
	}

	private static void ToggleFullScreen() {
		Screen.fullScreen = !Screen.fullScreen;
	}

	private void UpdateFullScreenButtonText()
	{
		fullScreenButtonText.text = Screen.fullScreen ? "Exit full screen" : "Go full screen";
	}
}
