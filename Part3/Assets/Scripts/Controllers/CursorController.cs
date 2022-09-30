using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    Texture2D _attack;
    Texture2D _hand;

    public enum CursorType
    {
        None,
        Attack,
        Hand,
        Loot,
    }

    //Cursor�ʱ�ȭ
    CursorType cursorType = CursorType.None;
    
    void Start()
    {
        _attack = Managers.Resource.Load<Texture2D>("Textures/Cursors/Attack");
        _hand = Managers.Resource.Load<Texture2D>("Textures/Cursors/Hand");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
            return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        int mask = 1 << (int)Define.Layer.Ground | 1 << (int)Define.Layer.Monster;
        if (Physics.Raycast(ray, out hit, 100.0f, mask))
        {
            //Ground�� �浹�ϸ� �ڵ� Ŀ���� Ÿ���� �ٲ𶧸� ����
            if (hit.collider.gameObject.layer == (int)Define.Layer.Ground)
            {
                if (cursorType != CursorType.Hand)
                {
                    Cursor.SetCursor(_hand, new Vector2(_hand.width / 5, 0), CursorMode.Auto);
                    cursorType = CursorType.Hand;
                }

            }

            //Monster�� �浹�ϸ� ���� Ŀ������ Ÿ���� �ٲ𶧸� ����
            else
            {
                if (cursorType != CursorType.Attack)
                {
                    Cursor.SetCursor(_attack, new Vector2(_hand.width / 5, 0), CursorMode.Auto);
                    cursorType = CursorType.Attack;
                }

            }


        }
    }
}
