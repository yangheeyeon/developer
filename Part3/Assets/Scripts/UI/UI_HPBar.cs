using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HPBar : UI_Base
{
    enum GameObjects
    {
        HPBar,
    }
    public override void Init()
    {
        //Enum.GetNames(typeof(GameObjects))으로 게임 오브젝트들 찾아 binding
        Bind<GameObject>(typeof(GameObjects));



    }
    Stat _stat;
    public void Start()
    {
        Init();
        _stat = transform.parent.GetComponent<Stat>();

    }
    public void Update()
    {

        //unityChan 부모로 넘기기
        Transform parent = transform.parent;
        //항상 unityChan머리 위에 위치하고 카메라와 같은 방향을 향하도록
        transform.position = parent.position + Vector3.up * (parent.GetComponent<Collider>().bounds.size.y);
        transform.rotation = Camera.main.transform.rotation;
        //hp업뎃
        float ratio = _stat.Hp / (float)_stat.MaxHp;
        SetHpRatio(ratio);
    }

    public void SetHpRatio(float ratio)
    {
        //"UnityEngine.GameObject" : {HPBar}
        GameObject go = Get<GameObject>((int)GameObjects.HPBar);
        if (go == null)
            return;
        else
            go.GetComponent<Slider>().value = ratio;
    }
}
