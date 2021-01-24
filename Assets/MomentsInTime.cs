using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Is used to store and retrieve the states and actions of objects and characters throughout moments in time.
/// </summary>
public class MomentsInTime
{
	//Dictionary<float, ObjectInTime> _momentsInTime = new Dictionary<float, ObjectInTime>();

	List<ObjectInTime> momentsInTime = new List<ObjectInTime>();
	public void AddObject(ObjectInTime stateInTime) {
		momentsInTime.Add(stateInTime);
	}

	/// <summary>
	/// Not used yet.
	/// </summary>
	/// <param name="time"></param>
	/// <param name="objectInTime"></param>
	public void AddObject(float time, ObjectInTime objectInTime) {
		AddObject(objectInTime);
	}
	public ObjectInTime GetObject(float time) {
		/*
		for (int i = 0; i < statesInTime.Capacity - 1; i++) {
			var s = statesInTime[i];
			if (time <= s.Time) {
				return s;
			}
		}

		return null;*/
		var matches = momentsInTime.Find(stateInTime => stateInTime.Time >= time);
		return matches;
	}

}
