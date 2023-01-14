using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Area
{
    
    private readonly GameObject Ground = GameObject.Find("Ground").gameObject;

    public List<GameObject> currentAnimals;
    public GameObject Tile { get; set; }

    private GameObject m_element;
    public GameObject Element
    {
        get
        {
            return m_element;
        }
        set
        {
            if (value != null)
            {
                m_element = value;
                m_element.transform.parent = Tile.transform;
            }
        }
    }

    public AreaType Type;

    public Area(int i, int j)
    {
        currentAnimals = new List<GameObject>();
        CreateTile(i, j);
    }
    private void CreateTile(int i, int j)
    {
        Tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Tile.transform.localScale = new Vector3(0.1f, 1, 0.1f);
        Tile.transform.parent = Ground.transform;
        Tile.transform.position = new Vector3(i, 0, j);
    }
}

public class GrassArea : Area
{
    private static int grassTileCount = 0;
    Material mat = GameObject.Find("Engine").GetComponent<Layout>().materials.FirstOrDefault(x => x.name == "Grass");
    public GrassArea(int i, int j) : base(i, j)
    {
        Tile.name = "Grass "+grassTileCount;
        Tile.layer = 3;
        Type = AreaType.Grass;
        Tile.GetComponent<MeshCollider>().enabled = false;
        Tile.GetComponent<Renderer>().material = mat;
        grassTileCount++;
    }
}

public class WaterArea : Area
{
    private static int waterTileCount = 0;
    Material mat = GameObject.Find("Engine").GetComponent<Layout>().materials.FirstOrDefault(x => x.name == "Water");
    public WaterArea(int i, int j) : base(i, j)
    {
        Tile.name = "Water "+ waterTileCount;
        Type = AreaType.Water;
        Tile.layer = 4;
        Tile.GetComponent<Renderer>().material = mat;
        waterTileCount++;
    }
}


public enum AreaType
{
    Water,
    Food,
    Obstacle,
    Grass
}
