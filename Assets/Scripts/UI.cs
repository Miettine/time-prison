using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
	Text TimeText;

	void Awake() {
		TimeText = GameObject.Find("TimeText").GetComponent<Text>();
	}
	// Start is called before the first frame update
	void Start()
	{
		
	}

	public void SetTimeText(float time) {
		TimeText.text = time.ToString("Time: 0");
	}
}
