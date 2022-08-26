using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//PoolManagers <= Resource Managers 
public class PoolManagers 
{
    #region Pool
    //pool 나누기
    class Pool
    {
        public GameObject Original { get; private set; }
        //UnityChan_Root 
        public Transform poolRoot { get; set; }
        //같은 pool마다 존재하는 스택
        public Stack<Poolable> _poolStack = new Stack<Poolable>();
        public void Init(GameObject original, int count = 5)
        {
            Original = original;
            poolRoot = new GameObject().transform;
            poolRoot.name = $"{original.name}_Root";

            //같은 Pool clone생성
            for(int i = 0; i< count; i++)
            {
                Push(Create());
            }
        }
        public Poolable Create()
        {
            GameObject go = Object.Instantiate(Original);
            go.name = Original.name;
            
            return go.GetOrAddComponent<Poolable>();

        }
        public void Push(Poolable poolable)
        {
            //쓰고 다시 반납
            if (poolable == null)
                return;
            //휴면 상태로 저장
            poolable.transform.parent = poolRoot;
            poolable.gameObject.SetActive(false);
            poolable.isUsable = false;

            _poolStack.Push(poolable);
        }
        public Poolable Pop(Transform parent)
        {
            //스택에 있으면 꺼내고 , 없으면 Original으로 새롭게 생성
            Poolable poolable;
            if (_poolStack.Count > 0)
                poolable = _poolStack.Pop();
            else
                poolable = Create();
            poolable.gameObject.SetActive(true);
            poolable.isUsable = true;
            //DontDestroyOnLoad 피하기 용도
            if (parent == null)
                poolable.transform.parent = Managers.Scene.CurrentScene.transform;
            poolable.transform.parent = parent;
            return poolable;
        }

    }
    #endregion 

    Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();
    Transform _root; 
    // @Pool_root 
    public void Init()
    {
        if(_root == null)
        {
            GameObject go = new GameObject() { name = "@Pool_root"};
            _root = go.transform;
        }
    }
    public void CreatePool(GameObject original, int count =5)
    {
        Pool pool = new Pool();
        pool.Init(original,count);
        pool.poolRoot.parent = _root;
        _pools.Add(original.name, pool);

       
    }
    //pool다시 반납
    public void Push(Poolable poolable)
    {
        string name = poolable.gameObject.name;
        if (_pools.ContainsKey(name) == false)
        {
            Object.Destroy(poolable.gameObject);
            return;
        }
         
        _pools[name].Push(poolable);

    }
    public Poolable Pop(GameObject original, Transform parent = null)
    {
        
        if (_pools.ContainsKey(original.name) == false)
             CreatePool(original);
       
        return _pools[original.name].Pop(parent);
    }
    public GameObject GetOriginal(string name)
    {
        if (_pools.ContainsKey(name) == false)
            return null;
        return _pools[name].Original;
    }
    public void Clear()
    {
        foreach(Transform child in _root)
        {
            Object.Destroy(child.gameObject);
        }
        _pools.Clear();
    }
}
