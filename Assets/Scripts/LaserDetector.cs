using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDetector : MonoBehaviour
{
	SecuritySystem securitySystem;

	LineRenderer laserLine;

	/// <summary>
	/// This should never have any real use in an actual level. 
	/// Thus not making it a serialized variable, changable in the editor
	/// </summary>
	float maxDistance = 20f;

	void Awake() {
		securitySystem = FindFirstObjectByType<SecuritySystem>(FindObjectsInactive.Include);
	}

	void Start() {
		laserLine = GetComponentInChildren<LineRenderer>();
	}

	void Update() {
		Vector3 laserDirection = transform.forward;

		Ray ray = new Ray(transform.position, laserDirection);

		Physics.Raycast(ray, 
			out RaycastHit hitInfo, 
			maxDistance, 
			LayerMask.GetMask("Walls", "Player", "PastPlayer"), 
			QueryTriggerInteraction.Ignore);

		laserLine.SetPosition(0, transform.position);

		if (hitInfo.collider != null) {
			laserLine.SetPosition(1, hitInfo.point);

			if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
				securitySystem.DetectedPresentPlayer();
			}
		} else {
			laserLine.SetPosition(1, this.transform.position + laserDirection * maxDistance);
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
			securitySystem.DetectedPresentPlayer();
		}
	}
}
