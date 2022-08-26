using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup : UI_Base
{

    public override void Init()
    {
        Managers.UI.setCanvas(gameObject, true);
    }

    public void ClosePopupUI()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
