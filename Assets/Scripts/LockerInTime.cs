using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockerInTime : ObjectInTime {

	public bool occupied = false;

	public LockerInTime(string id, float time, bool occupied) : base(id, time) {
		this.occupied = occupied;
	}
}
