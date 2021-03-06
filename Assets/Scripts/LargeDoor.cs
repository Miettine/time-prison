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
		Debug.Log("OpenByPastAction");
	}

	public void OpenByPresentAction() {
		isOpen = true;
		EnableGraphicsAndCollider(false);
		Debug.Log("OpenByPresentAction");
	}

	public void CloseByPastAction() {
		pastStateIsOpen = false;
		isOpen = false;
		EnableGraphicsAndCollider(true);
		Debug.Log("CloseByPastAction");
	}
	public void CloseByPresentAction() {
		isOpen = false;
		EnableGraphicsAndCollider(true);
		Debug.Log("CloseByPresentAction");
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
