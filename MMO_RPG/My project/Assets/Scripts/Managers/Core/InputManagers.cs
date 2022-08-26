
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class InputManagers
{
    float _pressedTime;
    Boolean _pressed = false;

    //대리자 -> 이벤트 알리미
    public Action keyAction = null;
    public Action<Define.MouseMode> MouseAction = null;
    public void OnUpdate()
    {
        //ui 버튼을 누를때
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        //키보드 눌리면 
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
                    //몬스터 lockTarget설정
                    MouseAction.Invoke(Define.MouseMode.PointerDown);
                    _pressedTime = Time.time;
                }
                //목적지로 이동
                MouseAction.Invoke(Define.MouseMode.Pressed);
                _pressed = true;
            }
            else
            {     

                //경과시간이 짧으면 Click도 발생시킴
                if (_pressed)
                {
                    //?
                    if(Time.time < _pressedTime + 0.2f)
                        MouseAction.Invoke(Define.MouseMode.Clicked);
                    //loctTarget 해제
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
