using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TimeTracker;

public class StateInTime {
	public ObjectInTime ObjectType { get; private set; }
	public float Time { get; private set; }
	public Vector3 Position { get; private set; }
	public Quaternion Rotation { get; private set; }

	public ActionType Action { get; private set; }

	public StateInTime(ObjectInTime objectType, float time, Vector3 position, Quaternion rotation, ActionType action) {
		ObjectType = objectType;
		Time = time;
		Position = position;
		Rotation = rotation;
		Action = action;
	}

	public override string ToString() {
		return $"time: {Time}, position:{Position}, action:{Action}";
	}

	public enum ActionType {
		Undefined,
		Standing,
		Walking,
		StartTimeTravel
	}
}