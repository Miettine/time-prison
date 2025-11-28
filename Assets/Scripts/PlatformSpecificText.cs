using System;
using TMPro;
using UnityEngine;
public class PlatformSpecificText : Singleton<PlatformSpecificText>
{

	TextMeshProUGUI mobileText;
	TextMeshProUGUI pcText;
	Player player;
	bool showTimeMachineObtainedTutorialText = false;

	private void Awake()
	{
		player = Player.GetInstance();

		mobileText = transform.Find("MobileText").GetComponent<TextMeshProUGUI>();
		pcText = transform.Find("PCText").GetComponent<TextMeshProUGUI>();
	}

	private void Start()
	{
		Hide();
	}

	internal void ShowTimeMachineObtainedTutorialText(Player.ControlMode currentControlMode)
	{
		showTimeMachineObtainedTutorialText = true;
	}
	void Update()
	{
		if (showTimeMachineObtainedTutorialText)
		{
			switch (player._ControlMode)
			{
				case Player.ControlMode.Keyboard:
					mobileText.gameObject.SetActive(false);
					pcText.gameObject.SetActive(true);
					break;
				case Player.ControlMode.Touch:
					mobileText.gameObject.SetActive(true);
					pcText.gameObject.SetActive(false);
					break;
			}
		}
	}

	internal void Hide()
	{
		showTimeMachineObtainedTutorialText = false;
		mobileText.gameObject.SetActive(false);
		pcText.gameObject.SetActive(false);
	}
}