using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoshsAI{
    public abstract class Node{
    protected NodeState state;
    public NodeState State{ get { return state; } }

    public abstract NodeState Evaluate();
    }

    public enum NodeState{
        FAILURE,
        SUCCESS,
        RUNNING
    }
}