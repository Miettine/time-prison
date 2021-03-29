using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TimeTravel;

public class CharacterInTime : ObjectInTime {
	public CharacterType _ObjectType { get; private set; }
	public Vector3 Position { get; private set; }
	public Quaternion Rotation { get; private set; }

	public ActionType Action { get; private set; }

	public CharacterInTime(string id, float time, CharacterType objectType, Vector3 position, Quaternion rotation, ActionType action) : base(id, time) {
		_ObjectType = objectType;
		Position = position;
		Rotation = rotation;
		Action = action;
	}
	/*
	public override string ToString() {
		return $"time: {Time}, position:{Position}, action:{Action}";
	}*/
	public enum CharacterType {
		Player
	}
	public enum ActionType {
		Undefined,
		Standing,
		Walking,
		StartTimeTravel,
		EnterLocker,
		HidingInLocker,
		ExitLocker
	}
}