using UnityEngine;
using System.Collections.Generic;

public class terraingenerator : MonoBehaviour
{
    public GameObject tilePrefab;
    public float tileWidth = 5f;
    public int visibleTiles = 8;
    public float scrollSpeed = 3f;
    public float groundY = -3.5f;

    private List<GameObject> tiles = new List<GameObject>();

    void Start()
    {
        float z = 0f;
        for (int i = 0; i < visibleTiles; i++)
        {
            GameObject t = Instantiate(tilePrefab, new Vector3(0, groundY, z), Quaternion.identity);
            tiles.Add(t);
            z += tileWidth; 
        }
    }

    void Update()
    {
        foreach (GameObject t in tiles)
            t.transform.position += Vector3.back * scrollSpeed * Time.deltaTime;

        GameObject first = tiles[0];
        if (first.transform.position.z < -tileWidth)
        {
            GameObject last = tiles[tiles.Count - 1];
            first.transform.position = new Vector3(0, groundY, last.transform.position.z + tileWidth);
            tiles.RemoveAt(0);
            tiles.Add(first);
        }
    }
}
