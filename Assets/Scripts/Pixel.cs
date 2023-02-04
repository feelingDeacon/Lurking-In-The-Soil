using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixel : MonoBehaviour
{
    public SpriteRenderer renderer;

    public void SetColor(Color color)
    {
        renderer.color = color;
    }
}
