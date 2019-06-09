using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseInfoManager : MonoBehaviour{

	public TextMeshProUGUI Content;

	public void Resume(){
		Time.timeScale = 1;
		SceneManager.UnloadSceneAsync("BaseInfo");
	}

}
