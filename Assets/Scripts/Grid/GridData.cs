using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GridData", menuName = "ScriptableObjects/GridData", order = 1)]
public class GridData : ScriptableObject
{
    //TODO :: FloodFill Defined shape with Triangles and Squares
    public Vector2Int GridDimensions;

    public Color primary, secondary;

    //We will still end up with a mostly rectangular grid
    //[X] [Y] [Z]
    public Dictionary<Vector2Int, GridObject> triangleGrid;

    public void Generate()
    {
        triangleGrid = new Dictionary<Vector2Int, GridObject>();
        
        for (int i = 0; i < GridDimensions.x; i++) {
            for (int j = 0; j < GridDimensions.y; j++) {
                triangleGrid.Add(new Vector2Int(i, j), new GridObject(ObjectType.empty));
            }   
        }
    }

    //TODO :: CHANGE TO VECTOR2INT
    public Vector3Int[] neighbors(Vector3Int a)
    {
        int pointsUp = (a.x + a.y + a.z) == 2 ? -1 : 1;
        List<Vector3Int> rval = new List<Vector3Int>();
        
        rval.Add(new Vector3Int(a.x + pointsUp, a.y, a.z));
        rval.Add(new Vector3Int(a.x, a.y + pointsUp, a.z));
        rval.Add(new Vector3Int(a.x, a.y, a.z + pointsUp));
        
        //This way cause I'm just kinda lazy
        //There is def a better way ¯\_(ツ)_/¯
        for (int i = 0; i < 3; i++)
        {
            if (!isValid(rval[i]))
                rval.Remove(rval[i]);
        }

        return rval.ToArray();
    }

    public bool isValid(Vector3Int coords)
    {
        //For now only check triangle grids
        if (GridDimensions.x > coords.x || coords.x > 0) {
            return false; //Out of Bounds
        }

        if (GridDimensions.y > coords.y || coords.y > 0) {
            return false; //out of bounds
        }
        return true;
    }

    public bool checkCollision(Vector2Int a, Vector2Int b)
    {
        return false;
    }

}
