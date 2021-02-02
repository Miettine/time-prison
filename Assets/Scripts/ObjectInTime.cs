using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TimeTravel;

public class ObjectInTime {
	public string Id { get; private set; }
	public ObjectType _ObjectType { get; private set; }
	public float Time { get; private set; }
	public Vector3 Position { get; private set; }
	public Quaternion Rotation { get; private set; }

	public ActionType Action { get; private set; }

	public ObjectInTime(string id, ObjectType objectType, float time, Vector3 position, Quaternion rotation, ActionType action) {
		Id = id;
		_ObjectType = objectType;
		Time = time;
		Position = position;
		Rotation = rotation;
		Action = action;
	}

	public override string ToString() {
		return $"time: {Time}, position:{Position}, action:{Action}";
	}
	public enum ObjectType {
		Player
	}
	public enum ActionType {
		Undefined,
		Standing,
		Walking,
		StartTimeTravel
	}
}