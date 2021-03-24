using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	private Transform playerTransform;
	private Vector3 relativePositionToPlayer;

	private void Awake() {
		playerTransform = FindObjectOfType<Player>().transform;
		relativePositionToPlayer = transform.position - playerTransform.position;
	}

	void Update()
	{

		this.transform.position = playerTransform.position + relativePositionToPlayer;
	}
}
