using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootBlock : Block
{
    public int maxHealth;
    public float minExtendInterval;
    public float maxExtendInterval;

    public bool isSource;
    public List<RootBlock> prevBlock;
    public RootBlock nextBlock;
    public int health;
    public int existedTime;
    public bool isDead;
    public int currDir; // 0:up, 0-7 clockwise
    public float nextExtendTime;
    

    public override void InitBlock(int xPos, int yPos)
    {
        base.InitBlock(xPos, yPos);
        prevBlock = new List<RootBlock>();
        nextBlock = null;
        isSource = false;
        health = maxHealth;
        existedTime = 0;
        isDead = false;
        nextExtendTime = Time.time + Random.Range(minExtendInterval, maxExtendInterval);
    }

    public override void UpdateBlock()
    {
        if (nextExtendTime <= Time.time)
        {
            ExtendRoot();
        }
    }

    private void ExtendRoot()
    {
        nextExtendTime = Time.time + Random.Range(minExtendInterval, maxExtendInterval);

        GetBlockToDirection(currDir, out int newX, out int newY);
        
        if (EnvironmentManager.Instance.IsBlockIndexEmpty(newX, newY))
        {
            RootBlock newRoot = (RootBlock)EnvironmentManager.Instance.CreateBlockAtIndex(BlockType.Root, newX, newY);
            newRoot.SetData(this, null, currDir);
            nextBlock = newRoot;
        }
    }

    public void SetData(RootBlock prev, RootBlock next, int direction, bool isSourceBlock = false)
    {
        if (prev)
        {
            prevBlock.Add(prev);
        }
        nextBlock = next;
        isSource = isSourceBlock;
        currDir = direction;
    }

    private void GetBlockToDirection(int dir, out int resX, out int resY)
    {
        resX = 0;
        resY = 0;
        switch (dir)
        {
            case 0:
                resX = x;
                resY = y + 1;
                break;
            case 1:
                resX = x + 1;
                resY = y + 1;
                break;
            case 2:
                resX = x + 1;
                resY = y;
                break;
            case 3:
                resX = x + 1;
                resY = y - 1;
                break;
            case 4:
                resX = x;
                resY = y - 1;
                break;
            case 5:
                resX = x - 1;
                resY = y - 1;
                break;
            case 6:
                resX = x - 1;
                resY = y;
                break;
            case 7:
                resX = x - 1;
                resY = y + 1;
                break;
        }
    }
}
