using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util

{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component//AddComponent 위함
    {
        T component = go.GetComponent<T>();

        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }
    //게임 오브젝트 찾고 싶을때
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {

        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;
        return transform.gameObject;
    }
    //특정 컴포넌트 찾고 싶을때
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;
        //직계 자식에서만 T Component 탐색
        if(recursive == false)
        {
            for(int i = 0; i<  go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
                   
            }
            
        }
        else
        {
            //자기 자신의 자손들중에서 T Component 탐색
            foreach(T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name)|| component.name == name)
                    return component;

            }
        }
        return null;


    }
}