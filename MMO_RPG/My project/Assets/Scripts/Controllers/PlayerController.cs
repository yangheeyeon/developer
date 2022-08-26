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
    //Cursor초기화
    
    Texture2D _attack;
    Texture2D _hand;
    
    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Player;

        _stat = gameObject.GetComponent<PlayerStat>();

        //마우스 입력이 있을때 
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
            //몬스터가 사정거리 보다 가까이 있으면 공격모드로 전환
            float distance = (_destPos - transform.position).magnitude;
            if (distance <= 1)
            {
                State = Define.State.Skill;
                return;
            }
        }
       

        //방향벡터 업뎃
        Vector3 dir = _destPos - transform.position;
       
        //이동할 거리가 너무 작을 때
        if (dir.magnitude < 0.1f)
        {
            State = Define.State.Idle;
        }
        else
        {
            
            
            

            //Block Layer과 충돌하면 멈추기
            Debug.DrawRay(transform.position + 0.5f * new Vector3(0, 1, 0), dir.normalized, Color.green);


            if( Physics.Raycast(transform.position,dir.normalized,1.0f, LayerMask.GetMask("Block")))
            {
                if (Input.GetMouseButton(0) == false)
                    State = Define.State.Idle;
                return;

            }
            //이동
            float moveDistance = Mathf.Clamp(Time.deltaTime * _stat.MoveSpeed, 0, dir.magnitude);
            transform.position += dir.normalized * moveDistance;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 20.0f);
        }

        #region
        //애니메이션 블랜딩
        //wait_run_ratio = Mathf.Lerp(wait_run_ratio, 1.0f, Time.deltaTime * 10.0f);
        //anim.SetFloat("wait_run_ratio", wait_run_ratio);
        //anim.Play("WAIT_RUN");

        #endregion
    }
    protected override void UpdateSkill()
    {
        //공격대상을 바라보도록
        if (lockTarget != null)
        {
            Vector3 dir = lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);//dir을 바라볼때의 벡터라고 생각
            transform.rotation = Quaternion.Slerp(transform.rotation, quat, 20 * Time.deltaTime);
        }

    }
   
    //event receiver
    public void OnHitEvent()
    {
        //공격시 몬스터 hp 감소
        if(lockTarget!=null)
        {
            Stat targetStat = lockTarget.GetComponent<Stat>();
            targetStat.OnAttacked(_stat);

        }

        //계속 공격할건지 결정
        if (_stopSkill)
        {
            
            State = Define.State.Idle;
        }
        else
        {
            State = Define.State.Skill;
        }
    }
    

  

    
    //왼쪽 마우스 버튼 이벤트 발생하면 호출!
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
                    //pointerUp이벤트 발생시에 공격중이었다면
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
            case Define.MouseMode.Pressed://필요성 -> 코드
                
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
            // 두 벡터 사이 각도만큼 회전
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
            // 바라보는 방향으로 회전
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right), 0.2f);
            transform.position += Vector3.right * Time.deltaTime * _stat.MoveSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            // 바라보는 방향으로 회전
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.left), 0.2f);
            transform.position += Vector3.left * Time.deltaTime * _stat.MoveSpeed;
        }

        //_move = false;
    }
}



