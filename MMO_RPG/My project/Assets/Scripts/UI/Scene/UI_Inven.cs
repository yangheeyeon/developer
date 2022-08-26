using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inven : UI_Scene
{
    enum GridPanel
    {
        GridPanel,
    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GridPanel));
        GameObject gridPanel = Get<GameObject>((int)GridPanel.GridPanel);
        //일단 모두 삭제
        foreach(Transform child in gridPanel.transform)
        {
            Managers.Resource.Destroy(child.gameObject);
        }
        //8개 InvenItem , 실제 인벤토리 정보 사용해서
        for(int i = 0; i< 8; i++)
        {
            GameObject inven = Managers.UI.MakeSubItem<UI_Inven_Item>(gridPanel.transform).gameObject;
            UI_Inven_Item invenItem = inven.GetOrAddComponent<UI_Inven_Item>();
            invenItem.SetInfo($"집행검{i}번");
        }
    }

   
}
