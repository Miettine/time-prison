using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PastPlayerController : MonoBehaviour
{
	TimeRecorder timeRecorder;

	private void Awake() {
		timeRecorder = GameObject.Find("Player").GetComponent<TimeRecorder>();

	}

	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		/*
		var stateInTime = timeRecorder.GetState(Time.time- 5f);
		//Debug.Log(stateInTime);
		if (stateInTime != null) {
			this.transform.position = stateInTime.Position;
			this.transform.rotation = stateInTime.Rotation;
		}*/
	}
}
