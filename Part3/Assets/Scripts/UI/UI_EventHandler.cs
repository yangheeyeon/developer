using System;
using UnityEngine;
using UnityEngine.EventSystems;
//UI 게임 오브젝트들은 인터페이스 구현(상속)으로 이벤트 알림
public class UI_EventHandler : MonoBehaviour,IPointerClickHandler,IDragHandler
{
    //이미지가 drag 되면 Action 호출
    //이미지가 click되면 Action 호출
    
    public Action<PointerEventData> OnDragHandler = null;
    public Action<PointerEventData> OnClickHandler = null;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick");
        //콜백 함수
        if (OnClickHandler != null)
            OnClickHandler.Invoke(eventData);
        else
            Debug.Log("null");
        
    }
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        //콜백 함수
        if (OnDragHandler != null)
            OnDragHandler.Invoke(eventData);
    }

   
}
