using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDetector : MonoBehaviour
{
	SecuritySystem securitySystem;

	LineRenderer laserLine;

	private void Awake() {
		securitySystem = FindObjectOfType<SecuritySystem>();
	}

	private void Start() {
		laserLine = GetComponent<LineRenderer>();
	}

	private void Update() {
		Ray ray = new Ray(this.transform.position, Vector3.right);
		Debug.DrawRay(this.transform.position, Vector3.right);
		Physics.Raycast(ray, out RaycastHit hitInfo, 20f, LayerMask.GetMask("Walls", "Player", "PastPlayer"), QueryTriggerInteraction.Ignore);

		laserLine.SetPosition(0, this.transform.position);
		if (hitInfo.collider != null) {
			laserLine.SetPosition(1, hitInfo.point);

			if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
				securitySystem.DetectedPlayer();
			}

		}else {
			laserLine.SetPosition(1, this.transform.position+ Vector3.right *20f);
		}
		
	}

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
			securitySystem.DetectedPlayer();
		}
	}
}
