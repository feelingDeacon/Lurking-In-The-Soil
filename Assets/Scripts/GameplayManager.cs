using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        EnvironmentManager.Instance.Init();
    }
    
    // Update is called once per frame
    void Update()
    {
        EnvironmentManager.Instance.UpdateManager();
        PlayerRuntime.Instance.UpdatePlayer();
    }

    private void FixedUpdate()
    {
        PlayerRuntime.Instance.FixedUpdatePlayer();
    }
}
