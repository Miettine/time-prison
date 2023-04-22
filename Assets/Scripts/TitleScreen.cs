using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : Singleton<TitleScreen> {

	[SerializeField]
	string fullScreenButtonName = "FullScreenButton";

	[SerializeField]
	string startButtonName = "StartGameButton";

	[SerializeField]
	string firstLevelSceneName = "Level1";

	private TextMeshProUGUI fullScreenButtonText;

	void Awake() {
		Button fullScreenButton = GameObject.Find(fullScreenButtonName).GetComponent<Button>();
		fullScreenButton.onClick.AddListener(() => ToggleFullScreen());
		fullScreenButtonText = fullScreenButton.transform.GetComponentInChildren<TextMeshProUGUI>();
		UpdateFullScreenButtonText();
		GameObject.Find(startButtonName).GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene(firstLevelSceneName));
	}

	public void ToggleFullScreen() {
		Screen.fullScreen = !Screen.fullScreen;
		UpdateFullScreenButtonText();
	}

	private void UpdateFullScreenButtonText() {
		if (Screen.fullScreen) {
			fullScreenButtonText.text = "Exit full screen";
		} else {
			fullScreenButtonText.text = "Go full screen";
		}
	}
}
