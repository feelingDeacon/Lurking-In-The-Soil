using System;
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

    private float centerOffsetX => - GameConstants.MapWidth / 2 + 0.5f;
    private float centerOffsetY => - GameConstants.MapHeight / 2 + 0.5f;

    public List<List<Block>> tilemap;
    
    public void Init()
    {
        SpawnGrid();
    }

    private void SpawnGrid()
    {
        tilemap = new List<List<Block>>();
        for (int i = 0; i < GameConstants.MapWidth; i++)
        {
            List<Block> currCol = new List<Block>();
            for (int j = 0; j < GameConstants.MapHeight; j++)
            {
                currCol.Add(null);
            }
            tilemap.Add(currCol);
        }
        
        // for (int i = 0; i < widthSize; i++)
        // {
        //     for (int j = 0; j < heightSize; j++)
        //     {
        //         Block newBlock = CreateBlockAtIndex(BlockType.Empty, i, j);
        //     }
        // }
    }

    public Block CreateBlockAtIndex(BlockType type, int x, int y)
    {
        GameObjectId blockGameObjectId;
        switch (type)
        {
            case BlockType.Empty:
                blockGameObjectId = GameObjectId.EmptyBlock;
                break;
            case BlockType.Root:
                blockGameObjectId = GameObjectId.RootBlock;
                break;
            case BlockType.Nexus:
                blockGameObjectId = GameObjectId.NexusBlock;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
        
        Block newBlock = ObjectPool.Instance.CreateObject(blockGameObjectId, transform,
            BlockIndexToWorldPos(x, y)).GetComponent<Block>();
        newBlock.InitBlock(x, y);
        
        SetBlock(newBlock, x, y);

        return newBlock;
    }

    public void RemoveBlockAtIndex(int x, int y)
    {
        if (tilemap[x][y])
        {
            tilemap[x][y].DestroyBlock();
        }

        tilemap[x][y] = null;
    }
    
    public Vector3 BlockIndexToWorldPos(int x, int y)
    {
        return new Vector3(x + centerOffsetX, y + centerOffsetY, 0);
    }

    public void UpdateManager()
    {
        for (int x = 0; x < GameConstants.MapWidth; x++)
        {
            for (int y = 0; y < GameConstants.MapHeight; y++)
            {
                if (tilemap[x][y])
                {
                    tilemap[x][y].UpdateBlock();
                }
            }
        }
    }

    public bool IsBlockIndexEmpty(int x, int y)
    {
        if (x < 0 || x >= GameConstants.MapWidth || y < 0 || y >= GameConstants.MapHeight)
        {
            return false;
        }
        if (tilemap[x][y])
        {
            return tilemap[x][y].blockType == BlockType.Empty;
        }
        else
        {
            return true;
        }
    }

    public bool IsBlockIndexMatchType(BlockType blockType, int x, int y)
    {
        if (x < 0 || x >= GameConstants.MapWidth || y < 0 || y >= GameConstants.MapHeight)
        {
            return false;
        }
        if (tilemap[x][y])
        {
            return tilemap[x][y].blockType == blockType;
        }
        else
        {
            return blockType == BlockType.Empty;
        }
    }

    public bool HasSurroundBlockMatchType(BlockType blockType, int x, int y)
    {
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i == x && j == y) continue;
                
                if (IsBlockIndexMatchType(blockType, i, j))
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    public void SetBlock(Block newBlock, int x, int y)
    {
        if (tilemap[x][y])
        {
            tilemap[x][y].DestroyBlock();
        }
        tilemap[x][y] = newBlock;
    }

    public Block GetBlock(int x, int y)
    {
        return tilemap[x][y];
    }
}
