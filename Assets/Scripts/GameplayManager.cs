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
    
    void Start()
    {
        ObjectPool.Instance.CacheAllObjects();
        InitGameplay();
        EnvironmentManager.Instance.Init();
    }
    
    void Update()
    {
        EnvironmentManager.Instance.UpdateManager();
        PlayerRuntime.Instance.UpdatePlayer();
        UpdateGameplay();
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            CreateNewRoot();
        }
    }

    private void FixedUpdate()
    {
        PlayerRuntime.Instance.FixedUpdatePlayer();
    }
    
    public void InitGameplay()
    {
    }

    public void UpdateGameplay()
    {
        
    }
    
    public void CreateNewRoot()
    {
        int x, y;
        do
        {
            if (0.5f.ChanceToBool())
            {
                y = Random.Range(0, EnvironmentManager.Instance.heightSize);

                if (0.5f.ChanceToBool())
                {
                    x = 0;
                }
                else
                {
                    x = EnvironmentManager.Instance.widthSize - 1;
                }
            }
            else
            {
                x = Random.Range(0, EnvironmentManager.Instance.widthSize);

                if (0.5f.ChanceToBool())
                {
                    y = 0;
                }
                else
                {
                    y = EnvironmentManager.Instance.heightSize - 1;
                }
            }
        } while (!EnvironmentManager.Instance.IsBlockIndexEmpty(x, y));
        
        RootBlock newRoot = (RootBlock)EnvironmentManager.Instance.CreateBlockAtIndex(BlockType.Root, x, y);
        newRoot.SetData(null, null, true);
    }
}
