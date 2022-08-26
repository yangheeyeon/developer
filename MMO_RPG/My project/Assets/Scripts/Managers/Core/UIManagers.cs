using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManagers 
{
    //유니티에서 *게임object* 는 *껍데기*
    
    int _order;
    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    UI_Scene _sceneUI;
    public void setCanvas(GameObject go , bool sort )
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }
    public GameObject Root
    {
        get {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
            {
                root = new GameObject { name = "@UI_Root" };
            }
            return root;
        }
        
    }
    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;
        //프리팹 인스턴스 생성
        GameObject go = Managers.Resource.Instantiate($"UI/WorldSpace/{name}");

        if (parent != null)
            go.transform.SetParent(parent);

        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return Util.GetOrAddComponent<T>(go);

    }
    public T MakeSubItem<T>(Transform parent = null,string name = null) where T: UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;
        
        GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{name}");

        if (parent != null)
            go.transform.SetParent(parent);
        

        return Util.GetOrAddComponent<T>(go);

    }
    public T showSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        

        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
        go.transform.SetParent(Root.transform);

        T sceneUI = Util.GetOrAddComponent<T>(go);

        _sceneUI = sceneUI;

        return sceneUI;


    }
    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

       

        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
        go.transform.SetParent(Root.transform);

        T _popup = Util.GetOrAddComponent<T>(go);
        _popupStack.Push(_popup);
        
        return _popup;


    }
    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0)
            return;
        if(_popupStack.Peek() != popup)
        {
            return;
        }
        ClosePopupUI();
            
    }
    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return ;
        UI_Popup popup = _popupStack.Pop();
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;
        _order--;
        

    }
    public void CloseAllPopupUI()
    {
        while(_popupStack.Count > 0)
        {
            ClosePopupUI();
        }
    }
    public void Clear()
    {
        CloseAllPopupUI();
        _sceneUI = null;
    }
}
