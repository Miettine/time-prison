using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PastPlayerController : MonoBehaviour
{
	float fieldOfViewDegrees;
	float fieldOfViewRange;
	Transform playerTransform;
	TimeTracker timeTracker;

	int lineOfSightLayerMask;
	int playerLayer;

	private void Awake() {
		timeTracker = FindObjectOfType<TimeTracker>();

		lineOfSightLayerMask = LayerMask.GetMask("Player", "Walls");
		playerLayer = LayerMask.NameToLayer("Player");
	}

	// Start is called before the first frame update
	void Start()
	{
		var light = GetComponentInChildren<Light>();
		fieldOfViewDegrees = light.spotAngle;
		fieldOfViewRange = light.range;

		playerTransform = FindObjectOfType<PlayerController>().transform;
	}

	// Update is called once per frame
	void Update()
	{
		if (SeesPresentPlayer()) {
			timeTracker.TimeParadox();
		}
	}
	
	private bool SeesPresentPlayer() {
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

	public bool HeardPresentPlayer(){
		throw new Exception("Not supported yet");
	}
}
