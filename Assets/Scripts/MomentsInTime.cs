using System;
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
	private T GetObject<T>(string id, float time) where T : ObjectInTime {

		var match = objectsInTime.Find(objectInTime => 
		id.Equals(objectInTime.Id) && 
		objectInTime.Time >= time &&
		objectInTime is T);

		return (T)match;
	}

	public CharacterInTime GetCharacter(string id, float time) => GetObject<CharacterInTime>(id, time);

	public DoorObjectInTime GetDoorObject(string id, float time) => GetObject<DoorObjectInTime>(id, time);

	public LockerInTime GetLockerObject(string id, float time) => GetObject<LockerInTime>(id, time);
}
