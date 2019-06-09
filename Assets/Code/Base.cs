using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Base : MonoBehaviour{
    
    public UnityEvent OpenEvent = new UnityEvent();

    public int index = 0;

    public bool isOpen;
    
    [TextArea]
    public string Content;

    public SpriteRenderer Sprite;

    void Start(){
        Sprite = GetComponent<SpriteRenderer>();
        OpenEvent.AddListener(OnOpen);
    }

    void OnOpen(){

        Time.timeScale = 0;
        StartCoroutine(ShowModalWindow());
    }

    IEnumerator ShowModalWindow(){

       AsyncOperation loading =  SceneManager.LoadSceneAsync("BaseInfo", LoadSceneMode.Additive);

       while (!loading.isDone) {
           yield return null;
       }
       
       GameObject BaseInfoManager = GameObject.Find("BaseInfoManager");
       BaseInfoManager.GetComponent<BaseInfoManager>().Content.text = Content;

    }

}
