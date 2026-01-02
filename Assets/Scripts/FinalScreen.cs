using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalScreen : Singleton<FinalScreen>
{
	private const string FullScreenButtonName = "FullScreenButton";

	private const string RestartButtonName = "RestartGameButton";

	private const string TitleScreenSceneName = "TitleScreen";

	private TextMeshProUGUI fullScreenButtonText;

	private void Awake()
	{
		var fullScreenButton = GameObject.Find(FullScreenButtonName).GetComponent<Button>();
		fullScreenButton.onClick.AddListener(ToggleFullScreen);
		fullScreenButtonText = fullScreenButton.transform.GetComponentInChildren<TextMeshProUGUI>();

		GameObject.Find(RestartButtonName).GetComponent<Button>().onClick.AddListener(LoadTitleScreen);
	}

	private static void LoadTitleScreen()
	{
		SceneManager.LoadScene(TitleScreenSceneName);
	}

	private void Update()
	{
		UpdateFullScreenButtonText();
		//The player can press the WebGL player's full screen button at any time.
		//This is why I must update the full screen button's text every frame.
	}

	private static void ToggleFullScreen()
	{
		Screen.fullScreen = !Screen.fullScreen;
	}

	private void UpdateFullScreenButtonText()
	{
		fullScreenButtonText.text = Screen.fullScreen ? "Exit full screen" : "Go full screen";
	}
}
