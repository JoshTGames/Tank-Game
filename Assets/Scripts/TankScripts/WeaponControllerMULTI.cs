using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WeaponControllerMULTI : NetworkBehaviour{
    [SerializeField] float minDistance, maxDistance;
    [SerializeField] Transform cannon;
    Vector3 curPos;
    float prvDist;
    public float attackCooldown; // Placeholder value
    [HideInInspector] public float cooldown; // actual value


    [SerializeField] GameObject ammunition;
    // PLAYER SETTINGS
    [SerializeField] bool isPlr, debugMode;
    Transform crosshair;
    Camera cam;
    [SerializeField] Transform ammoSpawnPoint;
    [SerializeField] float ammoLifetime;
    [SerializeField] Transform bulletFolder;    
    float rotateVel;  

    public override void OnStartAuthority(){
            enabled = true;

            curPos = cannon.position;
            if (!isPlr) { return; }
            crosshair = GameObject.Find("UI").transform.GetChild(0);
            cam = Camera.main;

            if (!bulletFolder) { bulletFolder = GameObject.Find("BulkFolder").transform; }   
        }
 
    [ClientCallback] private void Update() {              
        if(!cam){ return; }
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 mousePosWorld = cam.ScreenToWorldPoint(mousePos);
        Vector3 newMousePos = new Vector3(mousePosWorld.x, mousePosWorld.y);
        
        curPos = GetInterpolatedTargetPosition(newMousePos);
        if (crosshair) { crosshair.position = curPos; }

        RotateTurret(newMousePos);
        ReduceCooldown();
        if (Input.GetMouseButtonDown(0) && cooldown <=0 && !PauseMenuHandler.isPaused){   
            cooldown = attackCooldown;
            CMDSpawnAmmo(curPos);
        }       
    }    
    
    [Client] void RotateTurret(Vector3 mousePos){
        Vector3 direction = mousePos - cannon.parent.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = Mathf.SmoothDampAngle(cannon.parent.rotation.eulerAngles.z, angle, ref rotateVel, smoothingSpeed);        
        Quaternion newRot = Quaternion.AngleAxis(angle, Vector3.forward);      
        
        cannon.parent.rotation = newRot;  
    }
    
    [Client] void ReduceCooldown(){ cooldown -= (cooldown > 0) ? Time.deltaTime : 0; }

    [Command] void CMDSpawnAmmo(Vector3 pos){
        GameObject ammo = Instantiate(ammunition, ammoSpawnPoint.position, Quaternion.identity, bulletFolder);
        ammo.transform.rotation = Quaternion.FromToRotation(ammo.transform.right, ammoSpawnPoint.parent.right);
        NetworkServer.Spawn(ammo); 
        ProjectileControllerMULTI pCM = ammo.GetComponent<ProjectileControllerMULTI>();
        pCM.SetVectors(pos);         
        pCM.owner = transform;   
        
        Destroy(ammo, t: ammoLifetime);
    }
    
    [SerializeField] float smoothingSpeed;
    float posVel;
    [Client] Vector3 GetInterpolatedTargetPosition(Vector3 pos){
        float dist = (pos - cannon.position).magnitude;
        dist = Mathf.Clamp(dist, minDistance, maxDistance);
        dist = Mathf.SmoothDamp(prvDist, dist, ref posVel, smoothingSpeed);
        prvDist = dist;
        return cannon.position + cannon.right * dist;
    } 

    private void OnDrawGizmosSelected(){
        if (!cannon || Application.isPlaying || !debugMode) { return; }
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(cannon.position + new Vector3(minDistance, 0, 0), .2f);
        Gizmos.DrawSphere(cannon.position + new Vector3(maxDistance,0,0), .2f);
    }   
}
