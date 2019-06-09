using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Segment : MonoBehaviour{

    public UnityEvent ClickEvent;

    private MeshCollider collider;
    private MeshRenderer renderer;

    public GameManager manager;

    public Material IceMaterial;
    public Material WaterMaterial;
    public Material FrozenMaterial;
    public Material CrackedMaterial;
    public bool isShipOn;
    
    
    public enum StateType {Water, Frozen, Ice, Cracked}

    public StateType State = StateType.Ice;

    void Start(){
        collider = GetComponent<MeshCollider>();
        renderer = GetComponent<MeshRenderer>();
        
        
        
        ClickEvent.AddListener(OnClick);
    }

    public void UpdateCollider(){
        SetPolygonCollider3D.UpdatePolygonCollider2D(GetComponent<MeshFilter>());
    }
    
    public void setState(StateType newState){
        
        Material[] mats = GetComponent<Renderer>().materials;

        switch (newState) {
        
            case StateType.Water:
                mats[0] = WaterMaterial;
                break;
            case StateType.Frozen:
                mats[0] = FrozenMaterial;
                break;
            case StateType.Ice:
                mats[0] = IceMaterial;
                break;
            case StateType.Cracked:
                mats[0] = CrackedMaterial;
                break;
        }
        
        GetComponent<Renderer>().materials = mats;

        State = newState;
    }

    void OnClick(){
       
        if (State == StateType.Cracked) {
            manager.SegmentClickEvent.Invoke(this);
            SoundManager.PlaySound(0);
        }

        if (State == StateType.Frozen) {
            setState(StateType.Water);
            SoundManager.PlaySound(0);
        }
        
    }

}
