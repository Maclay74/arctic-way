using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour {

	public void Resume(){
		Time.timeScale = 1;
		SceneManager.UnloadSceneAsync("Pause");
	}

	public void MainMenu(){
		SceneManager.LoadSceneAsync("StartGame");
	}
}
