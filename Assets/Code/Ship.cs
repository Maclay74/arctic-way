
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Ship : MonoBehaviour{


    public Vector3[] path;
    
    
    public float TimeToSegment = 5f;
    private float TimeOnSegment = 0f;
    public int currentPointIndex = 0;

    public int startPoint = 0;
    public int finishPoint = 1;

    public GameManager manager;

    public int Reward = 0;

    private int direction = 1;
    private SpriteRenderer sprite;

    public List<Sprite> Sprites;
    
    private void Start(){
        
        sprite = GetComponentInChildren<SpriteRenderer>();
        
        TimeOnSegment = Time.time;
        currentPointIndex = startPoint;

        if (startPoint < finishPoint) 
            direction = 1;
        else {
            direction = -1;
        }

        Reward = Math.Abs(startPoint - finishPoint) * manager.ShipLoadingPrice;
        
        Vector3 startPosition = path[currentPointIndex];
        transform.position = startPosition;


        int randomSpriteIndex = UnityEngine.Random.Range(0, Sprites.Count);
        sprite.sprite = Sprites[randomSpriteIndex];
        
        sprite.enabled = true;
    }

    private void Update(){

        if (currentPointIndex == finishPoint) {
            manager.ShipDoneEvent.Invoke(this);
            Destroy(gameObject);
            return;
        }
        
        Vector3 currentPosition = path[currentPointIndex];

        try {
            Vector3 targetPosition = path[currentPointIndex + direction];
            
            float deltaTime = Mathf.InverseLerp(TimeOnSegment, TimeOnSegment + TimeToSegment, Time.time);
            Vector3 deltaPosition = Vector3.Lerp(currentPosition, targetPosition, deltaTime);

            transform.position = deltaPosition;
        
        
            Vector3 currentPositionDiff = targetPosition - currentPosition;
            currentPositionDiff.Normalize();
 
            float currentPositionZ = Mathf.Atan2(currentPositionDiff.y, currentPositionDiff.x) * Mathf.Rad2Deg;
            Quaternion currentRotation  = Quaternion.Euler(0f, 0f, currentPositionZ - 90);
        
            Quaternion targetRotation = currentRotation;
        

            if (currentPointIndex + direction * 2 < path.Length && currentPointIndex + direction * 2 > 0 ) {
                Vector3 targetPositionDiff = path[currentPointIndex + direction * 2] - targetPosition;
                targetPositionDiff.Normalize();

                float targetRotationZ = Mathf.Atan2(targetPositionDiff.y / 2 , targetPositionDiff.x / 2) * Mathf.Rad2Deg;
                targetRotation  = Quaternion.Euler(0f, 0f, targetRotationZ  - 90);
            }
        

            transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, deltaTime);

            if (deltaTime >= 1) {
                TimeOnSegment = Time.time;
            
                currentPointIndex += direction;

               
            }
        }
        catch (IndexOutOfRangeException e) {
            Debug.Log(currentPointIndex);
            Debug.Log(direction);
            Debug.Log(finishPoint);
        }
       
        
       

        
    }

    public void OnTriggerEnter2D(Collider2D other){

        GameObject otherGameObject = other.gameObject;
        if (otherGameObject.CompareTag("Segment")) {

            Segment script = otherGameObject.GetComponent<Segment>();
            script.isShipOn = true;
            if (script.State == Segment.StateType.Frozen) {
                manager.ShipFailedEvent.Invoke(this);
                Destroy(gameObject);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D other){
        
        GameObject otherGameObject = other.gameObject;
        if (otherGameObject.CompareTag("Segment")) {

            Segment script = otherGameObject.GetComponent<Segment>();
            script.isShipOn = false;
        }
    }
}
