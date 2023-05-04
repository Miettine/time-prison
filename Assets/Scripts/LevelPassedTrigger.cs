using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPassedTrigger : MonoBehaviour
{
	Game game;

	private void Awake() {
		game = Game.GetInstance();

		if (game == null) {
			throw new System.Exception("Did not find instance");
		}
	}
	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
			game.OnLevelPassedTriggerEnter();
		}
	}
}
