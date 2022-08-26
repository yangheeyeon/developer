using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagers 
{
    GameObject _player;
    HashSet<GameObject> _monsters = new HashSet<GameObject>();
    public GameObject GetPlayer { get { return _player; } }

    public Action<int> OnSpawnEvent;

    //플레이어, 몬스터 자동 생성
    public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(path, parent);

        switch (type)
        {
            case Define.WorldObject.Monster:
                _monsters.Add(go);
                if(OnSpawnEvent != null)
                    OnSpawnEvent.Invoke(1);
                break;
            case Define.WorldObject.Player:
                _player = go;
                break;
        }
        
        return go;
    }
    //GameObject 삭제
    public Define.WorldObject GetWorldObjectType(GameObject go)
    {

        BaseController bc = go.GetComponent<BaseController>();
        if (bc == null)
            return Define.WorldObject.Unknown;

        return bc.WorldObjectType;

        
    }
    public void DeSpawn(GameObject go)
    {
        Define.WorldObject type = GetWorldObjectType(go);

        switch (type)
        {
            case Define.WorldObject.Monster:
                if (_monsters.Contains(go))
                    _monsters.Remove(go);
                break;
            case Define.WorldObject.Player:
                if (_player == go)
                    _player = null;
                break;
        }
        OnSpawnEvent.Invoke(-1);
        Managers.Resource.Destroy(go);
    }
}
