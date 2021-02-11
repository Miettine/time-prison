using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void ReloadCurrentLevel(){
		 LoadSceneWithIndex(GetActiveSceneIndex());
	}

	internal void OnLevelPassedTriggerEnter() {
        LoadNextScene();
    }

    void LoadNextScene() {
        LoadSceneWithIndex(GetActiveSceneIndex() + 1);
    }
	
	void LoadSceneWithIndex(int index){
		SceneManager.LoadScene(index);
	}
	
	int GetActiveSceneIndex(){
		return SceneManager.GetActiveScene().buildIndex;
	}
}
