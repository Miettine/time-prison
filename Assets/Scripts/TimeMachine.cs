using UnityEngine;

/// <summary>
/// This class represents a time machine pickup in the game.
/// </summary>
public class TimeMachine : MonoBehaviour
{
	Player player;
	UI ui;

	void Awake()
	{
		player = Player.GetInstance();
		ui = UI.GetInstance();
	}

	void OnTriggerEnter(Collider other)
	{
		player.OnTimeMachinePickedUp();
		ui.OnTimeMachineObtained();

		Destroy(gameObject);
	}
}