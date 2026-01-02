using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TimeTravel;

public class CharacterInTime : ObjectInTime {
	public Vector3 Position { get; private set; }
	public Quaternion Rotation { get; private set; }

	public ActionType Action { get; private set; }

	public CharacterInTime(string id, float time, Vector3 position, Quaternion rotation, ActionType action) : base(id, time) {
		Position = position;
		Rotation = rotation;
		Action = action;
	}

	public enum ActionType {
		Undefined,
		Standing,
		Walking,
		StartTimeTravel
	}
}