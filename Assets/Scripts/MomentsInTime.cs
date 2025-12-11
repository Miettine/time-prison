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
	public void AddObject(float? time, ObjectInTime objectInTime) {
		AddObject(objectInTime);
	}
	public T GetObject<T>(string id, float time) where T : ObjectInTime {

		var match = objectsInTime.Find(objectInTime => 
		id.Equals(objectInTime.Name) && 
		objectInTime.Time >= time &&
		objectInTime is T);

		return (T)match;
	}

	public T GetObject<T>(float time) where T : ObjectInTime {

		var match = objectsInTime.Find(objectInTime =>
		objectInTime.Time >= time &&
		objectInTime is T);

		return (T)match;
	}
	public T GetCharacter<T>(string id, float time) where T : CharacterInTime
	{

		var match = objectsInTime.Find(objectInTime =>
		id.Equals(objectInTime.Name) &&
		objectInTime.Time >= time &&
		objectInTime is T);

		return (T)match;
	}

	// Added overload that filters by the requested action type
	public T GetCharacter<T>(string id, float time, CharacterInTime.ActionType action) where T : CharacterInTime
	{
		var match = objectsInTime.Find(objectInTime =>
		id.Equals(objectInTime.Name) &&
		objectInTime.Time >= time &&
		objectInTime is T &&
		((CharacterInTime)objectInTime).Action == action);

		return (T)match;
	}
}
