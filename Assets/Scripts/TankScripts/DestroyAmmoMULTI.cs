using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAmmoMULTI : MonoBehaviour{
    public Vector3 posToDestroyAt;
    bool posSet = false;
    public void SetPos(Vector3 pos){
        posSet = true;
        posToDestroyAt = pos;
    }
    void Update(){
        float dot = Vector2.Dot(transform.right, (posToDestroyAt - transform.position));             
        if(dot <= 0 && posSet){ Destroy(gameObject); }
    }
}
