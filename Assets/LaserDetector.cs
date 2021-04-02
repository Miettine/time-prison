using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDetector : MonoBehaviour
{

	SecuritySystem securitySystem;


	private void Awake() {
		securitySystem = FindObjectOfType<SecuritySystem>();
	}

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.layer == LayerMask.NameToLayer("Player")) {
			securitySystem.DetectedPlayer();
		}
	}
}
