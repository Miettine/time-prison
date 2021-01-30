using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	private Transform playerTransform;
	private Vector3 relativePositionToPlayer;

	private void Awake() {

	}

	// Start is called before the first frame update
	void Start()
	{
		var pc = GameObject.Find("Player").GetComponent<PlayerController>();
		playerTransform = pc.transform;
		relativePositionToPlayer = playerTransform.position + transform.position;
	}

	// Update is called once per frame
	void Update()
	{
		this.transform.position = playerTransform.position + relativePositionToPlayer;
	}
}
