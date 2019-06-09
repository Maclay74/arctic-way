using UnityEngine;

public class Movement : MonoBehaviour{

    public GameObject MainCamera;
    
    Vector2 mouseClickPos;
    Vector2 mouseCurrentPos;
    bool panning = false;
    
    void LateUpdate(){
      
        if (Input.GetKeyDown(KeyCode.Mouse0) && !panning) {
            mouseClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            panning = true;
        }
        
        if (panning) {
            mouseCurrentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var distance = mouseCurrentPos - mouseClickPos;
            
            MainCamera.transform.position += new Vector3(0,-distance.y,0);

            var currentPosition = MainCamera.transform.position;

            currentPosition.y = Mathf.Clamp(currentPosition.y, -9.8f, -0.48f);

            MainCamera.transform.position = currentPosition;

        }
 
        // If LMB is released, stop moving the camera
        if (Input.GetKeyUp(KeyCode.Mouse0))
            panning = false;
        
    }
}
