using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//PoolManagers <= Resource Managers 
public class PoolManagers 
{
    #region Pool
    //pool ������
    class Pool
    {
        public GameObject Original { get; private set; }
        //UnityChan_Root 
        public Transform poolRoot { get; set; }
        //���� pool���� �����ϴ� ����
        public Stack<Poolable> _poolStack = new Stack<Poolable>();
        public void Init(GameObject original, int count = 5)
        {
            Original = original;
            poolRoot = new GameObject().transform;
            poolRoot.name = $"{original.name}_Root";

            //���� Pool clone����
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
            //���� �ٽ� �ݳ�
            if (poolable == null)
                return;
            //�޸� ���·� ����
            poolable.transform.parent = poolRoot;
            poolable.gameObject.SetActive(false);
            poolable.isUsable = false;

            _poolStack.Push(poolable);
        }
        public Poolable Pop(Transform parent)
        {
            //���ÿ� ������ ������ , ������ Original���� ���Ӱ� ����
            Poolable poolable;
            if (_poolStack.Count > 0)
                poolable = _poolStack.Pop();
            else
                poolable = Create();
            poolable.gameObject.SetActive(true);
            poolable.isUsable = true;
            //DontDestroyOnLoad ���ϱ� �뵵
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
    //pool�ٽ� �ݳ�
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
