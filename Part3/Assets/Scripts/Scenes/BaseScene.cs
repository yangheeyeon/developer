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
        

        //scene�� EventSystem�� ������ ����

        //start,update�� �ڽ��� override�ϸ� �ڽĸ޼��� ȣ��
        //�ڽ� �޼��� ������ �θ� �޼��� ȣ��

        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if(obj == null)
        {
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";
        }
    }
    public abstract void Clear();
}
