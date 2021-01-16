using System.Collections;

using System.Collections.Generic;
using UnityEngine;

public class TimeRecorder : MonoBehaviour
{

	TimeTracker timeTracker;
	//float ErrorMargin = 0.1f;
	private void Awake() {
		timeTracker = FindObjectOfType<TimeTracker>();
	}
	// Start is called before the first frame update
	void Start()
	{
		
	}
	void FixedUpdate()
	{
		//var stateInTime = new StateInTime(TimeTracker.ObjectInTime.Player, Time.time, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
		//timeTracker.AddStateInTime(stateInTime);
	}

	
}
