using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoshsAI;
using PopEm.Multiplayer.Lobby;

public class ProjectileController : MonoBehaviour{
    [SerializeField] float speed;
    [SerializeField] GameObject[] particles;

    [SerializeField] Collider2D cc;
    [SerializeField] Rigidbody2D rb;


    [SerializeField] float damage, damageRadius;
    public LayerMask whatIsEnemy;
    public Transform owner;
    Vector3 endPos;
    public void SetVectors(Vector2 end){
        endPos = end;        
    }

    private void OnTriggerEnter2D(Collider2D collision){        
        Detonate(collision);
    }
    
    



    void Detonate(Collider2D collider){
        foreach (GameObject particle in particles){ // FX
            GameObject cloned = Instantiate(particle, transform.position + -transform.right/10, Quaternion.identity, transform.parent);
            cloned.GetComponent<ParticleSystem>().Play();
        }
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, damageRadius, whatIsEnemy);        
        foreach (Collider2D col in colliders){
            SpiderMechAI ai = col?.transform.GetComponent<SpiderMechAI>();            
            PLR_MOVE_SINGLEPLR player = col?.transform.GetComponent<PLR_MOVE_SINGLEPLR>();
            try{
                if (ai && col != collider) { ai.Health -= damage / 2; }
                else if(ai){ ai.Health -= damage; }

                else if(player && col != collider){ player.Health -= damage / 2;}
                else if(player){ player.Health -= damage; } 

                if(player && player.Health <= 0){Camera.main.GetComponent<Camera_Controller>().target = owner; }               
            }
            catch(System.Exception err) { Debug.Log(err); }
        }
        Destroy(gameObject);
    }

    private void Update(){
        float dot = Vector2.Dot(transform.right, (endPos - transform.position));        
        if (dot <= 0) { // IF INFRONT OF ENDPOINT
            Detonate(null);
        }
    }



    void FixedUpdate(){ // MOVEMENT
        rb.MovePosition(transform.position + transform.right * speed * Time.fixedDeltaTime);
    }
    private void OnDrawGizmos(){
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
