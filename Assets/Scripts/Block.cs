using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
    Empty,
    Root,
}
public abstract class Block : MonoBehaviour
{
    public SpriteRenderer renderer;
    public BlockType blockType;
    public int x;
    public int y;

    public virtual void InitBlock(int xPos, int yPos)
    {
        x = xPos;
        y = yPos;
    }
    
    public virtual void UpdateBlock()
    {
        
    }

    public virtual void DestroyBlock()
    {
        if (EnvironmentManager.Instance.tilemap[x][y] == this)
        {
            EnvironmentManager.Instance.tilemap[x][y] = null;
        }
        ObjectPool.Instance.DestroyObject(gameObject);
    }
}
