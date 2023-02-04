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
    public Vector2 currDir; // 0:up, 0-7 clockwise
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
    
    public void SetData(RootBlock prev, RootBlock next, Vector2 direction, bool isSourceBlock = false)
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

    private int _attemptSpawnIndexX, _attemptSpawnIndexY;
    private void ExtendRoot()
    {
        if (nextBlock) return;
        
        nextExtendTime = Time.time + Random.Range(minExtendInterval, maxExtendInterval);

        GetBlockIndexOfDirection(currDir, out int newX, out int newY);
        _attemptSpawnIndexX = newX;
        _attemptSpawnIndexY = newY;
        
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
    
    private void GetBlockIndexOfDirection(Vector2 dir, out int resX, out int resY)
    {
        ExtensionFunction.DirectionToIndex(dir, out int offsetX, out int offsetY);
        resX = x + offsetX;
        resY = y + offsetY;
    }
    
    void OnDrawGizmosSelected()
    {
        if (nextBlock)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, nextBlock.transform.position);
        }
        else
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, EnvironmentManager.Instance.BlockIndexToWorldPos(
                _attemptSpawnIndexX, _attemptSpawnIndexY));
        }

        if (prevBlocks.Count > 0)
        {
            Gizmos.color = Color.cyan;
            foreach (var prevBlock in prevBlocks)
            {
                Gizmos.DrawLine(transform.position, prevBlock.transform.position);
            }
        }
        
        Gizmos.color = Color.white;
        
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(currDir.x, currDir.y));
    }
}
