
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class InputManagers
{
    float _pressedTime;
    Boolean _pressed = false;

    //�븮�� -> �̺�Ʈ �˸���
    public Action keyAction = null;
    public Action<Define.MouseMode> MouseAction = null;
    public void OnUpdate()
    {
        //ui ��ư�� ������
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        //Ű���� ������ 
        if (Input.anyKey && keyAction != null)
            keyAction.Invoke();

        //OnMouseButton == Pressed
        //OnMouseButtonDown ==PointerDown
        //OnMouseButtonUp == PointerUp

        if (MouseAction != null)
        {
            if (Input.GetMouseButton(0))
            {
                if (!_pressed)
                {
                    //���� lockTarget����
                    MouseAction.Invoke(Define.MouseMode.PointerDown);
                    _pressedTime = Time.time;
                }
                //�������� �̵�
                MouseAction.Invoke(Define.MouseMode.Pressed);
                _pressed = true;
            }
            else
            {     

                //����ð��� ª���� Click�� �߻���Ŵ
                if (_pressed)
                {
                    //?
                    if(Time.time < _pressedTime + 0.2f)
                        MouseAction.Invoke(Define.MouseMode.Clicked);
                    //loctTarget ����
                    MouseAction.Invoke(Define.MouseMode.PointerUp);
                }
               
                _pressed = false;
                _pressedTime = 0;
            }
        }
    }

    public void Clear()
    {
        keyAction = null;
        MouseAction = null;
    }
}
