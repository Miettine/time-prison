using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeDoor : MonoBehaviour
{
	bool isOpen = false;
	public void Open() {
		gameObject.SetActive(false);
		isOpen = true;
	}

	public void Close() {
		gameObject.SetActive(true);
		isOpen = false;
	}

	internal bool IsOpen() {
		return isOpen;
	}
}
