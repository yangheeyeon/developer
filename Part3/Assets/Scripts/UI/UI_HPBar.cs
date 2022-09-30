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
        //Enum.GetNames(typeof(GameObjects))���� ���� ������Ʈ�� ã�� binding
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

        //unityChan �θ�� �ѱ��
        Transform parent = transform.parent;
        //�׻� unityChan�Ӹ� ���� ��ġ�ϰ� ī�޶�� ���� ������ ���ϵ���
        transform.position = parent.position + Vector3.up * (parent.GetComponent<Collider>().bounds.size.y);
        transform.rotation = Camera.main.transform.rotation;
        //hp����
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
