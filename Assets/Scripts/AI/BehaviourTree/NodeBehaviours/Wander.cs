using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace JoshsAI{
    public class Wander : Node{
        GameObject[] rooms;
        bool isRunning;
        Vector3 rndPos;
        AIPath agent;
        FaceTarget turret;
        public Wander(GameObject[] _rooms, AIPath _agent, FaceTarget _turret){            
            this.rooms = _rooms;            
            this.agent = _agent;
            this.turret = _turret;
        }



        Vector3 GetPos(){
            Transform spawns = rooms[UnityEngine.Random.Range(0, rooms.Length-1)].transform.Find("Spawns");
            Transform rndSpawn = spawns.GetChild(UnityEngine.Random.Range(0,spawns.childCount-1));
            return rndSpawn.position;
        }

        public override NodeState Evaluate(){
            turret.target = null;
            if(!isRunning){
                isRunning = true;
                rndPos = GetPos();
            }

            if(agent.reachedEndOfPath){ rndPos = GetPos(); }          
            agent.destination = rndPos;              
            return NodeState.RUNNING; 
        }
    }
}