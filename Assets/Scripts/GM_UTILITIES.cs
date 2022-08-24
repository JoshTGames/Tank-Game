using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GM_UTILITIES: MonoBehaviour{
    public static GM_UTILITIES current;

    public event Action<float[]> UpdateBoundaries;
    public event Action<Transform> UpdateTrackingTarget;
    public event Action<GridGen, GameObject[]> GetRooms;  

    GridGen curGrid;
    GameObject[] curRooms;

    private void Awake() => current = this;

    [SerializeField] GameObject singlePlayerTank, aiTank;
    [SerializeField] float aiQuantity;
    [SerializeField] Transform bulkFolder; 
    public Transform singlePlr;
    bool hasBooted;
    private void Update() {
        if(!singlePlayerTank || hasBooted){ return; }        
        Transform rndRoom = curRooms[UnityEngine.Random.Range(0,curRooms.Length-1)].transform;        
        Transform rndSpawn = rndRoom.Find("Spawns").GetChild(UnityEngine.Random.Range(0, rndRoom.Find("Spawns").childCount-1));

        GameObject newPlr = Instantiate(singlePlayerTank, rndSpawn.position, Quaternion.identity, bulkFolder);
        singlePlr = newPlr.transform;
        Vector3 dir = rndRoom.position - rndSpawn.position;
        newPlr.transform.rotation = Quaternion.FromToRotation(newPlr.transform.right, dir);

        for(int i = 0; i < aiQuantity; i++){
            Transform newRoom = rndRoom;
            while(rndRoom == newRoom){ newRoom = curRooms[UnityEngine.Random.Range(0, curRooms.Length - 1)].transform; }

            rndSpawn = newRoom.Find("Spawns").GetChild(UnityEngine.Random.Range(0, newRoom.Find("Spawns").childCount - 1));
            GameObject newAI = Instantiate(aiTank, rndSpawn.position, Quaternion.identity, bulkFolder);
            dir = newRoom.position - rndSpawn.position;
            newAI.transform.GetChild(0).rotation = Quaternion.FromToRotation(newAI.transform.GetChild(0).right, dir);
        }
        
        hasBooted = true;
    }



    public void UpdateCamBoundaries(float[] boundaries){
        UpdateBoundaries?.Invoke(boundaries);       
    }

    public void UpdateTarget(Transform target) => UpdateTrackingTarget?.Invoke(target);

    public void UpdateRooms(GridGen grid, GameObject[] rooms) { 
        GetRooms?.Invoke(grid, rooms);

        curGrid = grid;
        curRooms = rooms;
    }
}
