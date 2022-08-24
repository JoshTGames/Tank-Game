using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualStats_Follower : MonoBehaviour{
    private void Start(){
        if(!target){ GM_UTILITIES.current.UpdateTrackingTarget += UpdateTarget; }
    }

    public Transform target;
    void UpdateTarget(Transform _target){ 
        target = _target;
        transform.position = target.position + offset;
    }



    [SerializeField] float moveSmoothing;
    [SerializeField] Vector3 offset;
    Vector3 moveVel;
    private void LateUpdate(){
        if (!target) { return; }        
        transform.position = Vector3.SmoothDamp(transform.position, target.position + offset, ref moveVel, moveSmoothing);
    }
}