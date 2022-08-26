using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagersEx
{
    //@scene�� ���� �ִ� GameScene component
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }
    public void LoadScene(Define.Scene type)
    {
        //���� scene ���ְ� ���ο� scene 
       
        Managers.Clear();

        string name = GetSceneName(type);
        SceneManager.LoadScene(name);
    }
    string GetSceneName(Define.Scene type)
    {
        return System.Enum.GetName(typeof(Define.Scene), type);
    }
    public void Clear()
    {
        CurrentScene.Clear();
    }
    
}
