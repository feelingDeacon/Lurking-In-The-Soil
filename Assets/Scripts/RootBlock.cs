using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RootBlock : Block
{
    public int maxHealth;
    public int attackDamage;
    public float attackCooldown;
    public float minExtendInterval;
    public float maxExtendInterval;
    public float sinLength;
    public float sinHeight;
    public int minBranchThreshold;
    public int maxBranchThreshold;
    public float maxScale;

    public bool isSource;
    public RootBlock parent;
    public List<RootBlock> children;
    public int currHealth;
    public int currMaxHealth;
    public int currLength;
    public Vector2 currDir;
    public float currSin;
    public float nextExtendTime;
    public int nextBranchLength;
    public int currBranchCount;
    public float spawnTime;
    public float nextThickenTime;
    public bool willBranch;
    public bool willThicken;
    public float currScale;
    public float nextAttackTime;
    public bool isNextToNexus;

    public bool IsDead => CurrHealth <= 0;

    public int CurrHealth
    {
        get => currHealth;
        set
        {
            currHealth = value;
            currHealth = Math.Max(0, currHealth);
            currHealth = Math.Min(currMaxHealth, currHealth);
        }
    }

    public override void InitBlock(int xPos, int yPos)
    {
        base.InitBlock(xPos, yPos);
        parent = null;
        children = new List<RootBlock>();
        isSource = false;
        currMaxHealth = maxHealth;
        CurrHealth = currMaxHealth;
        currLength = 0;
        currDir = Vector2.up;
        currSin = 0;
        nextExtendTime = Time.time + Random.Range(minExtendInterval, maxExtendInterval);
        nextBranchLength = 100;
        currBranchCount = 0;
        spawnTime = Time.time;
        nextThickenTime = 100;
        willBranch = false;
        willThicken = false;
        currScale = 1;
        nextAttackTime = Time.time + attackCooldown;
        isNextToNexus = EnvironmentManager.Instance.HasSurroundBlockMatchType(BlockType.Nexus, x, y);
        UpdateColor();
        UpdateScale();
    }
    
    public void SetData(RootBlock newParent, Vector2 direction, int rootLength, 
        float currSinVal, float sinLengthScale, float sinHeightScale, 
        int nextBranchingLength, int branchCount, bool willThick, bool isSourceBlock = false)
    {
        parent = newParent;
        if (parent)
        {
            parent.children.Add(this);
        }
        isSource = isSourceBlock;
        currDir = direction;
        currSin = currSinVal;
        sinHeight = sinHeightScale;
        sinLength = sinLengthScale;
        currLength = rootLength;
        nextBranchLength = nextBranchingLength;
        willThicken = willThick;
        if (isSource)
        {
            nextBranchLength = currLength + Random.Range(minBranchThreshold, maxBranchThreshold);
        }
        currBranchCount = branchCount;

        if (currLength >= nextBranchLength)
        {
            willBranch = true;
            nextBranchLength = currLength + Random.Range(minBranchThreshold, maxBranchThreshold);
        }
        else
        {
            willBranch = false;
        }

        nextThickenTime = Time.time + maxExtendInterval * 10;
        
        // if (nextBlock == null)
        // {
        //     TryConnectFrontRoot();
        // }
    }

    public override void UpdateBlock()
    {
        bool hasAttacked = false;
        if (nextAttackTime <= Time.time)
        {
            hasAttacked = TryAttack();
        }
        
        if (!hasAttacked)
        {
            if (nextExtendTime <= Time.time)
            {
                ExtendRoot();
            }

            if (willBranch && spawnTime + maxExtendInterval * 8 <= Time.time)
            {
                BranchOffRoot();
            }
        }

        if (willThicken && nextThickenTime <= Time.time)
        {
            nextThickenTime = Time.time + maxExtendInterval * 10;
            
            // ThickenRoot();
            ThickenRoot1();
        }
    }

    public void GetHurt(int damage)
    {
        if (IsDead) return;
        CurrHealth -= damage;
        if (CurrHealth <= 0)
        {
            DestroyBlock();
        }
        else
        {
            UpdateColor();
        }
    }

    public void RemoveParent()
    {
        parent = null;
        GetHurt(currMaxHealth);
    }

    public void RemoveChild(RootBlock child)
    {
        children.Remove(child);
    }

    public ParticleSystem destroyPS;
    
    public override void DestroyBlock()
    {
        destroyPS.transform.SetParent(null);
        destroyPS.transform.position = transform.position;
        destroyPS.Play();
        if (parent)
        {
            parent.RemoveChild(this);
        }

        for (int i = children.Count - 1; i >= 0; i--)
        {
            children[i].RemoveParent();
        }
        children.Clear();

        GameplayManager.Instance.RootAmount++;
        
        base.DestroyBlock();
    }

    private int _attemptSpawnIndexX, _attemptSpawnIndexY;
    private void ExtendRoot()
    {
        if (children.Count > 0) return;
        
        nextExtendTime = Time.time + Random.Range(minExtendInterval, maxExtendInterval);
        
        GetBlockIndexOfDirection(currDir, out int newX, out int newY);
        _attemptSpawnIndexX = newX;
        _attemptSpawnIndexY = newY;
        
        if (EnvironmentManager.Instance.IsBlockIndexEmpty(newX, newY))
        {
            
            // default rotating
            Vector2 newDir = currDir.Rotate(Mathf.Sin(Mathf.Deg2Rad * currSin * sinLength) * sinHeight);
            
            // center dragging
            float signedAngle = Vector2.SignedAngle(currDir, -transform.position);
            newDir = newDir.Rotate((signedAngle >= 0 ? 1 : -1) * 
                GameConstants.CenterDragForce * currLength);

            RootBlock newRoot = (RootBlock)EnvironmentManager.Instance.CreateBlockAtIndex(BlockType.Root, newX, newY);
            newRoot.SetData(this, newDir, currLength + 1,
                currSin + GameConstants.SineIncrement, sinLength, sinHeight,
                nextBranchLength, currBranchCount, true);
        }
    }

    // private void TryConnectFrontRoot()
    // {
    //     GetBlockIndexOfDirection(currDir, out int fwdX, out int fwdY);
    //     if (EnvironmentManager.Instance.IsBlockIndexMatchType(BlockType.Root, fwdX, fwdY))
    //     {
    //         RootBlock forwardBlock = (RootBlock)EnvironmentManager.Instance.GetBlock(fwdX, fwdY);
    //         forwardBlock.prevBlocks.Add(this);
    //         nextBlocks.Add(forwardBlock);
    //     }
    //     // else
    //     // {
    //     //     GetBlockIndexOfDirection(currDir.GetLeftDirection(), out int leftX, out int leftY);
    //     //     GetBlockIndexOfDirection(currDir.GetRightDirection(), out int rightX, out int rightY);
    //     //     List<RootBlock> neighborRoots = new List<RootBlock>();
    //     //     if (EnvironmentManager.Instance.IsBlockIndexMatchType(BlockType.Root, leftX, leftY))
    //     //     {
    //     //         neighborRoots.Add((RootBlock)EnvironmentManager.Instance.GetBlock(leftX, leftY));
    //     //     }
    //     //     if (EnvironmentManager.Instance.IsBlockIndexMatchType(BlockType.Root, rightX, rightY))
    //     //     {
    //     //         neighborRoots.Add((RootBlock)EnvironmentManager.Instance.GetBlock(rightX, rightY));
    //     //     }
    //     //
    //     //     if (neighborRoots.Count > 0)
    //     //     {
    //     //         RootBlock selectedBlock = neighborRoots[Random.Range(0, neighborRoots.Count)];
    //     //         selectedBlock.prevBlocks.Add(this);
    //     //         nextBlock = selectedBlock;
    //     //     }
    //     // }
    // }

    // private void ThickenRoot()
    // {
    //
    //     List<int> dirList = new List<int>() { -135, 135, -90, 90, -45, 45 };
    //     foreach (var angle in dirList)
    //     {
    //         GetBlockIndexOfDirection(currDir.Rotate(angle), out int newX1, out int newY1);
    //
    //         if (EnvironmentManager.Instance.IsBlockIndexEmpty(newX1, newY1))
    //         {
    //             RootBlock newRoot = (RootBlock)EnvironmentManager.Instance.CreateBlockAtIndex(BlockType.Root, newX1, newY1);
    //             newRoot.SetData(null, this, currDir, currLength,
    //                 currSin, sinLength, sinHeight,
    //                 nextBranchLength + 3, currBranchCount, false);
    //             break;
    //         }
    //     }
    //     // GetBlockIndexOfDirection(currDir.Rotate(110), out int newX1, out int newY1);
    //     //
    //     // if (EnvironmentManager.Instance.IsBlockIndexEmpty(newX1, newY1))
    //     // {
    //     //     RootBlock newRoot = (RootBlock)EnvironmentManager.Instance.CreateBlockAtIndex(BlockType.Root, newX1, newY1);
    //     //     newRoot.SetData(this, nextBlock, currDir, currLength,
    //     //         currSin, sinLength, sinHeight,
    //     //         nextBranchLength, currBranchCount, false);
    //     // }
    //     // else
    //     // {
    //     //     GetBlockIndexOfDirection(currDir.Rotate(-110), out int newX2, out int newY2);
    //     //
    //     //     if (EnvironmentManager.Instance.IsBlockIndexEmpty(newX2, newY2))
    //     //     {
    //     //         RootBlock newRoot = (RootBlock)EnvironmentManager.Instance.CreateBlockAtIndex(BlockType.Root, newX2, newY2);
    //     //         newRoot.SetData(this, nextBlock, currDir, currLength,
    //     //             currSin, sinLength, sinHeight,
    //     //             nextBranchLength + 3, currBranchCount, false);
    //     //     }
    //     // }
    // }

    private void ThickenRoot1()
    {
        if (currScale >= maxScale) return;
        currScale += (maxScale-1) / GameConstants.ScaleAmount;
        currMaxHealth = (int)(maxHealth * currScale);
        CurrHealth += (int)((maxScale - 1) / GameConstants.ScaleAmount);
        UpdateScale();
    }
    
    private void BranchOffRoot()
    {
        willBranch = false;
        
        List<int> availableRotateAngles = new List<int>();
        GetBlockIndexOfDirection(currDir.GetLeftDirection(), out int x1, out int y1);
        if (EnvironmentManager.Instance.IsBlockIndexEmpty(x1, y1))
        {
            availableRotateAngles.Add(Random.Range(-40, -60));
        }
        GetBlockIndexOfDirection(currDir.GetRightDirection(), out int x2, out int y2);
        if (EnvironmentManager.Instance.IsBlockIndexEmpty(x2, y2))
        {
            availableRotateAngles.Add(Random.Range(40, 60));
        }
        GetBlockIndexOfDirection(currDir.Rotate(-90), out int x3, out int y3);
        if (EnvironmentManager.Instance.IsBlockIndexEmpty(x3, y3))
        {
            availableRotateAngles.Add(Random.Range(-65, -85));
        }
        GetBlockIndexOfDirection(currDir.Rotate(90), out int x4, out int y4);
        if (EnvironmentManager.Instance.IsBlockIndexEmpty(x4, y4))
        {
            availableRotateAngles.Add(Random.Range(65, 85));
        }

        if (availableRotateAngles.Count > 0)
        {
            Vector2 newDir = currDir.Rotate(availableRotateAngles[Random.Range(0, availableRotateAngles.Count)]);
            GetBlockIndexOfDirection(newDir, out int newX, out int newY);
            RootBlock newRoot = (RootBlock)EnvironmentManager.Instance.CreateBlockAtIndex(BlockType.Root, newX, newY);
            newRoot.SetData(this, newDir, currLength + 1,
                Random.Range(0, 360), sinLength * Random.Range(0.5f, 0.8f), sinHeight * Random.Range(0.5f, 0.8f),
                nextBranchLength, currBranchCount + 1, true);
        }
    }

    private void UpdateColor()
    {
        float val = (float)CurrHealth / currMaxHealth;
        renderer.color = new Color(val, val, val, 1);
    }

    private void UpdateScale()
    {
        currScale = Mathf.Min(currScale, maxScale);
        transform.localScale = Vector3.one * currScale;
    }
    
    private void GetBlockIndexOfDirection(Vector2 dir, out int resX, out int resY)
    {
        ExtensionFunction.DirectionToIndex(dir, out int offsetX, out int offsetY);
        resX = x + offsetX;
        resY = y + offsetY;
    }

    private bool TryAttack()
    {
        if (isNextToNexus)
        {
            nextAttackTime = Time.time + attackCooldown;
            GameplayManager.Instance.NexusHealth -= (int)(attackDamage * currScale);
            return true;
        }

        return false;
    }
    
    void OnDrawGizmosSelected()
    {
        if (children.Count > 0)
        {
            Gizmos.color = Color.yellow;
            foreach (var child in children)
            {
                Gizmos.DrawLine(transform.position, child.transform.position);
            }
        }
        else
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, EnvironmentManager.Instance.BlockIndexToWorldPos(
                _attemptSpawnIndexX, _attemptSpawnIndexY));
        }

        if (parent)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, parent.transform.position);
        }
        
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(currDir.x, currDir.y));
    }
}
