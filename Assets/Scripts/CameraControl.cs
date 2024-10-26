using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : Singleton<CameraControl> {


	private Transform playerTransform;
	private Vector3 relativePositionToTarget;
	bool followingPlayer;
	UI ui;

	[SerializeField] private float timeParadoxEffectMaxFieldOfView = 179f;

	private void Awake() {
		ui = UI.GetInstance();
		playerTransform = FindFirstObjectByType<Player>().transform;
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

	public IEnumerator MoveToTarget(Transform targetTransform, float cameraPanningTime) {
		Vector3 startPosition = transform.position;
		float elapsedTime = 0f;

		while (elapsedTime < cameraPanningTime) {
			elapsedTime += Time.deltaTime;
			float t = Mathf.Clamp01(elapsedTime / cameraPanningTime);
			transform.position = Vector3.Lerp(startPosition, GetCameraLocation(targetTransform), t);
			yield return null;
		}
	}

	internal void OnTimeParadox() {
		followingPlayer = false;
	}

	internal void StartTimelineResetAnimation() {
		StartCoroutine(TimelineResetAnimation());
	}

	IEnumerator TimelineResetAnimation() {
		float elapsedTime = 0f;

		Camera unityCamera = GetComponent<Camera>();
		float startFieldOfView = unityCamera.fieldOfView;
		float warpEffectLength = ui.GetTimeParadoxAnimationStepDelay();

		while (elapsedTime < warpEffectLength) {
			elapsedTime += Time.deltaTime;
			float t = Mathf.Clamp01(elapsedTime / warpEffectLength);

			// ChatGPT: Apply exponential function
			float exponentialT = Mathf.Pow(t, 2f);

			unityCamera.fieldOfView = Mathf.Lerp(startFieldOfView, timeParadoxEffectMaxFieldOfView, exponentialT);

			yield return null;
		}
	}
}
