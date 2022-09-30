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
        //�ϴ� ��� ����
        foreach(Transform child in gridPanel.transform)
        {
            Managers.Resource.Destroy(child.gameObject);
        }
        //8�� InvenItem , ���� �κ��丮 ���� ����ؼ�
        for(int i = 0; i< 8; i++)
        {
            GameObject inven = Managers.UI.MakeSubItem<UI_Inven_Item>(gridPanel.transform).gameObject;
            UI_Inven_Item invenItem = inven.GetOrAddComponent<UI_Inven_Item>();
            invenItem.SetInfo($"�����{i}��");
        }
    }

   
}
