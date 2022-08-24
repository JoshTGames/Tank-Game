using UnityEngine;

public class GridGen{
    #region VARIABLES
    public class Node{
        public Vector2Int position = Vector2Int.zero;
        public bool isUsable = true;
        public Node parent;

        public Node(Vector2Int pos, bool isUsable = true, Node parent = null){
            this.position = pos;
            this.isUsable = isUsable;
            this.parent = parent;
        }
    }

    Vector2Int dimensions;
    float cellSize;


    public Node[] nodes;
    #endregion

    public GridGen(Vector2Int dimensions, float cellSize){
        #region VARIABLE ASSIGNMENT
        this.dimensions = dimensions;
        this.cellSize = cellSize;

        nodes = new Node[dimensions.x * dimensions.y];
        #endregion        

        for (int x = 0; x < dimensions.x; x++){
            for (int y = 0; y < dimensions.y; y++){
                nodes[GetNodeIndex(x, y, dimensions.x)] = new Node(new Vector2Int(x, y));                
            }
        }
    }
    

    #region FUNCTIONS    
    /// <summary> RETURNS WORLD POSITION FROM A GRID AFTER NODE HAS BEEN CREATED -- THE PASSED VALUE NEEDS TO BE THE EXACT NODE POSITION </summary>
    public Vector2 GetWorldPos(int x, int y) { return new Vector2(x, y) * cellSize; }
    /// <summary> THIS RETURNS THE EXACT NODE POSITION CLOSEST TO THE GIVEN VECTOR </summary>
    public Vector2 GetNodeWorldPosition(Vector2 rawPosition){
        Vector2 min = nodes[0].position, max = nodes[nodes.Length - 1].position;
        rawPosition = new Vector2(Mathf.Clamp(rawPosition.x / 10, min.x, max.x), Mathf.Clamp(rawPosition.y / 10, min.y, max.y));
        return new Vector2(rawPosition.x / cellSize, rawPosition.y / cellSize);
    }
    /// <summary> RETURNS THE INDEX A GIVEN NODE IS LOCATED BASED ON THE POSITION PASSED </summary>
    public int GetNodeIndex(int x, int y, int width) {        
        if((x < 0 || x > GetDimensions().x-1) || (y < 0 || y > GetDimensions().y-1)){ return -1; }
        return x + width * y; 
    } // TREATS THE 1D ARRAY AS A 2D GRID AND RETURNS THE INDEX AT WHICH MATCHES A GIVEN POSITION

    public Vector2Int GetDimensions() { return dimensions; }
    public float GetCellSize() { return cellSize; }
    #endregion    
}