using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshsAI{
    public class Health : Node{
        SpiderMechAI ai;
        float threshold;
        
        public Health(SpiderMechAI _ai, float _threshold){
            this.ai = _ai;
            this.threshold = _threshold;
        }

        public override NodeState Evaluate(){ return (ai.GetHealth() <= threshold) ? NodeState.SUCCESS : NodeState.FAILURE; }
    }
}