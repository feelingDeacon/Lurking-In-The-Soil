using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootBlock : Block
{
    public bool isSource;
    public RootBlock prevBlock;
    public RootBlock nextBlock;
    public int health;
    public int maxHealth;
    public int existedTime;
    public bool isDead;

    
    
    public override void UpdateBlock()
    {
        
    }

    public void SetData(RootBlock prev, RootBlock next, bool isSourceBlock = false)
    {
        prevBlock = prev;
        nextBlock = next;
        isSource = isSourceBlock;
    }
}
