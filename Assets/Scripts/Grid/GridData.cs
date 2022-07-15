using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GridData", menuName = "ScriptableObjects/GridData", order = 1)]
public class GridData : ScriptableObject
{
    public struct Triangle
    {
        //This is a reference to the original gridData
        //Changing this with change for all
        public GridData gridData;
        public Vector3Int position;
        
        public Triangle[] neighbors
        {
            get
            {
                return null;
            }
        }
    }


    //TODO :: FloodFill Defined shape with Triangles and Squares
    public Vector3Int GridDimensions;
    public float TriangleWidth;

    
    //We will still end up with a mostly rectangular grid
    //[X] [Y] [Z]
    public Triangle[,,] triangleGrid;

    public void GenerateGrid()
    {
        //TODO :: DO SOMETHING!!!!!
        triangleGrid = new Triangle[GridDimensions.x,GridDimensions.y,GridDimensions.z];

    }

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

        if (GridDimensions.z > coords.z || coords.z > 0) {
            return false; //Out of bounds
        }
        return true;
    }

}
