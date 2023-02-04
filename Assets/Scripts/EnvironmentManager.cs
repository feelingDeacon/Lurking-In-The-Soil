using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    private static EnvironmentManager _instance;

    public static EnvironmentManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<EnvironmentManager>();
            }

            return _instance;
        }
    }

    private int widthAmount = 180;
    private int heightAmount = 180;
    private float centerOffsetX => - widthAmount / 2 + 0.5f;
    private float centerOffsetY => - heightAmount / 2;

    public List<List<Pixel>> tilemap;

    public GameObject pixelGameObject;

    public void Init()
    {
        SpawnGrid();
    }

    private void SpawnGrid()
    {
        tilemap = new List<List<Pixel>>();
        for (int i = 0; i < widthAmount; i++)
        {
            List<Pixel> currCol = new List<Pixel>();
            for (int j = 0; j < heightAmount; j++)
            {
                Pixel newPixel = Instantiate(pixelGameObject, new Vector3(i + centerOffsetX, j + centerOffsetY, 0),
                    Quaternion.identity, transform).GetComponent<Pixel>();
                currCol.Add(newPixel);
                Color x = Random.ColorHSV();
                x.a = 1;
                newPixel.SetColor(x);
            }
            tilemap.Add(currCol);
        }
    }

    public void UpdateManager()
    {
        
    }
}
