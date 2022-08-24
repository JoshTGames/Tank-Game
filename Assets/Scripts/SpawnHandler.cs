using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHandler : MonoBehaviour{
    GridGen grid;
    GameObject[] rooms;


    private void Start(){ 
        GM_UTILITIES.current.GetRooms += GetGridRooms;
    }


    public void SpawnCharacters(){

    }

    void GetGridRooms(GridGen _grid, GameObject[] _rooms){        
        grid = _grid;
        rooms = _rooms;
    }
}
