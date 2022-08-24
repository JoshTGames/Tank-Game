using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshsAI{
    public class Sequence : Node{
        protected List<Node> nodes = new List<Node>();

        public Sequence(List<Node> _nodes){ this.nodes = _nodes; }
        public override NodeState Evaluate(){
            bool isAnyRunning = false;
            foreach(Node node in nodes){
                switch(node.Evaluate()){
                    case NodeState.RUNNING:
                        isAnyRunning = true;
                        break;
                    case NodeState.SUCCESS:
                        break;
                    case NodeState.FAILURE:                        
                        return NodeState.FAILURE;                        
                    default:
                        break;
                }
            }            
            return (isAnyRunning)? NodeState.RUNNING : NodeState.SUCCESS;
        }
    }
}