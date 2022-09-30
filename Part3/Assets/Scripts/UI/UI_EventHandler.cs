using System;
using UnityEngine;
using UnityEngine.EventSystems;
//UI ���� ������Ʈ���� �������̽� ����(���)���� �̺�Ʈ �˸�
public class UI_EventHandler : MonoBehaviour,IPointerClickHandler,IDragHandler
{
    //�̹����� drag �Ǹ� Action ȣ��
    //�̹����� click�Ǹ� Action ȣ��
    
    public Action<PointerEventData> OnDragHandler = null;
    public Action<PointerEventData> OnClickHandler = null;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick");
        //�ݹ� �Լ�
        if (OnClickHandler != null)
            OnClickHandler.Invoke(eventData);
        else
            Debug.Log("null");
        
    }
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        //�ݹ� �Լ�
        if (OnDragHandler != null)
            OnDragHandler.Invoke(eventData);
    }

   
}
