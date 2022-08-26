using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UI_Base : MonoBehaviour
{
    //key , value
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    public abstract void Init();

    //enum type

    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {

        string[] names = Enum.GetNames(type);
        //T 타입 UI component를 딕셔너리에 저장
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        _objects.Add(typeof(T), objects);

        for(int i = 0; i < names.Length; i++)
        {
            //이름 배열만 넘겨주면
            // gameObject산하의 자식들 중에 names 에 있는 이름을 가진 것들을 모아모아 T타입으로 
            if (typeof(T) == typeof(GameObject))
                objects[i] = Util.FindChild(gameObject, names[i], true);
            else
                objects[i] = Util.FindChild<T>(gameObject, names[i], true);

            if (objects[i] == null)
                Debug.Log($"Failed to bind({names[i]})");
        }

    }
   
    //딕셔너리에 저장된 정보 가져오기
    protected T Get<T>(int i) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;//namespaceName+className+structName+ .. + 
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;
        return objects[i] as T;
    }
    //Text, Button, Image등 게임object들의 component Get함수
    protected GameObject GetObject(int index) { return Get<GameObject>(index); }
    protected Text GetText(int index) { return Get<Text>(index); }
    protected Button GetButton(int index) { return Get<Button>(index); }
    protected Image GetImage(int index) { return Get<Image>(index); }

    //이미지 object에 마우스따라 움직이는 이벤트 자동으로 추가
    public static void BindEvent(GameObject go,Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)    {
        //각각의 게임 오브젝트들이 이벤트 헨들러 가짐 
        UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

        switch (type)
        {
            case Define.UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;
            case Define.UIEvent.Drag:
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action;
                break;
        }



    }
}
