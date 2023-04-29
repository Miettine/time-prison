using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : Singleton<CameraControl> {

	[SerializeField] private float timeParadoxAnimationCameraPanningTime = 1.5f;
	private Transform playerTransform;
	private Vector3 relativePositionToTarget;
	bool followingPlayer;

	private void Awake() {
		playerTransform = FindObjectOfType<Player>().transform;
		relativePositionToTarget = transform.position - playerTransform.position;
	}

	private void Start() {
		followingPlayer = true;
	}

	void Update() {
		if (followingPlayer) {
			transform.position = GetCameraLocation(playerTransform);
		}
	}

	Vector3 GetCameraLocation(Transform pointToTarget) {
		return pointToTarget.position + relativePositionToTarget;
	}

	public IEnumerator MoveToTarget(Transform targetTransform) {
		Vector3 startPosition = transform.position;
		float elapsedTime = 0f;

		while (elapsedTime < timeParadoxAnimationCameraPanningTime) {
			elapsedTime += Time.deltaTime;
			float t = Mathf.Clamp01(elapsedTime / timeParadoxAnimationCameraPanningTime);
			transform.position = Vector3.Lerp(startPosition, GetCameraLocation(targetTransform), t);
			yield return null;
		}
	}

	internal void OnTimeParadox() {
		followingPlayer = false;
	}

	internal float GetTimeParadoxAnimationCameraPanningTime() {
		return timeParadoxAnimationCameraPanningTime;
	}
}
