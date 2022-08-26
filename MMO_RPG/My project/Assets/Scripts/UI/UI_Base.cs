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
        //T Ÿ�� UI component�� ��ųʸ��� ����
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        _objects.Add(typeof(T), objects);

        for(int i = 0; i < names.Length; i++)
        {
            //�̸� �迭�� �Ѱ��ָ�
            // gameObject������ �ڽĵ� �߿� names �� �ִ� �̸��� ���� �͵��� ��Ƹ�� TŸ������ 
            if (typeof(T) == typeof(GameObject))
                objects[i] = Util.FindChild(gameObject, names[i], true);
            else
                objects[i] = Util.FindChild<T>(gameObject, names[i], true);

            if (objects[i] == null)
                Debug.Log($"Failed to bind({names[i]})");
        }

    }
   
    //��ųʸ��� ����� ���� ��������
    protected T Get<T>(int i) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;//namespaceName+className+structName+ .. + 
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;
        return objects[i] as T;
    }
    //Text, Button, Image�� ����object���� component Get�Լ�
    protected GameObject GetObject(int index) { return Get<GameObject>(index); }
    protected Text GetText(int index) { return Get<Text>(index); }
    protected Button GetButton(int index) { return Get<Button>(index); }
    protected Image GetImage(int index) { return Get<Image>(index); }

    //�̹��� object�� ���콺���� �����̴� �̺�Ʈ �ڵ����� �߰�
    public static void BindEvent(GameObject go,Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)    {
        //������ ���� ������Ʈ���� �̺�Ʈ ��鷯 ���� 
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
