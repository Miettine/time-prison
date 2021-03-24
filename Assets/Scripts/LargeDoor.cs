using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeDoor : MonoBehaviour
{
	GameObject door;

	bool isOpen = false;

	bool pastStateIsOpen = false;

	//TimeTravel timeTravel;

	private void Awake() {
		door = transform.GetChild(0).gameObject;
	}

	public void OpenByPastAction() {
		pastStateIsOpen = true;
		isOpen = true;
		EnableGraphicsAndCollider(false);
	}

	public void OpenByPresentAction() {
		isOpen = true;
		EnableGraphicsAndCollider(false);
	}

	public void CloseByPastAction() {
		pastStateIsOpen = false;
		isOpen = false;
		EnableGraphicsAndCollider(true);
	}
	public void CloseByPresentAction() {
		isOpen = false;
		EnableGraphicsAndCollider(true);
	}

	private void EnableGraphicsAndCollider(bool enabled) {
		door.SetActive(enabled);
	}

	internal bool IsOpenByPresentAction() {
		return isOpen;
	}

	internal bool IsOpenByPastAction() {
		return pastStateIsOpen;
	}
	
	public bool HasStateContradiction(){
		return pastStateIsOpen != isOpen;
	}
}
