using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LargeDoor;

/// <summary>
/// This class was made in preparation of making the Guard enemy character. The purpose was to
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
	int doorsAndWallsLayer;
	Player player;

	/// <summary>
	/// The field of view degrees mercy tolerance is subtracted from the actual spot angle of the light component
	/// This is the angle in degrees that is subtracted to give some mercy to the player.
	/// </summary>
	private float fieldOfViewDegreesMercyTolerance = 15f;

	/// <summary>
	/// The field of view range mercy tolerance is subtracted from the actual range of the light component
	/// This is the length in meters that is subtracted to give some mercy to the player.
	/// </summary>
	private float fieldOfViewRangeMercyTolerance = 2f;

	/// <summary>
	/// It is possible to sneak right up to the past player without being seen or heard.
	/// It is also possible to materialize right on top of the past player.
	/// </summary>
	private float touchRange = 0.3f;

	public Transform TimeParadoxObject { get; private set; }

	void Awake() {
		timeTravel = FindFirstObjectByType<TimeTravel>(FindObjectsInactive.Include);

		lineOfSightLayerMask = LayerMask.GetMask("Player", "Walls", "Doors");
		playerLayer = LayerMask.NameToLayer("Player");
		doorsLayer = LayerMask.NameToLayer("Doors");
		doorsAndWallsLayer = LayerMask.GetMask("Doors", "Walls");
	}

	void Start() {
		// The field of view parameters are taken from a Light component that is a child of this GameObject.	
		// The field of view is slightly reduced to give some mercy to the player.
		// if the field of view is too precise, it can lead to a feeling of the game being unfair.
		var fieldOfViewLight = GetComponentInChildren<Light>();
		fieldOfViewDegrees = fieldOfViewLight.spotAngle - fieldOfViewDegreesMercyTolerance;
		fieldOfViewRange = fieldOfViewLight.range - fieldOfViewRangeMercyTolerance;

		player = FindFirstObjectByType<Player>(FindObjectsInactive.Include);
		playerTransform = player.transform;
	}

	protected bool TouchesPresentPlayer()
	{
		var toPlayer = playerTransform.position - transform.position;

		return toPlayer.magnitude < touchRange;
	}

	protected bool SeesPresentPlayer() {

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

		// We raise the ray's origin to approximately eye level. 1.55m above the ground is a common estimate for human eye height.
		// (1.7m height minus a small offset of 15cm which is the distance from the eyes to the top of the head)
		var vectorAtEyePoint = new Vector3(pos.x, pos.y + 1.55f, pos.z);

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

	protected bool SeesDoorPresentPlayerInteractedWith(out DoorTimeTravelState? doorTimeTravelState) {
		//TODO: Eliminate repeated code between this and SeesPresentPlayer.

		var doors = FindObjectsByType<LargeDoor>(FindObjectsSortMode.None);
		foreach (var door in doors) {

			if (door == null) {
				doorTimeTravelState = null;
				return false;
			}
			var targetTransform = door.transform;

			var toTarget = targetTransform.position - this.transform.position;

			bool withinRange = toTarget.magnitude < fieldOfViewRange;

			if (!withinRange) {
				doorTimeTravelState = null;
				return false;
			}

			//The direction where this past player character is pointing
			Vector3 lookDirection = transform.rotation * Vector3.forward;

			var lookDirectionToTargetAngle = Vector3.Angle(lookDirection, toTarget);

			bool withinFieldOfViewAngle = lookDirectionToTargetAngle < fieldOfViewDegrees / 2;

			if (!withinFieldOfViewAngle) {
				doorTimeTravelState = null;
				return false;
			}

			var pos = this.transform.position;

			var vectorAtEyePoint = new Vector3(pos.x, pos.y + 1.7f - 0.15f, pos.z);

			Ray ray = new Ray(vectorAtEyePoint, toTarget);
			Debug.DrawRay(vectorAtEyePoint, toTarget, Color.red);

			bool hasRaycastHit = Physics.Raycast(ray, out var hitInfo, fieldOfViewRange, doorsAndWallsLayer, QueryTriggerInteraction.Collide);

			if (!hasRaycastHit) {
				/*
				Previous checks to withinRange and withinFieldOfViewAngle should have eliminated the possibility of not hitting 
				anything with the raycast. This is unexpected behaviour so we are logging this as a warning.
				*/
				Debug.LogWarning("No raycast hit was detected when determining line of sight occlusion.");
				doorTimeTravelState = null;
				return false;
			}

			var collider = hitInfo.collider;

			if (collider == null) {
				//Failing to find a collider shouldn't be possible, so I am throwing an error.
				throw new Exception("Did not find a collider when determining line of sight occlusion.");
			}
			//Debug.Log($"{collider.gameObject.layer} != {collider.gameObject.layer}");
			if (collider.gameObject.layer != doorsLayer) {
				doorTimeTravelState = null;
				return false;
			}

			bool HasStateContradiction = timeTravel.HasStateContradiction(door, out DoorTimeTravelState doorTimeTravelState2);
			if (HasStateContradiction) {
				TimeParadoxObject = door.transform;
				doorTimeTravelState = doorTimeTravelState2;
				return true;
			} else {
				doorTimeTravelState = doorTimeTravelState2;
				return false;
			}
		}
		doorTimeTravelState = null;
		return false;
	}
}
