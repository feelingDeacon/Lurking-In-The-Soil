using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GameObjectId
{
    EmptyBlock,
    RootBlock,
}

public class ObjectPool : MonoBehaviour
{
    private static ObjectPool _instance;
    public static ObjectPool Instance {
        get {
            if (!_instance) {
                _instance = FindObjectOfType<ObjectPool>();
                _instance.InitObjectPool();
            }
            return _instance;
        }
    }
    
    private void Awake() {
        InitObjectPool();
    }
    
    private List<int> _poolKeys;
    private Dictionary<int, List<GameObject>> _pool;
    private Dictionary<GameObjectId, GameObject> _prefabDict;

    public void InitObjectPool()
    {
        _poolKeys = new List<int>();
        _pool = new Dictionary<int, List<GameObject>>();
        _prefabDict = new Dictionary<GameObjectId, GameObject>();
    }

    public void CacheAllObjects()
    {
        var objectIds = Enum.GetValues(typeof(GameObjectId)).Cast<GameObjectId>().ToList();
        foreach (var id in objectIds)
        {
            GameObject prefab = Resources.Load<GameObject>(id.ToString());
            CacheObject(prefab, 2);
            _prefabDict.Add(id, prefab);
        }
    }

    public void DestroyPoolAll()
    {
        // ***** Will cause lag *****
        foreach (var pair in _pool)
        {
            if (pair.Value != null)
            {
                foreach (var obj in pair.Value)
                {
                    if (obj != null)
                    {
                        Destroy(obj);
                    }
                }
            }
        }
        _poolKeys = new List<int>();
        _pool = new Dictionary<int, List<GameObject>>();
    }

    public void CacheObject(GameObject obj, int count, Action<GameObject> onObjCreated = null)
    {
        // ***** Will cause lag *****

        int key = obj.GetInstanceID();

        if (_poolKeys.Contains(key) == false && _pool.ContainsKey(key) == false)
        {
            _poolKeys.Add(key);
            _pool.Add(key, new List<GameObject>());
        }

        List<GameObject> goList = _pool[key];
        goList.RemoveAll(g => g == null);

        int createCount = count - goList.Count;
        
        //Debug.Log("Cache Object: " + obj.name + " count: " + createCount);
        for (int i = 0; i < createCount; i++)
        {
            GameObject go = (GameObject)Instantiate(obj, new Vector3(999, 999, 999), Quaternion.identity);
            go.transform.SetParent(gameObject.transform);
            if (onObjCreated != null)
            {
                onObjCreated(go);
            }
            go.SetActive(false);
            goList.Add(go);
        }
    }

    public GameObject CreateObject(GameObjectId objectId, Transform parent = null, Vector3 localPos = default(Vector3),
        Quaternion localRot = default(Quaternion))
    {
        return CreateObject(_prefabDict[objectId], parent, localPos, localRot);
    }
    
    public GameObject CreateObject(GameObject obj, Transform parent = null, Vector3 localPos = default(Vector3),
        Quaternion localRot = default(Quaternion))
    {
        int key = obj.GetInstanceID();

        if (_poolKeys.Contains(key) == false && _pool.ContainsKey(key) == false) {
            _poolKeys.Add(key);
            _pool.Add(key, new List<GameObject>());
        }

        List<GameObject> goList = _pool[key];
        goList.RemoveAll(g => g == null);
        GameObject go = null;

        for (int i = goList.Count - 1; i >= 0; i--) {
            go = goList[i];
            if (go.activeSelf == false) {
                go.transform.SetParent(parent);
                go.transform.localPosition = localPos;
                go.transform.localRotation = localRot;
                go.transform.localScale = Vector3.one;
                go.SetActive(true);
                return go;
            }
        }

        // Instantiate because there is no free GameObject in object pool.
        go = (GameObject)Instantiate(obj);
        go.transform.SetParent(parent);
        go.transform.localPosition = localPos;
        go.transform.localRotation = localRot;
        go.transform.localScale = Vector3.one;
        go.SetActive(true);
        goList.Add(go);

        return go;
    }

    public void DestroyObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
    }
}

