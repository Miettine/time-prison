using System.Collections.Generic;

/// <summary>
/// Is used to store and retrieve the states and actions of objects and characters throughout moments in time.
/// </summary>
public class MomentsInTime
{
	/// <summary>
	/// Internal list storing recorded object states in chronological order.
	/// Each entry represents the state of a particular object at a specific time (seconds since scene start).
	/// The list is used for lookups and is expected to be appended to in chronological order by the recorder.
	/// </summary>
	List<ObjectInTime> objectsInTime = new List<ObjectInTime>();

	/// <summary>
	/// Add a recorded object state to the collection.
	/// </summary>
	/// <param name="stateInTime">The object state to add. An instance derived from <see cref="ObjectInTime"/>.</param>
	public void AddObject(ObjectInTime stateInTime) {
		objectsInTime.Add(stateInTime);
	}

	/// <summary>
	/// Get a specific object state at a specific moment in time.
	/// Find the first object that matches the name, is of type T, and has a time greater than or equal to the specified time
	/// </summary>
	/// <typeparam name="T">The type of the object in time</typeparam>
	/// <param name="name">The name of the gameobject</param>
	/// <param name="time">The moment in time from scene start where you wish to retrieve the object's state</param>
	/// <remarks>For the record, I wrote this code before AI was invented. So this came from my own brain.</remarks>
	/// <returns> The matching object of type <typeparamref name="T"/>, or <c>null</c> if no match is found. </returns>
	public T GetObject<T>(string name, float time) where T : ObjectInTime {
		// The time comparison has a greater than or equal to operator.
		// The reason why we can use this is because the objects in time are placed in a list in chronological order. 
		// The exact time is almost never going to be found because we are recording frames at intervals and the game can be running at different framerates.
		var match = objectsInTime.Find(objectInTime => 
		name.Equals(objectInTime.Name) && 
		objectInTime.Time >= time &&
		objectInTime is T);

		return (T)match;
	}

	/// <summary>
	/// Get the first recorded object of type <typeparamref name="T"/> with a time greater than or equal to <paramref name="time"/>.
	/// </summary>
	/// <typeparam name="T">The concrete <see cref="ObjectInTime"/> derived type expected.</typeparam>
	/// <param name="time">The moment in time (seconds since scene start) from which to retrieve the state.</param>
	/// <returns>
	/// The matching object of type <typeparamref name="T"/>, or <c>null</c> if no match is found.
	/// </returns>
	public T GetObject<T>(float time) where T : ObjectInTime {

		var match = objectsInTime.Find(objectInTime =>
		objectInTime.Time >= time &&
		objectInTime is T);

		return (T)match;
	}

	public CharacterInTime GetCharacter(string id, float time)
	{
		var match = objectsInTime.Find(objectInTime =>
		id.Equals(objectInTime.Name) &&
		objectInTime.Time >= time &&
		objectInTime is CharacterInTime);

		return (CharacterInTime) match;
	}
}
