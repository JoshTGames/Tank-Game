using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace JoshsAI{
    public class Chase : Node{
        Transform target;
        FaceTarget turret;
        AIPath agent;
        public Chase(Transform _target, AIPath _agent, FaceTarget _turret){            
            this.target = _target;            
            this.agent = _agent;
            this.turret = _turret;
        }

        public override NodeState Evaluate(){
            if(!target){ return NodeState.FAILURE; }
            turret.target = target;
            if(!agent.reachedEndOfPath){                
                agent.destination = target.position;
                return NodeState.RUNNING;
            }            
            return NodeState.SUCCESS;
        }
    }
}