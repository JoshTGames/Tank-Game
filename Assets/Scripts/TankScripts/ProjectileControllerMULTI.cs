using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshsAI;
using PopEm.Multiplayer.Lobby;
using Mirror;
public class ProjectileControllerMULTI : NetworkBehaviour{
    [SerializeField] float speed;
    [SerializeField] GameObject[] particles;

    [SerializeField] Collider2D cc;
    [SerializeField] Rigidbody2D rb;


    [SerializeField] float damage, damageRadius;
    public LayerMask whatIsEnemy;
    public Transform owner;
    Vector3 endPos;
    
    bool canDetonate = false;
    [ClientRpc] public void SetVectors(Vector2 end){ 
        endPos = end; 
        canDetonate = true;        
    }

    [ServerCallback] private void OnTriggerEnter2D(Collider2D collision){        
        Detonate(collision.transform);
    }
    
    
     bool hasPlayed = false;
    [ClientRpc] void SpawnFX(Vector3 pos){
        if(hasPlayed){ return; }
        foreach (GameObject particle in particles){ // FX
            GameObject cloned = Instantiate(particle, pos , Quaternion.identity, transform.parent);
            cloned.GetComponent<ParticleSystem>().Play();            
        }
        hasPlayed = true;
    }

    bool hasHitAlready = false;

    [ClientRpc] void SetDestroyValue(Vector3 pos){ transform.GetComponent<DestroyAmmoMULTI>().SetPos(pos); }
    [Server] void Detonate(Transform obj){   
        if(hasHitAlready){ return; }  
        
        SpawnFX(transform.position + -transform.right/10); 
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, damageRadius, whatIsEnemy);        
        foreach (Collider2D col in colliders){                    
            Player_Movement player = col?.transform.GetComponent<Player_Movement>();
            try{                
                if(player && col.transform != obj){ player.health -= damage / 2;}
                else if(player){ player.health -= damage; } 

                if(player && player.health <= 0){ 
                    ChangeCamera(player.transform.GetComponent<NetworkIdentity>().connectionToClient, owner);
                    //Camera.main.GetComponent<Camera_Controller>().target = owner; 
                }               
            }
            catch(System.Exception err) { Debug.Log(err); }
        }
        hasHitAlready = true;
        SetDestroyValue(transform.position);        
    }

    [TargetRpc] void ChangeCamera(NetworkConnection conn, Transform target){
        Camera.main.GetComponent<Camera_Controller>().target = target;
    }

    [ServerCallback] private void Update(){
        float dot = Vector2.Dot(transform.right, (endPos - transform.position));        
        if (dot <= 0 && canDetonate) { // IF INFRONT OF ENDPOINT                          
            Detonate(null);         
        }
    }    

    [ServerCallback] void FixedUpdate(){ // MOVEMENT
        rb.MovePosition(transform.position + transform.right * speed * Time.fixedDeltaTime);
    }
    private void OnDrawGizmos(){
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
