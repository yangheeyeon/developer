using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManagers
{
    //1. Original�� pool�� �̹� ������ ������?
    public T Load<T>(string path) where T :Object
    {
        if(typeof(T) == typeof(GameObject))
        {
            int index = path.LastIndexOf('/');
            string name = path.Substring(index + 1);
            GameObject go = Managers.Pool.GetOriginal(name) ;
            if (go != null)
                return go as T;

        }
        return Resources.Load<T>(path);
    }
    //2. Ȥ�� �̹� Ǯ���� �ְ� ������?
    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if (original == null)
        {
            Debug.Log($"Failed to Load Prefabs/{path}");
            return null;//null��ü 
        }
        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;
     
        GameObject go = Object.Instantiate(original, parent);

        

        int idx = go.name.IndexOf("(Clone)");
        if(idx > 0)
        {
            go.name = go.name.Substring(0, idx);
        }
        return go;
    }

    //���� Ǯ���� �ʿ��� ���̶�� ������ ���� -> Ǯ�� �Ŵ������� ��Ź
    public void Destroy(GameObject go)
    {
        Poolable poolable = go.GetComponent<Poolable>();
        if (poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }
        if (go == null)
            return;
        Object.Destroy(go);
    }
}
