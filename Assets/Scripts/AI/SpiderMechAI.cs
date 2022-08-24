using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Pathfinding;

namespace JoshsAI{
    public class SpiderMechAI : MonoBehaviour{

    #region HEALTH SETTINGS
        [Serializable] public class HealthSettings{
            public float maxHealth, health;

            [Range(0,100)] public int lowHealthPercentage, restoreHealthPercentage;
            public float healthRestoreCooldown;
            [HideInInspector] public float curCooldown;
        }
        public HealthSettings healthSettings;
        public float Health{ 
            get{ return healthSettings.health; } 
            set{ healthSettings.health = Mathf.Clamp(value, 0, healthSettings.maxHealth);}
        }
        #endregion

        public float chasingRange, attackingRange;
        [SerializeField] Animator animator;
        [SerializeField] Rigidbody2D rB;
        [SerializeField] FaceTarget turret;

        [SerializeField] Transform tempTarget;
        [SerializeField] Transform[] barrels;
        [SerializeField] GameObject ammo;
        public float attackCooldown;
        [HideInInspector] public float curCooldown;

        [SerializeField] Transform bulletFolder;
        [SerializeField] LayerMask whatIsEnemy;
        Vector3 prvPos;
                
        Node topNode;


        private void Start() {
            healthSettings.health = healthSettings.maxHealth;
            bulletFolder = GameObject.Find("BulkFolder").transform;
            StartCoroutine(StartAI());
        }

        void CreateBehaviourTree(AIPath ai){                       
            #region NODE TYPES
            Health healthNode = new Health(this, healthSettings.lowHealthPercentage);

            IsCovered isCoveredNode = new IsCovered(tempTarget, transform);
            Inverter notCovered = new Inverter(isCoveredNode);

            Chase chase = new Chase(tempTarget, ai, turret);
            Range chaseRange = new Range(chasingRange, tempTarget, transform);

            Shoot shoot = new Shoot(tempTarget, this, turret);
            Range attackRange = new Range(attackingRange, tempTarget, transform);
            Wander wander = new Wander(Recursive_Backtracker.rooms, ai, turret);
            #endregion
            Sequence chaseSequence = new Sequence(new List<Node>(){ chaseRange, notCovered, chase });
            Sequence attackSequence = new Sequence(new List<Node>() { attackRange, notCovered, shoot });
            topNode = new Selector(new List<Node>{ attackSequence, chaseSequence, wander });
        }



        IEnumerator StartAI(){
            yield return new WaitUntil(()=> GM_UTILITIES.current.singlePlr);            
            tempTarget = GM_UTILITIES.current.singlePlr;
            CreateBehaviourTree(GetComponent<AIPath>());
            StopCoroutine(StartAI());
        }


        public float GetHealth(){ return Health; }

        private void FixedUpdate(){
            #region VELOCITY CALCULATION
            Vector3 dir = (transform.position - prvPos); // FINDS THE DISTANCE BETWEEN ITS PREVIOUS POSITION AND CURRENT POSITION
            float vel = Mathf.Abs(dir.normalized.magnitude); // NORMALISES THE VALUE TO FIND THE PURE DIRECTION OF THE DISTANCE MADE
            animator.SetFloat("Move", vel); // SETS THE ANIMATION VALUE TO THE VELOCITY
            prvPos = transform.position;

            turret.targetPos = (!turret.HasTarget)? transform.position + dir.normalized : turret.targetPos; // CONTINUOUSLY UPDATES THE TURRET TANK POSITION SO THAT IT FACES THE DIRECTION IT IS MOVING TOWARDS
            #endregion
            rB.velocity = Vector2.zero; // THIS IS INPLACE TO STOP RIGIDBODY SLIDING
        }
        private void Update() {            
            if (topNode == null) { return; } // If this does not exist, the AI has not generated and no need to perform any calculations.
            #region HEALTH RESTORATION
            if(Health < healthSettings.maxHealth){ // Will keep improving the health of the AI over a duration of time
                healthSettings.curCooldown -= (healthSettings.curCooldown >0) ? Time.deltaTime : 0;
                if(healthSettings.curCooldown <= 0){
                    healthSettings.curCooldown = healthSettings.healthRestoreCooldown;                
                    healthSettings.health = Mathf.Clamp(Health + ((healthSettings.maxHealth / healthSettings.restoreHealthPercentage)), 0, healthSettings.maxHealth);
                }
            }
            #endregion

            if (Health <= 0) { // Player death handling
                Destroy(transform.parent.gameObject);
            }

            curCooldown -= (curCooldown > 0) ? Time.deltaTime : 0; // Cooldown manager - If the value is greater than 0, it'll decrease over time.
            
            
            switch(topNode.Evaluate()){ // THIS CALLS THE FUNCTION WHICH INITIATES THE AI, IF THE TOP NODE FAILS, IT MEANS NO DECISION HAS BEEN MADE
                case NodeState.SUCCESS:                                     
                    break;
                case NodeState.FAILURE:
                    Debug.Log($"Agent {transform.name}: has no decisions");
                    break;
                case NodeState.RUNNING:
                    break;
            }
        }

        int barrelIndex;
        public NodeState Fire(){
            if(curCooldown > 0) { return NodeState.FAILURE; } // THIS WILL FAIL THE NODE SO THAT THE SEQUENCE MAY NOT BE FULFILLED
            GameObject clonedAmmo = Instantiate(ammo, barrels[barrelIndex].position, Quaternion.identity, bulletFolder);
            clonedAmmo.transform.rotation = Quaternion.FromToRotation(clonedAmmo.transform.right, barrels[barrelIndex].parent.right);

            ProjectileController pC = clonedAmmo.GetComponent<ProjectileController>();
            pC.owner = transform;
            pC.whatIsEnemy = whatIsEnemy;
            pC.SetVectors(tempTarget.position); // THIS SETS THE MAX POSITION THIS PROJECTILE WILL GO BEFORE DETONATION
            
            barrelIndex = (barrelIndex + 1) % barrels.Length; // USING A MODULUS FUNCTION TO ITERATE THROUGH EACH TURRET

            Destroy(clonedAmmo, t: 5f);

            curCooldown = attackCooldown;
            return NodeState.SUCCESS;
        }
    }
}