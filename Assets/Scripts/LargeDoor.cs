using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeDoor : MonoBehaviour
{
	MeshRenderer meshRenderer;
	new Collider collider;

	bool isOpen = false;
	public bool IsOpen { get => isOpen; set {
			meshRenderer.enabled = !value;
			collider.enabled = !value;
			isOpen = value;
		}
	}

	private void Awake() {
		meshRenderer = GetComponentInChildren<MeshRenderer>();
		collider = GetComponentInChildren<Collider>();
	}
}
