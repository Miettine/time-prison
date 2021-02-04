using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Is used to store and retrieve the states and actions of objects and characters throughout moments in time.
/// </summary>
public class MomentsInTime
{
	//Dictionary<float, ObjectInTime> _momentsInTime = new Dictionary<float, ObjectInTime>();

	List<ObjectInTime> objectsInTime = new List<ObjectInTime>();
	public void AddObject(ObjectInTime stateInTime) {
		objectsInTime.Add(stateInTime);
	}

	/// <summary>
	/// Not used yet.
	/// </summary>
	/// <param name="time"></param>
	/// <param name="objectInTime"></param>
	public void AddObject(float time, ObjectInTime objectInTime) {
		AddObject(objectInTime);
	}
	public ObjectInTime GetObject(string id, float time) {
		/*
		for (int i = 0; i < statesInTime.Capacity - 1; i++) {
			var s = statesInTime[i];
			if (time <= s.Time) {
				return s;
			}
		}

		return null;*/
		var matches = objectsInTime.Find(stateInTime => id.Equals(stateInTime.Id) && stateInTime.Time >= time);
		return matches;
	}

}
