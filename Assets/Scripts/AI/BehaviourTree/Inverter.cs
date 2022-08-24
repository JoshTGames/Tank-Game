using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshsAI{
    public class Inverter : Node{
        protected Node node;

        public Inverter(Node _node){ this.node = _node; }
        public override NodeState Evaluate(){
            switch(node.Evaluate()){
                case NodeState.RUNNING:                        
                    return NodeState.RUNNING;
                case NodeState.SUCCESS:
                    return NodeState.FAILURE;
                case NodeState.FAILURE:                        
                    return NodeState.SUCCESS;                   
                default:
                    break;
            }
            return NodeState.FAILURE;
        }
    }
}