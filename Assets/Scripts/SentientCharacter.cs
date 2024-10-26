using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class was made in preparation of making the Guard or the Warden. The purpose was to
/// re-use some code related to seeing or hearing the player. Development of the Guard enemy characters is discontinued.
/// </summary>
public class SentientCharacter : MonoBehaviour
{
	Transform playerTransform;
	protected TimeTravel timeTravel;

	float fieldOfViewDegrees;
	float fieldOfViewRange;

	int lineOfSightLayerMask;
	int playerLayer;
	int doorsLayer;
	Player player;
	public Transform TimeParadoxObject { get; private set; }

	void Awake() {
		timeTravel = FindFirstObjectByType<TimeTravel>(FindObjectsInactive.Include);

		lineOfSightLayerMask = LayerMask.GetMask("Player", "Walls", "Doors");
		playerLayer = LayerMask.NameToLayer("Player");
		doorsLayer = LayerMask.NameToLayer("Doors");
	}

	void Start() {
		var fieldOfViewLight = GetComponentInChildren<Light>();
		fieldOfViewDegrees = fieldOfViewLight.spotAngle;
		fieldOfViewRange = fieldOfViewLight.range;

		player = FindFirstObjectByType<Player>(FindObjectsInactive.Include);
		playerTransform = player.transform;
	}

	protected bool SeesPresentPlayer() {
		if (player.IsHiding()) {
			return false;
		}

		var toPlayer = playerTransform.position - this.transform.position;

		bool withinRange = toPlayer.magnitude < fieldOfViewRange;

		/*
		If the present player is not within the range of the field of view, the present player is definitely not within sight.
		*/
		if (!withinRange) {
			return false;
		}

		//The direction where this past player character is pointing
		Vector3 lookDirection = transform.rotation * Vector3.forward;

		//The angle in degrees from the look direction into the direction where the present player is.
		var lookDirectionToPlayerAngle = Vector3.Angle(lookDirection, toPlayer);

		//Look direction vector is at the center of the field of view, therefore, I must divide fieldOfViewDegrees by 2 here.
		bool withinFieldOfViewAngle = lookDirectionToPlayerAngle < fieldOfViewDegrees / 2;

		//The present player is close enough to be seen, but this past player is looking in another direction.
		if (!withinFieldOfViewAngle) {
			return false;
		}

		//Finally, we use raycasts to see if the player is hiding behind walls.

		var pos = this.transform.position;

		var vectorAtEyePoint = new Vector3(pos.x, pos.y + 1.7f - 0.15f, pos.z);

		Ray ray = new Ray(vectorAtEyePoint, toPlayer);
		Debug.DrawRay(vectorAtEyePoint, toPlayer, Color.red);

		bool hasRaycastHit = Physics.Raycast(ray, out var hitInfo, fieldOfViewRange, lineOfSightLayerMask);

		if (!hasRaycastHit) {
			/*
			Previous checks to withinRange and withinFieldOfViewAngle should have eliminated the possibility of not hitting 
			anything with the raycast. This is unexpected behaviour so we are logging this as a warning.
			*/
			Debug.LogWarning("No raycast hit was detected when determining line of sight occlusion.");
			return false;
		}

		var collider = hitInfo.collider;
		if (collider == null) {
			//Failing to find a collider shouldn't be possible, so I am throwing an error.
			throw new Exception("Did not find a collider when determining line of sight occlusion.");
		}

		return collider.gameObject.layer == playerLayer;

		/*
		var transformParent = collider.gameObject.transform.parent;
		if (transformParent == null) {
			return false;
		}
		return transformParent.name == "Player";*/
	}

	public bool HeardPresentPlayer() {
		throw new Exception("Not supported yet");
	}

	protected bool SeesObjectInteractionFromPresentPlayer() {
		//TODO: Eliminate repeated code between this and SeesPresentPlayer.

		var doors = FindObjectsByType<LargeDoor>(FindObjectsSortMode.None);
		foreach (var door in doors) {

			if (door == null) {
				return false;
			}
			var targetTransform = door.transform;

			var toTarget = targetTransform.position - this.transform.position;

			bool withinRange = toTarget.magnitude < fieldOfViewRange;

			if (!withinRange) {
				return false;
			}

			//The direction where this past player character is pointing
			Vector3 lookDirection = transform.rotation * Vector3.forward;

			var lookDirectionToTargetAngle = Vector3.Angle(lookDirection, toTarget);

			bool withinFieldOfViewAngle = lookDirectionToTargetAngle < fieldOfViewDegrees / 2;

			if (!withinFieldOfViewAngle) {
				return false;
			}

			var pos = this.transform.position;

			var vectorAtEyePoint = new Vector3(pos.x, pos.y + 1.7f - 0.15f, pos.z);

			Ray ray = new Ray(vectorAtEyePoint, toTarget);
			Debug.DrawRay(vectorAtEyePoint, toTarget, Color.red);

			bool hasRaycastHit = Physics.Raycast(ray, out var hitInfo, fieldOfViewRange, LayerMask.GetMask("Doors"), QueryTriggerInteraction.Collide);

			if (!hasRaycastHit) {
				/*
				Previous checks to withinRange and withinFieldOfViewAngle should have eliminated the possibility of not hitting 
				anything with the raycast. This is unexpected behaviour so we are logging this as a warning.
				*/
				Debug.LogWarning("No raycast hit was detected when determining line of sight occlusion.");
				return false;
			}

			var collider = hitInfo.collider;

			if (collider == null) {
				//Failing to find a collider shouldn't be possible, so I am throwing an error.
				throw new Exception("Did not find a collider when determining line of sight occlusion.");
			}
			//Debug.Log($"{collider.gameObject.layer} != {collider.gameObject.layer}");
			if (collider.gameObject.layer != doorsLayer) {
				return false;
			}

			bool HasStateContradiction = timeTravel.HasStateContradiction(door.name, door);
			if (HasStateContradiction) {
				TimeParadoxObject = door.transform;
				return true;
			} else {
				return false;
			}
		}
		return false;
	}
}
