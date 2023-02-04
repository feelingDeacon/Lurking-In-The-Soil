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

    public virtual void InitBlock()
    {
        
    }
    
    public virtual void UpdateBlock()
    {
        
    }

    public virtual void DestroyBlock()
    {
        Destroy(gameObject);
    }
}
