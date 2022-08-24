using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponController : MonoBehaviour{
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

 
    private void Start(){    
        curPos = cannon.position;
        if (!isPlr) { return; }
        crosshair = GameObject.Find("UI").transform.GetChild(0);
        cam = Camera.main;

        if (!bulletFolder) { bulletFolder = GameObject.Find("BulkFolder").transform; }
    }     
    private void Update() {    
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 mousePosWorld = cam.ScreenToWorldPoint(mousePos);
        Vector3 newMousePos = new Vector3(mousePosWorld.x, mousePosWorld.y);       

        curPos = GetInterpolatedTargetPosition(newMousePos);
        if (crosshair) { crosshair.position = curPos; }

        cooldown -= (cooldown > 0) ? Time.deltaTime : 0;
        if (Input.GetMouseButtonDown(0) && cooldown <=0 && !PauseMenuHandler.isPaused){
           
            cooldown = attackCooldown;
            GameObject ammo = Instantiate(ammunition, ammoSpawnPoint.position,Quaternion.identity, bulletFolder);
            ammo.transform.rotation = Quaternion.FromToRotation(ammo.transform.right, ammoSpawnPoint.parent.right);
            ammo.GetComponent<ProjectileController>().SetVectors(curPos);
            
            Destroy(ammo, t: ammoLifetime);
        }
    }

    [SerializeField] float smoothingSpeed;
    float posVel;
    Vector3 GetInterpolatedTargetPosition(Vector3 pos){
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
