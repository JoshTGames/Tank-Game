using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshsAI{
    public class IsCovered : Node{
        Transform target, origin;        
        public IsCovered(Transform _target, Transform _origin){            
            this.target = _target;
            this.origin = _origin;           
        }

        public override NodeState Evaluate(){
            if(!target){ return NodeState.FAILURE; }
            RaycastHit2D hit = Physics2D.Raycast(origin.position + ((target.position - origin.position).normalized / 2), target.position - origin.position);            
            
            if (hit.transform && hit.collider.transform != target){                
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }
}