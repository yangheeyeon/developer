using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : BaseController
{
    bool _stopSkill = false;
    PlayerStat _stat;
   
  
    public enum CursorType
    {
        None,
        Attack,
        Hand,
        Loot,
    }
    //Cursor�ʱ�ȭ
    
    Texture2D _attack;
    Texture2D _hand;
    
    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Player;

        _stat = gameObject.GetComponent<PlayerStat>();

        //���콺 �Է��� ������ 
        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;
        if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);
    }

    protected override void UpdateDie() {

    }
    protected override void UpdateIdle() 
    {
    }
    protected override void UpdateMoving()
    {
        
        if (lockTarget != null)
        {
            _destPos = lockTarget.transform.position;
            //���Ͱ� �����Ÿ� ���� ������ ������ ���ݸ��� ��ȯ
            float distance = (_destPos - transform.position).magnitude;
            if (distance <= 1)
            {
                State = Define.State.Skill;
                return;
            }
        }
       

        //���⺤�� ����
        Vector3 dir = _destPos - transform.position;
       
        //�̵��� �Ÿ��� �ʹ� ���� ��
        if (dir.magnitude < 0.1f)
        {
            State = Define.State.Idle;
        }
        else
        {
            
            
            

            //Block Layer�� �浹�ϸ� ���߱�
            Debug.DrawRay(transform.position + 0.5f * new Vector3(0, 1, 0), dir.normalized, Color.green);


            if( Physics.Raycast(transform.position,dir.normalized,1.0f, LayerMask.GetMask("Block")))
            {
                if (Input.GetMouseButton(0) == false)
                    State = Define.State.Idle;
                return;

            }
            //�̵�
            float moveDistance = Mathf.Clamp(Time.deltaTime * _stat.MoveSpeed, 0, dir.magnitude);
            transform.position += dir.normalized * moveDistance;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 20.0f);
        }

        #region
        //�ִϸ��̼� ����
        //wait_run_ratio = Mathf.Lerp(wait_run_ratio, 1.0f, Time.deltaTime * 10.0f);
        //anim.SetFloat("wait_run_ratio", wait_run_ratio);
        //anim.Play("WAIT_RUN");

        #endregion
    }
    protected override void UpdateSkill()
    {
        //���ݴ���� �ٶ󺸵���
        if (lockTarget != null)
        {
            Vector3 dir = lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);//dir�� �ٶ󺼶��� ���Ͷ�� ����
            transform.rotation = Quaternion.Slerp(transform.rotation, quat, 20 * Time.deltaTime);
        }

    }
   
    //event receiver
    public void OnHitEvent()
    {
        //���ݽ� ���� hp ����
        if(lockTarget!=null)
        {
            Stat targetStat = lockTarget.GetComponent<Stat>();
            targetStat.OnAttacked(_stat);

        }

        //��� �����Ұ��� ����
        if (_stopSkill)
        {
            
            State = Define.State.Idle;
        }
        else
        {
            State = Define.State.Skill;
        }
    }
    

  

    
    //���� ���콺 ��ư �̺�Ʈ �߻��ϸ� ȣ��!
    void OnMouseEvent(Define.MouseMode evt)
    {
      
        switch (State)
        {
            
            case Define.State.Idle:
                OnMouseEvent_IdleRun(evt);
                break;
            case Define.State.Moving:
                OnMouseEvent_IdleRun(evt);
                break;
            case Define.State.Skill:
                {
                    //pointerUp�̺�Ʈ �߻��ÿ� �������̾��ٸ�
                    if (evt == Define.MouseMode.PointerUp)
                        _stopSkill = true;
                }
                break;
        }

    }

    void OnMouseEvent_IdleRun(Define.MouseMode evt)
    {
        if (State == Define.State.Die)
            return;
        #region 
        //from camera to screen point
        /* Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
         Vector3 dir = mousePos - Camera.main.transform.position;
         dir = dir.normalized;
         Debug.DrawRay(Camera.main.transform.position, dir * 100, Color.red);*/
        #endregion

        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(Camera.main.transform.position, ray.direction*100.0f, Color.red, 2.0f);
        RaycastHit hit;
        int mask = 1 << (int)Define.Layer.Ground | 1 << (int)Define.Layer.Monster;
        bool raycastHit = Physics.Raycast(ray, out hit, 100.0f, mask);

        switch (evt)
        {
            case Define.MouseMode.PointerDown:
                if (raycastHit)
                {
                    _destPos = hit.point;
                    State = Define.State.Moving;
                    _stopSkill = false;
                    if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
                        lockTarget = hit.collider.gameObject;
                    else
                        lockTarget = null;

                }
                break;
            case Define.MouseMode.Pressed://�ʿ伺 -> �ڵ�
                
                if (lockTarget == null && raycastHit)
                    _destPos = hit.point;
                break;
            case Define.MouseMode.PointerUp:
                _stopSkill = true;
                break;
            

        }
    }
    void OnKeyBoard()
    {
        if (Input.GetKey(KeyCode.W))
        {
            // �� ���� ���� ������ŭ ȸ��
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), 0.2f);
            transform.position += Vector3.forward * Time.deltaTime * _stat.MoveSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.back), 0.2f);
            transform.position += Vector3.back * Time.deltaTime * _stat.MoveSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            // �ٶ󺸴� �������� ȸ��
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right), 0.2f);
            transform.position += Vector3.right * Time.deltaTime * _stat.MoveSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            // �ٶ󺸴� �������� ȸ��
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.left), 0.2f);
            transform.position += Vector3.left * Time.deltaTime * _stat.MoveSpeed;
        }

        //_move = false;
    }
}



