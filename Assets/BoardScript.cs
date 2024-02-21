using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardScript : MonoBehaviour
{
    public GameObject TilePrefab;
    public float Gap;
    public float Size;
    public int X;
    public int Z;
    // Start is called before the first frame update
    void Start()
    {
        MakeBoard(X, Z);
    }

    // Update is called once per frame
    void Update()
    {

    }
    //Creates board
    void MakeBoard(int x, int z)
    {
        float midx = ((x-1) * Size + (x-1) * Gap) / 2;
        float midz = ((z-1) * Size + (z-1) * Gap) / 2;
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < z; j++)
            {
                Vector3 coordinates = new Vector3(i*Size+i*Gap-midx, 0, j*Size+j*Gap-midz);
                GameObject tile = Instantiate(TilePrefab, coordinates, Quaternion.identity);
                tile.transform.parent = transform;
                tile.name = "Tile_" + i.ToString() + "_" + j.ToString();
            }
        }
    }
}
