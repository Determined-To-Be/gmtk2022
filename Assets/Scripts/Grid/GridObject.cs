using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public enum ObjectType {
    empty,
    dice,
    wall
}
public class GridObject
{
    public ObjectType type;
    public Color color;

    public GridObject(ObjectType type)
    {
        this.type = type;
        this.color = Color.HSVToRGB(Mathf.PerlinNoise(Random.Range(0, 1), 0), 1, 1);
    }

}