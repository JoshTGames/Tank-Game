using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

namespace PopEm.Multiplayer.Lobby{
    public class PLR_MOVE_SINGLEPLR : MonoBehaviour{ // DERIVES FROM MONOBEHAVIOUR ORIGINALLY
        Vector2 previousInput;
        Controls controls;
        Controls Controls{
            get{
                if(controls != null){ return controls; }
                return controls = new Controls();
            }
        }

        bool isPaused;

        
        public float maxHealth, health;
        [SerializeField][Range(0,1)] float healthRegenPercent;
        [SerializeField] float regenCooldown;
        float actualRegencooldown;
        public float Health{
            get{ return health; }
            set{ health = Mathf.Clamp(value, 0, maxHealth); }
        }
        [SerializeField] float speed;        
        Vector3 moveVel;

        [SerializeField] Rigidbody2D rB;

        [SerializeField] float speedSmoothing, rotateSmoothing;
        float rotateVel;

        

        
        private void OnEnable() => Controls.Enable();
        private void OnDisable() => Controls.Disable();

        void Start(){
            enabled = true;
            health = maxHealth;
            Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
            Controls.Player.Move.canceled += ctx => ResetMovement();

            GM_UTILITIES.current.UpdateTarget(transform);
        }
        
        void SetMovement(Vector2 move) => previousInput = move;
        void ResetMovement() => previousInput = Vector2.zero;

        void FixedUpdate() => Move();

        void Move(){
            if (isPaused) { return; }

            Vector3 targetPos = transform.position + transform.right * (previousInput.normalized.magnitude * speed * Time.fixedDeltaTime);
            
            if (previousInput != Vector2.zero){
                float angle = Mathf.Atan2(previousInput.y, previousInput.x) * Mathf.Rad2Deg;
                angle = Mathf.SmoothDampAngle(transform.rotation.eulerAngles.z, angle, ref rotateVel, rotateSmoothing);
                Quaternion newRot = Quaternion.AngleAxis(angle, Vector3.forward);

                transform.rotation = newRot;
            }
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref moveVel, speedSmoothing);
            rB.velocity = Vector2.zero;
        }     


        void Update(){
            if(Health <= 0){ Destroy(gameObject); }

            #region HEALTH RESTORATION
            if(Health < maxHealth){
                actualRegencooldown -= (actualRegencooldown >0) ? Time.deltaTime : 0;
                if(actualRegencooldown <= 0){
                    actualRegencooldown = regenCooldown;                
                    Health += maxHealth * healthRegenPercent;
                }
            }
            #endregion
        }
    }
}