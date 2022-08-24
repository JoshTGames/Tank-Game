using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWall : MonoBehaviour{
    [SerializeField] GameObject[] objs;
    [SerializeField][Range(0,1)] float chanceToSpawn;
    private void Start(){
        float luckyDip = (float)Random.Range(0,100)/100;
        
        if(luckyDip <= chanceToSpawn){
            Instantiate(objs[Random.Range(0, objs.Length)], transform.position, Quaternion.identity, transform);
        }        
    }
}
