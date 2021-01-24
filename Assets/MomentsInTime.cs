using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentsInTime
{
	List<ObjectInTime> momentsInTime = new List<ObjectInTime>();
	public void AddObjectInTime(ObjectInTime stateInTime) {
		momentsInTime.Add(stateInTime);
	}

	/// <summary>
	/// Not used yet.
	/// </summary>
	/// <param name="time"></param>
	/// <param name="objectInTime"></param>
	public void AddObjectInTime(float time, ObjectInTime objectInTime) {
		AddObjectInTime(objectInTime);
	}
	public ObjectInTime GetObjectInTime(float time) {
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
