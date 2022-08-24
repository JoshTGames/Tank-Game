using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FaceTarget : MonoBehaviour{
    Vector3 mousePos = Vector3.zero;
    [SerializeField] bool useMousePos;
    public Transform target;
    [HideInInspector] public bool HasTarget{ get{ return (target)? true: false; }}
    [HideInInspector] public Vector3 targetPos; // This is the position the object will face
    Camera cam;

    [SerializeField] float rotateSmoothing;
    float rotateVel;

    
 
    private void Start(){
        if (!useMousePos) { return; }
        cam = useMousePos ? Camera.main : null;        
    }

    
    private void Update(){
        if (useMousePos){
            mousePos = Input.mousePosition;
            Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);            
            targetPos = new Vector3(worldPos.x, worldPos.y, 0);
        }
        else if(target){ targetPos = target.position; }
        

        Vector3 direction = targetPos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = Mathf.SmoothDampAngle(transform.rotation.eulerAngles.z, angle, ref rotateVel, rotateSmoothing);        
        Quaternion newRot = Quaternion.AngleAxis(angle, Vector3.forward);      

        transform.rotation = newRot;        
    }
}
