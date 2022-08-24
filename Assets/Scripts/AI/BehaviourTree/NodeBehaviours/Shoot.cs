using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace JoshsAI{
    public class Shoot : Node{
        Transform target;
        FaceTarget turret;
        SpiderMechAI ai;

        public Shoot(Transform _target, SpiderMechAI _ai, FaceTarget _turret){            
            this.target = _target;              
            this.turret = _turret;
            this.ai = _ai;            
        }

        public override NodeState Evaluate(){   
            if(!target){ return NodeState.FAILURE; }         
            turret.target = target;            
            return ai.Fire();
        }
    }
}