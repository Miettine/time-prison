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

	public void OpenByPastAction() {
		pastStateIsOpen = true;
		isOpen = true;
		EnableGraphicsAndCollider(true);
	}

	public void OpenByPresentAction() {
		isOpen = true;
		EnableGraphicsAndCollider(true);
	}

	public void CloseByPastAction() {
		pastStateIsOpen = false;
		isOpen = false;
		EnableGraphicsAndCollider(false);
	}
	public void CloseByPresentAction() {
		isOpen = false;
		EnableGraphicsAndCollider(false);
	}

	private void EnableGraphicsAndCollider(bool open) {
		meshRenderer.enabled = !open;
		collider.enabled = !open;
	}

	internal bool IsOpenByPresentAction() {
		return isOpen;
	}

	internal bool IsOpenByPastAction() {
		return pastStateIsOpen;
	}
}
