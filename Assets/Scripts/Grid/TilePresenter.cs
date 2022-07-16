using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePresenter : MonoBehaviour
{
    //This is just a data class
    public Material material;
    public Vector2Int id;

    public Vector3 position(float seperation)
    {
        int even = id.y % 2;
        int evenRow = (id.y - even) % 4 / 2;
        return new Vector3((id.x + (0.5f * even) + (0.5f * evenRow)) * seperation, 0, (id.y - even) * 0.433f) * seperation;
    }

    public Quaternion rotation()
    {
        int even = id.y % 2;
        return Quaternion.Euler(0, 180 * even, 0);
    }
}
