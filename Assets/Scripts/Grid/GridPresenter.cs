using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPresenter : MonoBehaviour
{
    public float triangleSideLength;
    public bool resetMesh = false;
    public Material tileMaterial;

    public GridModel model;

    private Dictionary<Vector2Int, TilePresenter> tiles;
    private Mesh triangle;

    [Range(1, 1.20f)]
    public float seperation = 1;

    // Start is called before the first frame update
    void Start()
    {
        GenerateMesh();
        model.ResetBoard.AddListener(InitializeFromModel);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGameboard();
    }

    void UpdateGameboard()
    {
        // foreach (KeyValuePair<Vector2Int, GridObject> tile in model.gridData.triangleGrid)
        // {
        //     
        // }

        foreach (TilePresenter ctile in tiles.Values)
        {
            ctile.transform.position = ctile.position(seperation);
            ctile.transform.rotation = ctile.rotation();
        }
    }

    void CleanGameboard()
    {
        if (tiles == null)
            return;

        foreach (TilePresenter tile in tiles.Values) {
            Destroy(tile);
        }
        
        tiles.Clear();
        tiles = null;
    }
    
    void InitializeFromModel(){
        CleanGameboard();
        tiles = new Dictionary<Vector2Int, TilePresenter>();
        foreach (KeyValuePair<Vector2Int, GridObject> tile in model.gridData.triangleGrid)
        {

            GameObject go = new GameObject();
            TilePresenter ctile = go.AddComponent<TilePresenter>();
            ctile.id = tile.Key;
            
            ctile.transform.position = ctile.position(seperation);
            ctile.transform.rotation = ctile.rotation();
            ctile.transform.parent = this.transform;
            ctile.transform.name = tile.Key + "";

            MeshRenderer render = go.AddComponent<MeshRenderer>();
            MeshFilter filter = go.AddComponent<MeshFilter>();
            filter.mesh = triangle;
            render.material = tileMaterial;
            ctile.material = render.material;
            ctile.material.color = tile.Value.color;

            
            tiles.Add(tile.Key, ctile);
        }
    }

    void GenerateMesh()
    {
        Vector3[] verts = new[]
        {
            new Vector3(0.5f, 0, -0.433f),
            new Vector3(-0.5f, 0, -0.433f),
            new Vector3(0, 0, 0.433f)
        };

        triangle = new Mesh();

        triangle.vertices = verts;
        triangle.triangles = new int[] { 0, 1, 2 };
        triangle.normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up };
    }
}
