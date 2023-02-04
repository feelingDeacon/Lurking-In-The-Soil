using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RootBlock : Block
{
    public int maxHealth;
    public float minExtendInterval;
    public float maxExtendInterval;

    public bool isSource;
    public List<RootBlock> prevBlocks;
    public RootBlock nextBlock;
    public int health;
    public int existedTime;
    public bool isDead;
    public float currDir; // 0:up, 0-7 clockwise
    public float nextExtendTime;

    public override void InitBlock(int xPos, int yPos)
    {
        base.InitBlock(xPos, yPos);
        prevBlocks = new List<RootBlock>();
        nextBlock = null;
        isSource = false;
        health = maxHealth;
        existedTime = 0;
        isDead = false;
        nextExtendTime = Time.time + Random.Range(minExtendInterval, maxExtendInterval);
        UpdateColor();
    }
    
    public void SetData(RootBlock prev, RootBlock next, float direction, bool isSourceBlock = false)
    {
        if (prev)
        {
            prevBlocks.Add(prev);
        }
        nextBlock = next;
        isSource = isSourceBlock;
        currDir = direction;

        if (nextBlock == null)
        {
            TryConnectFrontRoot();
        }
    }

    public override void UpdateBlock()
    {
        if (nextExtendTime <= Time.time)
        {
            ExtendRoot();
        }
        
        
    }

    public void GetHurt(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            DestroyBlock();
        }
        else
        {
            UpdateColor();
        }
    }

    public void RemovePrevBlock(RootBlock removedBlock)
    {
        prevBlocks.Remove(removedBlock);
        if (prevBlocks.Count == 0)
        {
            GetHurt(maxHealth);
        }
    }

    public void RemoveNextBlock(RootBlock removedBlock)
    {
        nextBlock = null;
    }

    public override void DestroyBlock()
    {
        foreach (RootBlock prevBlock in prevBlocks)
        {
            prevBlock.RemoveNextBlock(this);
        }
        prevBlocks.Clear();

        if (nextBlock)
        {
            nextBlock.RemovePrevBlock(this);
        }
        
        base.DestroyBlock();
    }

    private void ExtendRoot()
    {
        if (nextBlock) return;
        
        nextExtendTime = Time.time + Random.Range(minExtendInterval, maxExtendInterval);

        GetBlockIndexOfDirection(currDir, out int newX, out int newY);
        
        if (EnvironmentManager.Instance.IsBlockIndexEmpty(newX, newY))
        {
            RootBlock newRoot = (RootBlock)EnvironmentManager.Instance.CreateBlockAtIndex(BlockType.Root, newX, newY);
            newRoot.SetData(this, null, currDir);
            nextBlock = newRoot;
        }
    }

    private void TryConnectFrontRoot()
    {
        GetBlockIndexOfDirection(currDir, out int fwdX, out int fwdY);
        if (EnvironmentManager.Instance.IsBlockIndexMatchType(BlockType.Root, fwdX, fwdY))
        {
            RootBlock forwardBlock = (RootBlock)EnvironmentManager.Instance.GetBlock(fwdX, fwdY);
            forwardBlock.prevBlocks.Add(this);
            nextBlock = forwardBlock;
        }
        else
        {
            GetBlockIndexOfDirection(currDir.GetLeftDirection(), out int leftX, out int leftY);
            GetBlockIndexOfDirection(currDir.GetRightDirection(), out int rightX, out int rightY);
            List<RootBlock> neighborRoots = new List<RootBlock>();
            if (EnvironmentManager.Instance.IsBlockIndexMatchType(BlockType.Root, leftX, leftY))
            {
                neighborRoots.Add((RootBlock)EnvironmentManager.Instance.GetBlock(leftX, leftY));
            }
            if (EnvironmentManager.Instance.IsBlockIndexMatchType(BlockType.Root, rightX, rightY))
            {
                neighborRoots.Add((RootBlock)EnvironmentManager.Instance.GetBlock(rightX, rightY));
            }

            if (neighborRoots.Count > 0)
            {
                RootBlock selectedBlock = neighborRoots[Random.Range(0, neighborRoots.Count)];
                selectedBlock.prevBlocks.Add(this);
                nextBlock = selectedBlock;
            }
        }
    }

    public void ThickenRoot()
    {
        
    }

    public void BranchOffRoot()
    {
        
    }

    private void UpdateColor()
    {
        var rendererColor = renderer.color;
        rendererColor.a = (float)health / maxHealth;
        renderer.color = rendererColor;
    }
    
    private void GetBlockIndexOfDirection(float dir, out int resX, out int resY)
    {
        resX = 0;
        resY = 0;
        int intDir = (int)Math.Round(dir);
        switch (intDir)
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
