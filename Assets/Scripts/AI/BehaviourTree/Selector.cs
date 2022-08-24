using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshsAI{
    public class Selector : Node{
        protected List<Node> nodes = new List<Node>();

        public Selector(List<Node> _nodes){ this.nodes = _nodes; }
        public override NodeState Evaluate(){
            
            foreach(Node node in nodes){
                switch(node.Evaluate()){
                    case NodeState.RUNNING:                        
                        return NodeState.RUNNING;
                    case NodeState.SUCCESS:
                        return NodeState.SUCCESS;
                    case NodeState.FAILURE:                        
                        break;                      
                    default:
                        break;
                }
            }            
            return NodeState.FAILURE;
        }
    }
}