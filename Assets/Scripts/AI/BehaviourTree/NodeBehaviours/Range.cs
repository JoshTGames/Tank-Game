using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshsAI{
    public class Range : Node{
        
        float range;
        Transform target, origin;
        
        
        public Range(float _range, Transform _target, Transform _origin){
            this.range = _range;
            this.target = _target;
            this.origin = _origin;            
        }

        public override NodeState Evaluate(){
            if(!target){ return NodeState.FAILURE; }
            float dist = (target.position - origin.position).magnitude;             
            return (dist <= range)? NodeState.SUCCESS : NodeState.FAILURE; 
        }
    }
}