using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private static MenuManager _instance;

    public static MenuManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<MenuManager>();
            }

            return _instance;
        }
    }
    
    void Start()
    {
        ObjectPool.Instance.CacheAllObjects();
        EnvironmentManager.Instance.Init();
        InitMenu();
    }

    private List<KeyValuePair<Vector2, Vector2>> rootSpawnLocation;
    

    void Update()
    {
        EnvironmentManager.Instance.UpdateManager();
        UpdateMenu();
    }

    public void InitMenu()
    {
        rootSpawnLocation = new List<KeyValuePair<Vector2, Vector2>>();
        for (int i = 0; i < Random.Range(4, 6); i++)
        {
            rootSpawnLocation.Add(
                new KeyValuePair<Vector2, Vector2>(new Vector2(0, Random.Range(0, GameConstants.MapHeight)), 
                    new Vector2(1, 0)));
        }
        
        for (int i = 0; i < Random.Range(4, 6); i++)
        {
            rootSpawnLocation.Add(
                new KeyValuePair<Vector2, Vector2>(
                    new Vector2(GameConstants.MapWidth - 1, Random.Range(0, GameConstants.MapHeight)), 
                    new Vector2(-1, 0)));
        }
        
        for (int i = 0; i < Random.Range(4, 6); i++)
        {
            rootSpawnLocation.Add(
                new KeyValuePair<Vector2, Vector2>(new Vector2(Random.Range(0, GameConstants.MapWidth), 0), 
                    new Vector2(0, 1)));
        }
        
        for (int i = 0; i < Random.Range(4, 6); i++)
        {
            rootSpawnLocation.Add(
                new KeyValuePair<Vector2, Vector2>(
                    new Vector2(Random.Range(0, GameConstants.MapWidth), GameConstants.MapHeight - 1), 
                    new Vector2(0, -1)));
        }

        rootSpawnLocation = rootSpawnLocation.OrderBy(x => Random.value).ToList();
        
        _nextSpawnRootTime = Time.time + 0.5f;
    }

    private float _nextSpawnRootTime;

    public void UpdateMenu()
    {
        if (_nextSpawnRootTime <= Time.time && rootSpawnLocation.Count > 0)
        {
            _nextSpawnRootTime = Time.time + 0.5f;
            CreateNewRoot();
        }
    }

    public void CreateNewRoot()
    {
        KeyValuePair<Vector2, Vector2> currRootData = rootSpawnLocation[0];

        int x = (int)currRootData.Key.x;
        int y = (int)currRootData.Key.y;
        Vector2 dir = currRootData.Value;
        
        RootBlock newRoot = (RootBlock)EnvironmentManager.Instance.CreateBlockAtIndex(BlockType.Root, x, y);
        newRoot.SetData(null, dir, 1, Random.Range(0, 360), 
            Random.Range(1f, 2f), Random.Range(10f, 20f), 
            0, 0, true, true);
        
        rootSpawnLocation.RemoveAt(0);
    }

    public void ClickStart()
    {
        SceneManager.LoadScene("GameScene");
    }
}
