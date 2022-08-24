using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

namespace PopEm.Multiplayer.Lobby{
    public class Player_Movement : NetworkBehaviour{ // DERIVES FROM MONOBEHAVIOUR ORIGINALLY
        Vector2 previousInput;
        Controls controls;
        Controls Controls{
            get{
                if(controls != null){ return controls; }
                return controls = new Controls();
            }
        }        
        [SyncVar] public float health, maxHealth;
        [SerializeField][Range(0,1)] float healthRegenPercent;
        [SerializeField] float regenCooldown;
        [SyncVar] float actualRegencooldown;
        
    
        [SerializeField] float speed;        
        Vector3 moveVel;

        [SerializeField] Rigidbody2D rB;

        [SerializeField] float speedSmoothing, rotateSmoothing;
        float rotateVel;

        

        
        [ClientCallback] private void OnEnable() => Controls.Enable();
        [ClientCallback] private void OnDisable() => Controls.Disable();

        public override void OnStartAuthority(){
            enabled = true;
            Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
            Controls.Player.Move.canceled += ctx => ResetMovement();    
            GM_UTILITIES.current.UpdateTarget(transform);                  
        }
        
        [Client] void SetMovement(Vector2 move) => previousInput = move;
        [Client] void ResetMovement() => previousInput = Vector2.zero;

        [ClientCallback] void FixedUpdate() => Move();

        [Client] void Move(){
            Vector3 targetPos = transform.position + transform.right * (previousInput.normalized.magnitude * speed * Time.fixedDeltaTime);
            
            if (previousInput != Vector2.zero){
                float angle = Mathf.Atan2(previousInput.y, previousInput.x) * Mathf.Rad2Deg;
                angle = Mathf.SmoothDampAngle(transform.rotation.eulerAngles.z, angle, ref rotateVel, rotateSmoothing);
                Quaternion newRot = Quaternion.AngleAxis(angle, Vector3.forward);

                transform.rotation = newRot;
            }
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref moveVel, speedSmoothing);
        }           


        [ClientCallback] private void Update() {
            if(!hasAuthority){ return; }
            CMDDie();            
            
            if(health < maxHealth){              
                actualRegencooldown -= (actualRegencooldown >0) ? Time.deltaTime : 0;   
                if(actualRegencooldown <= 0){
                    actualRegencooldown = regenCooldown;                
                    CMDHealthRegen(); 
                }
            }
                      
        }
        [Command] void CMDHealthRegen(){
            health = Mathf.Clamp(health + maxHealth * healthRegenPercent, 0, maxHealth);
        }

        [Command] void CMDDie(){
            if(health <= 0){ Destroy(gameObject); }
        }
    }
}