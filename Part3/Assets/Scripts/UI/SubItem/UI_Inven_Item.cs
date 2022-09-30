using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inven_Item : UI_Base
{
    enum GameObjects
    {
        ItemIcon,
        ItemNameText
    }
    void Start()
    {
        Init();
    }
    string _name = "";
    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        Get<GameObject>((int)GameObjects.ItemNameText).GetComponent<Text>().text = _name ;
        Get<GameObject>((int)GameObjects.ItemIcon).AddUIEvent( (PointerEventData)=> { Debug.Log($"{_name}"); });
    }
  
    public void SetInfo(string name)
    {
        _name = name;
    }
  
}
