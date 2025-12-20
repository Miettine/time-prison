using System;
using UnityEngine;

/// <summary>
/// The sound indicator that shows the radius of the player's sound wave when running.
/// This must be its own class and can't be a child of the Player.
/// The player rotates, and so the sound indicator must not rotate with the player.
/// Also this makes it easier to add new features such as animations or effects to the sound indicator.
/// </summary>
public class SoundIndicator : Singleton<SoundIndicator>
{
	float runningSoundWaveRadius;

	private void Awake()
	{
		var player = Player.GetInstance();
		runningSoundWaveRadius = player.RunningSoundWaveRadius;
	}
	void Start()
	{
		// Set the scale of the sound indicator to match the running sound wave radius.
		// The 3D model is already flat so we also need to scale the y axis.
		transform.localScale = new Vector3(runningSoundWaveRadius * 2, runningSoundWaveRadius * 2, runningSoundWaveRadius * 2);
	}

	internal void Hide()
	{
		gameObject.SetActive(false);
	}

	internal void Show()
	{
		gameObject.SetActive(true);
	}
}