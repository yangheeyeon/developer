using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{

    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    protected virtual void Init() {
        

        //scene에 EventSystem이 없으면 생성

        //start,update를 자식이 override하면 자식메서드 호출
        //자식 메서드 없으면 부모 메서드 호출

        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if(obj == null)
        {
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";
        }
    }
    public abstract void Clear();
}
