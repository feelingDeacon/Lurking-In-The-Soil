using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameplayManager : MonoBehaviour
{
    private static GameplayManager _instance;

    public static GameplayManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<GameplayManager>();
            }

            return _instance;
        }
    }
    
    private int _nexusHealth;
    public bool gameEnd;
    
    void Start()
    {
        ObjectPool.Instance.CacheAllObjects();
        EnvironmentManager.Instance.Init();
        InitGameplay();
    }
    
    void Update()
    {
        if (!gameEnd)
        {
            EnvironmentManager.Instance.UpdateManager();
            PlayerRuntime.Instance.UpdatePlayer();
            UpdateGameplay();
        
            if (Input.GetKeyDown(KeyCode.R))
            {
                CreateNewRoot();
            }
        }
    }

    private void FixedUpdate()
    {
        PlayerRuntime.Instance.FixedUpdatePlayer();
    }
    
    public void InitGameplay()
    {
        CreateNexus();
        NexusHealth = GameConstants.NexusMaxHealth;
        gameEnd = false;
    }

    public void UpdateGameplay()
    {
        
    }

    public void LoseGame()
    {
        
    }

    public int NexusHealth
    {
        get => _nexusHealth;
        set
        {
            if (_nexusHealth > value)
            {
                DamageNexus();
            }
            _nexusHealth = value;
            _nexusHealth = Math.Max(0, _nexusHealth);
            _nexusHealth = Math.Min(GameConstants.NexusMaxHealth, _nexusHealth);
            if (_nexusHealth <= 0 && !gameEnd)
            {
                gameEnd = true;
                LoseGame();
            }
        }
    }

    public void CreateNexus()
    {
        int nexusHalfWidth = 5;
        int nexusHalfHeight = 5;
        for (int x = GameConstants.MapWidth / 2 - nexusHalfWidth;
             x < GameConstants.MapWidth / 2 + nexusHalfWidth;
             x++)
        {
            for (int y = GameConstants.MapHeight / 2 - nexusHalfHeight;
                 y < GameConstants.MapHeight / 2 + nexusHalfHeight;
                 y++)
            {
                NexusBlock nexusBlock = (NexusBlock)EnvironmentManager.Instance.CreateBlockAtIndex(
                    BlockType.Nexus, x, y);
            }
        }
    }

    public void DamageNexus()
    {
        
    }

    public void CreateNewRoot()
    {
        int x, y;
        Vector2 dir;
        do
        {
            if (0.5f.ChanceToBool())
            {
                y = Random.Range(0, GameConstants.MapHeight);

                if (0.5f.ChanceToBool())
                {
                    x = 0;
                    // dir = 2;
                    dir = new Vector2(1, 0);
                }
                else
                {
                    x = GameConstants.MapWidth - 1;
                    // dir = 6;
                    dir = new Vector2(-1, 0);
                }
            }
            else
            {
                x = Random.Range(0, GameConstants.MapWidth);

                if (0.5f.ChanceToBool())
                {
                    y = 0;
                    // dir = 0;
                    dir = new Vector2(0, 1);
                }
                else
                {
                    y = GameConstants.MapHeight - 1;
                    // dir = 4;
                    dir = new Vector2(0, -1);
                }
            }
        } while (!EnvironmentManager.Instance.IsBlockIndexEmpty(x, y));
        
        RootBlock newRoot = (RootBlock)EnvironmentManager.Instance.CreateBlockAtIndex(BlockType.Root, x, y);
        newRoot.SetData(null, dir, 1, Random.Range(0, 360), 
            Random.Range(1f, 2f), Random.Range(10f, 20f), 
            0, 0, true, true);
    }
}
