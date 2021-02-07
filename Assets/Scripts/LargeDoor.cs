using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeDoor : MonoBehaviour
{
	MeshRenderer meshRenderer;
	//Collider triggerCollider;
	new Collider collider;

	bool isOpen = false;

	bool pastStateIsOpen = false;

	//TimeTravel timeTravel;

	private void Awake() {
		//timeTravel = FindObjectOfType<TimeTravel>();
		meshRenderer = GetComponentInChildren<MeshRenderer>();
		//triggerCollider = GetComponent<Collider>();
		collider = transform.GetChild(0).GetComponent<Collider>();
	}

	public void OpenInPast() {
		pastStateIsOpen = true;
		ShowOpenInGraphics(true);
	}

	public void OpenInPresent() {
		isOpen = true;
		ShowOpenInGraphics(true);
	}

	public void CloseInPast() {
		pastStateIsOpen = false;
		ShowOpenInGraphics(false);
	}
	public void CloseInPresent() {
		isOpen = false;
		ShowOpenInGraphics(false);
	}


	private void ShowOpenInGraphics(bool open) {
		meshRenderer.enabled = !open;
		collider.enabled = !open;
	}

	internal bool IsOpenInPresent() {
		return isOpen;
	}

	internal bool IsOpenInPast() {
		return pastStateIsOpen;
	}
}
