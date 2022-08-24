using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Recursive_Backtracker : MonoBehaviour{
    public GameObject cellWall;

    [Serializable] public class GridSettings{
        public Vector2Int gridSize;
        public float cellSize;
    }
    [SerializeField] GridSettings settings;
    public GridSettings Settings { get { return settings; } }

    public static GameObject[] rooms;
    GridGen grid;
    [SerializeField] bool createMaze;
    bool hasRan, createNavMesh;
    private void Update(){
        if (createMaze && createNavMesh){
            createNavMesh = false;            
            AstarPath.active.Scan(AstarPath.active.data.gridGraph);
        }

        if (hasRan) { return; }
        hasRan = true;
        createNavMesh = true;

        grid = new GridGen(settings.gridSize, settings.cellSize);
        if (createMaze){
            #region SPAWNING DEFAULT ROOMS
            rooms = new GameObject[grid.nodes.Length];
            for (int i = 0; i < rooms.Length; i++){
                rooms[i] = Instantiate(cellWall, grid.GetWorldPos(grid.nodes[i].position.x, grid.nodes[i].position.y), Quaternion.identity, transform);
            }
            #endregion
        
            RecursiveBacktrack(grid, UnityEngine.Random.Range(0, grid.nodes.Length-1));
        }        
        
        SetBoundaries();
        GM_UTILITIES.current.UpdateRooms(grid, rooms);             
    }

    public void SetBoundaries(){
        if (!createMaze){            
            rooms = new GameObject[grid.nodes.Length];
            for (int i = 0; i < rooms.Length; i++){                
                rooms[i] = transform.GetChild(i).gameObject;               
            }
        }

        int bottomLeftRoom = 0;
        int topRightRoom = grid.nodes.Length - 1;
        float[] boundaries = new float[4];
        boundaries[0] = rooms[bottomLeftRoom].transform.Find("BottomLeft").position.x;
        boundaries[1] = rooms[topRightRoom].transform.Find("TopRight").position.x;
        

        boundaries[2] = rooms[bottomLeftRoom].transform.Find("BottomLeft").position.y;
        boundaries[3] = rooms[topRightRoom].transform.Find("TopRight").position.y;
        
        GM_UTILITIES.current.UpdateCamBoundaries(boundaries);
    }
    
   




    Stack<int> visitedRooms = new Stack<int>();
    void RecursiveBacktrack(GridGen _grid, int curNode = 0){
        
        List<int> neighbours = GetNeighbours(_grid, _grid.nodes[curNode]); // LIST HOLDING ALL BRANCHABLE NEIGHBOURS
        if(neighbours.Count <= 0){ // BACK-TRACKING TO A PREVIOUS NODE TILL IT FINDS A NODE WITH AN AVAILABLE NEIGHBOUR...
            if (visitedRooms.Count <= 0) { return; } // IF STACK IS EMPTY, THEN THE ALGORITHM HAS COMPLETE
            int node = visitedRooms.Pop(); 
            RecursiveBacktrack(_grid, node); // CALL THIS FUNCTION FROM A PREVIOUS POINT
        }
        else{
            visitedRooms.Push(curNode); // ADDS NODE TO THE STACK
            int rndNeighbour = neighbours[UnityEngine.Random.Range(0, neighbours.Count)]; // BRANCH TO A RANDOM NEIGHBOUR GIVEN FROM THE LIST
            _grid.nodes[rndNeighbour].parent = _grid.nodes[curNode]; // SET THE PARENT OF THE NODE. THIS IS IMPORTANT TO MAKE SURE IT ISN'T RE-ACCESSED
            // CREATE OPENINGS BETWEEN BOTH ROOMS
            Vector2 direction = (_grid.nodes[rndNeighbour].position - _grid.nodes[curNode].position);
            ClearWall(curNode, direction);
            ClearWall(rndNeighbour, -direction);            
            RecursiveBacktrack(_grid, rndNeighbour); // RE-CALL THIS FUNCTION WITH THE NEIGHBOUR NODE
        }        
    }

    ///<summary>GET AVAILABLE SURROUNDING NEIGHBOURS FROM A GIVEN NODE</summary>
    List<int> GetNeighbours(GridGen _grid, GridGen.Node node){
        List<int> neighbours = new List<int>();
        int leftNeighbour = _grid.GetNodeIndex(node.position.x - 1, node.position.y, _grid.GetDimensions().x); // LEFT
        int rightNeighbour = _grid.GetNodeIndex(node.position.x + 1, node.position.y, _grid.GetDimensions().x); // RIGHT
        int upNeighbour = _grid.GetNodeIndex(node.position.x, node.position.y + 1, _grid.GetDimensions().x); // UP
        int downNeighbour = _grid.GetNodeIndex(node.position.x, node.position.y - 1, _grid.GetDimensions().x); // DOWN        

        neighbours.Add(leftNeighbour);
        neighbours.Add(rightNeighbour);
        neighbours.Add(upNeighbour);
        neighbours.Add(downNeighbour);
        
        for (int x = neighbours.Count-1; x >=0; x--){  
            if ((neighbours[x] > _grid.nodes.Length - 1 || neighbours[x] < 0) || _grid.nodes[neighbours[x]].parent != null) { neighbours.RemoveAt(x); } // MAKES SURE NO ANOMYLIES GET PASSED INTO LIST
        }        
        return neighbours;
    }

    /// <summary>CLEARS A WALL ON A GIVEN ROOM</summary>
    void ClearWall(int roomIndex, Vector2 direction){
        Direction dir = Direction.Left;
        if (direction == new Vector2(-1, 0)) { dir = Direction.Left; }
        else if (direction == new Vector2(1, 0)) { dir = Direction.Right; }
        else if (direction == new Vector2(0, 1)) { dir = Direction.Up; }
        else if (direction == new Vector2(0, -1)) { dir = Direction.Down; }

        Destroy(rooms[roomIndex].transform.GetChild((int)dir).gameObject); 
    }
    public enum Direction{
        Left = 0,
        Right = 1,
        Up = 2,
        Down = 3
    }    
}
