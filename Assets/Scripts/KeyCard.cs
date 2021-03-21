using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCard : MonoBehaviour {
	[SerializeField]
	KeyCardType type;

	internal KeyCardType GetKeyCardType() {
		return type;
	}
}
